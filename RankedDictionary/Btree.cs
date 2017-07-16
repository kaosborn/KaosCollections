//
// Library: KaosCollections
// File:    Btree.cs
// Purpose: Define base functionality.
//
// Copyright © 2009-2017 Kasey Osborn (github.com/kaosborn)
// MIT License - Use and redistribute freely
//

using System.Collections.Generic;
using System.Diagnostics;

namespace Kaos.Collections
{
    public partial class Btree<TKey>
    {
        protected Node root;
        protected readonly KeyLeaf leftmostLeaf;
        protected readonly int maxKeyCount;
        protected readonly IComparer<TKey> compareOp;

        protected const int MinimumOrder = 4;
        protected const int DefaultOrder = 128;
        protected const int MaximumOrder = 256;

        protected Btree (int order, IComparer<TKey> comparer, KeyLeaf leftmostLeaf)
        {
            this.compareOp = comparer ?? Comparer<TKey>.Default;
            this.maxKeyCount = order - 1;
            this.leftmostLeaf = leftmostLeaf;
        }


        /// <summary>Get the rightmost leaf of the tree.</summary>
        /// <returns>Rightmost leaf.</returns>
        protected KeyLeaf GetRightmost()
        {
            for (Node node = root;;)
                if (node is Branch branch)
                    node = branch.GetChild (branch.KeyCount);
                else
                    return (KeyLeaf) node;
        }


        /// <summary>Perform lite search for key.</summary>
        /// <param name="key">Target of search.</param>
        /// <param name="index">When found, holds index of returned Leaf; else ~index of nearest greater key.</param>
        /// <returns>Leaf holding target (found or not).</returns>
        protected KeyLeaf Find (TKey key, out int index)
        {
            //  Unfold on default comparer for 5% speed improvement.
            if (compareOp == Comparer<TKey>.Default)
                for (Node node = root;;)
                {
                    index = node.Search (key);

                    if (node is Branch branch)
                        node = branch.GetChild (index < 0 ? ~index : index + 1);
                    else
                        return (KeyLeaf) node;
                }
            else
                for (Node node = root;;)
                {
                    index = node.Search (key, compareOp);

                    if (node is Branch branch)
                        node = branch.GetChild (index < 0 ? ~index : index + 1);
                    else
                        return (KeyLeaf) node;
                }
        }


        protected void Remove2 (NodeVector nv)
        {
            int leafIndex = nv.TopNodeIndex;
            var leaf = (KeyLeaf) nv.TopNode;

            leaf.Remove (leafIndex);
            nv.UpdateWeight (-1);

            if (leafIndex == 0)
                if (leaf.KeyCount != 0)
                    nv.SetPivot (nv.TopNode.Key0);
                else
                {
                    Debug.Assert (leaf.RightLeaf==null, "only rightmost leaf should ever be empty");

                    // The leaf is empty so prune unless it is leftmost (therefore the only leaf).
                    if (leaf.LeftLeaf != null)
                    {
                        leaf.Prune();
                        nv.Demote();
                    }

                    return;
                }

            // Leaf underflow?
            if (leaf.KeyCount < (maxKeyCount + 1) / 2)
            {
                KeyLeaf rightLeaf = leaf.RightLeaf;
                if (rightLeaf != null)
                    if (leaf.KeyCount + rightLeaf.KeyCount > maxKeyCount)
                    {
                        // Balance leaves by shifting pairs from right leaf.
                        int shifts = (leaf.KeyCount + rightLeaf.KeyCount + 1) / 2 - leaf.KeyCount;
                        leaf.Shift (shifts);
                        nv.TraverseRight();
                        nv.SetPivot (rightLeaf.Key0);
                        nv.TiltLeft (shifts);
                    }
                    else
                    {
                        leaf.Coalesce();
                        nv.TraverseRight();
                        nv.TiltLeft (rightLeaf.KeyCount);
                        nv.Demote();
                    }
            }
        }
    }
}
