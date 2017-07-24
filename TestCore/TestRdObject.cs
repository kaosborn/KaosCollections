using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kaos.Collections;

namespace CollectionsTest
{
    public partial class Test_Btree
    {
        [TestMethod]
        public void UnitRd_ObjectContainsKey()
        {
            Setup();

            foreach (int key in iVals1)
                tree1.Add (key, key + 1000);

            Assert.IsTrue (objCol1.Contains (iVals1[0]));
            Assert.IsFalse (objCol1.Contains (-1));
            Assert.IsFalse (objCol1.Contains ("foo"));
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRd_ObjectContainsKey_ArgumentNull()
        {
            Setup();
            bool isOK = objCol2.Contains (null);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRd_ObjectCopyTo_ArgumentNull()
        {
            Setup();
            var target = new KeyValuePair<int,int>[iVals1.Length];
            objCol1.CopyTo (null, -1);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRd_ObjectCopyTo_ArgumentOutOfRange()
        {
            Setup();
            var target = new KeyValuePair<int,int>[iVals1.Length];
            objCol1.CopyTo (target, -1);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRd_ObjectCopyTo1_Argument()
        {
            Setup();
            var target = new KeyValuePair<int,int>[iVals1.Length,2];
            objCol1.CopyTo (target, 0);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRd_ObjectCopyTo2_Argument()
        {
            Setup();
            for (int key = 1; key < 10; ++key)
                tree1.Add (key, key + 1000);

            var target = new KeyValuePair<int,int>[1];
            objCol1.CopyTo (target, 0);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRd_ObjectCopyToBadType_Argument()
        {
            Setup();
            tree1.Add (42, 420);

            var target = new string[5];
            objCol1.CopyTo (target, 0);
        }


        [TestMethod]
        public void UnitRd_ObjectCopyTo()
        {
            Setup();
            foreach (int key in iVals1)
                tree1.Add (key, key + 1000);

            var target = new KeyValuePair<int,int>[iVals1.Length];

            objCol1.CopyTo (target, 0);

            for (int i = 0; i < iVals1.Length; ++i)
                Assert.AreEqual (target[i].Key + 1000, target[i].Value);
        }


        [TestMethod]
        public void UnitRd_ObjectCopyToDowncast()
        {
            Setup();
            tree2.Add ("aardvark", 1);
            tree2.Add ("bonobo", 2);

            IDictionary id = (IDictionary) tree2;

            var obj = new object[4];
            id.CopyTo (obj, 2);

            var pair = new KeyValuePair<string,int>();
            pair = (KeyValuePair<string,int>) obj[2];
            Assert.AreEqual ("aardvark", pair.Key);
        }


        [TestMethod]
        public void UnitRd_ObjectGetKeysEnumerator()
        {
            Setup();

            foreach (int k in iVals1)
                tree1.Add (k, k + 1000);

            int actualCount = 0;
            foreach (DictionaryEntry de in objCol1)
            {
                Assert.AreEqual ((int) de.Key + 1000, de.Value);
                ++actualCount;
            }

            Assert.AreEqual (iVals1.Length, actualCount);
        }


        [TestMethod]
        public void UnitRd_ObjectIsFixedSize()
        {
            Setup();
            Assert.IsFalse (objCol1.IsFixedSize);
        }


        [TestMethod]
        public void UnitRd_ObjectIsReadonly()
        {
            Setup();
            Assert.IsFalse (objCol1.IsReadOnly);
        }


        [TestMethod]
        public void UnitRd_ObjectIsSynchronized()
        {
            Setup();
            Assert.IsFalse (objCol1.IsSynchronized);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRd_ObjectAddNullKey_Argument()
        {
            Setup();
            objCol2.Add ((String) null, 1);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRd_ObjectAddBadKey_Argument()
        {
            Setup();
            objCol2.Add (23, 45);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRd_ObjectAddBadValue_Argument()
        {
            Setup();
            objCol2.Add ("razz", "matazz");
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRd_ObjectAddDupl_Argument()
        {
            Setup();
            objCol2.Add ("nn", 1);
            objCol2.Add ("nn", 2);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRd_ObjectItemGet_ArgumentNull()
        {
            Setup();
            objCol2.Add ("foo", 10);
            object j = objCol2[null];
        }


        [TestMethod]
        public void CrashRd_ObjectItemGetBadKey()
        {
            Setup();
            objCol2.Add ("foo", 10);
            object j = objCol2[45];
            Assert.IsNull (j);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRd_ObjectItemSetKey_ArgumentNull()
        {
            Setup();
            objCol2.Add ("foo", 10);
            objCol2[null] = "bar";
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRd_ObjectItemSetValue_ArgumentNull()
        {
            Setup();
            objCol2.Add ("foo", 10);
            objCol2["foo"] = null;
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRd_ObjectItemSetBadKey_Argument()
        {
            Setup();
            objCol2.Add ("foo", 10);
            objCol2[23] = 45;
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRd_ObjectItemSetBadValue_Argument()
        {
            Setup();
            objCol2.Add ("foo", 10);
            objCol2["red"] = "blue";
        }


        [TestMethod]
        public void UnitRd_ObjectItem()
        {
            Setup();

            object j1 = objCol2["foo"];
            Assert.IsNull (j1);

            objCol2.Add ("foo", 10);
            objCol2.Add ("bar", 20);

            objCol2["raz"] = 30;

            Assert.AreEqual (3, objCol2.Count);

            objCol2["bar"] = 40;

            Assert.AreEqual (3, objCol2.Count);

            object j2 = objCol2["bar"];
            Assert.AreEqual (40, (int) j2);

            objCol4[12] = "twelve";
            objCol4[13] = null;
            Assert.AreEqual (2, objCol4.Count);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRd_ObjectRemove_ArgumentNull()
        {
            Setup();
            objCol1.Remove (null);
        }


        [TestMethod]
        public void UnitRd_ObjectRemove()
        {
            Setup();

            Assert.AreEqual (0, objCol1.Count);
            objCol1.Add (17, 170);
            Assert.AreEqual (1, objCol1.Count);
            objCol1.Remove (18);
            Assert.AreEqual (1, objCol1.Count);
            objCol1.Remove (17);
            Assert.AreEqual (0, objCol1.Count);

            objCol1.Remove ("ignore wrong type");
        }


        // ===== ===== ===== ===== ===== ===== ===== ===== ===== ===== ===== =====


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRd_ObjectKeysCopyTo_ArgumentNull()
        {
            Setup();
            ICollection c1 = (ICollection) tree1.Keys;
            c1.CopyTo (null, -1);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRd_ObjectKeysCopyToMultiDimensional_Argument()
        {
            Setup();
            tree1.Add (42, 420);
            ICollection c1 = (ICollection) tree1.Keys;
            object[,] target = new object[2, 3];
            c1.CopyTo (target, -1);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRd_ObjectKeysCopyTo_ArgumentOutOfRange()
        {
            Setup();
            tree1.Add (42, 420);
            ICollection c1 = (ICollection) tree1.Keys;
            object[] target = new object[1];
            c1.CopyTo (target, -1);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRd_ObjectKeysCopyToNotLongEnough_Argument()
        {
            Setup();
            for (int i = 0; i < 10; ++i)
                tree1.Add (i + 100, i + 1000);
            ICollection c1 = (ICollection) tree1.Keys;
            object[] target = new object[10];
            c1.CopyTo (target, 5);
        }


        [TestMethod]
        public void UnitRd_ObjectKeysCopyTo()
        {
            int n = 10;
            Setup();
            for (int i = 0; i < n; ++i)
                tree1.Add (i + 100, i + 1000);

            ICollection c1 = (ICollection) tree1.Keys;

            object[] target = new object[n];

            c1.CopyTo (target, 0);

            for (int i = 0; i < n; ++i)
                Assert.AreEqual (i + 100, (int) target[i]);
        }


        [TestMethod]
        public void UnitRd_ObjectKeysGetEnumerator()
        {
            int n = 10;
            Setup();
            var oid = (IDictionary) tree1;

            for (int k = 0; k < n; ++k)
                tree1.Add (k, k + 1000);

            int expected = 0;
            foreach (object j in oid.Keys)
            {
                Assert.AreEqual (expected, (int) j);
                ++expected;
            }
        }

        // ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- -----

        [TestMethod]
        public void UnitRd_ObjectKeysCount()
        {
            int n = 10;
            Setup();

            ICollection c1 = (ICollection) tree1.Keys;

            Assert.AreEqual (0, c1.Count);

            for (int i = 0; i < n; ++i)
                tree1.Add (i + 100, i + 1000);

            Assert.AreEqual (n, c1.Count);
        }


        [TestMethod]
        public void UnitRd_ObjectKeysIsSynchronized()
        {
            Setup();
            Assert.IsFalse (((ICollection) tree1.Keys).IsSynchronized);
        }


        // ===== ===== ===== ===== ===== ===== ===== ===== ===== ===== ===== =====


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRd_ObjectValuesCopyTo_ArgumentNull()
        {
            Setup();
            ICollection c1 = (ICollection) tree1.Values;
            c1.CopyTo (null, -1);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRd_ObjectValuesCopyToMultiDimensional_Argument()
        {
            Setup();
            tree1.Add (42, 420);
            ICollection c1 = (ICollection) tree1.Values;
            object[,] target = new object[2, 3];
            c1.CopyTo (target, -1);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRd_ObjectValuesCopyTo_ArgumentOutOfRange()
        {
            Setup();
            tree1.Add (42, 420);
            ICollection c1 = (ICollection) tree1.Values;
            object[] target = new object[1];
            c1.CopyTo (target, -1);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRd_ObjectValuesCopyToNotLongEnough_Argument()
        {
            Setup();
            for (int i = 0; i < 10; ++i)
                tree1.Add (i + 100, i + 1000);
            ICollection c1 = (ICollection) tree1.Values;
            object[] target = new object[10];
            c1.CopyTo (target, 5);
        }


        [TestMethod]
        public void UnitRd_ObjectValuesCopyTo()
        {
            int n = 10;
            Setup();
            for (int i = 0; i < n; ++i)
                tree1.Add (i + 100, i + 1000);

            ICollection c1 = (ICollection) tree1.Values;

            object[] target = new object[n];

            c1.CopyTo (target, 0);

            for (int i = 0; i < n; ++i)
                Assert.AreEqual (i + 1000, (int) target[i]);
        }


        [TestMethod]
        public void UnitRd_ObjectValuesGetEnumerator()
        {
            int n = 10;
            Setup();
            var oid = (IDictionary) tree1;

            for (int k = 0; k < n; ++k)
                tree1.Add (k, k + 1000);

            int expected = 1000;
            foreach (object j in oid.Values)
            {
                Assert.AreEqual (expected, (int) j);
                ++expected;
            }
        }

        // ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- -----

        [TestMethod]
        public void UnitRd_ObjectValuesCount()
        {
            int n = 10;
            Setup();

            ICollection c1 = (ICollection) tree1.Values;

            Assert.AreEqual (0, c1.Count);

            for (int i = 0; i < n; ++i)
                tree1.Add (i + 100, i + 1000);

            Assert.AreEqual (n, c1.Count);
        }


        [TestMethod]
        public void UnitRd_ObjectValuesIsSynchronized()
        {
            Setup();
            Assert.IsFalse (((ICollection) tree1.Values).IsSynchronized);
        }
    }
}
