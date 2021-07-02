//
// Program: RdBench05.cs
// Purpose: Benchmark RankedDictionary against SortedDictionary with .csv file results.
//
// Usage notes:
// • For valid results, run Release build outside Visual Studio.
//

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Kaos.Collections;

namespace BenchApp
{
    class RdBench05
    {
        static RankedDictionary<long,long> rd;
        static SortedDictionary<long,long> sd;

        static public long RunSeek (IDictionary<long,long> tree, int reps)
        {
            Stopwatch watch = new Stopwatch();
            watch.Reset(); watch.Start();

            int f = 100;
            long sum = 0;
            for (long i = 0; i < reps / f; ++i)
                for (long j = i; j < reps; j += reps / f)
                    sum += tree[j];
            return watch.ElapsedMilliseconds;
        }

        static public long RunKeyIterate (IDictionary<long,long> tree)
        {
            Stopwatch watch = new Stopwatch();
            watch.Reset(); watch.Start();

            long sum = 0;
            foreach (var key in tree.Keys)
                sum += key;

            return watch.ElapsedMilliseconds;
        }

        static public long RunAddSequential (IDictionary<long,long> tree, int group, int scale)
        {
            int reps = group * scale;
            Stopwatch watch = new Stopwatch();
            watch.Reset(); watch.Start();

            for (long j = 0; j < reps; ++j)
                tree.Add (j, -j);

            return watch.ElapsedMilliseconds;
        }


        static void Main()
        {
            int scale = 1000000;
            long sumRd = 0, sumSd = 0;
            float sumChange = 0.0F;

            System.Threading.Thread.Sleep (500);

            Console.WriteLine ("Ranked Dictionary Add;Sorted Dictionary Add;Ranked Dictionary Seek;Sorted Dictionary Seek;Ranked Dictionary Iterate;Sorted Dictionary Iterate;" + scale);

            for (var group = 1; group <= 10; ++group)
            {
                rd = new RankedDictionary<long,long>();
                sd = new SortedDictionary<long,long>();

                long rdMs = RunAddSequential (rd, group, scale);
                long sdMs = RunAddSequential (sd, group, scale);
                sumRd += rdMs; sumSd += sdMs;

                long rdSMs = RunSeek (rd, group * scale);
                long sdSMs = RunSeek (sd, group * scale);

                long rdEMs = RunKeyIterate (rd);
                long sdEMs = RunKeyIterate (sd);

                Console.WriteLine ($"{rdMs,6};{sdMs,6};{rdSMs,6};{sdSMs,6};{rdEMs,6};{sdEMs,6}");

                sumChange += ((float) sumRd) / (float) sumSd;
            }
            float change = sumChange * 10 + 0.5F;
            Console.WriteLine ($"Add improvement={(100 - (int) change)}%");
        }

        /* Output:

        RankedDictionary Add;SortedDictionary Add;RankedDictionary Seek;SortedDictionary Seek;RankedDictionary Iterate;SortedDictionary Iterate;1000000
          2227;  3050;  1139;  2602;    81;   268
          4576;  7761;  2686;  5237;   180;   505
          6973; 10596;  4175;  8546;   275;   743
          9609; 14780;  5802; 11473;   359;   992
         12460; 19091;  7475; 14319;   454;  1288
         15543; 24756;  8739; 18050;   538;  1525
         17842; 27033; 10466; 20583;   674;  1823
         20769; 31120; 11790; 25496;   738;  2031
         23573; 34830; 13016; 27858;   812;  2270
         24964; 38980; 15260; 30497;   921;  2546

         */
    }
}
