//
// Library: KaosCollections
// File:    TDebugView.cs
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
    internal class ICollectionDebugView<T>
    {
        private readonly ICollection<T> target;

        public ICollectionDebugView (ICollection<T> collection)
         => this.target = collection ?? throw new ArgumentNullException (nameof (collection));

        [DebuggerBrowsable (DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get
            {
                var items = new T[target.Count];
                target.CopyTo (items, 0);
                return items;
            }
        }
    }


    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal class IEnumerableDebugView<T>
    {
        [DebuggerBrowsable (DebuggerBrowsableState.Never)]
        private readonly IEnumerable<T> target;

        public IEnumerableDebugView (IEnumerable<T> enumerable)
         => this.target = enumerable ?? throw new ArgumentNullException (nameof (enumerable));

        [DebuggerBrowsable (DebuggerBrowsableState.RootHidden)]
        public IEnumerable<T> Items
        {
            get
            {
                ((System.Collections.IEnumerator) target).Reset();
                foreach (T item in target)
                    yield return item;
            }
        }
    }
}
