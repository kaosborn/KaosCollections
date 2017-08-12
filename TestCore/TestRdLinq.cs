//
// File: TestRdLinq.cs
//
// Exercise some of the LINQ API derived from Enumerable. This is a partial sample and only
// Last() is required for coverage testing.
//

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CollectionsTest
{
    public partial class Test_Btree
    {
        [TestMethod]
        public void UnitRd_LinqAny()
        {
            Setup();

            var x1 = tree1.Any();

            tree1.Add (1, 10);
            tree1.Add (3, 30);
            tree1.Add (2, 20);

            var x2 = tree1.Any();

            Assert.IsFalse (x1);
            Assert.IsTrue (x2);
        }


        [TestMethod]
        public void UnitRd_LongCount()
        {
            Setup();
            tree1.Add (3, -33);
            tree1.Add (1, -11);
            tree1.Add (2, -22);

            var result = tree1.LongCount();
            var type = result.GetType();

            Assert.AreEqual (3, result);
            Assert.AreEqual ("Int64", type.Name);
        }
    }
}