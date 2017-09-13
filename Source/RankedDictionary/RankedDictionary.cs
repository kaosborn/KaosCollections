//
// Library: KaosCollections
// File:    RankedDictionary.cs
//
// Copyright © 2009-2017 Kasey Osborn (github.com/kaosborn)
// MIT License - Use and redistribute freely
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
#if NET35 || NET40 || NET45 || SERIALIZE
using System.Runtime.Serialization;
#endif

namespace Kaos.Collections
{
    /// <summary>
    /// Represents a collection of key/value pairs that are sorted on unique keys.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    /// <remarks>
    /// <para>
    /// This class emulates and extends
    /// <see cref="System.Collections.Generic.SortedDictionary{TKey,TValue}"/>while
    /// improving performance of operations on large collections.
    /// While primarily emulating SortedDictionary, this class also borrows heavily from
    /// <see href="https://msdn.microsoft.com/en-us/library/ms132319(v=vs.110).aspx">SortedList&lt;TKey,TValue&gt;</see>
    /// for indexing functionality:
    /// <list type="bullet">
    /// <item><see cref="IndexOfKey"/></item>
    /// <item><see cref="IndexOfValue"/></item>
    /// <item><see cref="RemoveAt"/></item>
    /// </list>
    /// <para>Extension methods have been directly implemented and optimized:</para>
    /// <list type="bullet">
    /// <item><see cref="ElementAt"/></item>
    /// <item><see cref="ElementAtOrDefault"/></item>
    /// <item><see cref="Last"/></item>
    /// </list>
    /// <para>Indexing functionality also includes:</para>
    /// <list type="bullet">
    /// <item><see cref="TryGetValueAndIndex"/></item>
    /// </list>
    /// <para>These optimized range enumerators are included:</para>
    /// <list type="bullet">
    /// <item><see cref="ElementsBetween"/></item>
    /// <item><see cref="ElementsFrom"/></item>
    /// </list>
    /// <para>Properties and a method have been shared with SortedSet:</para>
    /// <list type="bullet">
    /// <item><see cref="MinKey"/></item>
    /// <item><see cref="MaxKey"/></item>
    /// <item><see cref="RemoveWhere"/></item>
    /// <item><see cref="Reverse"/></item>
    /// </list>
    /// </para>
    /// <para>
    /// Keys must be immutable as long as they are used as keys in the
    /// <see cref="RankedDictionary{TKey,TValue}"/>class.
    /// Every key must be unique and cannot be null, but a value can be for a reference type.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code source="..\Bench\RdExample01\RdExample01.cs" lang="cs" />
    /// </example>
    [DebuggerTypeProxy (typeof (IDictionaryDebugView<,>))]
    [DebuggerDisplay ("Count = {Count}")]
#if NET35 || NET40 || NET45 || SERIALIZE
    [Serializable]
#endif
    public partial class RankedDictionary<TKey,TValue> :
        Btree<TKey>
        , IDictionary<TKey,TValue>
        , IDictionary
#if ! NET35 && ! NET40
        , IReadOnlyDictionary<TKey,TValue>
#endif
#if NET35 || NET40 || NET45 || SERIALIZE
        , ISerializable
        , IDeserializationCallback
#endif
    {
#if NET35 || NET40 || NET45 || SERIALIZE
        [NonSerialized]
#endif
        private KeyCollection keys;
#if NET35 || NET40 || NET45 || SERIALIZE
        [NonSerialized]
#endif
        private ValueCollection values;

        #region Constructors

        /// <summary>Initializes a new dictionary of key/value pairs that are sorted on unique keys using the default key comparer.</summary>
        public RankedDictionary() : base (Comparer<TKey>.Default, new PairLeaf())
        { }


        /// <summary>Initializes a new dictionary of key/value pairs that are sorted on unique keys using the supplied key comparer.</summary>
        /// <param name="comparer">Comparison operator for keys.</param>
        /// <remarks>
        /// <para>Every key in a dictionary must be unique according to the comparer.</para>
        /// <para>
        /// This class requires a IComparer implementation to perform key comparisons.
        /// If <em>comparer</em> is <b>null</b>, the default comparer for the type will be used.
        /// If the key type implements the IComparable&lt;T&gt; interface, the default comparer uses that implementation.
        /// If no comparison implementation is available, the Add method will fail on the second element.
        /// </para>
        /// </remarks>
        /// <example>
        /// <para>
        /// This program shows usage of a provided comparer combined with serialization.
        /// </para>
        /// <para>Note: Serialization is not supported in .NET Standard 1.0.</para>
        /// <code source="..\Bench\RdExample05\RdExample05.cs" lang="cs" />
        /// </example>
        /// <exception cref="InvalidOperationException">When <em>comparer</em> is <b>null</b> and no other comparer available.</exception>
        public RankedDictionary (IComparer<TKey> comparer) : base (comparer, new PairLeaf())
        { }

        /// <summary>Initializes a new dictionary that contains key/value pairs copied from the supplied dictionary and sorted by the default comparer.</summary>
        /// <param name="dictionary">The dictionary to be copied.</param>
        /// <exception cref="ArgumentNullException">When <em>dictionary</em> is <b>null</b>.</exception>
        public RankedDictionary (IDictionary<TKey,TValue> dictionary) : this (dictionary, Comparer<TKey>.Default)
        { }

        /// <summary>Initializes a new dictionary that contains key/value pairs copied from the supplied dictionary and sorted by the supplied comparer.</summary>
        /// <param name="dictionary">The dictionary to be copied.</param>
        /// <param name="comparer">Comparison operator for keys.</param>
        /// <exception cref="ArgumentNullException">When <em>dictionary</em> is <b>null</b>.</exception>
        /// <exception cref="InvalidOperationException">When <em>comparer</em> is <b>null</b> and no other comparer available.</exception>
        public RankedDictionary (IDictionary<TKey,TValue> dictionary, IComparer<TKey> comparer) : this (comparer)
        {
            if (dictionary == null)
                throw new ArgumentNullException (nameof (dictionary));

            foreach (KeyValuePair<TKey,TValue> pair in dictionary)
                Add (pair.Key, pair.Value);
        }

        #endregion

        #region Properties

        /// <summary>Gets or sets the value associated with the supplied key.</summary>
        /// <param name="key">The key of the value to get or set.</param>
        /// <returns>Value associated with the supplied key.</returns>
        /// <remarks>
        /// <para>
        /// Setting a value for a non-existent key performs an add operation.
        /// To get a value for a supplied index, use the <see cref="ElementAt"/> method.
        /// </para>
        /// <para>This is a O(log <em>n</em>) operation for both getting and setting.</para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">When <em>key</em> is <b>null</b>.</exception>
        /// <exception cref="KeyNotFoundException">When getting a value and <em>key</em> was not found.</exception>
        public TValue this[TKey key]
        {
            get
            {
                if (key == null)
                    throw new ArgumentNullException (nameof (key));

                var leaf = (PairLeaf) Find (key, out int index);
                if (index < 0)
                    throw new KeyNotFoundException ("The given key was not present in the dictionary.");
                return leaf.GetValue (index);
            }
            set
            {
                if (key == null)
                    throw new ArgumentNullException (nameof (key));

                StageBump();
                var path = new NodeVector (this, key);
                if (path.IsFound)
                    ((PairLeaf) path.TopNode).SetValue (path.TopIndex, value);
                else
                    Add2 (path, key, value);
            }
        }

        /// <summary>Returns a wrapper of the method used to order elements in the dictionary.</summary>
        /// <remarks>
        /// To override sorting based on the default comparer,
        /// supply an alternate comparer when constructing the dictionary.
        /// </remarks>
        public IComparer<TKey> Comparer => keyComparer;

        /// <summary>Gets the number of elements in the dictionary.</summary>
        /// <remarks>This is a O(1) operation.</remarks>
        public int Count => root.Weight;

        /// <summary>Gets only the collection of keys from this dictionary.</summary>
        /// <remarks>The keys given by this collection are sorted according to the
        /// <see cref="Comparer"/> property.</remarks>
        /// <example>
        /// This trivial example shows how to enumerate the keys of a dictionary.
        /// <code source="..\Bench\RdExample02\RdExample02.cs" lang="cs" />
        /// </example>
        public KeyCollection Keys
        {
            get
            {
                if (keys == null)
                    keys = new KeyCollection (this);
                return keys;
            }
        }

        /// <summary>Gets only the collection of keys from this key/value pair collection.</summary>
        /// <remarks>The keys given by this collection are sorted according to the
        /// <see cref="Comparer"/> property.</remarks>
        ICollection<TKey> IDictionary<TKey,TValue>.Keys => (ICollection<TKey>) Keys;

#if ! NET35 && ! NET40
        /// <summary>Gets a collection containing the keys in the dictionary.</summary>
        IEnumerable<TKey> IReadOnlyDictionary<TKey,TValue>.Keys => Keys;

        /// <summary>Gets a collection containing the values in the dictionary.</summary>
        IEnumerable<TValue> IReadOnlyDictionary<TKey,TValue>.Values => Values;
#endif

        /// <summary>Gets only the collection of values from this dictionary.</summary>
        /// <remarks>The values given by this collection are sorted in the same
        /// order as their respective keys in the <see cref="Keys"/> collection.</remarks>
        /// <example>
        /// This trivial example shows how to enumerate the values of a dictionary.
        /// <code source="..\Bench\RdExample02\RdExample02.cs" lang="cs" />
        /// </example>
        public ValueCollection Values
        {
            get
            {
                if (values == null)
                    values = new ValueCollection (this);
                return values;
            }
        }

        /// <summary>Gets only the collection of values from this key/value pair collection.</summary>
        /// <remarks>The values given by this collection are sorted in the same
        /// order as their respective keys in the <see cref="Keys"/> collection.</remarks>
        ICollection<TValue> IDictionary<TKey,TValue>.Values => (ICollection<TValue>) Values;

        /// <summary>Gets the maximum key in the dictionary per the comparer.</summary>
        /// <remarks>This is a O(1) operation.</remarks>
        public TKey MaxKey => Count==0 ? default (TKey) : rightmostLeaf.GetKey (rightmostLeaf.KeyCount-1);

        /// <summary>Gets the minimum key in the dictionary per the comparer.</summary>
        /// <remarks>This is a O(1) operation.</remarks>
        public TKey MinKey => Count==0 ? default (TKey) : leftmostLeaf.Key0;

        /// <summary>Indicates that this collection may be modified.</summary>
        bool ICollection<KeyValuePair<TKey,TValue>>.IsReadOnly => false;

        #endregion

        #region Methods

        /// <summary>Adds an element with the supplied key and value.</summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add. May be null.</param>
        /// <remarks>
        /// <para>
        /// If <em>key</em> is already in the dictionary, this method takes no action.
        /// </para>
        /// <para>This is a O(log <em>n</em>) operation.</para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">When <em>key</em> is <b>null</b>.</exception>
        /// <exception cref="ArgumentException">
        ///   When a key/value pair already exists with the supplied key;
        ///   when no comparer is available.
        /// </exception>
        public void Add (TKey key, TValue value)
        {
            if (key == null)
                throw new ArgumentNullException (nameof (key));

            var path = new NodeVector (this, key);
            if (path.IsFound)
                throw new ArgumentException ("An entry with the same key already exists.", nameof (key));

            Add2 (path, key, value);
        }

        private void Add2 (NodeVector path, TKey key, TValue value)
        {
            StageBump();

            var leaf = (PairLeaf) path.TopNode;
            int pathIndex = path.TopIndex;

            path.IncrementPathWeight();
            if (leaf.KeyCount < maxKeyCount)
            {
                leaf.Insert (pathIndex, key, value);
                return;
            }

            // Leaf is full so right split a new leaf.
            var newLeaf = new PairLeaf (leaf, maxKeyCount);

            if (newLeaf.rightLeaf != null)
                newLeaf.rightLeaf.leftLeaf = newLeaf;
            else
            {
                rightmostLeaf = newLeaf;

                if (pathIndex == leaf.KeyCount)
                {
                    newLeaf.Add (key, value);
                    path.Promote (key, (Node) newLeaf, true);
                    return;
                }
            }

            int splitIndex = leaf.KeyCount / 2 + 1;
            if (pathIndex < splitIndex)
            {
                // Left-side insert: Copy right side to the split leaf.
                newLeaf.Add (leaf, splitIndex - 1, leaf.KeyCount);
                leaf.Truncate (splitIndex - 1);
                leaf.Insert (pathIndex, key, value);
            }
            else
            {
                // Right-side insert: Copy split leaf parts and new key.
                newLeaf.Add (leaf, splitIndex, pathIndex);
                newLeaf.Add (key, value);
                newLeaf.Add (leaf, pathIndex, leaf.KeyCount);
                leaf.Truncate (splitIndex);
            }

            // Promote anchor of split leaf.
            path.Promote (newLeaf.Key0, (Node) newLeaf, newLeaf.rightLeaf == null);
        }


        /// <summary>Adds an element with the supplied key/value pair.</summary>
        /// <param name="keyValuePair">Contains the key and value of the element to add.</param>
        /// <exception cref="ArgumentException">When an element containing <em>key</em> has already been added.</exception>
        void ICollection<KeyValuePair<TKey,TValue>>.Add (KeyValuePair<TKey,TValue> keyValuePair)
        {
            var path = new NodeVector (this, keyValuePair.Key);
            if (path.IsFound)
                throw new ArgumentException ("An entry with the same key already exists.", nameof (keyValuePair));

            Add2 (path, keyValuePair.Key, keyValuePair.Value);
        }


        /// <summary>Removes all elements from the dictionary.</summary>
        /// <remarks>This is a O(1) operation.</remarks>
        public void Clear() => Initialize();


        /// <summary>Determines if the dictionary contains the supplied key.</summary>
        /// <param name="key">The key to locate in the dictionary.</param>
        /// <returns><b>true</b> if the dictionary contains <em>key</em>; otherwise <b>false</b>.</returns>
        /// <exception cref="ArgumentNullException">When the supplied key is <b>null</b>.</exception>
        public bool ContainsKey (TKey key)
        {
            if (key == null)
                throw new ArgumentNullException (nameof (key));

            var leaf = (PairLeaf) Find (key, out int index);
            return index >= 0;
        }


        /// <summary>Determines if the dictionary contains the supplied value.</summary>
        /// <remarks>This operation performs a sequential search.</remarks>
        /// <param name="value">Value to find.</param>
        /// <returns><b>true</b> if the collection contains the supplied value; otherwise <b>false</b>.</returns>
        public bool ContainsValue (TValue value)
        {
            if (value != null)
            {
                var comparer = EqualityComparer<TValue>.Default;
                for (var leaf = (PairLeaf) leftmostLeaf; leaf != null; leaf = (PairLeaf) leaf.rightLeaf)
                    for (int vix = 0; vix < leaf.ValueCount; ++vix)
                        if (comparer.Equals (leaf.GetValue (vix), value))
                            return true;
            }
            else
                for (var leaf = (PairLeaf) leftmostLeaf; leaf != null; leaf = (PairLeaf) leaf.rightLeaf)
                    for (int vix = 0; vix < leaf.ValueCount; ++vix)
                        if (leaf.GetValue (vix) == null)
                            return true;

            return false;
        }


        /// <summary>Determines if the collection contains the supplied key/value pair.</summary>
        /// <param name="keyValuePair">Key/value pair to find.</param>
        /// <returns><b>true</b> if the collection contains the supplied key/value pair; otherwise <b>false</b>.</returns>
        bool ICollection<KeyValuePair<TKey,TValue>>.Contains (KeyValuePair<TKey,TValue> keyValuePair)
        {
            var leaf = (PairLeaf) Find (keyValuePair.Key, out int index);
            if (index < 0)
                return false;
            return EqualityComparer<TValue>.Default.Equals (leaf.GetValue (index), keyValuePair.Value);
        }


        /// <summary>Copies the dictionary to a compatible array, starting at the supplied position.</summary>
        /// <param name="array">A one-dimensional array that is the destination of the copy.</param>
        /// <param name="index">The zero-based starting position in <em>array</em>.</param>
        /// <exception cref="ArgumentNullException">When <em>array</em> is <b>null</b>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">When <em>index</em> is less than zero.</exception>
        /// <exception cref="ArgumentException">When not enough space is given for the copy.</exception>
        public void CopyTo (KeyValuePair<TKey,TValue>[] array, int index)
        {
            if (array == null)
                throw new ArgumentNullException (nameof (array));

            if (index < 0)
                throw new ArgumentOutOfRangeException (nameof (index), "Index is less than zero.");

            if (Count > array.Length - index)
                throw new ArgumentException ("Destination array is not long enough to copy all the items in the collection. Check array index and length.");

            for (var leaf = (PairLeaf) leftmostLeaf; leaf != null; leaf = (PairLeaf) leaf.rightLeaf)
                for (int leafIndex = 0; leafIndex < leaf.KeyCount; ++leafIndex)
                    array[index++] = new KeyValuePair<TKey,TValue> (leaf.GetKey (leafIndex), leaf.GetValue (leafIndex));
        }


        /// <summary>Removes the element with the supplied key from the dictionary.</summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns><b>true</b> if the element was successfully found and removed; otherwise <b>false</b>.</returns>
        /// <remarks>This is a O(log <em>n</em>) operation.</remarks>
        /// <exception cref="ArgumentNullException">When <em>key</em> is <b>null</b>.</exception>
        public bool Remove (TKey key)
        {
            if (key == null)
                throw new ArgumentNullException (nameof (key));

            var path = new NodeVector (this, key);
            if (! path.IsFound)
                return false;

            Remove2 (path);
            return true;
        }

        /// <summary>Deletes the supplied key and its associated value from the collection.</summary>
        /// <param name="keyValuePair">Contains key and value to find and remove.</param>
        /// <returns><b>true</b> if key/value pair removed; otherwise <b>false</b>.</returns>
        /// <remarks>No operation is taken unless both key and value match.</remarks>
        bool ICollection<KeyValuePair<TKey,TValue>>.Remove (KeyValuePair<TKey,TValue> keyValuePair)
        {
            var path = new NodeVector (this, keyValuePair.Key);
            if (path.IsFound)
                if (EqualityComparer<TValue>.Default.Equals (keyValuePair.Value, ((PairLeaf) path.TopNode).GetValue (path.TopIndex)))
                {
                    Remove2 (path);
                    return true;
                }

            return false;
        }


        /// <summary>Removes the element at the supplied index.</summary>
        /// <param name="index">The zero-based position of the element to remove.</param>
        /// <remarks>This is a O(log <em>n</em>) operation.</remarks>
        /// <exception cref="ArgumentOutOfRangeException">When <em>index</em> is less than zero or greater than or equal to the number of elements.</exception>
        public void RemoveAt (int index)
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException (nameof (index), "Argument is out of the range of valid values.");

            var path = NodeVector.CreateForIndex (this, index);
            Remove2 (path);
        }


        /// <summary>Removes all elements that match the condition defined by the supplied predicate.</summary>
        /// <param name="match">The condition of the items to remove.</param>
        /// <returns>The number of items removed from the dictionary.</returns>
        /// <remarks>
        /// This is a O(<em>n</em> log <em>m</em>) operation
        /// where <em>m</em> is the count of items removed and <em>n</em> is the size of the dictionary.
        /// </remarks>
        /// <exception cref="ArgumentNullException">When <em>match</em> is <b>null</b>.</exception>
        public int RemoveWhere (Predicate<TKey> match)
        {
            return RemoveWhere2 (match);
        }


        /// <summary>Gets the value associated with the supplied key.</summary>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">
        /// If the key is found, its value is placed here;
        /// otherwise it will be loaded with the default value for its type.
        /// </param>
        /// <returns><b>true</b> if <em>key</em> is found; otherwise <b>false</b>.</returns>
        /// <remarks>This is a O(log <em>n</em>) operation.</remarks>
        /// <exception cref="ArgumentNullException">When <em>key</em> is <b>null</b>.</exception>
        public bool TryGetValue (TKey key, out TValue value)
        {
            if (key == null)
                throw new ArgumentNullException (nameof (key));

            var leaf = (PairLeaf) Find (key, out int index);
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

        #region Explicit object properties interface implementations

        /// <summary>Gets or sets the value associated with the supplied key.</summary>
        /// <exception cref="ArgumentNullException">When <em>key</em> is <b>null</b>.</exception>
        object IDictionary.this[object key]
        {
            get
            {
                if (key == null)
                    throw new ArgumentNullException (nameof (key));

                if (key is TKey)
                {
                    var leaf = (PairLeaf) Find ((TKey) key, out int index);
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
                    StageBump();
                    var path = new NodeVector (this, (TKey) key);
                    if (path.IsFound)
                        ((PairLeaf) path.TopNode).SetValue (path.TopIndex, (TValue) value);
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

        /// <summary>Indicating that the dictionary is not fixed size.</summary>
        bool IDictionary.IsFixedSize => false;

        /// <summary>Indicates that the dictionary may be modified.</summary>
        bool IDictionary.IsReadOnly => false;

        /// <summary>Indicates that the collection is not thread safe.</summary>
        bool ICollection.IsSynchronized => false;

        /// <summary>Gets an <see cref="ICollection"/> containing the keys of the dictionary.</summary>
        ICollection IDictionary.Keys => (ICollection) Keys;

        /// <summary>Gets an <see cref="ICollection"/> containing the values of the dictionary.</summary>
        ICollection IDictionary.Values => (ICollection) Values;

        #endregion

        #region Explicit object methods interface implementations

        /// <summary>Adds the supplied key and value to the dictionary.</summary>
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

            ((IDictionary<TKey,TValue>) this).Add ((TKey) key, (TValue) value);
        }


        /// <summary>Determines whether the dictionary contains a key/value pair with the supplied key.</summary>
        /// <param name="key">The key to locate in the dictionary.</param>
        /// <returns><b>true</b> if the dictionary contains <em>key</em>; otherwise <b>false</b>.</returns>
        /// <exception cref="ArgumentNullException">When <em>key</em> is <b>null</b>.</exception>
        bool IDictionary.Contains (object key)
        {
            if (key == null)
                throw new ArgumentNullException (nameof (key));

            if (! (key is TKey))
                return false;

            Leaf leaf = Find ((TKey) key, out int ix);
            return ix >= 0;
        }


        /// <summary>Copies the elements of the dictionary to an array, starting at the supplied array index.</summary>
        /// <param name="array">The destination array of the copy.</param>
        /// <param name="index">The zero-based index in <em>array</em> at which copying begins.</param>
        /// <exception cref="ArgumentNullException">When <em>array</em> is <b>null</b>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">When <em>index</em> is less than zero.</exception>
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
                throw new ArgumentOutOfRangeException (nameof (index), "Index is less than zero.");

            if (Count > array.Length - index)
                throw new ArgumentException ("Destination array is not long enough to copy all the items in the collection. Check array index and length.", nameof (array));

            if (! (array is KeyValuePair<TKey,TValue>[]) && array.GetType() != typeof (Object[]))
                throw new ArgumentException ("Target array type is not compatible with the type of items in the collection.", nameof (array));

            for (var leaf = (PairLeaf) leftmostLeaf; leaf != null; leaf = (PairLeaf) leaf.rightLeaf)
                for (int leafIndex = 0; leafIndex < leaf.KeyCount; ++leafIndex)
                {
                    array.SetValue (new KeyValuePair<TKey,TValue>(leaf.GetKey (leafIndex), leaf.GetValue (leafIndex)), index);
                    ++index;
                }
        }


        /// <summary>Removes the supplied key and its associated value from the collection.</summary>
        /// <param name="key">Key to remove.</param>
        /// <exception cref="ArgumentNullException">When <em>key</em> is <b>null</b>.</exception>
        void IDictionary.Remove (object key)
        {
            if (key == null)
                throw new ArgumentNullException (nameof (key));

            if (key is TKey tKey)
            {
                var path = new NodeVector (this, tKey);
                if (path.IsFound)
                    Remove2 (path);
            }
        }


        /// <summary>Gets an object that can be used to synchronize access to the collection.</summary>
        object ICollection.SyncRoot => GetSyncRoot();

        #endregion

        #region ISerializable implementation and support
#if NET35 || NET40 || NET45 || SERIALIZE

        private SerializationInfo serializationInfo;

        /// <summary>Initializes a new instance of the dictionary that contains serialized data.</summary>
        /// <param name="info">The object that contains the information required to serialize the dictionary.</param>
        /// <param name="context">The structure that contains the source and destination of the serialized stream.</param>
        protected RankedDictionary (SerializationInfo info, StreamingContext context) : base (new PairLeaf())
        {
            this.serializationInfo = info;
        }

        /// <summary>Returns the data needed to serialize the dictionary.</summary>
        /// <param name="info">An object that contains the information required to serialize the dictionary.</param>
        /// <param name="context">A structure that contains the source and destination of the serialized stream.</param>
        /// <exception cref="ArgumentNullException">When <em>info</em> is <b>null</b>.</exception>
        protected virtual void GetObjectData (SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException (nameof (info));

            info.AddValue ("Count", Count);
            info.AddValue ("Comparer", Comparer, typeof (IComparer<TKey>));
            info.AddValue ("Stage", stage);

            var keys = new TKey[Count];
            Keys.CopyTo (keys, 0);
            info.AddValue ("Keys", keys, typeof (TKey[]));

            var values = new TValue[Count];
            Values.CopyTo (values, 0);
            info.AddValue ("Values", values, typeof (TValue[]));
        }


        /// <summary>Implements the deserialization callback and raises the deserialization event when completed.</summary>
        /// <param name="sender">The source of the deserialization event.</param>
        /// <exception cref="ArgumentNullException">When <em>sender</em> is <b>null</b>.</exception>
        /// <exception cref="SerializationException">When the associated <em>SerializationInfo</em> is invalid.</exception>
        protected virtual void OnDeserialization (object sender)
        {
            if (keyComparer != null)
                return;  // Owner did the fixups.

            if (serializationInfo == null)
                throw new SerializationException ("Missing information.");

            keyComparer = (IComparer<TKey>) serializationInfo.GetValue ("Comparer", typeof (IComparer<TKey>));
            int storedCount = serializationInfo.GetInt32 ("Count");
            stage = serializationInfo.GetInt32 ("Stage");

            if (storedCount != 0)
            {
                var keys = (TKey[]) serializationInfo.GetValue ("Keys", typeof (TKey[]));
                if (keys == null)
                    throw new SerializationException ("Missing Keys.");

                var values = (TValue[]) serializationInfo.GetValue ("Values", typeof (TValue[]));
                if (keys == null)
                    throw new SerializationException ("Missing Values.");

                if (keys.Length != values.Length)
                    throw new SerializationException ("Mismatched key/value count.");

                for (int ix = 0; ix < keys.Length; ++ix)
                    Add (keys[ix], values[ix]);

                if (storedCount != keys.Length)
                    throw new SerializationException ("Mismatched count.");
            }

            serializationInfo = null;
        }


        /// <summary>Returns the data needed to serialize the dictionary.</summary>
        /// <param name="info">An object that contains the information required to serialize the dictionary.</param>
        /// <param name="context">A structure that contains the source and destination of the serialized stream.</param>
        /// <exception cref="ArgumentNullException">When <em>info</em> is <b>null</b>.</exception>
        void ISerializable.GetObjectData (SerializationInfo info, StreamingContext context)
        { GetObjectData(info, context); }


        /// <summary>Implements the deserialization callback and raises the deserialization event when completed.</summary>
        /// <param name="sender">The source of the deserialization event.</param>
        /// <exception cref="ArgumentNullException">When <em>sender</em> is <b>null</b>.</exception>
        /// <exception cref="SerializationException">When the associated <em>SerializationInfo</em> is invalid.</exception>
        void IDeserializationCallback.OnDeserialization (Object sender)
        { OnDeserialization (sender); }

#endif
        #endregion

        #region Bonus methods

        /// <summary>Returns an enumerator that iterates over a range with the supplied bounds.</summary>
        /// <param name="lower">Minimum key of the range.</param>
        /// <param name="upper">Maximum key of the range.</param>
        /// <returns>An enumerator for the specified range.</returns>
        /// <remarks>
        /// <para>
        /// If either <em>lower</em> or <em>upper</em> are present in the dictionary,
        /// they will be included in the results.
        /// </para>
        /// <para>
        /// Retrieving the first element is a O(log <em>n</em>) operation.
        /// Retrieving subsequent elements is a O(1) operation per element.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code source="..\Bench\RdExample03\RdExample03.cs" lang="cs" />
        /// </example>
        public IEnumerable<KeyValuePair<TKey,TValue>> ElementsBetween (TKey lower, TKey upper)
        {
            int stageFreeze = stage;
            var leaf = (PairLeaf) Find (lower, out int index);

            // When the supplied start key is not be found, start with the next highest key.
            if (index < 0)
                index = ~index;

            for (;;)
            {
                if (index < leaf.KeyCount)
                {
                    if (Comparer.Compare (leaf.GetKey (index), upper) > 0)
                        yield break;

                    yield return leaf.GetPair (index);
                    StageCheck (stageFreeze);
                    ++index;
                    continue;
                }

                leaf = (PairLeaf) leaf.rightLeaf;
                if (leaf == null)
                    yield break;

                index = 0;
            }
        }


        /// <summary>Returns an enumerator that iterates over a range with the supplied lower bound.</summary>
        /// <param name="lower">Minimum key of the range.</param>
        /// <returns>An enumerator for the specified range.</returns>
        /// <remarks>
        /// <para>
        /// If <em>lower</em> is present in the dictionary, it will be included in the results.
        /// </para>
        /// <para>
        /// Retrieving the initial item is a O(log <em>n</em>) operation.
        /// Retrieving each subsequent item is a O(1) operation.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">When <em>key</em> is <b>null</b>.</exception>
        public IEnumerable<KeyValuePair<TKey,TValue>> ElementsFrom (TKey lower)
        {
            if (lower == null)
                throw new ArgumentNullException (nameof (lower));

            int stageFreeze = stage;
            var leaf = (PairLeaf) Find (lower, out int index);

            // When the supplied start key is not be found, start with the next highest key.
            if (index < 0)
                index = ~index;

            for (;;)
            {
                if (index < leaf.KeyCount)
                {
                    yield return leaf.GetPair (index);
                    StageCheck (stageFreeze);
                    ++index;
                    continue;
                }

                leaf = (PairLeaf) leaf.rightLeaf;
                if (leaf == null)
                    yield break;

                index = 0;
            }
        }


        /// <summary>Gets the index of the element with the supplied key.</summary>
        /// <param name="key">The key of the element to seek.</param>
        /// <returns>The index of the element with the supplied key if found; otherwise the bitwise complement of the insert point.</returns>
        /// <remarks>This is a O(log <em>n</em>) operation.</remarks>
        /// <exception cref="ArgumentNullException">When <em>key</em> is <b>null</b>.</exception>
        public int IndexOfKey (TKey key)
        {
            if (key == null)
                throw new ArgumentNullException (nameof (key));

            return FindEdgeForIndex (key, out Leaf leaf, out int leafIndex, leftEdge:true);
        }


        /// <summary>Gets the index of the first element with the supplied value.</summary>
        /// <param name="value">The value of the element to seek.</param>
        /// <returns>The index of the element if found; otherwise -1.</returns>
        /// <remarks>This is a O(<em>n</em>) operation.</remarks>
        public int IndexOfValue (TValue value)
        {
            int result = 0;
            for (var leaf = (PairLeaf) leftmostLeaf; leaf != null; leaf = (PairLeaf) leaf.rightLeaf)
            {
                var ix = leaf.IndexOfValue (value);
                if (ix >= 0)
                    return result + ix;
                result += leaf.KeyCount;
            }

            return -1;
        }


        /// <summary>Gets the value and index associated with the supplied key.</summary>
        /// <param name="key">The key of the value and index to get.</param>
        /// <param name="value">If the key is found, its value is placed here; otherwise it will be loaded with the default value.</param>
        /// <param name="index">If the key is found, its index is placed here; otherwise it will be less than zero.</param>
        /// <returns><b>true</b> if supplied key is found; otherwise <b>false</b>.</returns>
        /// <remarks>This is a O(log <em>n</em>) operation.</remarks>
        public bool TryGetValueAndIndex (TKey key, out TValue value, out int index)
        {
            index = FindEdgeForIndex (key, out Leaf leaf, out int leafIx, leftEdge:true);
            if (index < 0)
            {
                value = default (TValue);
                return false;
            }

            if (leafIx >= leaf.KeyCount)
            {
                leaf = leaf.rightLeaf;
                leafIx = 0;
            }

            value = ((PairLeaf) leaf).GetValue (leafIx);
            return true;
        }

        #endregion

        #region LINQ instance implementation

        /// <summary>Gets the key/value pair at the supplied index.</summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The element at the supplied index.</returns>
        /// <remarks>This is a O(log <em>n</em>) operation.</remarks>
        /// <exception cref="ArgumentOutOfRangeException">When <em>index</em> is less than zero or greater than or equal to the number of keys.</exception>
        public KeyValuePair<TKey,TValue> ElementAt (int index)
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException (nameof (index), "Argument is out of the range of valid values.");

            var leaf = (PairLeaf) Find (ref index);
            return new KeyValuePair<TKey,TValue> (leaf.GetKey (index), leaf.GetValue (index));
        }


        /// <summary>Gets the key/value pair at the supplied index or the default if index is out of range.</summary>
        /// <param name="index">The zero-based index of the item to get.</param>
        /// <returns>The element at the supplied index.</returns>
        /// <remarks>This is a O(log <em>n</em>) operation.</remarks>
        public KeyValuePair<TKey,TValue> ElementAtOrDefault (int index)
        {
            if (index < 0 || index >= Count)
                return new KeyValuePair<TKey, TValue> (default (TKey), default (TValue));

            var leaf = (PairLeaf) Find (ref index);
            return new KeyValuePair<TKey,TValue> (leaf.GetKey (index), leaf.GetValue (index));
        }


        /// <summary>Gets the last key/value pair.</summary>
        /// <returns>The key/value pair with maximum key in dictionary.</returns>
        /// <remarks>This is a O(1) operation.</remarks>
        /// <exception cref="InvalidOperationException">When the collection is empty.</exception>
        public KeyValuePair<TKey,TValue> Last()
        {
            if (Count == 0)
                throw new InvalidOperationException ("Sequence contains no elements.");

            int ix = rightmostLeaf.KeyCount-1;
            return new KeyValuePair<TKey,TValue> (rightmostLeaf.GetKey (ix), ((PairLeaf) rightmostLeaf).GetValue (ix));
        }


        /// <summary>Returns an enumerator that iterates thru the dictionary in reverse order.</summary>
        /// <returns>An enumerator that reverse iterates thru the dictionary.</returns>
        public IEnumerable<KeyValuePair<TKey,TValue>> Reverse()
        {
            Enumerator etor = new Enumerator (this, isReverse:true);
            while (etor.MoveNext())
                yield return etor.Current;
        }

        #endregion

        #region Enumeration

        /// <summary>Gets an enumerator that iterates thru the dictionary.</summary>
        /// <returns>An enumerator for the dictionary.</returns>
        public Enumerator GetEnumerator() => new Enumerator (this);

        /// <summary>Gets an enumerator that iterates thru the dictionary.</summary>
        /// <returns>An enumerator for the dictionary.</returns>
        IEnumerator<KeyValuePair<TKey,TValue>> IEnumerable<KeyValuePair<TKey,TValue>>.GetEnumerator()
            => new Enumerator (this);

        /// <summary>Gets an enumerator that iterates thru the collection.</summary>
        /// <returns>An enumerator for the collection.</returns>
        IDictionaryEnumerator IDictionary.GetEnumerator()
            => new Enumerator (this, isReverse:false, nonGeneric:true);

        /// <summary>Gets an enumerator that iterates thru the collection.</summary>
        /// <returns>An enumerator for the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator() => new Enumerator (this);


        /// <summary>Enumerates the sorted key/value pairs of a <see cref="RankedDictionary{TKey,TValue}"/>.</summary>
        public sealed class Enumerator : IEnumerator<KeyValuePair<TKey,TValue>>, IDictionaryEnumerator
        {
            private readonly RankedDictionary<TKey,TValue> tree;
            private readonly bool isReverse;
            private readonly bool nonGeneric;
            private PairLeaf leaf;
            private int index;
            private int stageFreeze;
            private int state;  // -1=rewound; 0=active; 1=consumed

            /// <summary>Make an iterator that will loop thru the collection in order.</summary>
            /// <param name="dictionary">Collection containing these key/value pairs.</param>
            /// <param name="isReverse">Supply <b>true</b> to iterate from last to first.</param>
            /// <param name="nonGeneric">Supply <b>true</b> to indicate object Current should return DictionaryEntry values.</param>
            internal Enumerator (RankedDictionary<TKey,TValue> dictionary, bool isReverse=false, bool nonGeneric=false)
            {
                this.tree = dictionary;
                this.isReverse = isReverse;
                this.nonGeneric = nonGeneric;
                ((IEnumerator) this).Reset();
            }

            /// <summary>Gets the key of the element at the current position.</summary>
            object IDictionaryEnumerator.Key
            {
                get
                {
                    if (state != 0)
                        throw new InvalidOperationException ("Enumeration is not active.");
                    return leaf.GetKey (index);
                }
            }

            /// <summary>Gets the value of the element at the current position.</summary>
            object IDictionaryEnumerator.Value
            {
                get
                {
                    if (state != 0)
                        throw new InvalidOperationException ("Enumeration is not active.");
                    return leaf.GetValue (index);
                }
            }

            /// <summary>Gets the element at the current position as a <see cref="DictionaryEntry" />.</summary>
            DictionaryEntry IDictionaryEnumerator.Entry
            {
                get
                {
                    if (state != 0)
                        throw new InvalidOperationException ("Enumeration is not active.");
                    return new DictionaryEntry (leaf.GetKey (index), leaf.GetValue (index));
                }
            }

            /// <summary>Gets the element at the current position.</summary>
            object IEnumerator.Current
            {
                get
                {
                    tree.StageCheck (stageFreeze);
                    if (state != 0)
                        throw new InvalidOperationException ("Enumeration is not active.");

                    if (nonGeneric)
                        return new DictionaryEntry (leaf.GetKey (index), leaf.GetValue (index));
                    else
                        return leaf.GetPair (index);
                }
            }

            /// <summary>Gets the key/value pair at the current position.</summary>
            /// <exception cref="InvalidOperationException">When the dictionary was modified after the enumerator was created.</exception>
            public KeyValuePair<TKey,TValue> Current
            {
                get
                {
                    tree.StageCheck (stageFreeze);
                    return state == 0 ? leaf.GetPair (index)
                                      : new KeyValuePair<TKey,TValue> (default (TKey), default (TValue));
                }
            }

            /// <summary>Advances the enumerator to the next element in the dictionary.</summary>
            /// <returns><b>true</b> if the enumerator was successfully advanced to the next element; <b>false</b> if the enumerator has passed the end of the collection.</returns>
            /// <exception cref="InvalidOperationException">When the dictionary was modified after the enumerator was created.</exception>
            public bool MoveNext()
            {
                tree.StageCheck (stageFreeze);

                if (state != 0)
                    if (state > 0)
                        return false;
                    else
                    {
                        leaf = (PairLeaf) (isReverse ? tree.rightmostLeaf : tree.leftmostLeaf);
                        index = isReverse ? leaf.KeyCount : -1;
                        state = 0;
                    }

                if (isReverse)
                {
                    if (--index >= 0)
                        return true;

                    leaf = (PairLeaf) leaf.leftLeaf;
                    if (leaf != null)
                    { index = leaf.KeyCount - 1; return true; }
                }
                else
                {
                    if (++index < leaf.KeyCount)
                        return true;

                    leaf = (PairLeaf) leaf.rightLeaf;
                    if (leaf != null)
                    { index = 0; return true; }
                }

                state = 1;
                return false;
            }

            /// <summary>Rewinds the enumerator to its initial state.</summary>
            void IEnumerator.Reset()
            {
                stageFreeze = tree.stage;
                state = -1;
            }

            /// <summary>Releases all resources used by the enumerator.</summary>
            public void Dispose() { }
        }

        #endregion
    }
}
