//
// Program: RbChart02.cs
// Purpose: Show various tree mutation scenarios.
//
// Usage notes:
// • To include diagnostics and charts, run Debug build.
//

using System;
using System.Reflection;
using Kaos.Collections;

namespace ChartApp
{
    class RbChart02
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
            bag = new RankedBag<int>() { Capacity = 6 };

            Console.WriteLine ("Using Add (x), create tree of order 6:");
            for (int ii = 1; ii <= 6; ++ii) bag.Add (3);
            for (int ii = 1; ii <= 13; ++ii) bag.Add (5);
            for (int ii = 1; ii <= 7; ++ii) bag.Add (7);
            for (int ii = 1; ii <= 5; ++ii) bag.Add (9);
            WriteInfo();

            Console.WriteLine ("Remove (5, 10):");
            bag.Remove (5, 13);
            WriteInfo();

            bag.Clear();
            Console.WriteLine ("Using Add (x, n), create tree of order 6:");
            bag.Add (3, 6);
            bag.Add (5, 13);
            bag.Add (7, 7);
            bag.Add (9, 5);
            WriteInfo();

        }

        /* Output:

        Using Add (x), create tree of order 6:

        B0: 7
        B1: 3,5,5,7 | 9
        L2: 3,3,3,3,3|3,5,5,5,5|5,5,5,5,5|5,5,5,5,7|7,7,7,7,7 | 7,9,9,9,9|9

        Remove (5, 10):

        B0: 3,7,7,9
        L1: 3,3,3,3,3|3,7,7,7|7,7,7|7,9,9,9,9|9

        Using Add (x, n), create tree of order 6:

        B0: 5
        B1: 3,5,5,5 | 7,7,9
        L2: 3,3,3|3,3,3|5,5,5|5,5,5,5|5,5,5 | 5,5,5|7,7,7,7|7,7,7|9,9,9,9,9

        */
    }
}
