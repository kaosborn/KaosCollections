//
// Library: KaosCollections
// File:    TestRs.cs
//

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
#if TEST_BCL
using System.Collections.Generic;
#else
using Kaos.Collections;
#endif

namespace CollectionsTest
{
    public partial class Test_Btree
    {
        #region Test constructors

        [TestMethod]
        public void UnitRs_Inheritance()
        {
            Setup();
            setI.Add (42); setI.Add (21); setI.Add (63);

            var toISetI = setI as System.Collections.Generic.ISet<int>;
            var toIColI = setI as System.Collections.Generic.ICollection<int>;
            var toIEnuI = setI as System.Collections.Generic.IEnumerable<int>;
            var toIEnuO = setI as System.Collections.IEnumerable;
            var toIColO = setI as System.Collections.ICollection;
            var toIRocI = setI as System.Collections.Generic.IReadOnlyCollection<int>;

            Assert.IsNotNull (toISetI);
            Assert.IsNotNull (toIColI);
            Assert.IsNotNull (toIEnuI);
            Assert.IsNotNull (toIEnuO);
            Assert.IsNotNull (toIColO);
            Assert.IsNotNull (toIRocI);

            int ObjEnumCount = 0;
            for (var oe = toIEnuO.GetEnumerator(); oe.MoveNext(); )
                ++ObjEnumCount;

            Assert.AreEqual (3, toISetI.Count);
            Assert.AreEqual (3, toIColI.Count);
            Assert.AreEqual (3, toIEnuI.Count());
            Assert.AreEqual (3, ObjEnumCount);
            Assert.AreEqual (3, toIColO.Count);
            Assert.AreEqual (3, toIRocI.Count);
        }


        [TestMethod]
        public void UnitRs_Ctor0A1()
        {
            Setup();
            Assert.AreEqual (0, setTS1.Count);
        }

        [TestMethod]
#if TEST_BCL
        [ExpectedException (typeof (ArgumentException))]
#else
        [ExpectedException (typeof (InvalidOperationException))]
#endif
        public void UnitRs_Ctor1B_InvalidOperation()
        {
#if TEST_BCL
            var sansComparer = new SortedSet<Person>();
#else
            var sansComparer = new RankedSet<Person>();
#endif
            foreach (var name in Person.names)
                sansComparer.Add (new Person (name));
        }

        [TestMethod]
        public void UnitRs_Ctor1B1()
        {
            Setup();

            foreach (var name in Person.names) personSet.Add (new Person (name));
            personSet.Add (null);
            personSet.Add (new Person ("Zed"));

            Assert.AreEqual (Person.names.Length+2, personSet.Count);
        }


        [TestMethod]
        public void UnitRs_Ctor1B()
        {
#if TEST_BCL
            var set1 = new SortedSet<int> (iVals1);
            var set3 = new SortedSet<int> (iVals3);
#else
            var set1 = new RankedSet<int> (iVals1);
            var set3 = new RankedSet<int> (iVals3);
#endif
            Assert.AreEqual (iVals1.Length, set1.Count);
            Assert.AreEqual (4, set3.Count);
        }


        [TestMethod]
        public void UnitRs_Ctor2A()
        {
            var pa = new System.Collections.Generic.List<Person>();
            foreach (var name in Person.names) pa.Add (new Person (name));

#if TEST_BCL
            var people = new SortedSet<Person> (pa, new PersonComparer());
#else
            var people = new RankedSet<Person> (pa, new PersonComparer());
#endif
            Assert.AreEqual (Person.names.Length, people.Count);
        }

        #endregion

        #region Test properties

        [TestMethod]
        public void UnitRs_Max()
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
        public void UnitRs_Min()
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

        #region Test methods

        [TestMethod]
        public void UnitRs_AddNull()
        {
            Setup();

            // SortedSet allows null key (but SortedDictionary does not).
            setTS1.Add (null);
        }


        [TestMethod]
        public void UnitRs_Add()
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
        public void UnitRs_ContainsNull()
        {
            Setup();

            // SortedSet allows null arg (but SortedDictionary does not).
            setS.Contains (null);
        }


        [TestMethod]
        public void UnitRs_Contains()
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
        public void CrashRs_CopyTo_ArgumentNull()
        {
            Setup();
            string[] nada = null;
            setS.CopyTo (nada, 0, 1);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRs_CopyTo1_ArgumentOutOfRange()
        {
            var s1 = new string[1];
            Setup();
            setS.Add ("ee");
            setS.CopyTo (s1, -1);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRs_CopyTo2_ArgumentOutOfRange()
        {
            var s1 = new string[1];
            Setup();
            setS.Add ("ee");
            setS.CopyTo (s1, 0, -1);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRs_CopyTo_Argument()
        {
            var s1 = new string[1];
            Setup();
            setS.Add ("xx");
            setS.Add ("mm");

            setS.CopyTo (s1, 0, 2);
        }


        [TestMethod]
        public void UnitRs_CopyTo1()
        {
            var s1 = new string[3];
            Setup();
            setS.Add ("xx");
            setS.Add ("mm");

            setS.CopyTo (s1, 1, 2);
        }


        [TestMethod]
        public void UnitRs_CopyTo2()
        {
            var i3 = new TS1[3];
            Setup();

            setTS1.Add (new TS1 (4));
            setTS1.Add (new TS1 (2));

            setTS1.CopyTo (i3, 1, 2);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRs_CopyToOb_ArgumentNull()
        {
            Setup();
            var setSo = (System.Collections.ICollection) setS;
            object[] nada = null;
            setSo.CopyTo (nada, 0);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRs_CopyToOb1_Argument()
        {
            Setup();
            var setSo = (System.Collections.ICollection) setS;
            var multi = new string[1,2];
            var multiOb = (System.Collections.ICollection) multi;
            setSo.CopyTo (multi, 0);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRs_CopyToOb2_Argument()
        {
            Setup();
            var setSo = (System.Collections.ICollection) setS;
            var a11 = Array.CreateInstance (typeof (int), new int[]{1}, new int[]{1});

            setSo.CopyTo (a11, 1);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRs_CopyToOb_ArgumentOutOfRange()
        {
            var s1 = new string[1];
            Setup();
            setS.Add ("ee");
            var setSo = (System.Collections.ICollection) setS;

            setSo.CopyTo (s1, -1);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRs_CopyToOb3_Argument()
        {
            var s1 = new string[1];
            Setup();
            setS.Add ("ee");
            setS.Add ("bb");
            var setSo = (System.Collections.ICollection) setS;

            setSo.CopyTo (s1, 0);
        }


        [TestMethod]
        public void UnitRs_Remove()
        {
            bool isOk;
            Setup (4);

            foreach (int x1 in iVals1)
                setI.Add (x1);
            int count0 = setI.Count;

            isOk = setI.Remove (18);
            Assert.IsTrue (isOk);
            Assert.AreEqual (count0-1, setI.Count);

            isOk = setI.Remove (18);
            Assert.IsFalse (isOk);
            Assert.AreEqual (count0-1, setI.Count);
        }


        static bool IsEven (int val) { return val % 2 == 0; }
        static bool IsTrue (int val) { return true; }

        [TestMethod]
        public void UnitRs_RemoveWhere()
        {
            Setup (4);

            foreach (int x1 in iVals1)
                setI.Add (x1);

            int removeCount = setI.RemoveWhere (IsEven);
            Assert.AreEqual (4, removeCount);
            Assert.AreEqual (3, setI.Count);

            removeCount = setI.RemoveWhere (IsTrue);
            Assert.AreEqual (3, removeCount);
            Assert.AreEqual (0, setI.Count);

            removeCount = setI.RemoveWhere (IsTrue);
            Assert.AreEqual (0, removeCount);
        }


        [TestMethod]
        public void UnitRs_ReverseEmpty()
        {
            int total = 0;
            Setup (5);

            foreach (var countdown in setI.Reverse())
               ++total;

            Assert.AreEqual (0, total);
        }


        [TestMethod]
        public void UnitRs_Reverse()
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

        #endregion

        #region Test ISet methods

        [TestMethod]
        public void UnitRs_ExceptWith()
        {
            Setup();
            foreach (var v1 in iVals1)
                setI.Add (v1);

            var list1 = new System.Collections.Generic.List<int> (iVals1);
            var list2 = new System.Collections.Generic.List<int> (iVals2);

            setI.ExceptWith (iVals2);

            int expectedCount = iVals1.Length;
            foreach (int i2 in iVals2)
                if (list1.Contains (i2))
                    --expectedCount;

            foreach (int ii in setI)
                if (list2.Contains (ii))
                    Assert.Fail ("Unexpected = " + ii);

            Assert.AreEqual (expectedCount, setI.Count);
        }


        [TestMethod]
        public void UnitRs_IsSupersetOf()
        {
            bool isSuper;
            Setup();

            isSuper = setI.IsSupersetOf (new int[0]);
            Assert.IsTrue (isSuper);

            foreach (int item in iVals1)
                setI.Add (item);
            Assert.IsTrue (isSuper);

            isSuper = setI.IsSupersetOf (iVals4);
            Assert.IsTrue (isSuper);

            isSuper = setI.IsSupersetOf (iVals2);
            Assert.IsFalse (isSuper);
        }

        #endregion

        #region Test bonus methods
#if ! TEST_BCL

        [TestMethod]
        public void UnitRs_IndexOf()
        {
            Setup (4);
            for (int ii = 0; ii < 50; ++ii)
                setI.Add (ii*2);

            var iz = setI.IndexOf (-1);
            var i0 = setI.IndexOf (0);
            var i8 = setI.IndexOf (8);
            var i100 = setI.IndexOf (100);

            Assert.AreEqual (~0, iz);
            Assert.AreEqual (0, i0);
            Assert.AreEqual (4, i8);
            Assert.AreEqual (~50, i100);
        }

#endif
        #endregion
    }
}
