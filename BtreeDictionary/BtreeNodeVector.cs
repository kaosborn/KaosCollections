//
// Library: KaosCollections
// File:    BtreeNodeVector.cs
// Purpose: Defines nonpublic class that stores an element traversal path.
//
// Copyright © 2009-2017 Kasey Osborn (github.com/kaosborn)
// MIT License - Use and redistribute freely
//

using System.Collections.Generic;
using System.Diagnostics;

namespace Kaos.Collections
{
    public partial class BtreeDictionary<TKey,TValue>
    {
        /// <summary>Stack trace from root to leaf of a key/value pair.</summary>
        /// <remarks>
        /// Provides traversal path to existing key or insertion point for non-existing key
        /// along with various helper methods.
        /// </remarks>
        private class NodeVector
        {
            private readonly BtreeDictionary<TKey,TValue> owner;
            private readonly List<int> indexStack;
            private readonly List<Node> nodeStack;

            #region Constructors

            /// <summary>Perform search and store each level of path on the stack.</summary>
            /// <param name="tree">Tree to search.</param>
            /// <param name="key">Value to find.</param>
            public NodeVector (BtreeDictionary<TKey,TValue> tree, TKey key)
            {
                this.owner = tree;
                this.indexStack = new List<int>();
                this.nodeStack = new List<Node>();

                for (Node node = tree.root;;)
                {
                    Debug.Assert (node != null);

                    this.nodeStack.Add (node);
                    int ix = node.Search (key, tree.comparer);

                    if (node is Leaf)
                    {
                        IsFound = ix >= 0;
                        if (! IsFound)
                            ix = ~ix;
                        this.indexStack.Add (ix);
                        return;
                    }

                    ix = (ix < 0) ? ~ix : ix+1;
                    this.indexStack.Add (ix);
                    node = ((Branch) node).GetChild (ix);
                }
            }

            #endregion

            #region Properties

            public bool IsFound
            { get; private set; }

            public Node TopNode
            { get { return nodeStack[indexStack.Count - 1]; } }

            public int TopNodeIndex
            { get { return indexStack[indexStack.Count - 1]; } }

            public int Height
            { get { return indexStack.Count; } }


            public TValue LeafValue
            {
                get
                {
                    int leafIndex = indexStack.Count - 1;
                    return ((Leaf) nodeStack[leafIndex]).GetValue (indexStack[leafIndex]);
                }
                set
                {
                    int leafIndex = indexStack.Count - 1;
                    ((Leaf) nodeStack[leafIndex]).SetValue (indexStack[leafIndex], value);
                }
            }

            #endregion

            #region Methods

            /// <summary>
            /// Get the node to the immediate left of the node specified by NodeVector.
            /// </summary>
            public Node GetLeftNode()
            {
                Debug.Assert (indexStack.Count == nodeStack.Count);

                for (int depth = indexStack.Count - 2; depth >= 0; --depth)
                    if (indexStack[depth] > 0)
                    {
                        Node result = ((Branch) nodeStack[depth]).GetChild (indexStack[depth] - 1);
                        for (; depth < indexStack.Count - 2; ++depth)
                            result = ((Branch) result).GetChild (result.KeyCount);
                        return result;
                    }

                return null;
            }


            /// <summary>Get nearest key where left child path taken.</summary>
            /// <remarks>On entry, top of path refers to a branch.</remarks>
            public TKey GetPivot()
            {
                Debug.Assert (TopNode is Branch);
                for (int depth = indexStack.Count - 2; depth >= 0; --depth)
                {
                    if (indexStack[depth] > 0)
                        return nodeStack[depth].GetKey (indexStack[depth] - 1);
                }

                Debug.Assert (false, "no left pivot");
                return default (TKey);
            }


            /// <summary>Set nearest key where left child path taken.</summary>
            /// <remarks>On entry, top of vector refers to a branch.</remarks>
            public void SetPivot (TKey pivotKey)
            {
                for (int depth = indexStack.Count - 2; depth >= 0; --depth)
                    if (indexStack[depth] > 0)
                    {
                        nodeStack[depth].SetKey (indexStack[depth] - 1, pivotKey);
                        return;
                    }
            }


            public void Clear()
            {
                indexStack.Clear();
                nodeStack.Clear();
            }


            public void Pop()
            {
                nodeStack.RemoveAt (nodeStack.Count - 1);
                indexStack.RemoveAt (indexStack.Count - 1);
            }


            public void Push (Node newNode, int newNodeIndex)
            {
                nodeStack.Add (newNode);
                indexStack.Add (newNodeIndex);
            }


            /// <summary>Adjust tree path to node to the right.</summary>
            /// <returns>Node to immediate right of current path;
            /// <b>null</b> if current path at rightmost node.</returns>
            public Node TraverseRight()
            {
                Node node = null;
                int height = indexStack.Count;
                for (;;)
                {
                    if (indexStack.Count < 2)
                    {
                        Clear();
                        node = null;
                        break;
                    }

                    Pop();
                    node = TopNode;
                    int newIndex = TopNodeIndex + 1;

                    if (newIndex < ((Branch) node).ChildCount)
                    {
                        indexStack[indexStack.Count - 1] = newIndex;
                        node = ((Branch) node).GetChild (newIndex);
                        for (;;)
                        {
                            Push (node, 0);
                            if (indexStack.Count >= height)
                                break;
                            node = ((Branch) node).FirstChild;
                        }
                        break;
                    }
                }

                return node;
            }


            /// <summary>Insert element at this path.</summary>
            public void Insert (TKey key, TValue value)
            {
                var leaf = (Leaf) TopNode;
                int pathIndex = TopNodeIndex;

                if (leaf.KeyCount < owner.maxKeyCount)
                {
                    leaf.Insert (pathIndex, key, value);
                    ++owner.Count;
                    return;
                }

                // Leaf is full so right split a new leaf.
                var newLeaf = new Leaf (leaf, owner.maxKeyCount);

                if (newLeaf.RightLeaf == null && pathIndex == leaf.KeyCount)
                    newLeaf.Add (key, value);
                else
                {
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
                }

                // Promote anchor of split leaf.
                ++owner.Count;
                Promote (newLeaf.GetKey (0), (Node) newLeaf, newLeaf.RightLeaf == null);
            }


            // Leaf or branch has been split so insert the new anchor into a branch.
            private void Promote (TKey key, Node newNode, bool isAppend)
            {
                for (;;)
                {
                    if (Height == 1)
                    {
                        // Graft new root.
                        Debug.Assert (owner.root == TopNode);
                        owner.root = new Branch (TopNode, owner.maxKeyCount);
                        owner.root.Add (key, newNode);
                        break;
                    }

                    Pop();
                    var branch = (Branch) TopNode;
                    int branchIndex = TopNodeIndex;

                    if (branch.KeyCount < owner.maxKeyCount)
                    {
                        // Typical case where branch has room.
                        branch.InsertKey (branchIndex, key);
                        branch.Insert (branchIndex + 1, newNode);
                        break;
                    }

                    // Branch is full so right split a new branch.
                    var newBranch = new Branch (branch, owner.maxKeyCount);
                    int splitIndex = isAppend ? branch.KeyCount - 1 : (branch.KeyCount + 1) / 2;

                    if (branchIndex < splitIndex)
                    {
                        // Split with left-side insert.
                        for (int ix = splitIndex; ; ++ix)
                        {
                            if (ix >= branch.KeyCount)
                            {
                                newBranch.Add (branch.GetChild (ix));
                                break;
                            }
                            newBranch.Add (branch.GetKey (ix), branch.GetChild (ix));
                        }

                        TKey newPromotion = branch.GetKey (splitIndex - 1);
                        branch.Truncate (splitIndex - 1);
                        branch.InsertKey (branchIndex, key);
                        branch.Insert (branchIndex + 1, newNode);
                        key = newPromotion;
                    }
                    else
                    {
                        // Split branch with right-side insert (or cascade promote).
                        int leftIndex = splitIndex;

                        if (branchIndex > splitIndex)
                        {
                            for (;;)
                            {
                                ++leftIndex;
                                newBranch.Add (branch.GetChild (leftIndex));
                                if (leftIndex >= branchIndex)
                                    break;
                                newBranch.AddKey (branch.GetKey (leftIndex));
                            }
                            newBranch.AddKey (key);
                            key = branch.GetKey (splitIndex);
                        }

                        newBranch.Add (newNode);

                        while (leftIndex < branch.KeyCount)
                        {
                            newBranch.AddKey (branch.GetKey (leftIndex));
                            ++leftIndex;
                            newBranch.Add (branch.GetChild (leftIndex));
                        }

                        branch.Truncate (splitIndex);
                    }

                    newNode = newBranch;
                }
            }


            /// <summary>Delete key/value at this path.</summary>
            public void Delete()
            {
                int leafIndex = TopNodeIndex;
                var leaf = (Leaf) TopNode;

                leaf.Remove (leafIndex);
                --owner.Count;

                if (leafIndex == 0)
                    if (leaf.KeyCount != 0)
                        SetPivot (TopNode.GetKey (0));
                    else
                    {
                        Debug.Assert (leaf.RightLeaf==null, "only rightmost leaf should ever be empty");

                        // Leaf is empty.  Prune it unless it is the only leaf in the tree.
                        var leftLeaf = (Leaf) GetLeftNode();
                        if (leftLeaf != null)
                        {
                            leftLeaf.RightLeaf = leaf.RightLeaf;
                            Demote();
                        }

                        return;
                    }

                // Leaf underflow?
                if (leaf.KeyCount < (owner.maxKeyCount + 1) / 2)
                {
                    Leaf rightLeaf = leaf.RightLeaf;
                    if (rightLeaf != null)
                        if (leaf.KeyCount + rightLeaf.KeyCount > owner.maxKeyCount)
                        {
                            // Balance leaves by shifting pairs from right leaf.
                            int shifts = owner.maxKeyCount - (leaf.KeyCount + rightLeaf.KeyCount) / 2;
                            leaf.Add (rightLeaf, 0, shifts);
                            rightLeaf.Remove (0, shifts);
                            TraverseRight();
                            SetPivot (TopNode.GetKey (0));
                        }
                        else
                        {
                            // Coalesce right leaf to current leaf and prune right leaf.
                            leaf.Add (rightLeaf, 0, rightLeaf.KeyCount);
                            leaf.RightLeaf = rightLeaf.RightLeaf;
                            TraverseRight();
                            Demote();
                        }
                }
            }


            // Leaf has been emptied so non-lazy delete its pivot.
            private void Demote()
            {
                for (;;)
                {
                    Debug.Assert (Height > 0);
                    Pop();

                    var branch = (Branch) TopNode;
                    if (TopNodeIndex == 0)
                    {
                        if (branch.KeyCount == 0)
                            // Cascade when rightmost branch is empty.
                            continue;

                        // Rotate pivot for first pair.
                        TKey pivot = branch.GetKey (0);
                        branch.RemoveKey (0);
                        branch.RemoveChild (0);
                        SetPivot (pivot);
                    }
                    else
                    {
                        // Typical branch pivot delete.
                        branch.RemoveKey (TopNodeIndex - 1);
                        branch.RemoveChild (TopNodeIndex);
                    }

                    var right = (Branch) TraverseRight();
                    if (right == null)
                    {
                        if (branch == owner.root && branch.KeyCount == 0)
                        {
                            // Prune the empty root.
                            var newRoot = branch.FirstChild as Branch;
                            if (newRoot != null)
                                owner.root = newRoot;
                        }
                        return;
                    }

                    if (branch.KeyCount + right.KeyCount < owner.maxKeyCount)
                    {
                        // Coalesce left: move pivot and right sibling nodes.
                        branch.AddKey (GetPivot());

                        for (int ix = 0; ; ++ix)
                        {
                            branch.Add (right.GetChild (ix));
                            if (ix >= right.KeyCount)
                                break;
                            branch.AddKey (right.GetKey (ix));
                        }

                        // Cascade demotion.
                        continue;
                    }

                    // Branch underflow?
                    if (branch.KeyCount < owner.maxKeyCount / 2)
                    {
                        int shifts = (right.KeyCount - branch.KeyCount) / 2 - 1;

                        // Balance branches to keep ratio.  Rotate thru the pivot.
                        branch.AddKey (GetPivot());

                        // Shift pairs from right sibling.
                        for (int rightIndex = 0; ; ++rightIndex)
                        {
                            branch.Add (right.GetChild (rightIndex));

                            if (rightIndex >= shifts)
                                break;

                            branch.AddKey (right.GetKey (rightIndex));
                        }

                        SetPivot (right.GetKey (shifts));
                        right.Remove (0, shifts + 1);
                    }

                    return;
                }
            }

            #endregion

#if DEBUG
            /// <summary>Returns <b>true</b> if no left sibling.</summary>
            public bool IsFirstChild
            { get { return this.indexStack[Height - 2] == 0; } }


            /// <summary>Make an empty path.</summary>
            /// <param name="tree">Target of path.</param>
            /// <remarks>Used only for diagnostics.</remarks>
            public NodeVector (BtreeDictionary<TKey,TValue> tree)
            {
                indexStack = new List<int>();
                nodeStack = new List<Node>();
                IsFound = false;
                Push (tree.root, 0);
            }


            /// <summary>Make a path to leftmost branch or leaf at the given level.</summary>
            /// <param name="tree">Target of path.</param>
            /// <param name="level">Level of node to seek where root is level 0.</param>
            /// <remarks>Used only for diagnostics.</remarks>
            public NodeVector (BtreeDictionary<TKey,TValue> tree, int level) : this (tree)
            {
                for (Node node = TopNode; level > 0; --level)
                {
                    node = ((Branch) node).GetChild (0);
                    Push (node, 0);
                }
            }
#endif
        }
    }
}
