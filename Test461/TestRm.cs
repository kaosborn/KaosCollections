//
// Library: KaosCollections
// File:    TestRm.cs
//

#if ! TEST_BCL

using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kaos.Collections;

namespace Kaos.Test.Collections
{
    public partial class TestBtree
    {
        #region Test constructors

        [TestMethod]
        public void UnitRm_Inheritance()
        {
            var rm = new RankedMap<string,int>();

            Assert.IsTrue (rm is System.Collections.Generic.ICollection<KeyValuePair<string,int>>);
            Assert.IsTrue (rm is System.Collections.Generic.IEnumerable<KeyValuePair<string,int>>);
            Assert.IsTrue (rm is System.Collections.IEnumerable);
            Assert.IsTrue (rm is System.Collections.ICollection);

            Assert.IsTrue (rm is System.Collections.Generic.IReadOnlyCollection<KeyValuePair<string,int>>);

            Assert.IsTrue (rm.Keys is System.Collections.Generic.ICollection<string>);
            Assert.IsTrue (rm.Keys is System.Collections.Generic.IEnumerable<string>);
            Assert.IsTrue (rm.Keys is System.Collections.IEnumerable);
            Assert.IsTrue (rm.Keys is System.Collections.ICollection);
            Assert.IsTrue (rm.Keys is System.Collections.Generic.IReadOnlyCollection<string>);

            Assert.IsTrue (rm.Values is System.Collections.Generic.ICollection<int>);
            Assert.IsTrue (rm.Values is System.Collections.Generic.IEnumerable<int>);
            Assert.IsTrue (rm.Values is System.Collections.IEnumerable);
            Assert.IsTrue (rm.Values is System.Collections.ICollection);
            Assert.IsTrue (rm.Values is System.Collections.Generic.IReadOnlyCollection<int>);
        }


        [TestMethod]
        public void UnitRm_Ctor0Empty()
        {
            var rm = new RankedMap<string,int>();
            Assert.AreEqual (0, rm.Count);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRm_Ctor1NoComparer_InvalidOperation()
        {
            var comp0 = (Comparer<Person>) null;
            var rm = new RankedMap<Person,int> (comp0);

            rm.Add (new Person ("Carlos"), 1);
            rm.Add (new Person ("Macron"), 2);
        }

        [TestMethod]
        public void UnitRm_Ctor1A1()
        {
            var rm = new RankedMap<string,int> (StringComparer.OrdinalIgnoreCase);

            rm.Add ("AAA", 0);
            rm.Add ("bbb", 1);
            rm.Add ("CCC", 2);
            rm.Add ("ddd", 3);

            int actualPosition = 0;
            foreach (KeyValuePair<string,int> pair in rm)
            {
                Assert.AreEqual (actualPosition, pair.Value);
                ++actualPosition;
            }

            Assert.AreEqual (4, actualPosition);
        }

        [TestMethod]
        public void UnitRm_Ctor1A2()
        {
            var rm = new RankedMap<string,int> (StringComparer.Ordinal);
            rm.Add ("AAA", 0);
            rm.Add ("bbb", 2);
            rm.Add ("CCC", 1);
            rm.Add ("ddd", 3);

            int actualPosition = 0;
            foreach (KeyValuePair<string,int> kv in rm)
            {
                Assert.AreEqual (actualPosition, kv.Value);
                ++actualPosition;
            }

            Assert.AreEqual (4, actualPosition);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRm_Ctor1B_ArgumentNull()
        {
            ICollection<KeyValuePair<int,int>> gcp1 = null;
            ICollection<KeyValuePair<int,int>> gcp2 = new RankedMap<int,int> (gcp1);
        }


        [TestMethod]
        public void UnitRm_Ctor1B()
        {
            var gsl = new SortedList<string,int> { {"Gremlin",1}, {"Pacer",2} };
            var gcp = (ICollection<KeyValuePair<string,int>>) gsl;

            var rm = new RankedMap<string,int> (gcp);

            int actual = 0;
            foreach (KeyValuePair<string,int> kv in rm)
            {
                Assert.AreEqual (gsl.Keys[actual], kv.Key);
                Assert.AreEqual (gsl.Values[actual], kv.Value);
                ++actual;
            }

            Assert.AreEqual (2, actual);
        }

        [TestMethod]
        public void UnitRm_Ctor2()
        {
            ICollection<KeyValuePair<Person,int>> cpl = new SortedList<Person,int>(new PersonComparer());
            cpl.Add (new KeyValuePair<Person,int> (new Person ("fay"), 1));
            cpl.Add (new KeyValuePair<Person,int> (new Person ("ann"), 2));
            cpl.Add (new KeyValuePair<Person,int> (new Person ("sam"), 3));

            var people = new RankedMap<Person,int> (cpl, new PersonComparer());

            Assert.AreEqual (3, people.Count);
        }

        #endregion

        #region Test properties

        [TestMethod]
        public void UnitRm_Comparer()
        {
            var rm = new RankedMap<string,int>();
            IComparer<string> comp = rm.Comparer;
            Assert.AreEqual (Comparer<string>.Default, comp);
        }


        [TestMethod]
        public void UnitRm_gcIsReadonly()
        {
            var rm = new RankedMap<string,int>();
            var gcp = (ICollection<KeyValuePair<string,int>>) rm;
            Assert.IsFalse (gcp.IsReadOnly);
        }


        [TestMethod]
        public void UnitRm_ocIsSynchronized()
        {
            var rm = new RankedMap<string,int>();
            var oc = (ICollection) rm;
            Assert.IsFalse (oc.IsSynchronized);
        }


        [TestMethod]
        public void UnitRm_MinMax()
        {
            var rm = new RankedMap<int,int> { Capacity=4 };

            int min0 = rm.MinKey;
            int max0 = rm.MaxKey;

            Assert.AreEqual (default (int), min0);
            Assert.AreEqual (default (int), max0);

            for (int i1 = 1; i1 <= 99; ++i1)
                rm.Add (i1, i1 + 100);

            int min = rm.MinKey;
            int max = rm.MaxKey;

            Assert.AreEqual (1, min);
            Assert.AreEqual (99, max);
        }


        [TestMethod]
        public void UnitRm_ocSyncRoot()
        {
            var rm = new RankedMap<int,int>();
            var oc = (ICollection) rm;
            Assert.IsFalse (oc.SyncRoot.GetType().IsValueType);
        }

        #endregion

        #region Test methods

        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRm_Add_ArgumentNull()
        {
            var rm = new RankedMap<string,int>();
            rm.Add (null, 0);
        }

        [TestMethod]
        public void UnitRm_Add()
        {
            var rm = new RankedMap<string,int> { {"foo",1}, {"foo",2}, {"zax",3} };
            Assert.AreEqual (3, rm.Count);
        }

        [TestMethod]
        public void UnitRm_pcAddPair()
        {
            var rm = new RankedMap<string,int>();
            var pc = (ICollection<KeyValuePair<string,int>>) rm;

            var p1 = new KeyValuePair<string,int> ("beta", 1);
            var p2 = new KeyValuePair<string,int> (null, 98);
            var p3 = new KeyValuePair<string,int> (null, 99);

            pc.Add (p1);
            Assert.IsTrue (rm.ContainsKey ("beta"));

            // Adding a null key is allowed here to be consistent with SortedDictionary.
            pc.Add (p2);
            Assert.AreEqual (2, rm.Count);

            // Using the ICollection interface allows multiple nulls.
            pc.Add (p3);
            Assert.AreEqual (3, rm.Count);
        }



        [TestMethod]
        public void UnitRm_Clear()
        {
            var rm = new RankedMap<string,int> { Capacity=4 };

            rm.Add ("foo", 1);
            rm.Add ("foo", 2);
            rm.Add ("zax", 3);
            rm.Add ("zax", 4);
            rm.Add ("fod", 5);

            Assert.AreEqual (5, rm.Count);

            rm.Clear();

            Assert.AreEqual (0, rm.Count);

            int actualCount = 0;
            foreach (var pair in rm)
                ++actualCount;

            Assert.AreEqual (0, actualCount);
        }


        [TestMethod]
        public void UnitRm_pcContainsA()
        {
            var rm = new RankedMap<string,int> { Capacity=4 };
            var pc = (ICollection<KeyValuePair<string,int>>) rm;
            var nullKv = new KeyValuePair<string,int> (null, 0);
            var zedKv = new KeyValuePair<string,int> ("z", 0);

            foreach (var kv in greek)
                pc.Add (kv);

            foreach (var kv in greek)
                Assert.IsTrue (pc.Contains (kv));

            Assert.IsFalse (pc.Contains (zedKv));
            Assert.IsFalse (pc.Contains (nullKv));

            pc.Add (nullKv);
            Assert.IsTrue (pc.Contains (nullKv));
        }

        [TestMethod]
        public void UnitRm_pcContainsB()
        {
            var rm = new RankedMap<int,int> { Capacity=4 };
            var pc = (ICollection<KeyValuePair<int,int>>) rm;

            pc.Add (new KeyValuePair<int,int> (1,11));
            pc.Add (new KeyValuePair<int,int> (1,12));
            pc.Add (new KeyValuePair<int,int> (1,13));
            pc.Add (new KeyValuePair<int,int> (2,21));
            pc.Add (new KeyValuePair<int,int> (2,22));
            pc.Add (new KeyValuePair<int,int> (2,23));
            pc.Add (new KeyValuePair<int,int> (2,24));

            Assert.IsTrue (pc.Contains (new KeyValuePair<int,int> (2,23)));

            pc.Remove (new KeyValuePair<int,int> (2,22));
            Assert.IsFalse (pc.Contains (new KeyValuePair<int,int> (2,22)));
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRm_ContainsKey_ArgumentNull()
        {
            var rm = new RankedMap<string,int> { {"beta",2} };
            var zz = rm.ContainsKey (null);
        }

        [TestMethod]
        public void UnitRm_ContainsKey()
        {
            var rm = new RankedMap<int,int>();

            int key1 = 26;
            int key2 = 36;
            rm.Add (key1, key1 * 10);

            Assert.IsTrue (rm.ContainsKey (key1));
            Assert.IsFalse (rm.ContainsKey (key2));
        }


        [TestMethod]
        public void UnitRm_ContainsValue()
        {
            var rm = new RankedMap<int,int>();

            int key1 = 26;
            int key2 = 36;
            int key3 = 46;
            rm.Add (key1, key1 + 1000);
            rm.Add (key3, key3 + 1000);

            Assert.IsTrue (rm.ContainsValue (key1 + 1000));
            Assert.IsFalse (rm.ContainsValue (key2 + 1000));
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRm_CopyTo_ArgumentNull()
        {
            var rm = new RankedMap<int,int>();
            rm.CopyTo (null, -1);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRm_CopyTo1_ArgumentOutOfRange()
        {
            var rm = new RankedMap<int,int>();
            var target = new KeyValuePair<int,int>[iVals1.Length];
            rm.CopyTo (target, -1);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRm_CopyTo1_Argument()
        {
            var rm = new RankedMap<int,int>();
            for (int key = 1; key < 10; ++key)
                rm.Add (key, key + 1000);

            var target = new KeyValuePair<int,int>[10];
            rm.CopyTo (target, 25);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRm_CopyTo2_Argument()
        {
            var rm = new RankedMap<int,int>();
            for (int key = 1; key < 10; ++key)
                rm.Add (key, key + 1000);

            var target = new KeyValuePair<int,int>[4];
            rm.CopyTo (target, 2);
        }


        [TestMethod]
        public void UnitRm_CopyTo2()
        {
            var rm = new RankedMap<int,int>();
            int offset = 1, size = 20;

            for (int i = 0; i < size; ++i)
                rm.Add (i + 1000, i + 10000);

            var pairs = new KeyValuePair<int,int>[size + offset];

            rm.CopyTo (pairs, offset);

            for (int i = 0; i < offset; ++i)
            {
                Assert.AreEqual (0, pairs[i].Key);
                Assert.AreEqual (0, pairs[i].Value);
            }

            for (int i = 0; i < size; ++i)
            {
                Assert.AreEqual (i + 1000, pairs[i + offset].Key);
                Assert.AreEqual (i + 10000, pairs[i + offset].Value);
            }
        }



        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRm_ocCopyTo_ArgumentNull()
        {
            var rm = new RankedMap<int,int>();
            var oc = (ICollection) rm;
            oc.CopyTo (null, 0);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRm_ocCopyTo_ArgumentOutOfRange()
        {
            var rm = new RankedMap<int,int>();
            var oc = (ICollection) rm;
            var target = new KeyValuePair<int,int>[1];
            oc.CopyTo (target, -1);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRm_ocCopyTo1_Argument()
        {
            var rm = new RankedMap<string,int> { {"foo",1} };
            var oc = (ICollection) rm;
            var target = new KeyValuePair<string,int>[1,2];
            oc.CopyTo (target, 0);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRm_ocCopyTo2_Argument()
        {
            var rm = new RankedMap<int,int>();
            var oc = (ICollection) rm;
            for (int key = 1; key < 10; ++key)
                rm.Add (key, key + 1000);

            var target = new KeyValuePair<int,int>[1];
            oc.CopyTo (target, 0);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRm_ocCopyToBadType_Argument()
        {
            var rm = new RankedMap<int,int> { {42,420 } };
            var oc = (ICollection) rm;

            var target = new string[5];
            oc.CopyTo (target, 0);
        }


        [TestMethod]
        public void UnitRm_oCopyTo()
        {
            var rm = new RankedMap<int,int>();
            var oc = (ICollection) rm;
            foreach (int key in iVals1)
                rm.Add (key, key + 1000);

            var target = new KeyValuePair<int,int>[iVals1.Length];

            oc.CopyTo (target, 0);

            for (int i = 0; i < iVals1.Length; ++i)
                Assert.AreEqual (target[i].Key + 1000, target[i].Value);
        }


        [TestMethod]
        public void UnitRm_GetCount()
        {
            var map0 = new RankedMap<int,int>();
            var map = new RankedMap<int,int> { Capacity=4 };
            foreach (int ii in new int[] { 3, 5, 5, 5, 7 }) map.Add (ii, -ii);

            Assert.AreEqual (0, map0.GetCount (9));

            Assert.AreEqual (0, map.GetCount (1));
            Assert.AreEqual (1, map.GetCount (3));
            Assert.AreEqual (3, map.GetCount (5));
            Assert.AreEqual (1, map.GetCount (7));
            Assert.AreEqual (0, map.GetCount (9));
        }


        [TestMethod]
        public void UnitRm_GetDistinctCount()
        {
            var map0 = new RankedMap<int,int>();
            var map = new RankedMap<int,int> { Capacity=4 };
            foreach (int ii in new int[] { 3, 5, 5, 5, 7 }) map.Add (ii, -ii);

            Assert.AreEqual (0, map0.GetDistinctCount());
            Assert.AreEqual (3, map.GetDistinctCount());
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRm_IndexOfKey_ArgumentNull()
        {
            var rm = new RankedMap<string,int>();
            var zz = rm.IndexOfKey (null);
        }

        [TestMethod]
        public void UnitRm_IndexOfKey1()
        {
            var rm = new RankedMap<int,int> { Capacity=5 };
            for (int ii = 0; ii < 500; ii+=2)
                rm.Add (ii, ii+1000);

            for (int ii = 0; ii < 500; ii+=2)
            {
                int ix = rm.IndexOfKey (ii);
                Assert.AreEqual (ii/2, ix);
            }

            int iw = rm.IndexOfKey (-1);
            Assert.AreEqual (~0, iw);

            int iy = rm.IndexOfKey (500);
            Assert.AreEqual (~250, iy);
        }

        [TestMethod]
        public void UnitRm_IndexOfKey2()
        {
            var rm = new RankedMap<int,int> { Capacity=4 };
            int n = 100, w=5;

            for (int ii = 0; ii < n; ++ii)
                for (int i2 = 0; i2 < w; ++i2)
                    rm.Add (ii, -ii);

            for (int ii = 0; ii < n; ++ii)
                Assert.AreEqual (ii*w, rm.IndexOfKey (ii));
        }


        [TestMethod]
        public void UnitRm_IndexOfValue()
        {
            var rm = new RankedMap<int,int> { Capacity=4 };
            for (int ii = 0; ii < 500; ++ii)
                rm.Add (ii, ii+1000);

            var ix1 = rm.IndexOfValue (1400);
            Assert.AreEqual (400, ix1);

            var ix2 = rm.IndexOfValue (88888);
            Assert.AreEqual (-1, ix2);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRm_Remove_ArgumentNull()
        {
            var rm = new RankedMap<string,int> { {"apple",4} };
            bool isRemoved = rm.Remove ((string) null);
        }


        [TestMethod]
        public void UnitRm_Remove1()
        {
            var rm = new RankedMap<int,int>();

            foreach (int key in iVals1)
                rm.Add (key, key + 1000);

            int c0 = rm.Count;
            bool isRemoved1 = rm.Remove (iVals1[3]);
            bool isRemoved2 = rm.Remove (iVals1[3]);

            Assert.IsTrue (isRemoved1);
            Assert.IsFalse (isRemoved2);
            Assert.AreEqual (c0 - 1, rm.Count);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRm_Remove2_Argument()
        {
            var rm = new RankedMap<int,int>();
            rm.Remove (1, -1);
        }

        [TestMethod]
        public void UnitRm_Remove2()
        {
            var rm0 = new RankedMap<int,int>();
            var rm1 = new RankedMap<int,int> { Capacity=5 };
            var rm2 = new RankedMap<int,int> { Capacity=5 };

            foreach (int ii in new int[] { 3, 5, 5, 7, 7, 7, 9 })
                rm1.Add (ii, -ii);

            foreach (int ii in new int[] { 3, 3, 3, 5, 5, 5, 7, 7, 7, 9 })
                rm2.Add (ii, -ii);

            var rem0 = rm0.Remove (0, 1);
            Assert.AreEqual (0, rem0);

            var rem2 = rm1.Remove (2, 2);
            Assert.AreEqual (0, rem2);

            var rem70 = rm1.Remove (7, 0);
            Assert.AreEqual (0, rem70);

            var rem7 = rm1.Remove (7, 1);
            Assert.AreEqual (1, rem7);
            Assert.AreEqual (6, rm1.Count);

            var rem5 = rm1.Remove (5, 3);
            Assert.AreEqual (2, rem5);
            Assert.AreEqual (4, rm1.Count);

            var rem9 = rm1.Remove (10);
            Assert.IsFalse (rem9);

            var rem53 = rm2.Remove (5, 3);
            Assert.AreEqual (3, rem53);

            var rem33 = rm2.Remove (3, Int32.MaxValue);
            Assert.AreEqual (3, rem33);

            var rem99 = rm2.Remove (9, 9);
            Assert.AreEqual (1, rem99);

            Assert.AreEqual (3, rm2.Count);
        }


        [TestMethod]
        public void UnitRm_pcRemovePairNullValue()
        {
            var rm = new RankedMap<int,string>();
            var pc = (ICollection<KeyValuePair<int,string>>) rm;

            rm.Add (3, "cc");
            rm.Add (5, "ee");
            rm.Add (4, null);

            bool isOK = pc.Remove (new KeyValuePair<int,string> (99, null));
            Assert.IsFalse (isOK);

            isOK = pc.Remove (new KeyValuePair<int,string> (4, null));
            Assert.IsTrue (isOK);

            isOK = rm.ContainsKey (4);
            Assert.IsFalse (isOK);
        }

        [TestMethod]
        public void UnitRm_pcRemovePair()
        {
            var rm = new RankedMap<string,int>();
            var pc = (ICollection<KeyValuePair<string,int>>) rm;

            var pair0 = new KeyValuePair<string,int> (null, 0);
            var pair1 = new KeyValuePair<string,int> ("ten", 10);
            var pair2 = new KeyValuePair<string,int> ("ten", 100);
            var pair3 = new KeyValuePair<string,int> ("twenty", 20);

            pc.Add (pair0);
            pc.Add (pair1);
            pc.Add (pair3);
            Assert.AreEqual (3, rm.Count);

            bool isRemoved = pc.Remove (pair0);
            Assert.IsTrue (isRemoved);
            Assert.AreEqual (2, rm.Count);

            isRemoved = pc.Remove (pair0);
            Assert.IsFalse (isRemoved);
            Assert.AreEqual (2, rm.Count);

            isRemoved = pc.Remove (pair2);
            Assert.AreEqual (2, rm.Count);

            isRemoved = pc.Remove (pair1);
            Assert.IsTrue (isRemoved);
            Assert.AreEqual (1, rm.Count);
        }

        [TestMethod]
        public void StressRm_pcRemovePair()
        {
            for (int cap = 4; cap < 40; ++cap)
            {
                var rm = new RankedMap<int,int> { Capacity=cap };
                var pc = (ICollection<KeyValuePair<int,int>>) rm;
                var ed = new int[13];
                int i1;

                for (i1 = 0; i1 < 6; ++i1)
                    for (int i2 = 0; i2 < 3; ++i2)
                    { rm.Add (i1, -i1); ++ed[i1]; }
                rm.Add (4, -4); ++ed[4];

                for (; i1 < 11; ++i1)
                    for (int i2 = 0; i2 < 2; ++i2)
                    { rm.Add (i1, -i1); ++ed[i1]; }

                for (; i1 < 13; ++i1)
                    for (int i2 = 0; i2 < 3; ++i2)
                    { rm.Add (i1, -i1); ++ed[i1]; }
                rm.Add (11, 11); ++ed[11];
                rm.Add (11,-11); ++ed[11];

                pc.Remove (new KeyValuePair<int,int> ( 0,  0)); ed[0] -= 3;
                pc.Remove (new KeyValuePair<int,int> ( 2, -2)); ed[2] -= 3;
                pc.Remove (new KeyValuePair<int,int> ( 7, -7)); ed[7] -= 2;
                pc.Remove (new KeyValuePair<int,int> (11, 11)); ed[11]-= 1;
                pc.Remove (new KeyValuePair<int,int> (12,-12)); ed[12]-= 3;

                for (int ix = 0; ix < ed.Length; ++ix)
                    Assert.AreEqual (ed[ix], rm.Keys.GetCount (ix), "cap=" + cap + ",ix=" + ix);
#if DEBUG
                rm.SanityCheck();
#endif
            }
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRm_RemoveAll_ArgumentNull()
        {
            var rm = new RankedMap<int,int>();
            rm.RemoveAll (null);
        }

        [TestMethod]
        public void UnitRm_RemoveAll()
        {
            var rm0 = new RankedMap<int,int>();
            var rm = new RankedMap<int,int> { Capacity=4 };

            foreach (var ii in new int[] { 3, 3, 5, 5, 7, 7 })
                rm.Add (ii, -ii);

            int rem0 = rm0.RemoveAll (new int[] { 2 });
            Assert.AreEqual (0, rem0);

            int rem2 = rm.RemoveAll (new int[] { 2 });
            Assert.AreEqual (0, rem0);

            int rem57 = rm.RemoveAll (new int[] { 3, 7 });
            Assert.AreEqual (2, rem57);
            Assert.IsTrue (System.Linq.Enumerable.SequenceEqual (new int[] { 3, 5, 5, 7 }, rm.Keys));
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRm_RemoveAtA_ArgumentOutOfRange()
        {
            var rm = new RankedMap<int,int>() { { 42, 24 } };
            rm.RemoveAt (-1);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRm_RemoveAtB_ArgumentOutOfRange()
        {
            var rm = new RankedMap<int,int>();
            rm.RemoveAt (0);
        }

        [TestMethod]
        public void UnitRm_RemoveAt()
        {
            var rm = new RankedMap<int,int>() { Capacity=4 };
            for (int ii = 0; ii < 5000; ++ii)
                rm.Add (ii, -ii);

            for (int i2 = 4900; i2 >= 0; i2 -= 100)
                rm.RemoveAt (i2);

            for (int i2 = 0; i2 < 5000; ++i2)
                if (i2 % 100 == 0)
                {
                    Assert.IsFalse (rm.ContainsKey (i2));
                    Assert.IsFalse (rm.ContainsValue (-i2));
                }
                else
                {
                    Assert.IsTrue (rm.ContainsKey (i2));
                    Assert.IsTrue (rm.ContainsValue (-i2));
                }
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRm_RemoveRangeA_ArgumentOutOfRange()
        {
            var rm = new RankedMap<int,int>();
            rm.RemoveRange (-1, 0);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRm_RemoveRangeB_ArgumentOutOfRange()
        {
            var rm = new RankedMap<int,int>();
            rm.RemoveRange (0, -1);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRm_RemoveRange_Argument()
        {
            var rm = new RankedMap<int,int> { { 3,33}, {5,55} };
            rm.RemoveRange (1, 2);
        }

        [TestMethod]
        public void UnitRm_RemoveRange()
        {
            var rm = new RankedMap<int,int>() { Capacity=7 };
            for (int ii=0; ii<20; ++ii) rm.Add (ii, -ii);

            rm.RemoveRange (20, 0);
            Assert.AreEqual (20, rm.Count);

            rm.RemoveRange (12, 4);
            Assert.AreEqual (16, rm.Count);
#if DEBUG
            rm.SanityCheck();
#endif
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRm_RemoveWhere_ArgumentNull()
        {
            var rm = new RankedMap<int,int>();
            rm.RemoveWhere (null);
        }

        [TestMethod]
        public void UnitRm_RemoveWhereA()
        {
            var rm = new RankedMap<int,int> { Capacity=5 };
            int n = 2000;

            for (int ix = 0; ix < n; ++ix)
                rm.Add (ix/2, -ix);

            int removed = rm.RemoveWhere (IsEven);

            Assert.AreEqual (n/2, removed);
            foreach (int key in rm.Keys)
                Assert.IsTrue (key % 2 != 0);
        }


        [TestMethod]
        public void UnitRm_RemoveWhereB()
        {
            var rm = new RankedMap<int,int> { Capacity=4 };
            int n = 2000;

            for (int ix = 0; ix < n; ++ix)
                rm.Add (ix, -ix);

            int removed = rm.RemoveWhere (IsAlways);

            Assert.AreEqual (n, removed);
            Assert.AreEqual (0, rm.Count);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRm_RemoveWherePair_ArgumentNull()
        {
            var rm = new RankedMap<int,int>();
            rm.RemoveWherePair (null);
        }

        [TestMethod]
        public void UnitRm_RemoveWherePair()
        {
            var rm = new RankedMap<int,int> { Capacity=6 };

            for (int ix = 0; ix < 1100; ++ix)
                rm.Add (ix, -ix);

            int r1 = rm.RemoveWherePair (IsPairLeN1000);
            Assert.AreEqual (100, r1);
            Assert.AreEqual (1000, rm.Count);

            int c0 = rm.Count;
            int r2 = rm.RemoveWherePair (IsPairEven);

            Assert.AreEqual (500, r2);
            foreach (int v2 in rm.Values)
                Assert.IsTrue (v2 % 2 != 0);

            int r3 = rm.RemoveWherePair (IsPairAlways);
            Assert.AreEqual (500, r3);
            Assert.AreEqual (0, rm.Count);
        }

        #endregion

        #region Test generic enumeration

        [TestMethod]
        public void UnitRm_ElementsBetweenA()
        {
            var rm = new RankedMap<string,int>();
            var pc = (ICollection<KeyValuePair<string,int>>) rm;

            rm.Add ("Alpha", 1);
            rm.Add ("Beta", 2);
            rm.Add ("Omega", 24);

            int actual1 = 0;
            foreach (var kv in rm.ElementsBetween (null, "C"))
                ++actual1;

            pc.Add (new KeyValuePair<string,int> (null, 0));

            int actual2 = 0;
            foreach (var kv in rm.ElementsBetween (null, "C"))
                ++actual2;

            Assert.AreEqual (2, actual1);
            Assert.AreEqual (3, actual2);
        }

        [TestMethod]
        public void UnitRm_ElementsBetweenB()
        {
            var rm = new RankedMap<int,int> { Capacity=4 };

            for (int i = 90; i >= 0; i -= 10)
                rm.Add (i, -100 - i);

            int iterations = 0;
            int sumVals = 0;
            foreach (var kv in rm.ElementsBetween (35, 55))
            {
                ++iterations;
                sumVals += kv.Value;
            }

            Assert.AreEqual (2, iterations);
            Assert.AreEqual (-290, sumVals);
        }

        [TestMethod]
        public void UnitRm_ElementsBetweenMissingVal()
        {
            var rm = new RankedMap<int,int> { Capacity=8 };

            for (int i = 0; i < 1000; i += 2)
                rm.Add (i, -i);

            for (int i = 1; i < 990; i += 2)
            {
                bool isFirst = true;
                foreach (var item in rm.ElementsBetween (i, i+8))
                    if (isFirst)
                    {
                        Assert.AreEqual (i + 1, item.Key, "Incorrect key");
                        isFirst = false;
                    }
            }
        }

        [TestMethod]
        public void UnitRm_ElementsBetweenPassedEnd()
        {
            var rm = new RankedMap<int,int> { Capacity=5 };

            for (int i = 0; i < 1000; ++i)
                rm.Add (i, -i);

            int iterations = 0;
            int sumVals = 0;
            foreach (KeyValuePair<int,int> e in rm.ElementsBetween (500, 1500))
            {
                ++iterations;
                sumVals += e.Value;
            }

            Assert.AreEqual (500, iterations);
            Assert.AreEqual (-374750, sumVals, "Sum of values not correct");
        }


        [TestMethod]
        public void UnitRm_ElementsFrom()
        {
            var rm = new RankedMap<int,int> { Capacity=4 };

            for (int i = 1; i <= 1000; ++i)
                rm.Add (i, -i);

            int firstKey = -1;
            int actual1 = 0;
            foreach (var e in rm.ElementsFrom (501))
            {
                if (actual1 == 0)
                    firstKey = e.Key;
                ++actual1;
            }

            int actual2 = 0;
            foreach (var e in rm.ElementsFrom (-1))
                ++actual2;

            Assert.AreEqual (501, firstKey);
            Assert.AreEqual (500, actual1);
            Assert.AreEqual (1000, actual2);
        }

        [TestMethod]
        public void UnitRm_ElementsFromMissingVal()
        {
            var rm = new RankedMap<int,int>();

            for (int i = 0; i < 1000; i += 2)
                rm.Add (i, -i);

            for (int i = 1; i < 999; i += 2)
            {
                bool isFirst = true;
                foreach (var item in rm.ElementsFrom (i))
                    if (isFirst)
                    {
                        Assert.AreEqual (i + 1, item.Key, "Incorrect key");
                        isFirst = false;
                    }
            }
        }

        [TestMethod]
        public void UnitRm_ElementsFromPassedEnd()
        {
            var rm = new RankedMap<int,int>();

            for (int i = 0; i < 1000; ++i)
                rm.Add (i, -i);

            int iterations = 0;
            foreach (var item in rm.ElementsFrom (2000))
                ++iterations;

            Assert.AreEqual (0, iterations, "SkipUntilKey shouldn't find anything");
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRm_ElementsBetweenIndexes_ArgumentOutOfRangeA()
        {
            var rm = new RankedMap<int,int> { { 0,0 }, { 1,-1 } };
            foreach (var pair in rm.ElementsBetweenIndexes (-1, 0))
            { }
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRm_ElementsBetweenIndexes_ArgumentOutOfRangeB()
        {
            var rm = new RankedMap<int,int> { { 0,0 }, { 1,-1 } };
            foreach (var pair in rm.ElementsBetweenIndexes (2, 0))
            { }
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRm_ElementsBetweenIndexes_ArgumentOutOfRangeC()
        {
            var rm = new RankedMap<int,int> { { 0,0 }, { 1,-1 } };
            foreach (var pair in rm.ElementsBetweenIndexes (0, -1))
            { }
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRd_ElementsBetweenIndexes_ArgumentOutOfRangeD()
        {
            var rm = new RankedMap<int,int> { { 0,0 }, { 1,-1 } };
            foreach (var pair in rm.ElementsBetweenIndexes (0, 2))
            { }
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRm_ElementsBetweenIndexes_Argument()
        {
            var rm = new RankedMap<int,int> { { 0,0 }, { 1,-1 }, { 2,-2 } };
            foreach (var pair in rm.ElementsBetweenIndexes (2, 1))
            { }
        }

        [TestMethod]
        public void UnitRm_ElementsBetweenIndexes()
        {
            var rm = new RankedMap<int,int> { Capacity=4 };
            int n = 30;

            for (int ii = 0; ii < n; ++ii)
                rm.Add (ii, -ii);

            for (int p1 = 0; p1 < n; ++p1)
                for (int p2 = p1; p2 < n; ++p2)
                {
                    int actual = 0;
                    foreach (var pair in rm.ElementsBetweenIndexes (p1, p2))
                        actual += pair.Key;

                    int expected = (p2 - p1 + 1) * (p1 + p2) / 2;
                    Assert.AreEqual (expected, actual);
                }
        }


        [TestMethod]
        public void UnitRm_gcEnumeration()
        {
            var rm = new RankedMap<int,int> { Capacity=4 };
            var pc = (ICollection<KeyValuePair<int,int>>) rm;

            foreach (int k in iVals1)
                rm.Add (k, k + 100);

            int actualCount = 0;
            foreach (KeyValuePair<int,int> pair in pc)
            {
                Assert.AreEqual (pair.Key + 100, pair.Value);
                ++actualCount;
            }

            Assert.AreEqual (iVals1.Length, actualCount);
        }


        [TestMethod]
        public void UnitRm_peEtor()
        {
            var rm = new RankedMap<int,int> { Capacity=4 };
            var pe = (IEnumerable<KeyValuePair<int,int>>) rm;

            foreach (int k in iVals1)
                rm.Add (k, k + 100);

            int actualCount = 0;
            foreach (KeyValuePair<int,int> pair in pe)
            {
                Assert.AreEqual (pair.Key + 100, pair.Value);
                ++actualCount;
            }

            Assert.AreEqual (iVals1.Length, actualCount);
        }


        [TestMethod]
        public void UnitRm_GetEnumeratorOnEmpty()
        {
            var rm = new RankedMap<string,int>();
            int actual=0;

            using (var etor = rm.GetEnumerator())
            {
                while (etor.MoveNext())
                    ++actual;
                var zz = etor.Current;
            }

            Assert.AreEqual (0, actual);
        }


        [TestMethod]
        public void UnitRm_GetEnumeratorPastEnd()
        {
            var rm2 = new RankedMap<string,int>();
            bool isMoved;
            int actualCount=0, total=0;

            rm2.Add ("three", 3);
            rm2.Add ("one", 1);
            rm2.Add ("five", 5);
            rm2.Add ("three", 3);

            using (var etor = rm2.GetEnumerator())
            {
                while (etor.MoveNext())
                {
                    ++actualCount;
                    total += etor.Current.Value;
                }

                isMoved = etor.MoveNext();
            }

            Assert.AreEqual (4, actualCount);
            Assert.AreEqual (12, total);
            Assert.IsFalse (isMoved);
        }


        [TestMethod]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRm_oeEtorRewound_InvalidOperation()
        {
            var rm = new RankedMap<string,int> { {"cc",3} };

            IEnumerator<KeyValuePair<string,int>> kvEtor = rm.GetEnumerator();
            object zz = ((IEnumerator) kvEtor).Current;
        }


        [TestMethod]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRm_peEtorPastUnwound_InvalidOperation()
        {
            var rm = new RankedMap<string,int> { {"cc",3} };

            IEnumerator<KeyValuePair<string,int>> peEtor = rm.GetEnumerator();
            peEtor.MoveNext();
            peEtor.MoveNext();

            object zz = ((IEnumerator) peEtor).Current;
        }


        [TestMethod]
        public void UnitRm_pcEtorPairPastEnd()
        {
            var rm = new RankedMap<string,int> { {"nine",9} };

            IEnumerator<KeyValuePair<string,int>> pcEtor = rm.GetEnumerator();

            KeyValuePair<string,int> pair0 = pcEtor.Current;
            Assert.AreEqual (default (int), pair0.Value);
            Assert.AreEqual (default (string), pair0.Key);

            pcEtor.MoveNext();
            KeyValuePair<string,int> pair1 = pcEtor.Current;
            Assert.AreEqual ("nine", pair1.Key);
            Assert.AreEqual (9, pair1.Value);

            object oPair1 = ((IEnumerator) pcEtor).Current;
            Assert.AreEqual ("nine", ((KeyValuePair<string,int>) oPair1).Key);
            Assert.AreEqual (9, ((KeyValuePair<string,int>) oPair1).Value);

            pcEtor.MoveNext();
            KeyValuePair<string,int> pair2 = pcEtor.Current;
            Assert.AreEqual (default (string), pair2.Key);
            Assert.AreEqual (default (int), pair2.Value);
        }


        [TestMethod]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRm_EtorHotUpdate()
        {
            var rm = new RankedMap<string,int> { {"vv",1}, {"mm",2}, {"qq",3} };
            int n = 0;

            foreach (var kv in rm)
                if (++n == 2)
                    rm.Add ("kaboom", 4);
        }


        #endregion

        #region Test object enumeration

        [TestMethod]
        public void UnitRm_ocGetEnumerator()
        {
            var rm = new RankedMap<int,int>();
            var oc = (ICollection) rm;
            var zz = oc.GetEnumerator();
        }

        [TestMethod]
        public void UnitRm_oeEtor()
        {
            var rm = new RankedMap<int,string> { {3,"cc"} };
            var oc = (ICollection) rm;
            int rowCount = 0;

            foreach (object row in oc)
            {
                var kv = (DictionaryEntry) row;
                Assert.AreEqual (3, (int) kv.Key);
                Assert.AreEqual ("cc", (string) kv.Value);
                ++rowCount;
            }

            Assert.AreEqual (1, rowCount);
        }


        [TestMethod]
        public void UnitRm_oEtorEntry ()
        {
            var rm = new RankedMap<int,int>();
            var oc = (ICollection) rm;

            foreach (int k in iVals1)
                rm.Add (k, k + 1000);

            int actualCount = 0;
            foreach (var oItem in oc)
            {
                DictionaryEntry de = (DictionaryEntry) oItem;
                Assert.AreEqual ((int) de.Key + 1000, de.Value);
                ++actualCount;
            }

            Assert.AreEqual (iVals1.Length, actualCount);
        }

        #endregion
    }
}
#endif
