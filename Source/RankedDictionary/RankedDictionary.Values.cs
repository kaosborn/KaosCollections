//
// Library: KaosCollections
// File:    RankedDictionary.Values.cs
// Purpose: Define Values nested class.
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

            /// <summary>
            /// Make a new <b>"BtreeDictionary&lt;TKey,TValue&gt;.ValueCollection</b> that
            /// holds the values of a <see cref="RankedDictionary{TKey,TValue}"/>.
            /// </summary>
            /// <param name="dictionary">
            /// <see cref="RankedDictionary{TKey,TValue}"/> containing these keys.
            /// </param>
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

            /// <summary>
            /// Get the number of values in the collection.
            /// </summary>
            public int Count
            { get { return tree.Count; } }

            #endregion

            #region Methods

            /// <summary>
            /// Copy values to a target array starting as position <em>index</em> in the target.
            /// </summary>
            /// <param name="array">Array to modify.</param>
            /// <param name="index">Starting position in <em>array</em>.</param>
            public void CopyTo (TValue[] array, int index)
            {
                if (array == null)
                    throw new ArgumentNullException (nameof (array));

                if (index < 0)
                    throw new ArgumentOutOfRangeException (nameof (index), index, "Specified argument was out of the range of valid values.");

                if (Count > array.Length - index)
                    throw new ArgumentException ("Destination array is not long enough to copy all the items in the collection. Check array index and length.", nameof (array));

                for (Leaf leaf = tree.LeftmostLeaf; leaf != null; leaf = leaf.RightLeaf)
                    for (int leafIndex = 0; leafIndex < leaf.KeyCount; ++leafIndex)
                        array[index++] = leaf.GetValue (leafIndex);
            }


            /// <summary>
            /// Returns an enumerator that iterates through the ValueCollection.
            /// </summary>
            /// <returns>An enumerator for the collection.</returns>
            public IEnumerator<TValue> GetEnumerator()
            { return new Enumerator (tree); }

            #endregion

            #region Enumerator

            /// <summary>Enumerates the elements of a ValueCollection ordered by key.</summary>
            public sealed class Enumerator : IEnumerator<TValue>
            {
                private readonly RankedDictionary<TKey,TValue> tree;
                private Leaf currentLeaf;
                private int leafIndex;

                internal Enumerator (RankedDictionary<TKey,TValue> dictionary)
                {
                    this.tree = dictionary;
                    ((IEnumerator) this).Reset();
                }

                object IEnumerator.Current
                {
                    get
                    {
                        if (leafIndex < 0)
                            throw new InvalidOperationException();
                        return (object) Current;
                    }
                }

                /// <summary>
                /// Gets the element at the current position of the enumerator.
                /// </summary>
                public TValue Current
                { get { return leafIndex < 0? default (TValue) : currentLeaf.GetValue (leafIndex); } }

                /// <summary>Advances the enumerator to the next element in the collection.</summary>
                /// <returns><b>true</b> if the enumerator was successfully advanced to the next element; <b>false</b> if the enumerator has passed the end of the collection.</returns>
                public bool MoveNext()
                {
                    if (currentLeaf != null)
                    {
                        if (++leafIndex < currentLeaf.KeyCount)
                            return true;

                        currentLeaf = currentLeaf.RightLeaf;
                        if (currentLeaf != null)
                        { leafIndex = 0; return true; }

                        leafIndex = -1;
                    }

                    return false;
                }

                void IEnumerator.Reset()
                {
                    leafIndex = -1;
                    currentLeaf = tree.LeftmostLeaf;
                }

                /// <summary>Releases all resources used by the Enumerator.</summary>
                public void Dispose () { }
            }

            #endregion

            #region Explicit properties and methods interface implementations

            bool ICollection<TValue>.IsReadOnly
            { get { return true; } }

            bool ICollection.IsSynchronized
            { get { return false; } }

            object ICollection.SyncRoot
            { get { return tree.GetSyncRoot(); } }

            void ICollection<TValue>.Add (TValue value)
            { throw new NotSupportedException(); }

            void ICollection<TValue>.Clear()
            { throw new NotSupportedException(); }

            bool ICollection<TValue>.Contains (TValue value)
            { return tree.ContainsValue (value); }

            void ICollection.CopyTo (Array array, int index)
            {
                if (array == null)
                    throw new ArgumentNullException (nameof (array));

                if (array.Rank > 1)
                    throw new ArgumentException ("Multidimension array is not supported on this operation.", nameof (array));

                if (index < 0)
                    throw new ArgumentOutOfRangeException (nameof (index), index, "Index is less than 0.");

                if (Count > array.Length - index)
                    throw new ArgumentException ("Destination array is not long enough to copy all the items in the collection. Check array index and length.", nameof (array));

                for (Leaf leaf = tree.LeftmostLeaf; leaf != null; leaf = leaf.RightLeaf)
                    for (int leafIndex = 0; leafIndex < leaf.KeyCount; ++leafIndex)
                    {
                        array.SetValue (leaf.GetValue (leafIndex), index);
                        ++index;
                    }
            }

            IEnumerator IEnumerable.GetEnumerator()
            { return GetEnumerator(); }

            bool ICollection<TValue>.Remove (TValue value)
            { throw new NotSupportedException(); }

            #endregion
        }
    }
}
