//
// Library: KaosCollections
// File:    RankedMap.KeyCollection.cs
// Purpose: Define KeyCollection nested class.
//
// Copyright © 2009-2019 Kasey Osborn (github.com/kaosborn)
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
        /// Represents a collection of keys of a <see cref="RankedMap{TKey,TValue}"/>.
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


            /// <summary>Determines whether the map contains the supplied key.</summary>
            /// <param name="key">The key to locate.</param>
            /// <returns><b>true</b> if <em>key</em> is contained in the map; otherwise <b>false</b>.</returns>
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
            /// <example>
            /// The below snippet is part of a larger example of the
            /// <see cref="RankedMap{TKey,TValue}"/>class.
            /// <code source="..\Bench\RmExample01\RmExample01.cs" lang="cs" region="GetCount" />
            /// </example>
            public int GetCount (TKey key) => tree.GetCount2 (key);


            /// <summary>Returns the number of distinct keys in the map.</summary>
            /// <returns>The number of distinct keys in the map.</returns>
            /// <remarks>
            /// This is a O(<em>m</em> log <em>n</em>) operation
            /// where <em>m</em> is the distinct key count
            /// and <em>n</em> is <see cref="Count"/>.
            /// </remarks>
            /// <example>
            /// The below snippet is part of a larger example of the
            /// <see cref="RankedMap{TKey,TValue}"/>class.
            /// <code source="..\Bench\RmExample01\RmExample01.cs" lang="cs" region="GetDistinctCount" />
            /// </example>
            public int GetDistinctCount() => tree.GetDistinctCount2();


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
                    return default;

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
            public int IndexOf (TKey key)
                => tree.FindEdgeForIndex (key, out Leaf _, out int _, leftEdge:true);


            /// <summary>Gets the minimum key in the map per the comparer.</summary>
            /// <returns>The minimum key in the map.</returns>
            /// <remarks>This is a O(1) operation.</remarks>
            /// <exception cref="InvalidOperationException">When <see cref="Count"/> is zero.</exception>
            public TKey First()
            {
                if (Count == 0)
                    throw new InvalidOperationException ("Sequence contains no elements.");

                return tree.leftmostLeaf.Key0;
            }


            /// <summary>Gets the maximum key in the map per the comparer.</summary>
            /// <returns>The maximum key in the map.</returns>
            /// <remarks>This is a O(1) operation.</remarks>
            /// <exception cref="InvalidOperationException">When <see cref="Count"/> is zero.</exception>
            public TKey Last()
            {
                if (Count == 0)
                    throw new InvalidOperationException ("Sequence contains no elements.");

                return tree.rightmostLeaf.GetKey (tree.rightmostLeaf.KeyCount - 1);
            }


            /// <summary>This implementation always throws a <see cref="NotSupportedException" />.</summary>
            /// <param name="key">The key to remove.</param>
            /// <returns><b>true</b> if the object was removed; otherwise <b>false</b>.</returns>
            bool ICollection<TKey>.Remove (TKey key) => throw new NotSupportedException();


            /// <summary>Bypasses a supplied number of keys and yields the remaining keys.</summary>
            /// <param name="count">Number of keys to skip.</param>
            /// <returns>The keys after the supplied offset.</returns>
            /// <remarks>This is a O(log <em>n</em>) operation.</remarks>
            /// <example>
            /// In the below snippet, both Skip operations perform an order of magnitude faster than their LINQ equivalent.
            /// <code source="..\Bench\RxExample01\RxExample01.cs" lang="cs" region="RmkSkip" />
            /// </example>
            /// <exception cref="InvalidOperationException">When the map was modified after the enumerator was created.</exception>
            public Enumerator Skip (int count) => new Enumerator (tree, count);


            /// <summary>
            /// Bypasses keys as long as a supplied condition is true and yields the remaining keys.
            /// </summary>
            /// <param name="predicate">The condition to test for.</param>
            /// <returns>Remaining keys after the first key that does not satisfy the supplied condition.</returns>
            /// <exception cref="InvalidOperationException">When the map was modified after the enumerator was created.</exception>
            public Enumerator SkipWhile (Func<TKey,bool> predicate) => new Enumerator (tree, predicate);

            /// <summary>
            /// Bypasses keys as long as a supplied index-based condition is true and yields the remaining keys.
            /// </summary>
            /// <param name="predicate">The condition to test for.</param>
            /// <returns>Remaining keys after the first key that does not satisfy the supplied condition.</returns>
            /// <exception cref="InvalidOperationException">When the map was modified after the enumerator was created.</exception>
            public Enumerator SkipWhile (Func<TKey,int,bool> predicate) => new Enumerator (tree, predicate);


            /// <summary>Gets the least key greater than the supplied key.</summary>
            /// <param name="getKey">The key to use for comparison.</param>
            /// <param name="key">The actual key if found; otherwise the default.</param>
            /// <returns><b>true</b> if key greater than <em>getKey</em> is found; otherwise <b>false</b>.</returns>
            public bool TryGetGreaterThan (TKey getKey, out TKey key)
            {
                tree.TryGetGT (getKey, out Leaf leaf, out int index);
                if (leaf == null)
                { key = default; return false; }
                else
                { key = leaf.GetKey (index); return true; }
            }


            /// <summary>Gets the least key greater than or equal to the supplied key.</summary>
            /// <param name="getKey">The key to use for comparison.</param>
            /// <param name="key">The actual key if found; otherwise the default.</param>
            /// <returns><b>true</b> if key greater than or equal to <em>getKey</em> found; otherwise <b>false</b>.</returns>
            public bool TryGetGreaterThanOrEqual (TKey getKey, out TKey key)
            {
                tree.TryGetGE (getKey, out Leaf leaf, out int index);
                if (leaf == null)
                { key = default; return false; }
                else
                { key = leaf.GetKey (index); return true; }
            }


            /// <summary>Gets the greatest key that is less than the supplied key.</summary>
            /// <param name="getKey">The key to use for comparison.</param>
            /// <param name="key">The actual key if found; otherwise the default.</param>
            /// <returns><b>true</b> if key less than <em>getKey</em> found; otherwise <b>false</b>.</returns>
            public bool TryGetLessThan (TKey getKey, out TKey key)
            {
                tree.TryGetLT (getKey, out Leaf leaf, out int index);
                if (leaf == null)
                { key = default; return false; }
                else
                { key = leaf.GetKey (index); return true; }
            }


            /// <summary>Gets the greatest key that is less than or equal to the supplied key.</summary>
            /// <param name="getKey">The key to use for comparison.</param>
            /// <param name="key">The actual key if found; otherwise the default.</param>
            /// <returns><b>true</b> if key less than or equal to <em>getKey</em> found; otherwise <b>false</b>.</returns>
            public bool TryGetLessThanOrEqual (TKey getKey, out TKey key)
            {
                tree.TryGetLE (getKey, out Leaf leaf, out int index);
                if (leaf == null)
                { key = default; return false; }
                else
                { key = leaf.GetKey (index); return true; }
            }

            #endregion

            #region Enumeration

            /// <summary>Returns an enumerator that iterates thru the distinct map keys in sort order.</summary>
            /// <returns>An enumerator that iterates thru distinct keys.</returns>
            /// <remarks>
            /// <para>
            /// Retrieving each key is a O(log <em>n</em>) operation
            /// where <em>n</em> is <see cref="Count"/>.
            /// </para>
            /// </remarks>
            public IEnumerable<TKey> Distinct()
            {
                foreach (var key in tree.Distinct2())
                    yield return key;
            }


            /// <summary>Returns an enumerator that iterates thru the map keys in reverse order.</summary>
            /// <returns>An enumerator that reverse iterates thru the map keys.</returns>
            /// <exception cref="InvalidOperationException">When the map was modified after the enumerator was created.</exception>
            public Enumerator Reverse() => new Enumerator (tree, isReverse:true);


            /// <summary>Gets an enumerator that iterates thru the collection.</summary>
            /// <returns>An enumerator for the collection.</returns>
            public Enumerator GetEnumerator() => new Enumerator (tree);

            /// <summary>Gets an enumerator that iterates thru the collection.</summary>
            /// <returns>An enumerator for the collection.</returns>
            IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator() => new Enumerator (tree);

            /// <summary>Gets an enumerator that iterates thru the collection.</summary>
            /// <returns>An enumerator for the collection.</returns>
            IEnumerator IEnumerable.GetEnumerator() => new Enumerator (tree);


            /// <summary>Enumerates the items of a <see cref="RankedMap{TKey,TValue}.KeyCollection"/> in sort order.</summary>
            [DebuggerTypeProxy (typeof (IEnumerableKeysDebugView<,>))]
            public struct Enumerator : IEnumerator<TKey>, IEnumerable<TKey>
            {
                private readonly KeyEnumerator etor;

                internal Enumerator (RankedMap<TKey,TValue> map, bool isReverse=false) => etor = new KeyEnumerator (map, isReverse);

                internal Enumerator (RankedMap<TKey,TValue> map, int count) => etor = new KeyEnumerator (map, count);

                internal Enumerator (RankedMap<TKey,TValue> map, Func<TKey,bool> predicate) => etor = new KeyEnumerator (map, predicate);

                internal Enumerator (RankedMap<TKey,TValue> map, Func<TKey,int,bool> predicate) => etor = new KeyEnumerator (map, predicate);

                /// <summary>Gets the key at the current position.</summary>
                /// <exception cref="InvalidOperationException">When the enumerator is not active.</exception>
                object IEnumerator.Current
                {
                    get
                    {
                        if (etor.NotActive)
                            throw new InvalidOperationException ("Enumerator is not active.");
                        return (object) etor.CurrentKey;
                    }
                }

                /// <summary>Gets the key at the current position of the enumerator.</summary>
                public TKey Current => etor.CurrentKeyOrDefault;

                /// <summary>Advances the enumerator to the next key in the collection.</summary>
                /// <returns><b>true</b> if the enumerator was successfully advanced to the next key; <b>false</b> if the enumerator has passed the end of the collection.</returns>
                /// <exception cref="InvalidOperationException">When the map was modified after the enumerator was created.</exception>
                public bool MoveNext() => etor.Advance();

                /// <summary>Rewinds the enumerator to its initial state.</summary>
                void IEnumerator.Reset() => etor.Initialize();

                /// <summary>Releases all resources used by the enumerator.</summary>
                public void Dispose() { }

                /// <summary>Gets an iterator for this collection.</summary>
                /// <returns>An iterator for this collection.</returns>
                public IEnumerator<TKey> GetEnumerator() => this;

                /// <summary>Gets an iterator for this collection.</summary>
                /// <returns>An iterator for this collection.</returns>
                IEnumerator IEnumerable.GetEnumerator() => this;

                /// <summary>Bypasses a supplied number of keys and yields the remaining keys.</summary>
                /// <param name="count">Number of keys to skip.</param>
                /// <returns>The keys after the supplied offset.</returns>
                /// <remarks>This is a O(log <em>n</em>) operation.</remarks>
                /// <example>
                /// In the below snippet, both Skip operations perform an order of magnitude faster than their LINQ equivalent.
                /// <code source="..\Bench\RxExample01\RxExample01.cs" lang="cs" region="RmkSkip" />
                /// </example>
                /// <exception cref="InvalidOperationException">When the map was modified after the enumerator was created.</exception>
                public Enumerator Skip (int count)
                {
                    etor.Bypass (count);
                    return this;
                }

                /// <summary>
                /// Bypasses keys as long as a supplied condition is true and yields the remaining keys.
                /// </summary>
                /// <param name="predicate">The condition to test for.</param>
                /// <returns>Remaining keys after the first key that does not satisfy the supplied condition.</returns>
                /// <exception cref="InvalidOperationException">When the map was modified after the enumerator was created.</exception>
                public Enumerator SkipWhile (Func<TKey,bool> predicate)
                {
                    etor.BypassKey (predicate);
                    return this;
                }

                /// <summary>
                /// Bypasses keys as long as a supplied index-based condition is true and yields the remaining keys.
                /// </summary>
                /// <param name="predicate">The condition to test for.</param>
                /// <returns>Remaining keys after the first key that does not satisfy the supplied condition.</returns>
                /// <exception cref="InvalidOperationException">When the map was modified after the enumerator was created.</exception>
                public Enumerator SkipWhile (Func<TKey,int,bool> predicate)
                {
                    etor.BypassKey (predicate);
                    return this;
                }
            }

            #endregion
        }
    }
}
