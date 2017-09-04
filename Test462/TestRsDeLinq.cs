//
// File: TestRsDeLinq.cs
// Purpose: Exercise LINQ API optimized with instance methods.
//

using System;
#if TEST_BCL
using System.Linq;
#endif
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kaos.Test.Collections
{
    public partial class TestBtree
    {
        #region Test bonus LINQ instance implementations

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRsq_ElementAt1_ArgumentOutOfRange()
        {
            Setup();
            setI.Add (4);
            int key = setI.ElementAt (-1);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRsq_ElementAt2_ArgumentOutOfRange()
        {
            Setup();
            int item = setI.ElementAt (0);
        }

        [TestMethod]
        public void UnitRsq_ElementAt()
        {
            Setup();

            for (int ii = 0; ii <= 800; ii+=2)
                setI.Add (ii);

            for (int ii = 0; ii <= 400; ii+=2)
            {
                int key = setI.ElementAt (ii);
                Assert.AreEqual (ii*2, key);
            }
        }


        [TestMethod]
        public void UnitRsq_ElementAtOrDefault()
        {
            Setup();

            int keyM1 = setI.ElementAtOrDefault (-1);
            Assert.AreEqual (default (int), keyM1);

            int key0 = setI.ElementAtOrDefault (0);
            Assert.AreEqual (default (int), key0);

            setI.Add (9);

            int key00 = setI.ElementAtOrDefault (0);
            Assert.AreEqual (9, key00);

            int key1 = setI.ElementAtOrDefault (1);
            Assert.AreEqual (default (int), key1);
        }


        [TestMethod]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRsq_Last_InvalidOperation()
        {
            Setup();
            var key = setI.Last();
        }

        [TestMethod]
        public void UnitRsq_Last()
        {
            Setup (4);
            for (int ii = 99; ii >= 0; --ii) setI.Add (ii);

            int key = setI.Last();
            Assert.AreEqual (99, key);
        }

        #endregion
    }
}
