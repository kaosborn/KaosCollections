//
// Library: KaosCollections
// File:    Btree-Pair.cs
//
// Copyright © 2009-2018 Kasey Osborn (github.com/kaosborn)
// MIT License - Use and redistribute freely
//

using System;
using System.Collections.Generic;

namespace Kaos.Collections
{
    public abstract partial class Btree<T>
    {

        private protected void Add2<TValue> (NodeVector path, T key, TValue value)
        {
            StageBump();

            var leaf = (PairLeaf<TValue>) path.TopNode;
            int pathIndex = path.TopIndex;

            path.IncrementPathWeight();
            if (leaf.KeyCount < maxKeyCount)
            {
                leaf.Insert (pathIndex, key, value);
                return;
            }

            // Leaf is full so right split a new leaf.
            var newLeaf = new PairLeaf<TValue> (leaf, maxKeyCount);

            if (newLeaf.rightLeaf != null)
                newLeaf.rightLeaf.leftLeaf = newLeaf;
            else
            {
                rightmostLeaf = newLeaf;

                if (pathIndex == leaf.KeyCount)
                {
                    newLeaf.Add (key, value);
                    path.Promote (key, (Node) newLeaf, true);
                    return;
                }
            }

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

            // Promote anchor of split leaf.
            path.Promote (newLeaf.Key0, (Node) newLeaf, newLeaf.rightLeaf==null);
        }


        private protected int ContainsValue2<TValue> (TValue value)
        {
            int result = 0;

            if (value != null)
            {
                var comparer = System.Collections.Generic.EqualityComparer<TValue>.Default;
                for (var leaf = (PairLeaf<TValue>) leftmostLeaf; leaf != null; leaf = (PairLeaf<TValue>) leaf.rightLeaf)
                {
                    for (int vix = 0; vix < leaf.ValueCount; ++vix)
                        if (comparer.Equals (leaf.GetValue (vix), value))
                            return result + vix;
                    result += leaf.KeyCount;
                }
            }
            else
                for (var leaf = (PairLeaf<TValue>) leftmostLeaf; leaf != null; leaf = (PairLeaf<TValue>) leaf.rightLeaf)
                {
                    for (int vix = 0; vix < leaf.ValueCount; ++vix)
                        if (leaf.GetValue (vix) == null)
                            return result + vix;
                    result += leaf.KeyCount;
                }

            return -1;
        }


        private protected int RemoveWhere2<V> (Predicate<KeyValuePair<T,V>> match)
        {
            if (match == null)
                throw new ArgumentNullException (nameof (match));

            int stageFreeze = stage;
            int leafLoss = 0, treeLoss = 0;
            var path = NodeVector.CreateFromIndex (this, 0);
            var leaf = (PairLeaf<V>) path.TopNode;
            int ix = 0;

            for (;; ++ix)
            {
                if (ix >= leaf.KeyCount)
                {
                    endOfLeaf();
                    if (leaf == null)
                        break;
                }

                bool isMatch = match (new KeyValuePair<T,V> (leaf.GetKey (ix), leaf.GetValue (ix)));
                StageCheck (stageFreeze);
                if (isMatch)
                {
                    ++leafLoss;
                    stageFreeze = StageBump();
                }
                else if (leafLoss != 0)
                {
                    leaf.CopyPairLeft (ix, leafLoss);
                    if (ix == leafLoss)
                        path.SetPivot (leaf.Key0);
                }
            }

            if (treeLoss != 0)
                TrimRoot();
            return treeLoss;

            void endOfLeaf()
            {
                if (leafLoss == 0)
                { ix = 0; leaf = (PairLeaf<V>) path.TraverseRight(); }
                else
                {
                    leaf.Truncate (ix-leafLoss);
                    treeLoss += leafLoss; path.ChangePathWeight (-leafLoss); leafLoss = 0;

                    if (leaf.rightLeaf == null)
                    {
                        if (leaf.KeyCount == 0)
                            path.Balance();
                        leaf = null;
                    }
                    else if (! IsUnderflow (leaf.KeyCount))
                    { ix = 0; leaf = (PairLeaf<V>) path.TraverseRight(); }
                    else
                    {
                        var path2 = new NodeVector (path, path.Height);
                        if (leaf.KeyCount > 0)
                        { ix = leaf.KeyCount; path2.Balance(); }
                        else
                        {
                            ix = 0; leaf = (PairLeaf<V>) path.TraverseLeft();
                            path2.Balance();
                            if (leaf != null)
                                leaf = (PairLeaf<V>) path.TraverseRight();
                            else
                            {
                                path = NodeVector.CreateFromIndex (this, 0);
                                leaf = (PairLeaf<V>) path.TopNode;
                            }
                        }
                    }
                }
            }
        }
    }
}
