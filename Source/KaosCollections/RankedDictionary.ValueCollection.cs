//
// Library: KaosCollections
// File:    RankedDictionary.ValueCollection.cs
// Purpose: Define ValueCollection nested class.
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
    public partial class RankedDictionary<TKey,TValue>
    {
        /// <summary>
        /// Represents a collection of values of a <see cref="RankedDictionary{TKey,TValue}"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This class emulates and extends
        /// <see cref="SortedDictionary{TKey,TValue}.ValueCollection"/>
        /// while improving performance of operations on large collections.
        /// </para>
        /// <para>
        /// Optimized instance methods with the signatures of LINQ methods have been implemented:
        /// <list type="bullet">
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
        /// <item><see cref="P:Kaos.Collections.RankedDictionary`2.ValueCollection.Item(System.Int32)"/></item>
        /// <item><see cref="IndexOf"/></item>
        /// </list>
        /// </para>
        /// </remarks>
        [DebuggerTypeProxy (typeof (ICollectionValuesDebugView<,>))]
        [DebuggerDisplay ("Count = {Count}")]
        public sealed class ValueCollection :
            ICollection<TValue>,
            ICollection
#if ! NET35 && ! NET40
            , IReadOnlyCollection<TValue>
#endif
        {
            private readonly RankedDictionary<TKey,TValue> tree;

            #region Constructors

            /// <summary>Initializes a new collection that reflects the values of a <see cref="RankedDictionary{TKey,TValue}"/>.</summary>
            /// <param name="dictionary">Dictionary containing these keys.</param>
            /// <remarks>This is a O(1) operation.</remarks>
            /// <exception cref="ArgumentNullException">When <em>dictionary</em> is <b>null</b>.</exception>
            public ValueCollection (RankedDictionary<TKey,TValue> dictionary)
            {
                if (dictionary == null)
#pragma warning disable IDE0016
                    throw new ArgumentNullException (nameof (dictionary));
#pragma warning restore IDE0016

                this.tree = dictionary;
            }

            #endregion

            #region Properties

            /// <summary>Gets the number of values in the collection.</summary>
            public int Count
             => tree.Count;

            /// <summary>Indicates that the collection is read-only.</summary>
            bool ICollection<TValue>.IsReadOnly
             => true;

            /// <summary>Indicates that the collection is not thread safe.</summary>
            bool ICollection.IsSynchronized
             => false;

            /// <summary>Gets an object that can be used to synchronize access to the collection.</summary>
            object ICollection.SyncRoot
             => tree.GetSyncRoot();

            #endregion

            #region Methods

            /// <summary>This implementation always throws a <see cref="NotSupportedException" />.</summary>
            /// <param name="value">The object to add.</param>
            void ICollection<TValue>.Add (TValue value)
             => throw new NotSupportedException();


            /// <summary>This implementation always throws a <see cref="NotSupportedException" />.</summary>
            void ICollection<TValue>.Clear()
             => throw new NotSupportedException();


            /// <summary>Determines whether the dictionary contains the supplied value.</summary>
            /// <param name="value">The value to locate.</param>
            /// <returns><b>true</b> if <em>value</em> is contained in the dictionary; otherwise <b>false</b>.</returns>
            /// <remarks>This is a O(<em>n</em>) operation.</remarks>
            bool ICollection<TValue>.Contains (TValue value)
             => tree.ContainsValue2 (value) >= 0;


            /// <summary>Copies values to a supplied array, starting as the supplied position.</summary>
            /// <param name="array">A one-dimensional array that is the destination of the copy.</param>
            /// <param name="index">The zero-based starting position in <em>array</em>.</param>
            /// <exception cref="ArgumentNullException">When <em>array</em> is <b>null</b>.</exception>
            /// <exception cref="ArgumentOutOfRangeException">When <em>index</em> is less than zero.</exception>
            /// <exception cref="ArgumentException">When not enough space is given for the copy.</exception>
            public void CopyTo (TValue[] array, int index)
            {
                if (array == null)
                    throw new ArgumentNullException (nameof (array));

                if (index < 0)
                    throw new ArgumentOutOfRangeException (nameof (index), index, "Argument was out of the range of valid values.");

                if (Count > array.Length - index)
                    throw new ArgumentException ("Destination array is not long enough to copy all the items in the collection. Check array index and length.", nameof (array));

                for (var leaf = (PairLeaf<TValue>) tree.leftmostLeaf; leaf != null; leaf = (PairLeaf<TValue>) leaf.rightLeaf)
                {
                    leaf.CopyValuesTo (array, index, leaf.ValueCount);
                    index += leaf.ValueCount;
                }
            }

            /// <summary>Copies values to a supplied array, starting at the supplied position.</summary>
            /// <param name="array">A one-dimensional array that is the destination of the copy.</param>
            /// <param name="index">The zero-based starting position in <em>array</em>.</param>
            /// <exception cref="ArgumentNullException">When <em>array</em> is <b>null</b>.</exception>
            /// <exception cref="ArgumentOutOfRangeException">When <em>index</em> is less than zero.</exception>
            /// <exception cref="ArgumentException">When not enough space is given for the copy.</exception>
            void ICollection.CopyTo (Array array, int index)
            {
                if (array == null)
                    throw new ArgumentNullException (nameof (array));

                if (array.Rank > 1)
                    throw new ArgumentException ("Multidimension array is not supported on this operation.", nameof (array));

                if (index < 0)
                    throw new ArgumentOutOfRangeException (nameof (index), index, "Index is less than zero.");

                if (Count > array.Length - index)
                    throw new ArgumentException ("Destination array is not long enough to copy all the items in the collection. Check array index and length.", nameof (array));

                for (var leaf = (PairLeaf<TValue>) tree.leftmostLeaf; leaf != null; leaf = (PairLeaf<TValue>) leaf.rightLeaf)
                    for (int ix = 0; ix < leaf.KeyCount; ++ix)
                    {
                        array.SetValue (leaf.GetValue (ix), index);
                        ++index;
                    }
            }


            /// <summary>This implementation always throws a <see cref="NotSupportedException"/>.</summary>
            /// <param name="value">The value to remove.</param>
            /// <returns><b>true</b> if the object was removed; otherwise <b>false</b>.</returns>
            bool ICollection<TValue>.Remove (TValue value)
             => throw new NotSupportedException();

            #endregion

            #region Bonus methods

            /// <summary>Gets the value at the supplied index.</summary>
            /// <param name="index">The zero-based index of the value to get.</param>
            /// <returns>The value at <em>index</em>.</returns>
            /// <remarks>This is a O(log <em>n</em>) operation.</remarks>
            /// <exception cref="ArgumentOutOfRangeException">When <em>index</em> is less than zero or not less than the number of items.</exception>
            public TValue this[int index]
             => ElementAt (index);


            /// <summary>Gets the value at the supplied index.</summary>
            /// <param name="index">The zero-based index of the value to get.</param>
            /// <returns>The value at <em>index</em>.</returns>
            /// <remarks>This is a O(log <em>n</em>) operation.</remarks>
            /// <exception cref="ArgumentOutOfRangeException">When <em>index</em> is less than zero or greater than or equal to the number of keys.</exception>
            public TValue ElementAt (int index)
            {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException (nameof (index), "Argument is out of the range of valid values.");

                var leaf = (PairLeaf<TValue>) tree.Find (index, out int leafIndex);
                return leaf.GetValue (leafIndex);
            }


            /// <summary>Gets the value at the supplied index or the default if the index is out of range.</summary>
            /// <param name="index">The zero-based index of the value to get.</param>
            /// <returns>The value at <em>index</em>.</returns>
            /// <remarks>This is a O(log <em>n</em>) operation.</remarks>
            public TValue ElementAtOrDefault (int index)
            {
                if (index < 0 || index >= Count)
                    return default;

                var leaf = (PairLeaf<TValue>) tree.Find (index, out int leafIndex);
                return leaf.GetValue (leafIndex);
            }


            /// <summary>Gets the value of the element with the minimum key in the dictionary per the comparer.</summary>
            /// <returns>The value of the element with the minimum key.</returns>
            /// <remarks>This is a O(1) operation.</remarks>
            /// <exception cref="InvalidOperationException">When <see cref="Count"/> is zero.</exception>
            public TValue First()
            {
                if (Count == 0)
                    throw new InvalidOperationException ("Sequence contains no elements.");

                return ((PairLeaf<TValue>) tree.leftmostLeaf).GetValue (0);
            }


            /// <summary>Gets the index of the first element with the supplied value.</summary>
            /// <param name="value">The value to find.</param>
            /// <returns>The index of <em>value</em> if found; otherwise -1.</returns>
            /// <remarks>
            /// This is a O(<em>n</em>) operation.
            /// </remarks>
            public int IndexOf (TValue value)
             => tree.ContainsValue2<TValue> (value);


            /// <summary>Gets the value of the element with the maximum key in the dictionary per the comparer.</summary>
            /// <returns>The value of the element with the maximum key.</returns>
            /// <remarks>This is a O(1) operation.</remarks>
            /// <exception cref="InvalidOperationException">When <see cref="Count"/> is zero.</exception>
            public TValue Last()
            {
                if (Count == 0)
                    throw new InvalidOperationException ("Sequence contains no elements.");

                return ((PairLeaf<TValue>) tree.rightmostLeaf).GetValue (tree.rightmostLeaf.KeyCount - 1);
            }


            /// <summary>Bypasses a supplied number of values and yields the remaining values.</summary>
            /// <param name="count">Number of values to skip.</param>
            /// <returns>The values after the supplied offset.</returns>
            /// <remarks>This is a O(log <em>n</em>) operation.</remarks>
            /// <example>
            /// In the below snippet, both Skip operations perform an order of magnitude faster than their LINQ equivalent.
            /// <code source="..\Bench\RxExample01\RxExample01.cs" lang="cs" region="RdvSkip" />
            /// </example>
            /// <exception cref="InvalidOperationException">When the dictionary was modified after the enumerator was created.</exception>
            public Enumerator Skip (int count)
             => new Enumerator (tree, count);


            /// <summary>
            /// Bypasses values as long as a supplied condition is true and yields the remaining values.
            /// </summary>
            /// <param name="predicate">The condition to test for.</param>
            /// <returns>Remaining values after the first value that does not satisfy the supplied condition.</returns>
            /// <exception cref="InvalidOperationException">When the dictionary was modified after the enumerator was created.</exception>
            public Enumerator SkipWhile (Func<TValue,bool> predicate)
             => new Enumerator (tree, predicate);

            /// <summary>
            /// Bypasses values as long as a supplied index-based condition is true and yields the remaining values.
            /// </summary>
            /// <param name="predicate">The condition to test for.</param>
            /// <returns>Remaining values after the first value that does not satisfy the supplied condition.</returns>
            /// <exception cref="InvalidOperationException">When the dictionary was modified after the enumerator was created.</exception>
            public Enumerator SkipWhile (Func<TValue,int,bool> predicate)
             => new Enumerator (tree, predicate);


            /// <summary>Returns an enumerator that iterates thru the dictionary values in reverse key order.</summary>
            /// <returns>An enumerator that reverse iterates thru the dictionary values.</returns>
            /// <exception cref="InvalidOperationException">When the dictionary was modified after the enumerator was created.</exception>
            public Enumerator Reverse()
             => new Enumerator (tree, isReverse:true);

            #endregion

            #region Enumeration

            /// <summary>Gets an enumerator that iterates thru the collection.</summary>
            /// <returns>An enumerator for the collection.</returns>
            public Enumerator GetEnumerator()
             => new Enumerator (tree);

            /// <summary>Gets an enumerator that iterates thru the collection.</summary>
            /// <returns>An enumerator for the collection.</returns>
            IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
             => new Enumerator (tree);

            /// <summary>Gets an enumerator that iterates thru the collection.</summary>
            /// <returns>An enumerator for the collection.</returns>
            IEnumerator IEnumerable.GetEnumerator()
             => new Enumerator (tree);


            /// <summary>Enumerates the items of a <see cref="RankedDictionary{TKey,TValue}.ValueCollection"/> in key sort order.</summary>
            [DebuggerTypeProxy (typeof (IEnumerableValuesDebugView<,>))]
            public struct Enumerator : IEnumerator<TValue>, IEnumerable<TValue>
            {
                private readonly ValueEnumerator<TValue> etor;

                internal Enumerator (RankedDictionary<TKey,TValue> dary, bool isReverse=false)
                 => etor = new ValueEnumerator<TValue> (dary, isReverse);

                internal Enumerator (RankedDictionary<TKey,TValue> dary, int count)
                 => etor = new ValueEnumerator<TValue> (dary, count);

                internal Enumerator (RankedDictionary<TKey,TValue> dary, Func<TValue,bool> predicate)
                 => etor = new ValueEnumerator<TValue> (dary, predicate);

                internal Enumerator (RankedDictionary<TKey,TValue> dary, Func<TValue,int,bool> predicate)
                 => etor = new ValueEnumerator<TValue> (dary, predicate);

                /// <summary>Gets the value at the current position.</summary>
                /// <exception cref="InvalidOperationException">When the enumerator is not active.</exception>
                object IEnumerator.Current
                {
                    get
                    {
                        if (etor.NotActive)
                            throw new InvalidOperationException ("Enumerator is not active.");
                        return (object) etor.CurrentValue;
                    }
                }

                /// <summary>Gets the value at the current position of the enumerator.</summary>
                public TValue Current
                 => etor.CurrentValueOrDefault;

                /// <summary>Advances the enumerator to the next value in the collection.</summary>
                /// <returns><b>true</b> if the enumerator was successfully advanced to the next value; <b>false</b> if the enumerator has passed the end of the collection.</returns>
                /// <exception cref="InvalidOperationException">When the dictionary was modified after the enumerator was created.</exception>
                public bool MoveNext()
                 => etor.Advance();

                /// <summary>Rewinds the enumerator to its initial state.</summary>
                void IEnumerator.Reset()
                 => etor.Initialize();

                /// <summary>Releases all resources used by the enumerator.</summary>
                public void Dispose()
                { }

                /// <summary>Gets an iterator for this collection.</summary>
                /// <returns>An iterator for this collection.</returns>
                public IEnumerator<TValue> GetEnumerator()
                 => this;

                /// <summary>Gets an iterator for this collection.</summary>
                /// <returns>An iterator for this collection.</returns>
                IEnumerator IEnumerable.GetEnumerator()
                 => this;

                /// <summary>Bypasses a supplied number of values and yields the remaining values.</summary>
                /// <param name="count">Number of values to skip.</param>
                /// <returns>The values after the supplied offset.</returns>
                /// <remarks>This is a O(log <em>n</em>) operation.</remarks>
                /// <example>
                /// In the below snippet, both Skip operations perform an order of magnitude faster than their LINQ equivalent.
                /// <code source="..\Bench\RxExample01\RxExample01.cs" lang="cs" region="RdvSkip" />
                /// </example>
                /// <exception cref="InvalidOperationException">When the dictionary was modified after the enumerator was created.</exception>
                public Enumerator Skip (int count)
                {
                    etor.Bypass (count);
                    return this;
                }

                /// <summary>
                /// Bypasses values as long as a supplied condition is true and yields the remaining values.
                /// </summary>
                /// <param name="predicate">The condition to test for.</param>
                /// <returns>Remaining values after the first value that does not satisfy the supplied condition.</returns>
                /// <exception cref="InvalidOperationException">When the dictionary was modified after the enumerator was created.</exception>
                public Enumerator SkipWhile (Func<TValue,bool> predicate)
                {
                    etor.BypassValue (predicate);
                    return this;
                }

                /// <summary>
                /// Bypasses values as long as a supplied index-based condition is true and yields the remaining values.
                /// </summary>
                /// <param name="predicate">The condition to test for.</param>
                /// <returns>Remaining values after the first value that does not satisfy the supplied condition.</returns>
                /// <exception cref="InvalidOperationException">When the dictionary was modified after the enumerator was created.</exception>
                public Enumerator SkipWhile (Func<TValue,int,bool> predicate)
                {
                    etor.BypassValue (predicate);
                    return this;
                }
            }

            #endregion
        }
    }
}
