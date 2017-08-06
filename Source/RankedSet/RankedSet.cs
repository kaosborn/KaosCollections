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
#if NET35 || NET40 || SERIALIZE
using System.Runtime.Serialization;
#endif

namespace Kaos.Collections
{
    /// <summary>Represents a collection of sorted, unique items.</summary>
    /// <typeparam name="T">The type of the items in the set.</typeparam>
    /// <remarks>
    /// This class emulates and extends the
    /// <see cref="System.Collections.Generic.SortedSet{T}"/> class.
    /// </remarks>
#if NET35 || NET40 || SERIALIZE
    [Serializable]
#endif
    [DebuggerTypeProxy (typeof (ICollectionDebugView<>))]
    [DebuggerDisplay ("Count = {Count}")]
    public class RankedSet<T> :
        Btree<T>
#if ! NET35
        , ISet<T>
#endif
        , ICollection<T>
        , ICollection
#if ! NET35 && ! NET40
        , IReadOnlyCollection<T>
#endif
#if NET35 || NET40 || SERIALIZE
        , ISerializable, IDeserializationCallback
#endif
    {
        #region Constructors

        /// <summary>Initializes a new set of sorted items that uses the default item comparer.</summary>
        public RankedSet() : base (Comparer<T>.Default, new Leaf())
        { }

        /// <summary>Initializes a new set of sorted items that uses the supplied comparer.</summary>
        /// <param name="comparer">The comparer to use for sorting items.</param>
        /// <exception cref="InvalidOperationException">When <em>comparer</em> is <b>null</b> and no other comparer available.</exception>
        /// <example>
        /// This program shows usage of a custom comparer combined with serialization.
        /// Note: Serialization is is not supported in .NET Standard 1.0.
        /// <code source="..\Bench\RsExample05\RsExample05.cs" lang="cs" />
        /// </example>
        public RankedSet (IComparer<T> comparer) : base (comparer, new Leaf())
        { }

        /// <summary>Initializes a new set that contains items copied from the supplied collection.</summary>
        /// <param name="collection">The enumerable collection to be copied.</param>
        /// <remarks>
        /// This constructor is a O(<em>n</em> log <em>n</em>) operation, where <em>n</em> is the number of items.
        /// </remarks>
        /// <exception cref="ArgumentNullException">When <em>collection</em> is <b>null</b>.</exception>
        public RankedSet (IEnumerable<T> collection) : this (collection, Comparer<T>.Default)
        { }

        /// <summary>Initializes a new set that contains items copied from the supplied collection.</summary>
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

        bool ICollection<T>.IsReadOnly => false;

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => GetSyncRoot();

        #endregion

        #region Methods

        /// <summary>Adds an item to the set and returns a success indicator.</summary>
        /// <param name="item">The item to add.</param>
        /// <returns><b>true</b> if <em>item</em> was added to the set; otherwise <b>false</b>.</returns>
        /// <remarks>
        /// <para>
        /// If <em>item</em> is already in the set, this method returns <b>false</b> and does not throw an exception.
        /// </para>
        /// <para>This is a O(log <em>n</em>) operation.</para>
        /// </remarks>
        public bool Add (T item)
        {
            var path = new NodeVector (this, item);
            if (path.IsFound)
                return false;

            Add2 (path, item);
            return true;
        }

        void ICollection<T>.Add (T item)
        { Add (item); }

        private void Add2 (NodeVector path, T key)
        {
            StageBump();

            var leaf = (Leaf) path.TopNode;
            int pathIndex = path.TopIndex;

            path.UpdateWeight (1);
            if (leaf.KeyCount < maxKeyCount)
            {
                leaf.Insert (pathIndex, key);
                return;
            }

            // Leaf is full so right split a new leaf.
            var newLeaf = new Leaf (leaf, maxKeyCount);

            if (newLeaf.rightLeaf != null)
                newLeaf.rightLeaf.leftLeaf = newLeaf;
            else
            {
                rightmostLeaf = newLeaf;

                if (pathIndex == leaf.KeyCount)
                {
                    newLeaf.AddKey (key);
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

            // Promote anchor of split leaf.
            path.Promote (newLeaf.Key0, (Node) newLeaf, newLeaf.rightLeaf == null);
        }


        /// <summary>Determines whether the set contains a supplied item.</summary>
        /// <param name="item">The item to check for existence in the set.</param>
        /// <returns><b>true</b> if the set contains <em>item</em>; otherwise <b>false</b>.</returns>
        public bool Contains (T item)
        {
            Leaf leaf = Find (item, out int index);
            return index >= 0;
        }


        /// <summary>Copies the set to a compatible array, starting at the beginning of the target array.</summary>
        /// <param name="array">A one-dimensional array that is the destination of the items to copy from the set.</param>
        /// <remarks>This is a O(<em>n</em>) operation.</remarks>
        /// <exception cref="ArgumentNullException">When <em>array</em> is <b>null</b>.</exception>
        /// <exception cref="ArgumentException">When not enough space is available for the copy.</exception>
        public void CopyTo (T[] array)
        { CopyTo (array, 0, Count); }

        /// <summary>Copies the set to a compatible array, starting at the supplied position.</summary>
        /// <param name="array">A one-dimensional array that is the destination of the items to copy from the set.</param>
        /// <param name="index">The zero-based starting position.</param>
        /// <remarks>This is a O(<em>n</em>) operation.</remarks>
        /// <exception cref="ArgumentNullException">When <em>array</em> is <b>null</b>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">When <em>index</em> is less than zero.</exception>
        /// <exception cref="ArgumentException">When not enough space is available for the copy.</exception>
        public void CopyTo (T[] array, int index)
        { CopyTo (array, index, Count); }

        /// <summary>Copies a supplied number of items to a compatible array, starting at the supplied position.</summary>
        /// <param name="array">A one-dimensional array that is the destination of the items to copy from the set.</param>
        /// <param name="index">The zero-based starting position.</param>
        /// <param name="count">The number of items to copy.</param>
        /// <remarks>This is a O(<em>n</em>) operation.</remarks>
        /// <exception cref="ArgumentNullException">When <em>array</em> is <b>null</b>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">When <em>index</em> or <em>count</em> is less than zero.</exception>
        /// <exception cref="ArgumentException">When not enough space is available for the copy.</exception>
        public void CopyTo (T[] array, int index, int count)
        {
            if (array == null)
                throw new ArgumentNullException (nameof (array));

            if (index < 0)
                throw new ArgumentOutOfRangeException (nameof (index), index, "Argument was out of the range of valid values.");

            if (count < 0)
                throw new ArgumentOutOfRangeException (nameof (count), count, "Argument was out of the range of valid values.");

            if (count > array.Length - index)
                throw new ArgumentException ("Destination array is not long enough to copy all the items in the collection. Check array index and length.");

            for (Leaf leaf = leftmostLeaf; count > 0; )
            {
                int limIx = count < leaf.KeyCount ? count : leaf.KeyCount;

                for (int ix = 0; ix < limIx; ++ix)
                    array[index++] = leaf.GetKey (ix);

                leaf = leaf.rightLeaf;
                if (leaf == null)
                    break;

                count -= limIx;
            }
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

            if (array is T[] genArray)
            {
                CopyTo (genArray, index);
                return;
            }

            if (array is object[] obArray)
            {
                try
                {
                    int ix = 0;
                    foreach (var key in this)
                        obArray[ix++] = key;
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


        /// <summary>Removes an item from the set.</summary>
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


        /// <summary>Removes all items that match the condition defined by the supplied predicate.</summary>
        /// <param name="match">The condition of the items to remove.</param>
        /// <returns>The number of items removed from the set.</returns>
        /// <remarks>
        /// This is a O(<em>n</em> log <em>m</em>) operation
        /// where <em>m</em> is the count of items removed and <em>n</em> is the size of the set.
        /// </remarks>
        /// <exception cref="ArgumentNullException">When <em>match</em> is <b>null</b>.</exception>
        public int RemoveWhere (Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentNullException (nameof (match));

            int delCount = 0;

            for (Leaf leaf = rightmostLeaf; leaf != null; leaf = leaf.leftLeaf)
                for (int ix = leaf.KeyCount-1; ix >= 0; --ix)
                {
                    T key = leaf.GetKey (ix);
                    if (match (key))
                    {
                        ++delCount;
                        bool isOk = Remove (key);
                    }
                }

            return delCount;
        }


        /// <summary>Returns an IEnumerable that iterates over the set in reverse order.</summary>
        /// <returns>An enumerator that reverse iterates over the set.</returns>
        public IEnumerable<T> Reverse()
        {
            Enumerator enor = new Enumerator (this, isReverse:true);
            while (enor.MoveNext())
                yield return enor.Current;
        }


        IEnumerator<T> IEnumerable<T>.GetEnumerator()
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


        /// <summary>Determines whether the set and a supplied collection share common elements.</summary>
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
                    if (Comparer.Compare (oSet.Max, Min) >= 0 && Comparer.Compare (oSet.Min, Max) <= 0)
                        foreach (T key in oSet.GetBetween (Min, Max))
                            if (Contains (key))
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

                        if (ix >= leaf.KeyCount)
                        {
                            leaf = leaf.rightLeaf;
                            ix -= leaf.KeyCount;
                            break;
                        }
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

#endif
        #endregion

        #region Enumerator

        /// <summary>Enumerates the sorted elements of a KeyCollection.</summary>
        public sealed class Enumerator : IEnumerator<T>
        {
            private readonly RankedSet<T> tree;
            private readonly bool isReverse;
            private Leaf leaf;
            private int index;
            private int stageFreeze;
            private int state;  // -1=rewound; 0=active; 1=consumed

            internal Enumerator (RankedSet<T> set, bool isReverse=false)
            {
                this.tree = set;
                this.isReverse = isReverse;
                ((IEnumerator) this).Reset();
            }

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

            /// <summary>Gets the element at the current position of the enumerator.</summary>
            /// <exception cref="InvalidOperationException">When the set was modified after the enumerator was created.</exception>
            public T Current
            {
                get
                {
                    tree.StageCheck (stageFreeze);
                    return state != 0 ? default (T) : leaf.GetKey (index);
                }
            }

            /// <summary>Advances the enumerator to the next element in the collection.</summary>
            /// <returns><b>true</b> if the enumerator was successfully advanced to the next element; <b>false</b> if the enumerator has passed the end of the collection.</returns>
            /// <exception cref="InvalidOperationException">When the set was modified after the enumerator was created.</exception>
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

            void IEnumerator.Reset()
            {
                stageFreeze = tree.stage;
                state = -1;
            }

            /// <summary>Releases all resources used by the Enumerator.</summary>
            public void Dispose() { }
        }

        #endregion

        #region ISerializable
#if NET35 || NET40 || SERIALIZE

        private SerializationInfo serializationInfo;
        private RankedSet (SerializationInfo info, StreamingContext context) : base (new Btree<T>.Leaf())
        {
            this.serializationInfo = info;
        }

        /// <summary>Populates a SerializationInfo with target data.</summary>
        /// <param name="info">The SerializationInfo to populate.</param>
        /// <param name="context">The destination.</param>
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

        void ISerializable.GetObjectData (SerializationInfo info, StreamingContext context)
        { GetObjectData(info, context); }

        void IDeserializationCallback.OnDeserialization (Object sender)
        { OnDeserialization (sender); }

#endif
        #endregion

        #region Bonus methods

        /// <summary>Returns a subset range.</summary>
        /// <param name="lower">Minimum item value of range.</param>
        /// <param name="upper">Maximum item value of range.</param>
        /// <returns>An enumerator for all items between <em>lower</em> and <em>upper</em> inclusive.</returns>
        /// <remarks>
        /// <para>Neither <em>lower</em> or <em>upper</em> need to be present in the collection.</para>
        /// <para>
        /// Retrieving the initial item is a O(log <em>n</em>) operation.
        /// Retrieving each subsequent item is a O(1) operation.
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">When the set was modified after the enumerator was created.</exception>
        public IEnumerable<T> GetBetween (T lower, T upper)
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


        /// <summary>Gets the key at the supplied index.</summary>
        /// <param name="index">The zero-based index of the key to get.</param>
        /// <returns>The key at the supplied index.</returns>
        /// <remarks>This is a O(log <em>n</em>) operation.</remarks>
        /// <exception cref="ArgumentOutOfRangeException">When <em>index</em> is less than zero or greater than or equal to the number of keys.</exception>
        public T GetByIndex (int index)
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException (nameof (index), "Argument was out of the range of valid values.");

            var leaf = (Leaf) Find (ref index);
            return leaf.GetKey (index);
        }


        /// <summary>Provides range query support with ordered results.</summary>
        /// <param name="item">Minimum value of range.</param>
        /// <returns>An enumerator for the set for items greater than or equal to <em>item</em>.</returns>
        /// <remarks>
        /// <para>
        /// Retrieving the initial item is a O(log <em>n</em>) operation.
        /// Retrieving each subsequent item is a O(1) operation.
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">When the set was modified after the enumerator was created.</exception>
        public IEnumerable<T> GetFrom (T item)
        {
            int stageFreeze = stage;
            var leaf = (Leaf) Find (item, out int index);

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
        /// <param name="item">The item of the index to get.</param>
        /// <returns>The index of <em>item</em> if found; otherwise the bitwise complement of the insert point.</returns>
        /// <remarks>This is a O(log <em>n</em>) operation.</remarks>
        public int IndexOf (T item)
        {
            var path = new NodeVector (this, item);
            int result = path.GetIndex();
            return path.IsFound ? result : ~result;
        }

        #endregion
    }
}
