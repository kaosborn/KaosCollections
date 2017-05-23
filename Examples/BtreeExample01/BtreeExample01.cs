//
// Program: BtreeExample01.cs
// Purpose: Exercise BtreeDictionary.Remove() showing various tree mutation scenarios.
//

using System;
using Kaos.Collections;

namespace ExampleApp
{
    class BtreeExample01
    {
        static void Main()
        {
            // Create a tree of low order to keep the output small:
            var tree = new BtreeDictionary<int, int> (6);

            // Build a 3-level tree:
            for (int i = 2; i <= 92; i += 2)
                tree.Add (i, i + 100);

            // Prepare for examples by thinning the tree:
            for (int i = 2; i <= 22; i += 2)
                tree.Remove (i);
            foreach (int i in new int[] { 34, 38, 44, 46, 48, 54, 56, 60, 66, 68, 74, 76, 78 })
                tree.Remove (i);

#if DEBUG
            Console.WriteLine ("\nA BtreeDictionary of order 6 weighted to the right of the root:");
            Console.WriteLine ("(Order is defined as the maximum number of children of a node.)\n");
            tree.Dump(); tree.SanityCheck();

            Console.WriteLine ("\nCoalesce leaves, balance branches by deleting 24:\n");
            tree.Remove (24);
            tree.Dump(); tree.SanityCheck();

            Console.WriteLine ("\nUpdate a branch key by deleting 32:\n");
            tree.Remove (32);
            tree.Dump(); tree.SanityCheck();

            Console.WriteLine ("\nUpdate the root branch key by deleting 62:\n");
            tree.Remove (62);
            tree.Dump(); tree.SanityCheck();

            Console.WriteLine ("\nCoalesce leaves by deleting 58:\n");
            tree.Remove (58);
            tree.Dump(); tree.SanityCheck();

            Console.WriteLine ("\nDelete rightmost branches by deleting 92:");
            Console.WriteLine ("(Any rightmost node may contain as few as 1 element.)\n");
            tree.Remove (92);
            tree.Dump(); tree.SanityCheck();

            Console.WriteLine ("\nCoalesce leaf, coalesce branches, prune root by deleting 36:\n");
            tree.Remove (36);
            tree.Dump(); tree.SanityCheck();
#endif
        }
    }
}
