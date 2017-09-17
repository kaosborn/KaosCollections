//
// Library: KaosCollections
// File:    Btree.cs
// Purpose: Define base functionality for RankedDictionary, RankedSet, RankedBag.
//
// Copyright © 2009-2017 Kasey Osborn (github.com/kaosborn)
// MIT License - Use and redistribute freely
//

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

[assembly: CLSCompliant (true)]
namespace Kaos.Collections
{
    /// <summary>Provides base functionality for other classes in this library.</summary>
    /// <typeparam name="T">The type of the keys in the derived class.</typeparam>
    /// <remarks>This class cannot be directly instantiated.</remarks>
    public abstract partial class Btree<T>
    {
        private const int MinimumOrder = 4;
        private const int DefaultOrder = 128;
        private const int MaximumOrder = 256;

        internal Node root;
        internal Leaf rightmostLeaf;
        internal Leaf leftmostLeaf;
        internal IComparer<T> keyComparer;
        internal int maxKeyCount;
        internal int stage = 0;

        internal Btree (Leaf startLeaf)
        {
            this.maxKeyCount = DefaultOrder - 1;
            this.root = this.rightmostLeaf = this.leftmostLeaf = startLeaf;
        }

        internal Btree (IComparer<T> comparer, Leaf startLeaf) : this (startLeaf)
        {
            this.keyComparer = comparer ?? Comparer<T>.Default;
        }

        #region Nonpublic methods

        internal int Size => root.Weight;


        internal void CopyKeysTo1 (T[] array, int index, int count)
        {
            if (array == null)
                throw new ArgumentNullException (nameof (array));

            if (index < 0)
                throw new ArgumentOutOfRangeException (nameof (index), index, "Argument was out of the range of valid values.");

            if (count < 0)
                throw new ArgumentOutOfRangeException (nameof (count), count, "Argument was out of the range of valid values.");

            if (count > array.Length - index)
                throw new ArgumentException ("Destination array is not long enough to copy all the items in the collection. Check array index and length.");

            if (count > Size)
                count = Size;
            int stopIx = index + count;

            for (Leaf leaf = leftmostLeaf; ; leaf = leaf.rightLeaf)
            {
                if (leaf.KeyCount >= stopIx - index)
                {
                    leaf.CopyKeysTo (array, index, stopIx - index);
                    return;
                }

                leaf.CopyKeysTo (array, index, leaf.KeyCount);
                index += leaf.KeyCount;
            }
        }


        internal void CopyKeysTo2 (Array array, int index)
        {
            if (array == null)
                throw new ArgumentNullException (nameof (array));

            if (array.Rank != 1)
                throw new ArgumentException ("Multidimension array is not supported on this operation.", nameof (array));

            if (array.GetLowerBound (0) != 0)
                throw new ArgumentException ("Target array has non-zero lower bound.", nameof (array));

            if (array is T[] genericArray)
            {
                CopyKeysTo1 (genericArray, index, Size);
                return;
            }

            if (index < 0)
                throw new ArgumentOutOfRangeException (nameof (index), "Non-negative number required.");

            if (Size > array.Length - index)
                throw new ArgumentException ("Destination array is not long enough to copy all the items in the collection. Check array index and length.");

            if (array is object[] obArray)
                try
                {
                    for (Leaf leaf = leftmostLeaf; leaf != null; leaf = leaf.rightLeaf)
                        for (int ix = 0; ix < leaf.KeyCount; ++ix)
                            obArray[index++] = leaf.GetKey (ix);
                }
                catch (ArrayTypeMismatchException)
                { throw new ArgumentException ("Mismatched array type.", nameof (array)); }
            else
                throw new ArgumentException ("Invalid array type.", nameof (array));
        }


        /// <summary>Perform lite search for key.</summary>
        /// <param name="key">Target of search.</param>
        /// <param name="index">When found, holds index of returned Leaf; else ~index of nearest greater key.</param>
        /// <returns>Leaf holding target (found or not).</returns>
        internal Leaf Find (T key, out int index)
        {
            //  Unfold on default comparer for 5% speed improvement.
            if (keyComparer == Comparer<T>.Default)
                for (Node node = root;;)
                {
                    index = node.Search (key);

                    if (node is Branch branch)
                        node = branch.GetChild (index < 0 ? ~index : index + 1);
                    else
                        return (Leaf) node;
                }
            else
                for (Node node = root;;)
                {
                    index = node.Search (key, keyComparer);

                    if (node is Branch branch)
                        node = branch.GetChild (index < 0 ? ~index : index + 1);
                    else
                        return (Leaf) node;
                }
        }


        /// <summary>Perform traverse to leaf at index.</summary>
        /// <param name="index">On entry, holds index of collection; on exit holds index of leaf.</param>
        /// <returns>Leaf holding item at index position.</returns>
        internal Node Find (ref int index)
        {
            System.Diagnostics.Debug.Assert (index < Size);
            Node node = root;
            while (node is Branch branch)
                for (int ix = 0; ; ++ix)
                {
                    Node child = branch.GetChild (ix);
                    int cw = child.Weight;
                    if (cw > index)
                    {
                        node = child;
                        break;
                    }
                    index -= cw;
                }

            return node;
        }

        internal int FindEdgeForIndex (T key, out Leaf leaf, out int leafIndex, bool leftEdge=false)
        {
            bool isFound = false;
            int treeIndex = 0;
            leafIndex = 0;

            for (Node node = root;;)
            {
                int hi = node.KeyCount;
                if (leftEdge)
                    for (int lo = 0; lo != hi;)
                    {
                        int mid = (lo + hi) >> 1;
                        int diff = keyComparer.Compare (key, node.GetKey (mid));
                        if (diff <= 0)
                        {
                            if (diff == 0)
                                isFound = true;
                            hi = mid;
                        }
                        else
                            lo = mid + 1;
                    }
                else
                    for (int lo = 0; lo != hi;)
                    {
                        int mid = (lo + hi) >> 1;
                        int diff = keyComparer.Compare (key, node.GetKey (mid));
                        if (diff < 0)
                            hi = mid;
                        else
                        {
                            if (diff == 0)
                                isFound = true;
                            lo = mid + 1;
                        }
                    }

                if (node is Branch branch)
                {
                    if (hi <= branch.KeyCount >> 1)
                        for (int ix = 0; ix < hi; ++ix)
                            treeIndex += branch.GetChild (ix).Weight;
                    else
                    {
                        treeIndex += branch.Weight;
                        for (int ix = hi; ix <= branch.KeyCount; ++ix)
                            treeIndex -= branch.GetChild (ix).Weight;
                    }
                    node = branch.GetChild (hi);
                }
                else
                {
                    leafIndex = hi;
                    leaf = (Leaf) node;
                    return isFound ? treeIndex + hi : ~(treeIndex + hi);
                }
            }
        }

        internal bool FindEdgeLeft (T key, out Leaf leaf, out int leafIndex)
        {
            bool isFound = false;
            leafIndex = 0;

            for (Node node = root;;)
            {
                int hi = node.KeyCount;
                for (int lo = 0; lo != hi;)
                {
                    int mid = (lo + hi) >> 1;
                    int diff = keyComparer.Compare (key, node.GetKey (mid));
                    if (diff <= 0)
                    {
                        if (diff == 0)
                            isFound = true;
                        hi = mid;
                    }
                    else
                        lo = mid + 1;
                }

                if (node is Branch branch)
                    node = branch.GetChild (hi);
                else
                {
                    leafIndex = hi;
                    leaf = (Leaf) node;
                    return isFound;
                }
            }
        }

        internal bool FindEdgeRight (T key, out Leaf leaf, out int leafIndex)
        {
            bool isFound = false;
            leafIndex = 0;

            for (Node node = root;;)
            {
                int hi = node.KeyCount;
                for (int lo = 0; lo != hi; )
                {
                    int mid = (lo + hi) >> 1;
                    int diff = keyComparer.Compare (key, node.GetKey (mid));
                    if (diff < 0)
                        hi = mid;
                    else
                    {
                        if (diff == 0)
                            isFound = true;
                        lo = mid + 1;
                    }
                }

                if (node is Branch branch)
                    node = branch.GetChild (hi);
                else
                {
                    leafIndex = hi;
                    leaf = (Leaf) node;
                    return isFound;
                }
            }
        }


#if NET35 || NET40 || NET45 || SERIALIZE
        [NonSerialized]
#endif
        private object syncRoot = null;
        internal object GetSyncRoot()
        {
            if (syncRoot == null)
                Interlocked.CompareExchange (ref syncRoot, new object(), null);
            return syncRoot;
        }


        internal void Initialize()
        {
            StageBump();
            leftmostLeaf.Truncate (0);
            leftmostLeaf.rightLeaf = null;
            root = rightmostLeaf = leftmostLeaf;
        }


        internal void Remove2 (NodeVector path)
        {
            StageBump();

            ((Leaf) path.TopNode).Remove (path.TopIndex);
            path.DecrementPathWeight();
            Balance (path);
        }


        internal void RemoveRange2 (NodeVector path1, NodeVector path2)
        {
            var leaf1 = (Leaf) path1.TopNode; int ix1 = path1.TopIndex, deltaW1 = 0;
            var leaf2 = (Leaf) path2.TopNode; int ix2 = path2.TopIndex, deltaW2 = 0;
            int j1 = ix1 == 0 && leaf1.leftLeaf == null ? 0 : 1;
            int j2 = ix2 == leaf2.KeyCount ? 0 : 1;

            if (j1 != 0 && ix1 == 0)
            { leaf1 = leaf1.leftLeaf; ix1 = leaf1.KeyCount; path1.TraverseLeft(); }
            if (leaf1 == leaf2)
            {
                path2.ChangePathWeight (ix1-ix2);
                leaf2.RemoveRange (ix1, ix2-ix1);
                if (ix1 == 0) path2.SetPivot (leaf2.Key0);
                Balance (path2);
                return;
            }

            if (j2 == 0)
            { leaf1.rightLeaf = null; rightmostLeaf = leaf1; }
            else
            {   deltaW2 = -ix2; if (j1!=0) leaf1.rightLeaf = leaf2; leaf2.RemoveRange (0, ix2);
                if (j1 == 0)
                { leaf2.leftLeaf = null; leftmostLeaf = leaf2; }
                else
                { if (ix2 != 0) path2.SetPivot (leaf2.Key0);
                  deltaW1 = ix1-leaf1.KeyCount; leaf2.leftLeaf = leaf1; leaf1.RemoveRange (ix1, leaf1.KeyCount-ix1); }
            }
            for (int level = path2.Height-2; level >= 0; --level)
            {
                var bh1 = (Branch) path1.GetNode (level); ix1 = path1.GetIndex (level);
                var bh2 = (Branch) path2.GetNode (level); ix2 = path2.GetIndex (level);
                if (bh1 == bh2)
                {
                    for (int ix = ix1+j1; ix < ix2+1-j2; ++ix) deltaW1 -= bh2.GetChild(ix).Weight;
                    if (j1 == 0)
                        bh2.RemoveChildRange2 (0, ix2-ix1);
                    else
                        bh2.RemoveChildRange1 (ix1, ix2-ix1-j2);
                    bh2.AdjustWeight (deltaW1 + deltaW2);
                }
                else
                {
                    if (j1 != 0)
                    {
                        if (ix1 <= bh1.KeyCount)
                        {
                            for (int ix = ix1+1; ix <= bh1.KeyCount; ++ix) deltaW1 -= bh1.GetChild(ix).Weight;
                            bh1.RemoveChildRange1 (ix1, bh1.KeyCount - ix1);
                        }
                        bh1.AdjustWeight (deltaW1);
                    }
                    if (j2 != 0)
                    {
                        if (ix2 > 0)
                        {
                            for (int ix = 0; ix < ix2; ++ix) deltaW2 -= bh2.GetChild(ix).Weight;
                            path2.SetPivot (bh2.GetKey (ix2-1), level);
                            bh2.RemoveChildRange2 (0, ix2);
                        }
                        bh2.AdjustWeight (deltaW2);
                    }
                }
            }

            if (j1 != 0)
                BalanceLeaf (path1);  //TODO still underfilled

            while (root is Branch && root.KeyCount == 0)
                root = ((Branch) root).Child0;
        }


        internal void Balance (NodeVector path)
        {
            BalanceLeaf (path);
            while (root is Branch && root.KeyCount == 0)
                root = ((Branch) root).Child0;
        }


        internal void BalanceLeaf (NodeVector path)
        {
            int leafIndex = path.TopIndex;
            var leaf = (Leaf) path.TopNode;

            if (leafIndex == 0)
                if (leaf.KeyCount != 0)
                    path.SetPivot (path.TopNode.Key0);
                else
                {
                    if (leaf.rightLeaf != null)
                    {
                        leaf.rightLeaf.leftLeaf = leaf.leftLeaf;
                        if (leaf.leftLeaf != null)
                            leaf.leftLeaf.rightLeaf = leaf.rightLeaf;
                        else
                            leftmostLeaf = leaf.rightLeaf;
                        path.Demote();
                    }
                    else if (leaf.leftLeaf != null)
                    {
                        leaf.leftLeaf.rightLeaf = leaf.rightLeaf;
                        rightmostLeaf = leaf.leftLeaf;
                        path.Demote();
                    }

                    return;
                }

            // Leaf underflow?
            if (leaf.KeyCount < (maxKeyCount + 1) / 2)
            {
                Leaf rightLeaf = leaf.rightLeaf;
                if (rightLeaf != null)
                    if (leaf.KeyCount + rightLeaf.KeyCount > maxKeyCount)
                    {
                        // Balance leaves by shifting pairs from right leaf.
                        int shifts = (leaf.KeyCount + rightLeaf.KeyCount + 1) / 2 - leaf.KeyCount;
                        leaf.Shift (shifts);
                        path.TraverseRight();
                        path.SetPivot (rightLeaf.Key0);
                        path.TiltLeft (shifts);
                    }
                    else
                    {
                        leaf.Coalesce();
                        leaf.rightLeaf = rightLeaf.rightLeaf;
                        if (rightLeaf.rightLeaf == null)
                            rightmostLeaf = leaf;
                        else
                            rightLeaf.rightLeaf.leftLeaf = leaf;
                        path.TraverseRight();
                        path.TiltLeft (rightLeaf.KeyCount);
                        path.Demote();
                    }
            }
        }


        internal int RemoveWhere2 (Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentNullException (nameof (match));

            int result = 0;
            int stageFreeze = stage;

            for (Leaf leaf = rightmostLeaf; leaf != null; leaf = leaf.leftLeaf)
                for (int ix = leaf.KeyCount-1; ix >= 0; --ix)
                {
                    T key = leaf.GetKey (ix);
                    if (match (key))
                    {
                        StageCheck (stageFreeze);
                        var path = new NodeVector (this, key);
                        if (path.IsFound)
                        {
                            Remove2 (path);
                            stageFreeze = stage;
                            ++result;
                        }
                    }
                }

            return result;
        }


        /// <exclude />
        protected void StageBump()
        { ++stage; }

        /// <exclude />
        protected void StageCheck (int expected)
        {
            if (stage != expected)
                throw new InvalidOperationException ("Operation is not valid because collection was modified.");
        }

        #endregion

        #region Properties and methods

        /// <summary>Gets or sets the <em>order</em> of the underlying B+ tree structure.</summary>
        /// <remarks>
        /// <para>
        /// The <em>order</em> of a tree (also known as branching factor) is the maximum number of child nodes that a branch may reference.
        /// The minimum number of child node references for a non-rightmost branch is <em>order</em>/2.
        /// The maximum number of elements in a leaf is <em>order</em>-1.
        /// The minimum number of elements in a non-rightmost leaf is <em>order</em>/2.
        /// </para>
        /// <para>
        /// Changing this property may degrade performance and is provided for experimental purposes only.
        /// The default value of 128 should always be adequate.
        /// </para>
        /// <para>
        /// Attempts to set this value when <em>Count</em> is non-zero are ignored.
        /// Non-negative values below 4 or above 256 are ignored.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">When supplied value is less than zero.</exception>
        public int Capacity
        {
            get { return maxKeyCount + 1; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException ("Must be between " + MinimumOrder + " and " + MaximumOrder + ".");

                if (Size == 0 && value >= MinimumOrder && value <= MaximumOrder)
                    maxKeyCount = value - 1;
            }
        }

        #endregion

        #region Debug methods
#if DEBUG

        // Telemetry counters:
        /// <summary>Maximum number of keys that the existing branches can hold.</summary>
        public int BranchSlotCount { get; private set; }
        /// <summary>Number of keys contained in the branches.</summary>
        public int BranchSlotsUsed { get; private set; }
        /// <summary>Maximum number of keys that the existing leaves can hold.</summary>
        public int LeafSlotCount { get; private set; }
        /// <summary>Number of keys contained in the leaves.</summary>
        public int LeafSlotsUsed { get; private set; }


        /// <summary>Perform diagnostics check for data structure sanity errors.</summary>
        /// <remarks>
        /// Since this is an in-memory managed structure, any errors would indicate a bug.
        /// Also performs space complexity diagnostics to ensure that all non-rightmost nodes maintain 50% fill.
        /// </remarks>
        public void SanityCheck()
        {
            BranchSlotCount = 0;
            BranchSlotsUsed = 0;
            LeafSlotCount = 0;
            LeafSlotsUsed = 0;

            Leaf lastLeaf;
            if (root is Branch)
                lastLeaf = CheckBranch ((Branch) root, 1, GetHeight(), true, default (T), null);
            else
                lastLeaf = CheckLeaf ((Leaf) root, true, default (T), null);

            if (lastLeaf.rightLeaf != null)
                throw new InvalidOperationException ("Last leaf has invalid RightLeaf");

            if (root.Weight != LeafSlotsUsed)
                throw new InvalidOperationException ("Mismatched Count=" + root.Weight + ", expected=" + LeafSlotsUsed);

            if (leftmostLeaf.leftLeaf != null)
                throw new InvalidOperationException ("leftmostLeaf has a left leaf");

            if (rightmostLeaf.rightLeaf != null)
                throw new InvalidOperationException ("rightmostLeaf has a right leaf");
        }


        /// <summary>Gets maximum number of children of a branch.</summary>
        /// <returns>Maximum number of children of a branch.</returns>
        public int GetOrder() => maxKeyCount + 1;


        /// <summary>Gets the number of levels in the tree.</summary>
        /// <returns>Number of levels in the tree.</returns>
        public int GetHeight()
        {
            int level = 1;
            for (Node node = root; node is Branch; node = ((Branch) node).Child0)
                ++level;
            return level;
        }


        private Leaf CheckBranch
        (
            Branch branch,
            int level, int height,
            bool isRightmost,
            T anchor,  // ignored when isRightmost true
            Leaf visited
        )
        {
            BranchSlotCount += maxKeyCount;
            BranchSlotsUsed += branch.KeyCount;

            if (! isRightmost && (branch.KeyCount + 1) < maxKeyCount / 2)
                throw new InvalidOperationException ("Branch underfilled");

            if (branch.ChildCount != branch.KeyCount + 1)
                throw new InvalidOperationException ("Branch mismatched ChildCount, KeyCount");

            int actualWeight = 0;
            for (int i = 0; i < branch.ChildCount; ++i)
            {
                T anchor0 = i == 0 ? anchor : branch.GetKey (i - 1);
                bool isRightmost0 = isRightmost && i < branch.ChildCount;
                if (i < branch.KeyCount - 1)
                    if (keyComparer.Compare (branch.GetKey (i), branch.GetKey (i + 1)) > 0)
                        throw new InvalidOperationException ("Branch keys descending");

                if (level + 1 < height)
                    visited = CheckBranch ((Branch) branch.GetChild (i), level+1, height, isRightmost0, anchor0, visited);
                else
                    visited = CheckLeaf ((Leaf) branch.GetChild (i), isRightmost0, anchor0, visited);

                actualWeight += branch.GetChild (i).Weight;
            }
            if (branch.Weight != actualWeight)
                throw new InvalidOperationException ("Branch mismatched weight");

            return visited;
        }


        private Leaf CheckLeaf (Leaf leaf, bool isRightmost, T anchor, Leaf visited)
        {
            LeafSlotCount += maxKeyCount;
            LeafSlotsUsed += leaf.KeyCount;

            //if (leaf.rightLeaf != null && leaf.KeyCount < (maxKeyCount + 1) / 2)
            //TODO     throw new InvalidOperationException ("Leaf underflow");

            if (! anchor.Equals (default (T)) && ! anchor.Equals (leaf.Key0))
                throw new InvalidOperationException ("Leaf has wrong anchor");

            for (int i = 0; i < leaf.KeyCount; ++i)
                if (i < leaf.KeyCount - 1 && keyComparer.Compare (leaf.GetKey (i), leaf.GetKey (i + 1)) > 0)
                    throw new InvalidOperationException ("Leaf keys descending");

            if (visited == null)
            {
                if (! anchor.Equals (default (T)))
                    throw new InvalidOperationException ("Inconsistent visited, anchor");
            }
            else
                if (visited.rightLeaf != leaf)
                    throw new InvalidOperationException ("Leaf has bad RightLeaf");

            return leaf;
        }


        /// <summary>Gets telemetry summary.</summary>
        /// <returns>Telemetry summary.</returns>
        public string GetTreeStatsText()
        {
            SanityCheck();
            string result = "--- height = " + GetHeight();

            if (BranchSlotCount != 0)
                result += ", branch fill = " + (int) (BranchSlotsUsed * 100.0 / BranchSlotCount + 0.5) + "%";

            return result + ", leaf fill = " + (int) (LeafSlotsUsed * 100.0 / LeafSlotCount + 0.5) + "%";
        }


        /// <summary>Generates content of tree by level (breadth first).</summary>
        /// <returns>Text lines where each line is a level of the tree.</returns>
        public IEnumerable<string> GenerateTreeText (bool showWeight=false)
        {
            int level = 0;
            Node leftmost;
            var sb = new StringBuilder();

            for (;;)
            {
                var branchPath = new NodeVector (this, level);
                leftmost = branchPath.TopNode;
                if (leftmost is Leaf)
                    break;

                var branch = (Branch) leftmost;

                sb.Append ('B');
                sb.Append (level);
                sb.Append (": ");
                for (;;)
                {
                    branch.Append (sb);
                    if (showWeight)
                    {
                        sb.Append (" (");
                        sb.Append (branch.Weight);
                        sb.Append (") ");
                    }

                    branch = (Branch) branchPath.TraverseRight();
                    if (branch == null)
                        break;

                    sb.Append (" | ");
                }
                ++level;
                yield return sb.ToString();
                sb.Length = 0;
            }

            var leafPath = new NodeVector (this, level);
            sb.Append ('L');
            sb.Append (level);
            sb.Append (": ");
            for (var leaf = (Leaf) leftmost;;)
            {
                leaf.Append (sb);
                leaf = (Leaf) leafPath.TraverseRight();
                if (leaf == null)
                    break;

                if (leafPath.Child0)
                    sb.Append (" | ");
                else
                    sb.Append ('|');
            }
            yield return sb.ToString();
        }

#endif
        #endregion
    }
}
