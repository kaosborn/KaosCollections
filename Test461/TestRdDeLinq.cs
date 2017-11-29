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
            var zz = Enumerable.ElementAt (dary1, -1);
#else
            var zz = dary1.ElementAt (-1);
#endif
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRdq_ElementAtB_ArgumentOutOfRange()
        {
            Setup();
#if TEST_BCL
            var zz = Enumerable.ElementAt (dary1, 0);
#else
            var zz = dary1.ElementAt (0);
#endif
        }

        [TestMethod]
        public void UnitRdq_ElementAt()
        {
            Setup();
            int n = 800;

            for (int ii = 0; ii <= n; ii+=2)
                dary1.Add (ii, ~ii);

            for (int ii = 0; ii <= n/2; ii+=2)
            {
#if TEST_BCL
                KeyValuePair<int,int> pair = Enumerable.ElementAt (dary1, ii);
#else
                KeyValuePair<int,int> pair = dary1.ElementAt (ii);
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
            KeyValuePair<string,int> pairN = Enumerable.ElementAtOrDefault (dary2, -1);
            KeyValuePair<string,int> pair0 = Enumerable.ElementAtOrDefault (dary2, 0);
#else
            KeyValuePair<string,int> pairN = dary2.ElementAtOrDefault (-1);
            KeyValuePair<string,int> pair0 = dary2.ElementAtOrDefault (0);
#endif
            Assert.AreEqual (default (string), pairN.Key);
            Assert.AreEqual (default (int), pairN.Value);

            Assert.AreEqual (default (string), pair0.Key);
            Assert.AreEqual (default (int), pair0.Value);

            dary2.Add ("nein", -9);

#if TEST_BCL
            KeyValuePair<string,int> pairZ = Enumerable.ElementAtOrDefault (dary2, 0);
            KeyValuePair<string,int> pair1 = Enumerable.ElementAtOrDefault (dary2, 1);
#else
            KeyValuePair<string,int> pairZ = dary2.ElementAtOrDefault (0);
            KeyValuePair<string,int> pair1 = dary2.ElementAtOrDefault (1);
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
            var zz = Enumerable.Last (dary1);
#else
            var zz = dary1.Last();
#endif
        }

        [TestMethod]
        public void UnitRdq_Last()
        {
            Setup();
            dary1.Add (3, -33);
            dary1.Add (1, -11);
            dary1.Add (2, -22);
#if TEST_BCL
            var kv = Enumerable.Last (dary1);
#else
            var kv = dary1.Last();
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
            var keys = dary1.Keys;
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
            var keys = dary1.Keys;
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
            var keys = dary1.Keys;
            int n = 400;

            for (int ii = 1; ii <= n; ++ii)
                dary1.Add (ii, -ii);
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
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRdkq_Contains_ArgumentNull()
        {
            Setup();
            dary2.Add ("pithy", 1);

            var zz = dary2.Keys.Contains (null);
        }

        [TestMethod]
        public void UnitRdkq_Contains()
        {
            Setup (4);
            foreach (var kv in greek)
                dary5.Add (kv.Key, kv.Value);

            Assert.IsTrue (dary5.Keys.Contains ("Iota"));
            Assert.IsTrue (dary5.Keys.Contains ("IOTA"));
            Assert.IsFalse (dary5.Keys.Contains ("Zed"));
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRdkq_ElementAt_ArgumentOutOfRange1()
        {
            Setup();
#if TEST_BCL
            var zz = Enumerable.ElementAt (dary1.Keys, -1);
#else
            var zz = dary1.Keys.ElementAt (-1);
#endif
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRdkq_ElementAt_ArgumentOutOfRange2()
        {
            Setup();
#if TEST_BCL
            var zz = Enumerable.ElementAt (dary1.Keys, 0);
#else
            var zz = dary1.Keys.ElementAt (0);
#endif
        }

        [TestMethod]
        public void UnitRdkq_ElementAt()
        {
            Setup();
            var keys = dary2.Keys;
            dary2.Add ("aa", 0);
            dary2.Add ("bb",-1);
            dary2.Add ("cc",-2);

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
            var keys = dary2.Keys;
            dary2.Add ("aa", 0);
            dary2.Add ("bb",-1);
            dary2.Add ("cc",-2);

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


        [TestMethod]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRdkq_First_InvalidOperation()
        {
            Setup();
#if TEST_BCL
            var zz = Enumerable.First (dary1.Keys);
#else
            var zz = dary1.Keys.First();
#endif
        }

        [TestMethod]
        public void UnitRdkq_First()
        {
            Setup (4);
            for (int ii = 1; ii <= 9; ++ii) dary1.Add (ii,-ii);
#if TEST_BCL
            var k1 = Enumerable.First (dary1.Keys);
#else
            var k1 = dary1.Keys.First();
#endif
            Assert.AreEqual (1, k1);
        }


        [TestMethod]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRdkq_Last_InvalidOperation()
        {
            Setup();
#if TEST_BCL
            var zz = Enumerable.Last (dary1.Keys);
#else
            var zz = dary1.Keys.Last();
#endif
        }

        [TestMethod]
        public void UnitRdkq_Last()
        {
            Setup (4);
            for (int ii = 1; ii <= 9; ++ii) dary1.Add (ii,-ii);
#if TEST_BCL
            var k9 = Enumerable.Last (dary1.Keys);
#else
            var k9 = dary1.Keys.Last();
#endif
            Assert.AreEqual (9, k9);
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
            for (int ii = 9; ii > 0; --ii) dary1.Add (ii, -ii);

#if TEST_BCL
            foreach (int k1 in Enumerable.Reverse (dary1.Keys))
#else
            foreach (int k1 in dary1.Keys.Reverse())
#endif
                if (k1 == 4)
                    dary1.Clear();
        }

        [TestMethod]
        public void UnitRdkq_Reverse()
        {
            Setup (5);
            int n = 100;

            for (int i1 = 1; i1 <= n; ++i1)
                dary1.Add (i1, -i1);

            int a0 = 0, an = 0;
#if TEST_BCL
            foreach (var k0 in Enumerable.Reverse (dary2.Keys)) ++a0;
            foreach (var kn in Enumerable.Reverse (dary1.Keys)) ++an;
#else
            foreach (var k0 in dary2.Keys.Reverse()) ++a0;
            foreach (var kn in dary1.Keys.Reverse()) ++an;
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
            var zz = Enumerable.ElementAt (dary1.Values, -1);
#else
            var zz = dary1.Values.ElementAt (-1);
#endif
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRdvq_ElementAt_ArgumentOutOfRange2()
        {
            Setup();
#if TEST_BCL
            var zz = Enumerable.ElementAt (dary1.Values, 0);
#else
            var zz = dary1.Values.ElementAt (0);
#endif
        }

        [TestMethod]
        public void UnitRdvq_ElementAt()
        {
            Setup (4);
            foreach (var kv in greek) dary2.Add (kv.Key, kv.Value);
#if TEST_BCL
            var tree1 = Enumerable.ElementAt (dary2.Values, 6);
#else
            Assert.AreEqual (9, dary2.Values.ElementAt (6));
#endif
        }


        [TestMethod]
        public void UnitRdvq_ElementAtOrDefault()
        {
            Setup (4);
            foreach (var kv in greek) dary3.Add (kv.Key, kv.Value);
#if TEST_BCL
            Assert.IsNull (Enumerable.ElementAtOrDefault (dary3.Values, -1));
            Assert.AreEqual (22, Enumerable.ElementAtOrDefault (dary3.Values, 2));
            Assert.IsNull (Enumerable.ElementAtOrDefault (dary3.Values, dary3.Count));
#else
            Assert.IsNull (dary3.Values.ElementAtOrDefault (-1));
            Assert.AreEqual (22, dary3.Values.ElementAtOrDefault (2));
            Assert.IsNull (dary3.Values.ElementAtOrDefault (dary3.Count));
#endif
        }


        [TestMethod]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRdvq_First()
        {
            Setup();
#if TEST_BCL
            var zz = Enumerable.First (dary1.Values);
#else
            var zz = dary1.Values.First();
#endif
        }

        [TestMethod]
        public void UnitRdvq_First()
        {
            Setup();
            for (int ii = 9; ii >= 1; --ii) dary1.Add (ii, -ii);
#if TEST_BCL
            Assert.AreEqual (-1, Enumerable.First (dary1.Values));
#else
            Assert.AreEqual (-1, dary1.Values.First());
#endif
        }

        [TestMethod]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRdvq_Last()
        {
            Setup();
#if TEST_BCL
            var zz = Enumerable.Last (dary1.Values);
#else
            var zz = dary1.Values.Last();
#endif
        }

        [TestMethod]
        public void UnitRdvq_Last()
        {
            Setup (4);
            for (int ii = 9; ii >= 1; --ii) dary1.Add (ii, -ii);
#if TEST_BCL
            Assert.AreEqual (-9, Enumerable.Last (dary1.Values));
#else
            Assert.AreEqual (-9, dary1.Values.Last());
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
            for (int ii = 9; ii > 0; --ii) dary1.Add (ii, -ii);
#if TEST_BCL
            foreach (int v1 in Enumerable.Reverse (dary1.Values))
#else
            foreach (int v1 in dary1.Values.Reverse())
#endif
                if (v1 == -4)
                    dary1.Clear();
        }

        [TestMethod]
        public void UnitRdvq_Reverse()
        {
            Setup (5);
            int n = 100;

            for (int i1 = 1; i1 <= n; ++i1)
                dary1.Add (i1, -i1);

            int a0 = 0, an = 0;
#if TEST_BCL
            foreach (var v0 in Enumerable.Reverse (dary2.Values)) ++a0;
            foreach (var vn in Enumerable.Reverse (dary1.Values))
#else
            foreach (var v0 in dary2.Values.Reverse()) ++a0;
            foreach (var vn in dary1.Values.Reverse())
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
