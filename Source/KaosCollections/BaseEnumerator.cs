//
// Library: KaosCollections
// File:    BaseEnumerator.cs
//
// Copyright © 2009-2019 Kasey Osborn (github.com/kaosborn)
// MIT License - Use and redistribute freely
//

using System;

namespace Kaos.Collections
{
    public abstract partial class Btree<T>
    {
        /// <exclude />
        private protected abstract class BaseEnumerator
        {
            private readonly Btree<T> tree;
            private readonly bool isReverse=false;
            protected Leaf leaf=null;
            protected int leafIndex;
            private int start=0;
            private readonly int stageFreeze;
            private int state;  // -1=rewound; 0=active; 1=consumed

            public bool NotActive => state != 0;

            public BaseEnumerator (Btree<T> owner, bool isReverse=false)
            {
                this.tree = owner;
                this.isReverse = isReverse;
                this.stageFreeze = tree.stage;
                this.state = -1;
                if (isReverse)
                {
                    this.start = owner.root.Weight-1;
                    this.leaf = owner.rightmostLeaf;
                    this.leafIndex = owner.rightmostLeaf.KeyCount-1;
                }
                else
                    this.leaf = owner.leftmostLeaf;
            }

            public BaseEnumerator (Btree<T> owner, int count)
            {
                this.tree = owner;
                this.stageFreeze = owner.stage;
                this.state = -1;

                if (count > 0)
                    this.start = count;

                if (this.start <= owner.leftmostLeaf.KeyCount)
                {
                    this.leaf = owner.leftmostLeaf;
                    this.leafIndex = this.start;
                }
            }

            protected void Init()
            {
                state = -1;
                leaf = null;
            }

            protected bool AdvanceBase()
            {
                tree.StageCheck (stageFreeze);
                if (state == 0)
                    if (isReverse)
                        --leafIndex;
                    else
                        ++leafIndex;
                else if (state > 0)
                    return false;
                else
                {
                    if (leaf == null)
                        if (start >= tree.root.Weight)
                          { state = 1; return false; }
                        else
                            leaf = (Leaf) tree.Find (start, out leafIndex);
                    state = 0;
                }

                if (isReverse)
                {
                    if (leafIndex < 0)
                    {
                        leaf = leaf.leftLeaf;
                        if (leaf == null)
                        { state = 1; return false; }
                        leafIndex = leaf.KeyCount - 1;
                    }
                }
                else if (leafIndex >= leaf.KeyCount)
                {
                    leaf = leaf.rightLeaf;
                    if (leaf == null)
                    { state = 1; return false; }
                    leafIndex = 0;
                }

                return true;
            }

            public void Bypass (int count)
            {
                if (state < 0 && count > 0)
                    if (isReverse)
                        if (start < count)
                            state = 1;
                        else
                        {
                            start -= count;
                            if (leaf != null)
                                if (leafIndex < count)
                                    leaf = null;
                                else
                                    leafIndex -= count;
                        }
                    else
                        if (start >= tree.root.Weight - count)
                            state = 1;
                        else
                        {
                            start += count;
                            if (leaf != null)
                                if (leafIndex > leaf.KeyCount - count)
                                    leaf = null;
                                else
                                    leafIndex += count;
                        }
            }

            protected void Bypass2<X> (Func<X,bool> condition, Func<Leaf,int,X> getter)
            {
                if (state > 0)
                    return;

                if (isReverse)
                {
                    if (start >= 0)
                    {
                        if (leaf == null)
                            leaf = (Leaf) tree.Find (start, out leafIndex);

                        for (;;)
                        {
                            if (leafIndex < 0)
                            {
                                leaf = leaf.leftLeaf;
                                if (leaf == null)
                                    break;
                                leafIndex = leaf.KeyCount - 1;
                            }

                            if (! condition (getter (leaf, leafIndex)))
                                return;
                            --leafIndex;
                            --start;
                        }
                    }
                }
                else if (start < tree.root.Weight)
                {
                    if (leaf == null)
                        leaf = (Leaf) tree.Find (start, out leafIndex);

                    for (;;)
                    {
                        if (leafIndex >= leaf.KeyCount)
                        {
                            leaf = leaf.rightLeaf;
                            if (leaf == null)
                                break;
                            leafIndex = 0;
                        }

                        if (! condition (getter (leaf, leafIndex)))
                            return;
                        ++leafIndex;
                        ++start;
                    }
                }

                state = 1;
            }

            protected void Bypass3<X> (Func<X,int,bool> condition, Func<Leaf,int,X> getter)
            {
                if (state > 0)
                    return;

                if (isReverse)
                {
                    if (start >= 0)
                    {
                        if (leaf == null)
                            leaf = (Leaf) tree.Find (start, out leafIndex);

                        for (int offset = 0;; ++offset)
                        {
                            if (leafIndex < 0)
                            {
                                leaf = leaf.leftLeaf;
                                if (leaf == null)
                                    break;
                                leafIndex = leaf.KeyCount - 1;
                            }

                            if (! condition (getter (leaf, leafIndex), offset))
                              { start -= offset; return; }
                            --leafIndex;
                        }
                    }
                }
                else if (start < tree.root.Weight)
                {
                    if (leaf == null)
                        leaf = (Leaf) tree.Find (start, out leafIndex);

                    for (int offset = 0;; ++offset)
                    {
                        if (leafIndex >= leaf.KeyCount)
                        {
                            leaf = leaf.rightLeaf;
                            if (leaf == null)
                                break;
                            leafIndex = 0;
                        }

                        if (! condition (getter (leaf, leafIndex), offset))
                          { start += offset; return; }
                        ++leafIndex;
                    }
                }

                state = 1;
           }
        }
    }
}
