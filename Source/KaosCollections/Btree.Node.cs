//
// Library: KaosCollections
// File:    Btree.Node.cs
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
    }
}
