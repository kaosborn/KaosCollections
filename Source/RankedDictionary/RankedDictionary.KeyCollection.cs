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
        /// Represents a collection of keys of a <see cref="RankedDictionary{TKey,TValue}"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This class emulates and extends
        /// <see cref="SortedDictionary{TKey,TValue}.KeyCollection"/>
        /// while improving performance of operations on large collections.
        /// Enhancements include indexer property for array semantics and these methods:
        /// </para>
        /// <list type="bullet">
        /// <item><see cref="ElementAt"/></item>
        /// <item><see cref="ElementAtOrDefault"/></item>
        /// <item><see cref="IndexOf"/></item>
        /// </list>
        /// </remarks>
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


            /// <summary>Determines whether the dictionary contains the supplied key.</summary>
            /// <param name="key">The key to locate.</param>
            /// <returns><b>true</b> if <em>key</em> is contained in the dictionary; otherwise <b>false</b>.</returns>
            /// <exception cref="ArgumentNullException">When <em>key</em> is <b>null</b>.</exception>
            public bool Contains (TKey key)
                => tree.ContainsKey (key);


            /// <summary>Determines whether the collection contains the supplied key.</summary>
            /// <param name="key">The key to locate.</param>
            /// <returns><b>true</b> if <em>key</em> is contained in the collection; otherwise <b>false</b>.</returns>
            /// <remarks>This is a O(log <em>n</em>) operation.</remarks>
            bool ICollection<TKey>.Contains (TKey key)
                => tree.ContainsKey (key);


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
            /// <returns>The key at <em>index</em>.</returns>
            /// <remarks>This is a O(log <em>n</em>) operation.</remarks>
            /// <exception cref="ArgumentOutOfRangeException">When <em>index</em> is less than zero or not less than the number of items.</exception>
            public TKey this[int index]
            { get { return ElementAt (index); } }


            /// <summary>Gets the key at the supplied index.</summary>
            /// <param name="index">The zero-based index of the key to get.</param>
            /// <returns>The key at <em>index</em>.</returns>
            /// <remarks>This is a O(log <em>n</em>) operation.</remarks>
            /// <exception cref="ArgumentOutOfRangeException">When <em>index</em> is less than zero or greater than or equal to the number of keys.</exception>
            public TKey ElementAt (int index)
            {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException (nameof (index), "Argument is out of the range of valid values.");

                var leaf = tree.Find (index, out int leafIndex);
                return leaf.GetKey (leafIndex);
            }


            /// <summary>Gets the key at the supplied index or the default if the index is out of range.</summary>
            /// <param name="index">The zero-based index of the key to get.</param>
            /// <returns>The key at <em>index</em>.</returns>
            /// <remarks>This is a O(log <em>n</em>) operation.</remarks>
            public TKey ElementAtOrDefault (int index)
            {
                if (index < 0 || index >= Count)
                    return default (TKey);

                var leaf = tree.Find (index, out int leafIndex);
                return leaf.GetKey (leafIndex);
            }


            /// <summary>Gets the index of the supplied key.</summary>
            /// <param name="key">The key to find.</param>
            /// <returns>The index of <em>key</em> if found; otherwise a negative value holding the bitwise complement of the insert point.</returns>
            /// <remarks>
            /// <para>
            /// If <em>key</em> is not found, apply the bitwise complement operator
            /// (<see href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/bitwise-complement-operator">~</see>)
            /// to the result to get the index of the next higher key.
            /// </para>
            /// <para>
            /// This is a O(log <em>n</em>) operation.
            /// </para>
            /// </remarks>
            public int IndexOf (TKey key)
                => tree.FindEdgeForIndex (key, out Leaf leaf, out int leafIndex, leftEdge:true);


            /// <summary>Gets the minimum key in the dictionary per the comparer.</summary>
            /// <returns>The minimum key in the dictionary.</returns>
            /// <remarks>This is a O(1) operation.</remarks>
            /// <exception cref="InvalidOperationException">When <see cref="Count"/> is zero.</exception>
            public TKey First()
            {
                if (Count == 0)
                    throw new InvalidOperationException ("Sequence contains no elements");

                return tree.leftmostLeaf.Key0;
            }


            /// <summary>Gets the maximum key in the dictionary per the comparer.</summary>
            /// <returns>The maximum key in the dictionary.</returns>
            /// <remarks>This is a O(1) operation.</remarks>
            /// <exception cref="InvalidOperationException">When <see cref="Count"/> is zero.</exception>
            public TKey Last()
            {
                if (Count == 0)
                    throw new InvalidOperationException ("Sequence contains no elements");

                return tree.rightmostLeaf.GetKey (tree.rightmostLeaf.KeyCount - 1);
            }


            /// <summary>Gets the maximum key per the comparer.</summary>
            /// <returns>The maximum key per the comparer.</returns>
            /// <remarks>This is a O(1) operation.</remarks>
            public TKey Max()
            {
                if (Count == 0)
                    throw new InvalidOperationException ("Sequence contains no elements");

                return tree.rightmostLeaf.GetKey (tree.rightmostLeaf.KeyCount-1);
            }


            /// <summary>Gets the minimum key per the comparer.</summary>
            /// <returns>The minimum key per the comparer.</returns>
            /// <remarks>This is a O(1) operation.</remarks>
            public TKey Min()
            {
                if (Count == 0)
                    throw new InvalidOperationException ("Sequence contains no elements");

                return tree.leftmostLeaf.Key0;
            }


            /// <summary>Returns an enumerator that iterates thru the dictionary keys in reverse order.</summary>
            /// <returns>An enumerator that reverse iterates thru the dictionary keys.</returns>
            public IEnumerable<TKey> Reverse()
            {
                var stageFreeze = tree.stage;
                for (var leaf = tree.rightmostLeaf;;)
                {
                    for (int ix = leaf.KeyCount; --ix >= 0; )
                    {
                        yield return leaf.GetKey (ix);
                        tree.StageCheck (stageFreeze);
                    }
                    leaf = leaf.leftLeaf;
                    if (leaf == null)
                        yield break;
                }
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


            /// <summary>Enumerates the items of a <see cref="RankedDictionary{TKey,TValue}.KeyCollection"/> in sort order.</summary>
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
