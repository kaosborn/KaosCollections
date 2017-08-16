//
// Library: KaosCollections
// File:    RankedDictionary.Keys.cs
// Purpose: Define Keys nested class.
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

            /// <summary>Makes a new collection that holds the keys of a <see cref="RankedDictionary{TKey,TValue}"/>.</summary>
            /// <param name="dictionary"><see cref="RankedDictionary{TKey,TValue}"/> containing these keys.</param>
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

            #endregion

            #region Methods

            /// <summary>Copies keys to a supplied array starting as position <em>index</em> in the target.</summary>
            /// <param name="array">Destination of copy.</param>
            /// <param name="index">Starting position in <em>array</em> for copy operation.</param>
            /// <exception cref="ArgumentNullException">When <em>array</em> is <b>null</b>.</exception>
            /// <exception cref="ArgumentOutOfRangeException">When <em>index</em> is less than zero.</exception>
            /// <exception cref="ArgumentException">When not enough space is given for the copy.</exception>
            public void CopyTo (TKey[] array, int index)
            {
                if (array == null)
                    throw new ArgumentNullException (nameof (array));

                if (index < 0)
                    throw new ArgumentOutOfRangeException (nameof (index), index, "Argument was out of the range of valid values.");

                if (Count > array.Length - index)
                    throw new ArgumentException ("Destination array is not long enough to copy all the items in the collection. Check array index and length.", nameof (array));

                for (var leaf = (PairLeaf) tree.leftmostLeaf; leaf != null; leaf = (PairLeaf) leaf.rightLeaf)
                    for (int leafIndex = 0; leafIndex < leaf.KeyCount; ++leafIndex)
                        array[index++] = leaf.GetKey (leafIndex);
            }


            /// <summary>Returns an enumerator that iterates thru the KeyCollection.</summary>
            /// <returns>An enumerator for the collection.</returns>
            public IEnumerator<TKey> GetEnumerator()
            { return new Enumerator (tree); }

            #endregion

            #region Explicit properties and methods interface implementations

            bool ICollection<TKey>.IsReadOnly => true;

            bool ICollection.IsSynchronized => false;

            object ICollection.SyncRoot => tree.GetSyncRoot();


            void ICollection<TKey>.Add (TKey key)
            { throw new NotSupportedException(); }

            void ICollection<TKey>.Clear()
            { throw new NotSupportedException(); }

            bool ICollection<TKey>.Contains (TKey key)
            { return tree.ContainsKey (key); }

            void ICollection.CopyTo (Array array, int index)
            {
                if (array == null)
                    throw new ArgumentNullException (nameof (array));

                if (array.Rank > 1)
                    throw new ArgumentException ("Multidimension array is not supported on this operation.", nameof (array));

                if (index < 0)
                    throw new ArgumentOutOfRangeException (nameof (index), "Index is less than zero.");

                if (Count > array.Length - index)
                    throw new ArgumentException ("Destination array is not long enough to copy all the items in the collection. Check array index and length.", nameof (array));

                for (var leaf = (PairLeaf) tree.leftmostLeaf; leaf != null; leaf = (PairLeaf) leaf.rightLeaf)
                    for (int leafIndex = 0; leafIndex < leaf.KeyCount; ++leafIndex)
                    {
                        array.SetValue (leaf.GetKey (leafIndex), index);
                        ++index;
                    }
            }

            /// <summary>Gets an enumerator that iterates thru the collection.</summary>
            /// <returns>An enumerator for the collection.</returns>
            IEnumerator IEnumerable.GetEnumerator()
            { return GetEnumerator(); }

            bool ICollection<TKey>.Remove (TKey key)
            { throw new NotSupportedException(); }

            #endregion

            #region Enumerator

            /// <summary>Enumerates the sorted keys of a <see cref="RankedDictionary{TKey,TValue}.KeyCollection"/>.</summary>
            public sealed class Enumerator : IEnumerator<TKey>
            {
                private readonly RankedDictionary<TKey,TValue> tree;
                private PairLeaf leaf;
                private int index;
                private int stageFreeze;

                internal Enumerator (RankedDictionary<TKey,TValue> dictionary)
                {
                    this.tree = dictionary;
                    ((IEnumerator) this).Reset();
                }

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

                        leaf = (PairLeaf) leaf.rightLeaf;
                        if (leaf != null)
                        { index = 0; return true; }

                        index = -1;
                    }

                    return false;
                }

                void IEnumerator.Reset()
                {
                    stageFreeze = tree.stage;
                    index = -1;
                    leaf = (PairLeaf) tree.leftmostLeaf;
                }

                /// <summary>Releases all resources used by the Enumerator.</summary>
                public void Dispose() { }
            }

            #endregion
        }
    }
}
