using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kaos.Collections;

namespace CollectionsTest
{
#if ! TEST_SORTEDDICTIONARY
    public partial class Test_Btree
    {
        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void Crash_XtraCtor1E1_ArgumentOutOfRange()
        {
            var tree = new BtreeDictionary<int,int> (4);
        }


        [TestMethod]
        public void Unit_XtraCtor1()
        {
            var tree = new BtreeDictionary<int,int> (6);
            Assert.AreEqual (0, tree.Count);
        }


        [TestMethod]
        public void Unit_XtraBetweenKeys()
        {
            var bt = new BtreeDictionary<int,int>();

            for (int i = 90; i >= 0; i -= 10)
                bt.Add (i, -100 - i);

            int iterations = 0;
            int sumVals = 0;
            foreach (var kv in bt.BetweenKeys (35, 55))
            {
                ++iterations;
                sumVals += kv.Value;
            }

            Assert.AreEqual (2, iterations);
            Assert.AreEqual (-290, sumVals);
        }


        [TestMethod]
        public void Unit_XtraBetweenKeysPassedEnd()
        {
            var btree = new BtreeDictionary<int,int>();

            for (int i = 0; i < 1000; ++i)
                btree.Add (i, -i);

            int iterations = 0;
            int sumVals = 0;
            foreach (KeyValuePair<int,int> e in btree.BetweenKeys (500, 1500))
            {
                ++iterations;
                sumVals += e.Value;
            }

            Assert.AreEqual (500, iterations);
            Assert.AreEqual (-374750, sumVals, "Sum of values not correct");
        }


        [TestMethod]
        public void Unit_XtraSkipUntilKey()
        {
            var btree = new BtreeDictionary<int,int>();

            for (int i = 1; i <= 1000; ++i)
                btree.Add (i, -i);

            int firstKey = -1;
            int iterations = 0;
            foreach (var e in btree.SkipUntilKey (501))
            {
                if (iterations == 0)
                    firstKey = e.Key;
                ++iterations;
            }

            Assert.AreEqual (501, firstKey);
            Assert.AreEqual (500, iterations);
        }


        [TestMethod]
        public void Unit_XtraSkipUntilKeyMissingVal()
        {

            var btree = new BtreeDictionary<int,int>();

            for (int i = 0; i < 1000; i += 2)
                btree.Add (i, -i);

            for (int i = 1; i < 999; i += 2)
            {
                bool isFirst = true;
                foreach (var x in btree.SkipUntilKey (i))
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
        public void Unit_XtraSkipUntilKeyPassedEnd()
        {
            var btree = new BtreeDictionary<int,int>();

            for (int i = 0; i < 1000; ++i)
                btree.Add (i, -i);

            int iterations = 0;
            foreach (var x in btree.SkipUntilKey (2000))
                ++iterations;

            Assert.AreEqual (0, iterations, "SkipUntilKey shouldn't find anything");
        }
    }
#endif
}
