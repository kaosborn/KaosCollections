//
// File: TestRmDeLinq.cs
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
        public void CrashRmq_ElementAt1_ArgumentOutOfRange()
        {
            var rm = new RankedMap<int,int>();
#if TEST_BCL
            var zz = Enumerable.ElementAt (rm, -1);
#else
            var zz = rm.ElementAt (-1);
#endif
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRmq_ElementAt2_ArgumentOutOfRange()
        {
            var rm = new RankedMap<int,int>();
#if TEST_BCL
            var zz = Enumerable.ElementAt (rm, 0);
#else
            var zz = rm.ElementAt (0);
#endif
        }

        [TestMethod]
        public void UnitRmq_ElementAt()
        {
            var rm = new RankedMap<int,int> { Capacity=4 };
            int n = 800;

            for (int ii = 0; ii <= n; ii+=2)
                rm.Add (ii, ~ii);

            for (int ii = 0; ii <= n/2; ii+=2)
            {
#if TEST_BCL
                var kv = Enumerable.ElementAt (rm, ii);
#else
                var kv = rm.ElementAt (ii);
#endif
                Assert.AreEqual (ii*2, kv.Key);
                Assert.AreEqual (~(ii*2), kv.Value);
            }
        }


        [TestMethod]
        public void UnitRmq_ElementAtOrDefault()
        {
            var rm = new RankedMap<int?,int?>();

#if TEST_BCL
            var kvN = Enumerable.ElementAtOrDefault (rm, -1);
            var kv0 = Enumerable.ElementAtOrDefault (rm, 0);
#else
            var kvN = rm.ElementAtOrDefault (-1);
            var kv0 = rm.ElementAtOrDefault (0);
#endif
            Assert.AreEqual (default (int?), kvN.Key);
            Assert.AreEqual (default (int?), kvN.Value);

            Assert.AreEqual (default (int?), kv0.Key);
            Assert.AreEqual (default (int?), kv0.Value);

            rm.Add (9, -9);
#if TEST_BCL
            var kvZ = Enumerable.ElementAtOrDefault (rm, 0);
            var kv1 = Enumerable.ElementAtOrDefault (rm, 1);
#else
            var kvZ = rm.ElementAtOrDefault (0);
            var kv1 = rm.ElementAtOrDefault (1);
#endif
            Assert.AreEqual (9, kvZ.Key);
            Assert.AreEqual (-9, kvZ.Value);

            Assert.AreEqual (default (int?), kv1.Key);
            Assert.AreEqual (default (int?), kv1.Value);
        }

        #endregion

        #region Test enumeration (LINQ emulation)

        [TestMethod]
        public void UnitRmq_ReverseEmpty()
        {
            var rm = new RankedMap<int,int>();
            int actual = 0;

#if TEST_BCL
            foreach (var countdown in Enumerable.Reverse (rm))
#else
            foreach (var countdown in rm.Reverse())
#endif
               ++actual;

            Assert.AreEqual (0, actual);
        }

        [TestMethod]
#if ! TEST_BCL
        [ExpectedException (typeof (InvalidOperationException))]
#endif
        public void CrashRmq_ReverseHotUpdate()
        {
            var rm = new RankedMap<string,int> { {"aa",1}, {"bb",2}, {"cc",3}, {"dd",4} };
            int n = 0;

#if TEST_BCL
            foreach (var key in Enumerable.Reverse (rm))
#else
            foreach (var kv in rm.Reverse())
#endif
                if (++n == 2)
                    rm.Clear();
        }

        [TestMethod]
        public void UnitRmq_Reverse()
        {
            var rm = new RankedMap<int,int> { Capacity=5 };
            int n = 500, n1 = n;

            for (int ii=1; ii <= n; ++ii)
                rm.Add (ii, -ii);

#if TEST_BCL
            foreach (var actual in Enumerable.Reverse (rm))
#else
            foreach (var actual in rm.Reverse())
#endif
            {
                Assert.AreEqual (n1, actual.Key);
                Assert.AreEqual (-n1, actual.Value);
                --n1;
            }
            Assert.AreEqual (0, n1);
        }

        #endregion


        #region Test Keys methods (LINQ emulation)

        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRmkq_Contains_ArgumentNull()
        {
            var rm = new RankedMap<string,int> { {"beta",2} };
#if TEST_BCL
            var zz = Enumerable.Contains (rm.Keys, null);
#else
            var zz = rm.Keys.Contains (null);
#endif
        }

        [TestMethod]
        public void UnitRmkq_Contains()
        {
            var rm = new RankedMap<int,int> { {22,222 } };
#if TEST_BCL
            Assert.IsTrue (Enumerable.Contains (rm.Keys, 22));
            Assert.IsFalse (Enumerable.Contains (rm.Keys, 33));
#else
            Assert.IsTrue (rm.Keys.Contains (22));
            Assert.IsFalse (rm.Keys.Contains (33));
#endif
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRmkq_ElementAt_ArgumentOutOfRange1()
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
        public void CrashRmkq_ElementAt_ArgumentOutOfRange2()
        {
            var rm = new RankedMap<int,int>();
#if TEST_BCL
            var zz = Enumerable.ElementAt (rm.Keys, 0);
#else
            var zz = rm.Keys.ElementAt (0);
#endif
        }

        [TestMethod]
        public void UnitRmkq_ElementAt()
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
        public void UnitRmkq_ElementAtOrDefault()
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


        [TestMethod]
        public void UnitRmkq_ReverseEmpty()
        {
            var rm = new RankedMap<int,int> { Capacity=4 };
#if TEST_BCL
            foreach (var x in Enumerable.Reverse (rm.Keys))
#else
            foreach (var x in rm.Keys.Reverse())
#endif
            {
                Assert.Fail ("Unreachable");
            }
        }

        [TestMethod]
        public void UnitRmkq_Reverse()
        {
            var rm = new RankedMap<int,int> { Capacity=4 };
            int n = 100;

            for (int i1 = 1; i1 <= n; ++i1)
                rm.Add (i1, -i1);

            int expected = n;
#if TEST_BCL
            foreach (var x in Enumerable.Reverse (rm.Keys))
#else
            foreach (var x in rm.Keys.Reverse())
#endif
            {
                Assert.AreEqual (expected, x);
                --expected;
            }
        }

        #endregion


        #region Test Values methods (LINQ emulation)

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRmvq_ElementAt_ArgumentOutOfRange1()
        {
            var rm = new RankedMap<int,int>();
#if TEST_BCL
            var zz = Enumerable.ElementAt (rm.Values, -1);
#else
            var zz = rm.Values.ElementAt (-1);
#endif
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRmvq_ElementAt_ArgumentOutOfRange2()
        {
            var rm = new RankedMap<int,int>();
#if TEST_BCL
            var zz = Enumerable.ElementAt (rm.Values, 0);
#else
            var zz = rm.Values.ElementAt (0);
#endif
        }

        [TestMethod]
        public void UnitRdv_ElementAt()
        {
            var rm = new RankedMap<string,int> { {"aa",0}, {"bb",-1}, {"bb",-2} };

#if TEST_BCL
            Assert.AreEqual ( 0, Enumerable.ElementAt (rm.Values, 0));
            Assert.AreEqual (-1, Enumerable.ElementAt (rm.Values, 1));
            Assert.AreEqual (-2, Enumerable.ElementAt (rm.Values, 2));
#else
            Assert.AreEqual ( 0, rm.Values.ElementAt (0));
            Assert.AreEqual (-1, rm.Values.ElementAt (1));
            Assert.AreEqual (-2, rm.Values.ElementAt (2));
#endif
        }


        [TestMethod]
        public void UnitRmvq_ElementAtOrDefault()
        {
            var rm = new RankedMap<string,int?> { {"aa",0}, {"bb",-1}, {"bb",-2} };

#if TEST_BCL
            Assert.AreEqual ( 0, Enumerable.ElementAtOrDefault (rm.Values, 0));
            Assert.AreEqual (-1, Enumerable.ElementAtOrDefault (rm.Values, 1));
            Assert.AreEqual (-2, Enumerable.ElementAtOrDefault (rm.Values, 2));
            Assert.AreEqual (default (int?), Enumerable.ElementAtOrDefault (rm.Values, -1));
            Assert.AreEqual (default (int?), Enumerable.ElementAtOrDefault (rm.Values, 3));
#else
            Assert.AreEqual ( 0, rm.Values.ElementAtOrDefault (0));
            Assert.AreEqual (-1, rm.Values.ElementAtOrDefault (1));
            Assert.AreEqual (-2, rm.Values.ElementAtOrDefault (2));
            Assert.AreEqual (default (int?), rm.Values.ElementAtOrDefault (-1));
            Assert.AreEqual (default (int?), rm.Values.ElementAtOrDefault (3));
#endif
        }


        [TestMethod]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRmvq_First()
        {
            var rm = new RankedMap<int,int>();
#if TEST_BCL
            var zz = Enumerable.First (rm.Values);
#else
            var zz = rm.Values.First();
#endif
        }

        [TestMethod]
        public void UnitRmvq_First()
        {
            var rm = new RankedMap<int,int> { Capacity=4 };
            for (int ii = 9; ii >= 1; --ii) rm.Add (ii, -ii);
#if TEST_BCL
            Assert.AreEqual (-1, Enumerable.First (rm.Values));
#else
            Assert.AreEqual (-1, rm.Values.First());
#endif
        }

        [TestMethod]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRmvq_Last()
        {
            var rm = new RankedMap<int,int>();
#if TEST_BCL
            var zz = Enumerable.Last (rm.Values);
#else
            var zz = rm.Values.Last();
#endif
        }

        [TestMethod]
        public void UnitRmvq_Last()
        {
            var rm = new RankedMap<int,int> { Capacity=4 };
            for (int ii = 9; ii >= 1; --ii) rm.Add (ii, -ii);
#if TEST_BCL
            Assert.AreEqual (-9, Enumerable.Last (rm.Values));
#else
            Assert.AreEqual (-9, rm.Values.Last());
#endif
        }

        #endregion

        #region Test Values enumeration (LINQ emulation)

        [TestMethod]
        public void UnitRmvq_ReverseEmpty()
        {
            var rm = new RankedMap<int,int> { Capacity=4 };
#if TEST_BCL
            foreach (var x in Enumerable.Reverse (rm.Values))
#else
            foreach (var x in rm.Values.Reverse())
#endif
            {
                Assert.Fail ("Unreachable");
            }
        }

        [TestMethod]
        public void UnitRmvq_Reverse()
        {
            var rm = new RankedMap<int,int> { Capacity=4 };
            int n = 100;

            for (int i1 = 1; i1 <= n; ++i1)
                rm.Add (i1, -i1);

            int expected = -n;
#if TEST_BCL
            foreach (var x in Enumerable.Reverse (rm.Values))
#else
            foreach (var x in rm.Values.Reverse())
#endif
            {
                Assert.AreEqual (expected, x);
                ++expected;
            }
        }

        #endregion
    }
}
