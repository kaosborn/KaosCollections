//
// Program: RsChart04A.cs
// Purpose: Show various tree mutation scenarios.
//
// Usage notes:
// • To include diagnostics and charts, run Debug build.
//

using System;
using System.Reflection;
using Kaos.Collections;
[assembly: AssemblyVersion ("0.1.0.0")]

namespace ChartApp
{
    class RsChart04A
    {
        static RankedSet<int> set;

        static void WriteInfo (bool showStats=false)
        {
            Console.WriteLine();
#if DEBUG
            foreach (var lx in set.GenerateTreeText())
                Console.WriteLine (lx);

            set.SanityCheck();
            Console.WriteLine();

            if (showStats)
            {
                Console.WriteLine (set.GetTreeStatsText());
                Console.WriteLine();
            }
#endif
        }


        static void Main()
        {
            set = new RankedSet<int>() { Capacity=4 };

            Console.WriteLine ("Empty tree is a single leaf:");
            WriteInfo (true);

            Console.WriteLine ("Create sequentially loaded tree of order 4:");
            for (int i = 2; i <= 24; i+=2)
                set.Add (i);
            WriteInfo (true);

            Console.WriteLine ("Cascade split by adding 17:");
                set.Add (17);
            WriteInfo();

            Console.WriteLine ("Split a leaf by adding 3:");
                set.Add (3);
            WriteInfo();

            Console.WriteLine ("Create non-rightmost branch with 2 children by adding 9:");
                set.Add (9);
            WriteInfo();

            Console.WriteLine ("Cascade coalesce by removing 2:");
                set.Remove (2);
            WriteInfo();
        }

        /* Debug output:

        Empty tree is a single leaf:

        L0:

        --- height = 1, leaf fill = 0%

        Create sequentially loaded tree of order 4:

        B0: 8,14,20
        L1: 2,4,6|8,10,12|14,16,18|20,22,24

        --- height = 2, branch fill = 100%, leaf fill = 100%

        Cascade split by adding 17:

        B0: 17
        B1: 8,14 | 20
        L2: 2,4,6|8,10,12|14,16 | 17,18|20,22,24

        Split a leaf by adding 3:

        B0: 17
        B1: 4,8,14 | 20
        L2: 2,3|4,6|8,10,12|14,16 | 17,18|20,22,24

        Create non-rightmost branch with 2 children by adding 9:

        B0: 10,17
        B1: 4,8 | 14 | 20
        L2: 2,3|4,6|8,9 | 10,12|14,16 | 17,18|20,22,24

        Cascade coalesce by removing 2:

        B0: 17
        B1: 8,10,14 | 20
        L2: 3,4,6|8,9|10,12|14,16 | 17,18|20,22,24

        */
    }
}
