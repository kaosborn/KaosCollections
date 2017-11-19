//
// File: TestRmDeLinq.cs
// Purpose: Exercise LINQ replacement API for emulation verification.
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
        #region Test Keys properties (LINQ emulation)

        [TestMethod]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRmkq_Max()
        {
            var rm = new RankedMap<int,int>();
            var keys = rm.Keys;
#if TEST_BCL
            var zz = Enumerable.Max (rm.Keys);
#else
            int zz = keys.Max();
#endif
        }

        [TestMethod]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRmkq_Min()
        {
            var rm = new RankedMap<int,int>();
            var keys = rm.Keys;
#if TEST_BCL
            var zz = Enumerable.Min (rm.Keys);
#else
            int zz = keys.Min();
#endif
        }

        [TestMethod]
        public void UnitRmkq_MinMax()
        {
            var rm = new RankedMap<int,int> { Capacity=4 };
            var keys = rm.Keys;
            int n = 500;

            for (int ii = 1; ii <= n; ++ii)
                rm.Add (ii, -ii);
#if TEST_BCL
            Assert.AreEqual (1, Enumerable.Min (rm.Keys));
            Assert.AreEqual (n, Enumerable.Max (rm.Keys));
#else
            Assert.AreEqual (1, keys.Min());
            Assert.AreEqual (n, keys.Max());
#endif
        }

        #endregion

        #region Test Keys methods (LINQ emulation)

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRmk_ElementAt_ArgumentOutOfRange1()
        {
            var rm = new RankedMap<int,int>();
#if TEST_BCL
            var zz = Enumerable.ElementAt (rm.Keys, -1);
#else
            var zz = rm.Keys.ElementAt (-1);
#endif
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRmk_ElementAt_ArgumentOutOfRange2()
        {
            var rm = new RankedMap<int,int>();
#if TEST_BCL
            var zz = Enumerable.ElementAt (rm.Keys, 0);
#else
            var zz = rm.Keys.ElementAt (0);
#endif
        }

        [TestMethod]
        public void UnitRdk_ElementAt()
        {
            var rm = new RankedMap<string,int> { {"0zero",0}, {"1one",-1}, {"1one",-2} };

#if TEST_BCL
            Assert.AreEqual ("0zero",Enumerable.ElementAt (rm.Keys, 0));
            Assert.AreEqual ("1one", Enumerable.ElementAt (rm.Keys, 1));
            Assert.AreEqual ("1one", Enumerable.ElementAt (rm.Keys, 2));
#else
            Assert.AreEqual ("0zero",rm.Keys.ElementAt (0));
            Assert.AreEqual ("1one", rm.Keys.ElementAt (1));
            Assert.AreEqual ("1one", rm.Keys.ElementAt (2));
#endif
        }


        [TestMethod]
        public void UnitRmk_ElementAtOrDefault()
        {
            var rm = new RankedMap<string,int> { {"0zero",0}, {"1one",-1}, {"1one",-2} };

#if TEST_BCL
            Assert.AreEqual ("0zero",Enumerable.ElementAtOrDefault (rm.Keys, 0));
            Assert.AreEqual ("1one", Enumerable.ElementAtOrDefault (rm.Keys, 1));
            Assert.AreEqual ("1one", Enumerable.ElementAtOrDefault (rm.Keys, 2));
            Assert.AreEqual (default (string), Enumerable.ElementAtOrDefault (rm.Keys, -1));
            Assert.AreEqual (default (string), Enumerable.ElementAtOrDefault (rm.Keys, 3));
#else
            Assert.AreEqual ("0zero",rm.Keys.ElementAtOrDefault (0));
            Assert.AreEqual ("1one", rm.Keys.ElementAtOrDefault (1));
            Assert.AreEqual ("1one", rm.Keys.ElementAtOrDefault (2));
            Assert.AreEqual (default (string), rm.Keys.ElementAtOrDefault (-1));
            Assert.AreEqual (default (string), rm.Keys.ElementAtOrDefault (3));
#endif
        }

        #endregion

        #region Test Keys enumeration (LINQ emulation)

        [TestMethod]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRmkq_DistinctHotUpdate()
        {
            var rm = new RankedMap<string,int> { {"aa",1}, {"bb",2}, {"cc",3} };
            int n = 0;

#if TEST_BCL
            foreach (var key in Enumerable.Distinct (rm.Keys))
#else
            foreach (var kv in rm.Keys.Distinct())
#endif
                if (++n == 2)
                    rm.Remove ("aa");
        }

        [TestMethod]
        public void UnitRmkq_Distincts()
        {
            var rm = new RankedMap<int,int> { Capacity=4 };
            int n = 100;

            for (int i1 = 1; i1 < n; ++i1)
                for (int i2 = 0; i2 < i1; ++i2)
                    rm.Add (i1, i2 * n + i2);

            int expected = 1;
#if TEST_BCL
            foreach (var x in Enumerable.Distinct (rm.Keys))
#else
            foreach (var x in rm.Keys.Distinct())
#endif
            {
                Assert.AreEqual (expected, x);
                ++expected;
            }
        }

        #endregion
    }
}
