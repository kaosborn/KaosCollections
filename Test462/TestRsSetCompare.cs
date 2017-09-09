//
// Library: KaosCollections
// File:    TestRsSetCompare.cs
//

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
        IEqualityComparer<SortedSet<string>> setComparer
            = SortedSet<string>.CreateSetComparer();
#else
        RankedSet<string> setS1 = new RankedSet<string>(), setS2 = new RankedSet<string>();
        System.Collections.Generic.IEqualityComparer<RankedSet<string>> setComparer
            = RankedSet<string>.CreateSetComparer();
#endif

        [TestMethod]
        public void UnitRsc_ComparerEquals()
        {
#if TEST_BCL
            var setComparer2 = SortedSet<string>.CreateSetComparer();
            var setComparer3 = SortedSet<int>.CreateSetComparer();
#else
            var setComparer2 = RankedSet<string>.CreateSetComparer();
            var setComparer3 = RankedSet<int>.CreateSetComparer();
#endif
            bool eq0 = setComparer.Equals (null);
            Assert.IsFalse (eq0);

            bool eq1 = setComparer.Equals (setComparer);
            Assert.IsTrue (eq1);

            bool eq2 = setComparer.Equals (setComparer2);
            Assert.IsTrue (eq2);

            bool eq3 = setComparer.Equals (setComparer3);
            Assert.IsFalse (eq3);
        }


        [TestMethod]
        public void TestRsc_ComparerGetHashCode()
        {
            int comparerHashCode = setComparer.GetHashCode();

            Assert.AreNotEqual (0, comparerHashCode);
        }


        [TestMethod]
        public void UnitRsc_SetEquals()
        {
            setS1.Add ("ABC");
            setS2.Add ("DEF");
            bool eq2 = setComparer.Equals (setS1, setS2);
            Assert.IsFalse (eq2);

            setS2.Add ("ABC");
            bool eq3 = setComparer.Equals (setS1, setS2);
            Assert.IsFalse (eq3);

            setS1.Add ("DEF");
            bool eq4 = setComparer.Equals (setS1, setS2);
            Assert.IsTrue (eq4);

            setS2 = null;
            bool eq0 = setComparer.Equals (setS1, setS2);
            Assert.IsFalse (eq0);

            setS1 = null;
            bool eq1 = setComparer.Equals (setS1, setS2);
            Assert.IsTrue (eq1);
        }


        [TestMethod]
        public void TestRsc_SetGetHashCode()
        {
            setS1.Add ("ABC");
            setS2.Add ("DEF");

            int h1 = setS1.GetHashCode();
            int h2 = setS2.GetHashCode();

            Assert.AreNotEqual (h1, h2);
        }
    }
}
