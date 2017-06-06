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
        #region Nonpublic methods

        // Insert element at preseeked path.
        private void Insert (NodeVector path, TKey key, TValue value)
        {
            var leaf = (Leaf) path.TopNode;
            int leafIndex = path.TopNodeIndex;
            bool isAppend = false;

            if (leaf.KeyCount < maxKeyCount)
            {
                leaf.Insert (leafIndex, key, value);
                ++Count;
                return;
            }

            // Leaf overflow.  Right split a new leaf.
            var newLeaf = new Leaf (leaf, maxKeyCount);

            if (newLeaf.RightLeaf == null && leafIndex == leaf.KeyCount)
            {
                isAppend = true;
                newLeaf.Add (key, value);
            }
            else
            {
                int halfway = leaf.KeyCount / 2 + 1;

                if (leafIndex < halfway)
                {
                    // Left-side insert: Copy right side to the split leaf.
                    newLeaf.Add (leaf, halfway - 1, leaf.KeyCount);
                    leaf.Truncate (halfway - 1);
                    leaf.Insert (leafIndex, key, value);
                }
                else
                {
                    // Right-side insert: Copy split leaf parts and new key.
                    newLeaf.Add (leaf, halfway, leafIndex);
                    newLeaf.Add (key, value);
                    newLeaf.Add (leaf, leafIndex, leaf.KeyCount);
                    leaf.Truncate (halfway);
                }
            }

            // Promote anchor of split leaf.
            ++Count;
            Promote (path, newLeaf.GetKey (0), (Node) newLeaf, isAppend);
        }


        // Leaf has been split so insert the new anchor into a branch.
        private void Promote (NodeVector path, TKey key, Node newNode, bool isAppend)
        {
            for (;;)
            {
                if (path.Height == 1)
                {
                    // Graft new root.
                    Debug.Assert (root == path.TopNode);
                    root = new Branch (path.TopNode, maxKeyCount);
                    root.Add (key, newNode);
                    break;
                }

                path.Pop();
                var branch = (Branch) path.TopNode;
                int branchIndex = path.TopNodeIndex;

                if (branch.KeyCount < maxKeyCount)
                {
                    // Typical case where branch has room.
                    branch.InsertKey (branchIndex, key);
                    branch.Insert (branchIndex + 1, newNode);
                    break;
                }

                // Right split an overflowing branch.
                var newBranch = new Branch (branch, maxKeyCount);
                int halfway = isAppend ? branch.KeyCount - 2 : (branch.KeyCount + 1) / 2;

                if (branchIndex < halfway)
                {
                    // Split with left-side insert.
                    for (int ix = halfway; ; ++ix)
                    {
                        if (ix >= branch.KeyCount)
                        {
                            newBranch.Add (branch.GetChild (ix));
                            break;
                        }
                        newBranch.Add (branch.GetKey (ix), branch.GetChild (ix));
                    }

                    TKey newPromotion = branch.GetKey (halfway - 1);
                    branch.Truncate (halfway - 1);
                    branch.InsertKey (branchIndex, key);
                    branch.Insert (branchIndex + 1, newNode);
                    key = newPromotion;
                }
                else
                {
                    // Split branch with right-side insert (or cascade promote).
                    int moveIndex = halfway;

                    if (branchIndex > halfway)
                    {
                        for (;;)
                        {
                            ++moveIndex;
                            newBranch.Add (branch.GetChild (moveIndex));
                            if (moveIndex >= branchIndex)
                                break;
                            newBranch.AddKey (branch.GetKey (moveIndex));
                        }
                        newBranch.AddKey (key);
                        key = branch.GetKey (halfway);
                    }

                    newBranch.Add (newNode);

                    while (moveIndex < branch.KeyCount)
                    {
                        newBranch.AddKey (branch.GetKey (moveIndex));
                        ++moveIndex;
                        newBranch.Add (branch.GetChild (moveIndex));
                    }

                    branch.Truncate (halfway);
                }

                newNode = newBranch;
            }
        }

        #endregion
    }
}
