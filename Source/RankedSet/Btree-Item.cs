//
// Library: KaosCollections
// File:    Btree-Item.cs
//
// Copyright © 2009-2019 Kasey Osborn (github.com/kaosborn)
// MIT License - Use and redistribute freely
//

namespace Kaos.Collections
{
    public abstract partial class Btree<T>
    {
        private protected bool AddKey (T item, NodeVector path)
        {
            StageBump();

            path.IncrementPathWeight();

            var leaf = (Leaf) path.TopNode;
            if (leaf.KeyCount < maxKeyCount)
            {
                leaf.InsertKey (path.TopIndex, item);
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
                leaf.InsertKey (pathIndex, item);
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


        private protected System.Collections.Generic.IEnumerable<T> Distinct2()
        {
            if (root.Weight == 0)
                yield break;

            int stageFreeze = stage;
            int ix = 0;
            Leaf leaf = leftmostLeaf;
            for (T key = leaf.Key0;;)
            {
                yield return key;
                StageCheck (stageFreeze);

                if (++ix < leaf.KeyCount)
                {
                    T nextKey = leaf.GetKey (ix);
                    if (keyComparer.Compare (key, nextKey) != 0)
                    { key = nextKey; continue; }
                }

                FindEdgeRight (key, out leaf, out ix);
                if (ix >= leaf.KeyCount)
                {
                    leaf = leaf.rightLeaf;
                    if (leaf == null)
                        yield break;
                    ix = 0;
                }
                key = leaf.GetKey (ix);
            }
        }


        private protected int RemoveAll2 (System.Collections.Generic.IEnumerable<T> other)
        {
            int removed = 0;
            if (root.Weight > 0)
            {
                StageBump();
                if (other == this)
                {
                    removed = root.Weight;
                    Initialize();
                }
                else
                {
                    var oBag = other as RankedBag<T> ?? new RankedBag<T> (other, keyComparer);
                    if (oBag.Count > 0)
                        foreach (var oKey in oBag.Distinct())
                        {
                            var oCount = oBag.GetCount (oKey);
                            int actual = Remove2 (oKey, oCount);
                            removed += actual;
                        }
                }
            }
            return removed;
        }


        private protected void ReplaceKey (NodeVector path, T item)
        {
            StageBump();
            ((Leaf) path.TopNode).SetKey (path.TopIndex, item);
        }
    }
}
