//
// Program: RdBench01.cs
// Purpose: Benchmark SortedDictionary and RankedDictionary comparisons with range query narrative.
//
// Usage notes:
// • Adjust 'reps' to change test duration.  Higher values show greater RankedDictionary improvements.
// • To include diagnostic results, run Debug build.
// • For valid time results, run Release build outside Visual Studio.
//

using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using Kaos.Collections;
[assembly: System.Reflection.AssemblyVersion ("0.1.0.0")]

namespace BenchApp
{
    class RdBench01
    {
        static void Main()
        {
            int reps = 5000000;

            Console.WriteLine ("LINQ's Last extension method will enumerate over the *entire* collection giving");
            Console.WriteLine ("a time complexity of O(n) regardless of the data structure.  This is due to the");
            Console.WriteLine ("\"one size fits all\" approach of LINQ.  SortedDictionary supplies no optimized");
            Console.WriteLine ("implementation of Last. Also, SortedDictionary does not supply any optimized");
            Console.WriteLine ("way of performing queries based on a key range.  Again, the time complexity of");
            Console.WriteLine ("such an operation is O(n).\n");

            var sd = new SortedDictionary<int,int>();

            Console.Write ("Loading SortedDictionary with " + reps + " elements:\n\nLoad time = ");

            Stopwatch watch1 = new Stopwatch();
            watch1.Reset();
            watch1.Start();

            for (int i = 0; i < reps; ++i)
                sd.Add (i, -i);

            var time11 = watch1.ElapsedMilliseconds;

            var last1 = sd.Last();

            var time12 = watch1.ElapsedMilliseconds;

            Console.WriteLine (time11 + "ms");
            Console.WriteLine ("Last time = " + (time12 - time11) + "ms");

            ////

            Console.WriteLine ("\nRankedDictionary has its own implementation of Last() which does not suffer the");
            Console.WriteLine ("performance hit that SortedDictionary.Last() does.  RankedDictionary also");
            Console.WriteLine ("supports optimized range queries with its GetFrom(TKey) and");
            Console.WriteLine ("GetBetween(TKey,TKey) enumerators.\n");

            var bt = new RankedDictionary<int,int>();

            Console.Write ("Loading RankedDictionary with " + reps + " elements:\n\nLoad time = ");

            Stopwatch watch2 = new Stopwatch();
            watch2.Reset();
            watch2.Start();

            for (int i = 0; i < reps; ++i)
                bt.Add (i, -i);

            var time21 = watch2.ElapsedMilliseconds;

            var lastKV = bt.Last();

            var time22 = watch2.ElapsedMilliseconds;

            // Range query: Sum the middle 100 values.
            var rangeVals = bt.GetBetween (reps/2-50, reps/2+50).Sum (x => x.Value);

            var time23 = watch2.ElapsedMilliseconds;

            Console.WriteLine (time21 + "ms");
            Console.WriteLine ("Last time = " + (time22 - time21) + "ms");
            Console.WriteLine ("Range time = " + (time23 - time22) + "ms");

#if DEBUG
            bt.SanityCheck();
            Console.WriteLine();
            Console.Write ("---- height = " + bt.GetHeight());
            Console.Write (", branch fill = " + bt.BranchSlotsUsed * 100 / bt.BranchSlotCount + "%");
            Console.WriteLine (", leaf fill = " + bt.LeafSlotsUsed * 100 / bt.LeafSlotCount + "% ----");
#endif
        }
    }
}
