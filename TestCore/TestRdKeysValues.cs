using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kaos.Collections;

namespace CollectionsTest
{
    public partial class Test_Btree
    {
        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void Crash_KeysCopyTo_ArgumentNull()
        {
            Setup();
            var target = new int[10];
            tree1.Keys.CopyTo (null, -1);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void Crash_KeysCopyToOfValidValues_ArgumentOutOfRange()
        {
            Setup();
            var target = new int[iVals1.Length];
            tree1.Keys.CopyTo (target, -1);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void Crash_KeysCopyToNotLongEnough_Argument()
        {
            Setup();
            for (int key = 1; key < 10; ++key)
                tree1.Add (key, key + 1000);

            var target = new int[4];
            tree1.Keys.CopyTo (target, 2);
        }


        [TestMethod]
        public void Unit_KeysCopyTo()
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

        ////

        [TestMethod]
        public void Unit_KeysGetEnumerator()
        {
            int n = 100;
            Setup();

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

        // ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- -----

        [TestMethod]
        public void Unit_KeysCount()
        {
            Setup();
            foreach (int key in iVals1)
                tree1.Add (key, key + 1000);

            Assert.AreEqual (iVals1.Length, tree1.Keys.Count);
        }

        // ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- -----

        [TestMethod]
        public void Unit_ICollectionKeysContains()
        {
            Setup();
            tree2.Add ("alpha", 10);
            tree2.Add ("beta", 20);

            Assert.IsTrue (genKeys2.Contains ("beta"));
            Assert.IsFalse (genKeys2.Contains ("zed"));
        }


        [TestMethod]
        public void Unit_ICollectionKeysCopyTo()
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
        public void Unit_ICollectionKeysIsReadonly()
        {
            Setup();
            var gicKeys = (ICollection<int>) tree1.Keys;
            Assert.IsTrue (gicKeys.IsReadOnly);
        }


        [TestMethod]
        [ExpectedException (typeof (NotSupportedException))]
        public void Crash_ICollectionKeysAdd_NotSupported()
        {
            Setup();
            genKeys2.Add ("omega");
        }


        [TestMethod]
        [ExpectedException (typeof (NotSupportedException))]
        public void Crash_ICollectionKeysClear_NotSupported()
        {
            Setup();
            genKeys2.Clear();
        }


        [TestMethod]
        [ExpectedException (typeof (NotSupportedException))]
        public void Crash_ICollectionKeysRemove_NotSupported()
        {
            Setup();
            genKeys2.Remove ("omega");
        }

        ////

        [TestMethod]
        public void Unit_KeysSyncRoot()
        {
            Setup();

            var xt = (System.Collections.ICollection) tree2.Keys;
            var sr = xt.SyncRoot;
            
            Assert.IsTrue (sr is object);
        }

        // ===== ===== ===== ===== ===== ===== ===== ===== ===== ===== ===== =====

        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void Crash_ValuesCopyTo_ArgumentNull()
        {
            Setup();
            var target = new int[iVals1.Length];
            tree1.Values.CopyTo (null, -1);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void Crash_ValuesCopyToOfValidValues_ArgumentOutOfRange()
        {
            Setup();
            var target = new int[10];
            tree1.Values.CopyTo (target, -1);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void Crash_ValuesCopyToNotLongEnough_Argument()
        {
            Setup();

            for (int key = 1; key < 10; ++key)
                tree1.Add (key, key + 1000);

            var target = new int[4];
            tree1.Values.CopyTo (target, 2);
        }


        [TestMethod]
        public void Unit_ValuesCopyTo()
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
        public void Unit_ValuesGetEnumerator()
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

        ////

        [TestMethod]
        public void Unit_ValuesSyncRoot()
        {
            Setup();

            var xt = (System.Collections.ICollection) tree2.Values;
            var sr = xt.SyncRoot;

            Assert.IsTrue (sr is object);
        }

        // ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- -----

        [TestMethod]
        public void Unit_ValuesCount()
        {
            Setup();
            foreach (int key in iVals1)
                tree1.Add (key, key + 1000);

            Assert.AreEqual (iVals1.Length, tree1.Values.Count);
        }

        // ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- -----

        [TestMethod]
        public void Unit_ICollectionValuesContains()
        {
            Setup();
            tree2.Add ("alpha", 10);
            tree2.Add ("beta", 20);

            Assert.IsTrue (genValues2.Contains (20));
            Assert.IsFalse (genValues2.Contains (15));
        }


        [TestMethod]
        public void Unit_ICollectionValuesCopyTo()
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
        public void Unit_ICollectionValuesCollectionIsReadonly()
        {
            Setup();
            ICollection<int> ic = (ICollection<int>) tree1.Values;
            Assert.IsTrue (ic.IsReadOnly);
        }


        [TestMethod]
        [ExpectedException (typeof (NotSupportedException))]
        public void Crash_ICollectionValuesAdd_NotSupported()
        {
            Setup();
            genValues2.Add (9);
        }


        [TestMethod]
        [ExpectedException (typeof (NotSupportedException))]
        public void Crash_ICollectionValuesClear_NotSupported()
        {
            Setup();
            genValues2.Clear();
        }


        [TestMethod]
        [ExpectedException (typeof (NotSupportedException))]
        public void Crash_ICollectionValuesRemove_NotSupported()
        {
            Setup();
            genValues2.Remove (9);
        }
    }
}
