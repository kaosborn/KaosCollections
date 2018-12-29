//
// Library: KaosCollections
// File:    Btree.Node.cs
// Purpose: Define classes Node, Branch, Leaf.
//
// Copyright © 2009-2019 Kasey Osborn (github.com/kaosborn)
// MIT License - Use and redistribute freely
//

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Kaos.Collections
{
    public abstract partial class Btree<T>
    {
        /// <summary>Base page of the B+ tree. May be internal (Branch) or terminal (Leaf, PairLeaf).</summary>
        /// <exclude />
        private protected abstract class Node
        {
            protected readonly List<T> keys;

            public Node (int keyCapacity)
             => this.keys = new List<T> (keyCapacity);

            public abstract int Weight { get; }

            public int KeyCount => keys.Count;
            public T Key0 => keys[0];

            public void AddKey (T key) => keys.Add (key);
            public T GetKey (int index) => keys[index];
            public int Search (T key) => keys.BinarySearch (key);
            public int Search (T key, IComparer<T> comparer) => keys.BinarySearch (key, comparer);
            public void SetKey (int index, T key) => keys[index] = key;
            public void RemoveKey (int index) => keys.RemoveAt (index);
            public void RemoveKeys (int index, int count) => keys.RemoveRange (index, count);
            public void TruncateKeys (int index) => keys.RemoveRange (index, keys.Count - index);
            public void InsertKey (int index, T key) => keys.Insert (index, key);
            public void CopyKeysTo (T[] array, int index, int count) => keys.CopyTo (0, array, index, count);

            public void InsertKey (int index, T key, int count)
            {
                Debug.Assert (count > 0);

                int startCount = keys.Count;
                int add0 = count + index - startCount;

                if (add0 >= 0)
                {
                    while (--add0 >= 0)
                        keys.Add (key);
                    for (int p1 = index; p1 < startCount; ++p1)
                    {
                        keys.Add (keys[p1]);
                        keys[p1] = key;
                    }
                }
                else
                {
                    int p3 = startCount - count;
                    for (int p2 = p3; p2 < startCount; ++p2)
                        keys.Add (keys[p2]);
                    while (--p3 >= index)
                        keys[p3+count] = keys[p3];
                    while (--count >= 0)
                        keys[++p3] = key;
                }
            }

#if DEBUG
            public StringBuilder Append (StringBuilder sb)
            {
                for (int ix = 0; ix < KeyCount; ix++)
                {
                    if (ix > 0)
                        sb.Append (',');
                    sb.Append (GetKey (ix));
                }
                return sb;
            }

            public virtual void SanityCheck()
            { }
#endif
        }


        /// <summary>Internal node containing subdivisions.</summary>
        /// <remarks>
        /// Contains copies of the first key ('anchor') of every leaf except the leftmost.
        /// </remarks>
        /// <exclude />
        private protected sealed class Branch : Node
        {
            private readonly List<Node> childNodes;
            private int weight;

            public Branch (int keyCapacity) : base (keyCapacity)
             => this.childNodes = new List<Node> (keyCapacity + 1);

            public Branch (int keyCapacity, Node child, int weight=0) : base (keyCapacity)
            {
                this.childNodes = new List<Node> (keyCapacity + 1) { child };
                this.weight = weight;
            }

            public int ChildCount
             => childNodes.Count;

            public Node Child0
             => childNodes[0];

            public Node GetChild (int childIndex)
             => childNodes[childIndex];

            /// <summary>Number of elements in child leaves.</summary>
            public override int Weight
             => weight;

            public void AdjustWeight (int delta)
             => weight += delta;

            public void IncrementWeight() => ++weight;
            public void DecrementWeight() => --weight;

            public void RemoveChild (int index)
             => childNodes.RemoveAt (index);

            public void Truncate (int index)
            {
                TruncateKeys (index);
                childNodes.RemoveRange (index + 1, childNodes.Count - (index + 1));
            }

            
            public void RemoveChildRange1 (int index, int count)
            {
                if (count > 0)
                {
                    childNodes.RemoveRange (index+1, count);
                    keys.RemoveRange (index, count);
                }
            }

            public void RemoveChildRange2 (int index, int count)
            {
                childNodes.RemoveRange (index, count);
                keys.RemoveRange (index, count);
            }

            public void Add (Node node)
             => childNodes.Add (node);

            public void Add (T key, Node node)
            {
                AddKey (key);
                childNodes.Add (node);
            }

            public void Insert (int index, Node node)
             => childNodes.Insert (index, node);

            public void Remove (int index, int count)
            {
                RemoveKeys (index, count);
                childNodes.RemoveRange (index, count);
            }

#if DEBUG
            public override void SanityCheck()
            {
                if (keys.Count != this.childNodes.Count - 1)
                    throw new InvalidOperationException ("Mismatched keys/child count");
            }
#endif
        }


        /// <exclude />
        private protected class Leaf : Node
        {
            public Leaf leftLeaf,
                        rightLeaf;

            /// <summary>Create a siblingless leaf.</summary>
            /// <param name="capacity">The initial number of elements the page can store.</param>
            public Leaf (int capacity=0) : base (capacity)
             => this.leftLeaf = this.rightLeaf = null;


            /// <summary>Splice new leaf to right of leftLeaf".</summary>
            /// <param name="leftLeaf">Provides linked list insert point.</param>
            /// <param name="capacity">The initial number of elements the page can store.</param>
            /// <remarks>Caller must fixup rightLeaf field.</remarks>
            public Leaf (Leaf leftLeaf, int capacity) : base (capacity)
            {
                // Doubly linked list insertion.
                this.rightLeaf = leftLeaf.rightLeaf;
                leftLeaf.rightLeaf = this;
                this.leftLeaf = leftLeaf;
            }


            /// <summary>Number of key/value pairs in the subtree.</summary>
            public override int Weight
             => keys.Count;


            public void Add (Leaf source, int sourceStart, int sourceStop)
            {
                for (int ix = sourceStart; ix < sourceStop; ++ix)
                    keys.Add (source.GetKey (ix));
            }

            public virtual void Coalesce()
            {
                for (int ix = 0; ix < rightLeaf.KeyCount; ++ix)
                    keys.Add (rightLeaf.keys[ix]);
            }

            public void CopyLeafLeft (int index, int offset)
             => keys[index-offset] = keys[index];

            public virtual void MoveLeft (int count)
            {
                for (int ix = 0; ix < count; ++ix)
                    keys.Add (rightLeaf.keys[ix]);
                rightLeaf.keys.RemoveRange (0, count);
            }

            public virtual void RemoveRange (int index, int count)
             => keys.RemoveRange (index, count);

            public virtual void Truncate (int index)
             => keys.RemoveRange (index, keys.Count-index);
        }
    }
}
