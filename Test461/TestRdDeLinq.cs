//
// File: TestRdDeLinq.cs
// Purpose: Test LINQ emulation.
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
        #region Test methods (LINQ emulation)

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRdq_ElementAtA_ArgumentOutOfRange()
        {
            Setup();
#if TEST_BCL
            var zz = Enumerable.ElementAt (tree1, -1);
#else
            var zz = tree1.ElementAt (-1);
#endif
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRdq_ElementAtB_ArgumentOutOfRange()
        {
            Setup();
#if TEST_BCL
            var zz = Enumerable.ElementAt (tree1, 0);
#else
            var zz = tree1.ElementAt (0);
#endif
        }

        [TestMethod]
        public void UnitRdq_ElementAt()
        {
            Setup();
            int n = 800;

            for (int ii = 0; ii <= n; ii+=2)
                tree1.Add (ii, ~ii);

            for (int ii = 0; ii <= n/2; ii+=2)
            {
#if TEST_BCL
                KeyValuePair<int,int> pair = Enumerable.ElementAt (tree1, ii);
#else
                KeyValuePair<int,int> pair = tree1.ElementAt (ii);
#endif
                Assert.AreEqual (ii*2, pair.Key);
                Assert.AreEqual (~(ii*2), pair.Value);
            }
        }


        [TestMethod]
        public void UnitRdq_ElementAtOrDefault()
        {
            Setup();

#if TEST_BCL
            KeyValuePair<string,int> pairN = Enumerable.ElementAtOrDefault (tree2, -1);
            KeyValuePair<string,int> pair0 = Enumerable.ElementAtOrDefault (tree2, 0);
#else
            KeyValuePair<string,int> pairN = tree2.ElementAtOrDefault (-1);
            KeyValuePair<string,int> pair0 = tree2.ElementAtOrDefault (0);
#endif
            Assert.AreEqual (default (string), pairN.Key);
            Assert.AreEqual (default (int), pairN.Value);

            Assert.AreEqual (default (string), pair0.Key);
            Assert.AreEqual (default (int), pair0.Value);

            tree2.Add ("nein", -9);

#if TEST_BCL
            KeyValuePair<string,int> pairZ = Enumerable.ElementAtOrDefault (tree2, 0);
            KeyValuePair<string,int> pair1 = Enumerable.ElementAtOrDefault (tree2, 1);
#else
            KeyValuePair<string,int> pairZ = tree2.ElementAtOrDefault (0);
            KeyValuePair<string,int> pair1 = tree2.ElementAtOrDefault (1);
#endif
            Assert.AreEqual ("nein", pairZ.Key);
            Assert.AreEqual (-9, pairZ.Value);

            Assert.AreEqual (default (string), pair1.Key);
            Assert.AreEqual (default (int), pair1.Value);
        }


        [TestMethod]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRdq_Last_InvalidOperation()
        {
            Setup();
#if TEST_BCL
            var zz = Enumerable.Last (tree1);
#else
            var zz = tree1.Last();
#endif
        }

        [TestMethod]
        public void UnitRdq_Last()
        {
            Setup();
            tree1.Add (3, -33);
            tree1.Add (1, -11);
            tree1.Add (2, -22);
#if TEST_BCL
            var kv = Enumerable.Last (tree1);
#else
            var kv = tree1.Last();
#endif
            Assert.AreEqual (3, kv.Key, "didn't get expected last key");
            Assert.AreEqual (-33, kv.Value, "didn't get expected last value");
        }

        #endregion

        #region Test Keys properties (LINQ emulation)

        [TestMethod]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRdkq_Max()
        {
            Setup (4);
            var keys = tree1.Keys;
#if TEST_BCL
            int zz = Enumerable.Max (keys);
#else
            int zz = keys.Max();
#endif
        }

        [TestMethod]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRdkq_Min()
        {
            Setup (4);
            var keys = tree1.Keys;
#if TEST_BCL
            int zz = Enumerable.Min (keys);
#else
            int zz = keys.Min();
#endif
        }

        [TestMethod]
        public void UnitRdkq_MinMax()
        {
            Setup (4);
            var keys = tree1.Keys;
            int n = 400;

            for (int ii = 1; ii <= n; ++ii)
                tree1.Add (ii, -ii);
#if TEST_BCL
            Assert.AreEqual (1, Enumerable.Min (keys));
            Assert.AreEqual (n, Enumerable.Max (keys));
#else
            Assert.AreEqual (1, keys.Min());
            Assert.AreEqual (n, keys.Max());
#endif
        }

        #endregion


        #region Test Keys methods (LINQ emulation)

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRdk_ElementAt_ArgumentOutOfRange1()
        {
            Setup();
#if TEST_BCL
            var zz = Enumerable.ElementAt (tree1.Keys, -1);
#else
            var zz = tree1.Keys.ElementAt (-1);
#endif
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRdk_ElementAt_ArgumentOutOfRange2()
        {
            Setup();
#if TEST_BCL
            var zz = Enumerable.ElementAt (tree1.Keys, 0);
#else
            var zz = tree1.Keys.ElementAt (0);
#endif
        }

        [TestMethod]
        public void UnitRdk_ElementAt()
        {
            Setup();
            var keys = tree2.Keys;
            tree2.Add ("aa", 0);
            tree2.Add ("bb",-1);
            tree2.Add ("cc",-2);

#if TEST_BCL
            Assert.AreEqual ("aa", Enumerable.ElementAt (keys, 0));
            Assert.AreEqual ("bb", Enumerable.ElementAt (keys, 1));
            Assert.AreEqual ("cc", Enumerable.ElementAt (keys, 2));
#else
            Assert.AreEqual ("aa", keys.ElementAt (0));
            Assert.AreEqual ("bb", keys.ElementAt (1));
            Assert.AreEqual ("cc", keys.ElementAt (2));
#endif
        }


        [TestMethod]
        public void UnitRdk_ElementAtOrDefault()
        {
            Setup();
            var keys = tree2.Keys;
            tree2.Add ("aa", 0);
            tree2.Add ("bb",-1);
            tree2.Add ("cc",-2);

#if TEST_BCL
            Assert.AreEqual ("aa", Enumerable.ElementAtOrDefault (keys, 0));
            Assert.AreEqual ("bb", Enumerable.ElementAtOrDefault (keys, 1));
            Assert.AreEqual ("cc", Enumerable.ElementAtOrDefault (keys, 2));
            Assert.AreEqual (default (string), Enumerable.ElementAtOrDefault (keys, -1));
            Assert.AreEqual (default (string), Enumerable.ElementAtOrDefault (keys, 3));
#else
            Assert.AreEqual ("aa", keys.ElementAtOrDefault (0));
            Assert.AreEqual ("bb", keys.ElementAtOrDefault (1));
            Assert.AreEqual ("cc", keys.ElementAtOrDefault (2));
            Assert.AreEqual (default (string), keys.ElementAtOrDefault (-1));
            Assert.AreEqual (default (string), keys.ElementAtOrDefault (3));
#endif
        }

        #endregion
    }
}
