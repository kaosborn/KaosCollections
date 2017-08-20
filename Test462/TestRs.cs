//
// Library: KaosCollections
// File:    TestRs.cs
//

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
#if TEST_BCL
using System.Collections.Generic;
#else
using Kaos.Collections;
#endif

namespace Kaos.Test.Collections
{
    public partial class TestBtree
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
            Assert.AreEqual (3, System.Linq.Enumerable.Count (toIEnuI));
            Assert.AreEqual (3, ObjEnumCount);
            Assert.AreEqual (3, toIColO.Count);
            Assert.AreEqual (3, toIRocI.Count);
        }


#if TEST_BCL
        public class DerivedS : SortedSet<int> { }
#else
        public class DerivedS : RankedSet<int> { }
#endif

        [TestMethod]
        public void UnitRs_CtorSubclass()
        {
            var sub = new DerivedS();
            bool isRO = ((System.Collections.Generic.ICollection<int>) sub).IsReadOnly;
            Assert.IsFalse (isRO);
        }


        [TestMethod]
        public void UnitRs_Ctor0Empty()
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
        public void CrashRs_Ctor1ANullMissing_InvalidOperation()
        {
            var comp0 = (System.Collections.Generic.Comparer<Person>) null;
#if TEST_BCL
            var nullComparer = new SortedSet<Person> (comp0);
#else
            var nullComparer = new RankedSet<Person> (comp0);
#endif
            nullComparer.Add (new Person ("Zed"));
            nullComparer.Add (new Person ("Macron"));
        }

        // MS docs incorrectly state this will throw.
        [TestMethod]
        public void UnitRs_Ctor1ANullOk()
        {
            var comp0 = (System.Collections.Generic.Comparer<int>) null;
#if TEST_BCL
            var nullComparer = new SortedSet<int> (comp0);
#else
            var nullComparer = new RankedSet<int> (comp0);
#endif
            nullComparer.Add (4);
            nullComparer.Add (2);
        }

        [TestMethod]
#if TEST_BCL
        [ExpectedException (typeof (ArgumentException))]
#else
        [ExpectedException (typeof (InvalidOperationException))]
#endif
        public void CrashRs_Ctor1A_InvalidOperation()
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
        public void UnitRs_Ctor1A1()
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

        #region Test class comparison

        [TestMethod]
        public void UnitRs_CreateSetComparer0()
        {
            Setup();
            setI.Add (3); setI.Add (5); setI.Add (7);

#if TEST_BCL
            var setJ = new SortedSet<int>();
            var classComparer = SortedSet<int>.CreateSetComparer();
#else
            var setJ = new RankedSet<int>();
            System.Collections.Generic.IEqualityComparer<RankedSet<int>> classComparer
                = RankedSet<int>.CreateSetComparer();
#endif
            setJ.Add (3); setJ.Add (7);

            bool b1 = classComparer.Equals (setI, setJ);
            Assert.IsFalse (b1);

            setJ.Add (5);
            bool b2 = classComparer.Equals (setI, setJ);
            Assert.IsTrue (b2);
        }

        [TestMethod]
        public void UnitRs_CreateSetComparer1()
        {
            Setup();
#if TEST_BCL
            var setJ = new SortedSet<int>();
            var classComparer = SortedSet<int>.CreateSetComparer();
#else
            var setJ = new RankedSet<int>();
            System.Collections.Generic.IEqualityComparer<RankedSet<int>> classComparer
                = RankedSet<int>.CreateSetComparer (System.Collections.Generic.EqualityComparer<int>.Default);
#endif
            setJ.Add (3); setJ.Add (7);

            bool b1 = classComparer.Equals (setI, setI);
            Assert.IsTrue (b1);
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

            // SortedSet ignores duplicates (but SortedDictionary throws).
            isOk = setS.Add ("cc");
            Assert.IsFalse (isOk);

            Assert.AreEqual (3, setS.Count);
        }


        [TestMethod]
        public void UnitRs_Clear()
        {
            Setup (4);
            for (int ix = 0; ix < 50; ++ix)
                setI.Add (ix);

            Assert.AreEqual (50, setI.Count);

            int k1 = 0;
            foreach (var i1 in setI.Reverse())
                ++k1;
            Assert.AreEqual (50, k1);

            setI.Clear();

            int k2 = 0;
            foreach (var i1 in setI.Reverse())
                ++k2;
            Assert.AreEqual (0, k2);
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
        public void UnitRs_CopyToObjectArray()
        {
            var obArr = new object[2];
            Setup();
            setI.Add (4); setI.Add (2);
            ((System.Collections.ICollection) setI).CopyTo (obArr, 0);
            Assert.AreEqual (2, (int) obArr[0]);
            Assert.AreEqual (4, (int) obArr[1]);
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
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRs_CopyTo3A_Argument()
        {
            Setup (4);
            setI.Add (2); setI.Add (4);
            var a1 = new int[3];
            setI.CopyTo (a1, 1, 3);
        }

        [TestMethod]
        public void UnitRs_CopyTo3B()
        {
            Setup (4);
            setI.Add (5); setI.Add (0); setI.Add (3);
            var a1 = new int[] { -1, -1, -1, -1, -1 };

            setI.CopyTo (a1, 1, 4);
            Assert.AreEqual (-1, a1[0]);
            Assert.AreEqual (0, a1[1]);
            Assert.AreEqual (3, a1[2]);
            Assert.AreEqual (5, a1[3]);
            Assert.AreEqual (-1, a1[4]);
        }

        [TestMethod]
        public void UnitRs_CopyTo3C()
        {
            Setup (4);
            setI.Add (5); setI.Add (0); setI.Add (3);
            var a1 = new int[] { -1, -1, -1, -1 };

            setI.CopyTo (a1, 1, 2);
            Assert.AreEqual (-1, a1[0]);
            Assert.AreEqual (0, a1[1]);
            Assert.AreEqual (3, a1[2]);
            Assert.AreEqual (-1, a1[3]);
        }

        [TestMethod]
        public void UnitRs_CopyTo3D()
        {
            Setup();
            setI.Add (5); setI.Add (0); setI.Add (3);
            var a1 = new int[] { -1, -1 };

            setI.CopyTo (a1, 0, 0);

            Assert.AreEqual (-1, a1[0]);
            Assert.AreEqual (-1, a1[1]);
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
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRs_ExceptWith_ArgumentNull()
        {
            Setup();
            setI.ExceptWith (null);
        }

        [TestMethod]
        public void UnitRs_ExceptWith()
        {
            var a37 = new int[] { 3, 7 };
            var a5 = new int[] { 5 };
            var a133799 = new int[] { 1, 3, 3, 7, 9, 9 };
            var empty = new int[] { };
            Setup();

            setI.ExceptWith (empty);
            Assert.AreEqual (0, setI.Count);

            setI.ExceptWith (a37);
            Assert.AreEqual (0, setI.Count);

            setI.Add (3); setI.Add (5); setI.Add (7);

            setI.ExceptWith (a133799);
            Assert.AreEqual (1, setI.Count);
            Assert.AreEqual (5, setI.Min);

            setI.ExceptWith (a5);
            Assert.AreEqual (0, setI.Count);
        }

        [TestMethod]
        public void UnitRs_ExceptWithSelf()
        {
            Setup();
            setI.Add (4); setI.Add (2);

            setI.ExceptWith (setI);
            Assert.AreEqual (0, setI.Count);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRs_IntersectWith_ArgumentNull()
        {
            Setup();
            setI.IntersectWith (null);
        }

        [TestMethod]
        public void UnitRs_IntersectWith()
        {
            var a1 = new int[] { 3, 5, 7, 9, 11, 13 };
            var empty = new int[] { };
            Setup(4);

            setI.IntersectWith (empty);
            Assert.AreEqual (0, setI.Count);

            foreach (var v1 in a1) setI.Add (v1);
            setI.IntersectWith (new int[] { 1 });
            Assert.AreEqual (0, setI.Count);

            setI.Clear();
            foreach (var v1 in a1) setI.Add (v1);
            setI.IntersectWith (new int[] { 15 });
            Assert.AreEqual (0, setI.Count);

            setI.Clear();
            foreach (var v1 in a1) setI.Add (v1);
            setI.IntersectWith (new int[] { 1, 9, 15 });
            Assert.AreEqual (1, setI.Count);
            Assert.AreEqual (9, setI.Min);

            setI.IntersectWith (empty);
            Assert.AreEqual (0, setI.Count);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRs_SymmetricExceptWith_ArgumentNull()
        {
            Setup();
            setI.SymmetricExceptWith (null);
        }

        [TestMethod]
        public void UnitRs_SymmetricExceptWith()
        {
            var a37 = new int[] { 3, 7 };
            var a357 = new int[] { 3, 5, 7 };
            var a5599 = new int[] { 5, 5, 9, 9 };
            var empty = new int[] { };
            Setup();

            setI.SymmetricExceptWith (empty);
            Assert.AreEqual (0, setI.Count);

            setI.SymmetricExceptWith (a37);
            Assert.AreEqual (2, setI.Count);
            Assert.AreEqual (3, setI.Min);
            Assert.AreEqual (7, setI.Max);

            setI.SymmetricExceptWith (a357);
            Assert.AreEqual (1, setI.Count);
            Assert.AreEqual (5, setI.Min);

            setI.SymmetricExceptWith (a5599);
            Assert.AreEqual (1, setI.Count);
            Assert.AreEqual (9, setI.Min);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRs_UnionWith_ArgumentNull()
        {
            Setup();
            setI.UnionWith (null);
        }

        [TestMethod]
        public void UnitRs_UnionWith()
        {
            var a357 = new int[] { 3, 5, 7 };
            var a5599 = new int[] { 5, 5, 9, 9 };
            var empty = new int[] { };
            Setup();

            setI.UnionWith (empty);
            Assert.AreEqual (0, setI.Count);

            setI.UnionWith (a357);
            Assert.AreEqual (3, setI.Count);

            setI.UnionWith (a5599);
            Assert.AreEqual (4, setI.Count);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRs_IsSubsetOf_ArgumentNull()
        {
            Setup();
            bool result = setI.IsSubsetOf (null);
        }

        [TestMethod]
        public void UnitRs_IsSubsetOf()
        {
            var a35779 = new int[] { 3, 5, 7, 7, 9 };
            var a357 = new int[] { 3, 5, 7 };
            var a35 = new int[] { 3, 5 };
            var empty = new int[] { };
            Setup();

            Assert.IsTrue (setI.IsSubsetOf (a35));
            Assert.IsTrue (setI.IsSubsetOf (empty));

            setI.Add (3); setI.Add (5); setI.Add (7);

            Assert.IsTrue (setI.IsSubsetOf (a35779));
            Assert.IsTrue (setI.IsSubsetOf (a357));
            Assert.IsFalse (setI.IsSubsetOf (a35));
            Assert.IsFalse (setI.IsSubsetOf (empty));
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRs_IsProperSubsetOf_ArgumentNull()
        {
            Setup();
            bool result = setI.IsProperSubsetOf (null);
        }

        [TestMethod]
        public void UnitRs_IsProperSubsetOf()
        {
            var a35779 = new int[] { 3, 5, 7, 7, 9 };
            var a357 = new int[] { 3, 5, 7 };
            var a35 = new int[] { 3, 5 };
            var empty = new int[] { };
            Setup();

            Assert.IsTrue (setI.IsProperSubsetOf (a35));
            Assert.IsFalse (setI.IsProperSubsetOf (empty));

            setI.Add (3); setI.Add (5); setI.Add (7);

            Assert.IsTrue (setI.IsProperSubsetOf (a35779));
            Assert.IsFalse (setI.IsProperSubsetOf (a357));
            Assert.IsFalse (setI.IsProperSubsetOf (a35));
            Assert.IsFalse (setI.IsProperSubsetOf (empty));
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRs_IsSupersetOf_ArgumentNull()
        {
            Setup();
            bool result = setI.IsSupersetOf (null);
        }

        [TestMethod]
        public void UnitRs_IsSupersetOf()
        {
            var a3579 = new int[] { 3, 5, 7, 9 };
            var a357 = new int[] { 3, 5, 7 };
            var a35 = new int[] { 3, 5 };
            var a355 = new int[] { 3, 5, 5 };
            var empty = new int[] { };
            Setup();

            Assert.IsTrue (setI.IsSupersetOf (empty));
            Assert.IsFalse (setI.IsSupersetOf (a35));

            setI.Add (3); setI.Add (5); setI.Add (7);

            Assert.IsFalse (setI.IsSupersetOf (a3579));
            Assert.IsTrue (setI.IsSupersetOf (a357));
            Assert.IsTrue (setI.IsSupersetOf (a35));
            Assert.IsTrue (setI.IsSupersetOf (a355));
            Assert.IsTrue (setI.IsSupersetOf (empty));
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRs_IsProperSupersetOf_ArgumentNull()
        {
            Setup();
            bool result = setI.IsProperSupersetOf (null);
        }

        [TestMethod]
        public void UnitRs_IsProperSupersetOf()
        {
            var a3579 = new int[] { 3, 5, 7, 9 };
            var a357 = new int[] { 3, 5, 7 };
            var a35 = new int[] { 3, 5 };
            var a355 = new int[] { 3, 5, 5 };
            var empty = new int[] { };
            Setup();

            Assert.IsFalse (setI.IsProperSupersetOf (empty));
            Assert.IsFalse (setI.IsProperSupersetOf (a35));

            setI.Add (3); setI.Add (5); setI.Add (7);

            Assert.IsFalse (setI.IsProperSupersetOf (a3579));
            Assert.IsFalse (setI.IsProperSupersetOf (a357));
            Assert.IsTrue (setI.IsProperSupersetOf (a35));
            Assert.IsTrue (setI.IsProperSupersetOf (a355));
            Assert.IsTrue (setI.IsProperSupersetOf (empty));
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRs_Overlaps_ArgumentNull()
        {
            Setup();
            bool result = setI.Overlaps (null);
        }

        [TestMethod]
        public void UnitRs_Overlaps()
        {
            var a35779 = new int[] { 3, 5, 7, 7, 9 };
            var a357 = new int[] { 3, 5, 7 };
            var a35 = new int[] { 3, 5 };
            var a355 = new int[] { 3, 5, 5 };
            var a19 = new int[] { 1, 9 };
            var empty = new int[] { };
            Setup();

            Assert.IsFalse (setI.Overlaps (empty));
            Assert.IsFalse (setI.Overlaps (a35));

            setI.Add (3); setI.Add (5); setI.Add (7);

            Assert.IsTrue (setI.Overlaps (a35779));
            Assert.IsTrue (setI.Overlaps (a357));
            Assert.IsTrue (setI.Overlaps (a35));
            Assert.IsFalse (setI.Overlaps (a19));
            Assert.IsFalse (setI.Overlaps (empty));
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRs_SetEquals_ArgumentNull()
        {
            Setup();
            bool result = setI.SetEquals (null);
        }

        [TestMethod]
        public void UnitRs_SetEquals()
        {
            var a359 = new int[] { 3, 5, 9 };
            var a3557 = new int[] { 3, 5, 5, 7 };
            var a357 = new int[] { 3, 5, 7 };
            var a35 = new int[] { 3, 5 };
            var a355 = new int[] { 3, 5, 5 };
            var a19 = new int[] { 1, 9 };
            var empty = new int[] { };
            Setup();

            Assert.IsTrue (setI.SetEquals (empty));
            Assert.IsFalse (setI.SetEquals (a35));

            setI.Add (3); setI.Add (5); setI.Add (7);

            Assert.IsTrue (setI.SetEquals (a3557));
            Assert.IsTrue (setI.SetEquals (a357));
            Assert.IsFalse (setI.SetEquals (a359));
            Assert.IsFalse (setI.SetEquals (a35));
            Assert.IsFalse (setI.SetEquals (a355));
            Assert.IsFalse (setI.SetEquals (a19));
            Assert.IsFalse (setI.SetEquals (empty));
        }

        [TestMethod]
        public void UnitRs_SetEquals2()
        {
            Setup();

            personSet.Add (new Person ("Fred"));
            personSet.Add (new Person ("Wilma"));

            var pa = new Person[] { new Person ("Wilma"), new Person ("Fred") };

            Assert.IsTrue (personSet.SetEquals (pa));

            personSet.Add (new Person ("Pebbles"));
            Assert.IsFalse (personSet.SetEquals (pa));
        }

        #endregion

        #region Test bonus methods
#if ! TEST_BCL

        [TestMethod]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRsx_ElementsBetweenHotUpdate()
        {
            Setup (4);
            for (int ix=0; ix<10; ++ix) setI.Add (ix);

            int n = 0;
            foreach (int key in setI.ElementsBetween (3, 8))
            {
                if (++n == 2)
                    setI.Add (49);
            }
        }

        [TestMethod]
        public void UnitRsx_ElementsBetween()
        {
            Setup (4);
            for (int ix=0; ix<20; ++ix) setI.Add (ix);

            int expected = 5;
            foreach (int key in setI.ElementsBetween (5, 15))
            {
                Assert.AreEqual (expected, key);
                ++expected;
            }
            Assert.AreEqual (expected, 16);
        }


        [TestMethod]
        public void UnitRsx_ElementsFromNull()
        {
            Setup (4);

            foreach (var px in personSet.ElementsFrom (null))
            { }
        }

        [TestMethod]
        public void UnitRsx_ElementsFrom()
        {
            Setup (4);
            for (int ii = 0; ii < 30; ++ii)
                setI.Add (ii);

            int ix = 20;
            foreach (var kx in setI.ElementsFrom (ix))
            {
                Assert.AreEqual (ix, kx);
                ++ix;
            }

            Assert.AreEqual (30, ix);
        }


        [TestMethod]
        public void UnitRsx_IndexOf()
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


        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRsx_RemoveAtA_ArgumentOutOfRange()
        {
            var d1 = new RankedSet<int>();
            d1.Add (42);
            d1.RemoveAt (-1);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRsx_RemoveAtB_ArgumentOutOfRange()
        {
            var d1 = new RankedSet<int>();
            d1.RemoveAt (0);
        }

        [TestMethod]
        public void UnitRsx_RemoveAt()
        {
            var d1 = new RankedSet<int>();
            for (int ii = 0; ii < 5000; ++ii)
                d1.Add (ii);

            for (int i2 = 4990; i2 >= 0; i2 -= 10)
                d1.RemoveAt (i2);

            for (int i2 = 0; i2 < 5000; ++i2)
                if (i2 % 10 == 0)
                    Assert.IsFalse (d1.Contains (i2));
                else
                    Assert.IsTrue (d1.Contains (i2));
        }

#endif
        #endregion

        #region Test enumeration

        [TestMethod]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRs_EnumeratorOverflow_InvalidOperation()
        {
            Setup (4);
            for (int ix=0; ix<10; ++ix) setI.Add (ix);

            var iter = setI.GetEnumerator();
            while (iter.MoveNext())
            { }

            var val = ((System.Collections.IEnumerator) iter).Current;
        }

        [TestMethod]
        public void UnitRs_EnumeratorOverflowNoCrash()
        {
            Setup (4);
            for (int ix=0; ix<10; ++ix) setI.Add (ix);

            var iter = setI.GetEnumerator();
            while (iter.MoveNext())
            { }

            var val = iter.Current;
        }

        [TestMethod]
        public void UnitRs_GetEnumerator()
        {
            int k1 = 0, k2 = 0;
            Setup (4);
            for (int ix=0; ix<10; ++ix) setI.Add (ix);

            var iter = setI.GetEnumerator();
            while (iter.MoveNext())
            {
                int val = iter.Current;
                Assert.AreEqual (k1, val);
                ++k1;
            }
            Assert.AreEqual (10, k1);

            bool isValid = iter.MoveNext();
            Assert.IsFalse (isValid);

            ((System.Collections.IEnumerator) iter).Reset();
            while (iter.MoveNext())
            {
                int val = iter.Current;
                Assert.AreEqual (k2, val);
                ++k2;
            }
            Assert.AreEqual (10, k2);
        }

        [TestMethod]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRs_EnumHotUpdate()
        {
            Setup (4);
            for (int ix=0; ix<10; ++ix) setI.Add (ix);

            int n = 0;
            foreach (int kv in setI)
            {
                if (++n == 2)
                    setI.Add (49);
            }
        }

        #endregion
    }
}
