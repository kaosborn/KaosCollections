using System.Collections;
using System.Collections.Generic;

namespace Kaos.Collections
{
    public abstract partial class Btree<T>
    {
        /// <exclude />
        private protected class ValueEnumerator<V> : KeyEnumerator
        {
            public V CurrentValue => ((PairLeaf<V>) leaf).GetValue (leafIndex);
            public V CurrentValueOrDefault => state != 0 ? default : ((PairLeaf<V>) leaf).GetValue (leafIndex);

            public ValueEnumerator (Btree<T> owner, bool isReverse=false) : base (owner, isReverse)
            { }
        }


        /// <exclude />
        private protected class PairEnumerator<V> : KeyEnumerator
        {
            public bool NonGeneric { get; private set; }

            public DictionaryEntry CurrentEntry => new DictionaryEntry (leaf.GetKey (leafIndex), ((PairLeaf<V>) leaf).GetValue (leafIndex));
            public KeyValuePair<T,V> CurrentPair => ((PairLeaf<V>) leaf).GetPair (leafIndex);
            public KeyValuePair<T,V> CurrentPairOrDefault
                => state != 0 ? new KeyValuePair<T,V> (default, default) : ((PairLeaf<V>) leaf).GetPair (leafIndex);
            public V CurrentValue => ((PairLeaf<V>) leaf).GetValue (leafIndex);

            public PairEnumerator (Btree<T> owner, bool isReverse=false, bool nonGeneric=false) : base (owner, isReverse)
            {
                this.NonGeneric = nonGeneric;
            }
        }
    }
}
