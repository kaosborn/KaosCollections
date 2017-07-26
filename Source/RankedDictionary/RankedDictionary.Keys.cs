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

            /// <summary>
            /// Make a new <b>"BtreeDictionary&lt;TKey,TValue&gt;.KeyCollection</b> that
            /// holds the keys of a <see cref="RankedDictionary{TKey,TValue}"/>.
            /// </summary>
            /// <param name="dictionary">
            /// <see cref="RankedDictionary{TKey,TValue}"/> containing these keys.
            /// </param>
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

            /// <summary>
            /// Get the number of keys in the collection.
            /// </summary>
            public int Count
            { get { return tree.Count; } }

            #endregion

            #region Methods

            /// <summary>
            /// Copy keys to a target array starting as position <em>index</em> in the target.
            /// </summary>
            /// <param name="array">Array to modify.</param>
            /// <param name="index">Starting position in <em>array</em>.</param>
            public void CopyTo (TKey[] array, int index)
            {
                if (array == null)
                    throw new ArgumentNullException (nameof (array));

                if (index < 0)
                    throw new ArgumentOutOfRangeException (nameof (index), index, "Specified argument was out of the range of valid values.");

                if (Count > array.Length - index)
                    throw new ArgumentException ("Destination array is not long enough to copy all the items in the collection. Check array index and length.", nameof (array));

                for (var leaf = (Leaf) tree.leftmostLeaf; leaf != null; leaf = (Leaf) leaf.rightKeyLeaf)
                    for (int leafIndex = 0; leafIndex < leaf.KeyCount; ++leafIndex)
                        array[index++] = leaf.GetKey (leafIndex);
            }


            /// <summary>Returns an enumerator that iterates thru the KeyCollection.</summary>
            /// <returns>An enumerator for the collection.</returns>
            public IEnumerator<TKey> GetEnumerator()
            { return new Enumerator (tree); }

            #endregion

            #region Enumerator

            /// <summary>Enumerates the sorted elements of a KeyCollection.</summary>
            public sealed class Enumerator : IEnumerator<TKey>
            {
                private readonly RankedDictionary<TKey,TValue> tree;
                private Leaf leaf;
                private int index;

                internal Enumerator (RankedDictionary<TKey,TValue> dictionary)
                {
                    this.tree = dictionary;
                    ((IEnumerator) this).Reset();
                }

                object IEnumerator.Current
                {
                    get
                    {
                        if (index < 0)
                            throw new InvalidOperationException();
                        return (object) Current;
                    }
                }

                /// <summary>
                /// Gets the element at the current position of the enumerator.
                /// </summary>
                public TKey Current
                { get { return index < 0 ? default (TKey) : leaf.GetKey (index); } }

                /// <summary>Advances the enumerator to the next element in the collection.</summary>
                /// <returns><b>true</b> if the enumerator was successfully advanced to the next element; <b>false</b> if the enumerator has passed the end of the collection.</returns>
                public bool MoveNext()
                {
                    if (leaf != null)
                    {
                        if (++index < leaf.KeyCount)
                            return true;

                        leaf = (Leaf) leaf.rightKeyLeaf;
                        if (leaf != null)
                        { index = 0; return true; }

                        index = -1;
                    }

                    return false;
                }

                void IEnumerator.Reset()
                {
                    index = -1;
                    leaf = (Leaf) tree.leftmostLeaf;
                }

                /// <summary>Releases all resources used by the Enumerator.</summary>
                public void Dispose() { }
            }

            #endregion

            #region Explicit properties and methods interface implementations

            bool ICollection<TKey>.IsReadOnly
            { get { return true; } }

            bool ICollection.IsSynchronized
            { get { return false; } }

            object ICollection.SyncRoot
            { get { return tree.GetSyncRoot(); } }

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
                    throw new ArgumentOutOfRangeException (nameof (index), "Index is less than 0.");

                if (Count > array.Length - index)
                    throw new ArgumentException ("Destination array is not long enough to copy all the items in the collection. Check array index and length.", nameof (array));

                for (var leaf = (Leaf) tree.leftmostLeaf; leaf != null; leaf = (Leaf) leaf.rightKeyLeaf)
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
        }
    }
}
