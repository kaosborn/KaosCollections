//
// Library: KaosCollections
// File:    BtreeDelete.cs
// Purpose: Define nonpublic BtreeDictionary delete operations.
//
// Copyright © 2009-2017 Kasey Osborn (github.com/kaosborn)
// MIT License - Use and redistribute freely
//

using System.Diagnostics;

namespace Kaos.Collections
{
    public partial class BtreeDictionary<TKey, TValue>
    {
    private partial class NodeVector
    {
        #region Nonpublic methods

        // Delete specified key-vaue pair.
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
                    Debug.Assert (leaf.RightLeaf == null, "only the rightmost leaf should ever be emptied");

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
            if (leaf.KeyCount < owner.maxKeyCount / 2)
            {
                Leaf rightLeaf = leaf.RightLeaf;
                if (rightLeaf != null)
                    if (leaf.KeyCount + rightLeaf.KeyCount > owner.maxKeyCount)
                    {
                        // Balance leaves by shifting pairs from right leaf.
                        int shifts = owner.maxKeyCount - (leaf.KeyCount + rightLeaf.KeyCount - 1) / 2;
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
        public void Demote()
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

                    // Rotate pivot for first child.
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
    }
    }
}
