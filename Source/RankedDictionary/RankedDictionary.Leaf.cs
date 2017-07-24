//
// Library: KaosCollections
// File:    RankedDictionary.Leaf.cs
// Purpose: Define Leaf nested class.
//
// Copyright © 2009-2017 Kasey Osborn (github.com/kaosborn)
// MIT License - Use and redistribute freely
//

using System.Collections.Generic;
using System.Diagnostics;

namespace Kaos.Collections
{
    public partial class RankedDictionary<TKey,TValue>
    {
        /// <summary>An terminal B+ tree page.</summary>
        /// <remarks>
        /// All key/value pairs are contained in this class.
        /// Leaf is a sequential linked list also referenced by parent branches.
        /// </remarks>
        private sealed class Leaf : KeyLeaf
        {
            private readonly List<TValue> values;  // Payload.

            /// <summary>Create a siblingless leaf.</summary>
            /// <param name="capacity">The initial number of elements the page can store.</param>
            public Leaf (int capacity=0) : base (capacity)
            {
                this.values = new List<TValue> (capacity);
            }

            /// <summary>Splice this leaf to right of <paramref name="leftLeaf"/>.</summary>
            /// <param name="leftLeaf">Provides linked list insert point.</param>
            /// <param name="capacity">The initial number of elements the page can store.</param>
            public Leaf (Leaf leftLeaf, int capacity) : base (leftLeaf, capacity)
            {
                this.values = new List<TValue> (capacity);
            }

            /// <summary>Next leaf in linked list.</summary>
            public new Leaf RightLeaf
            { get { return (Leaf) base.RightLeaf; } }


            public int ValueCount
            { get { return values.Count; } }


            public KeyValuePair<TKey,TValue> GetPair (int index)
            { return new KeyValuePair<TKey,TValue> (keys[index], values[index]); }


            public static TValue GetValue (NodeVector path)
            {
                var leaf = (Leaf) path.TopNode;
                return leaf.values[path.TopNodeIndex];
            }

            public static void SetValue (NodeVector path, TValue value)
            {
                var leaf = (Leaf) path.TopNode;
                leaf.values[path.TopNodeIndex] = value;
            }


            public TValue GetValue (int index)
            { return values[index]; }


            public void SetValue (int index, TValue value)
            { values[index] = value; }


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

            public override void Coalesce()
            {
                var right = (Leaf) rightKeyLeaf;
                for (int ix = 0; ix < right.values.Count; ++ix)
                    values.Add (right.values[ix]);
                base.Coalesce();
            }

            public override void Shift (int shiftCount)
            {
                Leaf right = RightLeaf;
                for (int ix = 0; ix < shiftCount; ++ix)
                    values.Add (right.values[ix]);
                base.Shift (shiftCount);
            }

            public override void Truncate (int index)
            {
                Debug.Assert (index >= 0 && (values.Count == 0 || index < values.Count));
                values.RemoveRange (index, values.Count - index);
                base.Truncate (index);
            }

            public void Insert (int index, TKey key, TValue value)
            {
                Debug.Assert (index >= 0 && index <= ValueCount);
                InsertKey (index, key);
                values.Insert (index, value);
            }

            public override void Remove (int index)
            {
                Debug.Assert (index >= 0 && index <= ValueCount);
                values.RemoveAt (index);
                base.Remove (index);
            }

            public void Remove (int index, int count)
            {
                Debug.Assert (index >= 0 && index + count <= ValueCount);
                RemoveKeys (index, count);
                values.RemoveRange (index, count);
            }
        }
    }
}
