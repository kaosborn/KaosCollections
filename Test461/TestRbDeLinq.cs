//
// File: TestRbDeLinq.cs
// Purpose: Test LINQ emulation.
//

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SLE=System.Linq.Enumerable;
#if TEST_BCL
using System.Linq;
#endif
using Kaos.Collections;

namespace Kaos.Test.Collections
{
    public partial class TestBtree
    {
        #region Test methods (LINQ emulation)

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRbq_ElementAtA_ArgumentOutOfRange()
        {
            var rb = new RankedBag<int>();
#if TEST_BCL
            var zz = Enumerable.ElementAt (rb, -1);
#else
            var zz = rb.ElementAt (-1);
#endif
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRbq_ElementAtB_ArgumentOutOfRange()
        {
            var rb = new RankedBag<int>();
#if TEST_BCL
            var zz = Enumerable.ElementAt (rb, 0);
#else
            var zz = rb.ElementAt (0);
#endif
        }


        [TestMethod]
        public void UnitRbq_ElementAt()
        {
            var rb = new RankedBag<int> { Capacity=5 };
            int n = 200;

            for (int ii = 0; ii <= n; ++ii)
                rb.Add (ii/2);

            for (int ii = 0; ii <= n; ++ii)
            {
#if TEST_BCL
                Assert.AreEqual (ii/2, Enumerable.ElementAt (rb, ii));
#else
                Assert.AreEqual (ii/2, rb.ElementAt (ii));
#endif
            }
        }


        [TestMethod]
        public void UnitRbq_ElementAtOrDefault()
        {
            var rb = new RankedBag<int>();
#if TEST_BCL
            int keyN = Enumerable.ElementAtOrDefault (rb, -1);
            int key0 = Enumerable.ElementAtOrDefault (rb, 0);
#else
            int keyN = rb.ElementAtOrDefault (-1);
            int key0 = rb.ElementAtOrDefault (0);
#endif
            Assert.AreEqual (default (int), keyN);
            Assert.AreEqual (default (int), key0);

            rb.Add (9);
#if TEST_BCL
            int keyZ = Enumerable.ElementAtOrDefault (rb, 0);
            int key1 = Enumerable.ElementAtOrDefault (rb, 1);
#else
            int keyZ = rb.ElementAtOrDefault (0);
            int key1 = rb.ElementAtOrDefault (1);
#endif
            Assert.AreEqual (9, keyZ);
            Assert.AreEqual (default (int), key1);
        }


        [TestMethod]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRbq_First_InvalidOperation()
        {
            var rb = new RankedBag<int>();
#if TEST_BCL
            var zz = Enumerable.First (rb);
#else
            var zz = rb.First();
#endif
        }


        [TestMethod]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRbq_Last_InvalidOperation()
        {
            var rb = new RankedBag<int>();
#if TEST_BCL
            var zz = Enumerable.Last (rb);
#else
            var zz = rb.Last();
#endif
        }

        [TestMethod]
        public void UnitRbq_FirstLast()
        {
            var rb = new RankedBag<int> { Capacity=4 };
            int n = 99;
            for (int ii = n; ii >= 1; --ii) rb.Add (ii);
#if TEST_BCL
            Assert.AreEqual (1, Enumerable.First (rb));
            Assert.AreEqual (n, Enumerable.Last (rb));
#else
            Assert.AreEqual (1, rb.First());
            Assert.AreEqual (n, rb.Last());
#endif
        }


        [TestMethod]
        public void UnitRbq_Skip()
        {
            var rb = new RankedBag<int> { Capacity=5 };

            Assert.AreEqual (0, SLE.Count (rb.Skip (0)));
            Assert.AreEqual (0, SLE.Count (rb.Skip (-1)));
            Assert.AreEqual (0, SLE.Count (rb.Skip (1)));

            rb.Add (1);
            rb.Add (2);

            Assert.IsTrue (SLE.SequenceEqual (new int[] { 1,2 }, rb.Skip (-1)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { 1,2 }, rb.Skip (0)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { 2 }, rb.Skip (1)));
            Assert.AreEqual (0, SLE.Count (rb.Skip (2)));
            Assert.AreEqual (0, SLE.Count (rb.Skip (3)));

            Assert.IsTrue (SLE.SequenceEqual (new int[] { 1,2 }, rb.Skip(0).Skip (-1)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { 1,2 }, rb.Skip(0).Skip (0)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { 2 }, rb.Skip(0).Skip (1)));
            Assert.AreEqual (0, SLE.Count (rb.Skip(0).Skip (3)));

            Assert.IsTrue (SLE.SequenceEqual (new int[] { 2,1 }, rb.Reverse().Skip (-1)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { 2,1 }, rb.Reverse().Skip (0)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { 1 },   rb.Reverse().Skip (1)));
            Assert.AreEqual (0, SLE.Count (rb.Reverse().Skip (2)));
            Assert.AreEqual (0, SLE.Count (rb.Reverse().Skip (3)));
        }

        [TestMethod]
        public void StressRbq_SkipF()
        {
            var rb = new RankedBag<int> { Capacity=5 };
            int n = 25;

            for (int ix = 0; ix < n; ++ix)
                rb.Add (n + ix);

            for (int s1 = 0; s1 <= n; ++s1)
                for (int s2 = 0; s2 <= n-s1; ++s2)
                {
                    int e0 = n + s1+s2;
                    foreach (var a0 in rb.Skip (s1).Skip (s2))
                    {
                        Assert.AreEqual (e0, a0);
                        ++e0;
                    }
                    Assert.AreEqual (n + n, e0);
                }
        }

        [TestMethod]
        public void StressRbq_SkipR()
        {
            var rb = new RankedBag<int> { Capacity=5 };
            int n = 25;

            for (int ix = 0; ix < n; ++ix)
                rb.Add (n + ix);

            for (int s1 = 0; s1 <= n; ++s1)
            {
                int e0 = n + n - s1;
                foreach (var a0 in rb.Reverse().Skip (s1))
                {
                    --e0;
                    Assert.AreEqual (e0, a0);
                }
                Assert.AreEqual (n, e0);
            }
        }


        [TestMethod]
        public void UnitRbq_SkipWhile2Ctor()
        {
            var rb = new RankedBag<int> { Capacity=4 };

            Assert.AreEqual (0, SLE.Count (rb.SkipWhile (x => false)));
            Assert.AreEqual (0, SLE.Count (rb.SkipWhile (x => true)));

            rb.Add (-1);

            Assert.AreEqual (0, SLE.Count (rb.SkipWhile (x => true)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { -1 }, rb.SkipWhile (x => false)));

            rb.Add (-2);
            rb.Add (-3);

            Assert.IsTrue (SLE.SequenceEqual (new int[] { -2,-1 }, rb.SkipWhile (x => x%2!=0)));
        }

        [TestMethod]
        public void UnitRbq_SkipWhile2F()
        {
            var rb = new RankedBag<int> { Capacity=4 };

            Assert.AreEqual (0, SLE.Count (rb.Skip(0).SkipWhile (x => false)));
            Assert.AreEqual (0, SLE.Count (rb.Skip(0).SkipWhile (x => true)));

            rb.Add (-1);

            Assert.AreEqual (0, SLE.Count (rb.Skip(0).SkipWhile (x => true)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { -1 }, rb.Skip(0).SkipWhile (x => false)));

            rb.Add (-2);
            rb.Add (-3);

            Assert.IsTrue (SLE.SequenceEqual (new int[] { -2,-1 }, rb.Skip(0).SkipWhile (x => x%2!=0)));
        }

        [TestMethod]
        public void UnitRbq_SkipWhile2R()
        {
            var rb = new RankedBag<int> { Capacity=4 };

            Assert.AreEqual (0, SLE.Count (rb.Reverse().SkipWhile (x => false)));
            Assert.AreEqual (0, SLE.Count (rb.Reverse().SkipWhile (x => true)));

            rb.Add (-1);

            Assert.AreEqual (0, SLE.Count (rb.Reverse().SkipWhile (x => true)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { -1 }, rb.Reverse().SkipWhile (x => false)));

            rb.Add (-2);
            rb.Add (-3);

            Assert.IsTrue (SLE.SequenceEqual (new int[] { -2,-3 }, rb.Reverse().SkipWhile (x => x%2!=0)));
        }

        [TestMethod]
        public void UnitRbq_SkipWhile3Ctor()
        {
            var rb = new RankedBag<int> { Capacity=5 };

            Assert.AreEqual (0, SLE.Count (rb.SkipWhile ((x,i) => false)));
            Assert.AreEqual (0, SLE.Count (rb.SkipWhile ((x,i) => true)));

            rb.Add (1);

            Assert.AreEqual (0, SLE.Count (rb.SkipWhile ((x,i) => true)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { 1 }, rb.SkipWhile ((x,i) => false)));

            rb.Add (2);
            rb.Add (3);
            rb.Add (4);

            Assert.IsTrue (SLE.SequenceEqual (new int[] { 4 }, rb.SkipWhile ((x,i) => x%2!=0 || i<3)));
        }

        [TestMethod]
        public void UnitRbq_SkipWhile3F()
        {
            var rb = new RankedBag<int> { Capacity=5 };

            Assert.AreEqual (0, SLE.Count (rb.Skip(0).SkipWhile ((x,i) => false)));
            Assert.AreEqual (0, SLE.Count (rb.Skip(0).SkipWhile ((x,i) => true)));
            Assert.AreEqual (0, SLE.Count (rb.SkipWhile ((x,i) => true).SkipWhile ((x,i) => true)));

            rb.Add (1);

            Assert.AreEqual (0, SLE.Count (rb.Skip(0).SkipWhile ((x,i) => true)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { 1 }, rb.Skip(0).SkipWhile ((x,i) => false)));

            rb.Add (2);
            rb.Add (3);

            Assert.IsTrue (SLE.SequenceEqual (new int[] { 2,3 }, rb.Skip(0).SkipWhile ((x,i) => x%2!=0)));

            for (int i = 4; i < 50; ++i)
                rb.Add (i);

            Assert.IsTrue (SLE.SequenceEqual (new int[] { 48,49 }, rb.Skip(30).SkipWhile ((x,i) => x%3!=0 || i<15)));
        }

        [TestMethod]
        public void UnitRbq_SkipWhile3R()
        {
            var rb = new RankedBag<int> { Capacity=5 };

            Assert.AreEqual (0, SLE.Count (rb.Reverse().SkipWhile ((x,i) => false)));
            Assert.AreEqual (0, SLE.Count (rb.Reverse().SkipWhile ((x,i) => true)));
            Assert.AreEqual (0, SLE.Count (rb.Reverse().SkipWhile ((x,i) => true).SkipWhile ((x,i) => true)));

            rb.Add (1);

            Assert.AreEqual (0, SLE.Count (rb.Reverse ().SkipWhile ((x,i) => true)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { 1 }, rb.Reverse().SkipWhile ((x,i) => false)));

            rb.Add (2);
            rb.Add (3);

            Assert.IsTrue (SLE.SequenceEqual (new int[] { 2,1 }, rb.Reverse().SkipWhile ((x,i) => x%2!=0)));

            for (int i = 4; i < 50; ++i)
                rb.Add (i);

            Assert.IsTrue (SLE.SequenceEqual (new int[] { 3,2,1 }, rb.Reverse().Skip(20).SkipWhile ((x,i) => x%3!=0 || i<24)));
        }


        [TestMethod]
        public void StressRbq_SkipWhile()
        {
            var rb = new RankedBag<int> { Capacity=4 };
            int n = 25;

            for (int x1 = 0; x1 < n; ++x1)
            {
                rb.Clear();
                for (int x3 = 0; x3 < x1; ++x3)
                    rb.Add (x3);

                System.Collections.Generic.IEnumerable<int> q0 = rb.SkipWhile (x=>false);
                Assert.AreEqual (x1, SLE.Count (q0));
            }
        }

        #endregion

        #region Test enumeration (LINQ emulation)

        [TestMethod]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRbq_DistinctHotUpdate()
        {
            var rb = new RankedBag<int> { Capacity=4 };
            foreach (int ii in new int[] { 1,1,3,5,7,9 }) rb.Add (ii);
            int n = 0;
#if TEST_BCL
            foreach (var x in Enumerable.Distinct (rb))
#else
            foreach (var x in rb.Distinct())
#endif
                if (++n == 2)
                    rb.Remove (3);
        }

        [TestMethod]
        public void UnitRbq_Distinct()
        {
            var rb0 = new RankedBag<int>();
            var rb1 = new RankedBag<int> { Capacity=4 };

            int a0 = 0, a1 = 0;
            foreach (var ii in new int[] { 3, 5, 5, 7, 7 })
                rb1.Add (ii);
#if TEST_BCL
            foreach (var k0 in Enumerable.Distinct (rb0)) ++a0;
            foreach (var k1 in Enumerable.Distinct (rb1)) ++a1;
#else
            foreach (var k0 in rb0.Distinct()) ++a0;
            foreach (var k1 in rb1.Distinct()) ++a1;
#endif

            Assert.AreEqual (0, a0);
            Assert.AreEqual (3, a1);
        }


        [TestMethod]
#if ! TEST_BCL
        [ExpectedException (typeof (InvalidOperationException))]
#endif
        public void CrashRbq_ReverseHotUpdate()
        {
            var rb = new RankedBag<int> { Capacity=4 };
            for (int ii = 9; ii >= 0; --ii) rb.Add (ii);

            int a = 0;
#if TEST_BCL
            foreach (var key in Enumerable.Reverse (rb))
#else
            foreach (var x in rb.Reverse())
#endif
                if (++a == 2)
                    rb.Clear();
        }

        [TestMethod]
        public void UnitRbq_Reverse()
        {
            var rb0 = new RankedBag<int>();
            var rb1 = new RankedBag<int> { Capacity=4 };
            int n = 400;

            for (int i1 = 0; i1 < n; ++i1)
                rb1.Add (i1/2);

            int a0 = 0, a1 = 0;
#if TEST_BCL
            foreach (var k0 in Enumerable.Reverse (rb0)) ++a0;
            foreach (var k1 in Enumerable.Reverse (rb1))
#else
            foreach (var k0 in rb0.Reverse()) ++a0;
            foreach (var k1 in rb1.Reverse())
#endif
            {
                ++a1;
                Assert.AreEqual ((n-a1)/2, k1);
            }
            Assert.AreEqual (0, a0);
            Assert.AreEqual (n, a1);
        }


        [TestMethod]
        public void UnitRbq_Reset()
        {
            var rb = new RankedBag<int>(new int[] { 1,2,5,8,9 }) { Capacity=4 };

            System.Collections.Generic.IEnumerable<int> bagEtor = rb.Reverse().Skip (1).SkipWhile (x => x%2==0);

            Assert.IsTrue (SLE.SequenceEqual (new int[] { 5,2,1 }, bagEtor));

            ((System.Collections.IEnumerator) bagEtor).Reset();

            Assert.IsTrue (SLE.SequenceEqual (new int[] { 5,2,1 }, bagEtor));
        }

        #endregion

        #region Test bonus (LINQ emulation)
#if ! TEST_BCL

        [TestMethod]
        public void UnitRbqx_oEtorGetEnumerator()
        {
            var ia = new int[] { 2,2,3,3,5,6,8 };
            var rb = new RankedBag<int> (ia) { Capacity=4 };

            var oAble1 = (System.Collections.IEnumerable) rb;
            System.Collections.IEnumerator oEtor1 = oAble1.GetEnumerator();
            var oAble2 = (System.Collections.IEnumerable) oEtor1;
            System.Collections.IEnumerator oEtor2 = oAble2.GetEnumerator();

            int ix = 0;
            while (oEtor2.MoveNext())
            {
                object oItem = oEtor2.Current;
                Assert.AreEqual (ia[ix], oItem);
                ++ix;
            }
            Assert.AreEqual (ia.Length, ix);
        }

#endif
        #endregion
    }
}
