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

namespace Kaos.Collections
{
    /// <summary>Represents a collection of key/value pairs that are sorted on unique keys.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    /// <remarks>
    /// This class emulates and extends the
    /// <see cref="System.Collections.Generic.SortedDictionary{TKey,TValue}"/> class
    /// with the addition of the methods
    /// <see cref="GetByIndex"/>, <see cref="IndexOf"/>, <see cref="TryGetValueAndIndex"/>,
    /// <see cref="GetBetween"/>, <see cref="GetFrom"/>, and <see cref="Last"/>.
    /// </remarks>
    [DebuggerTypeProxy (typeof (IDictionaryDebugView<,>))]
    [DebuggerDisplay ("Count = {Count}")]
    public sealed partial class RankedDictionary<TKey,TValue> :
        Btree<TKey>
        , IDictionary<TKey,TValue>
        , IDictionary
#if ! NET35 && ! NET40
        , IReadOnlyDictionary<TKey,TValue>
#endif
    {
        private KeyCollection keys;
        private ValueCollection values;

        #region Constructors

        /// <summary>Initializes a new dictionary of key/value pairs that are sorted on unique keys using the default key comparer.</summary>
        public RankedDictionary() : base (Comparer<TKey>.Default, new PairLeaf())
        { }

        /// <summary>Initializes a new dictionary of key/value pairs that are sorted on unique keys using the supplied key comparer.</summary>
        /// <param name="comparer">Comparison operator for keys.</param>
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
        /// To get a value for a supplied index, use the <see cref="GetByIndex"/> method.
        /// </para>
        /// <para>This is a O(log <em>n</em>) operation.</para>
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
                    PairLeaf.SetValue (path, value);
                else
                    Add2 (path, key, value);
            }
        }


        /// <summary>Gets only the collection of keys from this dictionary.</summary>
        /// <remarks>The keys given by this collection are sorted according to the
        /// <see cref="Btree{TKey}.Comparer"/> property.</remarks>
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

        #endregion

        #region Methods

        /// <summary>Adds an element with the supplied key and value.</summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.  May be null.</param>
        /// <remarks>
        /// <para>
        /// If <em>key</em> is already in the dictionary, this method takes no action.
        /// </para>
        /// <para>This is a O(log <em>n</em>) operation.</para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">When <em>key</em> is <b>null</b>.</exception>
        /// <exception cref="ArgumentException">When a key/value pair has already been added with the supplied key.</exception>
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
            int pathIndex = path.TopNodeIndex;

            path.UpdateWeight (1);
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


        /// <summary>Determines if the collection contains the supplied key.</summary>
        /// <param name="key">Key to find.</param>
        /// <returns><b>true</b> if the collection contains the supplied key; otherwise <b>false</b>.</returns>
        /// <exception cref="ArgumentNullException">When supplied key is <b>null</b>.</exception>
        public bool ContainsKey (TKey key)
        {
            if (key == null)
                throw new ArgumentNullException (nameof (key));

            var leaf = (PairLeaf) Find (key, out int index);
            return index >= 0;
        }


        /// <summary>Determines if the collection contains the supplied value.</summary>
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


        /// <summary>Copies the collection to the supplied array offset.</summary>
        /// <param name="array">Destination of copy.</param>
        /// <param name="index">Starting position in <em>array</em> for copy operation.</param>
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
                throw new ArgumentException ("Destination array is not long enough to copy all the items in the collection. Check array index and length.", nameof (array));

            for (var leaf = (PairLeaf) leftmostLeaf; leaf != null; leaf = (PairLeaf) leaf.rightLeaf)
                for (int leafIndex = 0; leafIndex < leaf.KeyCount; ++leafIndex)
                    array[index++] = new KeyValuePair<TKey,TValue> (leaf.GetKey (leafIndex), leaf.GetValue (leafIndex));
        }


        /// <summary>Gets an enumerator that iterates thru the collection.</summary>
        /// <returns>An enumerator for the collection.</returns>
        public Enumerator GetEnumerator()
        { return new Enumerator (this); }


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

        #region Enumerator

        /// <summary>Provides sequential access to the dictionary.</summary>
        public sealed class Enumerator : IEnumerator<KeyValuePair<TKey,TValue>>, IDictionaryEnumerator
        {
            private readonly RankedDictionary<TKey,TValue> tree;
            private bool isGeneric;
            private PairLeaf leaf;
            private int index;
            private int stageFreeze;

            /// <summary>Make an iterator that will loop thru the collection in order.</summary>
            /// <param name="dictionary">Collection containing these key/value pairs.</param>
            /// <param name="isGeneric">Supply <b>false</b> to indicate object Current should return DictionaryEntry values.</param>
            internal Enumerator (RankedDictionary<TKey,TValue> dictionary, bool isGeneric=true)
            {
                this.tree = dictionary;
                this.isGeneric = isGeneric;
                ((IEnumerator) this).Reset();
            }

            object IDictionaryEnumerator.Key
            {
                get
                {
                    if (index < 0)
                        throw new InvalidOperationException ("Enumeration is not active.");
                    return leaf.GetKey (index);
                }
            }

            object IDictionaryEnumerator.Value
            {
                get
                {
                    if (index < 0)
                        throw new InvalidOperationException ("Enumeration is not active.");
                    return leaf.GetValue (index);
                }
            }

            DictionaryEntry IDictionaryEnumerator.Entry
            {
                get
                {
                    if (index < 0)
                        throw new InvalidOperationException ("Enumeration is not active.");
                    return new DictionaryEntry (leaf.GetKey (index), leaf.GetValue (index));
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    tree.StageCheck (stageFreeze);
                    if (index < 0)
                        throw new InvalidOperationException ("Enumeration is not active.");

                    if (isGeneric)
                        return leaf.GetPair (index);
                    else
                        return new DictionaryEntry (leaf.GetKey (index), leaf.GetValue (index));
                }
            }

            /// <summary>Gets the key/value pair at the current location.</summary>
            /// <exception cref="InvalidOperationException">When the dictionary was modified after the enumerator was created.</exception>
            public KeyValuePair<TKey,TValue> Current
            {
                get
                {
                    tree.StageCheck (stageFreeze);
                    return index < 0 ? new KeyValuePair<TKey,TValue> (default (TKey), default (TValue))
                                         : leaf.GetPair (index);
                }
            }

            /// <summary>Advances the enumerator to the next location.</summary>
            /// <returns><b>false</b> if no more data; otherwise <b>true</b>.</returns>
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

            /// <summary>Moves the enumerator back to its initial location.</summary>
            void IEnumerator.Reset()
            {
                stageFreeze = tree.stage;
                index = -1;
                leaf = (PairLeaf) tree.leftmostLeaf;
            }

            /// <exclude />
            public void Dispose() { }
        }

        #endregion

        #region Explicit generic properties and methods interface implementations

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


        IEnumerator<KeyValuePair<TKey,TValue>> IEnumerable<KeyValuePair<TKey,TValue>>.GetEnumerator()
        { return new Enumerator (this); }


        /// <summary>Indicates that this collection may be modified.</summary>
        bool ICollection<KeyValuePair<TKey,TValue>>.IsReadOnly => false;


        /// <summary>Gets only the collection of keys from this key/value pair collection.</summary>
        /// <remarks>The keys given by this collection are sorted according to the
        /// <see cref="Btree{TKey}.Comparer"/> property.</remarks>
        ICollection<TKey> IDictionary<TKey,TValue>.Keys => (ICollection<TKey>) Keys;


        /// <summary>Gets only the collection of values from this key/value pair collection.</summary>
        /// <remarks>The values given by this collection are sorted in the same
        /// order as their respective keys in the <see cref="Keys"/> collection.</remarks>
        ICollection<TValue> IDictionary<TKey,TValue>.Values => (ICollection<TValue>) Values;

#if ! NET35 && ! NET40
        IEnumerable<TKey> IReadOnlyDictionary<TKey,TValue>.Keys => Keys;

        IEnumerable<TValue> IReadOnlyDictionary<TKey,TValue>.Values => Values;
#endif

        /// <summary>Deletes the supplied key and its associated value from the collection.</summary>
        /// <param name="keyValuePair">Contains key and value to find and remove.</param>
        /// <returns><b>true</b> if key/value pair removed; otherwise <b>false</b>.</returns>
        /// <remarks>No operation is taken unless both key and value match.</remarks>
        bool ICollection<KeyValuePair<TKey,TValue>>.Remove (KeyValuePair<TKey,TValue> keyValuePair)
        {
            var path = new NodeVector (this, keyValuePair.Key);
            if (! path.IsFound || ! EqualityComparer<TValue>.Default.Equals (keyValuePair.Value, PairLeaf.GetValue (path)))
                return false;

            Remove2 (path);
            return true;
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
                        PairLeaf.SetValue (path, (TValue) value);
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

        bool IDictionary.IsFixedSize => false;

        /// <summary>Indicates that structure may be modified.</summary>
        bool IDictionary.IsReadOnly => false;

        bool ICollection.IsSynchronized => false;

        ICollection IDictionary.Keys => (ICollection) Keys;

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
        /// <returns><b>true</b> if the collection contains the supplied key; otherwise <b>false</b>.</returns>
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


        /// <summary>Gets an enumerator that iterates thru the collection.</summary>
        /// <returns>An enumerator for the collection.</returns>
        IDictionaryEnumerator IDictionary.GetEnumerator()
        { return new Enumerator (this, false); }


        /// <summary>Gets an enumerator that iterates thru the collection.</summary>
        /// <returns>An enumerator for the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        { return new Enumerator (this); }


        /// <summary>Removes the supplied key and its associated value from the collection.</summary>
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


        object ICollection.SyncRoot => GetSyncRoot();

        #endregion

        #region Bonus methods

        /// <summary>Provides range query support.</summary>
        /// <param name="lower">Minimum inclusive key value of range.</param>
        /// <param name="upper">Maximum inclusive key value of range.</param>
        /// <returns>An enumerator for all elements between lower and upper.</returns>
        /// <remarks>
        /// <para>
        /// Neither <em>lower</em> or <em>upper</em> need to be present in the collection.
        /// </para>
        /// <para>
        /// Retrieving the first element is a O(log <em>n</em>) operation.
        /// Retrieving subsequent elements is a O(1) operation.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code source="..\Bench\RdExample03\RdExample03.cs" lang="cs" />
        /// </example>
        public IEnumerable<KeyValuePair<TKey,TValue>> GetBetween (TKey lower, TKey upper)
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


        /// <summary>Gets the key/value pair at the supplied index.</summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The element at the supplied index.</returns>
        /// <remarks>This is a O(log <em>n</em>) operation.</remarks>
        /// <exception cref="ArgumentOutOfRangeException">When <em>index</em> is less than zero or greater than or equal to the number of keys.</exception>
        public KeyValuePair<TKey,TValue> GetByIndex (int index)
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException (nameof (index), "Argument is out of the range of valid values.");

            var leaf = (PairLeaf) Find (ref index);
            return new KeyValuePair<TKey,TValue> (leaf.GetKey (index), leaf.GetValue (index));
        }


        /// <summary>Provides range query support with ordered results.</summary>
        /// <param name="key">Minimum value of range.</param>
        /// <returns>An enumerator for the collection for key values greater than or equal to <em>key</em>.</returns>
        /// <exception cref="ArgumentNullException">When <em>key</em> is <b>null</b>.</exception>
        public IEnumerable<KeyValuePair<TKey,TValue>> GetFrom (TKey key)
        {
            if (key == null)
                throw new ArgumentNullException (nameof (key));

            int stageFreeze = stage;
            var leaf = (PairLeaf) Find (key, out int index);

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


        /// <summary>Gets the index of the supplied key.</summary>
        /// <param name="key">The key of the index to get.</param>
        /// <returns>The index of the element with the supplied key if found; otherwise the bitwise complement of the insert point.</returns>
        /// <remarks>This is a O(log <em>n</em>) operation.</remarks>
        /// <exception cref="ArgumentNullException">When <em>key</em> is <b>null</b>.</exception>
        public int IndexOf (TKey key)
        {
            if (key == null)
                throw new ArgumentNullException (nameof (key));

            var path = new NodeVector (this, key);
            int result = path.GetIndex();
            return path.IsFound ? result : ~result;
        }


        /// <summary>Gets the Last key/value pair.</summary>
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


        /// <summary>Gets the value and index of the supplied element.</summary>
        /// <param name="key">The key of the value and index to get.</param>
        /// <param name="value">If the key is found, its value is placed here; otherwise it will be loaded with the default value.</param>
        /// <param name="index">If the key is found, its index is placed here; otherwise it will be -1.</param>
        /// <returns><b>true</b> if supplied key is found; otherwise <b>false</b>.</returns>
        /// <remarks>This is a O(log <em>n</em>) operation.</remarks>
        public bool TryGetValueAndIndex (TKey key, out TValue value, out int index)
        {
            var path = new NodeVector (this, key);
            if (! path.IsFound)
            {
                value = default (TValue);
                index = -1;
                return false;
            }

            value = PairLeaf.GetValue (path);
            index = path.GetIndex();
            return true;
        }

        #endregion
    }
}
