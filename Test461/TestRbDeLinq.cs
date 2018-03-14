//
// File: TestRbDeLinq.cs
// Purpose: Test LINQ emulation.
//

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            int n = 800;

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
        public void UnitRbq_SkipA()
        {
            var rb = new RankedBag<int> { Capacity=5 };

            int k0 = System.Linq.Enumerable.Count (rb.Skip (0));
            Assert.AreEqual (0, k0);

            k0 = System.Linq.Enumerable.Count (rb.Skip (-1));
            Assert.AreEqual (0, k0);

            k0 = System.Linq.Enumerable.Count (rb.Skip (1));
            Assert.AreEqual (0, k0);

            rb.Add (1); rb.Add (2);

            int k1 = System.Linq.Enumerable.Count (rb.Skip (-1));
            Assert.AreEqual (2, k1);

            int k2 = System.Linq.Enumerable.Count (rb.Skip (3));
            Assert.AreEqual (0, k2);

            int k3 = System.Linq.Enumerable.Count (rb.Skip (0).Skip (-1));
            Assert.AreEqual (2, k3);

            int k4 = System.Linq.Enumerable.Count (rb.Skip (0).Skip (1));
            Assert.AreEqual (1, k4);

            int k5 = System.Linq.Enumerable.Count (rb.Skip (0).Skip (3));
            Assert.AreEqual (0, k5);

            int k6 = System.Linq.Enumerable.Count (rb.Reverse().Skip (-1));
            Assert.AreEqual (2, k6);

            int k7 = System.Linq.Enumerable.Count (rb.Reverse().Skip (1));
            Assert.AreEqual (1, k7);

            int k8 = System.Linq.Enumerable.Count (rb.Reverse().Skip (3));
            Assert.AreEqual (0, k8);
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

            int a0 = 0, a1 = 0;
            foreach (var k0 in rb.SkipWhile (x => false))
                ++a0;
            Assert.AreEqual (0, a0);

            foreach (var k0 in rb.SkipWhile (x => true))
                ++a0;
            Assert.AreEqual (0, a0);

            rb.Add (1);

            foreach (var k0 in rb.SkipWhile (x => false))
            {
                Assert.AreEqual (a0+1, k0);
                ++a0;
            }
            Assert.AreEqual (1, a0);

            foreach (var k0 in rb.SkipWhile (x => true))
                ++a1;
            Assert.AreEqual (0, a1);
        }

        [TestMethod]
        public void UnitRbq_SkipWhile2F()
        {
            var rb = new RankedBag<int> { Capacity=4 };
            System.Collections.Generic.IEnumerable<int> q0;
            int a0 = 0;

            foreach (var k0 in rb.Skip (0).SkipWhile (x => true))
                ++a0;

            q0 = rb.Skip (0).SkipWhile (x => true);
            Assert.AreEqual (0, System.Linq.Enumerable.Count (q0));

            rb.Add (-1);
            q0 = rb.Skip (0).SkipWhile (x => true);
            Assert.AreEqual (0, System.Linq.Enumerable.Count (q0));
            q0 = rb.Skip (0).SkipWhile (x => false);
            Assert.AreEqual (1, System.Linq.Enumerable.Count (q0));
        }

        [TestMethod]
        public void UnitRbq_SkipWhile2R()
        {
            var rb = new RankedBag<int> { Capacity=4 };

            int a0 = 0;
            foreach (var k0 in rb.Reverse().SkipWhile (x => false))
                ++a0;
            Assert.AreEqual (0, a0);

            foreach (var k0 in rb.Reverse().SkipWhile (x => true))
                ++a0;
            Assert.AreEqual (0, a0);

            rb.Add (1);

            foreach (var k0 in rb.Reverse().SkipWhile (x => false))
            {
                Assert.AreEqual (a0+1, k0);
                ++a0;
            }
            Assert.AreEqual (1, a0);
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

                Assert.AreEqual (x1, System.Linq.Enumerable.Count (q0));
            }
        }

        #endregion
    }
}
