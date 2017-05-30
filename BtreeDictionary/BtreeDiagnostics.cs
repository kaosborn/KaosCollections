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
        /// Perform diagnostics check for data structure internal errors. Since this is an
        /// in-memory managed structure, any errors would indicate a bug. Also performs space
        /// complexity diagnostics to ensure that all non-rightmost nodes maintain 50% fill.
        /// </summary>
        /// </exclude>
        public void SanityCheck()
        {
            int Order = root.KeyCapacity + 1;

            Leaf<TKey, TValue> lastLeaf = Check (root, 1, true, default (TKey), null);
            if (lastLeaf.RightLeaf != null)
                throw new BtreeInsaneException ("Last leaf has invalid RightLeaf");
        }


        private Leaf<TKey, TValue> Check
        (
            Branch<TKey> branch,
            int level,
            bool isRightmost,
            TKey anchor,  // ignored when isRightmost true
            Leaf<TKey, TValue> visited
        )
        {
            if (branch.KeyCapacity != root.KeyCapacity)
                throw new BtreeInsaneException ("Branch KeyCapacity Inconsistent");

            if (!isRightmost && (branch.KeyCount + 1) < branch.KeyCapacity / 2)
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
                    Branch<TKey> child = (Branch<TKey>) branch.GetChild (i);
                    visited = Check (child, level + 1, isRightmost0, anchor0, visited);
                }
                else
                {
                    Leaf<TKey, TValue> leaf = (Leaf<TKey, TValue>) branch.GetChild (i);
                    visited = Check (leaf, isRightmost0, anchor0, visited);
                }
            }
            return visited;
        }


        private Leaf<TKey, TValue> Check
        (
            Leaf<TKey, TValue> leaf,
            bool isRightmost,
            TKey anchor,
            Leaf<TKey, TValue> visited
        )
        {
            if (leaf.KeyCapacity != root.KeyCapacity)
                throw new BtreeInsaneException ("Leaf KeyCapacity Inconsistent");

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
        public void Dump()
        {
            int level = 0;
            Node<TKey> first;

            for (;;)
            {
                TreePath<TKey, TValue> branchPath = new TreePath<TKey, TValue> (this, level);
                first = branchPath.TopNode;
                if (first is Leaf<TKey, TValue>)
                    break;

                Branch<TKey> branch = (Branch<TKey>) first;

                Console.Write ("L{0}: ", level);
                for (; ; )
                {
                    branch.Dump();
                    branch = (Branch<TKey>) branchPath.TraverseRight();

                    if (branch == null)
                        break;
                    Console.Write (" | ");
                }
                ++level;
                Console.WriteLine();
            }

            TreePath<TKey, TValue> leafPath = new TreePath<TKey, TValue> (this, level);
            Console.Write ("L{0}: ", level);
            for (Leaf<TKey, TValue> leaf = (Leaf<TKey, TValue>) first;;)
            {
                leaf.Dump();
                leaf = (Leaf<TKey, TValue>) leafPath.TraverseRight();
                if (leaf == null)
                    break;

                if (leafPath.IsFirstChild)
                    Console.Write (" | ");
                else
                    Console.Write ("|");
            }
            Console.WriteLine();
        }
    }



    internal abstract partial class Node<TKey>
    {
        internal void Dump()
        {
            for (int k = 0; k < this.KeyCount; k++)
            {
                if (k > 0)
                    Console.Write (",");

                Console.Write (GetKey (k));
            }
        }
    }



    internal partial class TreePath<TKey, TValue>
    {
        internal bool IsFirstChild
        { get { return this.indexStack[Height - 2] == 0; } }

        
        /// <summary>Make an empty path.</summary>
        /// <param name="tree">Target of path.</param>
        internal TreePath (BtreeDictionary<TKey, TValue> tree)
        {
            indexStack = new List<int>();
            nodeStack = new List<Node<TKey>>();
            IsFound = false;

            Push (tree.root, 0);
        }


        /// <summary>Make a path to leftmost branch or leaf at the given level.</summary>
        /// <param name="tree">Target of path.</param>
        /// <param name="level">Level of node to seek where root is level 0.</param>
        /// <remarks>Used only for diagnostics.</remarks>
        internal TreePath (BtreeDictionary<TKey, TValue> tree, int level)
            : this (tree)
        {
            Node<TKey> node = TopNode;
            for (;;)
            {
                if (level <= 0)
                    break;
                node = ((Branch<TKey>) node).GetChild (0);
                Push (node, 0);
                --level;
            }
        }
    }
#endif
}
