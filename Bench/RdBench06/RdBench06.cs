//
// Program: RdBench06.cs
// Purpose: Benchmark SortedList v. RankedDictionary indexing.
//
// Usage notes:
// • Adjust 'reps' to change test span.
// • For valid time results, run Release build outside Visual Studio.
//

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Kaos.Collections;
[assembly: System.Reflection.AssemblyVersion ("0.1.0.0")]

namespace BenchApp
{
    class RdBench06
    {
        static void Main()
        {
            int reps = 10000000;
            var sl = new SortedList<int,int>();
            var rd = new RankedDictionary<int,int>();
            var watch = new Stopwatch();

            Console.WriteLine ("Loading SortedList...");
            for (int ix = 0; ix <= reps; ++ix)
                sl.Add (ix, -ix);

            Console.WriteLine ("Loading RankedDictionary...");
            for (int i = 0; i <= reps; ++i)
                rd.Add (i, -i);

            Thread.Sleep (500);

            Console.WriteLine ("Get Element at Index");
            Console.WriteLine ("Time in Milliseconds");
            Console.WriteLine ("Index by " + (reps/10));
            Console.WriteLine ("Count;row;SortedList;RankedDictionary");
            for (int row = 0, ix = 0; ix <= reps; ix += reps/10)
            {
                watch.Reset(); watch.Start();
                var slPair = sl.ElementAt (ix);
                var slTime = watch.ElapsedMilliseconds;
                if (slPair.Key != -slPair.Value)
                    Console.Write ("FAIL");

                watch.Reset(); watch.Start();
                var rdPair = rd.ElementAt (ix);
                var rdTime = watch.ElapsedMilliseconds;
                if (rdPair.Key != -rdPair.Value)
                    Console.Write ("FAIL");

                var lx = String.Format ("{0,9};{1,4};{2,5};{3,5}", ix, row, slTime, rdTime);
                if (row > 0)
                    Console.WriteLine (lx);
                ++row;
            }

            watch.Reset(); watch.Start();
            var slPair2 = sl.Last();
            var slTime2 = watch.ElapsedMilliseconds;
            if (slPair2.Key != -slPair2.Value)
                Console.Write ("FAIL");

            watch.Reset(); watch.Start();
            var rdPair2 = rd.Last();
            var rdTime2 = watch.ElapsedMilliseconds;
            if (rdPair2.Key != -rdPair2.Value)
                Console.Write ("FAIL");

            var lx2 = String.Format ("{0,9};{1};{2,5};{3,5}", reps, "Last", slTime2, rdTime2);
            Console.WriteLine (lx2);
        }

        /* Output:

        Loading SortedList...
        Loading RankedDictionary...
        Get Element at Index
        Time in Milliseconds
        Index by 10000000
        Count;row;SortedList;RankedDictionary
         10000000; 1;  392;    0
         20000000; 2;  779;    0
         30000000; 3; 1169;    0
         40000000; 4; 1568;    0
         50000000; 5; 1954;    0
         60000000; 6; 2345;    0
         70000000; 7; 2731;    0
         80000000; 8; 3133;    0
         90000000; 9; 3527;    0
        100000000;10; 3912;    0

        */
    }
}
