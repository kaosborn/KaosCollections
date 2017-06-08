//
// Program: BtreeExample01.cs
// Purpose: Exercise BtreeDictionary.Remove() showing various tree mutation scenarios.
//
// Usage notes:
// • To include diagnostic results, run Debug build.
//

using System;
using Kaos.Collections;

namespace ExampleApp
{
    class BtreeExample01
    {
        static BtreeDictionary<int,int> tree;

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


        static void Main()
        {
            int order = 6;
            // Create a tree of low order to keep the output small:
            tree = new BtreeDictionary<int, int> (order);

            // Build a 3-level tree:
            for (int i = 2; i <= 92; i += 2)
                tree.Add (i, i + 100);

            // Prepare for examples by thinning the tree:
            for (int i = 2; i <= 22; i += 2)
                tree.Remove (i);
            foreach (int i in new int[] { 34, 38, 44, 46, 48, 54, 56, 60, 66, 68, 74, 76, 78 })
                tree.Remove (i);

            Console.WriteLine ("A BtreeDictionary weighted to the right of the root.  Order = " + order + ":");
            WriteInfo();

            Console.WriteLine ("Coalesce leaves, balance branches by deleting 24:");
            tree.Remove (24);
            WriteInfo();

            Console.WriteLine ("Update a branch key by deleting 32:");
            tree.Remove (32);
            WriteInfo();

            Console.WriteLine ("Update the root branch key by deleting 62:");
            tree.Remove (62);
            WriteInfo();

            Console.WriteLine ("Coalesce leaves by deleting 58:");
            tree.Remove (58);
            WriteInfo();

            Console.WriteLine ("Delete rightmost branches by deleting 92:");
            Console.WriteLine ("(Any rightmost node may contain as few as 1 element.)");
            tree.Remove (92);
            WriteInfo();

            Console.WriteLine ("Coalesce leaf, coalesce branches, prune root by deleting 36:");
            tree.Remove (36);
            WriteInfo();
#if DEBUG
            Console.Write ("---- height = " + tree.GetHeight());
            Console.Write (", branch fill = " + tree.BranchSlotsUsed * 100 / tree.BranchSlotCount + "%");
            Console.WriteLine (", leaf fill = " + tree.LeafSlotsUsed * 100 / tree.LeafSlotCount + "% ----");
#endif
        }

        /* Output:

        A BtreeDictionary weighted to the right of the root.  Order = 6:

        L0: 42
        L1: 28,32 | 52,62,72,82,92
        L2: 24,26|28,30|32,36,40 | 42,50|52,58|62,64,70|72,80|82,84,86,88,90|92

        Coalesce leaves, balance branches by deleting 24:

        L0: 62
        L1: 32,42,52 | 72,82,92
        L2: 26,28,30|32,36,40|42,50|52,58 | 62,64,70|72,80|82,84,86,88,90|92

        Update a branch key by deleting 32:

        L0: 62
        L1: 36,42,52 | 72,82,92
        L2: 26,28,30|36,40|42,50|52,58 | 62,64,70|72,80|82,84,86,88,90|92

        Update the root branch key by deleting 62:

        L0: 64
        L1: 36,42,52 | 72,82,92
        L2: 26,28,30|36,40|42,50|52,58 | 64,70|72,80|82,84,86,88,90|92

        Coalesce leaves by deleting 58:

        L0: 72
        L1: 36,42,52 | 82,92
        L2: 26,28,30|36,40|42,50|52,64,70 | 72,80|82,84,86,88,90|92

        Delete rightmost branches by deleting 92:
        (Any rightmost node may contain as few as 1 element.)

        L0: 72
        L1: 36,42,52 | 82
        L2: 26,28,30|36,40|42,50|52,64,70 | 72,80|82,84,86,88,90

        Coalesce leaf, coalesce branches, prune root by deleting 36:

        L0: 40,52,72,82
        L1: 26,28,30|40,42,50|52,64,70|72,80|82,84,86,88,90

        ---- height = 2, branch fill = 80%, leaf fill = 64% ----

        */
    }
}
