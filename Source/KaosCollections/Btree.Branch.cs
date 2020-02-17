//
// Library: KaosCollections
// File:    Btree.Branch.cs
//
// Copyright © 2009-2020 Kasey Osborn (github.com/kaosborn)
// MIT License - Use and redistribute freely
//

using System;
using System.Collections.Generic;

namespace Kaos.Collections
{
    public abstract partial class Btree<T>
    {
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
    }
}
