//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Diagnostics;

namespace Kaos.Collections
{
    public abstract partial class Btree<T>
    {
        /// <exclude />
        private protected class KeyEnumerator
        {
            private readonly Btree<T> tree;
            private readonly bool isReverse = false;
            protected Leaf leaf = null;
            protected int leafIndex = 0;
            protected int index = 0;
            private int stageFreeze;
            protected int state;  // -1=rewound; 0=active; 1=consumed

            public T CurrentKey => leaf.GetKey (leafIndex);
            public T CurrentKeyOrDefault => state != 0 ? default : leaf.GetKey (leafIndex);
            public bool NotActive => state != 0;

            public void StageCheck() => tree.StageCheck (stageFreeze);

            public KeyEnumerator (Btree<T> owner, bool isReverse=false)
            {
                this.tree = owner;
                this.isReverse = isReverse;
                Init();
            }

            public KeyEnumerator (Btree<T> owner, int count)
            {
                tree = owner;
                stageFreeze = tree.stage;

                if (count >= tree.root.Weight)
                    state = 1;
                else
                {
                    state = -1;
                    if (count > 0)
                        index = count;

                    if (index <= tree.leftmostLeaf.KeyCount)
                    {
                        leaf = tree.leftmostLeaf;
                        leafIndex = index;
                    }
                }
            }

            public void Init()
            {
                state = -1;
                stageFreeze = tree.stage;
                if (isReverse)
                {
                    leaf = tree.rightmostLeaf;
                    leafIndex = leaf.KeyCount-1;
                    index = tree.root.Weight-1;
                }
                else
                {
                    leaf = tree.leftmostLeaf;
                    leafIndex = 0;
                    index = 0;
                }
            }

            public bool Advance()
            {
                tree.StageCheck (stageFreeze);
                if (state == 0)
                    if (isReverse)
                      { --index; --leafIndex; }
                    else
                      { ++index; ++leafIndex; }
                else if (state > 0)
                    return false;
                else
                {
                    if (leaf == null)
                        if (! isReverse && index <= tree.leftmostLeaf.KeyCount)
                          { leaf = tree.leftmostLeaf; leafIndex = index; }
                        else
                            if (index < tree.root.Weight)
                                leaf = (Leaf) tree.Find (index, out leafIndex);
                            else
                              { state = 1; return false; }
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
                        if (index < count)
                            state = 1;
                        else
                        {
                            index -= count;
                            if (leaf != null)
                                if (leafIndex < count)
                                    leaf = null;
                                else
                                    leafIndex -= count;
                        }
                    else
                        if (index >= tree.root.Weight - count)
                            state = 1;
                        else
                        {
                            index += count;
                            if (leaf != null)
                                if (leafIndex > leaf.KeyCount - count)
                                    leaf = null;
                                else
                                    leafIndex += count;
                        }
            }
        }
    }
}
