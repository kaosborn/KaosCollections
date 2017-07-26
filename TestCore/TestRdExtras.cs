using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kaos.Collections;

namespace CollectionsTest
{
#if ! TEST_BCL
    public partial class Test_Btree
    {
        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRd_XtraCtor0_ArgumentOutOfRange()
        {
            Btree.TreeOrder = 3;
        }


        [TestMethod]
        public void UnitRd_XtraCtor0()
        {
            var tree = new RankedDictionary<int,int>();
            Assert.AreEqual (0, tree.Count);
        }


        [TestMethod]
        public void UnitRd_XtraGetBetween()
        {
            var bt = new RankedDictionary<int,int>();

            for (int i = 90; i >= 0; i -= 10)
                bt.Add (i, -100 - i);

            int iterations = 0;
            int sumVals = 0;
            foreach (var kv in bt.GetBetween (35, 55))
            {
                ++iterations;
                sumVals += kv.Value;
            }

            Assert.AreEqual (2, iterations);
            Assert.AreEqual (-290, sumVals);
        }


        [TestMethod]
        public void UnitRd_XtraGetBetweenPassedEnd()
        {
            var btree = new RankedDictionary<int,int>();

            for (int i = 0; i < 1000; ++i)
                btree.Add (i, -i);

            int iterations = 0;
            int sumVals = 0;
            foreach (KeyValuePair<int,int> e in btree.GetBetween (500, 1500))
            {
                ++iterations;
                sumVals += e.Value;
            }

            Assert.AreEqual (500, iterations);
            Assert.AreEqual (-374750, sumVals, "Sum of values not correct");
        }


        [TestMethod]
        public void UnitRd_XtraSkipUntilKey()
        {
            var btree = new RankedDictionary<int,int>();

            for (int i = 1; i <= 1000; ++i)
                btree.Add (i, -i);

            int firstKey = -1;
            int iterations = 0;
            foreach (var e in btree.GetStartAt (501))
            {
                if (iterations == 0)
                    firstKey = e.Key;
                ++iterations;
            }

            Assert.AreEqual (501, firstKey);
            Assert.AreEqual (500, iterations);
        }


        [TestMethod]
        public void UnitRd_XtraSkipUntilKeyMissingVal()
        {

            var btree = new RankedDictionary<int,int>();

            for (int i = 0; i < 1000; i += 2)
                btree.Add (i, -i);

            for (int i = 1; i < 999; i += 2)
            {
                bool isFirst = true;
                foreach (var x in btree.GetStartAt (i))
                {
                    if (isFirst)
                    {
                        Assert.AreEqual (i + 1, x.Key, "Incorrect key value");
                        isFirst = false;
                    }
                }
            }
        }


        [TestMethod]
        public void UnitRd_XtraSkipUntilKeyPassedEnd()
        {
            var btree = new RankedDictionary<int,int>();

            for (int i = 0; i < 1000; ++i)
                btree.Add (i, -i);

            int iterations = 0;
            foreach (var x in btree.GetStartAt (2000))
                ++iterations;

            Assert.AreEqual (0, iterations, "SkipUntilKey shouldn't find anything");
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRd_RankedIndexOf1_ArgumentOutOfRange()
        {
            var tree = new RankedDictionary<int,int>();
            KeyValuePair<int,int> pair = tree.GetByIndex (-1);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRd_RankedIndexOf2_ArgumentOutOfRange()
        {
            var tree = new RankedDictionary<int,int>();
            KeyValuePair<int,int> pair = tree.GetByIndex (0);
        }


        [TestMethod]
        public void UnitRd_RankedIndexOf()
        {
            Btree.TreeOrder = 5;
            var tree = new RankedDictionary<int,int>();
            for (int ii = 0; ii < 500; ii+=2)
                tree.Add (ii, ii+1000);

            for (int ii = 0; ii < 500; ii+=2)
            {
                int ix = tree.IndexOf (ii);
                Assert.AreEqual (ii/2, ix);
            }

            int iw = tree.IndexOf (-1);
            Assert.AreEqual (~0, iw);

            int iy = tree.IndexOf (500);
            Assert.AreEqual (~250, iy);
        }


        [TestMethod]
        public void UnitRd_RankedGetValueIndex()
        {
            Btree.TreeOrder = 5;
            var tree = new RankedDictionary<int,int>();
            for (int ii = 0; ii < 500; ii+=2)
                tree.Add (ii, ii+1000);

            for (int ii = 0; ii < 500; ii+=2)
            {
                bool isOk = tree.TryGetValueAndIndex (ii, out int v1, out int i1);

                Assert.IsTrue (isOk);
                Assert.AreEqual (ii/2, i1);
                Assert.AreEqual (ii+1000, v1);
            }

            bool isOkNot = tree.TryGetValueAndIndex (111, out int v2, out int i2);
            Assert.IsFalse (isOkNot);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRd_RankedGetByIndex1_ArgumentOutOfRange()
        {
            Btree.TreeOrder = 4;
            var tree = new RankedDictionary<int,int>() { { 4, 104 } };
            KeyValuePair<int,int> pair = tree.GetByIndex (-1);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRd_RankedGetByIndex2_ArgumentOutOfRange()
        {
            Btree.TreeOrder = 4;
            var tree = new RankedDictionary<int,int>();
            KeyValuePair<int,int> pair = tree.GetByIndex (0);
        }


        [TestMethod]
        public void UnitRd_RankedGetByIndex()
        {
            Btree.TreeOrder = 4;
            var tree = new RankedDictionary<int,int>();
            for (int ii = 0; ii <= 800; ii+=2)
                tree.Add (ii, ii+100);

            for (int ii = 0; ii <= 400; ii+=2)
            {
                KeyValuePair<int,int> pair = tree.GetByIndex (ii);
                Assert.AreEqual (ii*2, pair.Key);
                Assert.AreEqual (ii*2+100, pair.Value);
            }
        }
    }
#endif
}
