//
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
using System.Diagnostics;

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
    [DebuggerTypeProxy (typeof (IDictionaryDebugView<,>))]
    [DebuggerDisplay ("Count = {Count}")]
    public sealed partial class BtreeDictionary<TKey,TValue> :
        IDictionary<TKey,TValue>,
        IDictionary
#if NETSTANDARD1_0
        , IReadOnlyDictionary<TKey,TValue>
#endif
        where TKey : IComparable
    {
        private Node root;
        private KeyCollection keys;
        private ValueCollection values;
        private readonly Leaf leftmostLeaf;
        private readonly int maxKeyCount;
        private readonly IComparer<TKey> comparer;
        private const int MinimumOrder = 4;
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
                throw new ArgumentOutOfRangeException (nameof (order), "Must be between " + MinimumOrder + " and " + MaximumOrder);

            this.comparer = comparer ?? Comparer<TKey>.Default;
            this.maxKeyCount = order - 1;
            this.root = this.leftmostLeaf = new Leaf();
        }


        /// <summary>
        /// Make a new <see cref="BtreeDictionary&lt;TKey,TValue&gt;"/>
        /// that consists of TKey/TValue pairs from the supplied source.
        /// </summary>
        /// <param name="dictionary">The source of the contents of the new
        /// <see cref="BtreeDictionary&lt;TKey,TValue&gt;"/>.</param>
        public BtreeDictionary (IDictionary<TKey,TValue> dictionary) : this (dictionary, null)
        { }


        /// <summary>
        /// Make a new <see cref="BtreeDictionary&lt;TKey,TValue&gt;"/> consisting of
        /// TKey/TValue pairs from the supplied source sorted by the supplied key comparer.
        /// </summary>
        /// <param name="dictionary">The source of the contents of the new
        /// <see cref="BtreeDictionary&lt;TKey,TValue&gt;"/>.</param>
        /// <param name="comparer">Comparison operator for keys.</param>
        /// <exception cref="ArgumentNullException">When <em>dictionary</em> is <b>null</b>.</exception>
        public BtreeDictionary (IDictionary<TKey,TValue> dictionary, IComparer<TKey> comparer) : this (comparer)
        {
            if (dictionary == null)
                throw new ArgumentNullException (nameof (dictionary));

            foreach (KeyValuePair<TKey,TValue> pair in dictionary)
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
                throw new ArgumentException ("An entry with the same key already exists.", nameof (key));

            path.Insert (key, value);
        }


        /// <summary>Remove all TKey/TValue pairs from the collection.</summary>
        public void Clear()
        {
            leftmostLeaf.Truncate (0);
            leftmostLeaf.RightLeaf = null;
            root = leftmostLeaf;
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
                for (Leaf leaf = leftmostLeaf; leaf != null; leaf = leaf.RightLeaf)
                    for (int vix = 0; vix < leaf.ValueCount; ++vix)
                        if (comparer.Equals (leaf.GetValue (vix), value))
                            return true;
            }
            else
                for (Leaf leaf = leftmostLeaf; leaf != null; leaf = leaf.RightLeaf)
                    for (int vix = 0; vix < leaf.ValueCount; ++vix)
                        if (leaf.GetValue (vix) == null)
                            return true;

            return false;
        }


        /// <summary>Copy the collection to the specified array offset.</summary>
        /// <param name="array">Destionation of copy.</param>
        /// <param name="index">Copy starts at this location.</param>
        /// <exception cref="ArgumentNullException">When <em>array</em> is <b>null</b>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">When <em>index</em> is less than 0.</exception>
        /// <exception cref="ArgumentException">When not enough space is given for the copy.</exception>
        public void CopyTo (KeyValuePair<TKey,TValue>[] array, int index)
        {
            if (array == null)
                throw new ArgumentNullException (nameof (array));

            if (index < 0)
                throw new ArgumentOutOfRangeException (nameof (index), "Index is less than 0.");

            if (Count > array.Length - index)
                throw new ArgumentException ("Destination array is not long enough to copy all the items in the collection. Check array index and length.", nameof (array));

            for (Leaf leaf = leftmostLeaf; leaf != null; leaf = leaf.RightLeaf)
                for (int leafIndex = 0; leafIndex < leaf.KeyCount; ++leafIndex)
                    array[index++] = new KeyValuePair<TKey,TValue> (leaf.GetKey (leafIndex), leaf.GetValue (leafIndex));
        }


        /// <summary>Gets an enumerator that iterates thru the collection.</summary>
        /// <returns>An enumerator for the collection.</returns>
        /// <remarks>Implements IEnumerable&lt;KeyValuePair&lt;TKey,TValue&gt;&gt;.</remarks>
        public IEnumerator<KeyValuePair<TKey,TValue>> GetEnumerator()
        { return new Enumerator (this); }


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
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">If the key is found, its value is placed here; otherwise
        /// it will be loaded with the default value for its type.</param>
        /// <returns><b>true</b> if supplied key is found; otherwise <b>false</b>.</returns>
        /// <exception cref="ArgumentNullException">When <em>key</em> is <b>null</b>.</exception>
        public bool TryGetValue (TKey key, out TValue value)
        {
            if (key == null)
                throw new ArgumentNullException (nameof (key));

            Leaf leaf = Find (key, out int index);
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

        #endregion

        #region Enumerator class

        /// <summary>Provides sequential access to the TKey/TValue collection.</summary>
        public sealed class Enumerator : IEnumerator<KeyValuePair<TKey,TValue>>, IDictionaryEnumerator
        {
            private readonly BtreeDictionary<TKey,TValue> tree;
            private Leaf currentLeaf;
            private int leafIndex;
            private bool isGeneric;

            /// <summary>Make an iterator that will loop thru the collection in order.</summary>
            /// <param name="dictionary"><see cref="BtreeDictionary&lt;TKey,TValue&gt;"/>containing these key/value pairs.</param>
            /// <param name="isGeneric">Supply <b>false</b> to indicate object Current should return DictionaryEntry values.</param>
            internal Enumerator (BtreeDictionary<TKey,TValue> dictionary, bool isGeneric=true)
            {
                this.tree = dictionary;
                this.isGeneric = isGeneric;
                ((IEnumerator) this).Reset();
            }

            object IDictionaryEnumerator.Key
            { get { if (leafIndex < 0)
                        throw new InvalidOperationException ("Enumeration is not active.");
                    return currentLeaf.GetKey (leafIndex); } }

            object IDictionaryEnumerator.Value
            { get { if (leafIndex < 0)
                        throw new InvalidOperationException ("Enumeration is not active.");
                    return currentLeaf.GetValue (leafIndex); } }

            DictionaryEntry IDictionaryEnumerator.Entry
            { get { if (leafIndex < 0)
                        throw new InvalidOperationException ("Enumeration is not active.");
                    return new DictionaryEntry (currentLeaf.GetKey (leafIndex), currentLeaf.GetValue (leafIndex)); } }

            object IEnumerator.Current
            {
                get
                {
                    if (leafIndex < 0)
                        throw new InvalidOperationException ("Enumeration is not active.");

                    if (isGeneric)
                        return currentLeaf.GetPair (leafIndex);
                    else
                        return new DictionaryEntry (currentLeaf.GetKey (leafIndex), currentLeaf.GetValue (leafIndex));
                }
            }

            /// <summary>Get the key/value pair at the current location.</summary>
            public KeyValuePair<TKey,TValue> Current
            { get { return leafIndex < 0? new KeyValuePair<TKey,TValue> (default (TKey), default (TValue))
                                        : currentLeaf.GetPair (leafIndex); } }

            /// <summary>Advance the enumerator to the next location.</summary>
            /// <returns><b>false</b> if no more data; otherwise <b>true</b>.</returns>
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

            /// <summary>Move the enumerator back to its initial location.</summary>
            void IEnumerator.Reset()
            { leafIndex = -1; currentLeaf = tree.leftmostLeaf; }

            /// <exclude />
            public void Dispose() { }
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
        { get { return root.Weight; } }


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

                Leaf leaf = Find (key, out int index);
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
        public KeyCollection Keys
        {
            get
            {
                if (keys == null)
                    keys = new KeyCollection (this);
                return keys;
            }
        }


        /// <summary>
        /// Get the collection of values in the <see cref="BtreeDictionary&lt;TKey,TValue&gt;"/>.
        /// </summary>
        public ValueCollection Values
        {
            get
            {
                if (values == null)
                    values = new ValueCollection (this);
                return values;
            }
        }

        #endregion

        #region Explicit methods

        /// <summary>Adds an element with the specified key/value pair.</summary>
        /// <param name="keyValuePair">Contains the key and value of the element to add.</param>
        /// <exception cref="ArgumentException">When supplied <em>key</em>
        /// has already been added.</exception>
        void ICollection<KeyValuePair<TKey,TValue>>.Add (KeyValuePair<TKey,TValue> keyValuePair)
        {
            var path = new NodeVector (this, keyValuePair.Key);
            if (path.IsFound)
                throw new ArgumentException ("An entry with the same key already exists.", nameof (keyValuePair));

            path.Insert (keyValuePair.Key, keyValuePair.Value);
        }


        /// <summary>Determine if the collection contains the supplied key and value.</summary>
        /// <param name="keyValuePair">Key/value pair to find.</param>
        /// <returns><b>true</b> if the collection contains the specified pair;
        /// otherwise <b>false</b>.</returns>
        bool ICollection<KeyValuePair<TKey,TValue>>.Contains (KeyValuePair<TKey,TValue> keyValuePair)
        {
            var leaf = Find (keyValuePair.Key, out int index);
            if (index < 0)
                return false;
            return EqualityComparer<TValue>.Default.Equals (leaf.GetValue (index), keyValuePair.Value);
        }


        /// <summary>Indicate that this collection may be modified.</summary>
        bool ICollection<KeyValuePair<TKey,TValue>>.IsReadOnly
        { get { return false; } }


        /// <summary>Get the collection of keys from this key/value the collection.</summary>
        /// <remarks>The keys given by this collection are sorted according to the
        /// <see cref="Comparer"/> property.</remarks>
        ICollection<TKey> IDictionary<TKey,TValue>.Keys
        { get { return (ICollection<TKey>) Keys; } }

        /// <summary>Get the collection of values from this key/value collection.</summary>
        /// <remarks>The values given by this collection are sorted in the same
        /// order as their respective keys in the <see cref="Keys"/> property.</remarks>
        ICollection<TValue> IDictionary<TKey,TValue>.Values
        { get { return (ICollection<TValue>) Values; } }

#if NETSTANDARD1_0
        IEnumerable<TKey> IReadOnlyDictionary<TKey,TValue>.Keys
        { get { return Keys; } }

        IEnumerable<TValue> IReadOnlyDictionary<TKey,TValue>.Values
        { get { return Values; } }
#endif

        /// <summary>Delete the supplied key and its associated value from the collection.
        /// </summary>
        /// <param name="keyValuePair">Contains key and value to find and remove. No operation is taken
        /// unless both key and value match.</param>
        /// <returns><b>true</b> if key/value pair removed; otherwise <b>false</b>.</returns>
        bool ICollection<KeyValuePair<TKey,TValue>>.Remove (KeyValuePair<TKey,TValue> keyValuePair)
        {
            var path = new NodeVector (this, keyValuePair.Key);
            if (! path.IsFound || ! EqualityComparer<TValue>.Default.Equals (keyValuePair.Value, path.LeafValue))
                return false;

            path.Delete();
            return true;
        }

        #endregion
    }
}
