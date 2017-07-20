﻿//
// Library: KaosCollections
// File:    ICollectionDebugView.cs
//
// Copyright © 2009-2017 Kasey Osborn (github.com/kaosborn)
// MIT License - Use and redistribute freely
//

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Kaos.Collections
{
    /// <exclude />
    internal class ICollectionDebugView<T>
    {
        private readonly ICollection<T> target;

        public ICollectionDebugView (ICollection<T> dictionary)
        {
            if (dictionary == null)
                throw new ArgumentNullException (nameof (dictionary));
            this.target = dictionary;
        }

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
}
