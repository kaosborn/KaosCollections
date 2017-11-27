using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kaos.Test.Collections
{
    public partial class TestBtree
    {
        #region Test object properties

        [TestMethod]
        public void UnitRd_odIsFixedSize()
        {
            Setup();
            var od = (IDictionary) dary1;
            Assert.IsFalse (od.IsFixedSize);
        }


        [TestMethod]
        public void UnitRd_odIsReadonly()
        {
            Setup();
            var od = (IDictionary) dary1;
            Assert.IsFalse (od.IsReadOnly);
        }


        [TestMethod]
        public void UnitRd_odIsSynchronized()
        {
            Setup();
            var od = (IDictionary) dary1;
            Assert.IsFalse (od.IsSynchronized);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRd_odItemGet_ArgumentNull()
        {
            Setup();
            var od = (IDictionary) dary2;
            od.Add ("foo", 10);
            object zz = od[null];
        }


        [TestMethod]
        public void UnitRd_odItemGetBadKey()
        {
            Setup();
            var od = (IDictionary) dary2;
            od.Add ("foo", 10);
            object zz = od[45];
            Assert.IsNull (zz);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRd_odItemSetKey_ArgumentNull()
        {
            Setup();
            var od = (IDictionary) dary2;
            od.Add ("foo", 10);
            od[null] = "bar";
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRd_odItemSetValue_ArgumentNull()
        {
            Setup();
            var od = (IDictionary) dary2;
            od.Add ("foo", 10);
            od["foo"] = null;
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRd_odItemSetBadKey_Argument()
        {
            Setup();
            var od = (IDictionary) dary2;
            od.Add ("foo", 10);
            od[23] = 45;
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRd_odItemSetBadValue_Argument()
        {
            Setup();
            var od = (IDictionary) dary2;
            od.Add ("foo", 10);
            od["red"] = "blue";
        }


        [TestMethod]
        public void UnitRd_odItem()
        {
            Setup();
            var od2 = (IDictionary) dary2;
            var od4 = (IDictionary) dary4;

            object j1 = od2["foo"];
            Assert.IsNull (j1);

            od2.Add ("foo", 10);
            od2.Add ("bar", 20);

            od2["raz"] = 30;

            Assert.AreEqual (3, od2.Count);

            od2["bar"] = 40;

            Assert.AreEqual (3, od2.Count);

            object j2 = od2["bar"];
            Assert.AreEqual (40, (int) j2);

            od4[12] = "twelve";
            od4[13] = null;
            Assert.AreEqual (2, od4.Count);
        }


        [TestMethod]
        public void UnitRd_odSyncRoot()
        {
            Setup();
            var od = (IDictionary) dary2;
            Assert.IsFalse (od.SyncRoot.GetType().IsValueType);
        }

        #endregion

        #region Test object methods

        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRd_odAddNullKey_Argument()
        {
            Setup();
            var od = (IDictionary) dary2;
            od.Add ((String) null, 1);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRd_odAddBadKey_Argument()
        {
            Setup();
            var od = (IDictionary) dary2;
            od.Add (23, 45);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRd_odAddBadValue_Argument()
        {
            Setup();
            var od = (IDictionary) dary2;
            od.Add ("razz", "matazz");
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRd_odAddDupl_Argument()
        {
            Setup();
            var od = (IDictionary) dary2;
            od.Add ("nn", 1);
            od.Add ("nn", 2);
        }


        [TestMethod]
        public void UnitRd_odContainsKey()
        {
            Setup();
            var od = (IDictionary) dary1;

            foreach (int key in iVals1)
                od.Add (key, key + 1000);

            Assert.IsTrue (od.Contains (iVals1[0]));
            Assert.IsFalse (od.Contains (-1));
            Assert.IsFalse (od.Contains ("foo"));
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRd_odContainsKey_ArgumentNull()
        {
            Setup();
            var od = (IDictionary) dary2;
            bool isOK = objCol2.Contains (null);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRd_odCopyTo_ArgumentNull()
        {
            Setup();
            var od = (IDictionary) dary1;
            var target = new KeyValuePair<int,int>[iVals1.Length];
            od.CopyTo (null, -1);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRd_odCopyTo_ArgumentOutOfRange()
        {
            Setup();
            var od = (IDictionary) dary1;
            var target = new KeyValuePair<int,int>[iVals1.Length];
            od.CopyTo (target, -1);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRd_odCopyTo1_Argument()
        {
            Setup();
            var od = (IDictionary) dary1;
            var target = new KeyValuePair<int,int>[iVals1.Length,2];
            od.CopyTo (target, 0);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRd_odCopyTo2_Argument()
        {
            Setup();
            var od = (IDictionary) dary1;

            for (int key = 1; key < 10; ++key)
                dary1.Add (key, key + 1000);

            var target = new KeyValuePair<int,int>[1];
            od.CopyTo (target, 0);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRd_odCopyToBadType_Argument()
        {
            Setup();
            var od = (IDictionary) dary1;
            dary1.Add (42, 420);

            var target = new string[5];
            od.CopyTo (target, 0);
        }


        [TestMethod]
        public void UnitRd_odCopyTo()
        {
            Setup();
            var od = (IDictionary) dary1;
            foreach (int key in iVals1)
                dary1.Add (key, key + 1000);

            var target = new KeyValuePair<int,int>[iVals1.Length];

            od.CopyTo (target, 0);

            for (int i = 0; i < iVals1.Length; ++i)
                Assert.AreEqual (target[i].Key + 1000, target[i].Value);
        }


        [TestMethod]
        public void UnitRd_odCopyToDowncast()
        {
            Setup();
            var od = (IDictionary) dary2;
            dary2.Add ("aardvark", 1);
            dary2.Add ("bonobo", 2);

            var obj = new object[4];
            od.CopyTo (obj, 2);

            var pair = new KeyValuePair<string,int>();
            pair = (KeyValuePair<string,int>) obj[2];
            Assert.AreEqual ("aardvark", pair.Key);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRd_odRemove_ArgumentNull()
        {
            Setup();
            var od = (IDictionary) dary1;
            od.Remove (null);
        }


        [TestMethod]
        public void UnitRd_odRemove()
        {
            Setup();
            var od = (IDictionary) dary1;

            Assert.AreEqual (0, od.Count);
            od.Add (17, 170);
            Assert.AreEqual (1, od.Count);
            od.Remove (18);
            Assert.AreEqual (1, od.Count);
            od.Remove (17);
            Assert.AreEqual (0, od.Count);

            objCol1.Remove ("ignore wrong type");
        }

        #endregion

        #region Test object enumeration

        [TestMethod]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRd_odEtorKey_InvalidOperation()
        {
            Setup();
            var od = (IDictionary) dary2;
            dary2.Add ("cc", 3);

            IDictionaryEnumerator oEtor = od.GetEnumerator();
            var key = oEtor.Key;
        }

        [TestMethod]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRd_odEtorValue_InvalidOperation()
        {
            Setup();
            var od = (IDictionary) dary2;
            dary2.Add ("cc", 3);

            IDictionaryEnumerator etor = od.GetEnumerator();
            var val = etor.Value;
        }

        [TestMethod]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRd_odEtorEntry_InvalidOperation()
        {
            Setup();
            var od = (IDictionary) dary2;
            dary2.Add ("cc", 3);

            IDictionaryEnumerator oEtor = od.GetEnumerator();
            DictionaryEntry entry = oEtor.Entry;
        }

        [TestMethod]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRd_odEtorCurrent_InvalidOperation()
        {
            Setup();
            var od = (IDictionary) dary2;
            dary2.Add ("cc", 3);

            IDictionaryEnumerator oEtor = od.GetEnumerator();
            var val = oEtor.Current;
        }

        [TestMethod]
        public void UnitRd_odEtor()
        {
            Setup();
            var od = (IDictionary) dary1;
            dary1.Add (3, 33);
            dary1.Add (5, 55);

            IDictionaryEnumerator etor = od.GetEnumerator();
            etor.MoveNext();
            object key = etor.Key;
            object val = etor.Value;
            DictionaryEntry de = etor.Entry;
            Assert.AreEqual (3, key);
            Assert.AreEqual (33, val);
            Assert.AreEqual (3, de.Key);
            Assert.AreEqual (33, de.Value);
        }

        [TestMethod]
        public void UnitRd_odEtorEntry()
        {
            Setup();
            var od = (IDictionary) dary1;

            foreach (int k in iVals1)
                dary1.Add (k, k + 1000);

            int actualCount = 0;
            foreach (DictionaryEntry de in od)
            {
                Assert.AreEqual ((int) de.Key + 1000, de.Value);
                ++actualCount;
            }

            Assert.AreEqual (iVals1.Length, actualCount);
        }

        #endregion

        #region Test object Keys

        [TestMethod]
        public void UnitRdk_ocCount()
        {
            Setup();
            var oc = (ICollection) dary1.Keys;
            int n = 10;

            Assert.AreEqual (0, oc.Count);

            for (int i = 0; i < n; ++i)
                dary1.Add (i + 100, i + 1000);

            Assert.AreEqual (n, oc.Count);
        }


        [TestMethod]
        public void UnitRdk_ocIsSynchronized()
        {
            Setup();
            var oc = (ICollection) dary1.Keys;
            Assert.IsFalse (oc.IsSynchronized);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRdk_ocCopyTo_ArgumentNull()
        {
            Setup();
            var oc = (ICollection) dary1.Keys;
            oc.CopyTo (null, -1);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRdk_ocCopyToMultiDimensional_Argument()
        {
            Setup();
            var oc = (ICollection) dary1.Keys;
            dary1.Add (42, 420);

            object[,] target = new object[2, 3];
            oc.CopyTo (target, -1);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRdk_ocCopyTo_ArgumentOutOfRange()
        {
            Setup();
            var oc = (ICollection) dary1.Keys;
            dary1.Add (42, 420);

            object[] target = new object[1];
            oc.CopyTo (target, -1);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRdk_ocCopyToNotLongEnough_Argument()
        {
            Setup();
            var oc = (ICollection) dary1.Keys;

            for (int i = 0; i < 10; ++i)
                dary1.Add (i + 100, i + 1000);


            object[] target = new object[10];
            oc.CopyTo (target, 5);
        }


        [TestMethod]
        public void UnitRdk_ocCopyTo()
        {
            Setup();
            var oc = (ICollection) dary1.Keys;
            int n = 10;

            for (int i = 0; i < n; ++i)
                dary1.Add (i + 100, i + 1000);

            object[] target = new object[n];

            oc.CopyTo (target, 0);

            for (int i = 0; i < n; ++i)
                Assert.AreEqual (i + 100, (int) target[i]);
        }


        [TestMethod]
        public void UnitRdk_odEtor()
        {
            Setup();
            var od = (IDictionary) dary1;
            int n = 10;

            for (int k = 0; k < n; ++k)
                dary1.Add (k, k + 1000);

            int expected = 0;
            foreach (object j in od.Keys)
            {
                Assert.AreEqual (expected, (int) j);
                ++expected;
            }
        }

        #endregion

        #region Test object Values

        [TestMethod]
        public void UnitRdv_ocValuesCount()
        {
            Setup();
            var oc = (ICollection) dary1.Values;
            int n = 10;

            Assert.AreEqual (0, oc.Count);

            for (int i = 0; i < n; ++i)
                dary1.Add (i + 100, i + 1000);

            Assert.AreEqual (n, oc.Count);
        }


        [TestMethod]
        public void UnitRdv_ocIsSynchronized()
        {
            Setup();
            var oc = (ICollection) dary1.Values;
            Assert.IsFalse (oc.IsSynchronized);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRdv_ocCopyTo_ArgumentNull()
        {
            Setup();
            var oc = (ICollection) dary1.Values;
            oc.CopyTo (null, -1);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRdv_ocCopyToMultiDimensional_Argument()
        {
            Setup();
            var oc = (ICollection) dary1.Values;

            dary1.Add (42, 420);
            object[,] target = new object[2, 3];

            oc.CopyTo (target, -1);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRdv_ocCopyTo_ArgumentOutOfRange()
        {
            Setup();
            var oc = (ICollection) dary1.Values;

            dary1.Add (42, 420);
            object[] target = new object[1];

            oc.CopyTo (target, -1);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRdv_ocCopyToNotLongEnough_Argument()
        {
            Setup();
            var oc = (ICollection) dary1.Values;

            for (int i = 0; i < 10; ++i)
                dary1.Add (i + 100, i + 1000);
            var target = new object[10];

            oc.CopyTo (target, 5);
        }


        [TestMethod]
        public void UnitRdv_ocCopyTo()
        {
            Setup();
            var oc = (ICollection) dary1.Values;
            int n = 10;

            for (int i = 0; i < n; ++i)
                dary1.Add (i + 100, i + 1000);
            object[] target = new object[n];

            oc.CopyTo (target, 0);

            for (int i = 0; i < n; ++i)
                Assert.AreEqual (i + 1000, (int) target[i]);
        }


        [TestMethod]
        public void UnitRdv_ocGetEnumerator()
        {
            Setup();
            var od = (IDictionary) dary1;
            int n = 10;

            for (int k = 0; k < n; ++k)
                dary1.Add (k, k + 1000);

            int expected = 1000;
            foreach (object j in od.Values)
            {
                Assert.AreEqual (expected, (int) j);
                ++expected;
            }
        }

        #endregion
    }
}
