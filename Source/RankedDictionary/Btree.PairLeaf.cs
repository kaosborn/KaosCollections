//
// Library: KaosCollections
// File:    Btree.PairLeaf.cs
// Purpose: Define Btree.PairLeaf class.
//
// Copyright © 2009-2017 Kasey Osborn (github.com/kaosborn)
// MIT License - Use and redistribute freely
//

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Kaos.Collections
{
    public abstract partial class Btree<T>
    {
        /// <summary>A terminal B+ tree node for key/value pairs.</summary>
        internal sealed class PairLeaf<TValue> : Leaf
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
            public PairLeaf (PairLeaf<TValue> leftLeaf, int capacity) : base (leftLeaf, capacity)
            {
                this.values = new List<TValue> (capacity);
            }

            public int ValueCount => values.Count;

            public KeyValuePair<T,TValue> GetPair (int index) => new KeyValuePair<T,TValue> (keys[index], values[index]);

            public TValue GetValue (int index) => values[index];

            public int IndexOfValue (TValue value) => values.IndexOf (value);

            public void SetValue (int index, TValue value)
            { values[index] = value; }

            public void CopyPairLeft (int index, int offset)
            {
                values[index-offset] = values[index];
                keys[index-offset] = keys[index];
            }

            public void Add (T key, TValue value)
            {
                AddKey (key);
                values.Add (value);
            }

            public void Add (PairLeaf<TValue> source, int sourceStart, int sourceStop)
            {
                for (int ix = sourceStart; ix < sourceStop; ++ix)
                    Add (source.GetKey (ix), source.GetValue (ix));
            }

            public void CopyValuesTo (TValue[] array, int index, int count)
            { values.CopyTo (0, array, index, count); }

            public override void Coalesce()
            {
                var right = (PairLeaf<TValue>) rightLeaf;
                for (int ix = 0; ix < right.values.Count; ++ix)
                    values.Add (right.values[ix]);
                base.Coalesce();
            }

            public void Insert (int index, T key, TValue value)
            {
                Debug.Assert (index >= 0 && index <= ValueCount);
                InsertKey (index, key);
                values.Insert (index, value);
            }

            public override void MoveLeft (int count)
            {
                var right = (PairLeaf<TValue>) rightLeaf;
                for (int ix = 0; ix < count; ++ix)
                    values.Add (right.values[ix]);
                right.values.RemoveRange (0, count);
                base.MoveLeft (count);
            }

            public override void Truncate (int index)
            {
                Debug.Assert (index >= 0 && (values.Count == 0 || index < values.Count));
                values.RemoveRange (index, values.Count - index);
                base.Truncate (index);
            }

            public override void RemoveRange (int index, int count)
            {
                base.RemoveRange (index, count);
                values.RemoveRange (index, count);
            }

#if DEBUG
            public override void SanityCheck()
            {
                if (keys.Count != values.Count)
                    throw new InvalidOperationException ("Mismatched keys/values count");
            }
#endif
        }
    }
}
