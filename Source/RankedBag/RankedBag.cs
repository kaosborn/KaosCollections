// File: RankedBag.cs
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
    /// Represents a collection of items that can be accessed in sort order or by index.
    /// </summary>
    /// <typeparam name="T">The type of the items in the bag.</typeparam>
    /// <remarks>
    /// <para>
    /// This class is similar to <see cref="RankedSet{T}"/> but with multiple occurrences of an item allowed.
    /// Items with multiple occurrences have each occurrence stored individually.
    /// This flattened implementation allows <see cref="RankedBag{T}"/> to be used
    /// as a multimap as well as the more typical multiset.
    /// Multimap usage requires supplying a user-defined comparer to the constructor.
    /// </para>
    /// <example>
    /// <para>The first program shows some basic operations of this class.</para>
    /// <para>
    /// <code source="..\Bench\RbExample01\RbExample01.cs" lang="cs" />
    /// The next program shows using this class as a multimap.
    /// Positions of each character are stored and displayed for a given string.
    /// </para>
    /// <code source="..\Bench\RbExample02\RbExample02.cs" lang="cs" />
    /// <para>
    /// Last is an example showing binary serialization round tripped.
    /// </para>
    /// <para>
    /// Note: Serialization is not supported in .NET Standard 1.0.
    /// </para>
    /// <code source="..\Bench\RbExample05\RbExample05.cs" lang="cs" />
    /// </example>
    /// </remarks>
    [DebuggerTypeProxy (typeof (ICollectionDebugView<>))]
    [DebuggerDisplay ("Count = {Count}")]
#if NET35 || NET40 || NET45 || SERIALIZE
    [Serializable]
#endif
    public class RankedBag<T> :
        Btree<T>
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

        /// <summary>Initializes a new bag instance that uses the default comparer.</summary>
        /// <exception cref="InvalidOperationException">When <em>comparer</em> is <b>null</b> and no other comparer available.</exception>
        public RankedBag() : base (Comparer<T>.Default, new Leaf())
        { }

        /// <summary>Initializes a new bag instance that uses the supplied comparer.</summary>
        /// <param name="comparer">The comparer to use for sorting items.</param>
        /// <example>
        /// <para>
        /// This program shows using this class as a multiset of case insensitive strings.
        /// </para>
        /// <code source="..\Bench\RbExample01\RbExample01.cs" lang="cs" />
        /// </example>
        /// <exception cref="InvalidOperationException">When <em>comparer</em> is <b>null</b> and no other comparer available.</exception>
        public RankedBag (IComparer<T> comparer) : base (comparer, new Leaf())
        { }

        /// <summary>Initializes a new bag instance that contains items copied from the supplied collection.</summary>
        /// <param name="collection">The enumerable collection to be copied.</param>
        /// <remarks>
        /// This constructor is a O(<em>n</em> log <em>n</em>) operation, where <em>n</em> is the size of <em>collection</em>.
        /// </remarks>
        /// <example>
        /// This program shows using this class for some basic statistical calculations.
        /// <code source="..\Bench\RbExample03\RbExample03.cs" lang="cs" />
        /// </example>
        /// <exception cref="InvalidOperationException">When <em>comparer</em> is <b>null</b> and no other comparer available.</exception>
        /// <exception cref="ArgumentNullException">When <em>collection</em> is <b>null</b>.</exception>
        public RankedBag (IEnumerable<T> collection) : this (collection, Comparer<T>.Default)
        { }

        /// <summary>Initializes a new bag instance that contains items copied from the supplied collection.</summary>
        /// <param name="collection">The enumerable collection to be copied. </param>
        /// <param name="comparer">The comparer to use for item sorting.</param>
        /// <remarks>
        /// This constructor is a O(<em>n</em> log <em>n</em>) operation, where <em>n</em> is the size of <em>collection</em>.
        /// </remarks>
        /// <exception cref="ArgumentNullException">When <em>collection</em> is <b>null</b>.</exception>
        /// <exception cref="InvalidOperationException">When <em>comparer</em> is <b>null</b> and no other comparer available.</exception>
        public RankedBag (IEnumerable<T> collection, IComparer<T> comparer) : this (comparer)
        {
            if (collection == null)
                throw new ArgumentNullException (nameof (collection));

            foreach (T key in collection)
                Add (key);
        }

        #endregion

        #region Properties

        /// <summary>Indicates that the collection is not read-only.</summary>
        bool ICollection<T>.IsReadOnly => false;

        /// <summary>Indicates that the collection is not thread safe.</summary>
        bool ICollection.IsSynchronized => false;

        /// <summary>Gets the maximum item in the bag per the comparer.</summary>
        /// <remarks>This is a O(1) operation.</remarks>
        public T Max => Count==0 ? default (T) : rightmostLeaf.GetKey (rightmostLeaf.KeyCount-1);

        /// <summary>Gets the minimum item in the bag per the comparer.</summary>
        /// <remarks>This is a O(1) operation.</remarks>
        public T Min => Count==0 ? default (T) : leftmostLeaf.Key0;

        /// <summary>Returns a wrapper of the method used to order items in the bag.</summary>
        /// <remarks>
        /// To override sorting based on the default comparer,
        /// supply an alternate comparer when constructing the bag.
        /// </remarks>
        public IComparer<T> Comparer => keyComparer;

        /// <summary>Gets the total number of occurrences of all items in the bag.</summary>
        /// <remarks>This is a O(1) operation.</remarks>
        public int Count => root.Weight;

        /// <summary>Gets an object that can be used to synchronize access to the collection.</summary>
        object ICollection.SyncRoot => GetSyncRoot();

        #endregion

        #region Methods

        /// <summary>Adds an item to the bag.</summary>
        /// <param name="item">The item to add.</param>
        void ICollection<T>.Add (T item)
        { AddKey (item, new NodeVector (this, item, leftEdge:false)); }

        /// <summary>Adds an item to the bag.</summary>
        /// <param name="item">The item to add.</param>
        /// <returns><b>true</b> if this is the first occurrence of this item; otherwise <b>false</b>.</returns>
        /// <remarks>
        /// <para>
        /// If the supplied item already occurs in the bag
        /// then the new item is added sequentially following the old items.
        /// </para>
        /// <para>This is a O(log <em>n</em>) operation.</para>
        /// </remarks>
        /// <exception cref="ArgumentException">When no comparer is available.</exception>
        public bool Add (T item)
        {
            var path = new NodeVector (this, item, leftEdge:false);
            AddKey (item, path);
            return ! path.IsFound;
        }


        /// <summary>Adds a supplied number of occurrences of the supplied item to the bag.</summary>
        /// <param name="item">The item to add.</param>
        /// <param name="count">The number of copies to add.</param>
        /// <returns><b>true</b> if <em>item</em> was not already in the bag; otherwise <b>false</b>.</returns>
        /// <remarks>
        /// <para>
        /// If the supplied item already occurs in the bag
        /// then the new item is added sequentially following the old items.
        /// </para>
        /// <para>
        /// This is a O(<em>m</em> log <em>n</em>) operation
        /// where <em>m</em> is <em>count</em> and <em>n</em> is <see cref="Count"/>.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentException">When <em>count</em> is less than zero.</exception>
        public bool Add (T item, int count)
        {
            if (count < 0)
                throw new ArgumentException ("Must be non-negative.", nameof (count));

            var path = new NodeVector (this, item);
            bool result = path.IsFound;

            if (count > 0)
                for (;;)
                {
                    int leafAdds = maxKeyCount - path.TopNode.KeyCount;
                    if (leafAdds == 0)
                    {
                        AddKey (item, path);
                        --count;
                    }
                    else
                    {
                        if (leafAdds > count)
                            leafAdds = count;
                        path.TopNode.InsertKey (path.TopIndex, item, leafAdds);
                        path.ChangePathWeight (leafAdds);
                        count -= leafAdds;
                    }

                    if (count == 0)
                        break;

                    path = new NodeVector (this, item);
                }

            return result;
        }


        /// <summary>Removes all items from the bag.</summary>
        /// <remarks>This is a O(1) operation.</remarks>
        public void Clear() => Initialize();


        /// <summary>Determines whether the bag contains the supplied item.</summary>
        /// <param name="item">The item to locate.</param>
        /// <returns><b>true</b> if <em>item</em> is contained in the bag; otherwise <b>false</b>.</returns>
        /// <remarks>This is a O(log <em>n</em>) operation.</remarks>
        public bool Contains (T item)
        {
            Leaf leaf = Find (item, out int ix);
            return ix >= 0;
        }


        /// <summary>Determines whether the bag is a subset of the supplied collection.</summary>
        /// <param name="other">The collection to compare to this bag.</param>
        /// <returns><b>true</b> if the bag is a subset of <em>other</em>; otherwise <b>false</b>.</returns>
        /// <remarks>This is a O(log<em>m</em> operation where <em>m</em> is the size of <em>other</em>.</remarks>
        /// <exception cref="ArgumentNullException">When <em>other</em> is <b>null</b>.</exception>
        public bool ContainsAll (IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException (nameof (other));

            var oBag = other as RankedBag<T> ?? new RankedBag<T> (other, Comparer);

            if (Count < oBag.Count)
                return false;
            if (oBag.Count == 0)
                return true;

            Leaf leaf1 = oBag.leftmostLeaf;
            int leafIx1 = 0;
            int treeIx1 = 0;
            for (;;)
            {
                T key = leaf1.GetKey (leafIx1);
                int treeIx2 = oBag.FindEdgeForIndex (key, out Leaf leaf2, out int leafIx2, leftEdge:false);
                if (treeIx2 - treeIx1 > GetCount (key))
                    return false;
                if (leafIx2 < leaf2.KeyCount)
                { leaf1 = leaf2; leafIx1 = leafIx2; }
                else
                {
                    leaf1 = leaf2.rightLeaf;
                    if (leaf1 == null)
                        return true;
                    leafIx1 = 0;
                }
                treeIx1 = treeIx2;
            }
        }


        /// <summary>Copies the items to a compatible array.</summary>
        /// <param name="array">A one-dimensional array that is the destination of the copy.</param>
        /// <remarks>This is a O(<em>n</em>) operation.</remarks>
        /// <exception cref="ArgumentNullException">When <em>array</em> is <b>null</b>.</exception>
        /// <exception cref="ArgumentException">When not enough space is available for the copy.</exception>
        public void CopyTo (T[] array)
        { CopyKeysTo1 (array, 0, Count); }

        /// <summary>Copies the items to a compatible array, starting at the supplied position.</summary>
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
        /// <exception cref="ArgumentOutOfRangeException">When <em>index</em> is less than zero.</exception>
        /// <exception cref="ArgumentException">When not enough space is available for the copy.</exception>
        public void CopyTo (T[] array, int index, int count)
        { CopyKeysTo1 (array, index, count); }

        /// <summary>Copies the bag to a compatible array, starting at the supplied array index.</summary>
        /// <param name="array">A one-dimensional array that is the destination of the copy.</param>
        /// <param name="index">The zero-based starting position in <em>array</em>.</param>
        void ICollection.CopyTo (Array array, int index)
        { CopyKeysTo2 (array, index); }


        /// <summary>Returns the number of occurrences of the supplied item in the bag.</summary>
        /// <param name="item">The item to return the number of occurrences for.</param>
        /// <returns>The number of occurrences of the supplied item.</returns>
        /// <remarks>
        /// <para>
        /// This is a O(log <em>n</em>) operation
        /// where <em>n</em> is <see cref="Count"/>.
        /// </para>
        /// </remarks>
        public int GetCount (T item)
        {
            int treeIx1 = FindEdgeForIndex (item, out Leaf leaf1, out int leafIx1, leftEdge:true);
            if (treeIx1 < 0)
                return 0;
            else
                return FindEdgeForIndex (item, out Leaf leaf2, out int leafIx2, leftEdge:false) - treeIx1;
        }


        /// <summary>Returns the number of distinct items in the bag.</summary>
        /// <returns>The number of distinct items in the bag.</returns>
        /// <remarks>
        /// This is a O(<em>m</em> log <em>n</em>) operation
        /// where <em>m</em> is the distinct item count
        /// and <em>n</em> is <see cref="Count"/>.
        /// </remarks>
        public int GetDistinctCount()
        {
            int result = 0;

            if (Count > 0)
            {
                Leaf leaf = leftmostLeaf;
                int leafIndex = 0;

                for (T currentKey = leaf.Key0;;)
                {
                    ++result;
                    if (leafIndex < leaf.KeyCount - 1)
                    {
                        ++leafIndex;
                        T nextKey = leaf.GetKey (leafIndex);
                        if (Comparer.Compare (currentKey, nextKey) != 0)
                        { currentKey = nextKey; continue; }
                    }

                    FindEdgeRight (currentKey, out leaf, out leafIndex);
                    if (leafIndex >= leaf.KeyCount)
                    {
                        leaf = leaf.rightLeaf;
                        if (leaf == null)
                            break;
                        leafIndex = 0;
                    }
                    currentKey = leaf.GetKey (leafIndex);
                }
            }

            return result;
        }


        /// <summary>Gets the index of the first occurrence of the supplied item.</summary>
        /// <param name="item">The item to find.</param>
        /// <returns>The index of <em>item</em> if found; otherwise a negative value holding the bitwise complement of the insert point.</returns>
        /// <remarks>
        /// <para>
        /// Items with multiple occurrences will return the occurrence with the lowest index.
        /// If <em>item</em> is not found, apply the bitwise complement operator
        /// (<see href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/bitwise-complement-operator">~</see>)
        /// to the result to get the index of the next higher item.
        /// </para>
        /// <para>
        /// This is a O(log <em>n</em>) operation.
        /// </para>
        /// </remarks>
        public int IndexOf (T item)
        {
            return FindEdgeForIndex (item, out Leaf leaf, out int leafIndex, leftEdge:true);
        }


        /// <summary>Removes all occurrences of the supplied item from the bag.</summary>
        /// <param name="item">The item to remove.</param>
        /// <returns><b>true</b> if any items were removed; otherwise <b>false</b>.</returns>
        /// <remarks>
        /// <para>
        /// To limit a remove to a single occurrences of an item, use <see cref="Remove(T, int)"/>.
        /// </para>
        /// <para>
        /// This is a O(log <em>n</em>) operation where <em>n</em> is <see cref="Count"/>.
        /// </para>
        /// </remarks>
        public bool Remove (T item)
        {
            var path1 = new NodeVector (this, item, leftEdge:true);
            if (! path1.IsFound)
                return false;

            var path2 = new NodeVector (this, item, leftEdge:false);

            StageBump();
            Delete (path1, path2);
            return true;
        }


        private int Remove2 (T item, int count)
        {
            if (count <= 0)
                return 0;

            var path1 = new NodeVector (this, item, leftEdge:true);
            if (! path1.IsFound)
                return 0;

            // Seek by offset is faster, so try that first.
            var path2 = NodeVector.CreateFromOffset (path1, count);
            if (path2 == null || Comparer.Compare (path2.LeftKey, item) != 0)
            {
                path2 = new NodeVector (this, item, leftEdge:false);
                count = path2.GetTreeIndex() - path1.GetTreeIndex();
            }

            StageBump();
            Delete (path1, path2);
            return count;
        }


        /// <summary>Removes a supplied number of occurrences of the supplied item from the bag.</summary>
        /// <param name="item">The item to remove.</param>
        /// <param name="count">The number of occurrences to remove.</param>
        /// <returns>The number of occurrences actually removed.</returns>
        /// <remarks>
        /// <para>
        /// For items with multiple occurrences, lowest indexed items are removed first.
        /// </para>
        /// <para>
        /// This is a O(log <em>n</em>) operation <em>n</em> is <see cref="Count"/>.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentException">When <em>count</em> is less than zero.</exception>
        public int Remove (T item, int count)
        {
            if (count < 0)
                throw new ArgumentException ("Must be non-negative.", nameof (count));
            return Remove2 (item, count);
        }


        /// <summary>
        /// Removes all elements that are in the supplied collection from the bag.
        /// </summary>
        /// <param name="other">The elements to remove.</param>
        /// <returns>The number of elements removed from the bag.</returns>
        /// <remarks>
        /// Cardinality is respected by this operation so that
        /// the occurrences of each item removed is the number of occurrences of that item in <em>other</em>.
        /// In precise terms,
        /// this operation removes min(<em>m</em>,<em>n</em>) occurrences of item
        /// where the bag contains <em>n</em> occurrences
        /// and <em>other</em> contains <em>m</em> occurrences.
        /// </remarks>
        /// <exception cref="ArgumentNullException">When <em>other</em> is <b>null</b>.</exception>
        public int RemoveAll (IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException (nameof (other));

            int removed = 0;
            if (Count > 0)
            {
                StageBump();
                var oBag = other as RankedBag<T> ?? new RankedBag<T> (other, Comparer);
                if (oBag.Count > 0)
                    foreach (var oKey in oBag.Distinct())
                    {
                        var oCount = oBag.GetCount (oKey);
                        Remove (oKey, oCount);
                        removed += oCount;
                    }
            }
            return removed;
        }


        /// <summary>Removes the element at the supplied index from the bag.</summary>
        /// <param name="index">The zero-based position of the item to remove.</param>
        /// <remarks>
        /// <para>
        /// After this operation, the index of all following items is reduced by one.
        /// </para>
        /// <para>
        /// This is a O(log <em>n</em>) operation.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">When <em>index</em> is less than zero or greater than or equal to <see cref="Count"/>.</exception>
        public void RemoveAt (int index)
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException (nameof (index), "Argument is out of the range of valid values.");

            RemoveAt2 (index);
        }


        /// <summary>Removes an index range of items from the bag.</summary>
        /// <param name="index">The zero-based starting index of the range of items to remove.</param>
        /// <param name="count">The number of items to remove.</param>
        /// <remarks>This is a O(log <em>n</em>) operation where <em>n</em> is <see cref="Count"/>.</remarks>
        /// <exception cref="ArgumentOutOfRangeException">When <em>index</em> or <em>count</em> is less than zero.</exception>
        /// <exception cref="ArgumentException">When <em>index</em> and <em>count</em> do not denote a valid range of items in the set.</exception>
        public void RemoveRange (int index, int count)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException ("Argument was out of the range of valid values.", nameof (index));

            if (count < 0)
                throw new ArgumentOutOfRangeException ("Argument was out of the range of valid values.", nameof (count));

            if (count > Size - index)
                throw new ArgumentException ("Argument was out of the range of valid values.");

            RemoveRange2 (index, count);
        }


        /// <summary>Removes all elements that match the condition defined by the supplied predicate from the bag.</summary>
        /// <param name="match">The condition of the items to remove.</param>
        /// <returns>The number of elements removed from the bag.</returns>
        /// <remarks>
        /// This is a O(<em>n</em> log <em>m</em>) operation
        /// where <em>m</em> is the count of items removed and <em>n</em> is <see cref="Count"/>.
        /// </remarks>
        /// <exception cref="ArgumentNullException">When <em>match</em> is <b>null</b>.</exception>
        /// <exception cref="InvalidOperationException">When the collection is updated from the supplied predicate.</exception>
        public int RemoveWhere (Predicate<T> match)
        {
            return RemoveWhere2 (match);
        }


        /// <summary>Removes any elements that are not in the supplied collection from the bag.</summary>
        /// <param name="other">The elements to retain.</param>
        /// <returns>The number of elements removed from the bag.</returns>
        /// <remarks>
        /// Cardinality is respected by this operation so that the occurrences
        /// of each item retained is the number of occurrences of that item in <em>other</em>.
        /// In precise terms,
        /// this operation removes max(<em>n</em>-<em>m</em>,0) occurrences of a given item
        /// where the bag contains <em>n</em> occurrences
        /// and <em>other</em> contains <em>m</em> occurrences.
        /// </remarks>
        /// <example>
        /// <code source="..\Bench\RbExample01\RbExample01.cs" lang="cs" />
        /// </example>
        /// <exception cref="ArgumentNullException">When <em>other</em> is <b>null</b>.</exception>
        public int RetainAll (IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException (nameof (other));

            int result = 0;
            if (Count > 0)
            {
                var oBag = other as RankedBag<T> ?? new RankedBag<T> (other, Comparer);
                if (oBag.Count == 0)
                {
                    result = Count;
                    if (result != 0)
                    { StageBump(); Clear(); }
                    return result;
                }

                T key2 = leftmostLeaf.Key0;
                for (int treeIx1 = 0;;)
                {
                    T key1 = key2;
                    int treeIx2 = FindEdgeForIndex (key1, out Leaf leaf2, out int leafIx2, leftEdge:false);
                    int toDelete = (treeIx2 - treeIx1) - oBag.GetCount (key1);

                    if (leafIx2 < leaf2.KeyCount)
                        key2 = leaf2.GetKey (leafIx2);
                    else
                    {
                        leaf2 = leaf2.rightLeaf;
                        if (leaf2 != null)
                            key2 = leaf2.Key0;
                    }

                    int deleted = Remove2 (key1, toDelete);
                    result += deleted;
                    if (leaf2 == null)
                        break;
                    treeIx1 = treeIx2 - deleted;
                }
            }

            return result;
        }

        #endregion

        #region ISerializable implementation and support
#if NET35 || NET40 || NET45 || SERIALIZE

        private SerializationInfo serializationInfo;

        /// <summary>Initializes a new bag instance that contains serialized data.</summary>
        /// <param name="info">The object that contains the information required to serialize the bag.</param>
        /// <param name="context">The structure that contains the source and destination of the serialized stream.</param>
        protected RankedBag (SerializationInfo info, StreamingContext context) : base (new Btree<T>.Leaf())
        {
            this.serializationInfo = info;
        }


        /// <summary>Returns the data needed to serialize the bag.</summary>
        /// <param name="info">An object that contains the information required to serialize the bag.</param>
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

            if (storedCount != 0)
            {
                var items = (T[]) serializationInfo.GetValue ("Items", typeof (T[]));
                if (items == null)
                    throw new SerializationException ("Missing Items.");

                for (int ix = 0; ix < items.Length; ++ix)
                    Add (items[ix]);

                if (storedCount != Count)
                    throw new SerializationException ("Mismatched count.");
            }

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


        /// <summary>Gets the last occurrence of the last item.</summary>
        /// <returns>The item sorted to the end of the bag.</returns>
        /// <remarks>This is a O(1) operation.</remarks>
        /// <exception cref="InvalidOperationException">When the collection is empty.</exception>
        public T Last()
        {
            if (Count == 0)
                throw new InvalidOperationException ("Sequence contains no elements.");

            return rightmostLeaf.GetKey (rightmostLeaf.KeyCount - 1);
        }

        #endregion

        #region Enumeration

        /// <summary>Returns an enumerator that iterates thru the distinct items of the bag.</summary>
        /// <returns>An enumerator that iterates thru distinct items.</returns>
        /// <remarks>
        /// <para>
        /// For items with multiple occurrences, the oldest occurrences are returned.
        /// </para>
        /// <para>
        /// This is a O(<em>m</em> log <em>n</em>) operation
        /// where <em>m</em> is the distinct item count
        /// and <em>n</em> is <see cref="Count"/>.
        /// </para>
        /// </remarks>
        public IEnumerable<T> Distinct()
        {
            if (Count == 0)
                yield break;

            int ix = 0;
            Leaf leaf = leftmostLeaf;
            for (T key = leaf.Key0;;)
            {
                yield return key;

                if (ix < leaf.KeyCount - 1)
                {
                    ++ix;
                    T nextKey = leaf.GetKey (ix);
                    if (Comparer.Compare (key, nextKey) != 0)
                    { key = nextKey; continue; }
                }

                FindEdgeRight (key, out leaf, out ix);
                if (ix >= leaf.KeyCount)
                {
                    leaf = leaf.rightLeaf;
                    if (leaf == null)
                        yield break;
                    ix = 0;
                }
                key = leaf.GetKey (ix);
            }
        }


        /// <summary>Returns an enumerator that iterates over a range with the supplied bounds.</summary>
        /// <param name="lower">Minimum item value of the range.</param>
        /// <param name="upper">Maximum item value of the range.</param>
        /// <returns>An enumerator for the supplied range.</returns>
        /// <remarks>
        /// <para>
        /// If either <em>lower</em> or <em>upper</em> are present in the bag,
        /// they will be included in the results.
        /// </para>
        /// <para>
        /// Retrieving the initial item is a O(log <em>n</em>) operation.
        /// Retrieving each subsequent item is a O(1) operation.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code source="..\Bench\RbExample03\RbExample03.cs" lang="cs" />
        /// </example>
        /// <exception cref="InvalidOperationException">When the bag was modified after the enumerator was created.</exception>
        public IEnumerable<T> ElementsBetween (T lower, T upper)
        {
            int stageFreeze = stage;

            FindEdgeLeft (lower, out Leaf leaf, out int ix);
            for (;;)
            {
                if (ix < leaf.KeyCount)
                {
                    T key = leaf.GetKey (ix);
                    if (Comparer.Compare (key, upper) > 0)
                        yield break;

                    yield return key;
                    StageCheck (stageFreeze);
                    ++ix;
                    continue;
                }

                leaf = leaf.rightLeaf;
                if (leaf == null)
                    yield break;

                ix = 0;
            }
        }


        /// <summary>Returns an enumerator that iterates over a range with the supplied lower bound.</summary>
        /// <param name="lower">Minimum item of the range.</param>
        /// <returns>An enumerator for the supplied range.</returns>
        /// <remarks>
        /// <para>
        /// If <em>lower</em> is present in the bag, it will be included in the results.
        /// </para>
        /// <para>
        /// Retrieving the initial item is a O(log <em>n</em>) operation.
        /// Retrieving each subsequent item is a O(1) operation.
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">When the bag was modified after the enumerator was created.</exception>
        public IEnumerable<T> ElementsFrom (T lower)
        {
            int stageFreeze = stage;

            FindEdgeLeft (lower, out Leaf leaf, out int ix);
            for (;;)
            {
                if (ix < leaf.KeyCount)
                {
                    yield return leaf.GetKey (ix);
                    StageCheck (stageFreeze);
                    ++ix;
                    continue;
                }

                leaf = (Leaf) leaf.rightLeaf;
                if (leaf == null)
                    yield break;

                ix = 0;
            }
        }


        /// <summary>Returns an enumerator that iterates over a range with the supplied index bounds.</summary>
        /// <param name="lowerIndex">Minimum index of the range.</param>
        /// <param name="upperIndex">Maximum index of the range.</param>
        /// <returns>An enumerator for the supplied index range.</returns>
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
        /// <exception cref="InvalidOperationException">When the bag was modified after the enumerator was created.</exception>
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


        /// <summary>Returns an IEnumerable that iterates thru the bag in reverse order.</summary>
        /// <returns>An enumerator that reverse iterates thru the bag.</returns>
        /// <exception cref="InvalidOperationException">When the bag was modified after the enumerator was created.</exception>
        public IEnumerable<T> Reverse()
        {
            Enumerator enor = new Enumerator (this, isReverse:true);
            while (enor.MoveNext())
                yield return enor.Current;
        }


        /// <summary>Returns an enumerator that iterates thru the bag.</summary>
        /// <returns>An enumerator that iterates thru the bag in sorted order.</returns>
        public Enumerator GetEnumerator() => new Enumerator (this);

        /// <summary>Returns an enumerator that iterates thru the bag.</summary>
        /// <returns>An enumerator that iterates thru the bag in sorted order.</returns>
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => new Enumerator (this);

        /// <summary>Returns an enumerator that iterates thru the collection.</summary>
        /// <returns>An enumerator that iterates thru the collection in sorted order.</returns>
        IEnumerator IEnumerable.GetEnumerator() => new Enumerator (this);


        /// <summary>Enumerates the sorted items of a <see cref="RankedBag{T}"/>.</summary>
        public sealed class Enumerator : IEnumerator<T>
        {
            private readonly RankedBag<T> tree;
            private readonly bool isReverse;
            private Leaf leaf;
            private int index;
            private int stageFreeze;
            private int state;  // -1=rewound; 0=active; 1=consumed

            internal Enumerator (RankedBag<T> bag, bool isReverse=false)
            {
                this.tree = bag;
                this.isReverse = isReverse;
                ((IEnumerator) this).Reset();
            }

            /// <summary>Gets the element at the current position.</summary>
            object IEnumerator.Current
            {
                get
                {
                    tree.StageCheck (stageFreeze);
                    if (state != 0)
                        throw new InvalidOperationException();
                    return (object) leaf.GetKey (index);
                }
            }

            /// <summary>Gets the item at the current position of the enumerator.</summary>
            /// <exception cref="InvalidOperationException">When the bag was modified after the enumerator was created.</exception>
            public T Current
            {
                get
                {
                    tree.StageCheck (stageFreeze);
                    return state != 0 ? default (T) : leaf.GetKey (index);
                }
            }

            /// <summary>Advances the enumerator to the next item in the bag.</summary>
            /// <returns><b>true</b> if the enumerator was successfully advanced to the next item; <b>false</b> if the enumerator has passed the end of the bag.</returns>
            /// <exception cref="InvalidOperationException">When the bag was modified after the enumerator was created.</exception>
            public bool MoveNext()
            {
                tree.StageCheck (stageFreeze);

                if (state != 0)
                    if (state > 0)
                        return false;
                    else
                    {
                        leaf = isReverse ? tree.rightmostLeaf : tree.leftmostLeaf;
                        index = isReverse ? leaf.KeyCount : -1;
                        state = 0;
                    }

                if (isReverse)
                {
                    if (--index >= 0)
                        return true;

                    leaf = leaf.leftLeaf;
                    if (leaf != null)
                    { index = leaf.KeyCount - 1; return true; }
                }
                else
                {
                    if (++index < leaf.KeyCount)
                        return true;

                    leaf = leaf.rightLeaf;
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
