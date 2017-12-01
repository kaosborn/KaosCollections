//
// Program: RsBench01.cs
// Purpose: Benchmark SortedSet.RemoveWhere versus RankedSet.RemoveWhere (v4).
//
// Usage notes:
// • Adjust 'maxN' to change test duration.
// • For valid time results, run Release build outside Visual Studio.
//

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Kaos.Collections;
[assembly: System.Reflection.AssemblyVersion ("0.1.0.0")]

namespace BenchApp
{
    class RsBench01
    {
        static int division = 1, divisionCount = 10;
        static bool IsCull (int x) => x % divisionCount < division;

        static void Main()
        {
            int maxN = 500000;
            var watch = new Stopwatch();
            bool isPass1 = true;

            Console.WriteLine ("Removals in Thousands,Set Size in Thousands,RankedSet,SortedSet");

            for (int n = maxN/10; n <= maxN;)
            {
                var c1 = new RankedSet<int>();
                for (int i = 0; i < n; ++i) c1.Add (i);
                watch.Reset(); watch.Start();
                c1.RemoveWhere (IsCull);
                var c1Time = watch.ElapsedMilliseconds;
                int c1Count = c1.Count; c1.Clear();

                var c2 = new SortedSet<int>();
                for (int i = 0; i < n; ++i) c2.Add (i);
                watch.Reset(); watch.Start();
                c2.RemoveWhere (IsCull);
                var c2Time = watch.ElapsedMilliseconds;
                int c2Count = c2.Count; c2.Clear();

                int removes = n * division / divisionCount;
                if (c1Count != n-removes || c2Count != n-removes)
                    Console.WriteLine ("*** ERROR ***" + c1Count + ", " + c2Count);

                if (isPass1)
                    isPass1 = false;
                else
                {
                    Console.WriteLine ("{0},{1},{2},{3}", removes/1000, n/1000, c1Time, c2Time);
                    n += maxN/10;
                }
            }
        }

        /* Output:

        Removals in Thousands,Set Size in Thousands,RankedSet,SortedSet
        5,50,5,1068
        10,100,8,5264
        15,150,13,12531
        20,200,16,21113
        25,250,20,23936
        30,300,24,51612
        35,350,28,59628
        40,400,32,85661
        45,450,36,91218
        50,500,40,107109

        */
    }
}
