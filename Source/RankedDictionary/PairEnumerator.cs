//
// Library: KaosCollections
// File:    PairEnumerator.cs
//
// Copyright © 2009-2018 Kasey Osborn (github.com/kaosborn)
// MIT License - Use and redistribute freely
//

using System;
using System.Collections;
using System.Collections.Generic;

namespace Kaos.Collections
{
    public abstract partial class Btree<T>
    {
        /// <exclude />
        private protected class ValueEnumerator<V> : KeyEnumerator
        {
            public ValueEnumerator (Btree<T> owner, bool isReverse=false) : base (owner, isReverse)
            { }

            public ValueEnumerator (Btree<T> owner, int count) : base (owner, count)
            { }

            public ValueEnumerator (Btree<T> owner, Func<V,bool> condition) : this (owner)
            { Bypass2 (condition, (leaf,ix) => ((PairLeaf<V>) leaf).GetValue (ix)); }

            public ValueEnumerator (Btree<T> owner, Func<V,int,bool> condition) : this (owner)
            { Bypass3 (condition, (leaf,ix) => ((PairLeaf<V>) leaf).GetValue (ix)); }


            public V CurrentValue { get; private set; }

            public V CurrentValueOrDefault => state != 0 ? default : CurrentValue;

            public new bool Advance()
            {
                bool ok = base.Advance();
                CurrentValue = ok ? ((PairLeaf<V>) leaf).GetValue (leafIndex) : default;
                return ok;
            }

            public void BypassValue (Func<V,bool> condition) => Bypass2 (condition, (leaf,ix) => ((PairLeaf<V>) leaf).GetValue (ix));

            public void BypassValue (Func<V,int,bool> condition) => Bypass3 (condition, (leaf,ix) => ((PairLeaf<V>) leaf).GetValue (ix));
        }


        /// <exclude />
        private protected class PairEnumerator<V> : KeyEnumerator
        {
            public bool NonGeneric { get; private set; }

            public PairEnumerator (Btree<T> owner, bool isReverse=false, bool nonGeneric=false) : base (owner, isReverse)
            { this.NonGeneric = nonGeneric; }

            public PairEnumerator (Btree<T> owner, int count) : base (owner, count)
            { }

            public PairEnumerator (Btree<T> owner, Func<KeyValuePair<T,V>,bool> condition) : this (owner)
            { Bypass2 (condition, (leaf,ix) => ((PairLeaf<V>) leaf).GetPair (ix)); }

            public PairEnumerator (Btree<T> owner, Func<KeyValuePair<T,V>,int,bool> condition) : this (owner)
            { Bypass3 (condition, (leaf,ix) => ((PairLeaf<V>) leaf).GetPair (ix)); }


            public DictionaryEntry CurrentEntry => new DictionaryEntry (CurrentPair.Key, CurrentPair.Value);

            public KeyValuePair<T,V> CurrentPair { get; private set; }

            public KeyValuePair<T,V> CurrentPairOrDefault
                => state != 0 ? new KeyValuePair<T,V> (default, default) : CurrentPair;

            public V CurrentValue => CurrentPair.Value;

            public new bool Advance()
            {
                bool ok = base.Advance();
                CurrentPair = ok ? ((PairLeaf<V>) leaf).GetPair (leafIndex) : default;
                return ok;
            }

            public void BypassPair (Func<KeyValuePair<T,V>,bool> condition) => Bypass2 (condition, (leaf,ix) => ((PairLeaf<V>) leaf).GetPair (ix));

            public void BypassPair (Func<KeyValuePair<T,V>,int,bool> condition) => Bypass3 (condition, (leaf,ix) => ((PairLeaf<V>) leaf).GetPair (ix));
        }
    }
}
