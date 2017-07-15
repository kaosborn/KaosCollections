using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
#if ! TEST_SORTEDDICTIONARY
using Kaos.Collections;
#endif

namespace CollectionsTest
{
    public partial class Test_Btree
    {
        [TestMethod]
        public void Unit_SdCtor1()
        {
#if TEST_SORTEDDICTIONARY
            var rs5 = new SortedSet<string>();
#else
            var rs5 = new RankedSet<int>();
#endif
            Assert.AreEqual (0, rs5.Count);
        }


        [TestMethod]
        public void Unit_SdCtor2()
        {
#if TEST_SORTEDDICTIONARY
            var tree = new SortedSet<string> (StringComparer.Ordinal);
#else
            var tree = new RankedSet<string> (StringComparer.Ordinal);
#endif
            tree.Add ("AAA");
            tree.Add ("CCC");
            tree.Add ("bbb");
            tree.Add ("ddd");

            Assert.AreEqual (4, tree.Count);
        }
    }
}
