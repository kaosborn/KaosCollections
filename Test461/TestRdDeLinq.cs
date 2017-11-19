//
// File: TestRdDeLinq.cs
// Purpose: Exercise LINQ API optimized with instance methods.
//

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
#if TEST_BCL
using System.Linq;
#endif

namespace Kaos.Test.Collections
{
    public partial class TestBtree
    {
        #region Test bonus Keys LINQ instance implementations

        [TestMethod]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRdkq_Max()
        {
            Setup (4);
            var keys = tree1.Keys;
            int zz = keys.Max();
        }

        [TestMethod]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRdkq_Min()
        {
            Setup (4);
            var keys = tree1.Keys;
            int zz = keys.Min();
        }

        [TestMethod]
        public void UnitRdkq_MinMax()
        {
            Setup (4);
            var keys = tree1.Keys;
            int n = 400;

            for (int ii = 1; ii <= n; ++ii)
                tree1.Add (ii, -ii);

            Assert.AreEqual (1, keys.Min());
            Assert.AreEqual (n, keys.Max());
        }

        #endregion

        #region Test bonus LINQ instance implementations

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRdq_ElementAt1_ArgumentOutOfRange()
        {
            Setup();
            KeyValuePair<int,int> pair = tree1.ElementAt (-1);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRdq_ElementAt2_ArgumentOutOfRange()
        {
            Setup();
            KeyValuePair<int,int> pair = tree1.ElementAt (0);
        }

        [TestMethod]
        public void UnitRdq_ElementAt()
        {
            Setup();

            for (int ii = 0; ii <= 800; ii+=2)
                tree1.Add (ii, ii+100);

            for (int ii = 0; ii <= 400; ii+=2)
            {
                KeyValuePair<int,int> pair = tree1.ElementAt (ii);
                Assert.AreEqual (ii*2, pair.Key);
                Assert.AreEqual (ii*2+100, pair.Value);
            }
        }


        [TestMethod]
        public void UnitRdq_ElementAtOrDefault()
        {
            Setup();

            KeyValuePair<int,int> pairM1 = tree1.ElementAtOrDefault (-1);
            Assert.AreEqual (default (int), pairM1.Key);
            Assert.AreEqual (default (int), pairM1.Value);

            KeyValuePair<int,int> pair0 = tree1.ElementAtOrDefault (0);
            Assert.AreEqual (default (int), pair0.Key);
            Assert.AreEqual (default (int), pair0.Value);

            tree1.Add (9, -9);

            KeyValuePair<int,int> pair00 = tree1.ElementAtOrDefault (0);
            Assert.AreEqual (9, pair00.Key);
            Assert.AreEqual (-9, pair00.Value);

            KeyValuePair<int,int> pair1 = tree1.ElementAtOrDefault (1);
            Assert.AreEqual (default (int), pair1.Key);
            Assert.AreEqual (default (int), pair1.Value);
        }


        [TestMethod]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRdq_Last_InvalidOperation()
        {
            Setup();
            var kv = tree2.Last();
        }

        [TestMethod]
        public void UnitRdq_Last()
        {
            Setup();
            tree1.Add (3, -33);
            tree1.Add (1, -11);
            tree1.Add (2, -22);

            var kv = tree1.Last();

            Assert.AreEqual (3, kv.Key, "didn't get expected last key");
            Assert.AreEqual (-33, kv.Value, "didn't get expected last value");
        }

        #endregion
    }
}