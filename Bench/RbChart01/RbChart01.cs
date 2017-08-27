//
// Program: RbChart01.cs
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
    class RbChart01
    {
        static RankedBag<int> bag;

        static void WriteInfo (bool showStats=false)
        {
            Console.WriteLine();
#if DEBUG
            foreach (var lx in bag.GenerateTreeText())
                Console.WriteLine (lx);

            bag.SanityCheck();
            Console.WriteLine();

            if (showStats)
            {
                Console.WriteLine (bag.GetTreeStatsText());
                Console.WriteLine();
            }
#endif
        }

        static void Main()
        {
            bag = new RankedBag<int>() { Capacity = 4 };

            Console.WriteLine ("Create tree of order 4:");
            for (int i = 2; i <= 12; i+=2)
                bag.Add (i);
            for (int i = 2; i <= 12; i+=2)
                bag.Add (i);
            for (int i = 2; i <= 12; i+=2)
                bag.Add (i);
            for (int i = 2; i <= 12; i+=2)
                bag.Add (i);
            WriteInfo();

            var a1 = new int[] { 4, 6, 6, 6 };
            Console.WriteLine ("\nRemoveAll " + String.Join (",", a1) + ":");
            bag.RemoveAll (a1);
            WriteInfo();

            Console.WriteLine ("\nRemoveAll 8:");
            bag.RemoveAll (new int[] { 8 });
            WriteInfo();

            Console.WriteLine ("\nRemoveAll 8:");
            bag.RemoveAll (new int[] { 8 });
            WriteInfo();

            Console.WriteLine ("GetCount(12) = " + bag.GetCount (12));

            var a2 = new int[] { 1, 2, 2, 5, 10, 21 };
            Console.WriteLine ("\nRetain " + String.Join (",", a2) + ":");
            bag.RetainAll (a2);
            WriteInfo();
        }

        /* Output:

        Create tree of order 4:

        B0: 4,8,10
        B1: 2,4 | 6,6 | 8,10 | 12
        L2: 2,2|2,2|4,4 | 4,4|6,6|6,6 | 8,8|8,8|10,10 | 10,10,12|12,12,12


        RemoveAll 4,6,6,6:

        B0: 6,10
        B1: 2,4 | 8,10 | 12
        L2: 2,2|2,2|4,4,4 | 6,8,8|8,8|10,10 | 10,10,12|12,12,12


        RemoveAll 8:

        B0: 6,10
        B1: 2,4 | 8,10 | 12
        L2: 2,2|2,2|4,4,4 | 6,8|8,8|10,10 | 10,10,12|12,12,12


        RemoveAll 8:

        B0: 6
        B1: 2,4 | 10,10,12
        L2: 2,2|2,2|4,4,4 | 6,8,8|10,10|10,10,12|12,12,12

        GetCount(12) = 4

        Retain 1,2,2,5,10,21:

        B0: 10
        L1: 2,2|10

        */
    }
}
