//
// Library: KaosCollections
// File:    RankedMap.KeyCollection.cs
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
    public partial class RankedMap<TKey,TValue>
    {
        /// <summary>
        /// Represents a collection of keys of the <see cref="RankedMap{TKey,TValue}"/>.
        /// </summary>
        [DebuggerTypeProxy (typeof (ICollectionKeysDebugView<,>))]
        [DebuggerDisplay("Count = {Count}")]
        public sealed class KeyCollection :
              ICollection<TKey>
            , ICollection
#if ! NET35 && ! NET40
            , IReadOnlyCollection<TKey>
#endif
        {
            private readonly RankedMap<TKey,TValue> tree;

            #region Constructors

            /// <summary>Initializes a new collection that reflects the keys of a <see cref="RankedMap{TKey,TValue}"/>.</summary>
            /// <param name="map">Map containing these keys.</param>
            /// <remarks>This is a O(1) operation.</remarks>
            /// <exception cref="ArgumentNullException">When <em>map</em> is <b>null</b>.</exception>
            public KeyCollection (RankedMap<TKey,TValue> map)
            {
                if (map == null)
#pragma warning disable IDE0016
                    throw new ArgumentNullException (nameof (map));
#pragma warning restore IDE0016

                this.tree = map;
            }

            #endregion

            #region Properties

            /// <summary>Gets the key at the supplied index.</summary>
            /// <param name="index">The zero-based index of the key to get.</param>
            /// <returns>The key at <em>index</em>.</returns>
            /// <remarks>This is a O(log <em>n</em>) operation.</remarks>
            /// <exception cref="ArgumentOutOfRangeException">When <em>index</em> is less than zero or not less than the number of items.</exception>
            public TKey this[int index]
            { get { return ElementAt (index); } }

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
            void ICollection<TKey>.Add (TKey key) => throw new NotSupportedException();


            /// <summary>This implementation always throws a <see cref="NotSupportedException" />.</summary>
            void ICollection<TKey>.Clear() => throw new NotSupportedException();


            /// <summary>Determines whether the key collection contains the supplied key.</summary>
            /// <param name="key">The key to locate.</param>
            /// <returns><b>true</b> if <em>key</em> is contained in the map; otherwise <b>false</b>.</returns>
            /// <exception cref="ArgumentNullException">When <em>key</em> is <b>null</b>.</exception>
            public bool Contains (TKey key)
            {
                if (key == null)
                    throw new ArgumentNullException (nameof (key));

                var leaf = (PairLeaf<TValue>) tree.Find (key, out int index);
                return index >= 0;
            }

            /// <summary>Determines whether the collection contains the supplied key.</summary>
            /// <param name="key">The key to locate.</param>
            /// <returns><b>true</b> if <em>key</em> is contained in the collection; otherwise <b>false</b>.</returns>
            /// <remarks>This is a O(log <em>n</em>) operation.</remarks>
            bool ICollection<TKey>.Contains (TKey key) => tree.ContainsKey (key);


            /// <summary>Copies keys to a supplied array, starting at the supplied position.</summary>
            /// <param name="array">A one-dimensional array that is the destination of the copy.</param>
            /// <param name="index">The zero-based starting position in <em>array</em>.</param>
            /// <exception cref="ArgumentNullException">When <em>array</em> is <b>null</b>.</exception>
            /// <exception cref="ArgumentOutOfRangeException">When <em>index</em> is less than zero.</exception>
            /// <exception cref="ArgumentException">When not enough space is given for the copy.</exception>
            public void CopyTo (TKey[] array, int index)
                => tree.CopyKeysTo1 (array, index, tree.Count);

            /// <summary>Copies keys to a supplied array, starting at the supplied position.</summary>
            /// <param name="array">A one-dimensional array that is the destination of the copy.</param>
            /// <param name="index">The zero-based starting position in <em>array</em>.</param>
            /// <exception cref="ArgumentNullException">When <em>array</em> is <b>null</b>.</exception>
            /// <exception cref="ArgumentOutOfRangeException">When <em>index</em> is less than zero.</exception>
            /// <exception cref="ArgumentException">When not enough space is given for the copy.</exception>
            void ICollection.CopyTo (Array array, int index)
                => tree.CopyKeysTo2 (array, index);


            /// <summary>Returns the number of elements with the supplied key in the map.</summary>
            /// <param name="key">The key to return the number of occurrences for.</param>
            /// <returns>The number of elements with the supplied key.</returns>
            /// <remarks>
            /// <para>
            /// This is a O(log <em>n</em>) operation
            /// where <em>n</em> is <see cref="Count"/>.
            /// </para>
            /// </remarks>
            public int GetCount (TKey key)
            {
                int treeIx1 = tree.FindEdgeForIndex (key, out Leaf leaf1, out int leafIx1, leftEdge: true);
                if (treeIx1 < 0)
                    return 0;
                else
                    return tree.FindEdgeForIndex (key, out Leaf leaf2, out int leafIx2, leftEdge: false) - treeIx1;
            }

            /// <summary>Gets the key at the supplied index.</summary>
            /// <param name="index">The zero-based index of the element to get.</param>
            /// <returns>The key at <em>index</em>.</returns>
            /// <remarks>This is a O(log <em>n</em>) operation.</remarks>
            /// <exception cref="ArgumentOutOfRangeException">When <em>index</em> is less than zero or not less than the number of items.</exception>
            public TKey ElementAt (int index)
            {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException (nameof (index), "out of range");

                var leaf = (PairLeaf<TValue>) tree.Find (index, out int leafIndex);
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

                var leaf = (PairLeaf<TValue>) tree.Find (index, out int leafIndex);
                return leaf.GetKey (leafIndex);
            }


            /// <summary>Gets the index of the first occurrence of supplied key.</summary>
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
            public int IndexOf (TKey key) => tree.FindEdgeForIndex (key, out Leaf leaf, out int leafIndex, leftEdge:true);


            /// <summary>Gets the maximum key per the comparer.</summary>
            /// <returns>The maximum key per the comparer.</returns>
            /// <remarks>This is a O(1) operation.</remarks>
            public TKey Max()
            {
                if (Count == 0)
                    throw new InvalidOperationException ("Sequence contains no elements");

                return tree.rightmostLeaf.GetKey (tree.rightmostLeaf.KeyCount - 1);
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


            /// <summary>This implementation always throws a <see cref="NotSupportedException" />.</summary>
            /// <param name="key">The key to remove.</param>
            /// <returns><b>true</b> if the object was removed; otherwise <b>false</b>.</returns>
            bool ICollection<TKey>.Remove (TKey key) => throw new NotSupportedException();

            #endregion

            #region Enumeration

            /// <summary>Returns an enumerator that iterates thru the distinct keys of the map.</summary>
            /// <returns>An enumerator that iterates thru distinct keys.</returns>
            /// <remarks>
            /// <para>
            /// Retrieving each key is a O(log <em>n</em>) operation
            /// where <em>n</em> is <see cref="Count"/>.
            /// </para>
            /// </remarks>
            public IEnumerable<TKey> Distinct()
            {
                if (Count == 0)
                    yield break;

                int stageFreeze = tree.stage;
                int ix = 0;
                Leaf leaf = tree.leftmostLeaf;
                for (TKey key = leaf.Key0;;)
                {
                    yield return key;
                    tree.StageCheck (stageFreeze);

                    if (++ix < leaf.KeyCount)
                    {
                        TKey nextKey = leaf.GetKey (ix);
                        if (tree.Comparer.Compare (key, nextKey) != 0)
                        { key = nextKey; continue; }
                    }

                    tree.FindEdgeRight (key, out leaf, out ix);
                    if (ix >= leaf.KeyCount)
                    {
                        leaf = leaf.rightLeaf;
                        if (leaf == null)
                            yield break;
                        ix = 0;
                    }
                    key = leaf.GetKey (ix);
                }
            }


            /// <summary>Gets an enumerator that iterates thru the collection.</summary>
            /// <returns>An enumerator for the collection.</returns>
            public Enumerator GetEnumerator() => new Enumerator (tree);

            /// <summary>Gets an enumerator that iterates thru the collection.</summary>
            /// <returns>An enumerator for the collection.</returns>
            IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator() => new Enumerator (tree);

            /// <summary>Gets an enumerator that iterates thru the collection.</summary>
            /// <returns>An enumerator for the collection.</returns>
            IEnumerator IEnumerable.GetEnumerator() => new Enumerator (tree);


            /// <summary>Enumerates the sorted keys of a <see cref="RankedMap{TKey,TValue}.KeyCollection"/>.</summary>
            public sealed class Enumerator : IEnumerator<TKey>
            {
                private readonly RankedMap<TKey,TValue> tree;
                private PairLeaf<TValue> leaf;
                private int index;
                private int stageFreeze;

                internal Enumerator (RankedMap<TKey,TValue> map)
                {
                    this.tree = map;
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
                /// <exception cref="InvalidOperationException">When the map was modified after the enumerator was created.</exception>
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
                /// <exception cref="InvalidOperationException">When the map was modified after the enumerator was created.</exception>
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
