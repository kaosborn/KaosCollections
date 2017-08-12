//
// Program: RsBench06.cs
// Purpose: Benchmark RankedSet versus SortedSet adds and indexing.
//
// Usage notes:
// • Adjust 'reps' to change final test size.
// • Adjust 'divs' to change number of proportial tests.
// • For valid time results, run Release build outside Visual Studio.
//

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Kaos.Collections;
[assembly: AssemblyVersion ("0.1.0.0")]

namespace BenchApp
{
    static class RsBench06
    {
        static int reps = 1000000;
        static int divs = 10;
        static readonly SortedSet<int> ss = new SortedSet<int>();
        static readonly RankedSet<int> rs = new RankedSet<int>();

        static void Main()
        {
            var watch = new Stopwatch();

            Console.WriteLine ("RankedSet Bulk Add and indexing");
            Console.WriteLine (divs.ToString() + " increments of " + (reps / divs));
            Console.WriteLine ("RankedSet Size;SortedSet Size;RankedSet Add;SortedSet Add;RankedSet ElementAt;SortedSet ELementAt;RankedSetLast;SortedSet Last");

            for (int k1 = 0; k1 <= divs; ++k1)
            {
                int reps0 = k1==0 ? reps : (reps / divs) * k1;
                rs.Clear();
                System.Threading.Thread.Sleep (200);

                watch.Reset(); watch.Start();
                for (int ix = 0; ix < reps0; ++ix)
                    rs.Add (ix);
                var loadRs = watch.ElapsedMilliseconds;

                ss.Clear();
                System.Threading.Thread.Sleep (200);
                watch.Reset(); watch.Start();
                for (int ix = 0; ix < reps0; ++ix)
                    ss.Add (ix);
                var loadSs = watch.ElapsedMilliseconds;

                watch.Reset(); watch.Start();
                var ss1 = ss.Last();
                var lastSs = watch.ElapsedMilliseconds;
                watch.Reset(); watch.Start();
                var ss2 = ss.ElementAt (reps0-1);
                var elAtSs = watch.ElapsedMilliseconds;

                watch.Reset(); watch.Start();
                var rs1 = rs.Last();
                var lastRs = watch.ElapsedMilliseconds;
                watch.Reset(); watch.Start();
                var rs2 = rs.ElementAt (reps0-1);
                var elAtRs = watch.ElapsedMilliseconds;

                if (k1 > 0)
                    Console.WriteLine ("{0,8};{1,8};{2,5};{3,5};{4,2};{5,4};{6,2};{7,4}",
                        rs.Count, ss.Count, loadRs, loadSs, lastRs, lastSs, elAtRs, elAtSs);
            }
        }
    }
}
