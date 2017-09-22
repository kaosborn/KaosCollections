//
// Library: KaosCollections
// File:    RankedDictionary.PairLeaf.cs
// Purpose: Define Leaf nested class.
//
// Copyright © 2009-2017 Kasey Osborn (github.com/kaosborn)
// MIT License - Use and redistribute freely
//

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Kaos.Collections
{
    public partial class RankedDictionary<TKey,TValue>
    {
        /// <summary>A terminal B+ tree node for key/value pairs.</summary>
        private sealed class PairLeaf : Leaf
        {
            private readonly List<TValue> values;

            /// <summary>Create a siblingless leaf.</summary>
            /// <param name="capacity">The initial number of elements the page can store.</param>
            public PairLeaf (int capacity=0) : base (capacity)
            {
                this.values = new List<TValue> (capacity);
            }

            /// <summary>Splice this leaf to right of <paramref name="leftLeaf"/>.</summary>
            /// <param name="leftLeaf">Provides linked list insert point.</param>
            /// <param name="capacity">The initial number of elements the page can store.</param>
            public PairLeaf (PairLeaf leftLeaf, int capacity) : base (leftLeaf, capacity)
            {
                this.values = new List<TValue> (capacity);
            }

            public int ValueCount => values.Count;

            public KeyValuePair<TKey,TValue> GetPair (int index) => new KeyValuePair<TKey,TValue> (keys[index], values[index]);

            public TValue GetValue (int index) => values[index];

            public int IndexOfValue (TValue value) => values.IndexOf (value);

            public void SetValue (int index, TValue value)
            { values[index] = value; }

            public void Add (TKey key, TValue value)
            {
                AddKey (key);
                values.Add (value);
            }

            public void Add (PairLeaf source, int sourceStart, int sourceStop)
            {
                for (int ix = sourceStart; ix < sourceStop; ++ix)
                    Add (source.GetKey (ix), source.GetValue (ix));
            }

            public void CopyValuesTo (TValue[] array, int index, int count)
            { values.CopyTo (0, array, index, count); }

            public override void Coalesce()
            {
                var right = (PairLeaf) rightLeaf;
                for (int ix = 0; ix < right.values.Count; ++ix)
                    values.Add (right.values[ix]);
                base.Coalesce();
            }

            public override void Shift (int shiftCount)
            {
                var right = (PairLeaf) rightLeaf;
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

            public override void RemoveRange (int index, int count)
            {
                base.RemoveRange (index, count);
                values.RemoveRange (index, count);
            }
        }
    }
}
