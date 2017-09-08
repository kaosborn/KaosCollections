//
// Library: KaosCollections
// File:    TestRsSetCompare.cs
//

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
#if TEST_BCL
using System.Collections.Generic;
#else
using Kaos.Collections;
#endif

namespace Kaos.Test.Collections
{
    public partial class TestBtree
    {
#if TEST_BCL
        SortedSet<string> setS1 = new SortedSet<string>(), setS2 = new SortedSet<string>();
#else
        RankedSet<string> setS1 = new RankedSet<string>(), setS2 = new RankedSet<string>();
#endif
        [TestMethod]
        public void UnitRs_SetComparer()
        {
#if TEST_BCL
            var sc = SortedSet<string>.CreateSetComparer();
#else
            var sc = RankedSet<string>.CreateSetComparer();
#endif
            setS1.Add ("ABC");
            setS2.Add ("DEF");
            bool eq2 = sc.Equals (setS1, setS2);
            Assert.IsFalse (eq2);

            setS2.Add ("ABC");
            bool eq3 = sc.Equals (setS1, setS2);
            Assert.IsFalse (eq3);

            setS1.Add ("DEF");
            bool eq4 = sc.Equals (setS1, setS2);
            Assert.IsTrue (eq4);

            setS2 = null;
            bool eq0 = sc.Equals (setS1, setS2);
            Assert.IsFalse (eq0);

            setS1 = null;
            bool eq1 = sc.Equals (setS1, setS2);
            Assert.IsTrue (eq1);
        }
    }
}
