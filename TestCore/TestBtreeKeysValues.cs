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
        public void Test_Keys_CopyTo_ArgumentNullException()
        {
            Setup();
            var target = new int[10];
            tree1.Keys.CopyTo (null, -1);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void Test_Keys_CopyTo_ArgumentOutOfRangeException_Of_Valid_Values()
        {
            Setup();
            var target = new int[keys.Length];
            tree1.Keys.CopyTo (target, -1);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void Test_Keys_CopyTo_ArgumentException_Not_Long_Enough()
        {
            Setup();
            for (int key = 1; key < 10; ++key)
                tree1.Add (key, key + 1000);

            var target = new int[4];
            tree1.Keys.CopyTo (target, 2);
        }


        [TestMethod]
        public void Test_Keys_CopyTo()
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
        public void Test_Keys_GetEnumerator()
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
        public void Test_Keys_Count()
        {
            Setup();
            foreach (int key in keys)
                tree1.Add (key, key + 1000);

            Assert.AreEqual (keys.Length, tree1.Keys.Count);
        }

        // ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- -----

        [TestMethod]
        public void Test_ICollection_Keys_Contains()
        {
            Setup();
            tree2.Add ("alpha", 10);
            tree2.Add ("beta", 20);

            Assert.IsTrue (genKeys2.Contains ("beta"));
            Assert.IsFalse (genKeys2.Contains ("zed"));
        }


        [TestMethod]
        public void Test_ICollection_Keys_CopyTo()
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
        public void Test_ICollection_Keys_IsReadonly()
        {
            Setup();
            Assert.IsTrue (genKeys1.IsReadOnly);
        }


        [TestMethod]
        [ExpectedException (typeof (NotSupportedException))]
        public void Test_ICollection_Keys_Add_NotSupportedException()
        {
            Setup();
            genKeys2.Add ("omega");
        }


        [TestMethod]
        [ExpectedException (typeof (NotSupportedException))]
        public void Test_ICollection_Keys_Clear_NotSupportedException()
        {
            Setup();
            genKeys2.Clear();
        }


        [TestMethod]
        [ExpectedException (typeof (NotSupportedException))]
        public void Test_ICollection_Keys_Remove_NotSupportedException()
        {
            Setup();
            genKeys2.Remove ("omega");
        }

        ////

        [TestMethod]
        public void Test_Keys_SyncRoot()
        {
            Setup();

            var xt = (System.Collections.ICollection) tree2.Keys;
            var sr = xt.SyncRoot;
            
            Assert.IsTrue (sr is object);
        }

        // ===== ===== ===== ===== ===== ===== ===== ===== ===== ===== ===== =====

        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void Test_Values_CopyTo_ArgumentNullException()
        {
            Setup();
            var target = new int[keys.Length];
            tree1.Values.CopyTo (null, -1);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void Test_Values_CopyTo_ArgumentOutOfRangeException_Of_Valid_Values()
        {
            Setup();
            var target = new int[10];
            tree1.Values.CopyTo (target, -1);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void Test_Values_CopyTo_ArgumentException_Not_Long_Enough()
        {
            Setup();
            for (int key = 1; key < 10; ++key)
                tree1.Add (key, key + 1000);

            var target = new int[4];
            tree1.Values.CopyTo (target, 2);
        }


        [TestMethod]
        public void Test_Values_CopyTo()
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
        public void Test_Values_GetEnumerator()
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
        public void Test_Values_SyncRoot()
        {
            Setup();

            var xt = (System.Collections.ICollection) tree2.Values;
            var sr = xt.SyncRoot;

            Assert.IsTrue (sr is object);
        }

        // ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- -----

        [TestMethod]
        public void Test_Values_Count()
        {
            Setup();
            foreach (int key in keys)
                tree1.Add (key, key + 1000);

            Assert.AreEqual (keys.Length, tree1.Values.Count);
        }

        // ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- -----

        [TestMethod]
        public void Test_ICollection_Values_Contains()
        {
            Setup();
            tree2.Add ("alpha", 10);
            tree2.Add ("beta", 20);

            Assert.IsTrue (genValues2.Contains (20));
            Assert.IsFalse (genValues2.Contains (15));
        }


        [TestMethod]
        public void Test_ICollection_Values_CopyTo()
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
        public void Test_ICollection_Values_Collection_IsReadonly()
        {
            Setup();
            ICollection<int> ic = (ICollection<int>) tree1.Values;
            Assert.IsTrue (ic.IsReadOnly);
        }


        [TestMethod]
        [ExpectedException (typeof (NotSupportedException))]
        public void Test_ICollection_Values_Add_NotSupportedException()
        {
            Setup();
            genValues2.Add (9);
        }


        [TestMethod]
        [ExpectedException (typeof (NotSupportedException))]
        public void Test_ICollection_Values_Clear_NotSupportedException()
        {
            Setup();
            genValues2.Clear();
        }


        [TestMethod]
        [ExpectedException (typeof (NotSupportedException))]
        public void Test_ICollection_Values_Remove_NotSupportedException()
        {
            Setup();
            genValues2.Remove (9);
        }
    }
}
