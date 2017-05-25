//
// Program: BtreeBench04.cs
// Purpose: Performance benchmark GUID adds
//
// Usage notes:
// - For valid results, run Release build outside Visual Studio.
// - Adjust reps to change test duration.  Higher values show greater BtreeDictionary improvements.
//

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Kaos.Collections;

namespace BenchApp
{
    class BtreeBench04
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

            var bt = new BtreeDictionary<Guid,int>();
            Console.Write ("\nLoading BtreeDictionary with " + reps + " elements:\n\nLoad time = ");

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
