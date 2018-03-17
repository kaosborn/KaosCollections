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
            private Func<V,bool> conditionV2;

            public ValueEnumerator (Btree<T> owner, bool isReverse=false) : base (owner, isReverse)
            { }

            public ValueEnumerator (Btree<T> owner, int count) : base (owner, count)
            { }

            public ValueEnumerator (Btree<T> owner, Func<V,bool> condition) : this (owner)
            { Bypass (condition); }

            public V CurrentValue => ((PairLeaf<V>) leaf).GetValue (leafIndex);

            public V CurrentValueOrDefault => state != 0 ? default : ((PairLeaf<V>) leaf).GetValue (leafIndex);

            protected override bool TestWhile2() => conditionV2 (((PairLeaf<V>) leaf).GetValue (leafIndex));

            public void Bypass (Func<V,bool> bypassCondition)
            {
                conditionV2 = bypassCondition;
                BypassWhile2();
            }
        }


        /// <exclude />
        private protected class PairEnumerator<V> : KeyEnumerator
        {
            private Func<KeyValuePair<T,V>,bool> conditionE2;
            public bool NonGeneric { get; private set; }

            public PairEnumerator (Btree<T> owner, bool isReverse=false, bool nonGeneric=false) : base (owner, isReverse)
            { this.NonGeneric = nonGeneric; }

            public PairEnumerator (Btree<T> owner, int count) : base (owner, count)
            { }

            public PairEnumerator (Btree<T> owner, Func<KeyValuePair<T,V>,bool> condition) : this (owner)
            { Bypass (condition); }

            public DictionaryEntry CurrentEntry => new DictionaryEntry (leaf.GetKey (leafIndex), ((PairLeaf<V>) leaf).GetValue (leafIndex));

            public KeyValuePair<T,V> CurrentPair => ((PairLeaf<V>) leaf).GetPair (leafIndex);

            public KeyValuePair<T,V> CurrentPairOrDefault
                => state != 0 ? new KeyValuePair<T,V> (default, default) : ((PairLeaf<V>) leaf).GetPair (leafIndex);

            public V CurrentValue => ((PairLeaf<V>) leaf).GetValue (leafIndex);

            protected override bool TestWhile2() => conditionE2 (((PairLeaf<V>) leaf).GetPair (leafIndex));

            public void Bypass (Func<KeyValuePair<T,V>,bool> bypassCondition)
            {
                conditionE2 = bypassCondition;
                BypassWhile2();
            }
        }
    }
}
