//
// Library: KaosCollections
// File:    RankedDictionaryObject.cs
// Purpose: Defines nongeneric API for RankedDictionary and its Keys and Values subclasses.
//
// Copyright © 2009-2017 Kasey Osborn (github.com/kaosborn)
// MIT License - Use and redistribute freely
//

using System;
using System.Collections;
using System.Collections.Generic;

namespace Kaos.Collections
{
    public partial class RankedDictionary<TKey,TValue>
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

            var genCol = (IDictionary<TKey,TValue>) this;
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

            KeyLeaf leaf = Find ((TKey) key, out int ix);
            return ix >= 0;
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
                throw new ArgumentException ("Destination array is not long enough to copy all the items in the collection. Check array index and length.", nameof (array));

            if (! (array is KeyValuePair<TKey,TValue>[]) && array.GetType() != typeof (Object[]))
                throw new ArgumentException ("Target array type is not compatible with the type of items in the collection.", nameof (array));

            for (Leaf leaf = LeftmostLeaf; leaf != null; leaf = leaf.RightLeaf)
                for (int leafIndex = 0; leafIndex < leaf.KeyCount; ++leafIndex)
                {
                    array.SetValue (new KeyValuePair<TKey,TValue>(leaf.GetKey (leafIndex), leaf.GetValue (leafIndex)), index);
                    ++index;
                }
        }


        /// <summary>Gets an enumerator that iterates thru the collection.</summary>
        /// <returns>An enumerator for the collection.</returns>
        IDictionaryEnumerator IDictionary.GetEnumerator()
        { return new Enumerator (this, false); }


        /// <summary>Gets an enumerator that iterates thru the collection.</summary>
        /// <returns>An enumerator for the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        { return new Enumerator (this); }


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
                Remove2 (path);
        }


        /// <summary>Deprecated.</summary>
        object ICollection.SyncRoot
        { get { return new Object(); } }

        #endregion

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
                    var leaf = (Leaf) Find ((TKey) key, out int index);
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
                        Leaf.SetValue (path, (TValue) value);
                    else
                        Add2 (path, (TKey) key, (TValue) value);
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
        { get { return (ICollection) Keys; } }

        ICollection IDictionary.Values
        { get { return (ICollection) Values; } }

        #endregion
    }
}
