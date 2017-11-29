//
// Program: RsBench01.cs
// Purpose: Benchmark old RemoveWhere (v3.1) versus new RemoveWhere (v4.0).
//
// Usage notes:
// • Without a special development configuration this test is not reproducible.
//

using System;
using System.Diagnostics;
using Kaos.Collections;
[assembly: System.Reflection.AssemblyVersion ("0.1.0.0")]

namespace BenchApp
{
    class RsBench01
    {
        static int division, divisionCount = 10;
        static bool IsCull (int x) => x % divisionCount < division;

        static void Main()
        {
            int n = 50000000;
            var watch = new Stopwatch();

            Console.WriteLine ("n;Removals;RankedBagMs;RankedSetMs");

            for (division = 0; division <= divisionCount; ++division)
            {
                // RankedBag is using the old algorithm:
                var c1 = new RankedBag<int>();
                for (int i = 0; i < n; ++i) c1.Add (i);
                watch.Reset(); watch.Start();
                c1.RemoveWhere (IsCull);
                var c1Time = watch.ElapsedMilliseconds;
                int c1Count = c1.Count; c1.Clear();

                // RankedSet is using the new algorithm:
                var c2 = new RankedSet<int>();
                for (int i = 0; i < n; ++i) c2.Add (i);
                watch.Reset(); watch.Start();
                c2.RemoveWhere (IsCull);
                var c2Time = watch.ElapsedMilliseconds;
                int c2Count = c2.Count; c2.Clear();

                int removes = n * division / divisionCount;
                Console.WriteLine ("{0},{1},{2},{3}", n, removes, c1Time, c2Time);
            }
        }


        /* Output for old RemoveWhere (RankedBag) vs. new RemoveWhere (RankedSet):

        n;Removals;RankedBagMs;RankedSetMs
        50000000,0,2999,3908
        50000000,5000000,18050,4183
        50000000,10000000,32200,4142
        50000000,15000000,47197,4172
        50000000,20000000,60528,4105
        50000000,25000000,76493,6725
        50000000,30000000,87890,6391
        50000000,35000000,106109,6483
        50000000,40000000,115870,7819
        50000000,45000000,133775,7950
        50000000,50000000,121599,5761

        */
    }
}
