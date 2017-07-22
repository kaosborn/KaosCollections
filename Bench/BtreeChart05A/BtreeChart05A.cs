//
// Program: BtreeChart05A.cs
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
    class BtreeChart05A
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
            tree = new RankedDictionary<int,int> (5);

            Console.WriteLine ("Create sequentially loaded tree of order 5:");
            for (int i = 2; i <= 10; i += 2)
                tree.Add (i, i + 100);
            WriteInfo (true);

            Console.WriteLine ("Add 12-40:");
            for (int i = 12; i <= 40; i += 2)
                tree.Add (i, i + 100);
            WriteInfo (true);

            Console.WriteLine ("Add 42:");
            tree.Add (42, 142);
            WriteInfo (true);
        }

        /* Debug output:

        Create sequentially loaded tree of order 5:

        B0: 10
        L1: 2,4,6,8|10

        --- height = 2, branch fill = 25%, leaf fill = 62%

        Add 12-40:

        B0: 10,18,26,34
        L1: 2,4,6,8|10,12,14,16|18,20,22,24|26,28,30,32|34,36,38,40

        --- height = 2, branch fill = 100%, leaf fill = 100%

        Add 42:

        B0: 34
        B1: 10,18,26 | 42
        L2: 2,4,6,8|10,12,14,16|18,20,22,24|26,28,30,32 | 34,36,38,40|42

        --- height = 3, branch fill = 41%, leaf fill = 87%

        */
    }
}
