//
// Library: KaosCollections
// File:    RankedDictionary.KeyCollection.cs
// Purpose: Define KeyCollection nested class.
//
// Copyright © 2009-2018 Kasey Osborn (github.com/kaosborn)
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
        /// while significantly improving performance on large collections.
        /// </para>
        /// <para>
        /// Optimized instance methods with the signatures of LINQ methods have been implemented:
        /// <list type="bullet">
        /// <item><see cref="Contains"/></item>
        /// <item><see cref="ElementAt"/></item>
        /// <item><see cref="ElementAtOrDefault"/></item>
        /// <item><see cref="First"/></item>
        /// <item><see cref="Last"/></item>
        /// <item><see cref="Reverse"/></item>
        /// </list>
        /// </para>
        /// <para>
        /// Indexing enhancements include:
        /// <list type="bullet">
        /// <item><see cref="P:Kaos.Collections.RankedDictionary`2.KeyCollection.Item(System.Int32)"/></item>
        /// <item><see cref="IndexOf"/></item>
        /// </list>
        /// </para>
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
                => tree.FindEdgeForIndex (key, out Leaf _, out int _, leftEdge:true);


            /// <summary>Gets the minimum key in the dictionary per the comparer.</summary>
            /// <returns>The minimum key in the dictionary.</returns>
            /// <remarks>This is a O(1) operation.</remarks>
            /// <exception cref="InvalidOperationException">When <see cref="Count"/> is zero.</exception>
            public TKey First()
            {
                if (Count == 0)
                    throw new InvalidOperationException ("Sequence contains no elements.");

                return tree.leftmostLeaf.Key0;
            }


            /// <summary>Gets the maximum key in the dictionary per the comparer.</summary>
            /// <returns>The maximum key in the dictionary.</returns>
            /// <remarks>This is a O(1) operation.</remarks>
            /// <exception cref="InvalidOperationException">When <see cref="Count"/> is zero.</exception>
            public TKey Last()
            {
                if (Count == 0)
                    throw new InvalidOperationException ("Sequence contains no elements.");

                return tree.rightmostLeaf.GetKey (tree.rightmostLeaf.KeyCount - 1);
            }


            /// <summary>Bypasses a supplied number of keys and yields the remaining keys.</summary>
            /// <param name="count">Number of keys to skip.</param>
            /// <returns>The keys after the supplied offset.</returns>
            /// <remarks>This is a O(log <em>n</em>) operation.</remarks>
            /// <example>
            /// In the below snippet, both Skip operations perform an order of magnitude faster than their LINQ equivalent.
            /// <code source="..\Bench\RxExample01\RxExample01.cs" lang="cs" region="RdkSkip" />
            /// </example>
            /// <exception cref="InvalidOperationException">When the dictionary was modified after the enumerator was created.</exception>
            public Enumerator Skip (int count) => new Enumerator (tree, count);


            /// <summary>
            /// Bypasses keys as long as a supplied condition is true and yields the remaining keys.
            /// </summary>
            /// <param name="predicate">The condition to test for.</param>
            /// <returns>Remaining keys after the first key that does not satisfy the supplied condition.</returns>
            /// <exception cref="InvalidOperationException">When the dictionary was modified after the enumerator was created.</exception>
            public Enumerator SkipWhile (Func<TKey,bool> predicate) => new Enumerator (tree, predicate);

            /// <summary>
            /// Bypasses keys as long as a supplied index-based condition is true and yields the remaining keys.
            /// </summary>
            /// <param name="predicate">The condition to test for.</param>
            /// <returns>Remaining keys after the first key that does not satisfy the supplied condition.</returns>
            /// <exception cref="InvalidOperationException">When the dictionary was modified after the enumerator was created.</exception>
            public Enumerator SkipWhile (Func<TKey,int,bool> predicate) => new Enumerator (tree, predicate);


            /// <summary>Gets the actual key for the supplied search key.</summary>
            /// <param name="getKey">The key to find.</param>
            /// <param name="key">
            /// If <em>key</em> is found, its actual value is placed here;
            /// otherwise it will be loaded with the default for its type.
            /// </param>
            /// <returns><b>true</b> if <em>getKey</em> is found; otherwise <b>false</b>.</returns>
            public bool TryGet (TKey getKey, out TKey key)
            {
                var leaf = tree.Find (getKey, out int index);
                if (index < 0)
                { key = default (TKey); return false; }

                key = leaf.GetKey (index);
                return true;
            }


            /// <summary>Gets the least key greater than the supplied key.</summary>
            /// <param name="getKey">The key to use for comparison.</param>
            /// <param name="key">The actual key found.</param>
            /// <returns><b>true</b> if a key greater than <em>getKey</em> is found; otherwise <b>false</b>.</returns>
            public bool TryGetGreaterThan (TKey getKey, out TKey key)
            {
                tree.TryGetGT (getKey, out Leaf leaf, out int index);
                if (leaf == null)
                { key = default (TKey); return false; }
                else
                { key = leaf.GetKey (index); return true; }
            }


            /// <summary>Gets the least key greater than or equal to the supplied key.</summary>
            /// <param name="getKey">The key to use for comparison.</param>
            /// <param name="key">The actual key found.</param>
            /// <returns><b>true</b> if a key greater than or equal to <em>getKey</em> found; otherwise <b>false</b>.</returns>
            public bool TryGetGreaterThanOrEqual (TKey getKey, out TKey key)
            {
                tree.TryGetGE (getKey, out Leaf leaf, out int index);
                if (leaf == null)
                { key = default (TKey); return false; }
                else
                { key = leaf.GetKey (index); return true; }
            }


            /// <summary>Gets the greatest key less than the supplied key.</summary>
            /// <param name="getKey">The key to use for comparison.</param>
            /// <param name="key">The actual key if found; otherwise the default.</param>
            /// <returns><b>true</b> if a key less than <em>getKey</em> is found; otherwise <b>false</b>.</returns>
            public bool TryGetLessThan (TKey getKey, out TKey key)
            {
                tree.TryGetLT (getKey, out Leaf leaf, out int index);
                if (leaf == null)
                { key = default (TKey); return false; }
                else
                { key = leaf.GetKey (index); return true; }
            }

            /// <summary>Gets the greatest key that is less than or equal to the supplied key.</summary>
            /// <param name="getKey">The key to use for comparison.</param>
            /// <param name="key">The actual key if found; otherwise the default.</param>
            /// <returns><b>true</b> if a key less than or equal to <em>getKey</em> found; otherwise <b>false</b>.</returns>
            public bool TryGetLessThanOrEqual (TKey getKey, out TKey key)
            {
                tree.TryGetLE (getKey, out Leaf leaf, out int index);
                if (leaf == null)
                { key = default (TKey); return false; }
                else
                { key = leaf.GetKey (index); return true; }
            }


            /// <summary>Returns an enumerator that iterates thru the dictionary keys in reverse order.</summary>
            /// <returns>An enumerator that reverse iterates thru the dictionary keys.</returns>
            /// <exception cref="InvalidOperationException">When the dictionary was modified after the enumerator was created.</exception>
            public Enumerator Reverse() => new Enumerator (tree, isReverse:true);

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
            [DebuggerTypeProxy (typeof (IEnumerableKeysDebugView<,>))]
            public struct Enumerator : IEnumerator<TKey>, IEnumerable<TKey>
            {
                private readonly KeyEnumerator etor;

                internal Enumerator (RankedDictionary<TKey,TValue> dary, bool isReverse=false) => etor = new KeyEnumerator (dary, isReverse);

                internal Enumerator (RankedDictionary<TKey,TValue> dary, int count) => etor = new KeyEnumerator (dary, count);

                internal Enumerator (RankedDictionary<TKey,TValue> dary, Func<TKey,bool> predicate) => etor = new KeyEnumerator (dary, predicate);

                internal Enumerator (RankedDictionary<TKey,TValue> dary, Func<TKey,int,bool> predicate) => etor = new KeyEnumerator (dary, predicate);

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
                /// <exception cref="InvalidOperationException">When the dictionary was modified after the enumerator was created.</exception>
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
                /// <code source="..\Bench\RxExample01\RxExample01.cs" lang="cs" region="RdkSkip" />
                /// </example>
                /// <exception cref="InvalidOperationException">When the dictionary was modified after the enumerator was created.</exception>
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
                /// <exception cref="InvalidOperationException">When the dictionary was modified after the enumerator was created.</exception>
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
                /// <exception cref="InvalidOperationException">When the dictionary was modified after the enumerator was created.</exception>
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
