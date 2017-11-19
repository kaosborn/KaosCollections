//
// Library: KaosCollections
// File:    TestRb.cs
//

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kaos.Collections;

namespace Kaos.Test.Collections
{
#if ! TEST_BCL
    public class NameItem
    {
        public NameItem (string name) { this.Name = name; }
        public String Name { get; private set; }
    }

    public partial class TestBtree
    {
        #region Test constructors

        [TestMethod]
        public void UnitRb_Inheritance()
        {
            var bagI = new RankedBag<int>();

            bagI.Add (42); bagI.Add (21); bagI.Add (63);

            var toIColI = bagI as System.Collections.Generic.ICollection<int>;
            var toIEnuI = bagI as System.Collections.Generic.IEnumerable<int>;
            var toIEnuO = bagI as System.Collections.IEnumerable;
            var toIColO = bagI as System.Collections.ICollection;
            var toIRocI = bagI as System.Collections.Generic.IReadOnlyCollection<int>;

            Assert.IsNotNull (toIColI);
            Assert.IsNotNull (toIEnuI);
            Assert.IsNotNull (toIEnuO);
            Assert.IsNotNull (toIColO);
            Assert.IsNotNull (toIRocI);

            int ObjEnumCount = 0;
            for (var oe = toIEnuO.GetEnumerator(); oe.MoveNext(); )
                ++ObjEnumCount;

            Assert.AreEqual (3, toIColI.Count);
            Assert.AreEqual (3, System.Linq.Enumerable.Count (toIEnuI));
            Assert.AreEqual (3, ObjEnumCount);
            Assert.AreEqual (3, toIColO.Count);
            Assert.AreEqual (3, toIRocI.Count);
        }

        public class DerivedB : RankedBag<int> { }

        [TestMethod]
        public void UnitRb_CtorSubclass()
        {
            var sub = new DerivedB();
            bool isRO = ((System.Collections.Generic.ICollection<int>) sub).IsReadOnly;
            Assert.IsFalse (isRO);
        }


        [TestMethod]
        public void UnitRb_Ctor0Empty()
        {
            var bagI = new RankedBag<int>();
            Assert.AreEqual (0, bagI.Count);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRb_Ctor1_ArgumentNull()
        {
            var bag = new RankedBag<int> ((System.Collections.Generic.IEnumerable<int>) null);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRb_Ctor2_ArgumentNull()
        {
            var bag = new RankedBag<int>((System.Collections.Generic.IEnumerable<int>) null, null);
        }

        #endregion

        #region Test properties

        [TestMethod]
        public void UnitRb_IsSynchronized()
        {
            var bag = new RankedBag<int>();
            var bagX = (System.Collections.ICollection) bag;
            bool isSync = bagX.IsSynchronized;
            Assert.IsFalse (isSync);
        }

        [TestMethod]
        public void UnitRb_MinMax()
        {
            var bag = new RankedBag<int>();
            var min0 = bag.Min;
            var max0 = bag.Max;

            bag.Add (3); bag.Add (5); bag.Add (7);
            var min1 = bag.Min;
            var max1 = bag.Max;

            Assert.AreEqual (3, min1);
            Assert.AreEqual (7, max1);
        }

        [TestMethod]
        public void UnitRb_ocSyncRoot()
        {
            var rb = new RankedBag<int>();
            var oc = (System.Collections.ICollection) rb;
            Assert.IsFalse (oc.SyncRoot.GetType().IsValueType);
        }

        #endregion

        #region Test methods

        [TestMethod]
        public void UnitRb_AddNull()
        {
            var bagNI = new RankedBag<NameItem>();
            bagNI.Add (null);
            Assert.AreEqual (1, bagNI.Count);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRb_Add2_ArgumentNull()
        {
            var bag = new RankedBag<int>();
            var result = bag.Add (1, -1);
        }

        [TestMethod]
        public void UnitRb_Add2()
        {
            var bagI = new RankedBag<int> (new int[] { 0, 0 });
            bool retVal = bagI.Add (0, 0);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (new int[] { 0, 0 }, bagI));
            Assert.IsTrue (retVal);
            Assert.AreEqual (2, bagI.Count);

            bagI = new RankedBag<int> (new int[] { 1, 1 });
            retVal = bagI.Add (2, 0);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (new int[] { 1, 1 }, bagI));
            Assert.IsFalse (retVal);
            Assert.AreEqual (2, bagI.Count);

            bagI = new RankedBag<int> (new int[] { 0, 1 });
            retVal = bagI.Add (2, 3);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (new int[] { 0, 1, 2, 2, 2 }, bagI));
            Assert.IsFalse (retVal);
            Assert.AreEqual (5, bagI.Count);

            bagI = new RankedBag<int> (new int[] { 0, 1 });
            retVal = bagI.Add (2, 3);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (new int[] { 0, 1, 2, 2, 2 }, bagI));
            Assert.IsFalse (retVal);
            Assert.AreEqual (5, bagI.Count);

            bagI = new RankedBag<int> (new int[] { 0, 2, 3 });
            retVal = bagI.Add (1, 2);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (new int[] { 0, 1, 1, 2, 3 }, bagI));
            Assert.IsFalse (retVal);
            Assert.AreEqual (5, bagI.Count);

            bagI = new RankedBag<int> (new int[] { 1, 2, 3, 4 });
            retVal = bagI.Add (0, 2);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (new int[] { 0, 0, 1, 2, 3, 4 }, bagI));
            Assert.IsFalse (retVal);
            Assert.AreEqual (6, bagI.Count);

            bagI = new RankedBag<int> (new int[] { 0, 2, 3 });
            retVal = bagI.Add (1, 4);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (new int[] { 0, 1, 1, 1, 1, 2, 3 }, bagI));
            Assert.IsFalse (retVal);
            Assert.AreEqual (7, bagI.Count);
        }


        [TestMethod]
        public void UnitRb_AddEx1()
        {
            var bag1 = new RankedBag<int>();
            var bagX = (System.Collections.Generic.ICollection<int>) bag1;

            bagX.Add (5);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (new int[] { 5 }, bag1));
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (new int[] { 5 }, bagX));
        }


        [TestMethod]
        public void UnitRb_Clear()
        {
            var bagI = new RankedBag<int>();

            for (int ix = 0; ix < 50; ++ix)
                bagI.Add (ix);

            Assert.AreEqual (50, bagI.Count);

            int k1 = 0;
            foreach (var i1 in bagI.Reverse())
                ++k1;
            Assert.AreEqual (50, k1);

            bagI.Clear();

            int k2 = 0;
            foreach (var i1 in bagI.Reverse())
                ++k2;
            Assert.AreEqual (0, k2);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRb_ContainsAll_ArgumentNull()
        {
            var bag = new RankedBag<int>() { 5 };
            var isX = bag.ContainsAll (null);
        }

        [TestMethod]
        public void UnitRb_ContainsAll()
        {
            var bag = new RankedBag<int>() { Capacity = 4 };
            foreach (var ii in new int[] { 3, 5, 5, 5, 7, 7, 9 })
                bag.Add (ii);

            var bag2 = new RankedBag<int>() { Capacity = 4 };
            foreach (var ii in bag)
                bag2.Add (ii);

            Assert.IsTrue (bag.ContainsAll (new int[] { }));
            Assert.IsTrue (bag.ContainsAll (new int[] { 5, 5 }));
            Assert.IsTrue (bag.ContainsAll (new int[] { 5, 5, 5 }));
            Assert.IsTrue (bag.ContainsAll (new int[] { 5, 7 }));
            Assert.IsFalse (bag.ContainsAll (new int[] { 5, 5, 5, 5 }));
            Assert.IsFalse (bag.ContainsAll (new int[] { 1, 2, 3, 4, 5, 6, 7, 8 }));

            Assert.IsTrue (bag.ContainsAll (bag2));
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRb_CopyTo1_ArgumentNull()
        {
            var bag = new RankedBag<int>() { 1 };
            bag.CopyTo (null);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRb_CopyTo1_Argument()
        {
            var bag = new RankedBag<int>() { 1, 11 };
            var d1 = new int[1];
            bag.CopyTo (d1);
        }

        [TestMethod]
        public void UnitRb_CopyTo1()
        {
            var e5 = new int[] { 3, 5, 5, 7, 0 };
            var s4 = new int[] { 3, 5, 5, 7 };
            var d4 = new int[4];
            var d5 = new int[5];
            var bag = new RankedBag<int> (s4);

            bag.CopyTo (d4);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (s4, d4));

            bag.CopyTo (d5);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (e5, d5));
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRb_CopyTo2_ArgumentNull()
        {
            var bag = new RankedBag<int>() { 2 };
            bag.CopyTo (null, 0);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRb_CopyTo2_ArgumentOutOfRange()
        {
            var d2 = new int[2];
            var bag = new RankedBag<int>() { 2, 22 };
            bag.CopyTo (d2, -1);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRb_CopyTo2_Argument()
        {
            var d2 = new int[2];
            var bag = new RankedBag<int>() { 2, 22 };
            bag.CopyTo (d2, 1);
        }

        [TestMethod]
        public void UnitRb_CopyTo2()
        {
            var s2 = new int[] { 3, 5 };
            var e4 = new int[] { 0, 3, 5, 0 };
            var d2 = new int[2];
            var d4 = new int[4];
            var bag = new RankedBag<int> (s2);

            bag.CopyTo (d2, 0);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (s2, d2));

            bag.CopyTo (d4, 1);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (e4, d4));
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRb_CopyTo3_ArgumentNull()
        {
            var bag = new RankedBag<int>() { 2 };
            bag.CopyTo (null, 0, 1);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRb_CopyTo3A_ArgumentOutOfRange()
        {
            var d2 = new int[2];
            var bag = new RankedBag<int>() { 2 };
            bag.CopyTo (d2, -1, 1);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRb_CopyTo3B_ArgumentOutOfRange()
        {
            var d2 = new int[2];
            var bag = new RankedBag<int>() { 2 };
            bag.CopyTo (d2, 0, -1);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRb_CopyTo3A_Argument()
        {
            var d2 = new int[2];
            var bag = new RankedBag<int>() { 2, 22 };
            bag.CopyTo (d2, 1, 2);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRb_CopyTo3B_Argument()
        {
            var d3 = new int[3];
            var bag = new RankedBag<int>() { 2, 22 };
            bag.CopyTo (d3, 1, 3);
        }

        [TestMethod]
        public void UnitRb_CopyTo3()
        {
            var s4 = new int[] { 3, 5, 5, 7 };
            var e2 = new int[] { 0, 0 };
            var e3 = new int[] { 0, 3, 5 };
            var e5 = new int[] { 0, 3, 5, 5, 0 };
            var d2 = new int[2];
            var d3 = new int[3];
            var d4 = new int[4];
            var d5 = new int[5];
            var d6 = new int[6];
            var bag = new RankedBag<int> (s4);

            bag.CopyTo (d2, 1, 0);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (e2, d2));

            bag.CopyTo (d3, 1, 2);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (e3, d3));

            bag.CopyTo (d4, 0, 4);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (s4, d4));

            bag.CopyTo (d5, 1, 3);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (e5, d5));
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRb_CopyTo2ng_ArgumentNull()
        {
            var bag = new RankedBag<int>() { 2 };
            var bagO = (System.Collections.ICollection) bag;
            bagO.CopyTo (null, 0);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRb_CopyTo2ng_ArgumentOutOfRange()
        {
            var d2 = new object[2];
            var bag = new RankedBag<int>() { 2, 22 };
            var bagO = (System.Collections.ICollection) bag;

            bagO.CopyTo (d2, -1);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRb_CopyTo2ngA_Argument()
        {
            var d2 = new object[2];
            var bag = new RankedBag<int>() { 2, 22 };
            var bagO = (System.Collections.ICollection) bag;

            bagO.CopyTo (d2, 1);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRb_CopyTo2ngC_Argument()
        {
            var bag = new RankedBag<int>() { 2, 22 };
            var bagO = (System.Collections.ICollection) bag;
            var multi = new int[1,2];
            var multiOb = (System.Collections.ICollection) multi;
            bagO.CopyTo (multi, 0);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRb_CopyTo2ngD_Argument()
        {
            var bag = new RankedBag<int>() { 2, 22 };
            var bagO = (System.Collections.ICollection) bag;
            var a11 = Array.CreateInstance (typeof (int), new int[]{1}, new int[]{1});

            bagO.CopyTo (a11, 1);
        }

        [TestMethod]
        public void UnitRb_CopyTo2ng()
        {
            var s4 = new int[] { 3, 5, 5, 7 };
            var e2 = new int[] { 0, 0 };
            var e3 = new int[] { 0, 3, 5 };
            var e5 = new int[] { 0, 3, 5, 5, 0 };
            var e6 = new int[] { 0, 3, 5, 5, 7, 0 };
            var d2 = new int[2];
            var d3 = new int[3];
            var d4 = new int[4];
            var d5 = new int[5];
            var d6 = new int[6];
            var bag = new RankedBag<int> (s4);
            var bagO = (System.Collections.ICollection) bag;

            bagO.CopyTo (d4, 0);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (s4, d4));

            bagO.CopyTo (d6, 1);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (e6, d6));
        }


        [TestMethod]
        public void UnitRb_GetCount()
        {
            var bag0 = new RankedBag<int>();
            var bag = new RankedBag<int> (new int[] { 3, 5, 5, 5, 7 });

            Assert.AreEqual (0, bag0.GetCount (4));

            Assert.AreEqual (0, bag.GetCount (1));
            Assert.AreEqual (1, bag.GetCount (3));
            Assert.AreEqual (3, bag.GetCount (5));
            Assert.AreEqual (1, bag.GetCount (7));
            Assert.AreEqual (0, bag.GetCount (9));
        }


        [TestMethod]
        public void UnitRb_GetDistinctCount()
        {
            var bag0 = new RankedBag<int>();
            var bag = new RankedBag<int> (new int[] { 3, 5, 5, 5, 7 });

            Assert.AreEqual (0, bag0.GetDistinctCount());
            Assert.AreEqual (3, bag.GetDistinctCount());
        }


        [TestMethod]
        public void StressRb_Counts()
        {
            int reps = 100;
            for (int order = 4; order <= 136; order += 8)
            {
                var bag = new RankedBag<int> { Capacity = order };

                for (int ix = 1; ix <= reps; ++ix)
                    bag.Add (ix, ix);

                for (int ix = 1; ix <= reps; ++ix)
                    Assert.AreEqual (ix, bag.GetCount (ix));

                Assert.AreEqual ((reps+1) * reps / 2, bag.Count);
                Assert.AreEqual (reps, bag.GetDistinctCount());
            }
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRb_ElementAt1_ArgumentOutOfRange()
        {
            var bag = new RankedBag<int>();
            int key = bag.ElementAt (-1);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRb_ElementAt2_ArgumentOutOfRange()
        {
            var bag = new RankedBag<int>();
            int key = bag.ElementAt (0);
        }

        [TestMethod]
        public void UnitRb_ElementAt()
        {
            int n = 800;
            var bag = new RankedBag<int>() { Capacity = 5 };

            for (int ii = 0; ii <= n; ++ii)
                bag.Add (ii/2);

            for (int ii = 0; ii <= n; ++ii)
            {
                int key = bag.ElementAt (ii);
                Assert.AreEqual (ii/2, key);
            }
        }


        [TestMethod]
        public void UnitRb_ElementAtOrDefault()
        {
            var bag = new RankedBag<int>();

            int keyM1 = bag.ElementAtOrDefault (-1);
            Assert.AreEqual (default (int), keyM1);

            int key0 = bag.ElementAtOrDefault (0);
            Assert.AreEqual (default (int), key0);

            bag.Add (9);

            int key00 = bag.ElementAtOrDefault (0);
            Assert.AreEqual (9, key00);

            int key1 = bag.ElementAtOrDefault (1);
            Assert.AreEqual (default (int), key1);
        }


        [TestMethod]
        public void UnitRb_IndexOf()
        {
            var bag0 = new RankedBag<int>();

            var bag = new RankedBag<int> (new int[] { 3, 5, 5, 7, 7 });
            bag.Capacity = 4;

            Assert.AreEqual (~0, bag0.IndexOf (9));

            Assert.AreEqual (~0, bag.IndexOf (2));
            Assert.AreEqual (0, bag.IndexOf (3));
            Assert.AreEqual (~1, bag.IndexOf (4));
            Assert.AreEqual (1, bag.IndexOf (5));
            Assert.AreEqual (~3, bag.IndexOf (6));
            Assert.AreEqual (3, bag.IndexOf (7));
            Assert.AreEqual (~5, bag.IndexOf (8));
        }


        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CrashRb_Last_InvalidOperation()
        {
            var bag = new RankedBag<int>();
            int key = bag.Last();
        }

        [TestMethod]
        public void UnitRb_Last()
        {
            var bag = new RankedBag<int>() { Capacity = 4 };
            for (int ii = 99; ii >= 0; --ii) bag.Add (ii);

            int key = bag.Last();
            Assert.AreEqual (99, key);
        }


        [TestMethod]
        public void UnitRb_Remove1()
        {
            var bag0 = new RankedBag<int>();
            var bag = new RankedBag<int>() { Capacity = 4 };

            foreach (int ii in new int[] { 3, 5, 5, 7, 7, 7, 9 })
                bag.Add (ii);

            bool rem0 = bag0.Remove (0);
            Assert.IsFalse (rem0);

            bool rem2 = bag.Remove (2);
            Assert.IsFalse (rem2);

            bool rem7 = bag.Remove (7);
            Assert.IsTrue (rem7);
            Assert.AreEqual (4, bag.Count);

            bool rem5 = bag.Remove (5);
            Assert.IsTrue (rem5);
            Assert.AreEqual (2, bag.Count);

            bool rem10 = bag.Remove (10);
            Assert.IsFalse (rem10);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRb_Remove2_Argument()
        {
            var bag = new RankedBag<int>();
            bag.Remove (1, -1);
        }

        [TestMethod]
        public void UnitRb_Remove2()
        {
            var bag0 = new RankedBag<int>();
            var bag1 = new RankedBag<int>() { Capacity = 4 };
            var bag2 = new RankedBag<int>() { Capacity = 4 };
            var bag3 = new RankedBag<int>() { Capacity = 5 };

            foreach (int ii in new int[] { 3, 5, 5, 7, 7, 7, 9 })
                bag1.Add (ii);

            foreach (int ii in new int[] { 3, 3, 3, 5, 5, 5, 7, 7, 7, 9 })
                bag2.Add (ii);

            for (int ii=0; ii<41; ++ii)
                bag3.Add (ii/5);

            var rem0 = bag0.Remove (0, 1);
            Assert.AreEqual (0, rem0);

            var rem2 = bag1.Remove (2, 2);
            Assert.AreEqual (0, rem2);

            var rem70 = bag1.Remove (7, 0);
            Assert.AreEqual (0, rem70);

            var rem7 = bag1.Remove (7, 1);
            Assert.AreEqual (1, rem7);
            Assert.AreEqual (6, bag1.Count);

            var rem5 = bag1.Remove (5, 3);
            Assert.AreEqual (2, rem5);
            Assert.AreEqual (4, bag1.Count);

            var rem9 = bag1.Remove (10);
            Assert.IsFalse (rem9);

            var rem53 = bag2.Remove (5, 3);
            Assert.AreEqual (3, rem53);

            var rem33 = bag2.Remove (3, 3);
            Assert.AreEqual (3, rem33);

            var rem99 = bag2.Remove (9, 9);
            Assert.AreEqual (1, rem99);

            Assert.AreEqual (3, bag2.Count);

            var rem35 = bag3.Remove (3, 9);
            Assert.AreEqual (5, rem35);
            Assert.AreEqual (36, bag3.Count);
            Assert.IsFalse (bag3.Contains (3));

            var rem65 = bag3.Remove (6, Int32.MaxValue);
            Assert.AreEqual (5, rem65);
            Assert.AreEqual (31, bag3.Count);
            Assert.IsFalse (bag3.Contains (6));
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRb_RemoveAll_ArgumentNull()
        {
            var bag = new RankedBag<int>();
            bag.RemoveAll (null);
        }

        [TestMethod]
        public void UnitRb_RemoveAll()
        {
            var bag0 = new RankedBag<int>();
            var bag = new RankedBag<int>() { Capacity = 4 };

            foreach (var ii in new int[] { 3, 5, 5, 7, 7 })
                bag.Add (ii);

            int rem0 = bag0.RemoveAll (new int[] { 2 });
            Assert.AreEqual (0, rem0);

            int rem2 = bag.RemoveAll (new int[] { 2 });
            Assert.AreEqual (0, rem0);

            int rem57 = bag.RemoveAll (new int[] { 5, 7 });
            Assert.AreEqual (2, rem57);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (new int[] { 3, 5, 7 }, bag));
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRb_RemoveAtA_ArgumentOutOfRange()
        {
            var bag = new RankedBag<int>();
            bag.Add (1);
            bag.RemoveAt (-1);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRb_RemoveAtB_ArgumentOutOfRange()
        {
            var bag = new RankedBag<int>();
            bag.RemoveAt (0);
        }

        [TestMethod]
        public void UnitRb_RemoveAt()
        {
            var bag = new RankedBag<int>();
            for (int i1 = 0; i1 < 5000; ++i1)
                bag.Add (i1);

            for (int i2 = 4900; i2 >= 0; i2 -= 100)
                bag.RemoveAt (i2);

            for (int i3 = 0; i3 < 5000; ++i3)
                if (i3 % 100 == 0)
                    Assert.IsFalse (bag.Contains (i3));
                else
                    Assert.IsTrue (bag.Contains (i3));

            var bag2 = new RankedBag<int>() { Capacity = 4 };
            for (int ii = 0; ii < 8; ++ii)
                bag2.Add (ii);
            bag2.RemoveAt (3);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (new int[] { 0, 1, 2, 4, 5, 6, 7 }, bag2));
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRb_RemoveRangeA_ArgumentOutOfRange()
        {
            var bag = new RankedBag<int>();
            bag.RemoveRange (-1, 0);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRb_RemoveRangeB_ArgumentOutOfRange()
        {
            var bag = new RankedBag<int>();
            bag.RemoveRange (0, -1);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRb_RemoveRange_Argument()
        {
            var bag = new RankedBag<int>();
            bag.Add (3); bag.Add (5);
            bag.RemoveRange (1, 2);
        }

        [TestMethod]
        public void UnitRb_RemoveRange()
        {
            var bag = new RankedBag<int>() { Capacity=7 };
            for (int ii=0; ii<20; ++ii) bag.Add (ii);

            bag.RemoveRange (20, 0);
            Assert.AreEqual (20, bag.Count);

            bag.RemoveRange (12, 4);
            Assert.AreEqual (16, bag.Count);
#if DEBUG
            bag.SanityCheck();
#endif
        }


        [TestMethod]
        public void UnitRb_RemoveWhere()
        {
            var bag0 = new RankedBag<int>();
            var bag = new RankedBag<int> (new int[] { 3, 4, 5, 5, 6, 6, 7, 7, 8 });


            bag0.RemoveWhere (IsEven);
            Assert.AreEqual (0, bag0.Count);

            bag.RemoveWhere (IsEven);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (new int[] { 3, 5, 5, 7, 7 }, bag));
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRb_RetainAll_ArgumentNull()
        {
            var bag = new RankedBag<int>() { 2 };
            bag.RetainAll (null);
        }

        [TestMethod]
        public void UnitRb_RetainAll()
        {
            var bag0 = new RankedBag<int>();
            var bag1 = new RankedBag<int>() { Capacity = 4 };
            var bag2 = new RankedBag<int>() { 3, 5, 7 };

            foreach (int ii in new int[] { 3, 4, 5, 5, 6, 6, 6, 7, 7, 8 })
                bag1.Add (ii);

            int del0 = bag0.RetainAll (new int[] { 2, 4 });
            Assert.AreEqual (0, del0);
            Assert.AreEqual (0, bag0.Count);

            int del1 = bag1.RetainAll (new int[] { 1, 4, 4, 6, 6, 9 });
            Assert.AreEqual (7, del1);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (new int[] { 4, 6, 6 }, bag1));

            int del2 = bag2.RetainAll (new int[] { });
            Assert.AreEqual (3, del2);
            Assert.AreEqual (0, bag2.Count);
        }

        #endregion

        #region Test enumeration

        [TestMethod]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRbq_DistinctHotUpdate()
        {
            var bag = new RankedBag<int> { 3,5,7,9 };
            int n = 0;

            //TODO for BCL foreach (var key in Enumerable.Distinct (bag))
            foreach (var kv in bag.Distinct())
                if (++n == 2)
                    bag.Remove (3);
        }

        [TestMethod]
        public void UnitRbq_Distinct()
        {
            var bag0 = new RankedBag<int>();
            var bag1 = new RankedBag<int> { Capacity = 4 };

            int a0 = 0, a1 = 0;
            foreach (var ii in new int[] { 3, 5, 5, 7, 7 })
                bag1.Add (ii);

            foreach (var k0 in bag0.Distinct())
                ++a0;

            foreach (var k1 in bag1.Distinct())
                ++a1;

            Assert.AreEqual (0, a0);
            Assert.AreEqual (3, a1);
        }

        [TestMethod]
        public void UnitRb_ElementsBetween()
        {
            var bag0 = new RankedBag<int>();
            var bag1 = new RankedBag<int>() { Capacity = 4 };
            var bag2 = new RankedBag<int> (new int[] { 5, 5, 5, 5, 5 });

            foreach (var k1 in new int[] { 3, 4, 5, 5, 6, 6, 7, 7, 8 })
                bag1.Add (k1);

            var d0 = new System.Collections.Generic.List<int> (bag0.ElementsBetween (2, 4));
            Assert.AreEqual (0, d0.Count);

            var d1 = new System.Collections.Generic.List<int> (bag1.ElementsBetween (5, 6));
            Assert.AreEqual (4, d1.Count);

            var d2 = new System.Collections.Generic.List<int> (bag1.ElementsBetween (5, 5));
            Assert.AreEqual (2, d2.Count);

            var d3 = new System.Collections.Generic.List<int> (bag2.ElementsBetween (5, 5));
            Assert.AreEqual (5, d3.Count);

            var d4 = new System.Collections.Generic.List<int> (bag2.ElementsBetween (1, 2));
            Assert.AreEqual (0, d4.Count);

            var d5 = new System.Collections.Generic.List<int> (bag2.ElementsBetween (9, 11));
            Assert.AreEqual (0, d5.Count);
        }


        [TestMethod]
        public void UnitRb_ElementsFrom()
        {
            var bag0 = new RankedBag<int>();
            var bag1 = new RankedBag<int>() { Capacity = 4 };
            var bag2 = new RankedBag<int> (new int[] { 5, 5, 5, 5, 5 });

            foreach (var i1 in new int[] { 3, 4, 5, 5, 6, 6, 7, 7, 8 })
                bag1.Add (i1);

            var d0 = new System.Collections.Generic.List<int> (bag0.ElementsFrom (0));
            Assert.AreEqual (0, d0.Count);

            var d1 = new System.Collections.Generic.List<int> (bag1.ElementsFrom (6));
            Assert.AreEqual (5, d1.Count);

            var d2 = new System.Collections.Generic.List<int> (bag1.ElementsFrom (1));
            Assert.AreEqual (9, d2.Count);

            var d3 = new System.Collections.Generic.List<int> (bag2.ElementsFrom (5));
            Assert.AreEqual (5, d3.Count);

            var d5 = new System.Collections.Generic.List<int> (bag2.ElementsFrom (9));
            Assert.AreEqual (0, d5.Count);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRb_ElementsInRangeA_ArgumentOutOfRange()
        {
            var bag = new RankedBag<int> { 0, 1, 2 };
            foreach (var val in bag.ElementsBetweenIndexes (-1, 0))
            { }
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRb_ElementsInRangeB_ArgumentOutOfRange()
        {
            var bag = new RankedBag<int> { 0, 1, 2 };
            foreach (var val in bag.ElementsBetweenIndexes (3, 0))
            { }
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRb_ElementsInRangeC_ArgumentOutOfRange()
        {
            var bag = new RankedBag<int> { 0, 1, 2 };
            foreach (var val in bag.ElementsBetweenIndexes (0, -1))
            { }
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRb_ElementsInRangeD_ArgumentOutOfRange()
        {
            var bag = new RankedBag<int> { 0, 1, 2 };
            foreach (var val in bag.ElementsBetweenIndexes (0, 3))
            { }
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRb_ElementsInRange_Argument()
        {
            var bag = new RankedBag<int> { 0, 1, 2 };
            foreach (var val in bag.ElementsBetweenIndexes (2, 1))
            { }
        }

        [TestMethod]
        public void UnitRb_ElementsInRange()
        {
            int n = 33;
            var bag = new RankedBag<int> { Capacity=4 };
            for (int ii = 0; ii < n; ++ii)
                bag.Add (ii);

            for (int p1 = 0; p1 < n; ++p1)
                for (int p2 = p1; p2 < n; ++p2)
                {
                    int actual = 0;
                    foreach (var val in bag.ElementsBetweenIndexes (p1, p2))
                        actual += val;

                    int expected = (p2 - p1 + 1) * (p1 + p2) / 2;
                    Assert.AreEqual (expected, actual);
                }
        }

        [TestMethod]
        public void UnitRb_Reverse()
        {
            int n = 800;
            var bag = new RankedBag<int> { Capacity = 4 };

            for (int i1 = 0; i1 < n; ++i1)
                bag.Add (i1/2);

            int i2 = n-1;
            foreach (var ii in bag.Reverse())
            {
                Assert.AreEqual (i2 / 2, ii);
                --i2;
            }
            Assert.AreEqual (i2, -1);
        }


        [TestMethod]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRb_EtorOverflow_InvalidOperation()
        {
            var bag = new RankedBag<int>() { Capacity = 4 };
            for (int ii=0; ii<10; ++ii) bag.Add (ii);

            var etor = bag.GetEnumerator();
            while (etor.MoveNext())
            { }

            var val = ((System.Collections.IEnumerator) etor).Current;
        }

        [TestMethod]
        public void UnitRb_ExGetEnumerator()
        {
            var bag = new RankedBag<int>();
            bag.Add (5);

            var xBag = ((System.Collections.Generic.ICollection<int>) bag);
            var xEtor = xBag.GetEnumerator();
            xEtor.MoveNext();
            Assert.AreEqual (5, xEtor.Current);
        }

        [TestMethod]
        public void UnitRb_GetEnumerator()
        {
            var bag = new RankedBag<int>() { Capacity = 4 };
            int e1 = 0, e2 = 0;
            for (int ii=0; ii<10; ++ii) bag.Add (ii);

            var etor = bag.GetEnumerator();
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
        public void CrashRb_EnumHotUpdate()
        {
            var bag = new RankedBag<int>() { Capacity = 4 };
            for (int ii=0; ii<10; ++ii) bag.Add (ii);

            int n = 0;
            foreach (int kv in bag)
            {
                if (++n == 2)
                    bag.Add (49);
            }
        }

        #endregion
    }
#endif
}
