//
// Library: KaosCollections
// File:    IDictionaryDebugView.cs
//
// Copyright © 2009-2017 Kasey Osborn (github.com/kaosborn)
// MIT License - Use and redistribute freely
//

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Kaos.Collections
{
    internal class IDictionaryDebugView<K,V>
    {
        private readonly IDictionary<K,V> target;

        public IDictionaryDebugView (IDictionary<K,V> dictionary)
        {
            if (dictionary == null)
                throw new ArgumentNullException (nameof (dictionary));
            this.target = dictionary;
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


    internal class ICollectionKeysDebugView<K,V>
    {
        private readonly ICollection<K> target;

        public ICollectionKeysDebugView (ICollection<K> collection)
        {
            if (collection == null)
                throw new ArgumentNullException (nameof (collection));
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


    internal class ICollectionValuesDebugView<K,V>
    {
        private readonly ICollection<V> target;

        public ICollectionValuesDebugView (ICollection<V> collection)
        {
            if (collection == null)
                throw new ArgumentNullException (nameof (collection));
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
