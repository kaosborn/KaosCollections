//
// Program: BtreeChart04A.cs
// Purpose: Show various tree mutation scenarios.
//
// Usage notes:
// • To include diagnostics and charts, run Debug build.
//

using System;
using Kaos.Collections;

namespace ChartApp
{
    class BtreeChart04A
    {
        static BtreeDictionary<int,int> tree = new BtreeDictionary<int,int> (4);

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
            Console.WriteLine ("Empty tree is a single leaf:");
            WriteInfo (true);

            Console.WriteLine ("Create sequentially loaded tree of order 4:");
            for (int i = 2; i <= 24; i+=2)
                tree.Add (i, i + 100);
            WriteInfo (true);

            Console.WriteLine ("Cascade split by adding 17:");
                tree.Add (17, 117);
            WriteInfo();

            Console.WriteLine ("Split a leaf by adding 3:");
                tree.Add (3, 103);
            WriteInfo();

            Console.WriteLine ("Create non-rightmost branch with 2 children by adding 9:");
                tree.Add (9, 109);
            WriteInfo();

            Console.WriteLine ("Cascade coalesce by removing 2:");
                tree.Remove (2);
            WriteInfo();
        }

        /* Debug output:

        Empty tree is a single leaf:

        L0:

        --- height = 1, leaf fill = 0%

        Sequentially loaded tree of order 4:

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
