﻿//
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
        public void CrashRdkq_ElementAt_ArgumentOutOfRange1()
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
        public void CrashRdkq_ElementAt_ArgumentOutOfRange2()
        {
            Setup();
#if TEST_BCL
            var zz = Enumerable.ElementAt (tree1.Keys, 0);
#else
            var zz = tree1.Keys.ElementAt (0);
#endif
        }

        [TestMethod]
        public void UnitRdkq_ElementAt()
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
        public void UnitRdkq_ElementAtOrDefault()
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

        #region Test Keys enumeration (LINQ emulation)

        [TestMethod]
#if ! TEST_BCL
        [ExpectedException (typeof (InvalidOperationException))]
#endif
        public void CrashRdkq_ReverseHotUpdate()
        {
            Setup (5);
            for (int ii = 9; ii > 0; --ii) tree1.Add (ii, -ii);

#if TEST_BCL
            foreach (int k1 in Enumerable.Reverse (tree1.Keys))
#else
            foreach (int k1 in tree1.Keys.Reverse())
#endif
                if (k1 == 4)
                    tree1.Clear();
        }

        [TestMethod]
        public void UnitRdkq_Reverse()
        {
            Setup (5);
            int n = 100;

            for (int i1 = 1; i1 <= n; ++i1)
                tree1.Add (i1, -i1);

            int a0 = 0, an = 0;
#if TEST_BCL
            foreach (var k0 in Enumerable.Reverse (tree2.Keys)) ++a0;
            foreach (var kn in Enumerable.Reverse (tree1.Keys)) ++an;
#else
            foreach (var k0 in tree2.Keys.Reverse()) ++a0;
            foreach (var kn in tree1.Keys.Reverse()) ++an;
#endif
            Assert.AreEqual (0, a0);
            Assert.AreEqual (n, an);
        }

        #endregion


        #region Test Values methods (LINQ emulation)

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRdvq_ElementAt_ArgumentOutOfRange1()
        {
            Setup();
#if TEST_BCL
            var zz = Enumerable.ElementAt (tree1.Values, -1);
#else
            var zz = tree1.Values.ElementAt (-1);
#endif
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRdvq_ElementAt_ArgumentOutOfRange2()
        {
            Setup();
#if TEST_BCL
            var zz = Enumerable.ElementAt (tree1.Values, 0);
#else
            var zz = tree1.Values.ElementAt (0);
#endif
        }

        [TestMethod]
        public void UnitRdvq_ElementAt()
        {
            Setup (4);
            foreach (var kv in greek) tree2.Add (kv.Key, kv.Value);
#if TEST_BCL
            var tree1 = Enumerable.ElementAt (tree2.Values, 6);
#else
            Assert.AreEqual (9, tree2.Values.ElementAt (6));
#endif
        }


        [TestMethod]
        public void UnitRdvq_ElementAtOrDefault()
        {
            Setup (4);
            foreach (var kv in greek) tree3.Add (kv.Key, kv.Value);
#if TEST_BCL
            Assert.IsNull (Enumerable.ElementAtOrDefault (tree3.Values, -1));
            Assert.AreEqual (22, Enumerable.ElementAtOrDefault (tree3.Values, 2));
            Assert.IsNull (Enumerable.ElementAtOrDefault (tree3.Values, tree3.Count));
#else
            Assert.IsNull (tree3.Values.ElementAtOrDefault (-1));
            Assert.AreEqual (22, tree3.Values.ElementAtOrDefault (2));
            Assert.IsNull (tree3.Values.ElementAtOrDefault (tree3.Count));
#endif
        }

        #endregion

        #region Test Values enumeration (LINQ emulation)

        [TestMethod]
#if ! TEST_BCL
        [ExpectedException (typeof (InvalidOperationException))]
#endif
        public void CrashRdvq_ReverseHotUpdate()
        {
            Setup (4);
            for (int ii = 9; ii > 0; --ii) tree1.Add (ii, -ii);
#if TEST_BCL
            foreach (int v1 in Enumerable.Reverse (tree1.Values))
#else
            foreach (int v1 in tree1.Values.Reverse())
#endif
                if (v1 == -4)
                    tree1.Clear();
        }

        [TestMethod]
        public void UnitRdvq_Reverse()
        {
            Setup (5);
            int n = 100;

            for (int i1 = 1; i1 <= n; ++i1)
                tree1.Add (i1, -i1);

            int a0 = 0, an = 0;
#if TEST_BCL
            foreach (var v0 in Enumerable.Reverse (tree2.Values)) ++a0;
            foreach (var vn in Enumerable.Reverse (tree1.Values))
#else
            foreach (var v0 in tree2.Values.Reverse()) ++a0;
            foreach (var vn in tree1.Values.Reverse())
#endif
            {
                Assert.AreEqual (an-n, vn);
                ++an;
            }

            Assert.AreEqual (0, a0);
            Assert.AreEqual (n, an);
        }

        #endregion
    }
}
