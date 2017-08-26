﻿//
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
            var bag = new RankedBag<int> () { 5 };
            var isX = bag.ContainsAll (null);
        }

        [TestMethod]
        public void UnitRb_ContainsAll()
        {
            var bag = new RankedBag<int> () { 3, 5, 5, 5, 7 };

            Assert.IsTrue (bag.ContainsAll (new int[] { }));
            Assert.IsTrue (bag.ContainsAll (new int[] { 5, 5 }));
            Assert.IsTrue (bag.ContainsAll (new int[] { 5, 5, 5 }));
            Assert.IsTrue (bag.ContainsAll (new int[] { 5, 7 }));
            Assert.IsFalse (bag.ContainsAll (new int[] { 5, 5, 5, 5 }));
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRb_CopyTo1_ArgumentNull()
        {
            var bag = new RankedBag<int> () { 1 };
            bag.CopyTo (null);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRb_CopyTo1_Argument()
        {
            var bag = new RankedBag<int> () { 1, 11 };
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
            var bag = new RankedBag<int> () { 2 };
            bag.CopyTo (null, 0);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRb_CopyTo2_ArgumentOutOfRange()
        {
            var d2 = new int[2];
            var bag = new RankedBag<int> () { 2, 22 };
            bag.CopyTo (d2, -1);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRb_CopyTo2_Argument()
        {
            var d2 = new int[2];
            var bag = new RankedBag<int> () { 2, 22 };
            bag.CopyTo (d2, 1);
        }

        [TestMethod]
        public void UnitRb_CopyTo2()
        {
            var s2 = new int[] { 3, 5 };
            var e4 = new int[] { 0, 3, 5, 0 };
            var d2 = new int[2];
            var d4 = new int[4];
            var bag = new RankedBag<int>(s2);

            bag.CopyTo (d2, 0);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (s2, d2));

            bag.CopyTo (d4, 1);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (e4, d4));
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRb_CopyTo3_ArgumentNull()
        {
            var bag = new RankedBag<int> () { 2 };
            bag.CopyTo (null, 0, 1);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRb_CopyTo3A_ArgumentOutOfRange()
        {
            var d2 = new int[2];
            var bag = new RankedBag<int> () { 2 };
            bag.CopyTo (d2, -1, 1);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRb_CopyTo3B_ArgumentOutOfRange()
        {
            var d2 = new int[2];
            var bag = new RankedBag<int> () { 2 };
            bag.CopyTo (d2, 0, -1);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRb_CopyTo3A_Argument()
        {
            var d2 = new int[2];
            var bag = new RankedBag<int> () { 2, 22 };
            bag.CopyTo (d2, 1, 2);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRb_CopyTo3B_Argument()
        {
            var d3 = new int[3];
            var bag = new RankedBag<int> () { 2, 22 };
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
        public void UnitRb_Remove1()
        {
            var bag0 = new RankedBag<int>();
            var bag = new RankedBag<int> (new int[] { 3, 5, 5, 7, 7 });

            var rem0 = bag0.Remove (0);
            Assert.IsFalse (rem0);

            var rem2 = bag.Remove (2);
            Assert.IsFalse (rem2);

            var rem5 = bag.Remove (5);
            Assert.IsTrue (rem5);
            Assert.AreEqual (4, bag.Count);

            var rem9 = bag.Remove (9);
            Assert.IsFalse (rem9);
        }


        [TestMethod]
        public void UnitRb_Remove2()
        {
            var bag0 = new RankedBag<int>();
            var bag = new RankedBag<int> (new int[] { 3, 5, 5, 7, 7 });

            var rem0 = bag0.Remove (0, 1);
            Assert.IsFalse (rem0);

            var rem2 = bag.Remove (2, 2);
            Assert.IsFalse (rem2);

            var rem5 = bag.Remove (5, 3);
            Assert.IsTrue (rem5);
            Assert.AreEqual (3, bag.Count);

            var rem7 = bag.Remove (7, 1);
            Assert.IsTrue (rem7);
            Assert.AreEqual (2, bag.Count);

            var rem9 = bag.Remove (9);
            Assert.IsFalse (rem9);
        }


        [TestMethod]
        public void UnitRb_RemoveAll()
        {
            var bag0 = new RankedBag<int>();
            var bag = new RankedBag<int> (new int[] { 3, 5, 5, 7, 7 });

            int rem0 = bag0.RemoveAll (new int[] { 2 });
            Assert.AreEqual (0, rem0);

            int rem2 = bag.RemoveAll (new int[] { 2 });
            Assert.AreEqual (0, rem0);

            int rem57 = bag.RemoveAll (new int[] { 5, 7 });
            Assert.AreEqual (2, rem57);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (new int[] { 3, 5, 7 }, bag));
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
            var bag = new RankedBag<int> (new int[] { 3, 4, 5, 5, 6, 6, 7, 7, 8 });

            bag0.RetainAll (new int[] { 2, 4 });
            Assert.AreEqual (0, bag0.Count);

            bag.RetainAll (new int[] { 1, 4, 6, 6, 9 });
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (new int[] { 4, 6, 6 }, bag));
        }

        #endregion

        #region Test enumeration

        [TestMethod]
        public void UnitRb_ElementsBetween()
        {
            var bag0 = new RankedBag<int>();
            var bag1 = new RankedBag<int> (new int[] { 3, 4, 5, 5, 6, 6, 7, 7, 8 });
            var bag2 = new RankedBag<int> (new int[] { 5, 5, 5, 5, 5 });

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
            var bag1 = new RankedBag<int> (new int[] { 3, 4, 5, 5, 6, 6, 7, 7, 8 });
            var bag2 = new RankedBag<int> (new int[] { 5, 5, 5, 5, 5 });

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

        #endregion
    }
#endif
}
