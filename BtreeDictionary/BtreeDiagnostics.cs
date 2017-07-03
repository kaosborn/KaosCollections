//
// Library: KaosCollections
// File:    BtreeDiagnostics.cs
// Purpose: Define BtreeDictionary API for Debug build only.
//
// Copyright © 2009-2017 Kasey Osborn (github.com/kaosborn)
// MIT License - Use and redistribute freely
//

using System;
using System.Collections.Generic;
using System.Text;

namespace Kaos.Collections
{
#if DEBUG
    public partial class BtreeDictionary<TKey, TValue>
    {
        // Telemetry counters:
        public int BranchSlotCount { get; private set; }
        public int BranchSlotsUsed { get; private set; }
        public int LeafSlotCount { get; private set; }
        public int LeafSlotsUsed { get; private set; }


        /// <summary>
        /// Perform diagnostics check for data structure sanity errors. Since this is an
        /// in-memory managed structure, any errors would indicate a bug. Also performs space
        /// complexity diagnostics to ensure that all non-rightmost nodes maintain 50% fill.
        /// </summary>
        /// </exclude>
        public void SanityCheck()
        {
            BranchSlotCount = 0;
            BranchSlotsUsed = 0;
            LeafSlotCount = 0;
            LeafSlotsUsed = 0;

            Leaf lastLeaf = Check (root, 1, GetHeight(), true, default (TKey), null);

            if (lastLeaf.RightLeaf != null)
                throw new InvalidOperationException ("Last leaf has invalid RightLeaf");

            if (Count != LeafSlotsUsed)
                throw new InvalidOperationException ("Mismatched Count=" + Count + ", expected=" + LeafSlotsUsed);
        }


        /// <summary>Maximum number of children of a branch.</summary>
        public int GetOrder()
        { return maxKeyCount + 1; }


        public int GetHeight()
        {
            int depth = 2;
            for (Branch branch = root;;)
            {
                Node node = branch.FirstChild;
                if (node is Leaf)
                    return depth;
                ++depth;
                branch = (Branch) node;
            }
        }


        private Leaf Check
        (
            Branch branch,
            int level, int height,
            bool isRightmost,
            TKey anchor,  // ignored when isRightmost true
            Leaf visited
        )
        {
            BranchSlotCount += maxKeyCount;
            BranchSlotsUsed += branch.KeyCount;

            if (! isRightmost && (branch.KeyCount + 1) < maxKeyCount / 2)
                throw new InvalidOperationException ("Branch underfilled");

            if (branch.ChildCount != branch.KeyCount + 1)
                throw new InvalidOperationException ("Branch ChildCount wrong");

            for (int i = 0; i < branch.ChildCount; ++i)
            {
                TKey anchor0 = i == 0 ? anchor : branch.GetKey (i - 1);
                bool isRightmost0 = isRightmost && i < branch.ChildCount;
                if (i < branch.KeyCount - 1)
                    if (branch.GetKey (i).CompareTo (branch.GetKey (i + 1)) >= 0)
                        throw new InvalidOperationException ("Branch keys not ascending");

                if (level + 1 < height)
                {
                    var child = (Branch) branch.GetChild (i);
                    visited = Check (child, level + 1, height, isRightmost0, anchor0, visited);
                }
                else
                {
                    var leaf = (Leaf) branch.GetChild (i);
                    visited = Check (leaf, isRightmost0, anchor0, visited);
                }
            }
            return visited;
        }


        private Leaf Check (Leaf leaf, bool isRightmost, TKey anchor, Leaf visited)
        {
            LeafSlotCount += maxKeyCount;
            LeafSlotsUsed += leaf.KeyCount;

            if (leaf.RightLeaf != null && leaf.KeyCount < (maxKeyCount + 1) / 2)
                throw new InvalidOperationException ("Leaf underfilled");

            if (! anchor.Equals (default (TKey)) && ! anchor.Equals (leaf.GetKey (0)))
                throw new InvalidOperationException ("Leaf has wrong anchor");

            for (int i = 0; i < leaf.KeyCount; ++i)
                if (i < leaf.KeyCount - 1 && leaf.GetKey (i).CompareTo (leaf.GetKey (i + 1)) >= 0)
                    throw new InvalidOperationException ("Leaf keys not ascending");

            if (visited == null)
            {
                if (! anchor.Equals (default (TKey)))
                    throw new InvalidOperationException ("Inconsistent visited, anchor");
            }
            else
                if (visited.RightLeaf != leaf)
                    throw new InvalidOperationException ("Leaf has bad RightLeaf");

            return leaf;
        }


        /// <summary>
        /// Display contents of tree by level (breadth first).
        /// </summary>
        /// </exclude>
        public IEnumerable<string> GenerateTreeText()
        {
            int level = 0;
            Node first;
            var sb = new StringBuilder();

            for (;;)
            {
                var branchPath = new NodeVector (this, level);
                first = branchPath.TopNode;
                if (first is Leaf)
                    break;

                var branch = (Branch) first;

                sb.Append ('L');
                sb.Append (level);
                sb.Append (": ");
                for (;;)
                {
                    branch.Append (sb);
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
            for (var leaf = (Leaf) first;;)
            {
                leaf.Append (sb);
                leaf = (Leaf) leafPath.TraverseRight();
                if (leaf == null)
                    break;

                if (leafPath.IsFirstChild)
                    sb.Append (" | ");
                else
                    sb.Append ('|');
            }
            yield return sb.ToString();
        }
    }
#endif
}
