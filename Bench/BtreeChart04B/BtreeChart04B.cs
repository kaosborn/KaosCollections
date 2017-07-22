//
// Program: BtreeChart04B.cs
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
    class BtreeChart04B
    {
        static RankedDictionary<int,int> tree;

        static void WriteInfo (bool showStats=false)
        {
            Console.WriteLine();
#if DEBUG
            foreach (var lx in tree.GenerateTreeText())
                Console.WriteLine (lx);

            tree.SanityCheck();
            Console.WriteLine();

            if (showStats)
            {
                Console.WriteLine (tree.GetTreeStatsText());
                Console.WriteLine();
            }
#endif
        }

        static void Main()
        {
            tree = new RankedDictionary<int,int> (4);

            Console.WriteLine ("Create sequentially loaded tree of order 4:");
            for (int i = 2; i <= 44; i+=2)
                tree.Add (i, i + 100);
            WriteInfo();

            Console.WriteLine ("Add 1,21:");
            tree.Add (1, 101);
            tree.Add (21, 221);
            WriteInfo();

            Console.WriteLine ("Add 9:");
            tree.Add (9, 111);
            WriteInfo();

            Console.WriteLine ("Add 27:");
                tree.Add (27, 270);
            WriteInfo();

            Console.WriteLine ("Remove 44:");
            tree.Remove (44);
            WriteInfo();

            Console.WriteLine ("Remove 40,42:");
            tree.Remove (40);
            tree.Remove (42);
            WriteInfo();

            Console.WriteLine ("Remove 38:");
            tree.Remove (38);
            WriteInfo();

            Console.WriteLine ("Remove 34,36:");
            tree.Remove (34);
            tree.Remove (36);
            WriteInfo();

            Console.WriteLine ("Remove 32:");
            tree.Remove (32);
            WriteInfo();

            Console.WriteLine ("Remove 28:");
            tree.Remove (28);
            WriteInfo();

            Console.WriteLine ("Seek 30:");
            var isOk = tree.TryGetValue (30, out int result30);

            Console.WriteLine();
            Console.WriteLine ("Result = " + result30);
            Console.WriteLine();

            Console.WriteLine ("Remove 30:");
            tree.Remove (30);
            WriteInfo();
        }

        /* Debug output:

        Create sequentially loaded tree of order 4:

        B0: 20,38
        B1: 8,14 | 26,32 | 44
        L2: 2,4,6|8,10,12|14,16,18 | 20,22,24|26,28,30|32,34,36 | 38,40,42|44

        Add 1,21:

        B0: 20,38
        B1: 4,8,14 | 22,26,32 | 44
        L2: 1,2|4,6|8,10,12|14,16,18 | 20,21|22,24|26,28,30|32,34,36 | 38,40,42|44

        Add 9:

        B0: 10,20,38
        B1: 4,8 | 14 | 22,26,32 | 44
        L2: 1,2|4,6|8,9 | 10,12|14,16,18 | 20,21|22,24|26,28,30|32,34,36 | 38,40,42|44

        Add 27:

        B0: 28
        B1: 10,20 | 38
        B2: 4,8 | 14 | 22,26 | 32 | 44
        L3: 1,2|4,6|8,9 | 10,12|14,16,18 | 20,21|22,24|26,27 | 28,30|32,34,36 | 38,40,42|44

        Remove 44:

        B0: 28
        B1: 10,20 | 38
        B2: 4,8 | 14 | 22,26 | 32 |
        L3: 1,2|4,6|8,9 | 10,12|14,16,18 | 20,21|22,24|26,27 | 28,30|32,34,36 | 38,40,42

        Remove 40,42:

        B0: 28
        B1: 10,20 | 38
        B2: 4,8 | 14 | 22,26 | 32 |
        L3: 1,2|4,6|8,9 | 10,12|14,16,18 | 20,21|22,24|26,27 | 28,30|32,34,36 | 38

        Remove 38:

        B0: 28
        B1: 10,20 |
        B2: 4,8 | 14 | 22,26 | 32
        L3: 1,2|4,6|8,9 | 10,12|14,16,18 | 20,21|22,24|26,27 | 28,30|32,34,36

        Remove 34,36:

        B0: 28
        B1: 10,20 |
        B2: 4,8 | 14 | 22,26 | 32
        L3: 1,2|4,6|8,9 | 10,12|14,16,18 | 20,21|22,24|26,27 | 28,30|32

        Remove 32:

        B0: 28
        B1: 10,20 |
        B2: 4,8 | 14 | 22,26 |
        L3: 1,2|4,6|8,9 | 10,12|14,16,18 | 20,21|22,24|26,27 | 28,30

        Remove 28:

        B0: 30
        B1: 10,20 |
        B2: 4,8 | 14 | 22,26 |
        L3: 1,2|4,6|8,9 | 10,12|14,16,18 | 20,21|22,24|26,27 | 30

        Seek 30:

        Result = 130

        Remove 30:

        B0: 10,20
        B1: 4,8 | 14 | 22,26
        L2: 1,2|4,6|8,9 | 10,12|14,16,18 | 20,21|22,24|26,27

        */
    }
}
