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
        public void UnitRbq_Last()
        {
            var rb = new RankedBag<int> { Capacity=4 };
            int n = 99;
            for (int ii = n; ii >= 0; --ii) rb.Add (ii);
#if TEST_BCL
            Assert.AreEqual (n, Enumerable.Last (rb));
#else
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

        #endregion
    }
}
