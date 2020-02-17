//
// Library: KaosCollections
// File:    ValueEnumerator.cs
//
// Copyright © 2009-2020 Kasey Osborn (github.com/kaosborn)
// MIT License - Use and redistribute freely
//

using System;

namespace Kaos.Collections
{
    public abstract partial class Btree<T>
    {
        /// <exclude />
        private protected class ValueEnumerator<V> : BaseEnumerator
        {
            public V CurrentValue { get; private set; }

            public V CurrentValueOrDefault => NotActive ? default : CurrentValue;

            public ValueEnumerator (Btree<T> owner, bool isReverse=false) : base (owner, isReverse)
            { }

            public ValueEnumerator (Btree<T> owner, int count) : base (owner, count)
            { }

            public ValueEnumerator (Btree<T> owner, Func<V,bool> condition) : base (owner)
             => Bypass2 (condition, (leaf,ix) => ((PairLeaf<V>) leaf).GetValue (ix));

            public ValueEnumerator (Btree<T> owner, Func<V,int,bool> condition) : base (owner)
             => Bypass3 (condition, (leaf,ix) => ((PairLeaf<V>) leaf).GetValue (ix));

            public void Initialize()
            {
                Init();
                CurrentValue = default;
            }

            public bool Advance()
            {
                if (AdvanceBase())
                  { CurrentValue = ((PairLeaf<V>) leaf).GetValue (leafIndex); return true; }
                else
                  { CurrentValue = default; return false; }
            }

            public void BypassValue (Func<V,bool> condition)
             => Bypass2 (condition, (leaf,ix) => ((PairLeaf<V>) leaf).GetValue (ix));

            public void BypassValue (Func<V,int,bool> condition)
             => Bypass3 (condition, (leaf,ix) => ((PairLeaf<V>) leaf).GetValue (ix));
        }
    }
}
