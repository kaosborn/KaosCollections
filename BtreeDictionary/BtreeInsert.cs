//
// Library: KaosCollections
// File:    BtreeInsert.cs
// Purpose: Define nonpublic BtreeDictionary insert operations.
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

        // Insert element at preseeked path.
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
                int splitIndex = isAppend ? branch.KeyCount - 2 : (branch.KeyCount + 1) / 2;

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

        #endregion
    }
}
}
