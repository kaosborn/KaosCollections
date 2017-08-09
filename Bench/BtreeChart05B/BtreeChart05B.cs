﻿//
// Program: BtreeChart05B.cs
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
    class BtreeChart05B
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
            tree = new RankedDictionary<int,int>();
            tree.Capacity = 5;

            Console.WriteLine ("Create sequentially loaded tree of order 5:");
            for (int i = 2; i <= 66; i += 2)
                tree.Add (i, i + 100);
            WriteInfo();

            Console.WriteLine ("Thin the tree by removing several keys:");
            foreach (int i in new int[] { 4,6,14,16,18,20,22,24,26,30,36,38,46 })
                tree.Remove (i);
            WriteInfo();

            Console.WriteLine ("Coalesce leaves, balance branches by removing 12:");
            tree.Remove (12);
            WriteInfo();

            Console.WriteLine ("Change a branch by removing 10:");
            tree.Remove (10);
            WriteInfo();

            Console.WriteLine ("Change the root by removing 50:");
            tree.Remove (50);
            WriteInfo();

            Console.WriteLine ("Coalesce leaves by removing 40:");
            tree.Remove (40);
            WriteInfo();

            Console.WriteLine ("Prune rightmost leaf by removing 66:");
            tree.Remove (66);
            WriteInfo();

            Console.WriteLine ("Coalesce leaves and branches, prune root by removing 8:");
            tree.Remove (8);
            WriteInfo();
        }

        /* Debug output:

        Create sequentially loaded tree of order 5:

        B0: 34
        B1: 10,18,26 | 42,50,58,66
        L2: 2,4,6,8|10,12,14,16|18,20,22,24|26,28,30,32 | 34,36,38,40|42,44,46,48|50,52,54,56|58,60,62,64|66

        Thin the tree by removing several keys:

        B0: 34
        B1: 10,28 | 42,50,58,66
        L2: 2,8|10,12|28,32 | 34,40|42,44,48|50,52,54,56|58,60,62,64|66

        Coalesce leaves, balance branches by removing 12:

        B0: 50
        B1: 10,34,42 | 58,66
        L2: 2,8|10,28,32|34,40|42,44,48 | 50,52,54,56|58,60,62,64|66

        Change a branch by removing 10:

        B0: 50
        B1: 28,34,42 | 58,66
        L2: 2,8|28,32|34,40|42,44,48 | 50,52,54,56|58,60,62,64|66

        Change the root by removing 50:

        B0: 52
        B1: 28,34,42 | 58,66
        L2: 2,8|28,32|34,40|42,44,48 | 52,54,56|58,60,62,64|66

        Coalesce leaves by removing 40:

        B0: 52
        B1: 28,34 | 58,66
        L2: 2,8|28,32|34,42,44,48 | 52,54,56|58,60,62,64|66

        Prune rightmost leaf by removing 66:

        B0: 52
        B1: 28,34 | 58
        L2: 2,8|28,32|34,42,44,48 | 52,54,56|58,60,62,64

        Coalesce leaves and branches, prune root by removing 8:

        B0: 34,52,58
        L1: 2,28,32|34,42,44,48|52,54,56|58,60,62,64

        */
    }
}
