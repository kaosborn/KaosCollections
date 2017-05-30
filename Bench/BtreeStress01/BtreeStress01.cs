//
// Program: BtreeStress01.cs
// Purpose: Brute force testing of insertions and deletions using permutations.
//

using System;
using System.Threading;
using Kaos.Collections;
using Kaos.Combinatorics;

namespace KwDataBench
{
    class BtreeBench03
    {
        static void Main()
        {
            var tree = new BtreeDictionary<int, int> (5);
            

            for (int w = 1; w < 21; ++w)
            {
                foreach (Permutation permAdd in new Permutation (w).GetRows())
                {
                    foreach (Permutation permDel in permAdd.GetRows())
                    {
                        for (int m = 0; m < permAdd.Picks; ++m)
                        {
                            tree.Add (permAdd[m], permAdd[m] + 100);

#if DEBUG
                            if (permDel.Rank == 0)
                            {
                                try
                                {
                                    tree.SanityCheck();
                                }
                                catch (DataMisalignedException ex)
                                {
                                    Console.WriteLine ("Insanity found: {0}", ex.Message);
                                    Console.WriteLine ("Order={0} add.Rank={1} m={2}", w, permAdd.Rank, m);
                                    throw;
                                }
                            }
#endif
                        }
                        //VERBOSE Console.WriteLine ("Added {0}", permAdd);

                        for (int m = 0; m < permDel.Picks; ++m)
                        {
                            tree.Remove (permDel[m]);

#if DEBUG
                            try
                            {
                                tree.SanityCheck();
                            }
                            catch (DataMisalignedException ex)
                            {
                                Console.WriteLine ("Insanity found: {0}", ex.Message);
                                Console.WriteLine ("Order={0} add.Rank={1} del.Rank={2} m={3}",
                                                   w, permAdd.Rank, permDel.Rank, m);
                                throw;
                            }
#endif
                        }
                        //VERBOSE Console.WriteLine ("  Del {0}", permDel);

#if DEBUG
                        if (tree.Count != 0)
                            throw new DataMisalignedException ("Count should be zero");
#endif
                        tree.Clear();
                    }
                }

                Console.WriteLine ("{2} - Completed Order {0} = {1}", w, new Permutation (w), DateTime.Now);
                Thread.Sleep (250);
            }
        }
    }
}
