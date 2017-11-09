//
// Library: KaosCollections
// File:    TestRd.cs
//

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
#if ! TEST_BCL
using Kaos.Collections;
#endif

namespace Kaos.Test.Collections
{
    public partial class TestBtree
    {
        static bool IsEvenValue (KeyValuePair<int,int> kv) => kv.Value % 2 == 0;

        #region Test constructors

        [TestMethod]
        public void UnitRd_Inheritance()
        {
            Setup();

            Assert.IsTrue (tree2 is System.Collections.Generic.IDictionary<string,int>);
            Assert.IsTrue (tree2 is System.Collections.Generic.ICollection<KeyValuePair<string,int>>);
            Assert.IsTrue (tree2 is System.Collections.Generic.IEnumerable<KeyValuePair<string,int>>);
            Assert.IsTrue (tree2 is System.Collections.IEnumerable);
            Assert.IsTrue (tree2 is System.Collections.IDictionary);
            Assert.IsTrue (tree2 is System.Collections.ICollection);

            Assert.IsTrue (tree2 is System.Collections.Generic.IReadOnlyDictionary<string,int>);
            Assert.IsTrue (tree2 is System.Collections.Generic.IReadOnlyCollection<KeyValuePair<string,int>>);

            Assert.IsTrue (tree2.Keys is System.Collections.Generic.ICollection<string>);
            Assert.IsTrue (tree2.Keys is System.Collections.Generic.IEnumerable<string>);
            Assert.IsTrue (tree2.Keys is System.Collections.IEnumerable);
            Assert.IsTrue (tree2.Keys is System.Collections.ICollection);
            Assert.IsTrue (tree2.Keys is System.Collections.Generic.IReadOnlyCollection<string>);

            Assert.IsTrue (tree2.Values is System.Collections.Generic.ICollection<int>);
            Assert.IsTrue (tree2.Values is System.Collections.Generic.IEnumerable<int>);
            Assert.IsTrue (tree2.Values is System.Collections.IEnumerable);
            Assert.IsTrue (tree2.Values is System.Collections.ICollection);
            Assert.IsTrue (tree2.Values is System.Collections.Generic.IReadOnlyCollection<int>);

            Setup();
            System.Collections.IEnumerable roKeys
                = ((System.Collections.Generic.IReadOnlyDictionary<string,int>) tree2).Keys;
            System.Collections.IEnumerable roVals
                = ((System.Collections.Generic.IReadOnlyDictionary<string,int>) tree2).Values;
        }


#if TEST_BCL
        public class DerivedD : SortedDictionary<int,int> { }
#else
        public class DerivedD : RankedDictionary<int,int> { }
#endif

        [TestMethod]
        public void UnitRd_CtorSubclass()
        {
            var sub = new DerivedD();
            bool isRO = ((System.Collections.Generic.IDictionary<int,int>) sub).IsReadOnly;
            Assert.IsFalse (isRO);
        }


        [TestMethod]
        public void UnitRd_Ctor0Empty()
        {
            Setup();
            Assert.AreEqual (0, tree1.Count);
        }


        [TestMethod]
#if TEST_BCL
        [ExpectedException (typeof (ArgumentException))]
#else
        [ExpectedException (typeof (InvalidOperationException))]
#endif
        public void CrashRd_Ctor1NoComparer_InvalidOperation()
        {
            var comp0 = (System.Collections.Generic.Comparer<Person>) null;
#if TEST_BCL
            var d1 = new SortedDictionary<Person,int> (comp0);
#else
            var d1 = new RankedDictionary<Person,int> (comp0);
#endif
            d1.Add (new Person ("Zed"), 1);
            d1.Add (new Person ("Macron"), 2);
        }

        [TestMethod]
        public void UnitRd_Ctor1A1()
        {
#if TEST_BCL
            var tree = new SortedDictionary<string,int> (StringComparer.OrdinalIgnoreCase);
#else
            var tree = new RankedDictionary<string,int> (StringComparer.OrdinalIgnoreCase);
#endif

            tree.Add ("AAA", 0);
            tree.Add ("bbb", 1);
            tree.Add ("CCC", 2);
            tree.Add ("ddd", 3);

            int actualPosition = 0;
            foreach (KeyValuePair<string,int> pair in tree)
            {
                Assert.AreEqual (actualPosition, pair.Value);
                ++actualPosition;
            }

            Assert.AreEqual (4, actualPosition);
        }


        [TestMethod]
        public void UnitRd_Ctor1A2()
        {
#if TEST_BCL
            var tree = new SortedDictionary<string,int> (StringComparer.Ordinal);
#else
            var tree = new RankedDictionary<string,int> (StringComparer.Ordinal);
#endif
            tree.Add ("AAA", 0);
            tree.Add ("bbb", 2);
            tree.Add ("CCC", 1);
            tree.Add ("ddd", 3);

            int actualPosition = 0;
            foreach (KeyValuePair<string,int> pair in tree)
            {
                Assert.AreEqual (actualPosition, pair.Value);
                ++actualPosition;
            }

            Assert.AreEqual (4, actualPosition);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRd_Ctor1B_ArgumentNull()
        {
            IDictionary<int,int> listArg = null;
#if TEST_BCL
            IDictionary<int,int> gcp = new SortedDictionary<int,int> (listArg);
#else
            IDictionary<int,int> gcp = new RankedDictionary<int,int> (listArg);
#endif
        }


        [TestMethod]
        public void UnitRd_Ctor1B()
        {
            Setup();

            IDictionary<string,int> sl = new SortedList<string,int>();
            sl.Add ("Gremlin", 1);
            sl.Add ("Pacer", 2);

#if TEST_BCL
            var dary = new SortedDictionary<string,int> (sl);
#else
            var dary = new RankedDictionary<string,int> (sl);
#endif

            Assert.AreEqual (1, dary["Gremlin"]);
            Assert.AreEqual (2, dary["Pacer"]);
        }

        [TestMethod]
        public void UnitRd_Ctor2()
        {
            IDictionary<Person,int> empDary = new SortedDictionary<Person,int> (new PersonComparer());
            empDary.Add(new KeyValuePair<Person,int>(new Person("fay"), 1));
            empDary.Add(new KeyValuePair<Person,int>(new Person("ann"), 2));
            empDary.Add(new KeyValuePair<Person,int>(new Person("sam"), 3));

#if TEST_BCL
            var people = new SortedDictionary<Person,int> (empDary, new PersonComparer());
#else
            var people = new RankedDictionary<Person,int> (empDary, new PersonComparer());
#endif
            Assert.AreEqual (3, people.Count);
        }

        #endregion

        #region Test properties

        [TestMethod]
        public void UnitRd_Comparer()
        {
            Setup();

            IComparer<string> result = tree2.Comparer;

            Assert.AreEqual (Comparer<string>.Default, result);
        }


        [TestMethod]
        public void UnitRd_Count()
        {
            Setup();

            for (int i = 0; i < iVals1.Length; ++i)
            {
                Assert.AreEqual (i, tree1.Count);
                tree1.Add (iVals1[i], iVals1[i] * 10);
            }
            Assert.AreEqual (iVals1.Length, tree1.Count);

            for (int i = 0; i < iVals1.Length; ++i)
            {
                tree1.Remove (iVals1[i]);
                Assert.AreEqual (iVals1.Length - i - 1, tree1.Count);
            }
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRd_Item_ArgumentNullA()
        {
            Setup();
            tree2[null] = 42;
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRd_Item_ArgumentNullB()
        {
            Setup();
            int zz = tree2[null];
        }


        [TestMethod]
        [ExpectedException (typeof (KeyNotFoundException))]
        public void CrashRd_Item_KeyNotFoundA()
        {
            Setup();
            tree2.Add ("pi", 9);

            int zz = tree2["omicron"];
        }


        [TestMethod]
        [ExpectedException (typeof (KeyNotFoundException))]
        public void CrashRd_Item_KeyNotFoundB()
        {
            Setup();
            tree1.Add (23, 230);

            int zz = tree1[9];
        }


        [TestMethod]
        public void UnitRd_Item()
        {
            Setup();

            tree2["seven"] = 7;
            tree2["eleven"] = 111;

            Assert.AreEqual (7, tree2["seven"]);
            Assert.AreEqual (111, tree2["eleven"]);

            tree2["eleven"] = 11;
            Assert.AreEqual (11, tree2["eleven"]);
        }

        #endregion

        #region Test methods

        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRd_Add_ArgumentNull()
        {
            Setup();
            tree2.Add (null, 0);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRd_Add_Argument()
        {
            Setup();
            tree2.Add ("foo", 1);
            tree2.Add ("foo", 2);
        }


        [TestMethod]
        public void UnitRd_AddCount()
        {
            Setup();

            tree1.Add (12, 120);
            tree1.Add (18, 180);

            Assert.AreEqual (2, tree1.Count);
        }


        [TestMethod]
        public void UnitRd_Clear()
        {
            Setup();

            tree1.Add (41, 410);
            Assert.AreEqual (1, tree1.Count);

            tree1.Clear();
            Assert.AreEqual (0, tree1.Count);

            int actualCount = 0;
            foreach (KeyValuePair<int,int> pair in tree1)
                ++actualCount;

            Assert.AreEqual(0, actualCount);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRd_ContainsKey_ArgumentNull()
        {
            Setup();

            tree2.Add ("gamma", 3);

            // The nongeneric interface allows insert null key, but this is BCL behavior so...
            bool zz = tree2.ContainsKey (null);
        }


        [TestMethod]
        public void UnitRd_ContainsKey()
        {
            Setup();

            int key1 = 26;
            int key2 = 36;
            tree1.Add (key1, key1 * 10);

            Assert.IsTrue (tree1.ContainsKey (key1));
            Assert.IsFalse (tree1.ContainsKey (key2));
        }


        [TestMethod]
        public void UnitRd_ContainsValue()
        {
            Setup();
            int n = 2000;

            for (int ii = 0; ii < n; ii += 2)
                tree1.Add (ii, -ii);

            for (int ii = 0; ii < n; ii += 2)
            {
                Assert.IsTrue (tree1.ContainsValue (-ii));
                Assert.IsFalse (tree1.ContainsValue (-ii - 1));
            }
        }


        [TestMethod]
        public void UnitRd_ContainsValueNullA()
        {
            Setup (4);

            for (int ii = 0; ii < 500; ++ii)
                tree3.Add (ii.ToString(), -ii);

            Assert.IsTrue (tree3.ContainsValue (-9));
            Assert.IsFalse (tree3.ContainsValue (null));

            tree3.Add ("NaN", null);
            Assert.IsTrue (tree3.ContainsValue (null));
        }


        [TestMethod]
        public void UnitRd_ContainsValueNullB()
        {
            Setup();

            tree4.Add (5, "ee");
            tree4.Add (3, "cc");
            tree4.Add (7, "gg");
            Assert.IsFalse (tree4.ContainsValue (null));

            tree4.Add (-1, null);
            Assert.IsFalse (tree4.ContainsValue ("dd"));
            Assert.IsTrue (tree4.ContainsValue ("cc"));
            Assert.IsTrue (tree4.ContainsValue (null));
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRd_CopyTo_ArgumentNull()
        {
            Setup();
            tree1.CopyTo (null, -1);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRd_CopyTo_ArgumentOutOfRange()
        {
            Setup();
            var target = new KeyValuePair<int,int>[iVals1.Length];
            tree1.CopyTo (target, -1);
        }


        // MS docs incorrectly state ArgumentOutOfRangeException for this case.
        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRd_CopyTo_ArgumentA()
        {
            Setup();
            for (int key = 1; key < 10; ++key)
                tree1.Add (key, key + 1000);

            var target = new KeyValuePair<int,int>[10];
            tree1.CopyTo (target, 25);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRd_CopyTo_ArgumentB()
        {
            Setup();
            for (int key = 1; key < 10; ++key)
                tree1.Add (key, key + 1000);

            var target = new System.Collections.Generic.KeyValuePair<int,int>[4];
            tree1.CopyTo (target, 2);
        }


        [TestMethod]
        public void UnitRd_CopyTo()
        {
            Setup();
            int offset = 1;
            int size = 20;

            for (int i = 0; i < size; ++i)
                tree1.Add (i + 1000, i + 10000);

            var pairs = new KeyValuePair<int,int>[size + offset];

            tree1.CopyTo (pairs, offset);

            for (int i = 0; i < offset; ++i)
            {
                Assert.AreEqual (0, pairs[i].Key);
                Assert.AreEqual (0, pairs[i].Value);
            }

            for (int i = 0; i < size; ++i)
            {
                Assert.AreEqual (i + 1000, pairs[i + offset].Key);
                Assert.AreEqual (i + 10000, pairs[i + offset].Value);
            }
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRd_Remove_ArgumentNull()
        {
            Setup();
            tree2.Add ("delta", 4);

            bool isRemoved = tree2.Remove ((String) null);
        }


        [TestMethod]
        public void UnitRd_Remove()
        {
            Setup();

            foreach (int key in iVals1)
                tree1.Add (key, key + 1000);

            int c0 = tree1.Count;
            bool isRemoved1 = tree1.Remove (iVals1[3]);
            bool isRemoved2 = tree1.Remove (iVals1[3]);

            Assert.IsTrue (isRemoved1);
            Assert.IsFalse (isRemoved2);
            Assert.AreEqual (c0 - 1, tree1.Count);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRd_TryGetValue_ArgumentNull()
        {
            Setup();

            int resultValue;
            tree2.TryGetValue (null, out resultValue);
        }


        [TestMethod]
        public void UnitRd_TryGetValueOrUnfoundKeyInt()
        {
            Setup (5);
            for (int i = 2; i <= 50; i += 2)
                tree1.Add (i, i + 300);

            bool result1 = tree1.TryGetValue (5, out int val1);
            bool result2 = tree1.TryGetValue (18, out int val2);
            bool result3 = tree1.TryGetValue (26, out int val3);

            Assert.AreEqual (val2, 318);
            Assert.AreEqual (val3, 326);

            Assert.IsFalse (result1);
            Assert.IsTrue (result2);
            Assert.IsTrue (result3);
        }


        [TestMethod]
        public void UnitRd_TryGetValueForUnfoundKeyString()
        {
#if TEST_BCL
            var sd = new SortedDictionary<string,int> (StringComparer.Ordinal);
#else
            var sd = new RankedDictionary<string,int> (StringComparer.Ordinal) { Capacity=4 };
#endif

            for (char c = 'A'; c <= 'Z'; ++c)
                sd.Add (c.ToString(), (int) c);

            bool result1 = sd.TryGetValue ("M", out int val1);
            bool result2 = sd.TryGetValue ("U", out int val2);
            bool result3 = sd.TryGetValue ("$", out int val3);

            Assert.AreEqual (val1, 'M');
            Assert.AreEqual (val2, 'U');

            Assert.IsTrue (result1);
            Assert.IsTrue (result2);
            Assert.IsFalse (result3);
        }


        [TestMethod]
        public void UnitRd_TryGetValue()
        {
            Setup();

            foreach (int key in iVals1)
                tree1.Add (key, key + 1000);

            int resultValue;
            tree1.TryGetValue (iVals1[0], out resultValue);

            Assert.AreEqual (iVals1[0] + 1000, resultValue);

            tree1.TryGetValue (50, out resultValue);
            Assert.AreEqual (default (int), resultValue);
        }

        #endregion

        #region Test enumeration

        [TestMethod]
        public void UnitRd_GetEnumeratorOnEmpty()
        {
            int actual = 0;
            Setup();

            using (var e1 = tree2.GetEnumerator())
            {
                while (e1.MoveNext())
                    ++actual;
                var zz = e1.Current;
            }

            Assert.AreEqual (0, actual);
        }


        [TestMethod]
        public void UnitRd_GetEnumeratorPastEnd()
        {
            bool isMoved;
            int actual1 = 0, total1 = 0;

            Setup();
            tree2.Add ("three", 3);
            tree2.Add ("one", 1);
            tree2.Add ("five", 5);

            using (var e1 = tree2.GetEnumerator())
            {
                while (e1.MoveNext())
                {
                    ++actual1;
                    total1 += e1.Current.Value;
                }

                isMoved = e1.MoveNext();
            }

            Assert.AreEqual (3, actual1);
            Assert.AreEqual (9, total1);
            Assert.IsFalse (isMoved);
        }


        [TestMethod]
        public void UnitRd_Etor()
        {
            Setup();

            for (int i = 0; i < iVals1.Length; ++i)
                tree1.Add (iVals1[i], iVals1[i] + 1000);

            int actualCount = 0;

            foreach (KeyValuePair<int,int> pair in tree1)
            {
                ++actualCount;
                Assert.AreEqual (pair.Key + 1000, pair.Value);
            }

            Assert.AreEqual (iVals1.Length, actualCount);
        }


        [TestMethod]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRd_oeCurrent_InvalidOperationA()
        {
            Setup();
            tree2.Add ("cc", 3);
            IEnumerator<KeyValuePair<string,int>> kvEtor = tree2.GetEnumerator();

            object zz = ((System.Collections.IEnumerator) kvEtor).Current;
        }


        [TestMethod]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRd_oeCurrent_InvalidOperationB()
        {
            Setup();
            tree2.Add ("cc", 3);
            IEnumerator<KeyValuePair<string,int>> kvEtor = tree2.GetEnumerator();
            kvEtor.MoveNext();
            kvEtor.MoveNext();

            object zz = ((System.Collections.IEnumerator) kvEtor).Current;
        }

        [TestMethod]
        public void UnitRd_EtorPair()
        {
            Setup();
            tree2.Add ("nine", 9);
            IEnumerator<KeyValuePair<string,int>> kvEnum = tree2.GetEnumerator();

            KeyValuePair<string,int> pair0 = kvEnum.Current;
            Assert.AreEqual (default (int), pair0.Value);
            Assert.AreEqual (default (string), pair0.Key);

            kvEnum.MoveNext();
            KeyValuePair<string,int> pair = kvEnum.Current;
            Assert.AreEqual (9, pair.Value);
            Assert.AreEqual ("nine", pair.Key);

            kvEnum.MoveNext();
            pair = kvEnum.Current;
            Assert.AreEqual (default (string), pair.Key);
            Assert.AreEqual (default (int), pair.Value);
        }


        [TestMethod]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRd_EtorHotUpdate()
        {
            Setup (4);
            tree2.Add ("vv", 1);
            tree2.Add ("mm", 2);
            tree2.Add ("qq", 3);

            int n = 0;
            foreach (var kv in tree2)
            {
                if (++n == 2)
                    tree2.Add ("breaks enum", 4);
            }
        }

        [TestMethod]
        public void UnitRd_oGetEnumerator()
        {
            Setup();
            var aa = (System.Collections.IEnumerable) tree1;
            var bb = aa.GetEnumerator();
        }

        #endregion

        #region Test explicit implementation

        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRd_pcAdd_Argument()
        {
            Setup();
            var pc = (ICollection<KeyValuePair<string,int>>) tree2;

            var p1 = new KeyValuePair<string,int> ("beta", 1);
            var p2 = new KeyValuePair<string,int> (null, 98);
            var p3 = new KeyValuePair<string,int> (null, 99);

            pc.Add (p1);

            // Adding a null key is allowed here!
            pc.Add (p2);

            int result2 = tree2.Count;
            Assert.AreEqual (2, result2);

            // Should bomb on the second null key.
            pc.Add (p3);
        }


        [TestMethod]
        public void UnitRd_pcAdd()
        {
            Setup();
            var pc = (ICollection<KeyValuePair<int,int>>) tree1;

            var p1 = new KeyValuePair<int,int> (17, 170);
            pc.Add (p1);

            Assert.AreEqual (1, tree1.Count);
            Assert.IsTrue (tree1.ContainsKey (17));
        }


        [TestMethod]
        public void UnitRd_pcContains()
        {
            Setup (4);
            var pc = (ICollection<KeyValuePair<string,int>>) tree2;

            var nullKv = new KeyValuePair<string,int> (null, 0);
            var zedKv = new KeyValuePair<string,int> ("z", 0);

            foreach (var kv in greek)
                pc.Add (kv);

            foreach (var kv in greek)
                Assert.IsTrue (pc.Contains (kv));

            Assert.IsFalse (pc.Contains (zedKv));
            Assert.IsFalse (pc.Contains (nullKv));

            // key of null allowed with generic collection explicit interface:
            pc.Add (nullKv);
            Assert.IsTrue (pc.Contains (nullKv));
        }


        [TestMethod]
        public void UnitRd_pcComparePairNullRef()
        {
            Setup();
            var pc = (ICollection<KeyValuePair<int,string>>) tree4;

            tree4.Add (3, "cc");
            tree4.Add (1, "aa");

            var pair0 = new KeyValuePair<int,string> (0, null);
            var pair1 = new KeyValuePair<int,string> (3, "cc");


            Assert.IsFalse (pc.Contains (pair0));
            Assert.IsTrue (pc.Contains (pair1));

            tree4.Add (0, null);
            Assert.IsTrue (pc.Contains (pair0));
        }


        [TestMethod]
        public void UnitRd_pcEtor()
        {
            Setup();
            var pc = (ICollection<KeyValuePair<int,int>>) tree1;

            foreach (int k in iVals1)
                tree1.Add (k, k + 100);

            int actualCount = 0;
            foreach (KeyValuePair<int,int> pair in pc)
            {
                Assert.AreEqual (pair.Key + 100, pair.Value);
                ++actualCount;
            }

            Assert.AreEqual (iVals1.Length, actualCount);
        }


        [TestMethod]
        public void UnitRd_peEtor()
        {
            Setup();
            var pe = (IEnumerable<KeyValuePair<int,int>>) tree1;

            foreach (int val in iVals1)
                tree1.Add (val, val + 100);

            int actualCount = 0;
            foreach (KeyValuePair<int,int> pair in pe)
            {
                Assert.AreEqual (pair.Key + 100, pair.Value);
                ++actualCount;
            }

            Assert.AreEqual (iVals1.Length, actualCount);
        }


        [TestMethod]
        public void UnitRd_oeGetEtor()
        {
            Setup();
            var oe = (System.Collections.IEnumerable) tree4;

            tree4.Add (3, "cc");

            int rowCount = 0;
            foreach (object row in oe)
            {
                var kv = (KeyValuePair<int,string>) row;
                Assert.AreEqual (3, kv.Key);
                Assert.AreEqual ("cc", kv.Value);
                ++rowCount;
            }

            Assert.AreEqual (1, rowCount);
        }


        [TestMethod]
        public void UnitRd_pcIsReadonly()
        {
            Setup();
            var pc = (ICollection<KeyValuePair<int,int>>) tree1;

            Assert.IsFalse (pc.IsReadOnly);
        }


        [TestMethod]
        public void UnitRd_gdKeys()
        {
            Setup();
            tree2.Add ("alpha", 1);
            tree2.Add ("beta", 2);
            var gd = (IDictionary<string,int>) tree2;
            int count = gd.Keys.Count;
            Assert.AreEqual (2, count);
        }


        [TestMethod]
        public void UnitRd_pcRemovePair()
        {
            Setup();
            var pc = (ICollection<KeyValuePair<string,int>>) tree2;

            var pair0 = new KeyValuePair<string,int> (null, 0);
            var pair1 = new KeyValuePair<string,int> ("ten", 10);
            var pair2 = new KeyValuePair<string,int> ("ten", 100);
            var pair3 = new KeyValuePair<string,int> ("twenty", 20);

            pc.Add (pair0);
            pc.Add (pair1);
            pc.Add (pair3);
            Assert.AreEqual (3, tree2.Count);

            bool isRemoved = pc.Remove (pair0);
            Assert.IsTrue (isRemoved);
            Assert.AreEqual (2, tree2.Count);

            isRemoved = pc.Remove (pair0);
            Assert.IsFalse (isRemoved);
            Assert.AreEqual (2, tree2.Count);

            isRemoved = pc.Remove (pair2);
            Assert.AreEqual (2, tree2.Count);

            isRemoved = pc.Remove (pair1);
            Assert.IsTrue (isRemoved);
            Assert.AreEqual (1, tree2.Count);
        }


        [TestMethod]
        public void UnitRd_pcRemovePairNull()
        {
            Setup();
            tree4.Add (3, "cc");
            tree4.Add (5, "ee");
            tree4.Add (4, null);

            var gpc = (ICollection<KeyValuePair<int,string>>) tree4;
            bool isOK = gpc.Remove (new KeyValuePair<int,string> (99, null));
            Assert.IsFalse (isOK);

            isOK = gpc.Remove (new KeyValuePair<int,string> (4, null));
            Assert.IsTrue (isOK);

            isOK = tree4.ContainsKey (4);
            Assert.IsFalse (isOK);
        }


        [TestMethod]
        public void UnitRd_gdValues()
        {
            Setup();
            tree2.Add ("alpha", 1);
            tree2.Add ("beta", 2);
            var gd = (IDictionary<string,int>) tree2;
            int count = gd.Values.Count;
            Assert.AreEqual (2, count);
        }

        #endregion

        #region Test bonus methods
#if ! TEST_BCL

        [TestMethod]
        public void UnitRd_xMinMax()
        {
            var rd = new RankedDictionary<int,int> { Capacity=4 };

            int min0 = rd.MinKey;
            int max0 = rd.MaxKey;

            Assert.AreEqual (default (int), min0);
            Assert.AreEqual (default (int), max0);

            for (int i1 = 1; i1 <= 99; ++i1)
                rd.Add (i1, i1 + 100);

            int min = rd.MinKey;
            int max = rd.MaxKey;

            Assert.AreEqual (1, min);
            Assert.AreEqual (99, max);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRd_xCapacity_ArgumentOutOfRange()
        {
            var rd = new RankedDictionary<int,int>();
            rd.Capacity = -1;
        }

        [TestMethod]
        public void UnitRd_xCapacity()
        {
            var rd = new RankedDictionary<int,int>();
            var initial = rd.Capacity;

            rd.Capacity = 0;
            Assert.AreEqual (initial, rd.Capacity);

            rd.Capacity = 3;
            Assert.AreEqual (initial, rd.Capacity);

            rd.Capacity = 257;
            Assert.AreEqual (initial, rd.Capacity);

            rd.Capacity = 4;
            Assert.AreEqual (4, rd.Capacity);

            rd.Capacity = 256;
            Assert.AreEqual (256, rd.Capacity);

            rd.Add (1, 11);
            rd.Capacity = 128;
            Assert.AreEqual (256, rd.Capacity);
        }


        [TestMethod]
        public void UnitRd_xElementsBetween()
        {
            var rd = new RankedDictionary<int,int>();

            for (int i = 90; i >= 0; i -= 10)
                rd.Add (i, -100 - i);

            int iterations = 0;
            int sumVals = 0;
            foreach (var kv in rd.ElementsBetween (35, 55))
            {
                ++iterations;
                sumVals += kv.Value;
            }

            Assert.AreEqual (2, iterations);
            Assert.AreEqual (-290, sumVals);
        }

        [TestMethod]
        public void UnitRd_xElementsBetweenPassedEnd()
        {
            var rd = new RankedDictionary<int,int>();

            for (int i = 0; i < 1000; ++i)
                rd.Add (i, -i);

            int iterations = 0;
            int sumVals = 0;
            foreach (KeyValuePair<int,int> e in rd.ElementsBetween (500, 1500))
            {
                ++iterations;
                sumVals += e.Value;
            }

            Assert.AreEqual (500, iterations);
            Assert.AreEqual (-374750, sumVals, "Sum of values not correct");
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRd_xElementsFrom_ArgumentNull()
        {
            var rd = new RankedDictionary<string,int>();
            foreach (var pair in rd.ElementsFrom (null))
            { }
        }

        [TestMethod]
        public void UnitRd_xElementsFrom()
        {
            var rd = new RankedDictionary<int,int>();

            for (int i = 1; i <= 1000; ++i)
                rd.Add (i, -i);

            int firstKey = -1;
            int iterations = 0;
            foreach (var e in rd.ElementsFrom (501))
            {
                if (iterations == 0)
                    firstKey = e.Key;
                ++iterations;
            }

            Assert.AreEqual (501, firstKey);
            Assert.AreEqual (500, iterations);
        }

        [TestMethod]
        public void UnitRd_xElementsFromMissingVal()
        {
            var rd = new RankedDictionary<int,int>();

            for (int i = 0; i < 1000; i += 2)
                rd.Add (i, -i);

            for (int i = 1; i < 999; i += 2)
            {
                bool isFirst = true;
                foreach (var x in rd.ElementsFrom (i))
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
        public void UnitRd_xElementsFromPassedEnd()
        {
            var rd = new RankedDictionary<int,int>();

            for (int i = 0; i < 1000; ++i)
                rd.Add (i, -i);

            int iterations = 0;
            foreach (var x in rd.ElementsFrom (2000))
                ++iterations;

            Assert.AreEqual (0, iterations, "SkipUntilKey shouldn't find anything");
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRd_xElementsBetweenIndexes_ArgumentOutOfRangeA()
        {
            var rd = new RankedDictionary<int,int> { { 0,0 }, { 1,-1 } };
            foreach (var pair in rd.ElementsBetweenIndexes (-1, 0))
            { }
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRd_xElementsBetweenIndexes_ArgumentOutOfRangeB()
        {
            var rd = new RankedDictionary<int,int> { { 0,0 }, { 1,-1 } };
            foreach (var pair in rd.ElementsBetweenIndexes (2, 0))
            { }
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRd_xElementsBetweenIndexes_ArgumentOutOfRangeC()
        {
            var rd = new RankedDictionary<int,int> { { 0,0 }, { 1,-1 } };
            foreach (var pair in rd.ElementsBetweenIndexes (0, -1))
            { }
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRd_xElementsBetweenIndexes_ArgumentOutOfRangeD()
        {
            var rd = new RankedDictionary<int,int> { { 0,0 }, { 1,-1 } };
            foreach (var pair in rd.ElementsBetweenIndexes (0, 2))
            { }
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRd_xElementsBetweenIndexes_Argument()
        {
            var rd = new RankedDictionary<int,int> { { 0,0 }, { 1,-1 }, { 2,-2 } };
            foreach (var pair in rd.ElementsBetweenIndexes (2, 1))
            { }
        }

        [TestMethod]
        public void UnitRd_xElementsBetweenIndexes()
        {
            int n = 30;
            var rd = new RankedDictionary<int,int> { Capacity=4 };
            for (int ii = 0; ii < n; ++ii)
                rd.Add (ii, -ii);

            for (int p1 = 0; p1 < n; ++p1)
                for (int p2 = p1; p2 < n; ++p2)
                {
                    int actual = 0;
                    foreach (var pair in rd.ElementsBetweenIndexes (p1, p2))
                        actual += pair.Key;

                    int expected = (p2 - p1 + 1) * (p1 + p2) / 2;
                    Assert.AreEqual (expected, actual);
                }
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRd_xIndexOfKey_ArgumentNull()
        {
            var rd = new RankedDictionary<string,int>();
            int ix = rd.IndexOfKey (null);
        }

        [TestMethod]
        public void UnitRd_xIndexOfKey()
        {
            var rd = new RankedDictionary<int,int> { Capacity=5 };
            for (int ii = 0; ii < 500; ii+=2)
                rd.Add (ii, ii+1000);

            for (int ii = 0; ii < 500; ii+=2)
            {
                int ix = rd.IndexOfKey (ii);
                Assert.AreEqual (ii/2, ix);
            }

            int iw = rd.IndexOfKey (-1);
            Assert.AreEqual (~0, iw);

            int iy = rd.IndexOfKey (500);
            Assert.AreEqual (~250, iy);
        }


        [TestMethod]
        public void UnitRd_xIndexOfValue()
        {
            var rd = new RankedDictionary<int,int>();
            for (int ii = 0; ii < 500; ++ii)
                rd.Add (ii, ii+1000);

            var ix1 = rd.IndexOfValue (1400);
            Assert.AreEqual (400, ix1);

            var ix2 = rd.IndexOfValue (88888);
            Assert.AreEqual (-1, ix2);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRd_xRemoveAtA_ArgumentOutOfRange()
        {
            var rd = new RankedDictionary<int,int>();
            rd.Add (42, 24);
            rd.RemoveAt (-1);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRd_xRemoveAtB_ArgumentOutOfRange()
        {
            var rd = new RankedDictionary<int,int>();
            rd.RemoveAt (0);
        }

        [TestMethod]
        public void UnitRd_xRemoveAt()
        {
            var rd = new RankedDictionary<int,int>();
            for (int ii = 0; ii < 5000; ++ii)
                rd.Add (ii, -ii);

            for (int i2 = 4900; i2 >= 0; i2 -= 100)
                rd.RemoveAt (i2);

            for (int i2 = 0; i2 < 5000; ++i2)
                if (i2 % 100 == 0)
                    Assert.IsFalse (rd.ContainsKey (i2));
                else
                    Assert.IsTrue (rd.ContainsKey (i2));
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRd_xRemoveRange_ArgumentOutOfRangeA()
        {
            var rd = new RankedDictionary<int,int>();
            rd.RemoveRange (-1, 0);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRd_xRemoveRange_ArgumentOutOfRangeB()
        {
            var rd = new RankedDictionary<int,int>();
            rd.RemoveRange (0, -1);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRd_xRemoveRange_Argument()
        {
            var rd = new RankedDictionary<int,int>();
            rd.Add (3, 33); rd.Add (5, 55);
            rd.RemoveRange (1, 2);
        }

        [TestMethod]
        public void UnitRd_xRemoveRange()
        {
            var rd = new RankedDictionary<int,int> { Capacity=7 };
            for (int ii=0; ii<20; ++ii) rd.Add (ii, -ii);

            rd.RemoveRange (20, 0);
            Assert.AreEqual (20, rd.Count);

            rd.RemoveRange (12, 4);
            Assert.AreEqual (16, rd.Count);
#if DEBUG
            rd.SanityCheck();
#endif
        }


        [TestMethod]
        public void UnitRd_xTryGetValueIndex()
        {
            var rd = new RankedDictionary<int,int>();
            rd.Capacity = 5;
            for (int ii = 0; ii < 500; ii+=2)
                rd.Add (ii, ii+1000);

            for (int ii = 0; ii < 500; ii+=2)
            {
                bool isOk = rd.TryGetValueAndIndex (ii, out int v1, out int i1);

                Assert.IsTrue (isOk);
                Assert.AreEqual (ii/2, i1);
                Assert.AreEqual (ii+1000, v1);
            }

            bool isOkNot = rd.TryGetValueAndIndex (111, out int v2, out int i2);
            Assert.IsFalse (isOkNot);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRd_RemoveWhere_ArgumentNull()
        {
            var rd = new RankedDictionary<int,int>();
            rd.RemoveWhere (null);
        }

        [TestMethod]
        public void UnitRd_xRemoveWhereA()
        {
            var rd = new RankedDictionary<int,int>();

            for (int ix = 0; ix < 1000; ++ix)
                rd.Add (ix, ix + 1000);

            int c0 = rd.Count;
            int removed = rd.RemoveWhere (IsEven);

            Assert.AreEqual (500, removed);
            foreach (int key in rd.Keys)
                Assert.IsTrue (key % 2 != 0);
        }


        [TestMethod]
        public void UnitRd_xRemoveWhereB()
        {
            int n = 2000;
            var rd = new RankedDictionary<int,int> { Capacity=7 };

            for (int ix = 0; ix < n; ++ix)
                rd.Add (ix, -ix);

            int removed = rd.RemoveWhere (IsAlways);

            Assert.AreEqual (n, removed);
            Assert.AreEqual (0, rd.Count);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRd_xRemoveWherePair_ArgumentNull()
        {
            var rd = new RankedDictionary<int,int>();
            rd.RemoveWherePair (null);
        }

        [TestMethod]
        public void UnitRd_xRemoveWherePair()
        {
            var rd = new RankedDictionary<int,int>();

            for (int ix = 0; ix < 1000; ++ix)
                rd.Add (ix, -ix);

            int c0 = rd.Count;
            int removed = rd.RemoveWherePair (IsEvenValue);

            Assert.AreEqual (500, removed);
            foreach (int val in rd.Values)
                Assert.IsTrue (val % 2 != 0);
        }


        [TestMethod]
        public void UnitRd_xReverseEmpty()
        {
            int total = 0;
            Setup (5);

            foreach (var countdown in tree1.Reverse())
               ++total;

            Assert.AreEqual (0, total);
        }

        [TestMethod]
        public void UnitRd_xReverse()
        {
            var rd = new RankedDictionary<int,int> { Capacity=5 };
            int expected = 500;

            for (int ii=1; ii <= expected; ++ii)
                rd.Add (ii, -ii);

            foreach (var actual in rd.Reverse())
            {
                Assert.AreEqual (expected, actual.Key);
                Assert.AreEqual (-expected, actual.Value);
                --expected;
            }
            Assert.AreEqual (0, expected);
        }

#endif
        #endregion
    }
}
