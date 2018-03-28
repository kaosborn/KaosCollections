//
// File: TestRsDeLinq.cs
// Purpose: Exercise LINQ API optimized with instance methods.
//

using System;
using SLE=System.Linq.Enumerable;
#if TEST_BCL
using System.Linq;
#endif
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kaos.Test.Collections
{
    public partial class TestBtree
    {
        #region Test methods (LINQ emulation)

        [TestMethod]
        public void UnitRsq_Contains()
        {
            Setup();
#if TEST_BCL
            Assert.IsFalse (Enumerable.Contains (setS, "qq"));
#else
            Assert.IsFalse (setS.Contains ("qq"));
#endif
            setS.Add ("aa");
            setS.Add ("xx");
            setS.Add ("mm");
            setS.Add (null);
#if TEST_BCL
            Assert.IsTrue (Enumerable.Contains (setS, null));
            Assert.IsTrue (Enumerable.Contains (setS, "mm"));
            Assert.IsFalse (Enumerable.Contains (setS, "bb"));
#else
            Assert.IsTrue (setS.Contains (null));
            Assert.IsTrue (setS.Contains ("mm"));
            Assert.IsFalse (setS.Contains ("bb"));
#endif
        }

#if ! TEST_BCL
        [TestMethod]
        public void StressRsq_Contains()
        {
            Setup (4);

            for (int ii = 1; ii <= 9999; ii+=2)
                setI.Add (ii);

            Assert.IsTrue (setI.Contains (1));
            Assert.IsFalse (setI.Contains (0));
            Assert.IsTrue (setI.Contains (499));
            Assert.IsFalse (setI.Contains (500));
            Assert.IsTrue (setI.Contains (9999));
            Assert.IsFalse (setI.Contains (10000));
        }
#endif

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRsq_ElementAt1_ArgumentOutOfRange()
        {
            Setup();
            setI.Add (4);
#if TEST_BCL
            int zz = Enumerable.ElementAt (setI, -1);
#else
            int zz = setI.ElementAt (-1);
#endif
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRsq_ElementAt2_ArgumentOutOfRange()
        {
            Setup();
#if TEST_BCL
            int zz = Enumerable.ElementAt (setI, 0);
#else
            int zz = setI.ElementAt (0);
#endif
        }

        [TestMethod]
        public void UnitRsq_ElementAt()
        {
            Setup (4);
            int n = 800;

            for (int ii = 0; ii <= n; ii += 2)
                setI.Add (ii);

            for (int ix = 0; ix <= n/2; ++ix)
            {
#if TEST_BCL
                Assert.AreEqual (ix*2, Enumerable.ElementAt (setI, ix));
#else
                Assert.AreEqual (ix*2, setI.ElementAt (ix));
#endif
            }
        }


        [TestMethod]
        public void UnitRsq_ElementAtOrDefault()
        {
            Setup();
#if TEST_BCL
            string keyN = Enumerable.ElementAtOrDefault (setS, -1);
            int key0 = Enumerable.ElementAtOrDefault (setI, 0);
#else
            string keyN = setS.ElementAtOrDefault (-1);
            int key0 = setI.ElementAtOrDefault (0);
#endif
            Assert.AreEqual (default (string), keyN);
            Assert.AreEqual (default (int), key0);

            setS.Add ("nein");
#if TEST_BCL
            string keyZ = Enumerable.ElementAtOrDefault (setS, 0);
            string key1 = Enumerable.ElementAtOrDefault (setS, 1);
#else
            string keyZ = setS.ElementAtOrDefault (0);
            string key1 = setS.ElementAtOrDefault (1);
#endif
            Assert.AreEqual ("nein", keyZ);
            Assert.AreEqual (default (string), key1);
        }


        [TestMethod]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRsq_First_InvalidOperation()
        {
            Setup();
#if TEST_BCL
            var zz = Enumerable.First (setI);
#else
            var zz = setI.First();
#endif
        }

        [TestMethod]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRsq_Last_InvalidOperation()
        {
            Setup();
#if TEST_BCL
            var zz = Enumerable.Last (setI);
#else
            var zz = setI.Last();
#endif
        }

        [TestMethod]
        public void UnitRsq_FirstLast()
        {
            Setup (4);
            int n = 99;

            for (int ii = n; ii >= 1; --ii) setI.Add (ii);
#if TEST_BCL
            Assert.AreEqual (1, Enumerable.First (setI));
            Assert.AreEqual (n, Enumerable.Last (setI));
#else
            Assert.AreEqual (1, setI.First());
            Assert.AreEqual (n, setI.Last());
#endif
        }

        #endregion

        #region Test enumeration (LINQ emulation)

        [TestMethod]
        public void UnitRsq_Reverse()
        {
            Setup (5);
            int n = 500;

            for (int ii=1; ii <= n; ++ii)
                setI.Add (ii);

            int a0 = 0, a1 = 0;
#if TEST_BCL
            foreach (var k0 in Enumerable.Reverse (setS)) ++a0;
            foreach (var k1 in Enumerable.Reverse (setI))
#else
            foreach (var k0 in setS.Reverse()) ++a0;
            foreach (var k1 in setI.Reverse())
#endif
            {
                Assert.AreEqual (n-a1, k1);
                ++a1;
            }
            Assert.AreEqual (0, a0);
            Assert.AreEqual (n, a1);
        }


        [TestMethod]
        public void UnitRsq_Skip()
        {
            Setup (4);

            Assert.AreEqual (0, SLE.Count (setI.Skip (-1)));
            Assert.AreEqual (0, SLE.Count (setI.Skip (0)));
            Assert.AreEqual (0, SLE.Count (setI.Skip (1)));
            Assert.AreEqual (0, SLE.Count (setI.Skip(0).Skip (-1)));
            Assert.AreEqual (0, SLE.Count (setI.Skip(0).Skip (0)));
            Assert.AreEqual (0, SLE.Count (setI.Skip(0).Skip (1)));
            Assert.AreEqual (0, SLE.Count (setI.Reverse().Skip (-1)));
            Assert.AreEqual (0, SLE.Count (setI.Reverse().Skip (0)));
            Assert.AreEqual (0, SLE.Count (setI.Reverse().Skip (1)));

            setI.Add (1);

            Assert.IsTrue (SLE.SequenceEqual (new int[] { 1 }, setI.Skip (-1)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { 1 }, setI.Skip (0)));
            Assert.AreEqual (0, SLE.Count (setI.Skip (1)));
            Assert.AreEqual (0, SLE.Count (setI.Skip (2)));
            Assert.AreEqual (0, SLE.Count (setI.Skip (1).Skip (1).Skip (1)));

            Assert.IsTrue (SLE.SequenceEqual (new int[] { 1 }, setI.Skip (0).Skip (-1)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { 1 }, setI.Skip (0).Skip (0)));
            Assert.AreEqual (0, SLE.Count (setI.Skip(0).Skip (1)));
            Assert.AreEqual (0, SLE.Count (setI.Skip(0).Skip (2)));

            Assert.IsTrue (SLE.SequenceEqual (new int[] { 1 }, setI.Reverse().Skip (-1)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { 1 }, setI.Reverse().Skip (0)));
            Assert.AreEqual (0, SLE.Count (setI.Reverse().Skip (1)));
            Assert.AreEqual (0, SLE.Count (setI.Reverse().Skip (2)));
            Assert.AreEqual (0, SLE.Count (setI.Reverse().Skip (1).Skip (1)));

            setI.Add (2);
            setI.Add (3);

            Assert.IsTrue (SLE.SequenceEqual (new int[] { 1,2,3 }, setI.Skip (-1)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { 1,2,3 }, setI.Skip (0)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { 2,3 },   setI.Skip (1)));
            Assert.AreEqual (0, SLE.Count (setI.Skip (3)));

            Assert.IsTrue (SLE.SequenceEqual (new int[] { 2,3 }, setI.Skip(0).Skip (1)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { 3 },   setI.Skip(1).Skip (1)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { 3 },   setI.Skip(0).Skip (2)));
            Assert.AreEqual (0, SLE.Count (setI.Skip(0).Skip (3)));

            Assert.IsTrue (SLE.SequenceEqual (new int[] { 3,2,1 }, setI.Reverse().Skip (-1)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { 3,2,1 }, setI.Reverse().Skip (0)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { 2,1 },   setI.Reverse().Skip (1)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { 1 },     setI.Reverse().Skip (1).Skip (1)));
            Assert.AreEqual (0, SLE.Count (setI.Reverse().Skip (3)));

            for (int i = 4; i <= 50; ++i)
                setI.Add (i);

            Assert.IsTrue (SLE.SequenceEqual (new int[] { 46,47,48,49,50 }, setI.Skip(30).Skip (15)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { 5,4,3,2,1 }, setI.Reverse().Skip(30).Skip (15)));
        }

        [TestMethod]
        public void StressRsq_SkipF()
        {
            Setup (4);
            int n = 20;

            for (int ix = 0; ix < n; ++ix)
                setI.Add (n + ix);

            for (int s1 = 0; s1 <= n; ++s1)
                for (int s2 = 0; s2 <= n-s1; ++s2)
                {
                    int e0 = n + s1+s2;
                    foreach (var a0 in setI.Skip (s1).Skip (s2))
                    {
                        Assert.AreEqual (e0, a0);
                        ++e0;
                    }
                    Assert.AreEqual (n + n, e0);
                }
        }

        [TestMethod]
        public void StressRsq_SkipR()
        {
            Setup (4);
            int n = 20;

            for (int ix = 0; ix < n; ++ix)
                setI.Add (n + ix);

            for (int s1 = 0; s1 <= n; ++s1)
            {
                int e0 = n + n - s1;
                foreach (var a0 in setI.Reverse().Skip (s1))
                {
                    --e0;
                    Assert.AreEqual (e0, a0);
                }
                Assert.AreEqual (n, e0);
            }
        }


        [TestMethod]
        public void UnitRsq_SkipWhile2Ctor()
        {
            Setup (5);

            Assert.AreEqual (0, SLE.Count (setI.SkipWhile (x => false)));
            Assert.AreEqual (0, SLE.Count (setI.SkipWhile (x => true)));

            setI.Add (1);

            Assert.AreEqual (0, SLE.Count (setI.SkipWhile (x => true)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { 1 }, setI.SkipWhile (x => false)));

            setI.Add (2);
            setI.Add (3);

            Assert.IsTrue (SLE.SequenceEqual (new int[] { 2,3 }, setI.SkipWhile (x => x%2!=0)));
        }

        [TestMethod]
        public void UnitRsq_SkipWhile2F()
        {
            Setup (5);

            Assert.AreEqual (0, SLE.Count (setI.Skip(0).SkipWhile (x => false)));
            Assert.AreEqual (0, SLE.Count (setI.Skip(0).SkipWhile (x => true)));
            Assert.AreEqual (0, SLE.Count (setI.SkipWhile (x => true).SkipWhile (x => true)));

            setI.Add (1);

            Assert.AreEqual (0, SLE.Count (setI.Skip(0).SkipWhile (x => true)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { 1 }, setI.Skip(0).SkipWhile (x => false)));

            setI.Add (2);
            setI.Add (3);

            Assert.IsTrue (SLE.SequenceEqual (new int[] { 2,3 }, setI.Skip(0).SkipWhile (x => x%2!=0)));

            for (int i = 4; i < 50; ++i)
                setI.Add (i);

            Assert.IsTrue (SLE.SequenceEqual (new int[] { 45,46,47,48,49 }, setI.Skip(30).SkipWhile (x => x<45)));
        }

        [TestMethod]
        public void UnitRsq_SkipWhile2R()
        {
            Setup (5);

            Assert.AreEqual (0, SLE.Count (setI.Reverse().SkipWhile (x => false)));
            Assert.AreEqual (0, SLE.Count (setI.Reverse().SkipWhile (x => true)));
            Assert.AreEqual (0, SLE.Count (setI.Reverse().SkipWhile (x => true).SkipWhile (x => true)));

            setI.Add (1);

            Assert.AreEqual (0, SLE.Count (setI.Reverse ().SkipWhile (x => true)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { 1 }, setI.Reverse().SkipWhile (x => false)));

            setI.Add (2);
            setI.Add (3);

            Assert.IsTrue (SLE.SequenceEqual (new int[] { 2,1 }, setI.Reverse().SkipWhile (x => x%2!=0)));

            for (int i = 4; i < 50; ++i)
                setI.Add (i);

            Assert.IsTrue (SLE.SequenceEqual (new int[] { 5,4,3,2,1 }, setI.Reverse().Skip(20).SkipWhile (x => x>5)));
        }


        [TestMethod]
        public void UnitRsq_SkipWhile3Ctor()
        {
            Setup (5);

            Assert.AreEqual (0, SLE.Count (setI.SkipWhile ((x,i) => false)));
            Assert.AreEqual (0, SLE.Count (setI.SkipWhile ((x,i) => true)));

            setI.Add (1);

            Assert.AreEqual (0, SLE.Count (setI.SkipWhile ((x,i) => true)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { 1 }, setI.SkipWhile ((x,i) => false)));

            setI.Add (2);
            setI.Add (3);
            setI.Add (4);

            Assert.IsTrue (SLE.SequenceEqual (new int[] { 4 }, setI.SkipWhile ((x,i) => x%2!=0 || i<3)));
        }

        [TestMethod]
        public void UnitRsq_SkipWhile3F()
        {
            Setup (5);

            Assert.AreEqual (0, SLE.Count (setI.Skip(0).SkipWhile ((x,i) => false)));
            Assert.AreEqual (0, SLE.Count (setI.Skip(0).SkipWhile ((x,i) => true)));
            Assert.AreEqual (0, SLE.Count (setI.SkipWhile ((x,i) => true).SkipWhile ((x,i) => true)));

            setI.Add (1);

            Assert.AreEqual (0, SLE.Count (setI.Skip(0).SkipWhile ((x,i) => true)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { 1 }, setI.Skip(0).SkipWhile ((x,i) => false)));

            setI.Add (2);
            setI.Add (3);

            Assert.IsTrue (SLE.SequenceEqual (new int[] { 2,3 }, setI.Skip(0).SkipWhile ((x,i) => x%2!=0)));

            for (int i = 4; i < 50; ++i)
                setI.Add (i);

            Assert.IsTrue (SLE.SequenceEqual (new int[] { 48,49 }, setI.Skip(30).SkipWhile ((x,i) => x%3!=0 || i<15)));
        }

        [TestMethod]
        public void UnitRsq_SkipWhile3R()
        {
            Setup (5);

            Assert.AreEqual (0, SLE.Count (setI.Reverse().SkipWhile ((x,i) => false)));
            Assert.AreEqual (0, SLE.Count (setI.Reverse().SkipWhile ((x,i) => true)));
            Assert.AreEqual (0, SLE.Count (setI.Reverse().SkipWhile ((x,i) => true).SkipWhile ((x,i) => true)));

            setI.Add (1);

            Assert.AreEqual (0, SLE.Count (setI.Reverse ().SkipWhile ((x,i) => true)));
            Assert.IsTrue (SLE.SequenceEqual (new int[] { 1 }, setI.Reverse().SkipWhile ((x,i) => false)));

            setI.Add (2);
            setI.Add (3);

            Assert.IsTrue (SLE.SequenceEqual (new int[] { 2,1 }, setI.Reverse().SkipWhile ((x,i) => x%2!=0)));

            for (int i = 4; i < 50; ++i)
                setI.Add (i);

            Assert.IsTrue (SLE.SequenceEqual (new int[] { 3,2,1 }, setI.Reverse().Skip(20).SkipWhile ((x,i) => x%3!=0 || i<24)));
        }


        [TestMethod]
        public void StressRsq_SkipWhile()
        {
            Setup (5);

            for (int x1 = 0; x1 < 20; ++x1)
            {
                setI.Clear();
                for (int x3 = 0; x3 < x1; ++x3)
                    setI.Add (x3);

                System.Collections.Generic.IEnumerable<int> q0 = setI.SkipWhile (x=>false);

                Assert.AreEqual (x1, SLE.Count (q0));
            }
        }

        #endregion

        #region Test bonus (LINQ emulation)
#if ! TEST_BCL

        [TestMethod]
        public void UnitRsqx_oEtorGetEnumerator()
        {
            Setup();
            var ia = new int[] { 2,3,5,6,8 };
            foreach (var x in ia) setI.Add (x);

            var oAble1 = (System.Collections.IEnumerable) setI;
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
