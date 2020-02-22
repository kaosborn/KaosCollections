//
// Library: KaosCollections
// File:    KVDebugView.cs
//
// Copyright © 2009-2020 Kasey Osborn (github.com/kaosborn)
// MIT License - Use and redistribute freely
//

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Kaos.Collections
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal class ICollectionDebugView<K,V>
    {
        private readonly ICollection<KeyValuePair<K,V>> target;

        public ICollectionDebugView (ICollection<KeyValuePair<K,V>> collection)
         => this.target = collection ?? throw new ArgumentNullException (nameof (collection));

        [DebuggerBrowsable (DebuggerBrowsableState.RootHidden)]
        public KeyValuePair<K,V>[] Items
        {
            get
            {
                var items = new KeyValuePair<K,V>[target.Count];
                target.CopyTo (items, 0);
                return items;
            }
        }
    }


    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal class ICollectionKeysDebugView<K,V>
    {
        private readonly ICollection<K> target;

        public ICollectionKeysDebugView (ICollection<K> collection)
         => this.target = collection ?? throw new ArgumentNullException (nameof (collection));

        [DebuggerBrowsable (DebuggerBrowsableState.RootHidden)]
        public K[] Items
        {
            get
            {
                var items = new K[target.Count];
                target.CopyTo (items, 0);
                return items;
            }
        }
    }


    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal class ICollectionValuesDebugView<K,V>
    {
        private readonly ICollection<V> target;

        public ICollectionValuesDebugView (ICollection<V> collection)
         => this.target = collection ?? throw new ArgumentNullException (nameof (collection));

        [DebuggerBrowsable (DebuggerBrowsableState.RootHidden)]
        public V[] Items
        {
            get
            {
                var items = new V[target.Count];
                target.CopyTo (items, 0);
                return items;
            }
        }
    }


    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal class IEnumerableDebugView<K,V>
    {
        [DebuggerBrowsable (DebuggerBrowsableState.Never)] 
        private readonly IEnumerable<KeyValuePair<K,V>> target;

        public IEnumerableDebugView (IEnumerable<KeyValuePair<K,V>> enumerable)
         => this.target = enumerable ?? throw new ArgumentNullException (nameof (enumerable));

        [DebuggerBrowsable (DebuggerBrowsableState.RootHidden)]
        public IEnumerable<KeyValuePair<K,V>> Items
        {
            get
            {
                ((System.Collections.IEnumerator) target).Reset();
                foreach (var item in target)
                    yield return item;
            }
        }
    }


    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal class IEnumerableKeysDebugView<K,V>
    {
        [DebuggerBrowsable (DebuggerBrowsableState.Never)] 
        private readonly IEnumerable<K> target;

        public IEnumerableKeysDebugView (IEnumerable<K> enumerable)
         => this.target = enumerable ?? throw new ArgumentNullException (nameof (enumerable));

        [DebuggerBrowsable (DebuggerBrowsableState.RootHidden)]
        public IEnumerable<K> Items
        {
            get
            {
                ((System.Collections.IEnumerator) target).Reset();
                foreach (var item in target)
                    yield return item;
            }
        }
    }


    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal class IEnumerableValuesDebugView<K,V>
    {
        [DebuggerBrowsable (DebuggerBrowsableState.Never)] 
        private readonly IEnumerable<V> target;

        public IEnumerableValuesDebugView (IEnumerable<V> enumerable)
         => this.target = enumerable ?? throw new ArgumentNullException (nameof (enumerable));

        [DebuggerBrowsable (DebuggerBrowsableState.RootHidden)]
        public IEnumerable<V> Items
        {
            get
            {
                ((System.Collections.IEnumerator) target).Reset();
                foreach (var item in target)
                    yield return item;
            }
        }
    }
}
