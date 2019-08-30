//
// Library: KaosCollections
// File:    KVDebugView.cs
//
// Copyright © 2009-2018 Kasey Osborn (github.com/kaosborn)
// MIT License - Use and redistribute freely
//

using System;
//
// Library: KaosCollections
// File:    KVDebugView.cs
//
// Copyright © 2009-2019 Kasey Osborn (github.com/kaosborn)
// MIT License - Use and redistribute freely
//

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


#if ! NET35 && ! NETSTANDARD1_0
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
    internal class IEnumerableDebugView<K,V>
    {
        [DebuggerBrowsable (DebuggerBrowsableState.Never)] 
        private readonly IEnumerable<KeyValuePair<K,V>> target;

        public IEnumerableDebugView (IEnumerable<KeyValuePair<K,V>> enumerable)
        {
            if (enumerable == null)
                throw new ArgumentNullException (nameof (enumerable));
            this.target = enumerable;
        }

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


#if ! NET35 && ! NETSTANDARD1_0
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
    internal class IEnumerableKeysDebugView<K,V>
    {
        [DebuggerBrowsable (DebuggerBrowsableState.Never)] 
        private readonly IEnumerable<K> target;

        public IEnumerableKeysDebugView (IEnumerable<K> enumerable)
        {
            if (enumerable == null)
                throw new ArgumentNullException (nameof (enumerable));
            this.target = enumerable;
        }

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

#if ! NET35 && ! NETSTANDARD1_0
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
    internal class IEnumerableValuesDebugView<K,V>
    {
        [DebuggerBrowsable (DebuggerBrowsableState.Never)] 
        private readonly IEnumerable<V> target;

        public IEnumerableValuesDebugView (IEnumerable<V> enumerable)
        {
            if (enumerable == null)
                throw new ArgumentNullException (nameof (enumerable));
            this.target = enumerable;
        }

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
