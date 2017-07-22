//
// Program: BtreeBench03.cs
// Purpose: Benchmark BtreeDictionary fill percent.
//
// Usage notes:
// • Run in Debug build for diagnostics.
//

using System;
using System.Reflection;
using Kaos.Collections;
[assembly: AssemblyVersion ("0.1.0.0")]

namespace BenchApp
{
    public class BtreeBench03
    {
        static RankedDictionary<Guid,int> tree;

        static void Main (string[] args)
        {
            foreach (var reps in new int[] { 100, 1000, 10000, 100000, 1000000, 10000000, 20000000, 40000000 })
            {
                Btree.TreeOrder = 128;
                tree = new RankedDictionary<Guid,int>();

                for (int ii = 0; ii < reps; ++ii)
                    tree.Add (Guid.NewGuid(), ii);

                Console.WriteLine (reps);
#if DEBUG
                Console.WriteLine (tree.GetTreeStatsText());
#endif
            }

        }
    }
}
