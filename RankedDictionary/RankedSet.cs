//
// Library: KaosCollections
// File:    RankedSet.cs
// Purpose: Defines BtreeDictionary generic API.
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
    [DebuggerDisplay ("Count = {Count}")]
    public partial class RankedSet<TKey> : Btree<TKey>,
#if ! NET35
        ISet<TKey>,
#endif
        ICollection<TKey>,
        ICollection
#if NETSTANDARD1_0
        , IReadOnlyCollection<TKey>
#endif
        where TKey : IComparable  //TODO remove where
    {
        private KeyLeaf LeftmostLeaf { get { return leftmostLeaf; } }

        public RankedSet() : this (DefaultOrder, Comparer<TKey>.Default)
        { }

        public RankedSet (int order) : this (order, Comparer<TKey>.Default)
        { }

        public RankedSet (int order, IComparer<TKey> comparer) : base (order, comparer, new KeyLeaf())
        { this.root = this.leftmostLeaf; }

        public RankedSet (IComparer<TKey> comparer) : this (DefaultOrder, comparer)
        { }

        #region Public properties

        public int Count
        { get { return root.Weight; } }

        bool ICollection<TKey>.IsReadOnly
        { get { return false; } }

        bool ICollection.IsSynchronized
        { get { return false; } }

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

        public TKey Min
        {
            get
            {
                if (Count == 0)
                    return default (TKey);
                return LeftmostLeaf.Key0;
            }
        }

        object ICollection.SyncRoot => throw new NotImplementedException ();

        #endregion

        public void Clear()
        {
            leftmostLeaf.Truncate (0);
            leftmostLeaf.rightKeyLeaf = null;
            root = leftmostLeaf;
        }


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

        private void Add2 (NodeVector nv, TKey key)
        {
            var leaf = (KeyLeaf) nv.TopNode;
            int pathIndex = nv.TopNodeIndex;

            nv.UpdateWeight (1);
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
            nv.Promote (newLeaf.Key0, (Node) newLeaf, newLeaf.RightLeaf == null);
        }


        public bool Contains (TKey key)
        {
            KeyLeaf leaf = Find (key, out int index);
            return index >= 0;
        }


        public void CopyTo (TKey[] array)
        { CopyTo (array, 0, Count); }

        public void CopyTo (TKey[] array, int index)
        { CopyTo (array, index, Count); }

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

        IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator()
        {
            return new Enumerator (this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator (this);
        }

        #region ISET implementation

        #if ! NET35
        public void ExceptWith (IEnumerable<TKey> other)
        {
            if (other == null)
                throw new ArgumentNullException (nameof(other));

            if (Count == 0)
                return;

            if (other == this)
            {
                Clear();
                return;
            }

            SortedSet<TKey> other2 = other as SortedSet<TKey>;

            if (other2 == null)
            {
                Clear();
                return;
            }

            throw new NotImplementedException();
        }

        public void IntersectWith (IEnumerable<TKey> other)
        {
            throw new NotImplementedException ();
        }

        public bool IsProperSubsetOf (IEnumerable<TKey> other)
        {
            throw new NotImplementedException ();
        }

        public bool IsProperSupersetOf (IEnumerable<TKey> other)
        {
            throw new NotImplementedException ();
        }

        public bool IsSubsetOf (IEnumerable<TKey> other)
        {
            throw new NotImplementedException ();
        }

        public bool IsSupersetOf (IEnumerable<TKey> other)
        {
            throw new NotImplementedException ();
        }

        public bool Overlaps (IEnumerable<TKey> other)
        {
            throw new NotImplementedException ();
        }

        public bool SetEquals (IEnumerable<TKey> other)
        {
            throw new NotImplementedException ();
        }

        public void SymmetricExceptWith (IEnumerable<TKey> other)
        {
            throw new NotImplementedException ();
        }

        public void UnionWith (IEnumerable<TKey> other)
        {
            throw new NotImplementedException ();
        }

#endif
        #endregion

        /// <summary>Enumerates the sorted elements of a KeyCollection.</summary>
        public sealed class Enumerator : IEnumerator<TKey>
        {
            private readonly RankedSet<TKey> tree;
            private KeyLeaf currentLeaf;
            private int leafIndex;

            internal Enumerator (RankedSet<TKey> set)
            {
                this.tree = set;
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

            void IEnumerator.Reset()
            {
                leafIndex = -1;
                currentLeaf = tree.LeftmostLeaf;
            }

            /// <summary>Releases all resources used by the Enumerator.</summary>
            public void Dispose() { }
        }

    }
}
