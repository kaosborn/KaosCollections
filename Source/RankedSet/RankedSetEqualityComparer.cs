//
// Library: KaosCollections
// File:    RankedSetEqualityComparer.cs
//
// Copyright © 2009-2019 Kasey Osborn (github.com/kaosborn)
// MIT License - Use and redistribute freely
//

using System.Collections.Generic;

namespace Kaos.Collections
{
    public partial class RankedSet<T>
    {
        private class RankedSetEqualityComparer : IEqualityComparer<RankedSet<T>>
        {
            private readonly IComparer<T> comparer;
            private readonly IEqualityComparer<T> equalityComparer;

            public RankedSetEqualityComparer (IEqualityComparer<T> equalityComparer)
            {
                this.comparer = Comparer<T>.Default;
                this.equalityComparer = equalityComparer ?? EqualityComparer<T>.Default;
            }

            public bool Equals (RankedSet<T> s1, RankedSet<T> s2) => RankedSet<T>.RankedSetEquals (s1, s2, comparer);

            public int GetHashCode (RankedSet<T> set)
            {
                int hashCode = 0;
                if (set != null)
                    foreach (T item in set)
                        hashCode = hashCode ^ (equalityComparer.GetHashCode (item) & 0x7FFFFFFF);

                return hashCode;
            }

            public override bool Equals (object obComparer)
            {
                var rsComparer = obComparer as RankedSetEqualityComparer;
                return rsComparer != null && comparer == rsComparer.comparer;
            }

            public override int GetHashCode() => comparer.GetHashCode() ^ equalityComparer.GetHashCode();
        }
    }
}
