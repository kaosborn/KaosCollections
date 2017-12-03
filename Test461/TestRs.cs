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
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRs_Ctor1B_ArgumentNull()
        {
            var nullArg = (System.Collections.Generic.ICollection<int>) null;
#if TEST_BCL
            var set = new SortedSet<int>(nullArg);
#else
            var set = new RankedSet<int>(nullArg);
#endif
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
        public void UnitRs_ocIsSynchronized()
        {
            Setup();
            var oc = (System.Collections.ICollection) setI;
            bool isSync = oc.IsSynchronized;
            Assert.IsFalse (isSync);
        }


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


        [TestMethod]
        public void UnitRs_ocSyncRoot()
        {
            Setup();
            var oc = (System.Collections.ICollection) setI;
            Assert.IsFalse (oc.SyncRoot.GetType().IsValueType);
        }

        #endregion

        #region Test methods

        [TestMethod]
        public void UnitRs_Add()
        {
            Setup();

            setS.Add ("aa");
            setS.Add ("cc");
            bool isOk1 = setS.Add ("bb");
            Assert.IsTrue (isOk1);
            bool isOk2 = setS.Add (null);
            Assert.IsTrue (isOk2);
            bool isOk3 = setS.Add ("cc");
            Assert.IsFalse (isOk3);

            Assert.AreEqual (4, setS.Count);
        }


        [TestMethod]
        public void UnitRs_ocAdd()
        {
            Setup();
            var oc = (System.Collections.Generic.ICollection<int>) setI;

            oc.Add (3); oc.Add (5);
            Assert.AreEqual (2, setI.Count);

            oc.Add (3);
            Assert.AreEqual (2, setI.Count);
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
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRs_CopyTo1_ArgumentNull()
        {
            Setup();
            setI.CopyTo (null);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRs_CopyTo1_Argument()
        {
            Setup();
            var d1 = new int[1];

            setI.Add (1); setI.Add (11);
            setI.CopyTo (d1);
        }

        [TestMethod]
        public void UnitRs_CopyTo1()
        {
            Setup();
            var e3 = new int[] { 3, 5, 7 };
            var e4 = new int[] { 3, 5, 7, 0 };
            var d3 = new int[3];
            var d4 = new int[4];

            setI.Add (3); setI.Add (5); setI.Add (7);

            setI.CopyTo (d3);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (e3, d3));

            setI.CopyTo (d4);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (e4, d4));
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRs_CopyTo2_ArgumentNull()
        {
            Setup();
            setI.CopyTo (null, 0);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRs_CopyTo2_ArgumentOutOfRange()
        {
            var d2 = new int[2];
            Setup();
            setI.Add (2);
            setI.CopyTo (d2, -1);
        }

        [TestMethod]
        public void UnitRs_CopyTo2A()
        {
            Setup();
            var e2 = new int[] { 3, 5 };
            var e4 = new int[] { 0, 3, 5, 0 };
            var d2 = new int[2];
            var d4 = new int[4];

            setI.Add (3); setI.Add (5);

            setI.CopyTo (d2, 0);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (e2, d2));

            setI.CopyTo (d4, 1);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (e4, d4));
        }

        [TestMethod]
        public void UnitRs_CopyTo2B()
        {
            Setup();
            var i3 = new TS1[3];

            setTS1.Add (new TS1 (4));
            setTS1.Add (new TS1 (2));

            setTS1.CopyTo (i3, 1, 2);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRs_CopyTo3_ArgumentNull()
        {
            Setup();
            setI.CopyTo (null, 0, 0);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRs_CopyTo3A_ArgumentOutOfRange()
        {
            Setup();
            var d2 = new int[2];

            setI.Add (2);
            setI.CopyTo (d2, -1, 0);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRs_CopyTo3B_ArgumentOutOfRange()
        {
            Setup();
            var d2 = new int[2];

            setI.Add (2);
            setI.CopyTo (d2, 0, -1);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRs_CopyTo3A_Argument()
        {
            Setup();
            var d2 = new int[2];

            setI.Add (3); setI.Add (5);

            setI.CopyTo (d2, 1, 2);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRs_CopyTo3B_Argument()
        {
            Setup();
            var d3 = new int[3];

            setI.Add (2); setI.Add (22);

            setI.CopyTo (d3, 1, 3);
        }

        [TestMethod]
        public void UnitRs_CopyTo3()
        {
            Setup();
            var e2 = new int[] { 0, 0 };
            var e3 = new int[] { 3, 5, 7 };
            var e4 = new int[] { 0, 3, 5, 0 };
            var e5 = new int[] { 0, 3, 5, 7, 0 };
            var d2 = new int[2];
            var d3 = new int[3];
            var d4 = new int[4];
            var d5 = new int[5];

            setI.Add (3); setI.Add (5); setI.Add (7);

            setI.CopyTo (d2, 1, 0);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (e2, d2));

            setI.CopyTo (d3, 0, 3);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (e3, d3));

            setI.CopyTo (d4, 1, 2);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (e4, d4));

            setI.CopyTo (d5, 1, 4);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (e5, d5));
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRs_ocCopyTo2_ArgumentNull()
        {
            Setup();
            var oc = (System.Collections.ICollection) setI;

            oc.CopyTo (null, 0);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRs_ocCopyTo2_ArgumentOutOfRange()
        {
            Setup();
            var oc = (System.Collections.ICollection) setI;
            var d1 = new object[1];

            oc.CopyTo (d1, -1);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRs_ocCopyTo2A_Argument()
        {
            Setup();
            var oc = (System.Collections.ICollection) setI;
            var d2 = new object[2];

            setI.Add (3); setI.Add (5);

            oc.CopyTo (d2, 1);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRs_ocCopyTo2B_Argument()
        {
            Setup();
            var oc = (System.Collections.ICollection) setS;
            var s2 = new string[1,2];
            oc.CopyTo (s2, 0);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRs_ocCopyTo2C_Argument()
        {
            Setup();
            var oc = (System.Collections.ICollection) setS;
            var a11 = Array.CreateInstance (typeof (int), new int[]{1}, new int[]{1});

            oc.CopyTo (a11, 1);
        }

        [TestMethod]
        [ExpectedException (typeof(ArgumentException))]
        public void CrashRs_CopyTo2D_Argument()
        {
            Setup();
            var oc = (System.Collections.ICollection) setI;
            var sa = new string[2];
            var oa = (object[]) sa;

            setI.Add (3); setI.Add (5);
            oc.CopyTo (oa, 0);
        }

        [TestMethod]
        public void UnitRs_ocCopyTo2()
        {
            Setup (4);
            var e2 = new object[] { 3, 5 };
            var e4 = new object[] { null, 3, 5, null };
            var e6 = new object[] { null, 3, 5, 7, 9, 11 };
            var d2 = new object[2];
            var d4 = new object[4];
            var d6 = new object[6];
            var oc = (System.Collections.ICollection) setI;
            setI.Add (3); setI.Add (5);

            oc.CopyTo (d2, 0);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (e2, d2));

            oc.CopyTo (d4, 1);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (e4, d4));

            setI.Add (7); setI.Add (9); setI.Add (11);
            oc.CopyTo (d6, 1);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (e6, d6));
        }

        [TestMethod]
        public void UnitRs_Remove()
        {
            int n = 3200;
            Setup (4);
            for (int ii = n; ii >= 0; --ii)
                setI.Add (ii);

            int expected = n+1;
            for (int i2 = n; i2 >= 0; i2 -= 10)
            {
                bool isRemoved = setI.Remove (i2);
                --expected;
                Assert.IsTrue (isRemoved);
            }

            Assert.AreEqual (expected, setI.Count);
            bool isRemoved2 = setI.Remove (10);
            Assert.IsFalse (isRemoved2);
        }


        static int rwCounter = 9;
        static bool IsAlways (int arg) { return true; }
        static bool IsEven (int arg) { return arg % 2 == 0; }
        static bool IsGe1000 (int arg) { return arg >= 1000; }
        static bool IsHotAlways (int arg) { staticSetI.Add (++rwCounter); return true; }
#if TEST_BCL
        static SortedSet<int> staticSetI = new SortedSet<int>();
#else
        static RankedSet<int> staticSetI = new RankedSet<int>();
#endif

        [TestMethod]
#if ! TEST_BCL
        [ExpectedException (typeof (InvalidOperationException))]
#endif
        public void CrashRs_RemoveWhereHotPredicate_InvalidOperation()
        {
            Setup (4);
            staticSetI.Add (3); staticSetI.Add (4);

            // This does not throw for BCL, but it really should:
            staticSetI.RemoveWhere (IsHotAlways);
        }

        [TestMethod]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRs_RemoveWhereHotEtor_InvalidOperation()
        {
            Setup();
            setI.Add (3); setI.Add (4);

            foreach (var key in setI)
                setI.RemoveWhere (IsEven);
        }

        [TestMethod]
        public void UnitRs_RemoveWhereHotNonUpdate()
        {
            Setup();
            setI.Add (3); setI.Add (5);

            foreach (var key in setI)
                setI.RemoveWhere (IsEven);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRs_RemoveWhere_ArgumentNull()
        {
            Setup();
            setI.RemoveWhere (null);
        }

        [TestMethod]
        public void UnitRs_RemoveWhere()
        {
            Setup (5);

            for (int ix = 0; ix < 1200; ++ix)
                setI.Add (ix);

            int r1 = setI.RemoveWhere (IsGe1000);
            Assert.AreEqual (200, r1);
            Assert.AreEqual (1000, setI.Count);

            int c0 = setI.Count;
            int r2 = setI.RemoveWhere (IsEven);

            Assert.AreEqual (500, r2);
            foreach (int k2 in setI)
                Assert.IsTrue (k2 % 2 != 0);

            int r3 = setI.RemoveWhere (IsAlways);
            Assert.AreEqual (500, r3);
            Assert.AreEqual (0, setI.Count);
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
            Setup();
            var a37 = new int[] { 3, 7 };
            var a5 = new int[] { 5 };
            var a133799 = new int[] { 1, 3, 3, 7, 9, 9 };
            var empty = new int[] { };

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
            Setup(4);
            var a1 = new int[] { 3, 5, 7, 9, 11, 13 };
            var empty = new int[] { };

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
            Setup (4);

            setI.SymmetricExceptWith (new int[] { });
            Assert.AreEqual (0, setI.Count);

            setI.SymmetricExceptWith (new int[] { 4, 5, 6 });
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (new int[] { 4, 5, 6}, setI));

            setI.SymmetricExceptWith (new int[] { 5, 7, 8 });
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (new int[] { 4, 6, 7, 8 }, setI));

            setI.SymmetricExceptWith (new int[] { 1, 2, 7, 8 });
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (new int[] { 1, 2, 4, 6 }, setI));

            setI.SymmetricExceptWith (new int[] { 2, 3 });
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual(new int[] { 1, 3, 4, 6 }, setI));
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
            Setup();
            var a357 = new int[] { 3, 5, 7 };
            var a5599 = new int[] { 5, 5, 9, 9 };
            var empty = new int[] { };

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
            Setup();
            var a35779 = new int[] { 3, 5, 7, 7, 9 };
            var a357 = new int[] { 3, 5, 7 };
            var a35 = new int[] { 3, 5 };
            var empty = new int[] { };

            Assert.IsTrue (setI.IsSubsetOf (a35));
            Assert.IsTrue (setI.IsSubsetOf (empty));

            setI.Add (3); setI.Add (5); setI.Add (7);

            Assert.IsTrue (setI.IsSubsetOf (a35779));
            Assert.IsTrue (setI.IsSubsetOf (a357));
            Assert.IsFalse (setI.IsSubsetOf (a35));
            Assert.IsFalse (setI.IsSubsetOf (empty));
            Assert.IsFalse (setI.IsSubsetOf (new int[] { 3, 5, 8 }));
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
            Setup();
            var a35779 = new int[] { 3, 5, 7, 7, 9 };
            var a357 = new int[] { 3, 5, 7 };
            var a35 = new int[] { 3, 5 };
            var empty = new int[] { };

            Assert.IsTrue (setI.IsProperSubsetOf (a35));
            Assert.IsFalse (setI.IsProperSubsetOf (empty));

            setI.Add (3); setI.Add (5); setI.Add (7);

            Assert.IsTrue (setI.IsProperSubsetOf (a35779));
            Assert.IsFalse (setI.IsProperSubsetOf (a357));
            Assert.IsFalse (setI.IsProperSubsetOf (a35));
            Assert.IsFalse (setI.IsProperSubsetOf (new int[] { 1, 2, 3, 4 }));
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
            Setup();
            var a3579 = new int[] { 3, 5, 7, 9 };
            var a357 = new int[] { 3, 5, 7 };
            var a35 = new int[] { 3, 5 };
            var a355 = new int[] { 3, 5, 5 };
            var empty = new int[] { };

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
            Setup();
            var a3579 = new int[] { 3, 5, 7, 9 };
            var a357 = new int[] { 3, 5, 7 };
            var a35 = new int[] { 3, 5 };
            var a355 = new int[] { 3, 5, 5 };
            var empty = new int[] { };

            Assert.IsFalse (setI.IsProperSupersetOf (empty));
            Assert.IsFalse (setI.IsProperSupersetOf (a35));

            setI.Add (3); setI.Add (5); setI.Add (7);

            Assert.IsFalse (setI.IsProperSupersetOf (a3579));
            Assert.IsFalse (setI.IsProperSupersetOf (a357));
            Assert.IsTrue (setI.IsProperSupersetOf (a35));
            Assert.IsTrue (setI.IsProperSupersetOf (a355));
            Assert.IsTrue (setI.IsProperSupersetOf (empty));
            Assert.IsFalse (setI.IsProperSupersetOf (new int[] { 2, 4 }));
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRs_Overlaps_ArgumentNull()
        {
            Setup();
            bool result = setI.Overlaps (null);
        }

        [TestMethod]
        public void UnitRs_OverlapsSet()
        {
            Setup();
            setI.Add (3); setI.Add (5); setI.Add (7);
            Assert.IsTrue (setI.Overlaps (setI));

#if TEST_BCL
            var set1 = new SortedSet<int> { 5, 6 };
            var set2 = new SortedSet<int> { 1, 8 };
#else
            var set1 = new RankedSet<int> { 5, 6 };
            var set2 = new RankedSet<int> { 1, 8 };
#endif
            bool isOlap1 = setI.Overlaps (set1);
            bool isOlap2 = setI.Overlaps (set2);

            Assert.IsTrue (setI.Overlaps (set1));
            Assert.IsFalse (setI.Overlaps (set2));
        }

        [TestMethod]
        public void UnitRs_OverlapsArray()
        {
            Setup();
            var a35779 = new int[] { 3, 5, 7, 7, 9 };
            var a357 = new int[] { 3, 5, 7 };
            var a35 = new int[] { 3, 5 };
            var a355 = new int[] { 3, 5, 5 };
            var a19 = new int[] { 1, 9 };
            var empty = new int[] { };

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
            Setup();
            var a359 = new int[] { 3, 5, 9 };
            var a3557 = new int[] { 3, 5, 5, 7 };
            var a357 = new int[] { 3, 5, 7 };
            var a35 = new int[] { 3, 5 };
            var a355 = new int[] { 3, 5, 5 };
            var a19 = new int[] { 1, 9 };
            var empty = new int[] { };

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

            int expected1 = 5;
            foreach (int key in setI.ElementsBetween (5, 15))
            {
                Assert.AreEqual (expected1, key);
                ++expected1;
            }
            Assert.AreEqual (expected1, 16);

            int expected2 = 15;
            foreach (int key in setI.ElementsBetween (15, 25))
            {
                Assert.AreEqual (expected2, key);
                ++expected2;
            }
            Assert.AreEqual (expected2, 20);
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
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRsx_ElementsBetweenIndexesA_ArgumentOutOfRange()
        {
            var set = new RankedSet<int> { 0, 1, 2 };
            foreach (var val in set.ElementsBetweenIndexes (-1, 0))
            { }
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRsx_ElementsBetweenIndexesB_ArgumentOutOfRange()
        {
            var set = new RankedSet<int> { 0, 1, 2 };
            foreach (var val in set.ElementsBetweenIndexes (3, 0))
            { }
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRsx_ElementsBetweenIndexesC_ArgumentOutOfRange()
        {
            var set = new RankedSet<int> { 0, 1, 2 };
            foreach (var val in set.ElementsBetweenIndexes (0, -1))
            { }
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRsx_ElementsBetweenIndexesD_ArgumentOutOfRange()
        {
            var set = new RankedSet<int> { 0, 1, 2 };
            foreach (var val in set.ElementsBetweenIndexes (0, 3))
            { }
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRsx_ElementsBetweenIndexes_Argument()
        {
            var set = new RankedSet<int> { 0, 1, 2 };
            foreach (var val in set.ElementsBetweenIndexes (2, 1))
            { }
        }

        [TestMethod]
        public void UnitRsx_ElementsBetweenIndexes()
        {
            int n = 33;
            var set = new RankedSet<int> { Capacity=4 };
            for (int ii = 0; ii < n; ++ii)
                set.Add (ii);

            for (int p1 = 0; p1 < n; ++p1)
                for (int p2 = p1; p2 < n; ++p2)
                {
                    int actual = 0;
                    foreach (var val in set.ElementsBetweenIndexes (p1, p2))
                        actual += val;

                    int expected = (p2 - p1 + 1) * (p1 + p2) / 2;
                    Assert.AreEqual (expected, actual);
                }
        }


        [TestMethod]
        public void UnitRsx_IndexOf()
        {
            Setup (4);
            for (int ii = 0; ii <= 98; ii+=2)
                setI.Add (ii);

            var iz = setI.IndexOf (-1);
            var i0 = setI.IndexOf (0);
            var i8 = setI.IndexOf (8);
            var i98 = setI.IndexOf (98);
            var i100 = setI.IndexOf (100);

            Assert.AreEqual (~0, iz);
            Assert.AreEqual (0, i0);
            Assert.AreEqual (4, i8);
            Assert.AreEqual (49, i98);
            Assert.AreEqual (~50, i100);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRsx_RemoveAtA_ArgumentOutOfRange()
        {
            var rs = new RankedSet<int> { 42 };
            rs.RemoveAt (-1);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRsx_RemoveAtB_ArgumentOutOfRange()
        {
            var rs = new RankedSet<int>();
            rs.RemoveAt (0);
        }


        [TestMethod]
        public void Unit_TryGetKey()
        {
            var rs = new RankedSet<string> (StringComparer.InvariantCultureIgnoreCase);

            rs.Add ("AAA");
            rs.Add ("bbb");
            rs.Add ("ccc");

            bool got1 = rs.TryGet ("aaa", out string actual1);
            Assert.IsTrue (got1);
            Assert.AreEqual ("AAA", actual1);

            bool got2 = rs.TryGet ("bb", out string actual2);
            Assert.IsFalse (got2);

            bool got3 = rs.TryGet ("CCC", out string actual3);
            Assert.IsTrue (got3);
            Assert.AreEqual ("ccc", actual3);
        }


        [TestMethod]
        public void UnitRsx_RemoveAt()
        {
            var rs = new RankedSet<int> { Capacity=5 };
            for (int ii = 0; ii < 5000; ++ii)
                rs.Add (ii);

            for (int i2 = 4990; i2 >= 0; i2 -= 10)
                rs.RemoveAt (i2);

            for (int i2 = 0; i2 < 5000; ++i2)
                if (i2 % 10 == 0)
                    Assert.IsFalse (rs.Contains (i2));
                else
                    Assert.IsTrue (rs.Contains (i2));
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRsx_RemoveRangeA_ArgumentOutOfRange()
        {
            var rs = new RankedSet<int>();
            rs.RemoveRange (-1, 0);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRsx_RemoveRangeB_ArgumentOutOfRange()
        {
            var rs = new RankedSet<int>();
            rs.RemoveRange (0, -1);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRsx_RemoveRange_Argument()
        {
            var rs = new RankedSet<int>();
            rs.Add (3); rs.Add (5);
            rs.RemoveRange (1, 2);
        }

        [TestMethod]
        public void UnitRsx_RemoveRange()
        {
            var set0 = new RankedSet<int> { Capacity=7 };
            for (int ii=0; ii<9; ++ii) set0.Add (ii);

            var set1 = new RankedSet<int> { Capacity=4 };
            for (int ii=0; ii<13; ++ii) set1.Add (ii);

            var set2 = new RankedSet<int> { Capacity=4 };
            for (int ii=0; ii<19; ++ii) set2.Add (ii);

            var set3 = new RankedSet<int> { Capacity=4 };
            for (int ii=0; ii<22; ++ii) set3.Add (ii);

            var set4 = new RankedSet<int> { Capacity=7 };
            for (int ii=0; ii<7; ++ii) set4.Add (ii);

            var set5 = new RankedSet<int> { Capacity=7 };
            for (int ii=0; ii<8; ++ii) set5.Add (ii);

            var set6 = new RankedSet<int> { Capacity=7 };
            for (int ii=0; ii<13; ++ii) set6.Add (ii);

            var set7 = new RankedSet<int> { Capacity=7 };
            for (int ii=0; ii<13; ++ii) set7.Add (ii);

            var set8 = new RankedSet<int> { Capacity=5 };
            for (int ii=0; ii<21; ++ii) set8.Add (ii);

            var set9 = new RankedSet<int> { Capacity=7 };
            for (int ii=0; ii<7; ++ii) set9.Add (ii);

            var setA = new RankedSet<int> { Capacity = 6 };
            for (int ii = 0; ii < 31; ++ii) setA.Add (ii);

            var setB = new RankedSet<int> { Capacity = 6 };
            for (int ii = 0; ii < 56; ++ii) setB.Add (ii);

            var setY = new RankedSet<int> { Capacity = 7 };
            for (int ii = 0; ii < 500; ++ii) setY.Add (ii);

            var setZ = new RankedSet<int>();

            set0.RemoveRange (0, 2);  Assert.AreEqual (7, set0.Count);
            set1.RemoveRange (10,2);  Assert.AreEqual (11,set1.Count);
            set2.RemoveRange (0, 6);  Assert.AreEqual (13,set2.Count);
            set3.RemoveRange (9, 6);  Assert.AreEqual (16,set3.Count);
            set4.RemoveRange (6, 1);  Assert.AreEqual (6, set4.Count);
            set5.RemoveRange (5, 2);  Assert.AreEqual (6, set5.Count);
            set6.RemoveRange (6, 4);  Assert.AreEqual (9, set6.Count);
            set7.RemoveRange (1, 6);  Assert.AreEqual (7, set7.Count);
            set8.RemoveRange (12,4);  Assert.AreEqual (17,set8.Count);
            set9.RemoveRange (1, 5);  Assert.AreEqual (2, set9.Count);
            setA.RemoveRange (5, 23); Assert.AreEqual (8, setA.Count);
            setB.RemoveRange (5, 40); Assert.AreEqual (16,setB.Count);
            setY.RemoveRange (0,500); Assert.AreEqual (0, setY.Count);
            setZ.RemoveRange (0, 0);  Assert.AreEqual (0, setZ.Count);
#if DEBUG
            set0.SanityCheck();
            set1.SanityCheck();
            set2.SanityCheck();
            set3.SanityCheck();
            set4.SanityCheck();
            set5.SanityCheck();
            set6.SanityCheck();
            set7.SanityCheck();
            set8.SanityCheck();
            set9.SanityCheck();
            setA.SanityCheck();
            setB.SanityCheck();
            setY.SanityCheck();
            setZ.SanityCheck();
#endif
        }

        [TestMethod]
        public void StressRsx_RemoveRange()
        {
            for (int width = 1; width <= 19; ++width)
            {
                for (int count = 0; count <= width; ++count)
                    for (int index = 0; index <= width - count; ++index)
                    {
                        var set = new RankedSet<int> { Capacity = 6 };
                        for (int ii = 0; ii < width; ++ii) set.Add (ii);
                        set.RemoveRange (index, count);
                        Assert.AreEqual (width-count, set.Count);
#if DEBUG
                        set.SanityCheck();
#endif
                    }
            }
        }

#endif
        #endregion

        #region Test enumeration

        [TestMethod]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRs_EtorOverflow_InvalidOperation()
        {
            Setup (4);
            for (int ix=0; ix<10; ++ix) setI.Add (ix);

            var etor = setI.GetEnumerator();
            while (etor.MoveNext())
            { }

            var val = ((System.Collections.IEnumerator) etor).Current;
        }

        [TestMethod]
        public void UnitRs_ocGetEnumerator()
        {
            Setup();
            var oc = ((System.Collections.Generic.ICollection<int>) setI);

            setI.Add (5);
            var xEtor = oc.GetEnumerator();

            xEtor.MoveNext();
            Assert.AreEqual (5, xEtor.Current);
        }

        [TestMethod]
        public void UnitRs_EtorOverflowNoCrash()
        {
            Setup (4);
            for (int ix=0; ix<10; ++ix) setI.Add (ix);

            var etor = setI.GetEnumerator();
            while (etor.MoveNext())
            { }

            var val = etor.Current;
        }

        [TestMethod]
        public void UnitRs_GetEnumerator()
        {
            int e1 = 0, e2 = 0;
            Setup (4);
            for (int ix=0; ix<10; ++ix) setI.Add (ix);

            var etor = setI.GetEnumerator();
            while (etor.MoveNext())
            {
                int gActual = etor.Current;
                object oActual = ((System.Collections.IEnumerator) etor).Current;
                Assert.AreEqual (e1, gActual);
                Assert.AreEqual (e1, oActual);
                ++e1;
            }
            Assert.AreEqual (10, e1);

            int gActualEnd = etor.Current;
            Assert.AreEqual (default (int), gActualEnd);

            bool isValid = etor.MoveNext();
            Assert.IsFalse (isValid);

            ((System.Collections.IEnumerator) etor).Reset();
            while (etor.MoveNext())
            {
                int val = etor.Current;
                Assert.AreEqual (e2, val);
                ++e2;
            }
            Assert.AreEqual (10, e2);
        }

        [TestMethod]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRs_EtorHotUpdate()
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
