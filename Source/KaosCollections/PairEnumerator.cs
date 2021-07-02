//
// Library: KaosCollections
// File:    PairEnumerator.cs
//
// Copyright © 2009-2020 Kasey Osborn (github.com/kaosborn)
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
        private protected class PairEnumerator<V> : BaseEnumerator
        {
            public KeyValuePair<T,V> CurrentPair { get; private set; }
            public bool NonGeneric { get; private set; }

            public KeyValuePair<T,V> CurrentPairOrDefault => NotActive ? default : CurrentPair;
            public DictionaryEntry CurrentEntry => new DictionaryEntry (CurrentPair.Key, CurrentPair.Value);

            public PairEnumerator (Btree<T> owner, bool isReverse=false, bool nonGeneric=false) : base (owner, isReverse)
             => this.NonGeneric = nonGeneric;

            public PairEnumerator (Btree<T> owner, int count) : base (owner, count)
            { }

            public PairEnumerator (Btree<T> owner, Func<KeyValuePair<T,V>,bool> condition) : this (owner)
             => Bypass2 (condition, (leaf,ix) => ((PairLeaf<V>) leaf).GetPair (ix));

            public PairEnumerator (Btree<T> owner, Func<KeyValuePair<T,V>,int,bool> condition) : this (owner)
             => Bypass3 (condition, (leaf,ix) => ((PairLeaf<V>) leaf).GetPair (ix));

            public void Initialize()
            {
                Init();
                CurrentPair = default;
            }

            public bool Advance()
            {
                if (AdvanceBase())
                { CurrentPair = ((PairLeaf<V>) leaf).GetPair (leafIndex); return true; }
                else
                { CurrentPair = default; return false; }
            }

            public void BypassPair (Func<KeyValuePair<T,V>,bool> condition)
             => Bypass2 (condition, (leaf,ix) => ((PairLeaf<V>) leaf).GetPair (ix));

            public void BypassPair (Func<KeyValuePair<T,V>,int,bool> condition)
             => Bypass3 (condition, (leaf,ix) => ((PairLeaf<V>) leaf).GetPair (ix));
        }
    }
}
