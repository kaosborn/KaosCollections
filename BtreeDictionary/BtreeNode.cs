//
// Library: KaosCollections
// File:    BtreeNodes.cs
// Purpose: Define nonpublic tree structure and its basic operations.
//
// Copyright © 2009-2017 Kasey Osborn (github.com/kaosborn)
// MIT License - Use and redistribute freely
//

using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Kaos.Collections
{
    public partial class BtreeDictionary<TKey, TValue>
    {
        private class Node
        {
            protected List<TKey> keys;

            protected Node (int order) { keys = new List<TKey> (order - 1); }

            public int KeyCount { get { return keys.Count; } }
            public int KeyCapacity { get { return keys.Capacity; } }
            public bool NotFull { get { return keys.Count < keys.Capacity; } }

            public void AddKey (TKey key) { keys.Add (key); }
            public TKey GetKey (int index) { return keys[index]; }
            public int Search (TKey key) { return keys.BinarySearch (key); }
            public int Search (TKey key, IComparer<TKey> comp) { return keys.BinarySearch (key, comp); }
            public void SetKey (int index, TKey key) { keys[index] = key; }
            public void RemoveKey (int index) { keys.RemoveAt (index); }
            public void RemoveKeys (int index, int count) { keys.RemoveRange (index, count); }
            public void TruncateKeys (int index) { keys.RemoveRange (index, keys.Count - index); }
            public void InsertKey (int index, TKey key) { keys.Insert (index, key); }

#if DEBUG
            public StringBuilder Append (StringBuilder sb)
            {
                for (int ix = 0; ix < this.KeyCount; ix++)
                {
                    if (ix > 0)
                        sb.Append (',');
                    sb.Append (GetKey (ix));
                }
                return sb;
            }
#endif
        }


        private class Branch : Node
        {
            private List<Node> childNodes;

            public Branch (Branch leftBranch) : base (leftBranch.ChildCount)
            {
                Init (leftBranch.ChildCount);
            }

            public Branch (Node child, int order) : base (order)
            {
                Init (order);
                Add (child);
            }

            private void Init (int order)
            { childNodes = new List<Node> (order); }

            public int ChildCount
            { get { return childNodes.Count; } }

            public Node GetChild (int childIndex)
            { return childNodes[childIndex]; }

            public Node FirstChild
            { get { return childNodes[0]; } }

            public void RemoveChild (int index)
            { childNodes.RemoveAt (index); }

            public void Truncate (int index)
            {
                TruncateKeys (index);
                childNodes.RemoveRange (index + 1, childNodes.Count - (index + 1));
            }

            public void Add (Node node)
            { childNodes.Add (node); }

            public void Add (TKey key, Node node)
            {
                AddKey (key);
                childNodes.Add (node);
            }

            public void Insert (int index, Node node)
            {
                childNodes.Insert (index, node);
            }

            public void Remove (int index, int count)
            {
                RemoveKeys (index, count);
                childNodes.RemoveRange (index, count);
            }
        }


        private class Leaf : Node
        {
            private Leaf rightLeaf;       // For the linked leaf list.
            private List<TValue> values;  // Payload.

            public Leaf (int order) : base (order)
            {
                values = new List<TValue> (order - 1);
                rightLeaf = null;
            }

            /// <summary>Splice a leaf to right of <paramref name="leftLeaf"/>.</summary>
            /// <param name="leftLeaf">Provides linked list insert point.</param>
            public Leaf (Leaf leftLeaf) : base (leftLeaf.KeyCapacity + 1)
            {
                values = new List<TValue> (leftLeaf.KeyCapacity);

                // Linked list insertion.
                rightLeaf = leftLeaf.rightLeaf;
                leftLeaf.rightLeaf = this;
            }


            /// <summary>Give next leaf in linked list.</summary>
            public Leaf RightLeaf
            {
                get { return rightLeaf; }
                set { rightLeaf = value; }
            }


            public int ValueCount
            { get { return values.Count; } }


            public KeyValuePair<TKey, TValue> GetPair (int pairIndex)
            { return new KeyValuePair<TKey, TValue> (keys[pairIndex], values[pairIndex]); }


            public TValue GetValue (int valueIndex)
            { return values[valueIndex]; }


            public void SetValue (int valueIndex, TValue value)
            { values[valueIndex] = value; }


            public void Add (TKey key, TValue value)
            {
                AddKey (key);
                values.Add (value);
            }

            public void Add (Leaf source, int sourceStart, int sourceStop)
            {
                for (int ix = sourceStart; ix < sourceStop; ++ix)
                    Add (source.GetKey (ix), source.GetValue (ix));
            }

            public void Insert (int index, TKey key, TValue value)
            {
                Debug.Assert (index >= 0 && index <= ValueCount);
                InsertKey (index, key);
                values.Insert (index, value);
            }

            public void Remove (int index)
            {
                Debug.Assert (index >= 0 && index <= ValueCount);
                RemoveKey (index);
                values.RemoveAt (index);
            }

            public void Remove (int index, int count)
            {
                Debug.Assert (index >= 0 && index + count <= ValueCount);
                RemoveKeys (index, count);
                values.RemoveRange (index, count);
            }

            public void Truncate (int index)
            {
                Debug.Assert (index >= 0 && index < ValueCount);
                RemoveKeys (index, KeyCount - index);
                values.RemoveRange (index, ValueCount - index);
            }
        }
    }
}
