//
// Library: KaosCollections
// File:    BtreeDictionaryObject.cs
// Purpose: Defines nongeneric API for BtreeDictionary and its Keys and Values subclasses.
//
// Copyright © 2009-2017 Kasey Osborn (github.com/kaosborn)
// MIT License - Use and redistribute freely
//

using System;
using System.Collections;
using System.Collections.Generic;

namespace Kaos.Collections
{
    public partial class BtreeDictionary<TKey, TValue>
    {
        #region Explicit object properties and methods

        /// <summary>Adds the specified key and value to the dictionary.</summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        /// <exception cref="ArgumentNullException">When <em>key</em> is <b>null</b>.</exception>
        /// <exception cref="ArgumentException">When an element with the same key already exists in the Dictionary.</exception>
        /// <exception cref="ArgumentException">When <em>key</em> is not a TKey.</exception>
        /// <exception cref="ArgumentException">When <em>value</em> is not a TValue.</exception>
        void IDictionary.Add (object key, object value)
        {
            if (key == null)
                throw new ArgumentNullException (nameof (key));

            if (! (key is TKey))
                throw new ArgumentException ("Parameter '" + nameof (key) + "' is not of type '" + typeof (TKey) + "'.");

            if (! (value is TValue))
                throw new ArgumentException ("Parameter '" + nameof (value) + "' is not of type '" + typeof (TValue) + "'.");

            var genCol = (IDictionary<TKey, TValue>) this;
            genCol.Add ((TKey) key, (TValue) value);
        }


        /// <summary>Determines whether the dictionary contains an element with the specified key.</summary>
        /// <param name="key">The key to locate in the dictionary.</param>
        /// <returns><b>true</b> if the collection contains the supplied key; otherwise <b>false</b>.</returns>
        /// <exception cref="ArgumentNullException">When <em>key</em> is <b>null</b>.</exception>
        bool IDictionary.Contains (object key)
        {
            if (key == null)
                throw new ArgumentNullException (nameof (key));

            if (! (key is TKey))
                return false;

            Leaf leaf = Find ((TKey) key, out int lix);
            return lix >= 0;
        }


        /// <summary>Copies the elements of the dictionary to an array, starting at the specified array index.</summary>
        /// <param name="array">The destination array of the copy.</param>
        /// <param name="index">The zero-based index in array at which copying begins.</param>
        /// <exception cref="ArgumentNullException">When <em>array</em> is <b>null</b>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">When <em>index</em> is less than 0.</exception>
        /// <exception cref="ArgumentException">
        /// When array is multidimensional,
        /// the number of elements in the source is greater than the available space,
        /// or the type of the source cannot be cast for the destination.
        /// </exception>
        void ICollection.CopyTo (Array array, int index)
        {
            if (array == null)
                throw new ArgumentNullException (nameof (array));

            if (array.Rank > 1)
                throw new ArgumentException ("Multidimension array is not supported on this operation.", nameof (array));

            if (index < 0)
                throw new ArgumentOutOfRangeException (nameof (index), "Index is less than 0.");

            if (Count > array.Length - index)
                throw new ArgumentException ("Destination array is not long enough to copy all the items in the collection. Check array index and length.");

            if (! (array is KeyValuePair<TKey, TValue>[]) && array.GetType() != typeof (Object[]))
                throw new ArgumentException ("Target array type is not compatible with the type of items in the collection.", nameof (array));

            foreach (KeyValuePair<TKey, TValue> pair in this)
            {
                array.SetValue (pair, index);
                ++index;
            }
        }

        /// <summary>Gets an enumerator that iterates thru the collection.</summary>
        /// <returns>An enumerator for the collection.</returns>
        IDictionaryEnumerator IDictionary.GetEnumerator()
        { return new BtreeObjectEnumerator (this); }


        /// <summary>Gets an enumerator that iterates thru the collection.</summary>
        /// <returns>An enumerator for the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        { return ((IDictionary) this).GetEnumerator(); }


        /// <summary>Remove the supplied key and its associated value from the collection.</summary>
        /// <param name="key">Key to remove.</param>
        /// <exception cref="ArgumentNullException">When <em>key</em> is <b>null</b>.</exception>
        void IDictionary.Remove (object key)
        {
            if (key == null)
                throw new ArgumentNullException (nameof (key));

            if (! (key is TKey))
                return;

            var path = new NodeVector (this, (TKey) key);
            if (path.IsFound)
                path.Delete();
        }


        /// <summary>Deprecated.</summary>
        object ICollection.SyncRoot
        { get { return new Object(); } }

        #endregion

        /// <summary>
        /// Represents an iterator for the collection.
        /// </summary>
        public class BtreeObjectEnumerator : IDictionaryEnumerator
        {
            BtreeDictionary<TKey, TValue> target;
            Leaf currentLeaf;
            int leafIndex;

            #region Constructors

            /// <summary>
            /// Make an enumerator that will loop thru the collection in order.
            /// </summary>
            /// <param name="dictionary">
            /// <see cref="BtreeDictionary&lt;TKey,TValue&gt;"/>
            /// containing these key/value pairs.
            /// </param>
            public BtreeObjectEnumerator (BtreeDictionary<TKey, TValue> dictionary)
            {
                target = dictionary;
                Reset();
            }

            #endregion

            #region Properties

            /// <summary>
            /// Get the key/value pair at the current location.
            /// </summary>
            public DictionaryEntry Entry
            { get { return new DictionaryEntry (Key, Value); } }

            /// <summary>
            /// Get the key/value pair at the current location.
            /// </summary>
            public object Current
            { get { return Entry; } }

            /// <summary>
            /// Get the key at the current location.
            /// </summary>
            public object Key
            { get { return currentLeaf.GetKey (leafIndex); } }

            /// <summary>
            /// Get the value at the current location.
            /// </summary>
            public object Value
            { get { return currentLeaf.GetValue (leafIndex); } }

            #endregion

            #region Methods

            /// <summary>
            /// Advance the enumerator to the next location.
            /// </summary>
            /// <returns><b>false</b> if no more data; otherwise <b>true</b></returns>
            public bool MoveNext()
            {
                if (++leafIndex < currentLeaf.KeyCount)
                    return true;

                leafIndex = 0;
                currentLeaf = currentLeaf.RightLeaf;
                return currentLeaf != null;
            }

            /// <summary>
            /// Move the enumerator back to its initial location.
            /// </summary>
            public void Reset()
            {
                leafIndex = -1;
                currentLeaf = target.GetFirstLeaf();
            }

            #endregion
        }

        #region Explicit properties and methods

        /// <summary>Get or set the value associated with the supplied key.</summary>
        /// <exception cref="ArgumentNullException">When <em>key</em> is <b>null</b>.</exception>
        object IDictionary.this[object key]
        {
            get
            {
                if (key == null)
                    throw new ArgumentNullException (nameof (key));

                if (key is TKey)
                {
                    Leaf leaf = Find ((TKey) key, out int index);
                    if (index >= 0)
                        return leaf.GetValue (index);
                }

                return null;
            }
            set
            {
                if (key == null)
                    throw new ArgumentNullException (nameof (key));

                if (value == null && default (TValue) != null)
                    throw new ArgumentNullException (nameof (value));

                if (! (key is TKey))
                    throw new ArgumentException ("Parameter '" + nameof (key) + "' is not of type '" + typeof (TKey) + "'.");

                try
                {
                    var path = new NodeVector (this, (TKey) key);
                    if (path.IsFound)
                        path.LeafValue = (TValue) value;
                    else
                        path.Insert ((TKey) key, (TValue) value);
                }
                catch (InvalidCastException)
                {
                    // Can't use 'is' for this because it won't handle null.
                    throw new ArgumentException ("Parameter 'value' is not of type '" + typeof (TValue) + "'.");
                }
            }
        }

        bool IDictionary.IsFixedSize
        { get { return false; } }

        /// <summary>Indicate that structure may be modified.</summary>
        bool IDictionary.IsReadOnly
        { get { return false; } }

        bool ICollection.IsSynchronized
        { get { return false; } }

        ICollection IDictionary.Keys
        { get { return (ICollection) keys; } }

        ICollection IDictionary.Values
        { get { return (ICollection) values; } }

        #endregion
    }
}
