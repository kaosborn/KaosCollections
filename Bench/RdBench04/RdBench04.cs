//
// Program: RdBench04.cs
// Purpose: Benchmark performance on GUID add operations.
//
// Usage notes:
// - For valid results, run Release build outside Visual Studio.
// - Adjust reps to change test duration.  Higher values show greater RankedDictionary improvements.
//

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Kaos.Collections;
[assembly: System.Reflection.AssemblyVersion ("0.1.0.0")]

namespace BenchApp
{
    class RdBench04
    {
        static void Main()
        {
            int reps = 5000000;
            var sd = new SortedDictionary<Guid,int>();
            Console.Write ("Loading SortedDictionary with " + reps + " elements:\n\nLoad time = ");

            Stopwatch watch1 = new Stopwatch();
            watch1.Reset();
            watch1.Start();

            for (int i = 0; i < reps; ++i)
            {
                var guid = Guid.NewGuid();
                sd.Add (guid, i);
            }

            var time11 = watch1.ElapsedMilliseconds;
            Console.WriteLine (time11 + "ms");

            ////

            for (int order=32; order <= 256; order+=16)
            {
                Btree.TreeOrder = order;
                var bt = new RankedDictionary<Guid,int>();
                Console.Write ("\nLoading BtreeDictionary (order="+order+") with " + reps + " elements:\n\nLoad time = ");

                Stopwatch watch2 = new Stopwatch();
                watch2.Reset();
                watch2.Start();

                for (int i = 0; i < reps; ++i)
                {
                    var guid = Guid.NewGuid();
                    bt.Add (guid, i);
                }

                var time21 = watch2.ElapsedMilliseconds;
                Console.WriteLine (time21 + "ms");
            }
        }
    }
}
