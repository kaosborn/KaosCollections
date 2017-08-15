//
// Program: RsChart05B.cs
// Purpose: Show various tree mutation scenarios of SymmetricExceptWith.
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
    class RsChart05B
    {
        static void WriteInfo (Btree<int> tree, bool showStats=false)
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
            for (int i = 1; i <= 12; i+= 1)
            {
                var rs1 = new RankedSet<int>();
                rs1.Capacity = 4;

                foreach (int kk in new int[] { 3, 5, 6, 7, 9, 10, 11 })
                    rs1.Add (kk);
                rs1.Remove (6); rs1.Remove (10);
                WriteInfo (rs1);

                rs1.SymmetricExceptWith (new int[] { i });
                Console.WriteLine ("SymmetricExceptWith " + i + ":");
                WriteInfo (rs1);
                Console.WriteLine ("----");
            }
        }
    }
}
