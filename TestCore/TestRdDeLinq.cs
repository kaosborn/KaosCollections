//
// File: TestRdDeLinq.cs
// Purpose: Exercise LINQ API optimized with instance methods.
//

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
#if TEST_BCL
using System.Linq;
#endif

namespace CollectionsTest
{
    public partial class Test_Btree
    {
        #region Test bonus LINQ instance implementations

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRdq_ElementAt1_ArgumentOutOfRange()
        {
            Setup();
            KeyValuePair<int,int> pair = tree1.ElementAt (-1);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRdx_ElementAt2_ArgumentOutOfRange()
        {
            Setup();
            KeyValuePair<int,int> pair = tree1.ElementAt (0);
        }

        [TestMethod]
        public void UnitRdq_ElementAt()
        {
            Setup();

            for (int ii = 0; ii <= 800; ii+=2)
                tree1.Add (ii, ii+100);

            for (int ii = 0; ii <= 400; ii+=2)
            {
                KeyValuePair<int,int> pair = tree1.ElementAt (ii);
                Assert.AreEqual (ii*2, pair.Key);
                Assert.AreEqual (ii*2+100, pair.Value);
            }
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRdq_ElementAtOD1_ArgumentOutOfRange()
        {
            Setup();
            KeyValuePair<int,int> pair = tree1.ElementAt (-1);
        }

        [TestMethod]
        public void UnitRdq_ElementAtOD2()
        {
            Setup();

            KeyValuePair<int,int> pair1 = tree1.ElementAtOrDefault (0);
            Assert.AreEqual (default (int), pair1.Key);
            Assert.AreEqual (default (int), pair1.Value);

            tree1.Add (9, -9);
            KeyValuePair<int,int> pair2 = tree1.ElementAtOrDefault (0);
            Assert.AreEqual (9, pair2.Key);
            Assert.AreEqual (-9, pair2.Value);

            KeyValuePair<int,int> pair3 = tree1.ElementAtOrDefault (1);
            Assert.AreEqual (default (int), pair3.Key);
            Assert.AreEqual (default (int), pair3.Value);
        }


        [TestMethod]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRdq_Last_InvalidOperation()
        {
            Setup();
            var kv = tree2.Last();
        }

        [TestMethod]
        public void UnitRdq_Last()
        {
            Setup();
            tree1.Add (3, -33);
            tree1.Add (1, -11);
            tree1.Add (2, -22);

            var kv = tree1.Last();

            Assert.AreEqual (3, kv.Key, "didn't get expected last key");
            Assert.AreEqual (-33, kv.Value, "didn't get expected last value");
        }

        #endregion
    }
}