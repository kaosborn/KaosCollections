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
            var rb = new RankedBag<int> { 42,21,63 };

            var toIColI = rb as System.Collections.Generic.ICollection<int>;
            var toIEnuI = rb as System.Collections.Generic.IEnumerable<int>;
            var toIEnuO = rb as System.Collections.IEnumerable;
            var toIColO = rb as System.Collections.ICollection;
            var toIRocI = rb as System.Collections.Generic.IReadOnlyCollection<int>;

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
            var rb = new RankedBag<int>();
            Assert.AreEqual (0, rb.Count);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRb_Ctor1_ArgumentNull()
        {
            var rb = new RankedBag<int> ((System.Collections.Generic.IEnumerable<int>) null);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRb_Ctor2_ArgumentNull()
        {
            var rb = new RankedBag<int>((System.Collections.Generic.IEnumerable<int>) null, null);
        }

        #endregion

        #region Test properties

        [TestMethod]
        public void UnitRb_IsSynchronized()
        {
            var rb = new RankedBag<int>();
            var oc = (System.Collections.ICollection) rb;
            bool isSync = oc.IsSynchronized;
            Assert.IsFalse (isSync);
        }


        [TestMethod]
        public void UnitRb_MinMax()
        {
            var rb = new RankedBag<int>();
            var min0 = rb.Min;
            var max0 = rb.Max;

            rb.Add (3); rb.Add (5); rb.Add (7);
            var min1 = rb.Min;
            var max1 = rb.Max;

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
            var rb = new RankedBag<NameItem>();
            rb.Add (null);
            Assert.AreEqual (1, rb.Count);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRb_Add2_ArgumentNull()
        {
            var rb = new RankedBag<int>();
            var zz = rb.Add (1, -1);
        }

        [TestMethod]
        public void UnitRb_Add2()
        {
            var rb = new RankedBag<int> (new int[] { 0, 0 });
            bool retVal = rb.Add (0, 0);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (new int[] { 0, 0 }, rb));
            Assert.IsTrue (retVal);
            Assert.AreEqual (2, rb.Count);

            rb = new RankedBag<int> (new int[] { 1, 1 });
            retVal = rb.Add (2, 0);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (new int[] { 1, 1 }, rb));
            Assert.IsFalse (retVal);
            Assert.AreEqual (2, rb.Count);

            rb = new RankedBag<int> (new int[] { 0, 1 });
            retVal = rb.Add (2, 3);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (new int[] { 0, 1, 2, 2, 2 }, rb));
            Assert.IsFalse (retVal);
            Assert.AreEqual (5, rb.Count);

            rb = new RankedBag<int> (new int[] { 0, 1 });
            retVal = rb.Add (2, 3);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (new int[] { 0, 1, 2, 2, 2 }, rb));
            Assert.IsFalse (retVal);
            Assert.AreEqual (5, rb.Count);

            rb = new RankedBag<int> (new int[] { 0, 2, 3 });
            retVal = rb.Add (1, 2);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (new int[] { 0, 1, 1, 2, 3 }, rb));
            Assert.IsFalse (retVal);
            Assert.AreEqual (5, rb.Count);

            rb = new RankedBag<int> (new int[] { 1, 2, 3, 4 });
            retVal = rb.Add (0, 2);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (new int[] { 0, 0, 1, 2, 3, 4 }, rb));
            Assert.IsFalse (retVal);
            Assert.AreEqual (6, rb.Count);

            rb = new RankedBag<int> (new int[] { 0, 2, 3 });
            retVal = rb.Add (1, 4);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (new int[] { 0, 1, 1, 1, 1, 2, 3 }, rb));
            Assert.IsFalse (retVal);
            Assert.AreEqual (7, rb.Count);
        }


        [TestMethod]
        public void UnitRb_AddEx1()
        {
            var rb = new RankedBag<int>();
            var gc = (System.Collections.Generic.ICollection<int>) rb;

            gc.Add (5);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (new int[] { 5 }, rb));
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (new int[] { 5 }, gc));
        }


        [TestMethod]
        public void UnitRb_Clear()
        {
            var rb = new RankedBag<int>();

            for (int ix = 0; ix < 50; ++ix)
                rb.Add (ix);

            Assert.AreEqual (50, rb.Count);

            int k1 = 0;
            foreach (var i1 in rb.Reverse())
                ++k1;
            Assert.AreEqual (50, k1);

            rb.Clear();

            int k2 = 0;
            foreach (var i1 in rb.Reverse())
                ++k2;
            Assert.AreEqual (0, k2);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRb_ContainsAll_ArgumentNull()
        {
            var rb = new RankedBag<int> { 5 };
            var zz = rb.ContainsAll (null);
        }

        [TestMethod]
        public void UnitRb_ContainsAll()
        {
            var rb1 = new RankedBag<int> { Capacity=4 };
            foreach (var ii in new int[] { 3, 5, 5, 5, 7, 7, 9 })
                rb1.Add (ii);

            var rb2 = new RankedBag<int> { Capacity=4 };
            foreach (var ii in rb1)
                rb2.Add (ii);

            Assert.IsTrue (rb1.ContainsAll (new int[] { }));
            Assert.IsTrue (rb1.ContainsAll (new int[] { 5, 5 }));
            Assert.IsTrue (rb1.ContainsAll (new int[] { 5, 5, 5 }));
            Assert.IsTrue (rb1.ContainsAll (new int[] { 5, 7 }));
            Assert.IsFalse (rb1.ContainsAll (new int[] { 5, 5, 5, 5 }));
            Assert.IsFalse (rb1.ContainsAll (new int[] { 1, 2, 3, 4, 5, 6, 7, 8 }));

            Assert.IsTrue (rb1.ContainsAll (rb2));
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRb_CopyTo1_ArgumentNull()
        {
            var rb = new RankedBag<int> { 1 };
            rb.CopyTo (null);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRb_CopyTo1_Argument()
        {
            var rb = new RankedBag<int> { 1, 11 };
            var d1 = new int[1];
            rb.CopyTo (d1);
        }

        [TestMethod]
        public void UnitRb_CopyTo1()
        {
            var e5 = new int[] { 3, 5, 5, 7, 0 };
            var s4 = new int[] { 3, 5, 5, 7 };
            var d4 = new int[4];
            var d5 = new int[5];
            var rb = new RankedBag<int> (s4);

            rb.CopyTo (d4);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (s4, d4));

            rb.CopyTo (d5);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (e5, d5));
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRb_CopyTo2_ArgumentNull()
        {
            var rb = new RankedBag<int> { 2 };
            rb.CopyTo (null, 0);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRb_CopyTo2_ArgumentOutOfRange()
        {
            var rb = new RankedBag<int> { 2, 22 };
            var d2 = new int[2];
            rb.CopyTo (d2, -1);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRb_CopyTo2_Argument()
        {
            var rb = new RankedBag<int> { 2, 22 };
            var d2 = new int[2];
            rb.CopyTo (d2, 1);
        }

        [TestMethod]
        public void UnitRb_CopyTo2()
        {
            var s2 = new int[] { 3, 5 };
            var e4 = new int[] { 0, 3, 5, 0 };
            var d2 = new int[2];
            var d4 = new int[4];
            var rb = new RankedBag<int> (s2);

            rb.CopyTo (d2, 0);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (s2, d2));

            rb.CopyTo (d4, 1);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (e4, d4));
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRb_CopyTo3_ArgumentNull()
        {
            var rb = new RankedBag<int> { 2 };
            rb.CopyTo (null, 0, 1);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRb_CopyTo3A_ArgumentOutOfRange()
        {
            var rb = new RankedBag<int> { 2 };
            var d2 = new int[2];
            rb.CopyTo (d2, -1, 1);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRb_CopyTo3B_ArgumentOutOfRange()
        {
            var rb = new RankedBag<int> { 2 };
            var d2 = new int[2];
            rb.CopyTo (d2, 0, -1);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRb_CopyTo3A_Argument()
        {
            var rb = new RankedBag<int> { 2, 22 };
            var d2 = new int[2];
            rb.CopyTo (d2, 1, 2);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRb_CopyTo3B_Argument()
        {
            var rb = new RankedBag<int> { 2, 22 };
            var d3 = new int[3];
            rb.CopyTo (d3, 1, 3);
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
            var rb = new RankedBag<int> (s4);

            rb.CopyTo (d2, 1, 0);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (e2, d2));

            rb.CopyTo (d3, 1, 2);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (e3, d3));

            rb.CopyTo (d4, 0, 4);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (s4, d4));

            rb.CopyTo (d5, 1, 3);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (e5, d5));
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRb_ocCopyTo2_ArgumentNull()
        {
            var rb = new RankedBag<int>() { 2 };
            var oc = (System.Collections.ICollection) rb;
            oc.CopyTo (null, 0);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRb_ocCopyTo2_ArgumentOutOfRange()
        {
            var rb = new RankedBag<int> { 3,5 };
            var oc = (System.Collections.ICollection) rb;
            var d2 = new object[2];

            oc.CopyTo (d2, -1);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRb_ocCopyTo2A_Argument()
        {
            var rb = new RankedBag<int> { 3,5 };
            var oc = (System.Collections.ICollection) rb;
            var d2 = new object[2];

            oc.CopyTo (d2, 1);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRb_ocCopyTo2C_Argument()
        {
            var rb = new RankedBag<int> { 3,7 };
            var oc = (System.Collections.ICollection) rb;
            var multi = new int[1,2];
            oc.CopyTo (multi, 0);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRb_ocCopyTo2D_Argument()
        {
            var rb = new RankedBag<int> { 5,7 };
            var oc = (System.Collections.ICollection) rb;
            var a11 = Array.CreateInstance (typeof (int), new int[]{1}, new int[]{1});

            oc.CopyTo (a11, 1);
        }

        [TestMethod]
        public void UnitRb_ocCopyTo2()
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
            var rb = new RankedBag<int> (s4);
            var oc = (System.Collections.ICollection) rb;

            oc.CopyTo (d4, 0);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (s4, d4));

            oc.CopyTo (d6, 1);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (e6, d6));
        }


        [TestMethod]
        public void UnitRb_GetCount()
        {
            var rb0 = new RankedBag<int>();
            var rb = new RankedBag<int> (new int[] { 3, 5, 5, 5, 7 });

            Assert.AreEqual (0, rb0.GetCount (4));

            Assert.AreEqual (0, rb.GetCount (1));
            Assert.AreEqual (1, rb.GetCount (3));
            Assert.AreEqual (3, rb.GetCount (5));
            Assert.AreEqual (1, rb.GetCount (7));
            Assert.AreEqual (0, rb.GetCount (9));
        }


        [TestMethod]
        public void UnitRb_GetDistinctCount()
        {
            var rb0 = new RankedBag<int>();
            var rb = new RankedBag<int> (new int[] { 3, 5, 5, 5, 7 });

            Assert.AreEqual (0, rb0.GetDistinctCount());
            Assert.AreEqual (3, rb.GetDistinctCount());
        }


        [TestMethod]
        public void StressRb_Counts()
        {
            int reps = 100;
            for (int order = 4; order <= 136; order += 8)
            {
                var rb = new RankedBag<int> { Capacity=order };

                for (int ix = 1; ix <= reps; ++ix)
                    rb.Add (ix, ix);

                for (int ix = 1; ix <= reps; ++ix)
                    Assert.AreEqual (ix, rb.GetCount (ix));

                Assert.AreEqual ((reps+1) * reps / 2, rb.Count);
                Assert.AreEqual (reps, rb.GetDistinctCount());
            }
        }


        [TestMethod]
        public void UnitRb_IndexOf()
        {
            var rb0 = new RankedBag<int>();
            var rb = new RankedBag<int> { Capacity=4 };

            foreach (int x in new int[] { 3, 5, 5, 7, 7 })
                rb.Add (x);

            Assert.AreEqual (~0, rb0.IndexOf (9));

            Assert.AreEqual (~0, rb.IndexOf (2));
            Assert.AreEqual ( 0, rb.IndexOf (3));
            Assert.AreEqual (~1, rb.IndexOf (4));
            Assert.AreEqual ( 1, rb.IndexOf (5));
            Assert.AreEqual (~3, rb.IndexOf (6));
            Assert.AreEqual ( 3, rb.IndexOf (7));
            Assert.AreEqual (~5, rb.IndexOf (8));
        }


        [TestMethod]
        public void UnitRb_Remove1()
        {
            var rb0 = new RankedBag<int>();
            var rb = new RankedBag<int> { Capacity=4 };

            foreach (int ii in new int[] { 3, 5, 5, 7, 7, 7, 9 })
                rb.Add (ii);

            bool rem0 = rb0.Remove (0);
            Assert.IsFalse (rem0);

            bool rem2 = rb.Remove (2);
            Assert.IsFalse (rem2);

            bool rem7 = rb.Remove (7);
            Assert.IsTrue (rem7);
            Assert.AreEqual (4, rb.Count);

            bool rem5 = rb.Remove (5);
            Assert.IsTrue (rem5);
            Assert.AreEqual (2, rb.Count);

            bool rem10 = rb.Remove (10);
            Assert.IsFalse (rem10);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRb_Remove2_Argument()
        {
            var rb = new RankedBag<int>();
            rb.Remove (1, -1);
        }

        [TestMethod]
        public void UnitRb_Remove2()
        {
            var rb0 = new RankedBag<int>();
            var rb1 = new RankedBag<int> { Capacity=4 };
            var rb2 = new RankedBag<int> { Capacity=4 };
            var rb3 = new RankedBag<int> { Capacity=5 };

            foreach (int ii in new int[] { 3, 5, 5, 7, 7, 7, 9 })
                rb1.Add (ii);

            foreach (int ii in new int[] { 3, 3, 3, 5, 5, 5, 7, 7, 7, 9 })
                rb2.Add (ii);

            for (int ii=0; ii<41; ++ii)
                rb3.Add (ii/5);

            var rem0 = rb0.Remove (0, 1);
            Assert.AreEqual (0, rem0);

            var rem2 = rb1.Remove (2, 2);
            Assert.AreEqual (0, rem2);

            var rem70 = rb1.Remove (7, 0);
            Assert.AreEqual (0, rem70);

            var rem7 = rb1.Remove (7, 1);
            Assert.AreEqual (1, rem7);
            Assert.AreEqual (6, rb1.Count);

            var rem5 = rb1.Remove (5, 3);
            Assert.AreEqual (2, rem5);
            Assert.AreEqual (4, rb1.Count);

            var rem9 = rb1.Remove (10);
            Assert.IsFalse (rem9);

            var rem53 = rb2.Remove (5, 3);
            Assert.AreEqual (3, rem53);

            var rem33 = rb2.Remove (3, 3);
            Assert.AreEqual (3, rem33);

            var rem99 = rb2.Remove (9, 9);
            Assert.AreEqual (1, rem99);

            Assert.AreEqual (3, rb2.Count);

            var rem35 = rb3.Remove (3, 9);
            Assert.AreEqual (5, rem35);
            Assert.AreEqual (36, rb3.Count);
            Assert.IsFalse (rb3.Contains (3));

            var rem65 = rb3.Remove (6, Int32.MaxValue);
            Assert.AreEqual (5, rem65);
            Assert.AreEqual (31, rb3.Count);
            Assert.IsFalse (rb3.Contains (6));
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRb_RemoveAll_ArgumentNull()
        {
            var rb = new RankedBag<int>();
            rb.RemoveAll (null);
        }

        [TestMethod]
        public void UnitRb_RemoveAll()
        {
            var rb0 = new RankedBag<int>();
            var rb = new RankedBag<int> { Capacity=4 };

            foreach (var ii in new int[] { 3, 5, 5, 7, 7 })
                rb.Add (ii);

            int rem0 = rb0.RemoveAll (new int[] { 2 });
            Assert.AreEqual (0, rem0);

            int rem1 = rb.RemoveAll (new int[] { });
            Assert.AreEqual (0, rem1);

            int rem2 = rb.RemoveAll (new int[] { 2 });
            Assert.AreEqual (0, rem0);

            int rem57 = rb.RemoveAll (new int[] { 5, 7 });
            Assert.AreEqual (2, rem57);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (new int[] { 3, 5, 7 }, rb));

            int rem4 = rb.RemoveAll (rb);
            Assert.AreEqual (3, rem4);
            Assert.AreEqual (0, rb.Count);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRb_RemoveAtA_ArgumentOutOfRange()
        {
            var rb = new RankedBag<int> { 1 };
            rb.RemoveAt (-1);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRb_RemoveAtB_ArgumentOutOfRange()
        {
            var rb = new RankedBag<int>();
            rb.RemoveAt (0);
        }

        [TestMethod]
        public void UnitRb_RemoveAt()
        {
            var rb1 = new RankedBag<int>();
            for (int i1 = 0; i1 < 5000; ++i1)
                rb1.Add (i1);

            for (int i2 = 4900; i2 >= 0; i2 -= 100)
                rb1.RemoveAt (i2);

            for (int i3 = 0; i3 < 5000; ++i3)
                if (i3 % 100 == 0)
                    Assert.IsFalse (rb1.Contains (i3));
                else
                    Assert.IsTrue (rb1.Contains (i3));

            var rb2 = new RankedBag<int> { Capacity=4 };
            for (int ii = 0; ii < 8; ++ii)
                rb2.Add (ii);
            rb2.RemoveAt (3);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (new int[] { 0, 1, 2, 4, 5, 6, 7 }, rb2));
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRb_RemoveRangeA_ArgumentOutOfRange()
        {
            var rb = new RankedBag<int>();
            rb.RemoveRange (-1, 0);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRb_RemoveRangeB_ArgumentOutOfRange()
        {
            var rb = new RankedBag<int>();
            rb.RemoveRange (0, -1);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRb_RemoveRange_Argument()
        {
            var rb = new RankedBag<int> { 3,5 };
            rb.RemoveRange (1, 2);
        }

        [TestMethod]
        public void UnitRb_RemoveRange()
        {
            var rb = new RankedBag<int> { Capacity=7 };
            for (int ii=0; ii<20; ++ii) rb.Add (ii);

            rb.RemoveRange (20, 0);
            Assert.AreEqual (20, rb.Count);

            rb.RemoveRange (12, 4);
            Assert.AreEqual (16, rb.Count);
#if DEBUG
            rb.SanityCheck();
#endif
        }


        [TestMethod]
        public void UnitRb_RemoveWhere()
        {
            var rb0 = new RankedBag<int>();
            var rb = new RankedBag<int> (new int[] { 3, 4, 5, 5, 6, 6, 7, 7, 8 });


            rb0.RemoveWhere (IsEven);
            Assert.AreEqual (0, rb0.Count);

            rb.RemoveWhere (IsEven);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (new int[] { 3, 5, 5, 7, 7 }, rb));
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRb_RetainAll_ArgumentNull()
        {
            var rb = new RankedBag<int> { 2 };
            rb.RetainAll (null);
        }

        [TestMethod]
        public void UnitRb_RetainAll()
        {
            var rb0 = new RankedBag<int>();
            var rb1 = new RankedBag<int> { Capacity=4 };
            var rb2 = new RankedBag<int> { 3, 5, 7 };

            foreach (int ii in new int[] { 3, 4, 5, 5, 6, 6, 6, 7, 7, 8 })
                rb1.Add (ii);

            int del0 = rb0.RetainAll (new int[] { 2, 4 });
            Assert.AreEqual (0, del0);
            Assert.AreEqual (0, rb0.Count);

            int del1 = rb1.RetainAll (new int[] { 1, 4, 4, 6, 6, 9 });
            Assert.AreEqual (7, del1);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (new int[] { 4, 6, 6 }, rb1));

            int del2 = rb2.RetainAll (new int[] { });
            Assert.AreEqual (3, del2);
            Assert.AreEqual (0, rb2.Count);
        }


        [TestMethod]
        public void UnitRb_TryGetEQ()
        {
            var rb = new RankedBag<string> (StringComparer.OrdinalIgnoreCase) { Capacity=4 };

            bool r0a = rb.TryGet ("AA", out string k0a);
            bool r0b = rb.TryGet ("BB", out string k0b);
            Assert.IsFalse (r0a);
            Assert.AreEqual (default (string), k0a);
            Assert.IsFalse (r0b);
            Assert.AreEqual (default (string), k0b);

            for (char cx = 'b'; cx <= 'y'; ++cx)
            {
                rb.Add (cx.ToString());
                rb.Add (Char.ToUpperInvariant(cx).ToString());
            }

            for (char c1 = 'b'; c1 <= 'y'; ++c1)
            {
                bool r1 = rb.TryGet (c1.ToString(), out string k1);

                Assert.IsTrue (r1);
                Assert.AreEqual (c1.ToString(), k1);
            }

            bool r2 = rb.TryGet ("A", out string k2);
            bool r3 = rb.TryGet ("z", out string k3);
            Assert.IsFalse (r2);
            Assert.AreEqual (default (string), k2);
            Assert.IsFalse (r3);
            Assert.AreEqual (default (string), k3);
        }


        [TestMethod]
        public void UnitRb_TryGetLELT()
        {
            var rb = new RankedBag<string> (StringComparer.OrdinalIgnoreCase) { Capacity=4 };

            bool r0a = rb.TryGetLessThanOrEqual ("AA", out string k0a);
            bool r0b = rb.TryGetLessThan ("BB", out string k0b);
            Assert.IsFalse (r0a);
            Assert.AreEqual (default (string), k0a);
            Assert.IsFalse (r0b);
            Assert.AreEqual (default (string), k0b);

            for (char cx = 'b'; cx <= 'y'; ++cx)
            {
                rb.Add (cx.ToString());
                rb.Add (Char.ToUpperInvariant(cx).ToString());
            }

            for (char c1 = 'c'; c1 <= 'y'; ++c1)
            {
                bool r1a = rb.TryGetLessThanOrEqual (c1.ToString().ToUpper(), out string k1a);
                bool r1b = rb.TryGetLessThan (c1.ToString(), out string k1b);

                Assert.IsTrue (r1a);
                Assert.AreEqual (c1.ToString(), k1a);
                Assert.IsTrue (r1b);
                Assert.AreEqual (((char) (c1-1)).ToString().ToUpper(), k1b);
            }

            bool r2a = rb.TryGetLessThanOrEqual ("A", out string k2a);
            bool r2b = rb.TryGetLessThan ("a", out string k2b);
            Assert.IsFalse (r2a);
            Assert.AreEqual (default (string), k2a);
            Assert.IsFalse (r2b);
            Assert.AreEqual (default (string), k2b);

            bool r3a = rb.TryGetLessThanOrEqual ("B", out string k3a);
            bool r3b = rb.TryGetLessThan ("b", out string k3b);
            Assert.IsTrue (r3a);
            Assert.AreEqual ("b", k3a);
            Assert.IsFalse (r3b);
            Assert.AreEqual (default (string), k3b);

            bool r4a = rb.TryGetLessThanOrEqual ("Z", out string k4a);
            bool r4b = rb.TryGetLessThan ("z", out string k4b);
            Assert.IsTrue (r4a);
            Assert.AreEqual ("Y", k4a);
            Assert.IsTrue (r4b);
            Assert.AreEqual ("Y", k4b);
        }


        [TestMethod]
        public void UnitRb_TryGetGEGT()
        {
            var rb = new RankedBag<string> (StringComparer.OrdinalIgnoreCase) { Capacity=4 };

            bool r0a = rb.TryGetGreaterThanOrEqual ("AA", out string k0a);
            bool r0b = rb.TryGetGreaterThan ("BB", out string k0b);
            Assert.IsFalse (r0a);
            Assert.AreEqual (default (string), k0a);
            Assert.IsFalse (r0b);
            Assert.AreEqual (default (string), k0b);

            for (char cx = 'b'; cx <= 'y'; ++cx)
            {
                rb.Add (cx.ToString());
                rb.Add (Char.ToUpperInvariant(cx).ToString());
            }

            for (char c1 = 'b'; c1 <= 'x'; ++c1)
            {
                bool r1a = rb.TryGetGreaterThanOrEqual (c1.ToString().ToUpper(), out string k1a);
                bool r1b = rb.TryGetGreaterThan (c1.ToString(), out string k1b);

                Assert.IsTrue (r1a);
                Assert.AreEqual (c1.ToString(), k1a);
                Assert.IsTrue (r1b);
                Assert.AreEqual (((char) (c1+1)).ToString(), k1b);
            }

            bool r2a = rb.TryGetGreaterThanOrEqual ("A", out string k2a);
            bool r2b = rb.TryGetGreaterThan ("a", out string k2b);
            Assert.IsTrue (r2a);
            Assert.AreEqual ("b", k2a);
            Assert.IsTrue (r2b);
            Assert.AreEqual ("b", k2b);

            bool r3a = rb.TryGetGreaterThanOrEqual ("Y", out string k3a);
            bool r3b = rb.TryGetGreaterThan ("y", out string k3b);
            Assert.IsTrue (r3a);
            Assert.AreEqual ("y", k3a);
            Assert.IsFalse (r3b);
            Assert.AreEqual (default (string), k3b);

            bool r4a = rb.TryGetGreaterThanOrEqual ("Z", out string k4a);
            bool r4b = rb.TryGetGreaterThan ("z", out string k4b);
            Assert.IsFalse (r4a);
            Assert.AreEqual (default (string), k4a);
            Assert.IsFalse (r4b);
            Assert.AreEqual (default (string), k4b);
        }

        #endregion

        #region Test enumeration

        [TestMethod]
        public void UnitRb_ElementsBetween()
        {
            var rb0 = new RankedBag<int>();
            var rb1 = new RankedBag<int> { Capacity=4 };
            var rb2 = new RankedBag<int> (new int[] { 5, 5, 5, 5, 5 });

            foreach (var k1 in new int[] { 3, 4, 5, 5, 6, 6, 7, 7, 8 })
                rb1.Add (k1);

            var d0 = new System.Collections.Generic.List<int> (rb0.ElementsBetween (2, 4));
            Assert.AreEqual (0, d0.Count);

            var d1 = new System.Collections.Generic.List<int> (rb1.ElementsBetween (5, 6));
            Assert.AreEqual (4, d1.Count);

            var d2 = new System.Collections.Generic.List<int> (rb1.ElementsBetween (5, 5));
            Assert.AreEqual (2, d2.Count);

            var d3 = new System.Collections.Generic.List<int> (rb2.ElementsBetween (5, 5));
            Assert.AreEqual (5, d3.Count);

            var d4 = new System.Collections.Generic.List<int> (rb2.ElementsBetween (1, 2));
            Assert.AreEqual (0, d4.Count);

            var d5 = new System.Collections.Generic.List<int> (rb2.ElementsBetween (9, 11));
            Assert.AreEqual (0, d5.Count);
        }


        [TestMethod]
        public void UnitRb_ElementsFrom()
        {
            var rb0 = new RankedBag<int>();
            var rb1 = new RankedBag<int> { Capacity=4 };
            var rb2 = new RankedBag<int> (new int[] { 5, 5, 5, 5, 5 });

            foreach (var i1 in new int[] { 3, 4, 5, 5, 6, 6, 7, 7, 8 })
                rb1.Add (i1);

            var d0 = new System.Collections.Generic.List<int> (rb0.ElementsFrom (0));
            Assert.AreEqual (0, d0.Count);

            var d1 = new System.Collections.Generic.List<int> (rb1.ElementsFrom (6));
            Assert.AreEqual (5, d1.Count);

            var d2 = new System.Collections.Generic.List<int> (rb1.ElementsFrom (1));
            Assert.AreEqual (9, d2.Count);

            var d3 = new System.Collections.Generic.List<int> (rb2.ElementsFrom (5));
            Assert.AreEqual (5, d3.Count);

            var d5 = new System.Collections.Generic.List<int> (rb2.ElementsFrom (9));
            Assert.AreEqual (0, d5.Count);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRb_ElementsBetweenIndexesA_ArgumentOutOfRange()
        {
            var rb = new RankedBag<int> { 0, 1, 2 };
            foreach (var val in rb.ElementsBetweenIndexes (-1, 0))
            { }
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRb_ElementsBetweenIndexesB_ArgumentOutOfRange()
        {
            var rb = new RankedBag<int> { 0, 1, 2 };
            foreach (var val in rb.ElementsBetweenIndexes (3, 0))
            { }
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRb_ElementsBetweenIndexesC_ArgumentOutOfRange()
        {
            var rb = new RankedBag<int> { 0, 1, 2 };
            foreach (var val in rb.ElementsBetweenIndexes (0, -1))
            { }
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRb_ElementsBetweenIndexesD_ArgumentOutOfRange()
        {
            var rb = new RankedBag<int> { 0, 1, 2 };
            foreach (var val in rb.ElementsBetweenIndexes (0, 3))
            { }
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRb_ElementsBetweenIndexes_Argument()
        {
            var rb = new RankedBag<int> { 0, 1, 2 };
            foreach (var val in rb.ElementsBetweenIndexes (2, 1))
            { }
        }

        [TestMethod]
        public void UnitRb_ElementsBetweenIndexes()
        {
            int n = 33;
            var rb = new RankedBag<int> { Capacity=4 };
            for (int ii = 0; ii < n; ++ii)
                rb.Add (ii);

            for (int p1 = 0; p1 < n; ++p1)
                for (int p2 = p1; p2 < n; ++p2)
                {
                    int actual = 0;
                    foreach (var val in rb.ElementsBetweenIndexes (p1, p2))
                        actual += val;

                    int expected = (p2 - p1 + 1) * (p1 + p2) / 2;
                    Assert.AreEqual (expected, actual);
                }
        }


        [TestMethod]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRb_EtorOverflow_InvalidOperation()
        {
            var rb = new RankedBag<int> { Capacity=4 };
            for (int ii=0; ii<10; ++ii) rb.Add (ii);

            var etor = rb.GetEnumerator();
            while (etor.MoveNext())
            { }

            var val = ((System.Collections.IEnumerator) etor).Current;
        }

        [TestMethod]
        public void UnitRb_gcGetEnumerator()
        {
            var rb = new RankedBag<int> { 5 };

            var gcBag = ((System.Collections.Generic.ICollection<int>) rb);
            var gcEtor = gcBag.GetEnumerator();
            gcEtor.MoveNext();
            Assert.AreEqual (5, gcEtor.Current);
        }

        [TestMethod]
        public void UnitRb_GetEnumerator()
        {
            var rb = new RankedBag<int> { Capacity=4 };
            for (int ii=0; ii<10; ++ii) rb.Add (ii);

            var etor = rb.GetEnumerator();

            int ix = 0;
            while (etor.MoveNext())
            {
                int gActual = etor.Current;
                object oActual = ((System.Collections.IEnumerator) etor).Current;
                Assert.AreEqual (ix, gActual);
                Assert.AreEqual (ix, oActual);
                ++ix;
            }
            Assert.AreEqual (10, ix);

            int gActualEnd = etor.Current;
            Assert.AreEqual (default (int), gActualEnd);

            bool isValid = etor.MoveNext();
            Assert.IsFalse (isValid);
        }

        [TestMethod]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRb_EtorHotUpdate()
        {
            var rb = new RankedBag<int> { Capacity=4 };
            for (int ii=0; ii<10; ++ii) rb.Add (ii);

            int n = 0;
            foreach (int kv in rb)
            {
                if (++n == 2)
                    rb.Add (49);
            }
        }


        [TestMethod]
        public void UnitRb_oReset()
        {
            var rb = new RankedBag<int> { Capacity=4 };
            var ia = new int[] { 1,2,2,5,8,8,9 };
            foreach (var x in ia)
                rb.Add (x);

            var etor = rb.GetEnumerator();

            int ix1 = 0;
            while (etor.MoveNext())
            {
                Assert.AreEqual (ia[ix1], etor.Current);
                ++ix1;
            }
            Assert.AreEqual (ia.Length, ix1);

            ((System.Collections.IEnumerator) etor).Reset();

            int ix2 = 0;
            while (etor.MoveNext())
            {
                Assert.AreEqual (ia[ix2], etor.Current);
                ++ix2;
            }
            Assert.AreEqual (ia.Length, ix2);
        }

        #endregion
    }
#endif
}
