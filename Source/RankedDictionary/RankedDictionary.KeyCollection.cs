//
// Library: KaosCollections
// File:    RankedDictionary.KeyCollection.cs
// Purpose: Define KeyCollection nested class.
//
// Copyright © 2009-2017 Kasey Osborn (github.com/kaosborn)
// MIT License - Use and redistribute freely
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Kaos.Collections
{
    public partial class RankedDictionary<TKey,TValue>
    {
        /// <summary>
        /// Represents the collection of keys of a <see cref="RankedDictionary{TKey,TValue}"/>.
        /// </summary>
        [DebuggerTypeProxy (typeof (ICollectionKeysDebugView<,>))]
        [DebuggerDisplay ("Count = {Count}")]
        public sealed class KeyCollection :
            ICollection<TKey>,
            ICollection
#if ! NET35 && ! NET40
            , IReadOnlyCollection<TKey>
#endif
        {
            private readonly RankedDictionary<TKey,TValue> tree;

            #region Constructors

            /// <summary>Initializes a new collection that reflects the keys of a <see cref="RankedDictionary{TKey,TValue}"/>.</summary>
            /// <param name="dictionary">Dictionary containing these keys.</param>
            /// <remarks>This is a O(1) operation.</remarks>
            /// <exception cref="ArgumentNullException">When <em>dictionary</em> is <b>null</b>.</exception>
            public KeyCollection (RankedDictionary<TKey,TValue> dictionary)
            {
                if (dictionary == null)
#pragma warning disable IDE0016
                    throw new ArgumentNullException (nameof (dictionary));
#pragma warning restore IDE0016

                this.tree = dictionary;
            }

            #endregion

            #region Properties

            /// <summary>Gets the number of keys in the collection.</summary>
            public int Count => tree.Count;

            /// <summary>Indicates that the collection is read-only.</summary>
            bool ICollection<TKey>.IsReadOnly => true;

            /// <summary>Indicates that the collection is not thread safe.</summary>
            bool ICollection.IsSynchronized => false;

            /// <summary>Gets an object that can be used to synchronize access to the collection.</summary>
            object ICollection.SyncRoot => tree.GetSyncRoot();

            #endregion

            #region Methods

            /// <summary>This implementation always throws a <see cref="NotSupportedException" />.</summary>
            /// <param name="key">The object to add.</param>
            void ICollection<TKey>.Add (TKey key)
            { throw new NotSupportedException(); }


            /// <summary>This implementation always throws a <see cref="NotSupportedException" />.</summary>
            void ICollection<TKey>.Clear()
            { throw new NotSupportedException(); }


            /// <summary>Determines whether the collection contains the supplied key.</summary>
            /// <param name="key">The key to locate in the collection.</param>
            /// <returns><b>true</b> if <em>key</em> is found in the collection; otherwise <b>false</b>.</returns>
            bool ICollection<TKey>.Contains (TKey key)
            { return tree.ContainsKey (key); }


            /// <summary>Copies keys to a supplied array, starting at the supplied position.</summary>
            /// <param name="array">A one-dimensional array that is the destination of the copy.</param>
            /// <param name="index">The zero-based starting position in <em>array</em>.</param>
            /// <exception cref="ArgumentNullException">When <em>array</em> is <b>null</b>.</exception>
            /// <exception cref="ArgumentOutOfRangeException">When <em>index</em> is less than zero.</exception>
            /// <exception cref="ArgumentException">When not enough space is given for the copy.</exception>
            public void CopyTo (TKey[] array, int index)
            { tree.CopyKeysTo1 (array, index, tree.Count); }

            /// <summary>Copies keys to a supplied array, starting at the supplied position.</summary>
            /// <param name="array">A one-dimensional array that is the destination of the copy.</param>
            /// <param name="index">The zero-based starting position in <em>array</em>.</param>
            /// <exception cref="ArgumentNullException">When <em>array</em> is <b>null</b>.</exception>
            /// <exception cref="ArgumentOutOfRangeException">When <em>index</em> is less than zero.</exception>
            /// <exception cref="ArgumentException">When not enough space is given for the copy.</exception>
            void ICollection.CopyTo (Array array, int index)
            { tree.CopyKeysTo2 (array, index); }


            /// <summary>This implementation always throws a <see cref="NotSupportedException" />.</summary>
            /// <param name="key">The key to remove.</param>
            /// <returns><b>true</b> if the object was removed; otherwise <b>false</b>.</returns>
            bool ICollection<TKey>.Remove (TKey key)
            { throw new NotSupportedException(); }

            #endregion

            #region Bonus methods

            /// <summary>Gets the key at the supplied index.</summary>
            /// <param name="index">The zero-based index of the key to get.</param>
            /// <returns>The key at the supplied index.</returns>
            /// <remarks>This is a O(log <em>n</em>) operation.</remarks>
            /// <exception cref="ArgumentOutOfRangeException">When <em>index</em> is less than zero or greater than or equal to the number of keys.</exception>
            public TKey ElementAt (int index)
            {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException (nameof (index), "Argument is out of the range of valid values.");

                var leaf = tree.Find (index, out int leafIndex);
                return leaf.GetKey (leafIndex);
            }


            /// <summary>Gets the key at the supplied index or the default if index is out of range.</summary>
            /// <param name="index">The zero-based index of the key to get.</param>
            /// <returns>The key at the supplied index.</returns>
            /// <remarks>This is a O(log <em>n</em>) operation.</remarks>
            public TKey ElementAtOrDefault (int index)
            {
                if (index < 0 || index >= Count)
                    return default (TKey);

                var leaf = tree.Find (index, out int leafIndex);
                return leaf.GetKey (leafIndex);
            }


            /// <summary>Gets the index of the supplied key.</summary>
            /// <param name="key">The key to seek.</param>
            /// <returns>The index of the supplied key if found; otherwise a negative value holding the bitwise complement of the insert point.</returns>
            /// <remarks>
            /// <para>
            /// If the item is not found, apply the bitwise complement operator
            /// (<see href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/bitwise-complement-operator">~</see>)
            /// to the result to get the index of the next higher element.
            /// </para>
            /// <para>
            /// This is a O(log <em>n</em>) operation.
            /// </para>
            /// </remarks>
            /// <exception cref="ArgumentNullException">When <em>key</em> is <b>null</b>.</exception>
            public int IndexOf (TKey key)
            {
                if (key == null)
                    throw new ArgumentNullException (nameof (key));

                return tree.FindEdgeForIndex (key, out Leaf leaf, out int leafIndex, leftEdge:true);
            }

            #endregion

            #region Enumeration

            /// <summary>Gets an enumerator that iterates thru the collection.</summary>
            /// <returns>An enumerator for the collection.</returns>
            public Enumerator GetEnumerator() => new Enumerator (tree);

            /// <summary>Gets an enumerator that iterates thru the collection.</summary>
            /// <returns>An enumerator for the collection.</returns>
            IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator() => new Enumerator (tree);

            /// <summary>Gets an enumerator that iterates thru the collection.</summary>
            /// <returns>An enumerator for the collection.</returns>
            IEnumerator IEnumerable.GetEnumerator() => new Enumerator (tree);


            /// <summary>Enumerates the sorted keys of a <see cref="RankedDictionary{TKey,TValue}.KeyCollection"/>.</summary>
            public sealed class Enumerator : IEnumerator<TKey>
            {
                private readonly RankedDictionary<TKey,TValue> tree;
                private PairLeaf<TValue> leaf;
                private int index;
                private int stageFreeze;

                internal Enumerator (RankedDictionary<TKey,TValue> dictionary)
                {
                    this.tree = dictionary;
                    ((IEnumerator) this).Reset();
                }

                /// <summary>Gets the key at the current position.</summary>
                object IEnumerator.Current
                {
                    get
                    {
                        tree.StageCheck (stageFreeze);
                        if (index < 0)
                            throw new InvalidOperationException();
                        return (object) Current;
                    }
                }

                /// <summary>Gets the key at the current position of the enumerator.</summary>
                /// <exception cref="InvalidOperationException">When the dictionary was modified after the enumerator was created.</exception>
                public TKey Current
                {
                    get
                    {
                        tree.StageCheck (stageFreeze);
                        return index < 0 ? default (TKey) : leaf.GetKey (index);
                    }
                }

                /// <summary>Advances the enumerator to the next key in the collection.</summary>
                /// <returns><b>true</b> if the enumerator was successfully advanced to the next key; <b>false</b> if the enumerator has passed the end of the collection.</returns>
                /// <exception cref="InvalidOperationException">When the dictionary was modified after the enumerator was created.</exception>
                    public bool MoveNext()
                {
                    tree.StageCheck (stageFreeze);

                    if (leaf != null)
                    {
                        if (++index < leaf.KeyCount)
                            return true;

                        leaf = (PairLeaf<TValue>) leaf.rightLeaf;
                        if (leaf != null)
                        { index = 0; return true; }

                        index = -1;
                    }

                    return false;
                }

                /// <summary>Rewinds the enumerator to its initial state.</summary>
                void IEnumerator.Reset()
                {
                    stageFreeze = tree.stage;
                    index = -1;
                    leaf = (PairLeaf<TValue>) tree.leftmostLeaf;
                }

                /// <summary>Releases all resources used by the enumerator.</summary>
                public void Dispose() { }
            }

            #endregion
        }
    }
}
