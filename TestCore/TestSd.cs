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
    }
}
