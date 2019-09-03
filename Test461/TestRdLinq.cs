//
// File: TestRdLinq.cs
//
// Exercise some of the LINQ API derived from Enumerable. This is a partial sample and only
// Last() is required for coverage testing.
//

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kaos.Test.Collections
{
    public partial class TestRd
    {
        [TestMethod]
        public void UnitRd_LinqAny()
        {
            Setup();

            var x1 = dary1.Any();

            dary1.Add (1, 10);
            dary1.Add (3, 30);
            dary1.Add (2, 20);

            var x2 = dary1.Any();

            Assert.IsFalse (x1);
            Assert.IsTrue (x2);
        }


        [TestMethod]
        public void UnitRd_LongCount()
        {
            Setup();
            dary1.Add (3, -33);
            dary1.Add (1, -11);
            dary1.Add (2, -22);

            var result = dary1.LongCount();
            var type = result.GetType();

            Assert.AreEqual (3, result);
            Assert.AreEqual ("Int64", type.Name);
        }
    }
}