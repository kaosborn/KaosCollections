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
        #region Test keys constructor

        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRdk_Ctor1_ArgumentNull()
        {
            Setup();
#if TEST_BCL
            var keys = new SortedDictionary<int,int>.KeyCollection (null);
#else
            var keys = new RankedDictionary<int,int>.KeyCollection (null);
#endif
        }

        #endregion

        #region Test Keys properties

        [TestMethod]
        public void UnitRdk_KeysCount()
        {
            Setup();
            foreach (int key in iVals1)
                tree1.Add (key, key + 1000);

            Assert.AreEqual (iVals1.Length, tree1.Keys.Count);
        }


        [TestMethod]
        public void UnitRdk_ICollectionKeysIsReadonly()
        {
            Setup();
            var gicKeys = (System.Collections.Generic.ICollection<int>) tree1.Keys;
            Assert.IsTrue (gicKeys.IsReadOnly);
        }


        [TestMethod]
        public void UnitRdk_KeysSyncRoot()
        {
            Setup();

            var xt = (System.Collections.ICollection) tree2.Keys;
            var sr = xt.SyncRoot;
            
            Assert.IsTrue (sr is object);
        }

        #endregion

        #region Test Keys methods

        [TestMethod]
        [ExpectedException (typeof (NotSupportedException))]
        public void CrashRdk_ICollectionKeysAdd_NotSupported()
        {
            Setup();
            genKeys2.Add ("omega");
        }


        [TestMethod]
        [ExpectedException (typeof (NotSupportedException))]
        public void CrashRdk_ICollectionKeysClear_NotSupported()
        {
            Setup();
            genKeys2.Clear();
        }


        [TestMethod]
        public void UnitRdk_ICollectionKeysContains()
        {
            Setup();
            tree2.Add ("alpha", 10);
            tree2.Add ("beta", 20);

            Assert.IsTrue (genKeys2.Contains ("beta"));
            Assert.IsFalse (genKeys2.Contains ("zed"));
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRdk_KeysCopyTo_ArgumentNull()
        {
            Setup();
            var target = new int[10];
            tree1.Keys.CopyTo (null, -1);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRdk_KeysCopyToOfValidValues_ArgumentOutOfRange()
        {
            Setup();
            var target = new int[iVals1.Length];
            tree1.Keys.CopyTo (target, -1);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRdk_KeysCopyToNotLongEnough_Argument()
        {
            Setup();
            for (int key = 1; key < 10; ++key)
                tree1.Add (key, key + 1000);

            var target = new int[4];
            tree1.Keys.CopyTo (target, 2);
        }

        [TestMethod]
        public void UnitRdk_KeysCopyTo()
        {
            int n = 10;
            int offset = 5;
            Setup();
            for (int k = 0; k < n; ++k)
                tree1.Add (k, k + 1000);

            int[] target = new int[n + offset];
            tree1.Keys.CopyTo (target, offset);

            for (int k = 0; k < n; ++k)
                Assert.AreEqual (k, target[k + offset]);
        }


        [TestMethod]
        public void UnitRdk_ICollectionKeysCopyTo()
        {
            Setup();
            tree2.Add ("alpha", 1);
            tree2.Add ("beta", 2);
            tree2.Add ("gamma", 3);

            var target = new string[tree2.Count];

            genKeys2.CopyTo (target, 0);

            Assert.AreEqual ("alpha", target[0]);
            Assert.AreEqual ("beta", target[1]);
            Assert.AreEqual ("gamma", target[2]);
        }


        [TestMethod]
        [ExpectedException (typeof (NotSupportedException))]
        public void CrashRdk_ICollectionKeysRemove_NotSupported()
        {
            Setup();
            genKeys2.Remove ("omega");
        }

        #endregion

        #region Test Keys enumeration

        [TestMethod]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRdk_Current_InvalidOperation()
        {
            Setup();
            tree2.Add ("CC", 3);

            System.Collections.ICollection oKeys = objCol2.Keys;
            var etor = oKeys.GetEnumerator();

            object cur = etor.Current;
        }

        [TestMethod]
        public void UnitRdk_GetEnumerator()
        {
            int n = 100;
            Setup (4);

            for (int k = 0; k < n; ++k)
                tree1.Add (k, k + 1000);

            int actualCount = 0;
            foreach (int key in tree1.Keys)
            {
                Assert.AreEqual (actualCount, key);
                ++actualCount;
            }

            Assert.AreEqual (n, actualCount);
        }

        [TestMethod]
        public void UnitRdk_ExGetEnumerator()
        {
            int n = 10;
            Setup();

            for (int k = 0; k < n; ++k)
                tree2.Add (k.ToString(), k);

            int expected = 0;
            var etor = genKeys2.GetEnumerator();

            var rewoundKey = etor.Current;
            Assert.AreEqual (rewoundKey, default (string));

            while (etor.MoveNext())
            {
                var key = etor.Current;
                Assert.AreEqual (expected.ToString(), key);
                ++expected;
            }
            Assert.AreEqual (n, expected);
        }

        [TestMethod]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRdk_EtorHotUpdate()
        {
            Setup (4);
            tree2.Add ("vv", 1);
            tree2.Add ("mm", 2);
            tree2.Add ("qq", 3);

            int n = 0;
            foreach (var kv in tree2.Keys)
            {
                if (++n == 2)
                    tree2.Remove ("vv");
            }
        }

        #endregion


        #region Test values constructor

        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRdv_Ctor1_ArgumentNull()
        {
            Setup();
#if TEST_BCL
            var vals = new SortedDictionary<int,int>.ValueCollection (null);
#else
            var vals = new RankedDictionary<int,int>.ValueCollection (null);
#endif
        }

        #endregion

        #region Test Values properties

        [TestMethod]
        public void UnitRdv_ValuesCount()
        {
            Setup();
            foreach (int key in iVals1)
                tree1.Add (key, key + 1000);

            Assert.AreEqual (iVals1.Length, tree1.Values.Count);
        }


        [TestMethod]
        public void UnitRdv_ICollectionValuesCollectionIsReadonly()
        {
            Setup();
            var ic = (System.Collections.Generic.ICollection<int>) tree1.Values;
            Assert.IsTrue (ic.IsReadOnly);
        }


        [TestMethod]
        public void UnitRdv_ValuesSyncRoot()
        {
            Setup();

            var xt = (System.Collections.ICollection) tree2.Values;
            var sr = xt.SyncRoot;

            Assert.IsTrue (sr is object);
        }

        #endregion

        #region Test Values methods

        [TestMethod]
        [ExpectedException (typeof (NotSupportedException))]
        public void CrashRdv_ICollectionValuesAdd_NotSupported()
        {
            Setup();
            genValues2.Add (9);
        }


        [TestMethod]
        [ExpectedException (typeof (NotSupportedException))]
        public void CrashRdv_ICollectionValuesClear_NotSupported()
        {
            Setup();
            genValues2.Clear();
        }


        [TestMethod]
        public void UnitRdv_ICollectionValuesContains()
        {
            Setup();
            tree2.Add ("alpha", 10);
            tree2.Add ("beta", 20);

            Assert.IsTrue (genValues2.Contains (20));
            Assert.IsFalse (genValues2.Contains (15));
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRdv_ValuesCopyTo_ArgumentNull()
        {
            Setup();
            var target = new int[iVals1.Length];
            tree1.Values.CopyTo (null, -1);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRdv_ValuesCopyToOfValidValues_ArgumentOutOfRange()
        {
            Setup();
            var target = new int[10];
            tree1.Values.CopyTo (target, -1);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRdv_ValuesCopyToNotLongEnough_Argument()
        {
            Setup();

            for (int key = 1; key < 10; ++key)
                tree1.Add (key, key + 1000);

            var target = new int[4];
            tree1.Values.CopyTo (target, 2);
        }

        [TestMethod]
        public void UnitRdv_ValuesCopyTo()
        {
            int n = 10;
            int offset = 5;
            Setup();
            for (int k = 0; k < n; ++k)
                tree1.Add (k, k + 1000);

            int[] target = new int[n + offset];
            tree1.Values.CopyTo (target, offset);

            for (int k = 0; k < n; ++k)
                Assert.AreEqual (k + 1000, target[k + offset]);
        }


        [TestMethod]
        public void UnitRdv_ICollectionValuesCopyTo()
        {
            Setup();
            tree2.Add ("alpha", 1);
            tree2.Add ("beta", 2);
            tree2.Add ("gamma", 3);

            var target = new int[tree2.Count];

            genValues2.CopyTo (target, 0);

            Assert.AreEqual (1, target[0]);
            Assert.AreEqual (2, target[1]);
            Assert.AreEqual (3, target[2]);
        }


        [TestMethod]
        [ExpectedException (typeof (NotSupportedException))]
        public void CrashRdv_ICollectionValuesRemove_NotSupported()
        {
            Setup();
            genValues2.Remove (9);
        }

        #endregion

        #region Test Values enumeration

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CrashRdv_Current_InvalidOperation()
        {
            Setup();
            tree2.Add ("CC", 3);

            System.Collections.ICollection oVals = objCol2.Values;
            var etor = oVals.GetEnumerator();

            object cur = etor.Current;
        }

        [TestMethod]
        public void UnitRdv_GetEnumerator()
        {
            int n = 100;
            Setup();

            for (int k = 0; k < n; ++k)
                tree1.Add (k, k + 1000);

            int actualCount = 0;
            foreach (int value in tree1.Values)
            {
                Assert.AreEqual (actualCount + 1000, value);
                ++actualCount;
            }

            Assert.AreEqual (n, actualCount);
        }

        [TestMethod]
        public void UnitRdv_ExGetEnumerator()
        {
            int n = 10;
            Setup();

            for (int k = 0; k < n; ++k)
                tree2.Add (k.ToString(), k);

            int expected = 0;
            var etor = genValues2.GetEnumerator();

            var rewoundVal = etor.Current;
            Assert.AreEqual (rewoundVal, default (int));

            while (etor.MoveNext())
            {
                var val = etor.Current;
                Assert.AreEqual (expected, val);
                ++expected;
            }
            Assert.AreEqual (n, expected);
        }

        [TestMethod]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRdv_EtorHotUpdate()
        {
            Setup (4);
            tree2.Add ("vv", 1);
            tree2.Add ("mm", 2);
            tree2.Add ("qq", 3);

            int n = 0;
            foreach (var kv in tree2.Keys)
            {
                if (++n == 2)
                    tree2.Clear();
            }
        }

        #endregion
    }
}
