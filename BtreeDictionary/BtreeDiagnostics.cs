//
// Library: KaosCollections
// File:    BtreeDiagnostics.cs
// Purpose: Define BtreeDictionary API for Debug builds only.
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
    public class BtreeInsaneException : Exception
    {
        public BtreeInsaneException () { }
        public BtreeInsaneException (string message) : base (message) { }
        public BtreeInsaneException (string message, Exception inner) : base (message, inner) { }
    }


    public partial class BtreeDictionary<TKey, TValue>
    {
        /// <summary>
        /// Perform diagnostics check for data structure sanity errors. Since this is an
        /// in-memory managed structure, any errors would indicate a bug. Also performs space
        /// complexity diagnostics to ensure that all non-rightmost nodes maintain 50% fill.
        /// </summary>
        /// </exclude>
        public void SanityCheck()
        {
            int Order = root.KeyCapacity + 1;

            Leaf lastLeaf = Check (root, 1, true, default (TKey), null);
            if (lastLeaf.RightLeaf != null)
                throw new BtreeInsaneException ("Last leaf has invalid RightLeaf");
        }


        private Leaf Check
        (
            Branch branch,
            int level,
            bool isRightmost,
            TKey anchor,  // ignored when isRightmost true
            Leaf visited
        )
        {
            if (branch.KeyCapacity != root.KeyCapacity)
                throw new BtreeInsaneException ("Branch KeyCapacity inconsistent");

            if (! isRightmost && (branch.KeyCount + 1) < branch.KeyCapacity / 2)
                throw new BtreeInsaneException ("Branch underfilled");

            if (branch.ChildCount != branch.KeyCount + 1)
                throw new BtreeInsaneException ("Branch ChildCount wrong");

            for (int i = 0; i < branch.ChildCount; ++i)
            {
                TKey anchor0 = i == 0 ? anchor : branch.GetKey (i - 1);
                bool isRightmost0 = isRightmost && i < branch.ChildCount;
                if (i < branch.KeyCount - 1)
                    if (branch.GetKey (i).CompareTo (branch.GetKey (i + 1)) >= 0)
                        throw new BtreeInsaneException ("Branch keys not ascending");

                if (level + 1 < height)
                {
                    var child = (Branch) branch.GetChild (i);
                    visited = Check (child, level + 1, isRightmost0, anchor0, visited);
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
            if (leaf.KeyCapacity != root.KeyCapacity)
                throw new BtreeInsaneException ("Leaf KeyCapacity inconsistent");

            if (! isRightmost && leaf.KeyCount < leaf.KeyCapacity / 2)
                throw new BtreeInsaneException ("Leaf underfilled");

            if (! anchor.Equals (default (TKey)) && !anchor.Equals (leaf.GetKey (0)))
                throw new BtreeInsaneException ("Leaf has wrong anchor");

            for (int i = 0; i < leaf.KeyCount; ++i)
                if (i < leaf.KeyCount - 1 && leaf.GetKey (i).CompareTo (leaf.GetKey (i + 1)) >= 0)
                    throw new BtreeInsaneException ("Leaf keys not ascending");

            if (visited == null)
            {
                if (! anchor.Equals (default (TKey)))
                    throw new BtreeInsaneException ("Inconsistent visited, anchor");
            }
            else
                if (visited.RightLeaf != leaf)
                    throw new BtreeInsaneException ("Leaf has bad RightLeaf");

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
