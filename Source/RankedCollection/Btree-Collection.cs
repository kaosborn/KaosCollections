//
// Library: KaosCollections
// File:    RankedCollection.cs
//
// Copyright © 2009-2017 Kasey Osborn (github.com/kaosborn)
// MIT License - Use and redistribute freely
//

namespace Kaos.Collections
{
    public abstract partial class Btree<T>
    {
        internal bool AddKey (T item, NodeVector path)
        {
            StageBump();

            path.IncrementPathWeight();

            var leaf = (Leaf) path.TopNode;
            if (leaf.KeyCount < maxKeyCount)
            {
                leaf.Insert (path.TopIndex, item);
                return true;
            }

            // Leaf is full so right split a new leaf.
            var newLeaf = new Leaf (leaf, maxKeyCount);
            int pathIndex = path.TopIndex;

            if (newLeaf.rightLeaf != null)
                newLeaf.rightLeaf.leftLeaf = newLeaf;
            else
            {
                rightmostLeaf = newLeaf;

                if (pathIndex == leaf.KeyCount)
                {
                    newLeaf.AddKey (item);
                    path.Promote (item, (Node) newLeaf, true);
                    return true;
                }
            }

            int splitIndex = leaf.KeyCount / 2 + 1;
            if (pathIndex < splitIndex)
            {
                // Left-side insert: Copy right side to the split leaf.
                newLeaf.Add (leaf, splitIndex - 1, leaf.KeyCount);
                leaf.Truncate (splitIndex - 1);
                leaf.Insert (pathIndex, item);
            }
            else
            {
                // Right-side insert: Copy split leaf parts and new key.
                newLeaf.Add (leaf, splitIndex, pathIndex);
                newLeaf.AddKey (item);
                newLeaf.Add (leaf, pathIndex, leaf.KeyCount);
                leaf.Truncate (splitIndex);
            }

            // Promote anchor of split leaf.
            path.Promote (newLeaf.Key0, (Node) newLeaf, newLeaf.rightLeaf == null);
            return true;
        }
    }
}
