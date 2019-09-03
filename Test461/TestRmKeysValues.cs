//
// Library: KaosCollections
// File:    TestRmKeysValues.cs
//

#if ! TEST_BCL

using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kaos.Collections;

namespace Kaos.Test.Collections
{
    public partial class TestRm
    {
        #region Test Keys constructor

        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRmk_Ctor_ArgumentNull()
        {
            var kc = new RankedMap<int,int>.KeyCollection (null);
        }

        [TestMethod]
        public void UnitRmk_Ctor()
        {
            var rm = new RankedMap<int,int> { {1,-1} };
            var kc = new RankedMap<int,int>.KeyCollection (rm);

            Assert.AreEqual (1, kc.Count);
        }

        #endregion

        #region Test Keys properties

        [TestMethod]
        public void UnitRmk_Item()
        {
            var rm = new RankedMap<string,int> { {"0zero",0}, {"1one",-1} };

            Assert.AreEqual ("0zero", rm.Keys[0]);
            Assert.AreEqual ("1one", rm.Keys[1]);
        }


        [TestMethod]
        public void UnitRmk_gcIsReadonly()
        {
            var rm = new RankedMap<int,int>();
            var gc = (ICollection<int>) rm.Keys;
            Assert.IsTrue (gc.IsReadOnly);
        }


        [TestMethod]
        public void UnitRmk_gcIsSynchronized()
        {
            var rm = new RankedMap<int,int>();
            var oc = (ICollection) rm.Keys;
            Assert.IsFalse (oc.IsSynchronized);
        }


        [TestMethod]
        public void UnitRmk_ocSyncRoot()
        {
            var rm = new RankedMap<int,int>();
            var oc = (ICollection) rm.Keys;
            Assert.IsFalse (oc.SyncRoot.GetType().IsValueType);
        }

        #endregion

        #region Test Keys methods

        [TestMethod]
        [ExpectedException (typeof (NotSupportedException))]
        public void CrashRmk_gcAdd_NotSupported()
        {
            var rm = new RankedMap<string,int>();
            var gc = (ICollection<string>) rm.Keys;
            gc.Add ("omega");
        }


        [TestMethod]
        [ExpectedException (typeof (NotSupportedException))]
        public void CrashRmk_gcClear_NotSupported()
        {
            var rm = new RankedMap<int,int>();
            var gc = (ICollection<int>) rm.Keys;
            gc.Clear();
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRmk_gcContains_ArgumentNull()
        {
            var rm = new RankedMap<string,int> { {"zed", 26} };
            var gc = (ICollection<string>) rm.Keys;
            var zz = gc.Contains (null);
        }

        [TestMethod]
        public void UnitRmk_gcContains()
        {
            var rm = new RankedMap<string,int>();
            var gc = (ICollection<string>) rm.Keys;

            rm.Add ("alpha", 10);
            rm.Add ("delta", 40);

            Assert.IsTrue (gc.Contains ("delta"));
            Assert.IsFalse (gc.Contains ("zed"));
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRmk_CopyTo_ArgumentNull()
        {
            var rm = new RankedMap<int,int>();
            rm.Keys.CopyTo (null, -1);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRmk_CopyTo_ArgumentOutOfRange()
        {
            var rm = new RankedMap<int,int> { {1,11} };
            var target = new int[1];
            rm.Keys.CopyTo (target, -1);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRmk_CopyTo_Argument()
        {
            var rm = new RankedMap<int,int>();
            var target = new int[4];

            for (int key = 1; key < 10; ++key)
                rm.Add (key, key + 1000);

            rm.Keys.CopyTo (target, 2);
        }

        [TestMethod]
        public void UnitRmk_CopyTo()
        {
            var rm = new RankedMap<int,int>();
            int n = 10, offset = 5;

            for (int k = 0; k < n; ++k)
                rm.Add (k, k + 1000);

            int[] target = new int[n + offset];
            rm.Keys.CopyTo (target, offset);

            for (int k = 0; k < n; ++k)
                Assert.AreEqual (k, target[k + offset]);
        }

        [TestMethod]
        public void UnitRmk_gcCopyTo()
        {
            var rm = new RankedMap<string,int>();
            var gc = (ICollection<string>) rm.Keys;

            rm.Add ("alpha", 1);
            rm.Add ("beta", 2);
            rm.Add ("gamma", 3);

            var target = new string[rm.Count];

            gc.CopyTo (target, 0);

            Assert.AreEqual ("alpha", target[0]);
            Assert.AreEqual ("beta",  target[1]);
            Assert.AreEqual ("gamma", target[2]);
        }

        [TestMethod]
        public void UnitRmk_ocCopyTo()
        {
            var rm = new RankedMap<char,int> { {'a',1}, {'b',2}, {'z',26} };
            var oc = (ICollection) rm.Keys;
            var target = new char[4];

            oc.CopyTo (target, 1);

            Assert.AreEqual ('a', target[1]);
            Assert.AreEqual ('b', target[2]);
            Assert.AreEqual ('z', target[3]);
        }


        [TestMethod]
        public void UnitRmk_GetCount()
        {
            var rm = new RankedMap<int,int> { Capacity=4 };
            foreach (int ii in new int[] { 1, 3, 5, 5, 5, 7 }) rm.Add (ii, -ii);

            Assert.AreEqual (0, rm.Keys.GetCount (0));
            Assert.AreEqual (1, rm.Keys.GetCount (3));
            Assert.AreEqual (3, rm.Keys.GetCount (5));
            Assert.AreEqual (1, rm.Keys.GetCount (7));
            Assert.AreEqual (0, rm.Keys.GetCount (9));
        }


        [TestMethod]
        public void UnitRmk_GetDistinctCount()
        {
            var rm0 = new RankedMap<int,int>();
            var rm = new RankedMap<int,int> { Capacity=4 };
            foreach (int ii in new int[] { 3, 5, 5, 5, 7 }) rm.Add (ii, -ii);

            Assert.AreEqual (0, rm0.Keys.GetDistinctCount());
            Assert.AreEqual (3, rm.Keys.GetDistinctCount());
        }


        [TestMethod]
        public void UnitRmk_IndexOf()
        {
            var rm = new RankedMap<string,int> { {"0zero",0}, {"1one",-1}, {"1one",-2} };
            var pc = (ICollection<KeyValuePair<string,int>>) rm;

            pc.Add (new KeyValuePair<string,int> (null, -1));

            Assert.AreEqual (0, rm.Keys.IndexOf (null));
            Assert.AreEqual (1, rm.Keys.IndexOf ("0zero"));
            Assert.AreEqual (2, rm.Keys.IndexOf ("1one"));
            Assert.AreEqual (~1, rm.Keys.IndexOf ("00"));
            Assert.AreEqual (~2, rm.Keys.IndexOf ("11"));
        }


        [TestMethod]
        [ExpectedException (typeof (NotSupportedException))]
        public void CrashRmk_gcRemove_NotSupported()
        {
            var rm = new RankedMap<string,int>();
            var gc = (ICollection<string>) rm.Keys;
            gc.Remove ("omega");
        }


        [TestMethod]
        public void UnitRmk_TryGetGEGT()
        {
            var rm = new RankedMap<string,int> (StringComparer.OrdinalIgnoreCase) { Capacity=4 };
            for (char ci = 'b'; ci <= 'y'; ++ci)
            {
                rm.Add (ci.ToString().ToUpper(), 'a'-ci);
                rm.Add (ci.ToString(), ci-'a');
            }

            bool r0a = rm.Keys.TryGetGreaterThan ("a", out string k0a);
            Assert.IsTrue (r0a);
            Assert.AreEqual ("B", k0a);

            bool r0b = rm.Keys.TryGetGreaterThanOrEqual ("a", out string k0b);
            Assert.IsTrue (r0b);
            Assert.AreEqual ("B", k0b);

            bool r1 = rm.Keys.TryGetGreaterThan ("B", out string k1);
            Assert.IsTrue (r1);
            Assert.AreEqual ("C", k1);

            bool r2 = rm.Keys.TryGetGreaterThanOrEqual ("b", out string k2);
            Assert.IsTrue (r2);
            Assert.AreEqual ("B", k2);

            bool r3 = rm.Keys.TryGetGreaterThanOrEqual ("a", out string k3);
            Assert.IsTrue (r3);
            Assert.AreEqual ("B", k3);

            bool r9a = rm.Keys.TryGetGreaterThan ("y", out string k9a);
            Assert.IsFalse (r9a);
            Assert.AreEqual (default (string), k9a);

            bool r9b = rm.Keys.TryGetGreaterThan ("z", out string k9b);
            Assert.IsFalse (r9a);
            Assert.AreEqual (default (string), k9b);

            bool r9c = rm.Keys.TryGetGreaterThanOrEqual ("z", out string k9c);
            Assert.IsFalse (r9c);
            Assert.AreEqual (default (string), k9c);
        }


        [TestMethod]
        public void UnitRmkx_TryGetLELT()
        {
            var rm = new RankedMap<string,int> (StringComparer.OrdinalIgnoreCase) { Capacity=4 };
            for (char ci = 'b'; ci <= 'y'; ++ci)
            {
                rm.Add (ci.ToString().ToUpper(), 'a'-ci);
                rm.Add (ci.ToString(), ci-'a');
            }

            bool r0a = rm.Keys.TryGetLessThan ("B", out string k0a);
            Assert.IsFalse (r0a);
            Assert.AreEqual (default (string), k0a);

            bool r0b = rm.Keys.TryGetLessThanOrEqual ("A", out string k0b);
            Assert.IsFalse (r0b);
            Assert.AreEqual (default (string), k0b);

            bool r1 = rm.Keys.TryGetLessThan ("C", out string k1);
            Assert.IsTrue (r1);
            Assert.AreEqual ("b", k1);

            bool r2 = rm.Keys.TryGetLessThanOrEqual ("C", out string k2);
            Assert.IsTrue (r2);
            Assert.AreEqual ("C", k2);

            bool r3 = rm.Keys.TryGetLessThanOrEqual ("d", out string k3);
            Assert.IsTrue (r3);
            Assert.AreEqual ("D", k3);
        }

        #endregion

        #region Test Keys enumeration

        [TestMethod]
        public void UnitRmk_gcEtor()
        {
            var rm = new RankedMap<int,int> { Capacity=4 };
            int n = 100;

            for (int k = 0; k < n; ++k)
                rm.Add (k, k + 1000);

            int actualCount = 0;
            foreach (int key in rm.Keys)
            {
                Assert.AreEqual (actualCount, key);
                ++actualCount;
            }

            Assert.AreEqual (n, actualCount);
        }

        [TestMethod]
        public void UnitRmk_gcGetEnumerator()
        {
            var rm = new RankedMap<string,int> { Capacity=4 };
            var gc = (ICollection<string>) rm.Keys;
            int n = 10;

            for (int k = 0; k < n; ++k)
                rm.Add (k.ToString(), k);

            int expected = 0;
            var etor = gc.GetEnumerator();

            var rewoundKey = etor.Current;
            Assert.AreEqual (rewoundKey, default (string));

            while (etor.MoveNext())
            {
                var key = etor.Current;
                Assert.AreEqual (expected.ToString(), key);
                Assert.AreEqual (expected.ToString(), (string) ((IEnumerator) etor).Current);
                ++expected;
            }
            Assert.AreEqual (n, expected);
        }

        [TestMethod]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRmk_EtorHotUpdate()
        {
            var rm = new RankedMap<string,int> { {"vv",1}, {"mm",2}, {"qq",3} };
            int n = 0;

            foreach (var kv in rm.Keys)
                if (++n == 2)
                    rm.Remove ("vv");
        }

        [TestMethod]
        public void UnitRmk_EtorCurrentHotUpdate()
        {
            var rm1 = new RankedMap<int,int> { {3,-3} };
            var etor1 = rm1.Keys.GetEnumerator();
            Assert.AreEqual (default (int), etor1.Current);
            bool ok1 = etor1.MoveNext();
            Assert.AreEqual (3, etor1.Current);
            rm1.Remove (3);
            Assert.AreEqual (3, etor1.Current);

            var rm2 = new RankedMap<string,int> { {"CC",3} };
            var etor2 = rm2.Keys.GetEnumerator();
            Assert.AreEqual (default (string), etor2.Current);
            bool ok2 = etor2.MoveNext();
            Assert.AreEqual ("CC", etor2.Current);
            rm2.Clear();
            Assert.AreEqual ("CC", etor2.Current);
        }

        #endregion

        #region Test Keys object enumeration

        [TestMethod]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRmk_ocCurrent_InvalidOperation()
        {
            var rm = new RankedMap<char,int> { {'C',3} };
            var oc = (ICollection) rm.Keys;

            IEnumerator etor = oc.GetEnumerator();
            object cur = etor.Current;
        }

        [TestMethod]
        public void UnitRmk_ocEtor()
        {
            var rm = new RankedMap<char,int> { {'a',1}, {'b',2}, {'c',3} };
            var oc = (ICollection) rm.Keys;

            int actual = 0;
            foreach (object oItem in oc)
            {
                Assert.AreEqual (rm.ElementAt (actual).Key, (char) oItem);
                ++actual;
            }

            Assert.AreEqual (rm.Count, actual);
        }


        [TestMethod]
        public void UnitRmk_ocCurrent_HotUpdate()
        {
            var rm = new RankedMap<char,int> { {'c',3} };

            System.Collections.ICollection oc = rm.Keys;
            System.Collections.IEnumerator etor = oc.GetEnumerator();

            bool ok = etor.MoveNext();
            Assert.AreEqual ('c', etor.Current);

            rm.Clear();
            Assert.AreEqual ('c', etor.Current);
        }


        [TestMethod]
        public void UnitRmk_oReset()
        {
            var rm = new RankedMap<int,int> { Capacity=4 };
            int n = 7;

            for (int ix = 0; ix < n; ++ix)
                rm.Add (ix*10, -ix);

            RankedMap<int,int>.KeyCollection.Enumerator etor = rm.Keys.GetEnumerator();

            int ix1 = 0;
            while (etor.MoveNext())
            {
                Assert.AreEqual (ix1*10, etor.Current);
                ++ix1;
            }
            Assert.AreEqual (n, ix1);

            ((System.Collections.IEnumerator) etor).Reset();

            int ix2 = 0;
            while (etor.MoveNext())
            {
                Assert.AreEqual (ix2*10, etor.Current);
                ++ix2;
            }
            Assert.AreEqual (n, ix2);
        }

        #endregion


        #region Test Values constructor

        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRmv_Ctor_ArgumentNull()
        {
            var vc = new RankedMap<int,int>.ValueCollection (null);
        }

        [TestMethod]
        public void UnitRmv_Ctor()
        {
            var rm = new RankedMap<int,int> { {1,-1} };
            var vc = new RankedMap<int,int>.ValueCollection (rm);

            Assert.AreEqual (1, vc.Count);
        }

        #endregion

        #region Test Values properties

        [TestMethod]
        public void UnitRmv_Item()
        {
            var rm = new RankedMap<string,int> { {"0zero",0}, {"1one",-1} };

            Assert.AreEqual ( 0, rm.Values[0]);
            Assert.AreEqual (-1, rm.Values[1]);
        }


        [TestMethod]
        public void UnitRmv_gcIsReadonly()
        {
            var rm = new RankedMap<int,int>();
            var gc = (ICollection<int>) rm.Values;
            Assert.IsTrue (gc.IsReadOnly);
        }


        [TestMethod]
        public void UnitRmv_gcIsSynchronized()
        {
            var rm = new RankedMap<int,int>();
            var oc = (ICollection) rm.Values;
            Assert.IsFalse (oc.IsSynchronized);
        }


        [TestMethod]
        public void UnitRmv_ocSyncRoot()
        {
            var rm = new RankedMap<int,int>();
            var oc = (ICollection) rm.Values;
            Assert.IsFalse (oc.SyncRoot.GetType().IsValueType);
        }

        #endregion

        #region Test Values methods

        [TestMethod]
        [ExpectedException (typeof (NotSupportedException))]
        public void CrashRmv_gcAdd_NotSupported()
        {
            var rm = new RankedMap<string,int>();
            var gc = (ICollection<int>) rm.Values;
            gc.Add (9);
        }


        [TestMethod]
        [ExpectedException (typeof (NotSupportedException))]
        public void CrashRmv_gcClear_NotSupported()
        {
            var rm = new RankedMap<int,int>();
            var gc = (ICollection<int>) rm.Values;
            gc.Clear();
        }


        [TestMethod]
        public void UnitRmv_gcContains()
        {
            var rm = new RankedMap<string,int>();
            var gc = (ICollection<int>) rm.Values;

            rm.Add ("alpha", 10);
            rm.Add ("beta", 20);

            Assert.IsTrue (gc.Contains (20));
            Assert.IsFalse (gc.Contains (-9));
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRmv_CopyTo_ArgumentNull()
        {
            var rm = new RankedMap<int,int>();
            rm.Values.CopyTo (null, -1);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRmv_CopyTo_ArgumentOutOfRange()
        {
            var rm = new RankedMap<int,int>() { {1,11} };
            var target = new int[1];
            rm.Values.CopyTo (target, -1);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRmv_CopyTo_Argument()
        {
            var rm = new RankedMap<int,int>();
            var target = new int[4];

            for (int key = 1; key < 10; ++key)
                rm.Add (key, key + 1000);

            rm.Values.CopyTo (target, 2);
        }


        [TestMethod]
        public void UnitRmv_CopyTo()
        {
            var rm = new RankedMap<int,int>();
            int n = 10, offset = 5;

            for (int k = 0; k < n; ++k)
                rm.Add (k, -k);

            int[] target = new int[n + offset];
            rm.Values.CopyTo (target, offset);

            for (int k = 0; k < n; ++k)
                Assert.AreEqual (k, -target[k + offset]);
        }

        [TestMethod]
        public void UnitRmv_gcCopyTo()
        {
            var rm = new RankedMap<string,int>();
            var gc = (ICollection<int>) rm.Values;

            rm.Add ("alpha", 1);
            rm.Add ("beta",  2);
            rm.Add ("gamma", 3);

            var target = new int[rm.Count];

            gc.CopyTo (target, 0);

            Assert.AreEqual (1, target[0]);
            Assert.AreEqual (2, target[1]);
            Assert.AreEqual (3, target[2]);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRmv_ocCopyTo_ArgumentNull()
        {
            var rm = new RankedMap<int,int>();
            var oc = (ICollection) rm.Values;
            oc.CopyTo (null, 0);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRmv_ocCopyToMultiDimensional_Argument()
        {
            var rm = new RankedMap<int,int> { {42,420} };
            var oc = (ICollection) rm.Values;
            object[,] target = new object[2,3];
            oc.CopyTo (target, -1);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRmv_ocCopyTo_ArgumentOutOfRange()
        {
            var rm = new RankedMap<int,int> { {42,420} };
            var oc = (ICollection) rm.Values;
            var target = new object[1];
            oc.CopyTo (target, -1);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRmv_ocCopyToNotLongEnough_Argument()
        {
            var rm = new RankedMap<int,int> { Capacity=4 };
            var oc = (ICollection) rm.Values;

            for (int i = 0; i < 10; ++i)
                rm.Add (i + 100, i + 1000);
            var target = new object[10];

            oc.CopyTo (target, 5);
        }

        [TestMethod]
        public void UnitRmv_ocCopyToA()
        {
            var rm = new RankedMap<int,int> { Capacity=4 };
            var oc = (ICollection) rm.Values;
            int n = 10;

            for (int i = 0; i < n; ++i)
                rm.Add (i + 100, i + 1000);

            object[] target = new object[n];

            oc.CopyTo (target, 0);

            for (int i = 0; i < n; ++i)
                Assert.AreEqual (i + 1000, (int) target[i]);
        }


        [TestMethod]
        public void UnitRmv_ocCopyToB()
        {
            var rm = new RankedMap<char,int>() { {'a',1}, {'b',2}, {'z',26} };
            var oc = (ICollection) rm.Values;
            var target = new int[4];

            oc.CopyTo (target, 1);

            Assert.AreEqual (1, target[1]);
            Assert.AreEqual (2, target[2]);
            Assert.AreEqual (26,target[3]);
        }


        [TestMethod]
        public void UnitRmv_IndexOf()
        {
            var rm = new RankedMap<string,int?> { {"1one",1}, {"2two",2}, {"2two",2}, {"3tree",3}, {"9nine",null} };

            Assert.AreEqual (0, rm.Values.IndexOf (1));
            Assert.AreEqual (1, rm.Values.IndexOf (2));
            Assert.AreEqual (3, rm.Values.IndexOf (3));
            Assert.AreEqual (4, rm.Values.IndexOf ((int?) null));
            Assert.AreEqual (-1, rm.Values.IndexOf (0));
            Assert.AreEqual (-1, rm.Values.IndexOf (4));
        }


        [TestMethod]
        [ExpectedException (typeof (NotSupportedException))]
        public void CrashRmv_gcRemove_NotSupported()
        {
            var rm = new RankedMap<int,int>();
            var gc = (ICollection<int>) rm.Values;
            gc.Remove (9);
        }

        #endregion

        #region Test Values generic enumeration

        [TestMethod]
        public void UnitRmv_GetEnumerator()
        {
            var rm = new RankedMap<int,int> { Capacity=4 };
            int n = 100;

            for (int k = 0; k < n; ++k)
                rm.Add (k, k + 1000);

            int actualCount = 0;
            foreach (int val in rm.Values)
            {
                Assert.AreEqual (actualCount+1000, val);
                ++actualCount;
            }

            Assert.AreEqual (n, actualCount);
        }

        [TestMethod]
        public void UnitRmv_gcGetEnumerator()
        {
            var rm = new RankedMap<string,int?> { Capacity=4 };
            var gc = (ICollection<int?>) rm.Values;
            int n = 10;

            for (int k = 0; k < n; ++k)
                rm.Add (k.ToString(), k+1000);

            int expected = 0;
            var etor = gc.GetEnumerator();

            var rewoundKey = etor.Current;
            Assert.AreEqual (rewoundKey, default (int?));

            while (etor.MoveNext())
            {
                var val = etor.Current;
                Assert.AreEqual (expected+1000, val);
                ++expected;
            }
            Assert.AreEqual (n, expected);
        }

        [TestMethod]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRmv_EtorHotUpdate()
        {
            var rm = new RankedMap<string,int> { {"vv",1}, {"mm",2}, {"qq",3} };
            int n = 0;

            foreach (var kv in rm.Values)
                if (++n == 2)
                    rm.Remove ("vv");
        }

        [TestMethod]
        public void UnitRmv_EtorCurrentHotUpdate()
        {
            var rm1 = new RankedMap<int,int> { {3,-3} };
            var etor1 = rm1.Values.GetEnumerator();
            Assert.AreEqual (default (int), etor1.Current);
            bool ok1 = etor1.MoveNext();
            Assert.AreEqual (-3, etor1.Current);
            rm1.Remove (3);
            Assert.AreEqual (-3, etor1.Current);

            var rm2 = new RankedMap<string,int> { {"CC",3} };
            var etor2 = rm2.Values.GetEnumerator();
            Assert.AreEqual (default (int), etor2.Current);
            bool ok2 = etor2.MoveNext();
            Assert.AreEqual (3, etor2.Current);
            rm2.Clear();
            Assert.AreEqual (3, etor2.Current);
        }


        [TestMethod]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRmv_ocCurrent_InvalidOperation()
        {
            var rm = new RankedMap<char,int> { {'C',3} };
            var oc = (ICollection) rm.Values;

            IEnumerator etor = oc.GetEnumerator();
            object cur = etor.Current;
        }

        #endregion

        #region Test Values object enumeration

        [TestMethod]
        public void UnitRmv_ocEtor()
        {
            var rm = new RankedMap<char,int> { {'a',1}, {'b',2}, {'c',3} };
            var oc = (ICollection) rm.Values;

            int ix = 0;
            foreach (object oItem in oc)
            {
                Assert.AreEqual (ix+1, (int) oItem);
                ++ix;
            }
            Assert.AreEqual (rm.Count, ix);
        }


        [TestMethod]
        public void UnitRmv_ocCurrent_HotUpdate()
        {
            var rm = new RankedMap<char,int> { {'c',3} };

            System.Collections.ICollection oc = rm.Values;
            System.Collections.IEnumerator etor = oc.GetEnumerator();

            bool ok = etor.MoveNext();
            Assert.AreEqual (3, etor.Current);

            rm.Clear();
            Assert.AreEqual (3, etor.Current);
        }


        [TestMethod]
        public void UnitRmv_oReset()
        {
            var rm = new RankedMap<int,int> { Capacity=4 };
            int n = 7;

            for (int ix = 0; ix < n; ++ix)
                rm.Add (ix, -ix);

            RankedMap<int,int>.ValueCollection.Enumerator etor = rm.Values.GetEnumerator();

            int ix1 = 0;
            while (etor.MoveNext())
            {
                Assert.AreEqual (-ix1, etor.Current);
                ++ix1;
            }
            Assert.AreEqual (n, ix1);

            ((System.Collections.IEnumerator) etor).Reset();

            int ix2 = 0;
            while (etor.MoveNext())
            {
                Assert.AreEqual (-ix2, etor.Current);
                ++ix2;
            }
            Assert.AreEqual (n, ix2);
        }

        #endregion
    }
}

#endif
