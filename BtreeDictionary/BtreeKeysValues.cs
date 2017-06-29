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
            /// Copy keys to a target array starting as position <em>arrayIndex</em> in the target.
            /// </summary>
            /// <param name="array">Array to modify.</param>
            /// <param name="arrayIndex">Starting position in <em>array</em>.</param>
            public void CopyTo (TKey[] array, int arrayIndex)
            {
                if (array == null)
                    throw new ArgumentNullException (nameof (array));

                if (arrayIndex < 0)
                    throw new ArgumentOutOfRangeException (nameof (arrayIndex), arrayIndex, "Specified argument was out of the range of valid values.");

                if (arrayIndex + Count > array.Length)
                    throw new ArgumentException ("Destination array is not long enough to copy all the items in the collection. Check array index and length.");

                foreach (TKey key in this)
                {
                    array[arrayIndex] = key;
                    ++arrayIndex;
                }
            }

            #endregion

            #region Iteration

            /// <summary>
            /// Get an iterator that will loop thru the collection in order.
            /// </summary>
            public IEnumerator<TKey> GetEnumerator()
            { return new BtreeKeysEnumerator (tree); }


            /// <summary>
            /// Get an enumerator that will loop thru the collection in order.
            /// </summary>
            private class BtreeKeysEnumerator : IEnumerator<TKey>
            {
                private readonly BtreeDictionary<TKey,TValue> target;
                private Leaf currentLeaf;
                private int leafIndex;

                // Long form used for 5% performance increase.
                public BtreeKeysEnumerator (BtreeDictionary<TKey,TValue> tree)
                {
                    this.target = tree;
                    Reset();
                }

                object IEnumerator.Current
                { get { return (object) Current; } }

                public TKey Current
                { get { return currentLeaf.GetKey (leafIndex); } }

                public bool MoveNext()
                {
                    if (++leafIndex < currentLeaf.KeyCount)
                        return true;

                    leafIndex = 0;
                    currentLeaf = currentLeaf.RightLeaf;
                    return currentLeaf != null;
                }

                public void Reset()
                {
                    leafIndex = -1;
                    currentLeaf = target.GetFirstLeaf();
                }

                public void Dispose() { Dispose (true); GC.SuppressFinalize (this); }
                protected virtual void Dispose (bool disposing) { }
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

                if (index + Count > array.Length)
                    throw new ArgumentException ("Destination array is not long enough to copy all the items in the collection. Check array index and length.");

                foreach (TKey key in this)
                {
                    array.SetValue (key, index);
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
            /// Copy values to a target array starting as position <em>arrayIndex</em> in the target.
            /// </summary>
            /// <param name="array">Array to modify.</param>
            /// <param name="arrayIndex">Starting position in <em>array</em>.</param>
            public void CopyTo (TValue[] array, int arrayIndex)
            {
                if (array == null)
                    throw new ArgumentNullException (nameof (array));

                if (arrayIndex < 0)
                    throw new ArgumentOutOfRangeException (nameof (arrayIndex), arrayIndex, "Specified argument was out of the range of valid values.");

                if (arrayIndex + Count > array.Length)
                    throw new ArgumentException ("Destination array is not long enough to copy all the items in the collection. Check array index and length.");

                foreach (TValue value in this)
                {
                    array[arrayIndex] = value;
                    ++arrayIndex;
                }
            }

            #endregion

            #region Iteration

            /// <summary>
            /// Get an enumerator that will loop thru the collection of values.
            /// </summary>
            /// <returns>An enumerator for the collection.</returns>
            public IEnumerator<TValue> GetEnumerator()
            {
                for (Leaf currentLeaf = tree.GetFirstLeaf();; )
                {
                    for (int leafIndex = 0; leafIndex < currentLeaf.KeyCount; ++leafIndex)
                        yield return currentLeaf.GetValue (leafIndex);

                    currentLeaf = currentLeaf.RightLeaf;
                    if (currentLeaf == null)
                        break;
                }
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

                if (index + Count > array.Length)
                    throw new ArgumentException ("Destination array is not long enough to copy all the items in the collection. Check array index and length.");

                foreach (TValue value in this)
                {
                    array.SetValue (value, index);
                    ++index;
                }
            }

            bool ICollection<TValue>.IsReadOnly
            { get { return true; } }

            bool ICollection<TValue>.Remove (TValue val)
            { throw new NotSupportedException(); }

            bool ICollection.IsSynchronized
            { get { return false; } }

            object ICollection.SyncRoot
            { get { return ((ICollection) tree).SyncRoot; } }

            #endregion
        }
    }
}
