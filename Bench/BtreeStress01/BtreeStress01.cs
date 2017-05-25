//
// Program: BtreeStress01.cs
// Purpose: Stress BtreeDictionary with many permutation adds.
//

using System;
using Kaos.Collections;
using Kaos.Combinatorics;

namespace BenchApp
{
    class BtreeStress01
    {
        static void Main()
        {
            var tree = new BtreeDictionary<int, int>(5);

            for (int w = 1; w < Permutation.MaxChoices; ++w)
            {
                var pt0 = new Permutation (w);
                Console.WriteLine (pt0);
                foreach (Permutation pt in pt0.GetRows())
                {
                    for (int m = 0; m < pt.Choices; ++m)
                        tree.Add (pt[m], pt[m] + 100);

                    tree.Clear();
                }
            }
        }
    }
}
