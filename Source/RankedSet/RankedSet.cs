//
// Library: KaosCollections
// File:    RankedSet.cs
//
// Copyright © 2009-2018 Kasey Osborn (github.com/kaosborn)
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
    /// Represents a collection of distinct items that can be accessed in sort order or by index.
    /// </summary>
    /// <typeparam name="T">The type of the items in the set.</typeparam>
    /// <remarks>
    /// <para>
    /// This class emulates and extends
    /// <see cref="System.Collections.Generic.SortedSet{T}"/> while
    /// significantly improving performance on large collections.
    /// </para>
    /// <para>
    /// Optimized instance methods with the signatures of LINQ methods have been implemented:
    /// <list type="bullet">
    /// <item><see cref="ElementAt"/></item>
    /// <item><see cref="ElementAtOrDefault"/></item>
    /// <item><see cref="First"/></item>
    /// <item><see cref="Last"/></item>
    /// <item><see cref="Reverse"/></item>
    /// </list>
    /// </para>
    /// <para>
    /// Indexing enhancements include:
    /// <list type="bullet">
    /// <item><see cref="IndexOf"/></item>
    /// <item><see cref="RemoveAt"/></item>
    /// <item><see cref="RemoveRange"/></item>
    /// </list>
    /// </para>
    /// <para>
    /// Optimized range enumerators are provided:
    /// <list type="bullet">
    /// <item><see cref="ElementsBetween"/></item>
    /// <item><see cref="ElementsFrom"/></item>
    /// <item><see cref="ElementsBetweenIndexes"/></item>
    /// </list>
    /// </para>
    /// <para>
    /// An equivalent to BCL's <see cref="SortedSet{T}.GetViewBetween(T,T)"/> has not implemented for performance reasons.
    /// However, <see cref="ElementsBetween"/> is available for range enumeration.
    /// </para>
    /// </remarks>
    /// <example>
    /// <para>This program shows some basic operations of this class.</para>
    /// <code source="..\Bench\RsExample01\RsExample01.cs" lang="cs" />
    /// <para>Next is an example showing binary serialization round tripped.</para>
    /// <para>Note: Serialization is not supported in .NET Standard 1.0.</para>
    /// <code source="..\Bench\RsExample05\RsExample05.cs" lang="cs" />
    /// </example>
    [DebuggerTypeProxy (typeof (ICollectionDebugView<>))]
    [DebuggerDisplay ("Count = {Count}")]
#if NET35 || NET40 || NET45 || SERIALIZE
    [Serializable]
#endif
    public partial class RankedSet<T> :
        Btree<T>
#if ! NET35
        , ISet<T>
#endif
        , ICollection<T>
        , ICollection
#if ! NET35 && ! NET40
        , IReadOnlyCollection<T>
#endif
#if NET35 || NET40 || NET45 || SERIALIZE
        , ISerializable
        , IDeserializationCallback
#endif
    {
        #region Constructors

        /// <summary>Initializes a new set instance that uses the default item comparer.</summary>
        public RankedSet() : base (Comparer<T>.Default, new Leaf())
        { }

        /// <summary>Initializes a new set instance that uses the supplied comparer.</summary>
        /// <param name="comparer">The comparer to use for sorting items.</param>
        /// <example>
        /// This program shows usage of a custom comparer for case insensitivity.
        /// <code source="..\Bench\RsExample01\RsExample01.cs" lang="cs" />
        /// </example>
        /// <exception cref="InvalidOperationException">When <em>comparer</em> is <b>null</b> and no other comparer available.</exception>
        public RankedSet (IComparer<T> comparer) : base (comparer, new Leaf())
        { }

        /// <summary>Initializes a new set instance that contains items copied from the supplied collection.</summary>
        /// <param name="collection">The enumerable collection to be copied.</param>
        /// <remarks>
        /// This constructor is a O(<em>n</em> log <em>n</em>) operation, where <em>n</em> is the number of items.
        /// </remarks>
        /// <exception cref="ArgumentNullException">When <em>collection</em> is <b>null</b>.</exception>
        public RankedSet (IEnumerable<T> collection) : this (collection, Comparer<T>.Default)
        { }

        /// <summary>Initializes a new set instance that contains items copied from the supplied collection.</summary>
        /// <param name="collection">The enumerable collection to be copied. </param>
        /// <param name="comparer">The comparer to use for item sorting.</param>
        /// <remarks>
        /// This constructor is a O(<em>n</em> log <em>n</em>) operation, where <em>n</em> is the number of items.
        /// </remarks>
        /// <exception cref="ArgumentNullException">When <em>collection</em> is <b>null</b>.</exception>
        /// <exception cref="InvalidOperationException">When <em>comparer</em> is <b>null</b> and no other comparer available.</exception>
        public RankedSet (IEnumerable<T> collection, IComparer<T> comparer) : this (comparer)
        {
            if (collection == null)
                throw new ArgumentNullException (nameof (collection));

            foreach (T key in collection)
                Add (key);
        }

        #endregion

        #region Properties

        /// <summary>Returns a wrapper of the method used to order items in the set.</summary>
        /// <remarks>
        /// To override sorting based on the default comparer,
        /// supply an alternate comparer when constructing the set.
        /// </remarks>
        public IComparer<T> Comparer => keyComparer;

        /// <summary>Gets the number of items in the set.</summary>
        /// <remarks>This is a O(1) operation.</remarks>
        public int Count => root.Weight;

        /// <summary>Indicates that the collection is not read-only.</summary>
        bool ICollection<T>.IsReadOnly => false;

        /// <summary>Indicates that the collection is not thread safe.</summary>
        bool ICollection.IsSynchronized => false;

        /// <summary>Gets the maximum item in the set per the comparer.</summary>
        /// <remarks>This is a O(1) operation.</remarks>
        public T Max => Count==0 ? default (T) : rightmostLeaf.GetKey (rightmostLeaf.KeyCount - 1);

        /// <summary>Gets the minimum item in the set per the comparer.</summary>
        /// <remarks>This is a O(1) operation.</remarks>
        public T Min => Count==0 ? default (T) : leftmostLeaf.Key0;

        /// <summary>Gets an object that can be used to synchronize access to the collection.</summary>
        object ICollection.SyncRoot => GetSyncRoot();

        #endregion

        #region Methods

        /// <summary>Adds an item to the set.</summary>
        /// <param name="item">The item to add.</param>
        void ICollection<T>.Add (T item)
        { Add (item); }

        /// <summary>Adds an item to the set and returns a success indicator.</summary>
        /// <param name="item">The item to add.</param>
        /// <returns><b>true</b> if <em>item</em> was added to the set; otherwise <b>false</b>.</returns>
        /// <remarks>
        /// <para>
        /// If <em>item</em> is already in the set, this method returns <b>false</b> and does not throw an exception.
        /// </para>
        /// <para>This is a O(log <em>n</em>) operation.</para>
        /// </remarks>
        /// <exception cref="ArgumentException">When no comparer is available.</exception>
        public bool Add (T item)
        {
            var path = new NodeVector (this, item);
            if (path.IsFound)
                return false;

            return AddKey (item, path);
        }


        /// <summary>Removes all items from the set.</summary>
        /// <remarks>This is a O(1) operation.</remarks>
        public void Clear() => Initialize();


        /// <summary>Determines whether the set contains the supplied item.</summary>
        /// <param name="item">The item to locate.</param>
        /// <returns><b>true</b> if <em>item</em> is contained in the set; otherwise <b>false</b>.</returns>
        /// <remarks>This is a O(log <em>n</em>) operation.</remarks>
        public bool Contains (T item)
        {
            Leaf leaf = Find (item, out int index);
            return index >= 0;
        }


        /// <summary>Copies the set to a compatible array, starting at the beginning of the array.</summary>
        /// <param name="array">A one-dimensional array that is the destination of the copy.</param>
        /// <remarks>This is a O(<em>n</em>) operation.</remarks>
        /// <exception cref="ArgumentNullException">When <em>array</em> is <b>null</b>.</exception>
        /// <exception cref="ArgumentException">When not enough space is available for the copy.</exception>
        public void CopyTo (T[] array)
        { CopyKeysTo1 (array, 0, Count); }

        /// <summary>Copies the set to a compatible array, starting at the supplied position.</summary>
        /// <param name="array">A one-dimensional array that is the destination of the copy.</param>
        /// <param name="index">The zero-based starting position in <em>array</em>.</param>
        /// <remarks>This is a O(<em>n</em>) operation.</remarks>
        /// <exception cref="ArgumentNullException">When <em>array</em> is <b>null</b>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">When <em>index</em> is less than zero.</exception>
        /// <exception cref="ArgumentException">When not enough space is available for the copy.</exception>
        public void CopyTo (T[] array, int index)
        { CopyKeysTo1 (array, index, Count); }

        /// <summary>Copies a supplied number of items to a compatible array, starting at the supplied position.</summary>
        /// <param name="array">A one-dimensional array that is the destination of the copy.</param>
        /// <param name="index">The zero-based starting position in <em>array</em>.</param>
        /// <param name="count">The number of items to copy.</param>
        /// <remarks>This is a O(<em>n</em>) operation.</remarks>
        /// <exception cref="ArgumentNullException">When <em>array</em> is <b>null</b>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">When <em>index</em> or <em>count</em> is less than zero.</exception>
        /// <exception cref="ArgumentException">When not enough space is available for the copy.</exception>
        public void CopyTo (T[] array, int index, int count)
        { CopyKeysTo1 (array, index, count); }

        /// <summary>Copies the set to a compatible array, starting at the supplied array index.</summary>
        /// <param name="array">A one-dimensional array that is the destination of the copy.</param>
        /// <param name="index">The zero-based starting position in <em>array</em>.</param>
        void ICollection.CopyTo (Array array, int index)
        { CopyKeysTo2 (array, index); }


        /// <summary>Removes the supplied item from the set.</summary>
        /// <param name="item">The item to remove.</param>
        /// <returns><b>true</b> if <em>item</em> was found and removed; otherwise <b>false</b>.</returns>
        /// <remarks>This is a O(log <em>n</em>) operation.</remarks>
        public bool Remove (T item)
        {
            var path = new NodeVector (this, item);
            if (! path.IsFound)
                return false;

            Remove2 (path);
            return true;
        }


        /// <summary>Removes an index range of items from the set.</summary>
        /// <param name="index">The zero-based starting index of the range of items to remove.</param>
        /// <param name="count">The number of items to remove.</param>
        /// <remarks>This is a O(log <em>n</em>) operation where <em>n</em> is <see cref="Count"/>.</remarks>
        /// <example>
        /// <para>Here, this method is is used to truncate a set.</para>
        /// <code source="..\Bench\RsExample01\RsExample01.cs" lang="cs"/>
        /// </example>
        /// <exception cref="ArgumentOutOfRangeException">When <em>index</em> or <em>count</em> is less than zero.</exception>
        /// <exception cref="ArgumentException">When <em>index</em> and <em>count</em> do not denote a valid range of items in the set.</exception>
        public void RemoveRange (int index, int count)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException ("Argument was out of the range of valid values.", nameof (index));

            if (count < 0)
                throw new ArgumentOutOfRangeException ("Argument was out of the range of valid values.", nameof (count));

            if (count > root.Weight - index)
                throw new ArgumentException ("Argument was out of the range of valid values.");

            RemoveRange2 (index, count);
        }


        /// <summary>Removes all items that match the condition defined by the supplied predicate from the set.</summary>
        /// <param name="match">The condition of the items to remove.</param>
        /// <returns>The number of items removed from the set.</returns>
        /// <remarks>
        /// This is a O(<em>n</em> log <em>m</em>) operation
        /// where <em>m</em> is the number of items removed and <em>n</em> is <see cref="Count"/>.
        /// </remarks>
        /// <example>
        /// <para>Here, this method is is used to remove strings containing a space.</para>
        /// <code source="..\Bench\RsExample01\RsExample01.cs" lang="cs"/>
        /// </example>
        /// <exception cref="ArgumentNullException">When <em>match</em> is <b>null</b>.</exception>
        public int RemoveWhere (Predicate<T> match) => RemoveWhere2 (match);

        #endregion

        #region ISet implementation

        /// <summary>Removes all items in the supplied collection from the set.</summary>
        /// <param name="other">The collection of items to remove.</param>
        /// <remarks>
        /// Duplicate values in <em>other</em> are ignored.
        /// Values in <em>other</em> that are not in the set are ignored.
        /// </remarks>
        /// <example>
        /// <code source="..\Bench\RsExample04\RsExample04.cs" lang="cs" />
        /// </example>
        /// <exception cref="ArgumentNullException">When <em>other</em> is <b>null</b>.</exception>
        public void ExceptWith (IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException (nameof (other));

            StageBump();
            if (Count > 0)
                if (other == this)
                    Clear();
                else
                    foreach (T key in other)
                        Remove (key);
        }


        /// <summary>Removes all items that are not in a supplied collection.</summary>
        /// <param name="other">The collection of items to intersect.</param>
        /// <example>
        /// <code source="..\Bench\RsExample04\RsExample04.cs" lang="cs" />
        /// </example>
        /// <exception cref="ArgumentNullException">When <em>other</em> is <b>null</b>.</exception>
        public void IntersectWith (IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException (nameof (other));

            if (Count == 0)
                return;

            StageBump();
            var oSet = other as RankedSet<T> ?? new RankedSet<T> (other, Comparer);

            if (oSet.Count == 0 || Comparer.Compare (oSet.Max, Min) < 0 || Comparer.Compare (oSet.Min, Max) > 0)
            {
                Clear();
                return;
            }

            for (Leaf leaf = rightmostLeaf; leaf != null; leaf = leaf.leftLeaf)
                for (int ix = leaf.KeyCount-1; ix >= 0; --ix)
                {
                    T key = leaf.GetKey (ix);
                    if (! oSet.Contains (key))
                        Remove (key);
                }
        }


        /// <summary>Determines whether the set is a proper subset of the supplied collection.</summary>
        /// <param name="other">The collection to compare to this set.</param>
        /// <returns><b>true</b> if the set is a proper subset of <em>other</em>; otherwise <b>false</b>.</returns>
        /// <example>
        /// <code source="..\Bench\RsExample03\RsExample03.cs" lang="cs" />
        /// </example>
        /// <exception cref="ArgumentNullException">When <em>other</em> is <b>null</b>.</exception>
        public bool IsProperSubsetOf (IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException (nameof (other));

            var oSet = other as RankedSet<T> ?? new RankedSet<T> (other, Comparer);

            if (Count >= oSet.Count)
                return false;

            foreach (T key in this)
                if (! oSet.Contains (key))
                    return false;

            return true;
        }


        /// <summary>Determines whether the set is a proper superset of the supplied collection.</summary>
        /// <param name="other">The collection to compare to this set.</param>
        /// <returns><b>true</b> if the set is a proper superset of <em>other</em>; otherwise <b>false</b>.</returns>
        /// <example>
        /// <code source="..\Bench\RsExample03\RsExample03.cs" lang="cs" />
        /// </example>
        /// <exception cref="ArgumentNullException">When <em>other</em> is <b>null</b>.</exception>
        public bool IsProperSupersetOf (IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException (nameof (other));

            var oSet = other as RankedSet<T> ?? new RankedSet<T> (other, Comparer);

            if (Count <= oSet.Count)
                return false;

            foreach (T key in other)
                if (! Contains (key))
                    return false;

            return true;
        }


        /// <summary>Determines whether the set is a subset of the supplied collection.</summary>
        /// <param name="other">The collection to compare to this set.</param>
        /// <returns><b>true</b> if the set is a subset of <em>other</em>; otherwise <b>false</b>.</returns>
        /// <example>
        /// <code source="..\Bench\RsExample03\RsExample03.cs" lang="cs" />
        /// </example>
        /// <exception cref="ArgumentNullException">When <em>other</em> is <b>null</b>.</exception>
        public bool IsSubsetOf (IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException (nameof (other));

            var oSet = other as RankedSet<T> ?? new RankedSet<T> (other, Comparer);

            if (Count > oSet.Count)
                return false;

            foreach (T key in this)
                if (! oSet.Contains (key))
                    return false;

            return true;
        }


        /// <summary>Determines whether a set is a superset of the supplied collection.</summary>
        /// <param name="other">The items to compare to the current set.</param>
        /// <returns><b>true</b> if the set is a superset of <em>other</em>; otherwise <b>false</b>.</returns>
        /// <example>
        /// <code source="..\Bench\RsExample03\RsExample03.cs" lang="cs" />
        /// </example>
        /// <exception cref="ArgumentNullException">When <em>other</em> is <b>null</b>.</exception>
        public bool IsSupersetOf (IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException (nameof (other));

            foreach (T key in other)
                if (! Contains (key))
                    return false;

            return true;
        }


        /// <summary>Determines whether the set and a supplied collection share common items.</summary>
        /// <param name="other">The collection to compare to this set.</param>
        /// <returns><b>true</b> if the set and <em>other</em> share at least one common item; otherwise <b>false</b>.</returns>
        /// <example>
        /// <code source="..\Bench\RsExample03\RsExample03.cs" lang="cs" />
        /// </example>
        /// <exception cref="ArgumentNullException">When <em>other</em> is <b>null</b>.</exception>
        public bool Overlaps (IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException (nameof (other));

            if (Count != 0)
                if (other is RankedSet<T> oSet)
                {
                    RankedSet<T> set1, set2;
                    if (Count > oSet.Count)
                    { set1 = this; set2 = oSet; }
                    else
                    { set2 = this; set1 = oSet; }

                    foreach (T key in set2.ElementsBetween (set1.Min, set1.Max))
                        if (set1.Contains (key))
                            return true;
                }
                else
                    foreach (T key in other)
                        if (Contains (key))
                            return true;

            return false;
        }


        /// <summary>Determines whether the set and the supplied collection contain the same items.</summary>
        /// <param name="other">The collection to compare to this set.</param>
        /// <returns><b>true</b> if the set is equal to <em>other</em>; otherwise <b>false</b>.</returns>
        /// <remarks>Duplicate values in <em>other</em> are ignored.</remarks>
        /// <example>
        /// <code source="..\Bench\RsExample03\RsExample03.cs" lang="cs" />
        /// </example>
        /// <exception cref="ArgumentNullException">When <em>other</em> is <b>null</b>.</exception>
        public bool SetEquals (IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException (nameof (other));

            var oSet = other as RankedSet<T> ?? new RankedSet<T> (other, Comparer);

            if (Count != oSet.Count)
                return false;

            foreach (T key in oSet)
                if (! Contains (key))
                    return false;

            return true;
        }


        /// <summary>Modifies the set so that it contains only items that are present either in itself or in the supplied collection, but not both.</summary>
        /// <param name="other">The collection to compare to this set.</param>
        /// <example>
        /// <code source="..\Bench\RsExample04\RsExample04.cs" lang="cs" />
        /// </example>
        /// <exception cref="ArgumentNullException">When <em>other</em> is <b>null</b>.</exception>
        public void SymmetricExceptWith (IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException (nameof (other));

            var oSet = other as RankedSet<T> ?? new RankedSet<T> (other, Comparer);
            if (oSet.Count == 0)
                return;

            StageBump();
            Enumerator oEtor = oSet.GetEnumerator();
            oEtor.MoveNext();
            T oKey = oEtor.Current;

            for (Leaf leaf = leftmostLeaf; leaf != null; leaf = leaf.rightLeaf)
                for (int ix = 0; ix < leaf.KeyCount; )
                    for (T key = leaf.GetKey (ix);;)
                    {
                        int diff = Comparer.Compare (oKey, key);
                        if (diff >= 0)
                        {
                            if (diff > 0)
                                ++ix;
                            else
                            {
                                Remove (key);
                                if (! oEtor.MoveNext())
                                    return;
                                oKey = oEtor.Current;
                            }
                            break;
                        }

                        Add (oKey);
                        if (! oEtor.MoveNext())
                            return;
                        oKey = oEtor.Current;
                    }

            for (;;)
            {
                Add (oKey);
                if (! oEtor.MoveNext())
                    return;
                oKey = oEtor.Current;
            }
        }


        /// <summary>Add all items in <em>other</em> to this set that are not already in this set.</summary>
        /// <param name="other">The collection to add to this set.</param>
        /// <remarks>Duplicate values in <em>other</em> are ignored.</remarks>
        /// <example>
        /// <code source="..\Bench\RsExample04\RsExample04.cs" lang="cs" />
        /// </example>
        /// <exception cref="ArgumentNullException">When <em>other</em> is <b>null</b>.</exception>
        public void UnionWith (IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException (nameof (other));

            StageBump();
            foreach (T key in other)
                Add (key);
        }

        #endregion

        #region ISerializable implementation and support
#if NET35 || NET40 || NET45 || SERIALIZE

        private SerializationInfo serializationInfo;

        /// <summary>Initializes a new set instance that contains serialized data.</summary>
        /// <param name="info">The object that contains the information required to serialize the set.</param>
        /// <param name="context">The structure that contains the source and destination of the serialized stream.</param>
        protected RankedSet (SerializationInfo info, StreamingContext context) : base (new Btree<T>.Leaf())
        {
            this.serializationInfo = info;
        }


        /// <summary>Returns the data needed to serialize the set.</summary>
        /// <param name="info">An object that contains the information required to serialize the set.</param>
        /// <param name="context">A structure that contains the source and destination of the serialized stream.</param>
        /// <exception cref="ArgumentNullException">When <em>info</em> is <b>null</b>.</exception>
        protected virtual void GetObjectData (SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException (nameof (info));

            info.AddValue ("Count", Count);
            info.AddValue ("Comparer", Comparer, typeof (IComparer<T>));
            info.AddValue ("Stage", stage);

            var items = new T[Count];
            CopyTo (items, 0);
            info.AddValue ("Items", items, typeof (T[]));
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

            keyComparer = (IComparer<T>) serializationInfo.GetValue ("Comparer", typeof (IComparer<T>));
            int storedCount = serializationInfo.GetInt32 ("Count");
            stage = serializationInfo.GetInt32 ("Stage");

            var items = (T[]) serializationInfo.GetValue ("Items", typeof (T[]));
            if (items == null)
                throw new SerializationException ("Missing Items.");

            for (int ix = 0; ix < items.Length; ++ix)
                Add (items[ix]);

            if (storedCount != Count)
                throw new SerializationException ("Mismatched count.");

            serializationInfo = null;
        }


        /// <summary>Returns the data needed to serialize the set.</summary>
        /// <param name="info">An object that contains the information required to serialize the set.</param>
        /// <param name="context">A structure that contains the source and destination of the serialized stream.</param>
        /// <exception cref="ArgumentNullException">When <em>info</em> is <b>null</b>.</exception>
        void ISerializable.GetObjectData (SerializationInfo info, StreamingContext context)
        { GetObjectData (info, context); }


        /// <summary>Implements the deserialization callback and raises the deserialization event when completed.</summary>
        /// <param name="sender">The source of the deserialization event.</param>
        /// <exception cref="ArgumentNullException">When <em>sender</em> is <b>null</b>.</exception>
        /// <exception cref="SerializationException">When the associated <em>SerializationInfo</em> is invalid.</exception>
        void IDeserializationCallback.OnDeserialization (Object sender)
        { OnDeserialization (sender); }

#endif
        #endregion

        #region LINQ instance implementation

        /// <summary>Gets the item at the supplied index.</summary>
        /// <param name="index">The zero-based index of the item to get.</param>
        /// <returns>The item at <em>index</em>.</returns>
        /// <remarks>This is a O(log <em>n</em>) operation.</remarks>
        /// <exception cref="ArgumentOutOfRangeException">When <em>index</em> is less than zero or not less than the number of items.</exception>
        public T ElementAt (int index)
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException (nameof (index), "Argument was out of the range of valid values.");

            var leaf = (Leaf) Find (index, out int leafIndex);
            return leaf.GetKey (leafIndex);
        }


        /// <summary>Gets the item at the supplied index or the default if the index is out of range.</summary>
        /// <param name="index">The zero-based index of the item to get.</param>
        /// <returns>The item at <em>index</em>.</returns>
        /// <remarks>This is a O(log <em>n</em>) operation.</remarks>
        public T ElementAtOrDefault (int index)
        {
            if (index < 0 || index >= Count)
                return default (T);

            var leaf = (Leaf) Find (index, out int leafIndex);
            return leaf.GetKey (leafIndex);
        }


        /// <summary>Gets the minimum item in the set per the comparer.</summary>
        /// <returns>The minimum item in the set.</returns>
        /// <remarks>This is a O(1) operation.</remarks>
        /// <exception cref="InvalidOperationException">When <see cref="Count"/> is zero.</exception>
        public T First()
        {
            if (Count == 0)
                throw new InvalidOperationException ("Sequence contains no elements.");

            return leftmostLeaf.Key0;
        }


        /// <summary>Gets the maximum item in the set per the comparer.</summary>
        /// <returns>The maximum item in the set.</returns>
        /// <remarks>This is a O(1) operation.</remarks>
        /// <exception cref="InvalidOperationException">When <see cref="Count"/> is zero.</exception>
        public T Last()
        {
            if (Count == 0)
                throw new InvalidOperationException ("Sequence contains no elements.");

            return rightmostLeaf.GetKey (rightmostLeaf.KeyCount - 1);
        }


        /// <summary>Returns an IEnumerable that iterates thru the set in reverse order.</summary>
        /// <returns>An enumerator that reverse iterates thru the set.</returns>
        /// <exception cref="InvalidOperationException">When the set was modified after the enumerator was created.</exception>
        public Enumerator Reverse() => new Enumerator (this, isReverse:true);

        #endregion

        #region Bonus methods

        /// <summary>Returns an enumerator that iterates over a range with the supplied bounds.</summary>
        /// <param name="lower">Minimum item value of the range.</param>
        /// <param name="upper">Maximum item value of the range.</param>
        /// <returns>An enumerator for the specified range.</returns>
        /// <remarks>
        /// <para>
        /// If either <em>lower</em> or <em>upper</em> are present in the set,
        /// they will be included in the results.
        /// </para>
        /// <para>
        /// Retrieving the initial item is a O(log <em>n</em>) operation.
        /// Retrieving each subsequent item is a O(1) operation.
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">When the set was modified after the enumerator was created.</exception>
        public IEnumerable<T> ElementsBetween (T lower, T upper)
        {
            int stageFreeze = stage;
            var leaf = (Leaf) Find (lower, out int index);

            // When the supplied start key is not be found, start with the next highest key.
            if (index < 0)
                index = ~index;

            for (;;)
            {
                if (index < leaf.KeyCount)
                {
                    if (Comparer.Compare (leaf.GetKey (index), upper) > 0)
                        yield break;

                    yield return leaf.GetKey (index);
                    StageCheck (stageFreeze);
                    ++index;
                    continue;
                }

                leaf = leaf.rightLeaf;
                if (leaf == null)
                    yield break;

                index = 0;
            }
        }


        /// <summary>Returns an enumerator that iterates over a range with the supplied index bounds.</summary>
        /// <param name="lowerIndex">Minimum index of the range.</param>
        /// <param name="upperIndex">Maximum index of the range.</param>
        /// <returns>An enumerator for the specified index range.</returns>
        /// <remarks>
        /// <para>
        /// Index bounds are inclusive.
        /// </para>
        /// <para>
        /// Retrieving the initial item is a O(log <em>n</em>) operation.
        /// Retrieving each subsequent item is a O(1) operation.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">When <em>lowerIndex</em> is less than zero or not less than <see cref="Count"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">When <em>upperIndex</em> is less than zero or not less than <see cref="Count"/>.</exception>
        /// <exception cref="ArgumentException">When <em>lowerIndex</em> and <em>upperIndex</em> do not denote a valid range of indexes.</exception>
        /// <exception cref="InvalidOperationException">When the set was modified after the enumerator was created.</exception>
        public IEnumerable<T> ElementsBetweenIndexes (int lowerIndex, int upperIndex)
        {
            if (lowerIndex < 0 || lowerIndex >= Count)
                throw new ArgumentOutOfRangeException (nameof (lowerIndex), "Argument was out of the range of valid values.");

            if (upperIndex < 0 || upperIndex >= Count)
                throw new ArgumentOutOfRangeException (nameof (upperIndex), "Argument was out of the range of valid values.");

            int toGo = upperIndex - lowerIndex;
            if (toGo < 0)
                throw new ArgumentException ("Arguments were out of the range of valid values.");

            int stageFreeze = stage;
            var leaf = (Leaf) Find (lowerIndex, out int index);
            do
            {
                if (index >= leaf.KeyCount)
                { index = 0; leaf = leaf.rightLeaf; }

                yield return leaf.GetKey (index);
                StageCheck (stageFreeze);
                ++index;
            }
            while (--toGo >= 0);
        }


        /// <summary>Returns an enumerator that iterates over a range with the supplied lower bound.</summary>
        /// <param name="lower">Minimum of the range.</param>
        /// <returns>An enumerator for the specified range.</returns>
        /// <remarks>
        /// <para>
        /// If <em>lower</em> is present in the set, it will be included in the results.
        /// </para>
        /// <para>
        /// Retrieving the initial item is a O(log <em>n</em>) operation.
        /// Retrieving each subsequent item is a O(1) operation.
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">When the set was modified after the enumerator was created.</exception>
        public IEnumerable<T> ElementsFrom (T lower)
        {
            int stageFreeze = stage;
            var leaf = (Leaf) Find (lower, out int index);

            // When the supplied start key is not be found, start with the next highest key.
            if (index < 0)
                index = ~index;

            for (;;)
            {
                if (index < leaf.KeyCount)
                {
                    yield return leaf.GetKey (index);
                    StageCheck (stageFreeze);
                    ++index;
                    continue;
                }

                leaf = (Leaf) leaf.rightLeaf;
                if (leaf == null)
                    yield break;

                index = 0;
            }
        }


        /// <summary>Gets the index of the supplied item.</summary>
        /// <param name="item">The item to find.</param>
        /// <returns>The index of <em>item</em> if found; otherwise a negative value holding the bitwise complement of the insert point.</returns>
        /// <remarks>
        /// <para>
        /// If <em>item</em> is not found, apply the bitwise complement operator
        /// (<see href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/bitwise-complement-operator">~</see>)
        /// to the result to get the index of the next higher item.
        /// </para>
        /// <para>
        /// This is a O(log <em>n</em>) operation.
        /// </para>
        /// </remarks>
        public int IndexOf (T item)
            => FindEdgeForIndex (item, out Leaf _, out int _, leftEdge:true);


        /// <summary>Removes the item at the supplied index from the set.</summary>
        /// <param name="index">The zero-based position of the item to remove.</param>
        /// <para>
        /// After this operation, the position of all following items is reduced by one.
        /// </para>
        /// <para>
        /// This is a O(log <em>n</em>) operation.
        /// </para>
        /// <exception cref="ArgumentOutOfRangeException">When <em>index</em> is less than zero or greater than or equal to <see cref="Count"/>.</exception>
        public void RemoveAt (int index)
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException (nameof (index), "Argument is out of the range of valid values.");

            RemoveAt2 (index);
        }


        /// <summary>Replace an item if present.</summary>
        /// <param name="item">The replacement item.</param>
        /// <returns><b>true</b> if an item is replaced; otherwise <b>false</b>.</returns>
        /// <remarks>
        /// This single operation is equivalent to performing a
        /// <see cref="Remove"/> operation followed by an
        /// <see cref="Add"/> operation.
        /// </remarks>
        public bool Replace (T item)
        {
            var path = new NodeVector (this, item);
            if (! path.IsFound)
                return false;

            ReplaceKey (path, item);
            return true;
        }


        /// <summary>Replace an item or optionally add it if missing.</summary>
        /// <param name="item">The replacement or new item.</param>
        /// <param name="addIfMissing"><b>true</b> to add <em>item</em> if not already present.</param>
        /// <returns><b>true</b> if item is replaced; otherwise <b>false</b>.</returns>
        /// <remarks>
        /// This operation is an optimized alternative to performing the implicit operations separately.
        /// </remarks>
        public bool Replace (T item, bool addIfMissing)
        {
            var path = new NodeVector (this, item);
            if (! path.IsFound)
            {
                if (addIfMissing)
                    AddKey (item, path);
                return false;
            }

            ReplaceKey (path, item);
            return true;
        }


        /// <summary>Bypasses a supplied number of items and yields the remaining items.</summary>
        /// <param name="count">Number of items to skip.</param>
        /// <returns>The items after the supplied offset.</returns>
        /// <remarks>This is a O(log <em>n</em>) operation.</remarks>
        /// <example>
        /// In the below snippet, both Skip operations perform an order of magnitude faster than their LINQ equivalent.
        /// <code source="..\Bench\RxExample01\RxExample01.cs" lang="cs" region="RsSkip" />
        /// </example>
        /// <exception cref="InvalidOperationException">When the set was modified after the enumerator was created.</exception>
        public Enumerator Skip (int count) => new Enumerator (this, count);


        /// <summary>
        /// Bypasses elements as long as a supplied condition is true and yields the remaining items.
        /// </summary>
        /// <param name="predicate">The condition to test for.</param>
        /// <returns>Remaining items after the first item that does not satisfy the supplied condition.</returns>
        /// <exception cref="InvalidOperationException">When the set was modified after the enumerator was created.</exception>
        public Enumerator SkipWhile (Func<T,bool> predicate) => new Enumerator (this, predicate);

        /// <summary>
        /// Bypasses elements as long as a supplied index-based condition is true and yields the remaining items.
        /// </summary>
        /// <param name="predicate">The condition to test for.</param>
        /// <returns>Remaining items after the first item that does not satisfy the supplied condition.</returns>
        /// <exception cref="InvalidOperationException">When the set was modified after the enumerator was created.</exception>
        public Enumerator SkipWhile (Func<T,int,bool> predicate) => new Enumerator (this, predicate);


        /// <summary>Gets the actual item for the supplied search item.</summary>
        /// <param name="getItem">The item to find.</param>
        /// <param name="item">
        /// If <em>item</em> is found, its value is placed here;
        /// otherwise it will be loaded with the default for its type.
        /// </param>
        /// <returns><b>true</b> if <em>getItem</em> is found; otherwise <b>false</b>.</returns>
        public bool TryGet (T getItem, out T item)
        {
            var leaf = Find (getItem, out int index);
            if (index < 0)
            { item = default (T); return false; }

            item = leaf.GetKey (index);
            return true;
        }


        /// <summary>Gets the least item greater than the supplied item.</summary>
        /// <param name="getItem">The item to use for comparison.</param>
        /// <param name="item">The actual item if found; otherwise the default.</param>
        /// <returns><b>true</b> if item greater than <em>getItem</em> is found; otherwise <b>false</b>.</returns>
        public bool TryGetGreaterThan (T getItem, out T item)
        {
            TryGetGT (getItem, out Leaf leaf, out int index);
            if (leaf == null)
            { item = default (T); return false; }
            else
            { item = leaf.GetKey (index); return true; }
        }


        /// <summary>Gets the least item greater than or equal to the supplied item.</summary>
        /// <param name="getItem">The item to use for comparison.</param>
        /// <param name="item">The actual item if found; otherwise the default.</param>
        /// <returns><b>true</b> if item greater than or equal to <em>getItem</em> found; otherwise <b>false</b>.</returns>
        public bool TryGetGreaterThanOrEqual (T getItem, out T item)
        {
            TryGetGE (getItem, out Leaf leaf, out int index);
            if (leaf == null)
            { item = default (T); return false; }
            else
            { item = leaf.GetKey (index); return true; }
        }


        /// <summary>Gets the greatest item that is less than the supplied item.</summary>
        /// <param name="getItem">The item to use for comparison.</param>
        /// <param name="item">The actual item if found; otherwise the default.</param>
        /// <returns><b>true</b> if item less than <em>item</em> found; otherwise <b>false</b>.</returns>
        public bool TryGetLessThan (T getItem, out T item)
        {
            TryGetLT (getItem, out Leaf leaf, out int index);
            if (leaf == null)
            { item = default (T); return false; }
            else
            { item = leaf.GetKey (index); return true; }
        }


        /// <summary>Gets the greatest item that is less than or equal to the supplied item.</summary>
        /// <param name="getItem">The item to use for comparison.</param>
        /// <param name="item">The actual item if found; otherwise the default.</param>
        /// <returns><b>true</b> if item less than or equal to <em>item</em> found; otherwise <b>false</b>.</returns>
        public bool TryGetLessThanOrEqual (T getItem, out T item)
        {
            TryGetLE (getItem, out Leaf leaf, out int index);
            if (leaf == null)
            { item = default (T); return false; }
            else
            { item = leaf.GetKey (index); return true; }
        }

        #endregion

        #region Class comparison

        /// <summary>Returns an equality comparer that can be used to create a collection that contains sets.</summary>
        /// <returns>An equality comparer for creating a collection of sets.</returns>
        /// <example>
        /// <code source="..\Bench\RsExample06\RsExample06.cs" lang="cs" />
        /// </example>
        public static IEqualityComparer<RankedSet<T>> CreateSetComparer()
        {
            return CreateSetComparer (null);
        }


        /// <summary>Returns an equality comparer using a supplied comparer that can be used to create a collection that contains sets.</summary>
        /// <param name="memberEqualityComparer">Used for creating the returned comparer.</param>
        /// <returns>An equality comparer for creating a collection of sets.</returns>
        public static IEqualityComparer<RankedSet<T>> CreateSetComparer (IEqualityComparer<T> memberEqualityComparer)
        {
            return new RankedSetEqualityComparer (memberEqualityComparer);
        }


        private bool HasEqualComparer (RankedSet<T> other)
        {
            return Comparer == other.Comparer || Comparer.Equals (other.Comparer);
        }


        private static bool RankedSetEquals (RankedSet<T> set1, RankedSet<T> set2, IComparer<T> comparer)
        {
            if (set1 == null)
                return set2 == null;

            if (set2 == null)
                return false;

            if (set1.HasEqualComparer (set2))
                return set1.Count == set2.Count && set1.SetEquals (set2);
            
            bool found = false;
            foreach (T item1 in set1)
            {
                found = false;
                foreach (T item2 in set2)
                    if (comparer.Compare (item1, item2) == 0)
                    {
                        found = true;
                        break;
                    }

                if (! found)
                    return false;
            }

            return true;
        }

        #endregion

        #region Enumeration

        /// <summary>Returns an enumerator that iterates thru the set.</summary>
        /// <returns>An enumerator that iterates thru the set in sorted order.</returns>
        public Enumerator GetEnumerator() => new Enumerator (this);

        /// <summary>Returns an enumerator that iterates thru the set.</summary>
        /// <returns>An enumerator that iterates thru the set in sorted order.</returns>
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => new Enumerator (this);

        /// <summary>Returns an enumerator that iterates thru the collection.</summary>
        /// <returns>An enumerator that iterates thru the collection in sorted order.</returns>
        IEnumerator IEnumerable.GetEnumerator() => new Enumerator (this);


        /// <summary>Enumerates the items of a <see cref="RankedSet{T}"/> in sort order.</summary>
        [DebuggerTypeProxy (typeof (IEnumerableDebugView<>))]
        public struct Enumerator : IEnumerator<T>, IEnumerable<T>
        {
            private readonly KeyEnumerator etor;

            internal Enumerator (RankedSet<T> set, bool isReverse=false) => etor = new KeyEnumerator (set, isReverse);

            internal Enumerator (RankedSet<T> set, int count) => etor = new KeyEnumerator (set, count);

            internal Enumerator (RankedSet<T> set, Func<T,bool> predicate) => etor = new KeyEnumerator (set, predicate);

            internal Enumerator (RankedSet<T> set, Func<T,int,bool> predicate) => etor = new KeyEnumerator (set, predicate);

            /// <summary>Gets the item at the current position.</summary>
            /// <exception cref="InvalidOperationException">When the enumerator is not active.</exception>
            object IEnumerator.Current
            {
                get
                {
                    if (etor.NotActive)
                        throw new InvalidOperationException ("Enumerator is not active.");
                    return (object) etor.CurrentKey;
                }
            }

            /// <summary>Gets the item at the current position of the enumerator.</summary>
            public T Current => etor.CurrentKeyOrDefault;

            /// <summary>Advances the enumerator to the next item in the set.</summary>
            /// <returns><b>true</b> if the enumerator was successfully advanced to the next item; <b>false</b> if the enumerator has passed the end of the set.</returns>
            /// <exception cref="InvalidOperationException">When the set was modified after the enumerator was created.</exception>
            public bool MoveNext() => etor.Advance();

            /// <summary>Rewinds the enumerator to its initial state.</summary>
            void IEnumerator.Reset() => etor.Initialize();

            /// <summary>Releases all resources used by the enumerator.</summary>
            public void Dispose() { }

            /// <summary>Gets an iterator for this collection.</summary>
            /// <returns>An iterator for this collection.</returns>
            public IEnumerator<T> GetEnumerator() => this;

            /// <summary>Gets an iterator for this collection.</summary>
            /// <returns>An iterator for this collection.</returns>
            IEnumerator IEnumerable.GetEnumerator() => this;

            /// <summary>Bypasses a supplied number of items and yields the remaining items.</summary>
            /// <param name="count">Number of items to skip.</param>
            /// <returns>The items after the supplied offset.</returns>
            /// <remarks>This is a O(log <em>n</em>) operation.</remarks>
            /// <example>
            /// In the below snippet, both Skip operations perform an order of magnitude faster than their LINQ equivalent.
            /// <code source="..\Bench\RxExample01\RxExample01.cs" lang="cs" region="RsSkip" />
            /// </example>
            /// <exception cref="InvalidOperationException">When the set was modified after the enumerator was created.</exception>
            public Enumerator Skip (int count)
            {
                etor.Bypass (count);
                return this;
            }

            /// <summary>
            /// Bypasses items as long as a supplied condition is true and yields the remaining items.
            /// </summary>
            /// <param name="predicate">The condition to test for.</param>
            /// <returns>Remaining items after the first item that does not satisfy the supplied condition.</returns>
            /// <exception cref="InvalidOperationException">When the set was modified after the enumerator was created.</exception>
            public Enumerator SkipWhile (Func<T,bool> predicate)
            {
                etor.BypassKey (predicate);
                return this;
            }

            /// <summary>
            /// Bypasses items as long as a supplied index-based condition is true and yields the remaining items.
            /// </summary>
            /// <param name="predicate">The condition to test for.</param>
            /// <returns>Remaining items after the first item that does not satisfy the supplied condition.</returns>
            /// <exception cref="InvalidOperationException">When the set was modified after the enumerator was created.</exception>
            public Enumerator SkipWhile (Func<T,int,bool> predicate)
            {
                etor.BypassKey (predicate);
                return this;
            }
        }

        #endregion
    }
}
