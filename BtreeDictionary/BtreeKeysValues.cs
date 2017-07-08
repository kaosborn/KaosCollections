//
// Library: KaosCollections
// File:    BtreeKeysValues.cs
// Purpose: Define BtreeDictionary Keys and Values nested classes.
//
// Copyright © 2009-2017 Kasey Osborn (github.com/kaosborn)
// MIT License - Use and redistribute freely
//

using System;
using System.Collections;
using System.Collections.Generic;

namespace Kaos.Collections
{
    public partial class BtreeDictionary<TKey,TValue>
    {
        /// <summary>
        /// Represents a collection of keys of a <see cref="BtreeDictionary&lt;TKey,TValue&gt;"/>.
        /// </summary>
        public sealed class KeyCollection :
            ICollection<TKey>,
            ICollection
#if NETSTANDARD1_0
            , IReadOnlyCollection<TKey>
#endif
        {
            private readonly BtreeDictionary<TKey,TValue> tree;

            #region Constructors

            /// <summary>
            /// Make a new <b>"BtreeDictionary&lt;TKey,TValue&gt;.KeyCollection</b> that
            /// holds the keys of a <see cref="BtreeDictionary&lt;TKey,TValue&gt;"/>.
            /// </summary>
            /// <param name="dictionary">
            /// <see cref="BtreeDictionary&lt;TKey,TValue&gt;"/> containing these keys.
            /// </param>
            /// <exception cref="ArgumentNullException">When <em>dictionary</em> is <b>null</b>.</exception>
            public KeyCollection (BtreeDictionary<TKey,TValue> dictionary)
            {
                if (dictionary == null)
                    throw new ArgumentNullException (nameof (dictionary));

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

                for (Leaf leaf = tree.leftmostLeaf; leaf != null; leaf = leaf.RightLeaf)
                    for (int leafIndex = 0; leafIndex < leaf.KeyCount; ++leafIndex)
                        array[index++] = leaf.GetKey (leafIndex);
            }

            #endregion

            #region Iteration

            /// <summary>Returns an enumerator that iterates through the KeyCollection.</summary>
            public IEnumerator<TKey> GetEnumerator()
            { return new Enumerator (tree); }


            /// <summary>Enumerates the sorted elements of a KeyCollection.</summary>
            public sealed class Enumerator : IEnumerator<TKey>
            {
                private readonly BtreeDictionary<TKey,TValue> tree;
                private Leaf currentLeaf;
                private int leafIndex;

                internal Enumerator (BtreeDictionary<TKey,TValue> dictionary)
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

                public TKey Current
                { get { return leafIndex < 0? default (TKey) : currentLeaf.GetKey (leafIndex); } }

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
                    currentLeaf = tree.leftmostLeaf;
                }

                public void Dispose() { }
            }

            #endregion

            #region Explicit properties and methods

            /// <summary>Gets an enumerator that iterates thru the collection.</summary>
            /// <returns>An enumerator for the collection.</returns>
            IEnumerator IEnumerable.GetEnumerator()
            { return GetEnumerator(); }

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

                for (Leaf leaf = tree.leftmostLeaf; leaf != null; leaf = leaf.RightLeaf)
                    for (int leafIndex = 0; leafIndex < leaf.KeyCount; ++leafIndex)
                    {
                        array.SetValue (leaf.GetKey (leafIndex), index);
                        ++index;
                    }
            }

            bool ICollection<TKey>.IsReadOnly
            { get { return true; } }

            bool ICollection<TKey>.Remove (TKey key)
            { throw new NotSupportedException(); }

            bool ICollection.IsSynchronized
            { get { return false; } }

            object ICollection.SyncRoot
            { get { return ((ICollection) tree).SyncRoot; } }

            #endregion
        }


        /// <summary>
        /// Represents a collection of values of a <see cref="BtreeDictionary&lt;TKey,TValue&gt;"/>.
        /// </summary>
        public sealed class ValueCollection :
            ICollection<TValue>,
            ICollection
#if NETSTANDARD1_0
            , IReadOnlyCollection<TValue>
#endif
        {
            private readonly BtreeDictionary<TKey,TValue> tree;

            #region Constructors

            /// <summary>
            /// Make a new <b>"BtreeDictionary&lt;TKey,TValue&gt;.ValueCollection</b> that
            /// holds the values of a <see cref="BtreeDictionary&lt;TKey,TValue&gt;"/>.
            /// </summary>
            /// <param name="dictionary">
            /// <see cref="BtreeDictionary&lt;TKey,TValue&gt;"/> containing these keys.
            /// </param>
            /// <exception cref="ArgumentNullException">When <em>dictionary</em> is <b>null</b>.</exception>
            public ValueCollection (BtreeDictionary<TKey,TValue> dictionary)
            {
                if (dictionary == null)
                    throw new ArgumentNullException (nameof (dictionary));

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

                for (Leaf leaf = tree.leftmostLeaf; leaf != null; leaf = leaf.RightLeaf)
                    for (int leafIndex = 0; leafIndex < leaf.KeyCount; ++leafIndex)
                        array[index++] = leaf.GetValue (leafIndex);
            }

            #endregion

            #region Iteration

            /// <summary>
            /// Returns an enumerator that iterates through the ValueCollection.
            /// </summary>
            /// <returns>An enumerator for the collection.</returns>
            public IEnumerator<TValue> GetEnumerator()
            { return new Enumerator (tree); }


            /// <summary>Enumerates the elements of a ValueCollection ordered by key.</summary>
            public sealed class Enumerator : IEnumerator<TValue>
            {
                private readonly BtreeDictionary<TKey,TValue> tree;
                private Leaf currentLeaf;
                private int leafIndex;

                internal Enumerator (BtreeDictionary<TKey,TValue> dictionary)
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

                public TValue Current
                { get { return leafIndex < 0? default (TValue) : currentLeaf.GetValue (leafIndex); } }

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
                    currentLeaf = tree.leftmostLeaf;
                }

                public void Dispose () { }
            }

            #endregion

            #region Explicit properties and methods

            IEnumerator IEnumerable.GetEnumerator()
            { return GetEnumerator(); }

            /// <exclude />
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

                for (Leaf leaf = tree.leftmostLeaf; leaf != null; leaf = leaf.RightLeaf)
                    for (int leafIndex = 0; leafIndex < leaf.KeyCount; ++leafIndex)
                    {
                        array.SetValue (leaf.GetValue (leafIndex), index);
                        ++index;
                    }
            }

            bool ICollection<TValue>.IsReadOnly
            { get { return true; } }

            bool ICollection<TValue>.Remove (TValue value)
            { throw new NotSupportedException(); }

            bool ICollection.IsSynchronized
            { get { return false; } }

            object ICollection.SyncRoot
            { get { return ((ICollection) tree).SyncRoot; } }

            #endregion
        }
    }
}
