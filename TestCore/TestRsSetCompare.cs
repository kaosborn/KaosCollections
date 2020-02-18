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
    public class UserComparer : System.Collections.Generic.Comparer<User>
    {
        public override int Compare(User x, User y) => string.Compare(x.Name, y.Name);
    }

    public class User : System.IComparable<User>
    {
        public string Name { get; private set; }
        public User(string name) { this.Name = name; }
        public int CompareTo(User other) => string.Compare(this.Name, other.Name);
    }

    public partial class TestRs
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
        public void UnitRsc_Equals()
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
        public void UnitRsc_GetHashCode()
        {
            int comparerHashCode0 = setComparer.GetHashCode (null);
            Assert.AreEqual (0, comparerHashCode0);

            int comparerHashCode1 = setComparer.GetHashCode();
            Assert.AreNotEqual (0, comparerHashCode1);
        }


        [TestMethod]
        public void UnitRsc_SetGetHashCode()
        {
            int hc0 = setComparer.GetHashCode (setS1);
            setS1.Add ("ABC");
            int hc1 = setComparer.GetHashCode (setS1);

            Assert.AreNotEqual (hc0, hc1);
        }


        [TestMethod]
        public void UnitRsc_SetEquals1()
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
        public void UnitRsc_SetEquals2()
        {
#if TEST_BCL
            var cp = SortedSet<User>.CreateSetComparer();
            var user1 = new SortedSet<User>(new UserComparer());
            var user2 = new SortedSet<User>(new UserComparer());
#else
            var cp = RankedSet<User>.CreateSetComparer();
            var user1 = new RankedSet<User>(new UserComparer());
            var user2 = new RankedSet<User>(new UserComparer());
#endif
            bool eq0 = cp.Equals (user1, user2);
            Assert.IsTrue (eq0);

            user1.Add (new User ("admin"));
            user2.Add (new User ("tester"));
            bool eq1 = cp.Equals (user1, user2);
            Assert.IsFalse (eq1);

            user1.Add (new User ("tester"));
            user2.Add (new User ("admin"));
            bool eq2 = cp.Equals (user1, user2);
            Assert.IsTrue (eq2);
        }
    }
}
