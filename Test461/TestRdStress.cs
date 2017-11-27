using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kaos.Test.Collections
{
    public partial class TestBtree
    {
        [TestMethod]
        public void StressRd_WithLongPermutations()
        {
            for (int order = 5; order < 9; ++order)
            {
                Setup (order);

                for (int w = 1; w <= 500; ++w)
                {
                    for (int m = 0; m < w; m++)
                        dary1.Add (m, m + 1000);
                    for (int m = w - 1; m >= 0; --m)
                        dary1.Remove (m);

                    Assert.AreEqual (0, dary1.Count);
                }
            }
        }


        [TestMethod]
        public void StressRd_RemoveForLongBranchShift()
        {
            Setup (6);

            for (int size = 1; size < 90; ++size)
            {
                for (int i = 1; i <= size; ++i)
                    dary1.Add (i, i+200);

                for (int i = 1; i <= size; ++i)
                {
                    dary1.Remove (i);

                    foreach (var kv in dary1)
                        Assert.AreEqual (kv.Key+200, kv.Value);
#if (! TEST_BCL && DEBUG)
                    dary1.SanityCheck();
#endif
                }
            }
        }


        [TestMethod]
        public void StressRd_RemoveSlidingWindowForCoalesce()
        {
            Setup (5);

            for (int size = 65; size <= 75; ++size)
            {
                for (int a = 1; a <= size; ++a)
                    for (int b = a; b <= size; ++b)
                    {
                        dary1.Clear();
                        for (int i = 1; i <= size; ++i)
                            dary1.Add (i, i + 100);

                        for (int i = a; i <= b; ++i)
                            dary1.Remove (i);

                        foreach (var kv in dary1)
                            Assert.AreEqual (kv.Key+100, kv.Value);

#if (! TEST_BCL && DEBUG)
                        dary1.SanityCheck();
#endif
                    }
            }
        }


        [TestMethod]
        public void StressRd_RemoveSpanForNontrivialCoalesce()
        {
            Setup();

            for (int key = 1; key < 70; ++key)
                dary1.Add (key, key + 100);

            for (int key = 19; key <= 25; ++key)
                dary1.Remove (key);
        }


        [TestMethod]
        public void StressRd_RemoveSpanForBranchBalancing()
        {
            Setup (6);

            for (int key = 1; key <= 46; ++key)
                dary1.Add (key, key + 100);

            for (int key = 1; key <= 46; ++key)
            {
                dary1.Remove (key);

#if (! TEST_BCL && DEBUG)
                dary1.SanityCheck();
#endif
            }
        }


        [TestMethod]
        public void StressRd_AddForSplits()
        {
            Setup (5);

            for (int k = 0; k < 99; k += 8)
                dary1.Add (k, k + 100);

            dary1.Add (20, 1);
            dary1.Add (50, 1);
            dary1.Add (66, 132);
            dary1.Remove (20);
            dary1.Add (38, 147);
            dary1.Add (35, 142);
            dary1.Add (12, 142);
            dary1.Add (10, 147);
            dary1.Add (36, 147);
            dary1.Remove (12);
            dary1.Remove (8);
            dary1.Remove (10);
            dary1.Remove (88);

            dary1.Remove (56);
            dary1.Remove (80);
            dary1.Remove (96);
            dary1.Add (18, 118);
            dary1.Add (11, 111);

#if (! TEST_BCL && DEBUG)
            dary1.SanityCheck();
#endif
        }
    }
}
