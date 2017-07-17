using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
#if TEST_SORTEDDICTIONARY
using System.Collections.Generic;
#else
using Kaos.Collections;
#endif

namespace CollectionsTest
{
    public partial class Test_Btree
    {
        [TestMethod]
        public void UnitSd_Ctor1()
        {
            Setup();
            Assert.AreEqual (0, setTS1.Count);
        }


        [TestMethod]
        public void UnitSd_Ctor6()
        {
#if TEST_SORTED_DICTIONARY
            var set1 = new SortedSet<int> (keys);
            var set3 = new SortedSet<int> (iVals3);
#else
            var set1 = new RankedSet<int> (keys);
            var set3 = new RankedSet<int> (iVals3);
#endif
            Assert.AreEqual (keys.Length, set1.Count);
            Assert.AreEqual (4, set3.Count);
        }

        #region Test properties

        [TestMethod]
        public void UnitSd_Max()
        {
            Setup (5);
            Assert.AreEqual (default (int), setI.Max);

            setI.Add (2);
            setI.Add (1);
            Assert.AreEqual (2, setI.Max);

            for (int ii = 996; ii >= 3; --ii)
                setI.Add (ii);
            Assert.AreEqual (996, setI.Max);
        }


        [TestMethod]
        public void UnitSd_Min()
        {
            Setup (4);
            Assert.AreEqual (default (int), setI.Min);

            setI.Add (97);
            setI.Add (98);
            Assert.AreEqual (97, setI.Min);

            for (int ii = 96; ii >= 1; --ii)
                setI.Add (ii);
            Assert.AreEqual (1, setI.Min);
        }

        #endregion

        [TestMethod]
        public void UnitSd_AddNull()
        {
            Setup();

            // SortedSet allows null key (but SortedDictionary does not).
            setTS1.Add (null);
        }


        [TestMethod]
        public void UnitSd_Add()
        {
            bool isOk;
            Setup();

            setS.Add ("aa");
            setS.Add ("cc");
            isOk = setS.Add ("bb");
            Assert.IsTrue (isOk);

            // SortedSet ignores duplicates (but SortedDictionary does not).
            isOk = setS.Add ("cc");
            Assert.IsFalse (isOk);

            Assert.AreEqual (3, setS.Count);
        }


        [TestMethod]
        public void UnitSd_ContainsNull()
        {
            Setup();

            // SortedSet allows null arg (but SortedDictionary does not).
            setS.Contains (null);
        }


        [TestMethod]
        public void UnitSd_Contains()
        {
            Setup();

            setS.Add ("aa");
            setS.Add ("xx");
            setS.Add ("mm");

            Assert.IsTrue (setS.Contains ("mm"));
            Assert.IsFalse (setS.Contains ("bb"));
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashSd_CopyTo_ArgumentNull()
        {
            Setup();
            string[] nada = null;
            setS.CopyTo (nada, 0, 1);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashSd_CopyTo_ArgumentOutOfRange1()
        {
            var s1 = new string[1];
            Setup();
            setS.Add ("ee");
            setS.CopyTo (s1, -1);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashSd_CopyTo_ArgumentOutOfRange2()
        {
            var s1 = new string[1];
            Setup();
            setS.Add ("ee");
            setS.CopyTo (s1, 0, -1);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashSd_CopyTo_Argument()
        {
            var s1 = new string[1];
            Setup();
            setS.Add ("xx");
            setS.Add ("mm");

            setS.CopyTo (s1, 0, 2);
        }


        [TestMethod]
        public void UnitSd_CopyTo1()
        {
            var s1 = new string[3];
            Setup();
            setS.Add ("xx");
            setS.Add ("mm");

            setS.CopyTo (s1, 1, 2);
        }


        [TestMethod]
        public void UnitSd_CopyTo2()
        {
            var i3 = new TS1[3];
            Setup();

            setTS1.Add (new TS1 (4));
            setTS1.Add (new TS1 (2));

            setTS1.CopyTo (i3, 1, 2);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashSd_CopyToOb_ArgumentNull()
        {
            Setup();
            var setSo = (System.Collections.ICollection) setS;
            object[] nada = null;
            setSo.CopyTo (nada, 0);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashSd_CopyToOb_Argument1()
        {
            Setup();
            var setSo = (System.Collections.ICollection) setS;
            var multi = new string[1,2];
            var multiOb = (System.Collections.ICollection) multi;
            setSo.CopyTo (multi, 0);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashSd_CopyToOb_Argument2()
        {
            Setup();
            var setSo = (System.Collections.ICollection) setS;
            var a11 = Array.CreateInstance (typeof (int), new int[]{1}, new int[]{1});

            setSo.CopyTo (a11, 1);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashSd_CopyToOb_ArgumentOutOfRange()
        {
            var s1 = new string[1];
            Setup();
            setS.Add ("ee");
            var setSo = (System.Collections.ICollection) setS;

            setSo.CopyTo (s1, -1);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashSd_CopyToOb_Argument3()
        {
            var s1 = new string[1];
            Setup();
            setS.Add ("ee");
            setS.Add ("bb");
            var setSo = (System.Collections.ICollection) setS;

            setSo.CopyTo (s1, 0);
        }


        [TestMethod]
        public void UnitSd_Remove()
        {
            bool isOk;
            Setup (4);

            foreach (int x1 in keys)
                setI.Add (x1);
            int count0 = setI.Count;

            isOk = setI.Remove (18);
            Assert.IsTrue (isOk);
            Assert.AreEqual (count0-1, setI.Count);

            isOk = setI.Remove (18);
            Assert.IsFalse (isOk);
            Assert.AreEqual (count0-1, setI.Count);
        }


        [TestMethod]
        public void UnitSd_ReverseEmpty()
        {
            int total = 0;
            Setup (5);

            foreach (var countdown in setI.Reverse())
               ++total;

            Assert.AreEqual (0, total);
        }


        [TestMethod]
        public void UnitSd_Reverse()
        {
            int expected = 500;
            Setup (5);
            for (int ii=1; ii <= expected; ++ii)
                setI.Add (ii);

            foreach (var actual in setI.Reverse())
            {
                Assert.AreEqual (expected, actual);
                --expected;
            }
            Assert.AreEqual (0, expected);
        }

        #region ISet tests

        [TestMethod]
        public void UnitSd_ExceptWith()
        {
            Setup();
            foreach (var v1 in keys)
                setI.Add (v1);

            var list1 = new System.Collections.Generic.List<int> (keys);
            var list2 = new System.Collections.Generic.List<int> (iVals2);

            setI.ExceptWith (iVals2);

            int expectedCount = keys.Length;
            foreach (int i2 in iVals2)
                if (list1.Contains (i2))
                    --expectedCount;

            foreach (int ii in setI)
                if (list2.Contains (ii))
                    Assert.Fail ("Unexpected = " + ii);

            Assert.AreEqual (expectedCount, setI.Count);
        }

        #endregion
    }
}
