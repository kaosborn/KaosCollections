//
// Library: KaosCollections
// File:    BtreeNodeVector.cs
// Purpose: Defines nonpublic class that stores an element location path.
//
// Copyright © 2009-2017 Kasey Osborn (github.com/kaosborn)
// MIT License - Use and redistribute freely
//

using System.Collections.Generic;
using System.Diagnostics;

namespace Kaos.Collections
{
    public partial class BtreeDictionary<TKey, TValue>
    {
        /// <summary>Stack trace from root to leaf of a key/value pair.</summary>
        /// <remarks>
        /// Performs search function for key.
        /// Provides directions to existing key or insertion point for non-existing key.
        /// </remarks>
        /// <typeparam name="TKey">Key type.</typeparam>
        /// <typeparam name="TValue">Value type.</typeparam>
        private class NodeVector
        {
            private List<int> indexStack;
            private List<Node> nodeStack;

            /// <summary>Perform search and store each level of path on the stack.</summary>
            /// <param name="tree">Tree to search.</param>
            /// <param name="key">Value to find.</param>
            public NodeVector (BtreeDictionary<TKey, TValue> tree, TKey key)
            {
                indexStack = new List<int>();
                nodeStack = new List<Node>();

                for (Node node = tree.root;;)
                {
                    Debug.Assert (node != null);

                    nodeStack.Add (node);
                    int ix = node.Search (key, tree.comparer);

                    if (node is Leaf)
                    {
                        IsFound = ix >= 0;
                        if (! IsFound)
                            ix = ~ix;
                        indexStack.Add (ix);
                        return;
                    }

                    ix = (ix < 0)? ~ix : ix+1;
                    indexStack.Add (ix);
                    node = ((Branch) node).GetChild (ix);
                }
            }

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
            /// <returns>Node to immediate right of current path; <b>null</b> if current path
            /// at rightmost node.</returns>
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

#if DEBUG
            public bool IsFirstChild
            { get { return this.indexStack[Height - 2] == 0; } }

        
            /// <summary>Make an empty path.</summary>
            /// <param name="tree">Target of path.</param>
            public NodeVector (BtreeDictionary<TKey, TValue> tree)
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
            public NodeVector (BtreeDictionary<TKey, TValue> tree, int level) : this (tree)
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
