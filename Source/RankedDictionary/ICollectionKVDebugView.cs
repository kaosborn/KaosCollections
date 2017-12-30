//
// Library: KaosCollections
// File:    ICollectionKVDebugView.cs
//
// Copyright © 2009-2018 Kasey Osborn (github.com/kaosborn)
// MIT License - Use and redistribute freely
//

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Kaos.Collections
{
#if ! NET35 && ! NETSTANDARD1_0
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
    internal class ICollectionDebugView<K,V>
    {
        private readonly ICollection<KeyValuePair<K,V>> target;

        public ICollectionDebugView (ICollection<KeyValuePair<K,V>> collection)
        {
            if (collection == null)
                throw new ArgumentNullException (nameof (collection));
            this.target = collection;
        }

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


#if ! NET35 && ! NETSTANDARD1_0
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
    internal class ICollectionKeysDebugView<K,V>
    {
        private readonly ICollection<K> target;

        public ICollectionKeysDebugView (ICollection<K> collection)
        {
            if (collection == null)
#pragma warning disable IDE0016
                throw new ArgumentNullException (nameof (collection));
#pragma warning restore IDE0016
            this.target = collection;
        }

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


#if ! NET35 && ! NETSTANDARD1_0
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
    internal class ICollectionValuesDebugView<K,V>
    {
        private readonly ICollection<V> target;

        public ICollectionValuesDebugView (ICollection<V> collection)
        {
            if (collection == null)
#pragma warning disable IDE0016
                throw new ArgumentNullException (nameof (collection));
#pragma warning restore IDE0016
            this.target = collection;
        }

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
}
