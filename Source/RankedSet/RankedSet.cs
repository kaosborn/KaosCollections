//
// Library: KaosCollections
// File:    RankedSet.cs
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
    /// <summary>Represents a collection of sorted, unique items.</summary>
    /// <typeparam name="TKey">The type of the items in the set.</typeparam>
    /// <remarks>
    /// This class emulates and augments the
    /// <see cref="System.Collections.Generic.SortedSet&lt;TKey&gt;"/> class.
    /// </remarks>
    [DebuggerTypeProxy (typeof (ICollectionDebugView<>))]
    [DebuggerDisplay ("Count = {Count}")]
    public sealed class RankedSet<TKey> : Btree<TKey>,
#if ! NET35
        ISet<TKey>,
#endif
        ICollection<TKey>,
        ICollection
#if ! NET35 && ! NET40
        , IReadOnlyCollection<TKey>
#endif
    {
        private KeyLeaf LeftmostLeaf { get { return leftmostLeaf; } }

        #region Constructors

        /// <summary>Initializes a new set of sorted items that uses the default item comparer.</summary>
        public RankedSet() : this (defaultOrder, Comparer<TKey>.Default)
        { }

        /// <summary>Initializes a new set of sorted items that uses the supplied comparer.</summary>
        /// <param name="comparer">The comparer to use for sorting items.</param>
        public RankedSet (IComparer<TKey> comparer) : this (defaultOrder, comparer)
        { }

        /// <summary>Initializes a new set that contains items copied from the specified collection.</summary>
        /// <param name="collection">The enumerable collection to be copied.</param>
        /// <exception cref="ArgumentNullException">When <em>collection</em> is <b>null</b>.</exception>
        public RankedSet (IEnumerable<TKey> collection) : this (collection, Comparer<TKey>.Default)
        { }

        /// <summary>Initializes a new set that contains items copied from the specified collection.</summary>
        /// <param name="collection">The enumerable collection to be copied. </param>
        /// <param name="comparer">The comparer to use for item sorting.</param>
        /// <exception cref="ArgumentNullException">When <em>collection</em> is <b>null</b>.</exception>
        public RankedSet (IEnumerable<TKey> collection, IComparer<TKey> comparer) : this (defaultOrder, comparer)
        {
            if (collection == null)
                throw new ArgumentNullException (nameof (collection));

            foreach (TKey item in collection)
                Add (item);
        }

        /// <summary>Initializes a new set of sorted items.</summary>
        /// <param name="order">Maximum number of children of a branch.</param>
        /// <remarks>This constuctor is provided for experimental purposes
        /// and its use may result in degraded performance.</remarks>
        /// <exception cref="ArgumentOutOfRangeException">When <em>order</em> is too big or too small.</exception>
        public RankedSet (int order) : this (order, Comparer<TKey>.Default)
        { }

        /// <summary>Initializes a new set of sorted items.</summary>
        /// <param name="order">Maximum number of children of a branch.</param>
        /// <param name="comparer">The comparer to use for sorting items.</param>
        /// <remarks>This constuctor is provided for experimental purposes
        /// and its use may result in degraded performance.</remarks>
        /// <exception cref="ArgumentOutOfRangeException">When <em>order</em> is too big or too small.</exception>
        public RankedSet (int order, IComparer<TKey> comparer) : base (order, comparer, new KeyLeaf())
        { }

        #endregion

        #region Properties

        bool ICollection<TKey>.IsReadOnly
        { get { return false; } }

        bool ICollection.IsSynchronized
        { get { return false; } }

        /// <summary>Gets the maximum value in the set per the comparer.</summary>
        public TKey Max
        {
            get
            {
                if (Count == 0)
                    return default (TKey);
                KeyLeaf rightmost = GetRightmost();
                return rightmost.GetKey (rightmost.KeyCount-1);
            }
        }

        /// <summary>Gets the minimum value in the set per the comparer.</summary>
        public TKey Min
        {
            get
            {
                if (Count == 0)
                    return default (TKey);
                return LeftmostLeaf.Key0;
            }
        }

        /// <summary>Deprecated.</summary>
        object ICollection.SyncRoot => GetSyncRoot();

        #endregion

        #region Methods

        /// <summary>Removes all items from the set.</summary>
        public void Clear()
        {
            leftmostLeaf.Chop();
            root = leftmostLeaf;
        }


        /// <summary>Adds an item to the set and returns a success indicator.</summary>
        /// <param name="item">The item to add.</param>
        /// <returns><b>true</b> if the item was added to the set; otherwise <b>false</b>.</returns>
        public bool Add (TKey item)
        {
            var path = new NodeVector (this, item);
            if (path.IsFound)
                return false;

            Add2 (path, item);
            return true;
        }

        void ICollection<TKey>.Add (TKey item)
        { Add (item); }

        private void Add2 (NodeVector path, TKey key)
        {
            var leaf = (KeyLeaf) path.TopNode;
            int pathIndex = path.TopNodeIndex;

            path.UpdateWeight (1);
            if (leaf.KeyCount < maxKeyCount)
            {
                leaf.Insert (pathIndex, key);
                return;
            }

            // Leaf is full so right split a new leaf.
            var newLeaf = new KeyLeaf (leaf, maxKeyCount);

            if (newLeaf.RightLeaf == null && pathIndex == leaf.KeyCount)
                newLeaf.AddKey (key);
            else
            {
                int splitIndex = leaf.KeyCount / 2 + 1;

                if (pathIndex < splitIndex)
                {
                    // Left-side insert: Copy right side to the split leaf.
                    newLeaf.Add (leaf, splitIndex - 1, leaf.KeyCount);
                    leaf.Truncate (splitIndex - 1);
                    leaf.Insert (pathIndex, key);
                }
                else
                {
                    // Right-side insert: Copy split leaf parts and new key.
                    newLeaf.Add (leaf, splitIndex, pathIndex);
                    newLeaf.AddKey (key);
                    newLeaf.Add (leaf, pathIndex, leaf.KeyCount);
                    leaf.Truncate (splitIndex);
                }
            }

            // Promote anchor of split leaf.
            path.Promote (newLeaf.Key0, (Node) newLeaf, newLeaf.RightLeaf == null);
        }


        /// <summary>Determines whether the set contains a specific item.</summary>
        /// <param name="item">The item to check for existence in the set.</param>
        /// <returns><b>true</b> if the set contains the item; otherwise <b>false</b>.</returns>
        public bool Contains (TKey item)
        {
            KeyLeaf leaf = Find (item, out int index);
            return index >= 0;
        }


        /// <summary>Copies the set to a compatible array, starting at the beginning of the target array.</summary>
        /// <param name="array">A one-dimensional array that is the destination of the items to copy from the set.</param>
        /// <exception cref="ArgumentNullException">When <em>array</em> is <b>null</b>.</exception>
        /// <exception cref="ArgumentException">When not enough space is given for the copy.</exception>
        public void CopyTo (TKey[] array)
        { CopyTo (array, 0, Count); }

        /// <summary>Copies the set to a compatible array, starting at the beginning of the target array.</summary>
        /// <param name="array">A one-dimensional array that is the destination of the items to copy from the set.</param>
        /// <param name="index">The zero-based starting position.</param>
        /// <exception cref="ArgumentNullException">When <em>array</em> is <b>null</b>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">When <em>index</em> is less than zero.</exception>
        /// <exception cref="ArgumentException">When not enough space is given for the copy.</exception>
        public void CopyTo (TKey[] array, int index)
        { CopyTo (array, index, Count); }

        /// <summary>Copies the set to a compatible array, starting at the specified position.</summary>
        /// <param name="array">A one-dimensional array that is the destination of the items to copy from the set.</param>
        /// <param name="index">The zero-based starting position.</param>
        /// <param name="count">The number of items to copy.</param>
        /// <exception cref="ArgumentNullException">When <em>array</em> is <b>null</b>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">When <em>index</em> or <em>count</em> is less than zero.</exception>
        /// <exception cref="ArgumentException">When not enough space is given for the copy.</exception>
        public void CopyTo (TKey[] array, int index, int count)
        {
            if (array == null)
                throw new ArgumentNullException (nameof (array));

            if (index < 0)
                throw new ArgumentOutOfRangeException (nameof (index), index, "Specified argument was out of the range of valid values.");

            if (count < 0)
                throw new ArgumentOutOfRangeException (nameof (count), count, "Specified argument was out of the range of valid values.");

            if (Count > array.Length - index)
                throw new ArgumentException ("Destination array is not long enough to copy all the items in the collection. Check array index and length.", nameof (array));

            for (KeyLeaf leaf = LeftmostLeaf; leaf != null; leaf = leaf.RightLeaf)
                for (int klix = 0; klix < leaf.KeyCount; ++klix)
                    array[index++] = leaf.GetKey (klix);
        }

        void ICollection.CopyTo (Array array, int index)
        {
            if (array == null)
                throw new ArgumentNullException (nameof (array));

            if (array.Rank != 1)
                throw new ArgumentException ("Multidimension array is not supported on this operation.", nameof (array));

            if (array.GetLowerBound (0) != 0)
                throw new ArgumentException ("Target array has non-zero lower bound.", nameof (array));

            if (index < 0)
                throw new ArgumentOutOfRangeException (nameof (index), "Non-negative number required.");

            if (Count > array.Length - index)
                throw new ArgumentException ("Destination array is not long enough to copy all the items in the collection. Check array index and length.", nameof (array));

            if (array is TKey[] genArray)
            {
                CopyTo (genArray, index);
                return;
            }

            if (array is object[] obArray)
            {
                try
                {
                    int ix = 0;
                    foreach (var item in this)
                        obArray[ix++] = item;
                }
                catch (ArrayTypeMismatchException)
                { throw new ArgumentException ("Mismatched array type.", nameof (array)); }
            }
            else
                throw new ArgumentException ("Invalid array type.", nameof (array));
        }


        /// <summary>Returns an enumerator that iterates thru the set.</summary>
        /// <returns>An enumerator that iterates thru the set in sorted order.</returns>
        public Enumerator GetEnumerator() => new Enumerator (this);


        /// <summary>Removes a specified item from the set.</summary>
        /// <param name="item">The item to remove.</param>
        /// <returns><b>true</b> if the item was found and removed; otherwise <b>false</b>.</returns>
        public bool Remove (TKey item)
        {
            var path = new NodeVector (this, item);
            if (! path.IsFound)
                return false;

            Remove2 (path);
            return true;
        }


        /// <summary>Removes all items that match the condition defined by the specified predicate.</summary>
        /// <param name="match">The condition of the items to remove.</param>
        /// <returns>The number of elements removed.</returns>
        /// <exception cref="ArgumentNullException">When <em>array</em> is <b>match</b>.</exception>
        public int RemoveWhere (Predicate<TKey> match)
        {
            int delCount = 0;

            if (match == null)
                throw new ArgumentNullException (nameof (match));

            for (KeyLeaf leaf = GetRightmost(); leaf != null; leaf = leaf.LeftLeaf)
                for (int ix = leaf.KeyCount-1; ix >= 0; --ix)
                {
                    TKey key = leaf.GetKey (ix);
                    bool isMatch = match (key);
                    if (isMatch)
                    {
                        ++delCount;
                        bool isOk = Remove (key);
                    }
                }

            return delCount;
        }


        /// <summary>Returns an IEnumerable that iterates over the set in reverse order.</summary>
        /// <returns>An enumerator that reverse iterates over the set.</returns>
        public IEnumerable<TKey> Reverse()
        {
            Enumerator enor = new Enumerator (this, reverse:true);
            while (enor.MoveNext())
                yield return enor.Current;
        }


        IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator()
        {
            return new Enumerator (this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator (this);
        }

        #endregion

        #region ISet methods implementation
#if ! NET35

        /// <summary>Removes all items that are in a supplied collection.</summary>
        /// <param name="other">The collection of items to remove.</param>
        /// <exception cref="ArgumentNullException">When <em>other</em> is <b>null</b>.</exception>
        public void ExceptWith (IEnumerable<TKey> other)
        {
            if (other == null)
                throw new ArgumentNullException (nameof (other));

            if (Count > 0)
                if (other == this)
                    Clear();
                else
                    foreach (TKey item in other)
                        Remove (item);
        }


        /// <summary>Removes all items that are not in a supplied collection.</summary>
        /// <param name="other">The collection of items to intersect.</param>
        /// <exception cref="ArgumentNullException">When <em>other</em> is <b>null</b>.</exception>
        public void IntersectWith (IEnumerable<TKey> other)
        {
            if (other == null)
                throw new ArgumentNullException (nameof (other));

            var oSet = other as RankedSet<TKey> ?? new RankedSet<TKey> (other);

            if (oSet.Count == 0)
            {
                Clear();
                return;
            }

            for (KeyLeaf leaf = LeftmostLeaf; leaf != null; leaf = leaf.RightLeaf)
                for (int ix = 0; ix < leaf.KeyCount; )
                {
                    var key = leaf.GetKey (ix);
                    if (! oSet.Contains (key))
                        Remove (key);
                    else
                        ++ix;
                }
        }


        /// <summary>Determines whether the set is a proper subset of the supplied collection..</summary>
        /// <param name="other">The collection to compare to this set.</param>
        /// <returns><b>true</b> if the set is a proper subset of <em>other</em>; otherwise <b>false</b>.</returns>
        /// <exception cref="ArgumentNullException">When <em>other</em> is <b>null</b>.</exception>
        public bool IsProperSubsetOf (IEnumerable<TKey> other)
        {
            if (other == null)
                throw new ArgumentNullException (nameof (other));

            var oSet = other as RankedSet<TKey> ?? new RankedSet<TKey> (other);

            if (Count >= oSet.Count)
                return false;

            foreach (var item in this)
                if (! oSet.Contains (item))
                    return false;

            return true;
        }


        /// <summary>Determines whether the set is a proper superset of the supplied collection..</summary>
        /// <param name="other">The collection to compare to this set.</param>
        /// <returns><b>true</b> if the set is a proper superset of <em>other</em>; otherwise <b>false</b>.</returns>
        /// <exception cref="ArgumentNullException">When <em>other</em> is <b>null</b>.</exception>
        public bool IsProperSupersetOf (IEnumerable<TKey> other)
        {
            if (other == null)
                throw new ArgumentNullException (nameof (other));

            var oSet = other as RankedSet<TKey> ?? new RankedSet<TKey> (other);

            if (Count <= oSet.Count)
                return false;

            foreach (var item in other)
                if (! Contains (item))
                    return false;

            return true;
        }


        /// <summary>Determines whether the set is a subset of the supplied collection..</summary>
        /// <param name="other">The collection to compare to this set.</param>
        /// <returns><b>true</b> if the set is a subset of <em>other</em>; otherwise <b>false</b>.</returns>
        /// <exception cref="ArgumentNullException">When <em>other</em> is <b>null</b>.</exception>
        public bool IsSubsetOf (IEnumerable<TKey> other)
        {
            if (other == null)
                throw new ArgumentNullException (nameof (other));

            var oSet = other as RankedSet<TKey> ?? new RankedSet<TKey> (other);

            if (Count > oSet.Count)
                return false;

            foreach (var item in this)
                if (! oSet.Contains (item))
                    return false;

            return true;
        }


        /// <summary>Determines whether a set is a superset of the specified collection.</summary>
        /// <param name="other">The items to compare to the current set.</param>
        /// <returns><b>true</b> if the set is a superset of <em>other</em>; otherwise <b>false</b>.</returns>
        /// <exception cref="ArgumentNullException">When <em>other</em> is <b>null</b>.</exception>
        public bool IsSupersetOf (IEnumerable<TKey> other)
        {
            if (other == null)
                throw new ArgumentNullException (nameof (other));

            foreach (TKey item in other)
                if (! Contains (item))
                    return false;

            return true;
        }


        /// <summary>Determines whether the set and a supplied collection share common elements.</summary>
        /// <param name="other">The collection to compare to this set.</param>
        /// <returns><b>true</b> if the set and <em>other</em> share at least one common item; otherwise <b>false</b>.</returns>
        /// <exception cref="ArgumentNullException">When <em>other</em> is <b>null</b>.</exception>
        public bool Overlaps (IEnumerable<TKey> other)
        {
            if (other == null)
                throw new ArgumentNullException (nameof (other));

            if (Count == 0)
                return false;

            if (other is RankedSet<TKey> oSet)
            {
                if (Comparer.Compare (oSet.Max, Min) < 0)
                    return false;
                if (Comparer.Compare (oSet.Min, Max) > 0)
                    return false;

                foreach (var item in oSet.GetBetween (Min, Max))
                    if (Contains (item))
                        return true;

                return false;
            }

            foreach (var item in other)
                if (Contains (item))
                    return true;

            return false;
        }


        /// <summary>Determines whether the set and the supplied collection contain the same items.</summary>
        /// <param name="other">The collection to compare to this set.</param>
        /// <returns><b>true</b> if the set is equal to <em>other</em>; otherwise <b>false</b>.</returns>
        /// <exception cref="ArgumentNullException">When <em>other</em> is <b>null</b>.</exception>
        public bool SetEquals (IEnumerable<TKey> other)
        {
            if (other == null)
                throw new ArgumentNullException (nameof (other));

            var oSet = other as RankedSet<TKey> ?? new RankedSet<TKey> (other);

            if (Count != oSet.Count)
                return false;

            foreach (var item in oSet)
                if (! Contains (item))
                    return false;

            return true;
        }


        /// <summary>Modifies the set so that it contains only items that are present either in itself or in the supplied collection, but not both.</summary>
        /// <param name="other">The collection to compare to this set.</param>
        /// <remarks>Not yet implemented.</remarks>
        /// <exception cref="ArgumentNullException">When <em>other</em> is <b>null</b>.</exception>
        public void SymmetricExceptWith (IEnumerable<TKey> other)
        {
            if (other == null)
                throw new ArgumentNullException (nameof (other));

            var oSet = other as RankedSet<TKey> ?? new RankedSet<TKey> (other);
            if (oSet.Count == 0)
                return;

            Enumerator oNum = oSet.GetEnumerator();
            oNum.MoveNext();
            TKey oKey = oNum.Current;

            for (KeyLeaf leaf = LeftmostLeaf; leaf != null; leaf = leaf.RightLeaf)
                for (int klix = 0; klix < leaf.KeyCount; )
                    for (TKey key = leaf.GetKey (klix);;)
                    {
                        int diff = Comparer.Compare (oKey, key);
                        if (diff >= 0)
                        {
                            if (diff > 0)
                                ++klix;
                            else
                            {
                                Remove (key);
                                if (! oNum.MoveNext())
                                    return;
                                oKey = oNum.Current;
                            }
                            break;
                        }

                        Add (oKey);
                        if (! oNum.MoveNext())
                            return;
                        oKey = oNum.Current;

                        if (klix >= leaf.KeyCount)
                        {
                            leaf = leaf.RightLeaf;
                            klix -= leaf.KeyCount;
                            break;
                        }
                    }

            for (;;)
            {
                Add (oKey);
                if (! oNum.MoveNext())
                    return;
                oKey = oNum.Current;
            }
        }


        /// <summary>Add all items in <em>other</em> to this set that are not already in this set.</summary>
        /// <param name="other">The collection to add to this set.</param>
        /// <exception cref="ArgumentNullException">When <em>other</em> is <b>null</b>.</exception>
        public void UnionWith (IEnumerable<TKey> other)
        {
            if (other == null)
                throw new ArgumentNullException (nameof (other));

            foreach (var item in other)
                Add (item);
        }

#endif
        #endregion

        #region Enumerator

        /// <summary>Enumerates the sorted elements of a KeyCollection.</summary>
        public sealed class Enumerator : IEnumerator<TKey>
        {
            private readonly RankedSet<TKey> tree;
            private readonly bool isReverse;
            private KeyLeaf currentLeaf;
            private int leafIndex;

            internal Enumerator (RankedSet<TKey> set, bool reverse=false)
            {
                this.tree = set;
                this.isReverse = reverse;
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

            /// <summary>
            /// Gets the element at the current position of the enumerator.
            /// </summary>
            public TKey Current
            { get { return leafIndex < 0? default (TKey) : currentLeaf.GetKey (leafIndex); } }

            /// <summary>Advances the enumerator to the next element in the collection.</summary>
            /// <returns><b>true</b> if the enumerator was successfully advanced to the next element; <b>false</b> if the enumerator has passed the end of the collection.</returns>
            public bool MoveNext()
            {
                if (currentLeaf != null)
                    if (isReverse)
                    {
                        if (--leafIndex >= 0)
                            return true;

                        currentLeaf = currentLeaf.LeftLeaf;
                        if (currentLeaf != null)
                        { leafIndex = currentLeaf.KeyCount - 1; return true; }
                    }
                    else
                    {
                        if (++leafIndex < currentLeaf.KeyCount)
                            return true;

                        currentLeaf = currentLeaf.RightLeaf;
                        if (currentLeaf != null)
                        { leafIndex = 0; return true; }
                    }

                leafIndex = -1;
                return false;
            }

            void IEnumerator.Reset()
            {
                currentLeaf = isReverse? tree.GetRightmost() : tree.LeftmostLeaf;
                leafIndex = isReverse? currentLeaf.KeyCount : -1;
            }

            /// <summary>Releases all resources used by the Enumerator.</summary>
            public void Dispose() { }
        }

        #endregion

        #region Bonus methods

        /// <summary>Returns a subset range.</summary>
        /// <param name="lower">Minimum item value of range.</param>
        /// <param name="upper">Maximum item value of range.</param>
        /// <returns>An enumerator for all items between <em>lower</em> and <em>upper</em> inclusive.</returns>
        /// <remarks>
        /// Neither <em>lower</em> or <em>upper</em> need to be present in the collection.
        /// </remarks>
        public IEnumerable<TKey> GetBetween (TKey lower, TKey upper)
        {
            var leaf = (KeyLeaf) Find (lower, out int index);

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
                    ++index;
                    continue;
                }

                leaf = leaf.RightLeaf;
                if (leaf == null)
                    yield break;

                index = 0;
            }
        }


        /// <summary>Gets the key at the specified index.</summary>
        /// <param name="index">The zero-based index of the key to get.</param>
        /// <returns>The key at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">When <em>index</em> is less than zero or greater than or equal to the number of keys.</exception>
        public TKey GetByIndex (int index)
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException (nameof (index), "Specified argument was out of the range of valid values.");

            var leaf = (KeyLeaf) Find (ref index);
            return leaf.GetKey (index);
        }


        /// <summary>Gets the index of the specified item.</summary>
        /// <param name="item">The item of the index to get.</param>
        /// <returns>The index of the specified item if found; otherwise the bitwise complement of the insert point.</returns>
        public int IndexOf (TKey item)
        {
            var path = new NodeVector (this, item);
            int result = path.GetIndex();
            return path.IsFound ? result : ~result;
        }

        #endregion
    }
}
