//
// Library: KaosCollections
// File:    RankedDictionary.ValueCollection.cs
// Purpose: Define ValueCollection nested class.
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
        /// Represents a collection of values of a <see cref="RankedDictionary{TKey,TValue}"/>.
        /// </summary>
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
            /// <param name="dictionary"><see cref="RankedDictionary{TKey,TValue}"/> containing these keys.</param>
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
            public int Count => tree.Count;

            /// <summary>Indicates that the collection is read-only.</summary>
            bool ICollection<TValue>.IsReadOnly => true;

            /// <summary>Indicates that the collection is not thread safe.</summary>
            bool ICollection.IsSynchronized => false;

            /// <summary>Gets an object that can be used to synchronize access to the collection.</summary>
            object ICollection.SyncRoot => tree.GetSyncRoot();

            #endregion

            #region Methods

            /// <summary>This implementation always throws a <see cref="NotSupportedException" />.</summary>
            /// <param name="value">The object to add.</param>
            void ICollection<TValue>.Add (TValue value)
            { throw new NotSupportedException(); }


            /// <summary>This implementation always throws a <see cref="NotSupportedException" />.</summary>
            void ICollection<TValue>.Clear()
            { throw new NotSupportedException(); }


            /// <summary>Determines whether the collection contains the supplied value.</summary>
            /// <param name="value">The value to locate in the collection.</param>
            /// <returns><b>true</b> if <em>value</em> is found in the collection; otherwise <b>false</b>.</returns>
            bool ICollection<TValue>.Contains (TValue value)
            { return tree.ContainsValue (value); }


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

                for (var leaf = (PairLeaf) tree.leftmostLeaf; leaf != null; leaf = (PairLeaf) leaf.rightLeaf)
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

                for (var leaf = (PairLeaf) tree.leftmostLeaf; leaf != null; leaf = (PairLeaf) leaf.rightLeaf)
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
            { throw new NotSupportedException(); }

            #endregion

            #region Enumeration

            /// <summary>Gets an enumerator that iterates thru the collection.</summary>
            /// <returns>An enumerator for the collection.</returns>
            public Enumerator GetEnumerator() => new Enumerator (tree);

            /// <summary>Gets an enumerator that iterates thru the collection.</summary>
            /// <returns>An enumerator for the collection.</returns>
            IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator() => new Enumerator (tree);

            /// <summary>Gets an enumerator that iterates thru the collection.</summary>
            /// <returns>An enumerator for the collection.</returns>
            IEnumerator IEnumerable.GetEnumerator() => new Enumerator (tree);


            /// <summary>Enumerates the items of a <see cref="RankedDictionary{TKey,TValue}.ValueCollection"/>.</summary>
            public sealed class Enumerator : IEnumerator<TValue>
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

                /// <summary>Gets the value at the current position.</summary>
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

                /// <summary>Gets the value at the current position of the enumerator.</summary>
                /// <exception cref="InvalidOperationException">When the dictionary was modified after the enumerator was created.</exception>
                public TValue Current
                {
                    get
                    {
                        tree.StageCheck (stageFreeze);
                        return index < 0 ? default (TValue) : leaf.GetValue (index);
                    }
                }

                /// <summary>Advances the enumerator to the next value in the collection.</summary>
                /// <returns><b>true</b> if the enumerator was successfully advanced to the next value; <b>false</b> if the enumerator has passed the end of the collection.</returns>
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

                /// <summary>Rewinds the enumerator to its initial state.</summary>
                void IEnumerator.Reset()
                {
                    stageFreeze = tree.stage;
                    index = -1;
                    leaf = (PairLeaf) tree.leftmostLeaf;
                }

                /// <summary>Releases all resources used by the enumerator.</summary>
                public void Dispose() { }
            }

            #endregion
        }
    }
}
