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
        static int reps = 50000000;
        static BtreeDictionary<Guid,int> tree = new BtreeDictionary<Guid,int> (128);

        static void Main (string[] args)
        {
            for (int ii = 0; ii < reps; ++ii)
                tree.Add (Guid.NewGuid(), ii);

            Console.WriteLine (reps);
            Console.WriteLine (tree.GetTreeStatsText());
        }
    }
}
