//
// Program: BtreeChart06A.cs
// Purpose: Show various tree mutation scenarios.
//
// Usage notes:
// • To include diagnostics and charts, run Debug build.
//

using System;
using Kaos.Collections;

namespace ChartApp
{
    class BtreeChart06A
    {
        static BtreeDictionary<int,int> tree = new BtreeDictionary<int,int> (6);

        static void WriteInfo()
        {
            Console.WriteLine();
#if DEBUG
            foreach (var lx in tree.GenerateTreeText())
                Console.WriteLine (lx);

            tree.SanityCheck();
            Console.WriteLine();
#endif
        }

        static void WriteStats (BtreeDictionary<int,int> tree)
        {
#if DEBUG
            Console.Write ("--- height = " + tree.GetHeight());
            Console.Write (", branch fill = " + tree.BranchSlotsUsed * 100 / tree.BranchSlotCount + "%");
            Console.WriteLine (", leaf fill = " + tree.LeafSlotsUsed * 100 / tree.LeafSlotCount + "%");
#endif
        }

        static void Main()
        {
            Console.WriteLine ("Fill all nodes by sequential loading tree of order 6:");
            for (int i = 2; i <= 60; i+=2)
                tree.Add (i, i + 100);
            WriteInfo();

            Console.WriteLine ("Create rightmost nodes with 1 key by appending 62:");
                tree.Add (62, 162);
            WriteInfo();

            Console.WriteLine ("Split leaf by adding 5:");
            tree.Add (5, 105);
            WriteInfo();

            Console.WriteLine ("Split leaf, cascade split branches by adding 25:");
            tree.Add (25, 125);
            WriteInfo();

            Console.WriteLine ("Balance leaves, replace 2 pivots by removing 6:");
            tree.Remove (6);
            WriteInfo();

            Console.WriteLine ("Coalesce leaves, coalesce branches by removing 4:");
            tree.Remove (4);
            WriteInfo();

            Console.WriteLine ("Prune leaf by removing 62:");
            tree.Remove (62);
            WriteInfo();

            Console.WriteLine ("Remove 54-60:");
            tree.Remove (54);
            tree.Remove (56);
            tree.Remove (58);
            tree.Remove (60);
            WriteInfo();

            Console.WriteLine ("Prune leaf and branch by removing 52:");
            tree.Remove (52);
            WriteInfo();
        }

        /* Debug output:


        */
    }
}
