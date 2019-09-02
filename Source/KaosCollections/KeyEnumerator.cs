//
// Library: KaosCollections
// File:    KeyEnumerator.cs
//
// Copyright © 2009-2019 Kasey Osborn (github.com/kaosborn)
// MIT License - Use and redistribute freely
//

using System;

namespace Kaos.Collections
{
    public abstract partial class Btree<T>
    {
        /// <exclude />
        private protected class KeyEnumerator : BaseEnumerator
        {
            public T CurrentKey { get; private set; }

            public T CurrentKeyOrDefault
             => NotActive ? default : CurrentKey;

            public KeyEnumerator (Btree<T> owner, bool isReverse=false) : base (owner, isReverse)
            { }

            public KeyEnumerator (Btree<T> owner, int count) : base (owner, count)
            { }

            public KeyEnumerator (Btree<T> owner, Func<T,bool> condition) : base (owner)
             => Bypass2 (condition, (leaf,ix) => leaf.GetKey (ix));

            public KeyEnumerator (Btree<T> owner, Func<T,int,bool> condition) : base (owner)
             => Bypass3 (condition, (leaf,ix) => leaf.GetKey (ix));


            public void Initialize()
            {
                Init();
                CurrentKey = default;
            }

            public bool Advance()
            {
                if (AdvanceBase())
                  { CurrentKey = leaf.GetKey (leafIndex); return true; }
                else
                  { CurrentKey = default; return false; }
            }

            public void BypassKey (Func<T,bool> condition)
             => Bypass2 (condition, (leaf,ix) => leaf.GetKey (ix));

            public void BypassKey (Func<T,int,bool> condition)
             => Bypass3 (condition, (leaf,ix) => leaf.GetKey (ix));
       }
    }
}
