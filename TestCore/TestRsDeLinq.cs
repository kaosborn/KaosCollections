//
// File: TestRsDeLinq.cs
// Purpose: Exercise LINQ API optimized with instance methods.
//

using System;
#if TEST_BCL
using System.Linq;
#endif
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CollectionsTest
{
    public partial class Test_Btree
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
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRsq_ElementAtOD1_ArgumentOutOfRange()
        {
            Setup();
            int key = setI.ElementAt (-1);
        }

        [TestMethod]
        public void UnitRsq_ElementAtOD2()
        {
            Setup();

            int item1 = setI.ElementAtOrDefault (0);
            Assert.AreEqual (default (int), item1);

            setI.Add (9);
            int item2 = setI.ElementAtOrDefault (0);
            Assert.AreEqual (9, item2);

            int item3 = setI.ElementAtOrDefault (1);
            Assert.AreEqual (default (int), item3);
        }


        [TestMethod]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRsq_Last_InvalidOperation()
        {
            Setup();
            var item = setI.Last();
        }

        [TestMethod]
        public void UnitRsq_Last()
        {
            Setup();
            setI.Add (7);
            setI.Add (5);
            setI.Add (3);

            int item = setI.Last();
            Assert.AreEqual (7, item, "didn't get expected last key");
        }

        #endregion
    }
}
