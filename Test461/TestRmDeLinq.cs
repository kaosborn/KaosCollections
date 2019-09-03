//
// File: TestRmDeLinq.cs
// Purpose: Test LINQ emulation.
//

using System;
using SLE=System.Linq.Enumerable;
using SCG=System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
#if TEST_BCL
using System.Linq;
#endif
using Kaos.Collections;

namespace Kaos.Test.Collections
{
    public partial class TestRm
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


        [TestMethod]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRmq_First_InvalidOperation()
        {
            var rm = new RankedMap<int,int>();
#if TEST_BCL
            var zz = Enumerable.First (rm);
#else
            var zz = rm.First();
#endif
        }


        [TestMethod]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRmq_Last_InvalidOperation()
        {
            var rm = new RankedMap<int,int>();
#if TEST_BCL
            var zz = Enumerable.Last (rm);
#else
            var zz = rm.Last();
#endif
        }

        [TestMethod]
        public void UnitRmq_FirstLast()
        {
            var rm = new RankedMap<int,int>() { Capacity=5 };
            for (int ii = 9; ii >= 1; --ii) rm.Add (ii, -ii);
#if TEST_BCL
            var kv1 = Enumerable.First (rm);
            var kv9 = Enumerable.Last (rm);
#else
            var kv1 = rm.First();
            var kv9 = rm.Last();
#endif
            Assert.AreEqual (1, kv1.Key, "wrong first key");
            Assert.AreEqual (-1, kv1.Value, "wrong first value");
            Assert.AreEqual (9, kv9.Key, "wrong last key");
            Assert.AreEqual (-9, kv9.Value, "wrong last value");
        }


        [TestMethod]
        public void UnitRmq_Skip()
        {
            var rm = new RankedMap<int,int> { Capacity=5 };

            Assert.AreEqual (0, SLE.Count (rm.Skip (-1)));
            Assert.AreEqual (0, SLE.Count (rm.Skip (0)));
            Assert.AreEqual (0, SLE.Count (rm.Skip (1)));

            rm.Add (1,11); rm.Add (2,22);
            var p1 = new SCG.KeyValuePair<int,int> (1,11);
            var p2 = new SCG.KeyValuePair<int,int> (2,22);

            Assert.IsTrue (SLE.SequenceEqual (new SCG.KeyValuePair<int,int>[] { p1,p2 }, rm.Skip (-1)));
            Assert.IsTrue (SLE.SequenceEqual (new SCG.KeyValuePair<int,int>[] { p1,p2 }, rm.Skip (0)));
            Assert.IsTrue (SLE.SequenceEqual (new SCG.KeyValuePair<int,int>[] { p2 },    rm.Skip (1)));

            Assert.AreEqual (0, SLE.Count (rm.Skip (0).Skip (2)));
            Assert.AreEqual (0, SLE.Count (rm.Skip (0).Skip (3)));
            Assert.IsTrue (SLE.SequenceEqual (new SCG.KeyValuePair<int,int>[] { p1,p2 }, rm.Skip (0).Skip (-1)));
            Assert.IsTrue (SLE.SequenceEqual (new SCG.KeyValuePair<int,int>[] { p1,p2 }, rm.Skip (0).Skip (0)));
            Assert.IsTrue (SLE.SequenceEqual (new SCG.KeyValuePair<int,int>[] { p2 },    rm.Skip (0).Skip (1)));

            Assert.IsTrue (SLE.SequenceEqual (new SCG.KeyValuePair<int,int>[] { p2 }, rm.Skip (1).Skip (-1)));
            Assert.IsTrue (SLE.SequenceEqual (new SCG.KeyValuePair<int,int>[] { p2 }, rm.Skip (1).Skip (0)));

            Assert.AreEqual (0, SLE.Count (rm.Skip (2).Skip (0)));
            Assert.AreEqual (0, SLE.Count (rm.Skip (2).Skip (1)));

            Assert.IsTrue (SLE.SequenceEqual (new SCG.KeyValuePair<int,int>[] { p2,p1 }, rm.Reverse().Skip (-1)));
            Assert.IsTrue (SLE.SequenceEqual (new SCG.KeyValuePair<int,int>[] { p2,p1 }, rm.Reverse().Skip (0)));
            Assert.IsTrue (SLE.SequenceEqual (new SCG.KeyValuePair<int,int>[] { p1 },    rm.Reverse().Skip (1)));
            Assert.AreEqual (0, SLE.Count (rm.Reverse().Skip (2)));
            Assert.AreEqual (0, SLE.Count (rm.Reverse().Skip (3)));
        }


        [TestMethod]
        public void UnitRmq_SkipWhile2Ctor()
        {
            var rm = new RankedMap<int,int> { Capacity=4 };
            var p1 = new SCG.KeyValuePair<int,int> (1,11);
            var p2 = new SCG.KeyValuePair<int,int> (2,22);

            Assert.AreEqual (0, SLE.Count (rm.SkipWhile (x => false)));
            Assert.AreEqual (0, SLE.Count (rm.SkipWhile (x => true)));

            rm.Add (p1.Key, p1.Value);
            rm.Add (p2.Key, p2.Value);

            Assert.IsTrue (SLE.SequenceEqual (new SCG.KeyValuePair<int,int>[] { p1,p2 }, rm.SkipWhile (x => false)));
            Assert.IsTrue (SLE.SequenceEqual (new SCG.KeyValuePair<int,int>[] { p2 },    rm.SkipWhile (x => x.Key%2!=0)));
            Assert.AreEqual (0, SLE.Count (rm.SkipWhile (x => true)));
        }

        [TestMethod]
        public void UnitRmq_SkipWhile2F()
        {
            var rm = new RankedMap<int,int> { Capacity=5 };
            var p1 = new SCG.KeyValuePair<int,int> (1,-1);
            var p2 = new SCG.KeyValuePair<int,int> (2,-2);
            var p3 = new SCG.KeyValuePair<int,int> (3,-3);

            Assert.AreEqual (0, SLE.Count (rm.Skip (0).SkipWhile (x => false)));
            Assert.AreEqual (0, SLE.Count (rm.Skip (0).SkipWhile (x => true)));

            rm.Add (p1.Key, p1.Value);

            Assert.AreEqual (0, SLE.Count (rm.Skip (0).SkipWhile (x => true)));
            Assert.IsTrue (SLE.SequenceEqual (new SCG.KeyValuePair<int,int>[] { p1 }, rm.Skip(0).SkipWhile (x => false)));

            rm.Add (p2.Key, p2.Value);
            rm.Add (p3.Key, p3.Value);

            Assert.IsTrue (SLE.SequenceEqual (new SCG.KeyValuePair<int,int>[] { p2,p3 }, rm.Skip(0).SkipWhile (kv => kv.Key%2!=0)));
        }

        [TestMethod]
        public void UnitRmq_SkipWhile2R()
        {
            var rm = new RankedMap<int,int> { Capacity=4 };
            var p1 = new SCG.KeyValuePair<int,int> (1,-1);
            var p2 = new SCG.KeyValuePair<int,int> (2,-2);
            var p3 = new SCG.KeyValuePair<int,int> (3,-3);

            Assert.AreEqual (0, SLE.Count (rm.Reverse().SkipWhile (x => false)));
            Assert.AreEqual (0, SLE.Count (rm.Reverse().SkipWhile (x => true)));

            rm.Add (p1.Key, p1.Value);

            Assert.IsTrue (SLE.SequenceEqual (new SCG.KeyValuePair<int,int>[] { p1 }, rm.Reverse().SkipWhile (x => false)));
            Assert.AreEqual (0, SLE.Count (rm.Reverse().SkipWhile (x => true)));

            rm.Add (p2.Key, p2.Value);
            rm.Add (p3.Key, p3.Value);

            Assert.IsTrue (SLE.SequenceEqual (new SCG.KeyValuePair<int,int>[] { p2,p1 }, rm.Reverse().SkipWhile (x => x.Key%2!=0)));
        }


        [TestMethod]
        public void UnitRmq_SkipWhile3Ctor()
        {
            var rm = new RankedMap<int,int> { Capacity=4 };
            var p1 = new SCG.KeyValuePair<int,int> (1,11);
            var p2 = new SCG.KeyValuePair<int,int> (2,22);

            Assert.AreEqual (0, SLE.Count (rm.SkipWhile ((p,i) => false)));
            Assert.AreEqual (0, SLE.Count (rm.SkipWhile ((p,i) => true)));

            rm.Add (p1.Key, p1.Value);
            rm.Add (p2.Key, p2.Value);

            Assert.IsTrue (SLE.SequenceEqual (new SCG.KeyValuePair<int,int>[] { p1,p2 }, rm.SkipWhile ((p,i) => false)));
            Assert.IsTrue (SLE.SequenceEqual (new SCG.KeyValuePair<int,int>[] { p2 },    rm.SkipWhile ((p,i) => p.Key%2!=0)));
            Assert.AreEqual (0, SLE.Count (rm.SkipWhile ((p,i) => true)));
        }

        [TestMethod]
        public void UnitRmq_SkipWhile3F()
        {
            var rm = new RankedMap<int,int> { Capacity=5 };
            var p1 = new SCG.KeyValuePair<int,int> (1,-1);
            var p2 = new SCG.KeyValuePair<int,int> (2,-2);
            var p3 = new SCG.KeyValuePair<int,int> (3,-3);

            Assert.AreEqual (0, SLE.Count (rm.Skip (0).SkipWhile ((p,i) => false)));
            Assert.AreEqual (0, SLE.Count (rm.Skip (0).SkipWhile ((p,i) => true)));

            rm.Add (p1.Key, p1.Value);

            Assert.AreEqual (0, SLE.Count (rm.Skip (0).SkipWhile ((p,i) => true)));
            Assert.IsTrue (SLE.SequenceEqual (new SCG.KeyValuePair<int,int>[] { p1 }, rm.Skip(0).SkipWhile ((p,i) => false)));

            rm.Add (p2.Key, p2.Value);
            rm.Add (p3.Key, p3.Value);

            Assert.IsTrue (SLE.SequenceEqual (new SCG.KeyValuePair<int,int>[] { p3 }, rm.Skip(0).SkipWhile ((p,i) => p.Key%2==0 || i<1)));
            Assert.AreEqual (0, SLE.Count (rm.Skip (0).SkipWhile ((p,i) => true)));
        }

        [TestMethod]
        public void UnitRmq_SkipWhile3R()
        {
            var rm = new RankedMap<int,int> { Capacity=4 };
            var p1 = new SCG.KeyValuePair<int,int> (1,-1);
            var p2 = new SCG.KeyValuePair<int,int> (2,-2);
            var p3 = new SCG.KeyValuePair<int,int> (3,-3);

            Assert.AreEqual (0, SLE.Count (rm.Reverse().SkipWhile ((p,i) => false)));
            Assert.AreEqual (0, SLE.Count (rm.Reverse().SkipWhile ((p,i) => true)));

            rm.Add (p1.Key, p1.Value);

            Assert.IsTrue (SLE.SequenceEqual (new SCG.KeyValuePair<int,int>[] { p1 }, rm.Reverse().SkipWhile ((p,i) => false)));
            Assert.AreEqual (0, SLE.Count (rm.Reverse().SkipWhile ((p,i) => true)));

            rm.Add (p2.Key, p2.Value);
            rm.Add (p3.Key, p3.Value);

            Assert.IsTrue (SLE.SequenceEqual (new SCG.KeyValuePair<int,int>[] { p1 }, rm.Reverse().SkipWhile ((p,i) => p.Key%2==0 || i<1)));
            Assert.AreEqual (0, SLE.Count (rm.Reverse().SkipWhile ((p,i) => true)));
        }

        #endregion

        #region Test enumeration (LINQ emulation)

        [TestMethod]
        public void UnitRmq_ReverseEmpty()
        {
            var rm = new RankedMap<int,int>();
            int actual = 0;

#if TEST_BCL
            foreach (var kv in Enumerable.Reverse (rm))
#else
            foreach (var kv in rm.Reverse())
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

        #region Test bonus (LINQ emulation)
#if ! TEST_BCL

        [TestMethod]
        public void UnitRmqx_oEtorGetEnumerator()
        {
            var ia = new int[] { 2,2,3,5,6,8,8 };
            var rm = new RankedMap<int,int> { Capacity=4 };
            foreach (var x in ia) rm.Add (x, -x);

            var oAble1 = (System.Collections.IEnumerable) rm;
            System.Collections.IEnumerator oEtor1 = oAble1.GetEnumerator();
            var oAble2 = (System.Collections.IEnumerable) oEtor1;
            System.Collections.IEnumerator oEtor2 = oAble2.GetEnumerator();

            int ix = 0;
            while (oEtor2.MoveNext())
            {
                var entry = (System.Collections.DictionaryEntry) oEtor2.Current;
                Assert.AreEqual (ia[ix], entry.Key);
                Assert.AreEqual (-ia[ix], entry.Value);
                ++ix;
            }
            Assert.AreEqual (ia.Length, ix);
        }

#endif
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
        public void CrashRmkq_First()
        {
            var rm = new RankedMap<int,int>();
            var keys = rm.Keys;
#if TEST_BCL
            var zz = Enumerable.First (rm.Keys);
#else
            int zz = keys.First();
#endif
        }

        [TestMethod]
        public void UnitRmkq_First()
        {
            var rm = new RankedMap<int,int> { Capacity=4 };
            var keys = rm.Keys;
            int n = 50;

            for (int ii = n; ii >= 1; --ii)
                rm.Add (ii, -ii);
#if TEST_BCL
            Assert.AreEqual (1, Enumerable.First (rm.Keys));
#else
            Assert.AreEqual (1, keys.First());
#endif
        }


        [TestMethod]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRmkq_Last_InvalidOperation()
        {
            var rm = new RankedMap<int,int>();
#if TEST_BCL
            var zz = Enumerable.Last (rm.Keys);
#else
            var zz = rm.Keys.Last();
#endif
        }

        [TestMethod]
        public void UnitRmkq_Last()
        {
            var rm = new RankedMap<int,int> { Capacity=5 };
            for (int ii = 0; ii <= 9; ++ii) rm.Add (ii,-ii);
#if TEST_BCL
            var key = Enumerable.Last (rm.Keys);
#else
            var key = rm.Keys.Last();
#endif
            Assert.AreEqual (9, key);
        }


        [TestMethod]
        public void UnitRmkq_Skip()
        {
            var rm = new RankedMap<int,int> { Capacity=5 };

            Assert.AreEqual (0, SLE.Count (rm.Keys.Skip (-1)));
            Assert.AreEqual (0, SLE.Count (rm.Keys.Skip (0)));
            Assert.AreEqual (0, SLE.Count (rm.Keys.Skip (1)));

            rm.Add (1,-1);
            rm.Add (2,-2);

            Assert.IsTrue (SLE.SequenceEqual (new int[] { 1,2 }, rm.Keys.Skip (-1)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { 1,2 }, rm.Keys.Skip (0)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { 2 },   rm.Keys.Skip (1)));
            Assert.AreEqual (0, SLE.Count (rm.Keys.Skip (2)));
            Assert.AreEqual (0, SLE.Count (rm.Keys.Skip (3)));

            Assert.IsTrue (SLE.SequenceEqual (new int[] { 1,2 }, rm.Keys.Skip (0).Skip (-1)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { 1,2 }, rm.Keys.Skip (0).Skip (0)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { 2 },   rm.Keys.Skip (0).Skip (1)));
            Assert.AreEqual (0, SLE.Count (rm.Skip(0).Skip (3)));

            Assert.IsTrue (SLE.SequenceEqual (new int[] { 2,1 }, rm.Keys.Reverse().Skip (-1)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { 2,1 }, rm.Keys.Reverse().Skip (0)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { 1 },   rm.Keys.Reverse().Skip (1)));
            Assert.AreEqual (0, SLE.Count (rm.Keys.Reverse().Skip (2)));
            Assert.AreEqual (0, SLE.Count (rm.Keys.Reverse().Skip (3)));
        }

        [TestMethod]
        public void UnitRmkq_SkipWhile2Ctor()
        {
            var rm = new RankedMap<int,int> { Capacity=4 };

            Assert.AreEqual (0, SLE.Count (rm.Keys.SkipWhile (x => false)));
            Assert.AreEqual (0, SLE.Count (rm.Keys.SkipWhile (x => true)));

            rm.Add (1,-1);
            rm.Add (2,-2);

            Assert.IsTrue (SLE.SequenceEqual (new int[] { 1,2 }, rm.Keys.SkipWhile (x => false)));
            Assert.AreEqual (0, SLE.Count (rm.Keys.SkipWhile (x => true)));

            rm.Add (3,-3);

            Assert.IsTrue (SLE.SequenceEqual (new int[] { 2,3 }, rm.Keys.SkipWhile (x => x%2!=0)));
        }

        [TestMethod]
        public void UnitRmkq_SkipWhile2F()
        {
            var rm = new RankedMap<int,int> { Capacity=4 };

            Assert.AreEqual (0, SLE.Count (rm.Keys.Skip(0).SkipWhile (x => false)));
            Assert.AreEqual (0, SLE.Count (rm.Keys.Skip(0).SkipWhile (x => true)));

            rm.Add (1,-1);

            Assert.AreEqual (0, SLE.Count (rm.Keys.Skip(0).SkipWhile (x => true)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { 1 }, rm.Keys.Skip(0).SkipWhile (x => false)));

            rm.Add (2,-2);
            rm.Add (3,-3);

            Assert.IsTrue (SLE.SequenceEqual (new int[] { 2,3 }, rm.Keys.Skip(0).SkipWhile (x => x%2!=0)));
        }

        [TestMethod]
        public void UnitRmkq_SkipWhile2R()
        {
            var rm = new RankedMap<int,int> { Capacity=4 };

            Assert.AreEqual (0, SLE.Count (rm.Keys.Reverse().SkipWhile (x => false)));
            Assert.AreEqual (0, SLE.Count (rm.Keys.Reverse().SkipWhile (x => true)));

            rm.Add (1,-1);

            Assert.AreEqual (0, SLE.Count (rm.Keys.Reverse().SkipWhile (x => true)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { 1 }, rm.Keys.Reverse().SkipWhile (x => false)));

            rm.Add (2,-2);
            rm.Add (3,-3);

            Assert.IsTrue (SLE.SequenceEqual (new int[] { 2,1 }, rm.Keys.Reverse().SkipWhile (x => x%2!=0)));
        }


        [TestMethod]
        public void UnitRmkq_SkipWhile3Ctor()
        {
            var rm = new RankedMap<int,int> { Capacity=4 };

            Assert.AreEqual (0, SLE.Count (rm.Keys.SkipWhile ((k,i) => false)));
            Assert.AreEqual (0, SLE.Count (rm.Keys.SkipWhile ((k,i) => true)));

            rm.Add (1,-1);
            rm.Add (2,-2);

            Assert.IsTrue (SLE.SequenceEqual (new int[] { 1,2 }, rm.Keys.SkipWhile ((k,i) => false)));
            Assert.AreEqual (0, SLE.Count (rm.Keys.SkipWhile ((k,i) => true)));

            rm.Add (3,-3);
            rm.Add (4,-4);
            rm.Add (5,-5);

            Assert.IsTrue (SLE.SequenceEqual (new int[] { 4,5 }, rm.Keys.SkipWhile ((k,i) => i<2 || k%2!=0)));
        }

        [TestMethod]
        public void UnitRmkq_SkipWhile3F()
        {
            var rm = new RankedMap<int,int> { Capacity=4 };

            Assert.AreEqual (0, SLE.Count (rm.Keys.Skip(0).SkipWhile ((k,i) => false)));
            Assert.AreEqual (0, SLE.Count (rm.Keys.Skip(0).SkipWhile ((k,i) => true)));

            rm.Add (1,-1);
            rm.Add (2,-2);

            Assert.AreEqual (0, SLE.Count (rm.Keys.Skip(0).SkipWhile ((k,i) => true)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { 1,2 }, rm.Keys.Skip(0).SkipWhile ((k,i) => false)));

            rm.Add (3,-3);
            rm.Add (4,-4);

            Assert.IsTrue (SLE.SequenceEqual (new int[] { 3,4 }, rm.Keys.Skip(0).SkipWhile ((k,i) => i<1 || k%2==0)));
        }

        [TestMethod]
        public void UnitRmkq_SkipWhile3R()
        {
            var rm = new RankedMap<int,int> { Capacity=4 };

            Assert.AreEqual (0, SLE.Count (rm.Keys.Reverse().SkipWhile ((k,i) => false)));
            Assert.AreEqual (0, SLE.Count (rm.Keys.Reverse().SkipWhile ((k,i) => true)));

            rm.Add (1,-1);
            rm.Add (2,-2);

            Assert.AreEqual (0, SLE.Count (rm.Keys.Reverse().SkipWhile ((k,i) => true)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { 2,1 }, rm.Keys.Reverse().SkipWhile ((k,i) => false)));

            rm.Add (3,-3);
            rm.Add (4,-4);

            Assert.IsTrue (SLE.SequenceEqual (new int[] { 2,1 }, rm.Keys.Reverse().SkipWhile ((k,i) => i<1 || k%2!=0)));
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
        public void UnitRmkq_Distinct()
        {
            var rm0 = new RankedMap<int,int>();
            var rm1 = new RankedMap<int,int> { Capacity=9 };
            int n = 100;

            for (int ii = 0; ii < n; ++ii)
                rm1.Add (ii/2, -ii);

            int a0 = 0, a1 = 0;
#if TEST_BCL
            foreach (var x in Enumerable.Distinct (rm0.Keys)) ++a0;
            foreach (var x in Enumerable.Distinct (rm1.Keys)) ++a1;
#else
            foreach (var x in rm0.Keys.Distinct()) ++a0;
            foreach (var x in rm1.Keys.Distinct()) ++a1;
#endif
            Assert.AreEqual (0, a0);
            Assert.AreEqual (n/2, a1);
        }


        [TestMethod]
#if ! TEST_BCL
        [ExpectedException (typeof (InvalidOperationException))]
#endif
        public void CrashRmkq_ReverseHotUpdate()
        {
            var rm = new RankedMap<int,int> { Capacity=6 };
            for (int ii = 9; ii > 0; --ii) rm.Add (ii, -ii);

#if TEST_BCL
            foreach (int k1 in Enumerable.Reverse (rm.Keys))
#else
            foreach (int k1 in rm.Keys.Reverse())
#endif
                if (k1 == 4)
                    rm.Clear();
        }

        [TestMethod]
        public void UnitRmkq_Reverse()
        {
            var rm0 = new RankedMap<int,int>();
            var rm1 = new RankedMap<int,int> { Capacity=4 };
            int n = 100;

            for (int i1 = 1; i1 <= n; ++i1)
                rm1.Add (i1/2, -i1);

            int a0 = 0, a1 = 0;
#if TEST_BCL
            foreach (var k0 in Enumerable.Reverse (rm0.Keys)) ++a0;
            foreach (var k1 in Enumerable.Reverse (rm1.Keys))
#else
            foreach (var k0 in rm0.Keys.Reverse()) ++a0;
            foreach (var k1 in rm1.Keys.Reverse())
#endif
            {
                Assert.AreEqual ((n-a1)/2, k1);
                ++a1;
            }

            Assert.AreEqual (0, a0);
            Assert.AreEqual (n, a1);
        }

        #endregion

        #region Test Keys bonus (LINQ emulation)
#if ! TEST_BCL

        [TestMethod]
        public void UnitRmkqx_oEtorGetEnumerator()
        {
            var ia = new int[] { 2,3,5,5,6,8 };
            var rm = new RankedMap<int,int> { Capacity=4 };
            foreach (var x in ia) rm.Add (x, -x);

            var oAble1 = (System.Collections.IEnumerable) rm.Keys;
            System.Collections.IEnumerator oEtor1 = oAble1.GetEnumerator();
            var oAble2 = (System.Collections.IEnumerable) oEtor1;
            System.Collections.IEnumerator oEtor2 = oAble2.GetEnumerator();

            int ix = 0;
            while (oEtor2.MoveNext())
            {
                var oKey = oEtor2.Current;
                Assert.AreEqual (ia[ix], oKey);
                ++ix;
            }
            Assert.AreEqual (ia.Length, ix);
        }

#endif
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
        public void UnitRmvq_ElementAt()
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


        [TestMethod]
        public void UnitRmvq_Skip()
        {
            var rm = new RankedMap<int,int> { Capacity=5 };

            Assert.AreEqual (0, SLE.Count (rm.Values.Skip (-1)));
            Assert.AreEqual (0, SLE.Count (rm.Values.Skip (0)));
            Assert.AreEqual (0, SLE.Count (rm.Values.Skip (1)));

            rm.Add (1,11);
            rm.Add (2,22);

            Assert.IsTrue (SLE.SequenceEqual (new int[] { 11,22 }, rm.Values.Skip (-1)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { 11,22 }, rm.Values.Skip (0)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { 22 },    rm.Values.Skip (1)));

            Assert.AreEqual (0, SLE.Count (rm.Values.Skip (0).Skip (2)));
            Assert.AreEqual (0, SLE.Count (rm.Values.Skip (0).Skip (3)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { 11, 22 }, rm.Values.Skip (0).Skip (-1)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { 11, 22 }, rm.Values.Skip (0).Skip (0)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { 22 },     rm.Values.Skip (0).Skip (1)));

            Assert.IsTrue (SLE.SequenceEqual (new int[] { 22 }, rm.Values.Skip (1).Skip (-1)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { 22 }, rm.Values.Skip (1).Skip (0)));

            Assert.AreEqual (0, SLE.Count (rm.Values.Skip (2).Skip (0)));
            Assert.AreEqual (0, SLE.Count (rm.Values.Skip (2).Skip (1)));

            Assert.IsTrue (SLE.SequenceEqual (new int[] { 22,11 }, rm.Values.Reverse().Skip (-1)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { 22,11 }, rm.Values.Reverse().Skip (0)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { 11 },    rm.Values.Reverse().Skip (1)));
            Assert.AreEqual (0, SLE.Count (rm.Values.Reverse().Skip (2)));
            Assert.AreEqual (0, SLE.Count (rm.Values.Reverse().Skip (3)));
        }


        [TestMethod]
        public void UnitRmvq_SkipWhile2Ctor()
        {
            var rm = new RankedMap<int,int> { Capacity=4 };

            Assert.AreEqual (0, SLE.Count (rm.Values.SkipWhile (x => false)));
            Assert.AreEqual (0, SLE.Count (rm.Values.SkipWhile (x => true)));

            rm.Add (1,-1);
            rm.Add (2,-2);

            Assert.IsTrue (SLE.SequenceEqual (new int[] { -1,-2 }, rm.Values.SkipWhile (x => false)));
            Assert.AreEqual (0, SLE.Count (rm.Values.SkipWhile (x => true)));

            rm.Add (3,-3);

            Assert.IsTrue (SLE.SequenceEqual (new int[] { -2,-3 }, rm.Values.SkipWhile (x => x%2!=0)));
        }

        [TestMethod]
        public void UnitRmvq_SkipWhile2F()
        {
            var rm = new RankedMap<int,int> { Capacity=4 };

            Assert.AreEqual (0, SLE.Count (rm.Values.Skip(0).SkipWhile (x => false)));
            Assert.AreEqual (0, SLE.Count (rm.Values.Skip(0).SkipWhile (x => true)));

            rm.Add (1,-1);

            Assert.AreEqual (0, SLE.Count (rm.Values.Skip(0).SkipWhile (x => true)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { -1 }, rm.Values.Skip(0).SkipWhile (x => false)));

            rm.Add (2,-2);
            rm.Add (3,-3);

            Assert.IsTrue (SLE.SequenceEqual (new int[] { -2,-3 }, rm.Values.Skip(0).SkipWhile (x => x%2!=0)));
        }

        [TestMethod]
        public void UnitRmvq_SkipWhile2R()
        {
            var rm = new RankedMap<int,int> { Capacity=4 };

            Assert.AreEqual (0, SLE.Count (rm.Values.Reverse().SkipWhile (x => false)));
            Assert.AreEqual (0, SLE.Count (rm.Values.Reverse().SkipWhile (x => true)));

            rm.Add (1,-1);

            Assert.AreEqual (0, SLE.Count (rm.Values.Reverse().SkipWhile (x => true)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { -1 }, rm.Values.Reverse().SkipWhile (x => false)));

            rm.Add (2,-2);
            rm.Add (3,-3);

            Assert.IsTrue (SLE.SequenceEqual (new int[] { -2,-1 }, rm.Values.Reverse().SkipWhile (x => x%2!=0)));
        }


        [TestMethod]
        public void UnitRmvq_SkipWhile3Ctor()
        {
            var rm = new RankedMap<int,int> { Capacity=4 };

            Assert.AreEqual (0, SLE.Count (rm.Values.SkipWhile ((v,i) => false)));
            Assert.AreEqual (0, SLE.Count (rm.Values.SkipWhile ((v,i) => true)));

            rm.Add (1,-1);
            rm.Add (2,-2);

            Assert.IsTrue (SLE.SequenceEqual (new int[] { -1,-2 }, rm.Values.SkipWhile ((v,i) => false)));
            Assert.AreEqual (0, SLE.Count (rm.Values.SkipWhile ((v,i) => true)));

            rm.Add (3,-3);
            rm.Add (4,-4);
            rm.Add (5,-5);

            Assert.IsTrue (SLE.SequenceEqual (new int[] { -4,-5 }, rm.Values.SkipWhile ((v,i) => v>-3 || i%2==0)));
        }

        [TestMethod]
        public void UnitRmvq_SkipWhile3F()
        {
            var rm = new RankedMap<int,int> { Capacity=4 };

            Assert.AreEqual (0, SLE.Count (rm.Values.Skip(0).SkipWhile ((v,i) => false)));
            Assert.AreEqual (0, SLE.Count (rm.Values.Skip(0).SkipWhile ((v,i) => true)));

            rm.Add (1,-1);
            rm.Add (2,-2);

            Assert.AreEqual (0, SLE.Count (rm.Values.Skip(0).SkipWhile ((v,i) => true)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { -1,-2 }, rm.Values.Skip(0).SkipWhile ((v,i) => false)));

            rm.Add (3,-3);
            rm.Add (4,-4);

            Assert.IsTrue (SLE.SequenceEqual (new int[] { -3,-4 }, rm.Values.Skip(0).SkipWhile ((v,i) => v>-2 || i%2!=0)));
        }

        [TestMethod]
        public void UnitRmvq_SkipWhile3R()
        {
            var rm = new RankedMap<int,int> { Capacity=4 };

            Assert.AreEqual (0, SLE.Count (rm.Values.Reverse().SkipWhile ((v,i) => false)));
            Assert.AreEqual (0, SLE.Count (rm.Values.Reverse().SkipWhile ((v,i) => true)));

            rm.Add (1,-1);
            rm.Add (2,-2);

            Assert.AreEqual (0, SLE.Count (rm.Values.Reverse().SkipWhile ((v,i) => true)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { -2,-1 }, rm.Values.Reverse().SkipWhile ((v,i) => false)));

            rm.Add (3,-3);
            rm.Add (4,-4);

            Assert.IsTrue (SLE.SequenceEqual (new int[] { -2,-1 }, rm.Values.Reverse().SkipWhile ((v,i) => v<-3 || i%2!=0)));
        }

        #endregion

        #region Test Values enumeration (LINQ emulation)

        [TestMethod]
#if ! TEST_BCL
        [ExpectedException (typeof (InvalidOperationException))]
#endif
        public void CrashRmvq_ReverseHotUpdate()
        {
            var rm = new RankedMap<int,int> { Capacity=7 };
            for (int ii = 9; ii > 0; --ii) rm.Add (ii, -ii);

#if TEST_BCL
            foreach (int v1 in Enumerable.Reverse (rm.Values))
#else
            foreach (int v1 in rm.Values.Reverse())
#endif
                if (v1 == -4)
                    rm.Clear();
        }

        [TestMethod]
        public void UnitRmvq_Reverse()
        {
            var rm0 = new RankedMap<int,int>();
            var rm1 = new RankedMap<int,int> { Capacity=4 };
            int n = 120;

            for (int i1 = 1; i1 <= n; ++i1)
                rm1.Add (i1, -i1);

            int a0 = 0, a1 = 0;
#if TEST_BCL
            foreach (var v0 in Enumerable.Reverse (rm0.Values)) ++a0;
            foreach (var v1 in Enumerable.Reverse (rm1.Values))
#else
            foreach (var v0 in rm0.Values.Reverse()) ++a0;
            foreach (var v1 in rm1.Values.Reverse())
#endif
            {
                Assert.AreEqual (-(n-a1), v1);
                ++a1;
            }

            Assert.AreEqual (0, a0);
            Assert.AreEqual (n, a1);
        }

        #endregion

        #region Test Values bonus (LINQ emulation)
#if ! TEST_BCL

        [TestMethod]
        public void UnitRmvqx_oEtorGetEnumerator()
        {
            var ia = new int[] { 2,3,3,5,6,8,8 };
            var rm = new RankedMap<int,int> { Capacity=4 };
            foreach (var x in ia) rm.Add (x, -x);

            var oAble1 = (System.Collections.IEnumerable) rm.Values;
            System.Collections.IEnumerator oEtor1 = oAble1.GetEnumerator();
            var oAble2 = (System.Collections.IEnumerable) oEtor1;
            System.Collections.IEnumerator oEtor2 = oAble2.GetEnumerator();

            int ix = 0;
            while (oEtor2.MoveNext())
            {
                var oValue = oEtor2.Current;
                Assert.AreEqual (-ia[ix], oValue);
                ++ix;
            }
            Assert.AreEqual (ia.Length, ix);
        }

#endif
        #endregion
    }
}
