﻿//
// Library: KaosCollections
// File:    BtreeDictionary.cs
// Purpose: Defines BtreeDictionary generic API.
//
// Copyright © 2009-2017 Kasey Osborn (github.com/kaosborn)
// MIT License - Use and redistribute freely
//

using System;
using System.Collections;
using System.Collections.Generic;

[assembly: CLSCompliant (true)]
namespace Kaos.Collections
{
    /// <summary>Represents a collection of key/value pairs that are sorted on the key.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    /// <remarks>
    /// This class is a functional equivalent of the
    /// <see cref="System.Collections.Generic.SortedDictionary&lt;TKey,TValue&gt;"/>
    /// with the addition of the methods
    /// <see cref="BetweenKeys"/>, <see cref="SkipUntilKey"/>, and <see cref="Last"/>.
    /// </remarks>
    public sealed partial class BtreeDictionary<TKey, TValue> :
        IDictionary<TKey, TValue>,
        IDictionary
#if NETSTANDARD1_0
        , IReadOnlyDictionary<TKey, TValue>
#endif
        where TKey : IComparable
    {
        private Branch root;
        private IComparer<TKey> comparer;
        private int maxKeyCount;
        private KeyCollection keys;
        private ValueCollection values;
        private const int MinimumOrder = 5;
        private const int DefaultOrder = 128;
        private const int MaximumOrder = 256;

        #region Constructors

        /// <summary>
        /// Make a new BTD
        /// consisting of TKey/TValue pairs.
        /// </summary>
        public BtreeDictionary() : this (DefaultOrder, null)
        { }


        /// <summary>
        /// Make a new <see cref="BtreeDictionary&lt;TKey,TValue&gt;"/>
        /// consisting of TKey/TValue pairs.
        /// </summary>
        /// <param name="order">Maximum number of children of a node.</param>
        public BtreeDictionary (int order) : this (order, null)
        { }


        /// <summary>
        /// Make a new <see cref="BtreeDictionary&lt;TKey,TValue&gt;"/>
        /// consisting of TKey/TValue pairs sorted by the supplied key comparer.
        /// </summary>
        /// <param name="comparer">Comparison operator for keys.</param>
        public BtreeDictionary (IComparer<TKey> comparer) : this (DefaultOrder, comparer)
        { }


        /// <summary>
        /// Make a new <see cref="BtreeDictionary&lt;TKey,TValue&gt;"/>
        /// consisting of TKey/TValue pairs sorted by the supplied key comparer.
        /// </summary>
        /// <param name="order">Maximum number of children of a node.</param>
        /// <param name="comparer">Comparison operator for keys.</param>
        public BtreeDictionary (int order, IComparer<TKey> comparer)
        {
            if (order < MinimumOrder || order > MaximumOrder)
                throw new ArgumentOutOfRangeException ("order", "Must be between " + MinimumOrder + " and " + MaximumOrder);

            this.comparer = comparer ?? Comparer<TKey>.Default;

            // Create an empty tree consisting of an empty branch and an empty leaf.
            this.maxKeyCount = order - 1;
            this.root = new Branch (new Leaf());

            // Allocate the virtual subcollections.
            this.keys = new KeyCollection (this);
            this.values = new ValueCollection (this);
        }


        /// <summary>
        /// Make a new <see cref="BtreeDictionary&lt;TKey,TValue&gt;"/>
        /// that consists of TKey/TValue pairs from the supplied source.
        /// </summary>
        /// <param name="dictionary">The source of the contents of the new
        /// <see cref="BtreeDictionary&lt;TKey,TValue&gt;"/>.</param>
        public BtreeDictionary (IDictionary<TKey, TValue> dictionary) : this (dictionary, null)
        { }


        /// <summary>
        /// Make a new <see cref="BtreeDictionary&lt;TKey,TValue&gt;"/> consisting of
        /// TKey/TValue pairs from the supplied source sorted by the supplied key comparer.
        /// </summary>
        /// <param name="dictionary">The source of the contents of the new
        /// <see cref="BtreeDictionary&lt;TKey,TValue&gt;"/>.</param>
        /// <param name="comparer">Comparison operator for keys.</param>
        /// <exception cref="ArgumentNullException">When <em>dictionary</em> is <b>null</b>.</exception>
        public BtreeDictionary (IDictionary<TKey, TValue> dictionary, IComparer<TKey> comparer) : this (comparer)
        {
            if (dictionary == null)
                throw new ArgumentNullException ("dictionary");

            foreach (KeyValuePair<TKey, TValue> pair in dictionary)
                Add (pair.Key, pair.Value);
        }

        #endregion

        #region Methods

        /// <summary>Adds an element with the specified key and value.</summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.  May be null.</param>
        /// <exception cref="ArgumentNullException">When <em>key</em> is <b>null</b>.</exception>
        /// <exception cref="ArgumentException">When supplied <em>key</em>
        /// has already been added.</exception>
        public void Add (TKey key, TValue value)
        {
            if (key == null)
                throw new ArgumentNullException (nameof (key));

            var path = new NodeVector (this, key);
            if (path.IsFound)
                throw new ArgumentException ("An entry with the same key already exists.");

            path.Insert (key, value);
        }


        /// <summary>Remove all TKey/TValue pairs from the collection.</summary>
        public void Clear()
        {
            root = new Branch (new Leaf (maxKeyCount), maxKeyCount);
            Count = 0;
        }


        /// <summary>
        /// Determine if the collection contains the supplied key.
        /// </summary>
        /// <param name="key">Key to find.</param>
        /// <returns><b>true</b> if the collection contains the supplied key;
        /// otherwise <b>false</b>.</returns>
        /// <exception cref="ArgumentNullException">When supplied key is <b>null</b>.</exception>
        public bool ContainsKey (TKey key)
        {
            if (key == null)
                throw new ArgumentNullException (nameof (key));

            Leaf leaf = Find (key, out int index);
            return index >= 0;
        }


        /// <summary>Determine if the collection contains the supplied value.</summary>
        /// <remarks>This operation performs a sequential search.</remarks>
        /// <param name="value">Value to find.</param>
        /// <returns><b>true</b> if the collection contains the specified value;
        /// otherwise <b>false</b>.</returns>
        public bool ContainsValue (TValue value)
        {
            if (value != null)
            {
                var comparer = EqualityComparer<TValue>.Default;
                for (Leaf leaf = GetFirstLeaf(); leaf != null; leaf = leaf.RightLeaf)
                    for (int vix = 0; vix < leaf.ValueCount; ++vix)
                        if (comparer.Equals (leaf.GetValue (vix), value))
                            return true;
            }
            else
                for (Leaf leaf = GetFirstLeaf(); leaf != null; leaf = leaf.RightLeaf)
                    for (int vix = 0; vix < leaf.ValueCount; ++vix)
                        if (leaf.GetValue (vix) == null)
                            return true;

            return false;
        }


        /// <summary>Copy the collection to the specified array offset.</summary>
        /// <param name="array">Destionation of copy.</param>
        /// <param name="arrayIndex">Copy starts at this location.</param>
        /// <exception cref="ArgumentNullException">When <em>array</em> is <b>null</b>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">When <em>arrayIndex</em> is less than 0.</exception>
        /// <exception cref="ArgumentException">When not enough space is given for the copy.</exception>
        public void CopyTo (KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException (nameof (array));

            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException (nameof (arrayIndex), "Index is less than zero.");

            if (arrayIndex + Count > array.Length)
                throw new ArgumentException ("Destination array is not long enough to copy all the items in the collection. Check array index and length.");

            foreach (KeyValuePair<TKey, TValue> pair in this)
            {
                array[arrayIndex] = pair;
                ++arrayIndex;
            }
        }


        /// <summary>Gets an enumerator that iterates thru the collection.</summary>
        /// <returns>An enumerator for the collection.</returns>
        /// <remarks>Implements IEnumerable&lt;KeyValuePair&lt;TKey, TValue&gt;&gt;.</remarks>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        { return new BtreeEnumerator (this); }


        /// <summary>Remove the key/value pair from the dictionary.</summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns><b>true</b> if the element was successfully found and removed;
        /// otherwise <b>false</b>.</returns>
        /// <exception cref="ArgumentNullException">When <em>key</em> is <b>null</b>.</exception>
        public bool Remove (TKey key)
        {
            if (key == null)
                throw new ArgumentNullException (nameof (key));

            var path = new NodeVector (this, key);
            if (! path.IsFound)
                return false;

            path.Delete();
            return true;
        }


        /// <summary>Get the value associated with the supplied key.</summary>
        /// <param name="key">Target of search.</param>
        /// <param name="value">If the key is found, its value is placed here; otherwise
        /// it will be loaded with the default value for its type.</param>
        /// <returns><b>true</b> if supplied key is found; otherwise <b>false</b>.</returns>
        /// <exception cref="ArgumentNullException">When <em>key</em> is <b>null</b>.</exception>
        public bool TryGetValue (TKey key, out TValue value)
        {
            if (key == null)
                throw new ArgumentNullException (nameof (key));

            int index;
            Leaf leaf = Find (key, out index);
            if (index >= 0)
            {
                value = leaf.GetValue (index);
                return true;
            }
            else
            {
                value = default (TValue);
                return false;
            }
        }

        // ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- -----

        /// <summary>Provides sequential access to the TKey/TValue collection.</summary>
        public sealed class BtreeEnumerator : IEnumerator<KeyValuePair<TKey, TValue>>
        {
            // Long form is 10% faster than yield syntax form.
            BtreeDictionary<TKey, TValue> target;
            Leaf currentLeaf;
            int leafIndex;

            /// <summary>
            /// Make an iterator that will loop thru the collection in order.
            /// </summary>
            /// <param name="dictionary">
            /// <see cref="BtreeDictionary&lt;TKey,TValue&gt;"/>
            /// containing these key/value pairs.
            /// </param>
            public BtreeEnumerator (BtreeDictionary<TKey, TValue> dictionary)
            {
                target = dictionary;
                Reset();
            }

            object System.Collections.IEnumerator.Current
            { get { return (object) Current; } }

            /// <summary>
            /// Get the key/value pair at the current location.
            /// </summary>
            public KeyValuePair<TKey, TValue> Current
            { get { return currentLeaf.GetPair (leafIndex); } }

            /// <summary>
            /// Advance the enumerator to the next location.
            /// </summary>
            /// <returns><b>false</b> if no more data; otherewise <b>true</b></returns>
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
            { leafIndex = -1; currentLeaf = target.GetFirstLeaf(); }

            /// <exclude />
            public void Dispose() { GC.SuppressFinalize (this); }
        }

        #endregion

        #region Properties

        /// <summary>Used to order keys in the sorted dictionary.</summary>
        /// <remarks>To override sorting based on the default comparer, supply an
        /// alternate comparer when constructed.</remarks>
        public IComparer<TKey> Comparer
        { get { return comparer; } }


        /// <summary>Get the number of key/value pairs in the dictionary.</summary>
        public int Count
        { get; private set; }


        /// <summary>Get or set the value associated with the supplied key.</summary>
        /// <param name="key">The key of the association.</param>
        /// <returns>Value associated with the specified key.</returns>
        /// <exception cref="ArgumentNullException">When <em>key</em> is <b>null</b>.</exception>
        /// <exception cref="KeyNotFoundException">When getting a value for a non-existant key.</exception>
        /// <remarks>Setting a value for a non-existant key performs an insert operation.</remarks>
        public TValue this[TKey key]
        {
            get
            {
                if (key == null)
                    throw new ArgumentNullException (nameof (key));

                int index;
                Leaf leaf = Find (key, out index);
                if (index < 0)
                    throw new KeyNotFoundException ("The given key was not present in the dictionary.");
                return leaf.GetValue (index);
            }
            set
            {
                if (key == null)
                    throw new ArgumentNullException (nameof (key));

                var path = new NodeVector (this, key);
                if (path.IsFound)
                    path.LeafValue = value;
                else
                    path.Insert (key, value);
            }
        }


        /// <summary>
        /// Get the collection of keys in the <see cref="BtreeDictionary&lt;TKey,TValue&gt;"/>.
        /// </summary>
        public ICollection<TKey> Keys
        { get { return keys; } }


        /// <summary>
        /// Get the collection of values in the <see cref="BtreeDictionary&lt;TKey,TValue&gt;"/>.
        /// </summary>
        public ICollection<TValue> Values
        { get { return values; } }

        #endregion

        #region Explicit Methods

        /// <summary>Adds an element with the specified key/value pair.</summary>
        /// <param name="keyValuePair">Contains the key and value of the element to add.</param>
        /// <exception cref="ArgumentException">When supplied <em>key</em>
        /// has already been added.</exception>
        void ICollection<KeyValuePair<TKey, TValue>>.Add (KeyValuePair<TKey, TValue> keyValuePair)
        {
            var path = new NodeVector (this, keyValuePair.Key);
            if (path.IsFound)
                throw new ArgumentException ("An entry with the same key already exists.");

            path.Insert (keyValuePair.Key, keyValuePair.Value);
        }


        /// <summary>Determine if the collection contains the supplied key and value.</summary>
        /// <param name="pair">Key/value pair to find.</param>
        /// <returns><b>true</b> if the collection contains the specified pair;
        /// otherwise <b>false</b>.</returns>
        bool ICollection<KeyValuePair<TKey, TValue>>.Contains (KeyValuePair<TKey, TValue> pair)
        {
            var leaf = Find (pair.Key, out int index);
            if (index < 0)
                return false;
            return EqualityComparer<TValue>.Default.Equals (leaf.GetValue (index), pair.Value);
        }


        /// <summary>Indicate that this collection may be modified.</summary>
        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        { get { return false; } }


        /// <summary>Get the collection of keys from this key/value the collection.</summary>
        /// <remarks>The keys given by this collection are sorted according to the
        /// <see cref="Comparer"/> property.</remarks>
        ICollection<TKey> IDictionary<TKey, TValue>.Keys
        { get { return (ICollection<TKey>) keys; } }

#if NETSTANDARD1_0
        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys
        { get { return Keys; } }

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values
        { get { return Values; } }
#endif

        /// <summary>Delete the supplied key and its associated value from the collection.
        /// </summary>
        /// <param name="pair">Contains key and value to find and remove. No operation is taken
        /// unless both key and value match.</param>
        /// <returns><b>true</b> if key/value pair removed; otherwise <b>false</b>.</returns>
        bool ICollection<KeyValuePair<TKey, TValue>>.Remove (KeyValuePair<TKey, TValue> pair)
        {
            var path = new NodeVector (this, pair.Key);
            if (! path.IsFound || ! EqualityComparer<TValue>.Default.Equals (pair.Value, path.LeafValue))
                return false;

            path.Delete();
            return true;
        }

        /// <summary>Get the collection of values from this key/value collection.</summary>
        /// <remarks>The values given by this collection are sorted in the same
        /// order as their respective keys in the <see cref="Keys"/> property.</remarks>
        ICollection<TValue> IDictionary<TKey, TValue>.Values
        { get { return (ICollection<TValue>) values; } }

        #endregion
    }
}
