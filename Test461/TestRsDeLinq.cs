//
// File: TestRsDeLinq.cs
// Purpose: Exercise LINQ API optimized with instance methods.
//

using System;
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
        public void UnitRsq_Last()
        {
            Setup (4);
            int n = 99;

            for (int ii = n; ii >= 0; --ii) setI.Add (ii);
#if TEST_BCL
            Assert.AreEqual (n, Enumerable.Last (setI));
#else
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

        #endregion
    }
}
