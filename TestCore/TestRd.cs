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

namespace CollectionsTest
{
    public partial class Test_Btree
    {
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
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRd_Ctor1NoComparer_InvalidOperation()
        {
            var comp0 = (System.Collections.Generic.Comparer<Person>) null;
#if TEST_BCL
            var d1 = new SortedDictionary<Person,int> (comp0);
#else
            var d1 = new SortedDictionary<Person,int> (comp0);
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
            tree.Add ("CCC", 1);
            tree.Add ("bbb", 2);
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
            IDictionary<int,int> idx = new SortedDictionary<int,int> (listArg);
#else
            IDictionary<int,int> idx = new RankedDictionary<int,int> (listArg);
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
            var tree = new SortedDictionary<string,int> (sl);
#else
            var tree = new RankedDictionary<string,int> (sl);
#endif

            Assert.AreEqual (1, tree["Gremlin"]);
            Assert.AreEqual (2, tree["Pacer"]);
        }

        [TestMethod]
        public void UnitRd_Ctor2A()
        {
            IDictionary<Person,int> empDary = new SortedDictionary<Person,int>(new PersonComparer());
            empDary.Add (new KeyValuePair<Person,int> (new Person ("fay"), 1));
            empDary.Add (new KeyValuePair<Person,int> (new Person ("ann"), 2));
            empDary.Add (new KeyValuePair<Person,int> (new Person ("sam"), 3));

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
        public void CrashRd_Item1_ArgumentNull()
        {
            Setup();
            tree2[null] = 42;
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRd_Item2_ArgumentNull()
        {
            Setup();
            int x = tree2[null];
        }


        [TestMethod]
        [ExpectedException (typeof (KeyNotFoundException))]
        public void CrashRd_Item1_KeyNotFound()
        {
            Setup();
            tree2.Add ("pi", 9);

            int x = tree2["omicron"];
        }


        [TestMethod]
        [ExpectedException (typeof (KeyNotFoundException))]
        public void CrashRd_Item2_KeyNotFound()
        {
            Setup();
            tree1.Add (23, 230);

            int val = tree1[9];
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

        // Keys, Values property testing is implicit in their API tests.

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
        public void CrashRd_AddEntryAlreadyExists_Argument()
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

            Assert.AreEqual (0, actualCount);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void CrashRd_ContainsKey_ArgumentNull()
        {
            Setup();

            tree2.Add ("gamma", 3);
            bool result = tree2.ContainsKey (null);
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

            int key1 = 26;
            int key2 = 36;
            int key3 = 46;
            tree1.Add (key1, key1 + 1000);
            tree1.Add (key3, key3 + 1000);

            Assert.IsTrue (tree1.ContainsValue (key1 + 1000));
            Assert.IsFalse (tree1.ContainsValue (key2 + 1000));
        }


        [TestMethod]
        public void UnitRd_ContainsValueNullStruct()
        {
            Setup();

            tree3.Add ("9", 9);
            Assert.IsTrue (tree3.ContainsValue (9));
            Assert.IsFalse (tree3.ContainsValue (null));

            tree3.Add ("unknown", null);
            Assert.IsTrue (tree3.ContainsValue (null));
        }


        [TestMethod]
        public void UnitRd_ContainsValueNullRef()
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
            var target = new KeyValuePair<int,int>[iVals1.Length];
            tree1.CopyTo (null, -1);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRd_CopyTo1_ArgumentOutOfRange()
        {
            Setup();
            var target = new KeyValuePair<int,int>[iVals1.Length];
            tree1.CopyTo (target, -1);
        }


        // MS docs incorrectly state ArgumentOutOfRangeException for this case.
        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRd_CopyTo1_Argument()
        {
            Setup();
            for (int key = 1; key < 10; ++key)
                tree1.Add (key, key + 1000);

            var target = new KeyValuePair<int,int>[10];
            tree1.CopyTo (target, 25);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRd_CopyTo2_Argument()
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

            bool isRemoved = tree2.Remove ((string) null);
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
            for (int i = 2; i <= 50; i+=2)
                tree1.Add (i, i+300);

            int val1, val2, val3;

            bool result1 = tree1.TryGetValue (5, out val1);
            bool result2 = tree1.TryGetValue (18, out val2);
            bool result3 = tree1.TryGetValue (26, out val3);

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
            var sd = new RankedDictionary<string,int> (StringComparer.Ordinal);
#endif

            for (char c = 'A'; c <= 'Z'; ++c)
                sd.Add (c.ToString(), (int) c);

            int val1, val2, val3;

            bool result1 = sd.TryGetValue ("M", out val1);
            bool result2 = sd.TryGetValue ("U", out val2);
            bool result3 = sd.TryGetValue ("$", out val3);

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
            int actual=0;
            Setup();

            using (var e1 = tree2.GetEnumerator())
            {
                while (e1.MoveNext())
                    ++actual;
                var junk = e1.Current;
            }

            Assert.AreEqual (0, actual);
        }


        [TestMethod]
        public void UnitRd_GetEnumeratorPastEnd()
        {
            bool isMoved;
            int actual1=0, total1=0;

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
        public void UnitRd_EnumeratorIteration()
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
        public void CrashRd_EnumeratorObjectPair1_InvalidOperation()
        {
            Setup();
            tree2.Add ("cc", 3);
            IEnumerator<KeyValuePair<string,int>> kvEnum = tree2.GetEnumerator();

            object jp = ((System.Collections.IEnumerator) kvEnum).Current;
        }


        [TestMethod]
        [ExpectedException (typeof (InvalidOperationException))]
        public void CrashRd_EnumeratorObjectPair2_InvalidOperation()
        {
            Setup();
            tree2.Add ("cc", 3);
            IEnumerator<KeyValuePair<string,int>> kvEnum = tree2.GetEnumerator();
            kvEnum.MoveNext();
            kvEnum.MoveNext();

            object jp = ((System.Collections.IEnumerator) kvEnum).Current;
        }


        [TestMethod]
        public void UnitRd_EnumeratorPair()
        {
            Setup();
            tree2.Add ("nine", 9);
            IEnumerator<KeyValuePair<string,int>> kvEnum = tree2.GetEnumerator();

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
        public void CrashRd_EnumHotUpdate()
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
        public void UnitRd_ObjectGetEnumerator()
        {
            Setup();
            var aa = (System.Collections.IEnumerable) tree1;
            var bb = aa.GetEnumerator();
        }

        #endregion

        #region Test ICollection implementation

        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void CrashRd_ICollectionAddPairEntryAlreadyExists_Argument()
        {
            Setup();
            var p1 = new KeyValuePair<string,int> ("beta", 1);
            var p2 = new KeyValuePair<string,int> (null, 98);
            var p3 = new KeyValuePair<string,int> (null, 99);

            genCol2.Add (p1);

            // Adding a null key is allowed here!
            genCol2.Add (p2);

            int result2 = tree2.Count;
            Assert.AreEqual (2, result2);

            // Should bomb on the second null key.
            genCol2.Add (p3);
        }


        [TestMethod]
        public void UnitRd_ICollectionAddPair()
        {
            Setup();
            var gic = (ICollection<KeyValuePair<int,int>>) tree1;

            var p1 = new KeyValuePair<int,int> (17, 170);
            gic.Add (p1);

            Assert.AreEqual (1, tree1.Count);
            Assert.IsTrue (tree1.ContainsKey (17));
        }


        [TestMethod]
        public void UnitRd_ICollectionContainsPair()
        {
            Setup();
            KeyValuePair<string,int> pair0 = new KeyValuePair<string,int> (null, 0);
            KeyValuePair<string,int> pair1 = new KeyValuePair<string,int> ("alpha", 1);
            KeyValuePair<string,int> pair2 = new KeyValuePair<string,int> ("delta", 4);
            KeyValuePair<string,int> pair3 = new KeyValuePair<string,int> ("zed", 99);

            tree2.Add (pair1.Key, pair1.Value);

            Assert.IsTrue (genCol2.Contains (pair1));
            Assert.IsFalse (genCol2.Contains (pair2));
            Assert.IsFalse (genCol2.Contains (pair0));
            Assert.IsFalse (genCol2.Contains (pair3));
        }


        [TestMethod]
        public void UnitRd_ICollectionComparePairNullRef()
        {
            Setup();
            tree4.Add (3, "cc");
            tree4.Add (1, "aa");

            var pair0 = new KeyValuePair<int,string> (0, null);
            var pair1 = new KeyValuePair<int,string> (3, "cc");

            var gcol = (ICollection<KeyValuePair<int,string>>) tree4;

            Assert.IsFalse (gcol.Contains (pair0));
            Assert.IsTrue (gcol.Contains (pair1));

            tree4.Add (0, null);
            Assert.IsTrue (gcol.Contains (pair0));
        }


        [TestMethod]
        public void UnitRd_ICollectionGetEnumerator()
        {
            Setup();
            var gic = (ICollection<KeyValuePair<int,int>>) tree1;


            foreach (int k in iVals1)
                tree1.Add (k, k + 100);

            int actualCount = 0;
            foreach (KeyValuePair<int,int> pair in gic)
            {
                Assert.AreEqual (pair.Key + 100, pair.Value);
                ++actualCount;
            }

            Assert.AreEqual (iVals1.Length, actualCount);
        }


        [TestMethod]
        public void UnitRd_IEnumerablePairGetEnumerator()
        {
            Setup();
            foreach (int k in iVals1)
                tree1.Add (k, k + 100);

            var x = (IEnumerable<KeyValuePair<int,int>>) tree1;

            int actualCount = 0;
            foreach (KeyValuePair<int,int> pair in x)
            {
                Assert.AreEqual (pair.Key + 100, pair.Value);
                ++actualCount;
            }

            Assert.AreEqual (iVals1.Length, actualCount);
        }


        [TestMethod]
        public void UnitRd_IEnumerableGetEnumerator()
        {
            Setup();
            tree4.Add (3, "cc");
            int rowCount = 0;

            foreach (var row in (System.Collections.IEnumerable) tree4)
            {
                var kv = (KeyValuePair<int,string>) row;
                Assert.AreEqual (3, kv.Key);
                Assert.AreEqual ("cc", kv.Value);
                ++rowCount;
            }

            Assert.AreEqual (1, rowCount);
        }


        [TestMethod]
        public void UnitRd_ICollectionIsReadonly()
        {
            Setup();
            var gic = (ICollection<KeyValuePair<int,int>>) tree1;

            Assert.IsFalse (gic.IsReadOnly);
        }


        [TestMethod]
        public void UnitRd_IDictionaryKeys()
        {
            Setup();
            tree2.Add ("alpha", 1);
            tree2.Add ("beta", 2);
            var genId = (IDictionary<string,int>) tree2;
            int count = genId.Keys.Count;
            Assert.AreEqual (2, count);
        }


        [TestMethod]
        public void UnitRd_ICollectionRemovePair()
        {
            Setup();

            KeyValuePair<string,int> pair0 = new KeyValuePair<string,int> (null, 0);
            KeyValuePair<string,int> pair1 = new KeyValuePair<string,int> ("ten", 10);
            KeyValuePair<string,int> pair2 = new KeyValuePair<string,int> ("ten", 100);
            KeyValuePair<string,int> pair3 = new KeyValuePair<string,int> ("twenty", 20);

            genCol2.Add (pair0);
            genCol2.Add (pair1);
            genCol2.Add (pair3);
            Assert.AreEqual (3, tree2.Count);

            bool isRemoved = genCol2.Remove (pair0);
            Assert.IsTrue (isRemoved);
            Assert.AreEqual (2, tree2.Count);

            isRemoved = genCol2.Remove (pair0);
            Assert.IsFalse (isRemoved);
            Assert.AreEqual (2, tree2.Count);

            isRemoved = genCol2.Remove (pair2);
            Assert.AreEqual (2, tree2.Count);

            isRemoved = genCol2.Remove (pair1);
            Assert.IsTrue (isRemoved);
            Assert.AreEqual (1, tree2.Count);
        }


        [TestMethod]
        public void UnitRd_ICollectionRemovePairNull()
        {
            Setup();
            tree4.Add (3, "cc");
            tree4.Add (5, "ee");
            tree4.Add (4, null);

            var pc = (ICollection<KeyValuePair<int,string>>) tree4;
            bool isOK = pc.Remove (new KeyValuePair<int,string> (99, null));
            Assert.IsFalse (isOK);

            isOK = pc.Remove (new KeyValuePair<int,string> (4, null));
            Assert.IsTrue (isOK);

            isOK = tree4.ContainsKey (4);
            Assert.IsFalse (isOK);
        }


        [TestMethod]
        public void UnitRd_IDictionaryValues()
        {
            Setup();
            tree2.Add ("alpha", 1);
            tree2.Add ("beta", 2);
            var genId = (IDictionary<string,int>) tree2;
            int count = genId.Values.Count;
            Assert.AreEqual (2, count);
        }

        #endregion

        #region Test bonus methods
#if ! TEST_BCL

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRdx_Capacity_ArgumentOutOfRange()
        {
            var bt = new RankedDictionary<int,int>();
            bt.Capacity = -1;
        }

        [TestMethod]
        public void UnitRdx_Capacity()
        {
            var bt = new RankedDictionary<int,int>();
            var initial = bt.Capacity;

            bt.Capacity = 0;
            Assert.AreEqual (initial, bt.Capacity);

            bt.Capacity = 3;
            Assert.AreEqual (initial, bt.Capacity);

            bt.Capacity = 257;
            Assert.AreEqual (initial, bt.Capacity);

            bt.Capacity = 4;
            Assert.AreEqual (4, bt.Capacity);

            bt.Capacity = 256;
            Assert.AreEqual (256, bt.Capacity);

            bt.Add (1, 11);
            bt.Capacity = 128;
            Assert.AreEqual (256, bt.Capacity);
        }


        [TestMethod]
        public void UnitRdx_GetBetween()
        {
            var bt = new RankedDictionary<int,int>();

            for (int i = 90; i >= 0; i -= 10)
                bt.Add (i, -100 - i);

            int iterations = 0;
            int sumVals = 0;
            foreach (var kv in bt.GetBetween (35, 55))
            {
                ++iterations;
                sumVals += kv.Value;
            }

            Assert.AreEqual (2, iterations);
            Assert.AreEqual (-290, sumVals);
        }

        [TestMethod]
        public void UnitRdx_GetBetweenPassedEnd()
        {
            var btree = new RankedDictionary<int,int>();

            for (int i = 0; i < 1000; ++i)
                btree.Add (i, -i);

            int iterations = 0;
            int sumVals = 0;
            foreach (KeyValuePair<int,int> e in btree.GetBetween (500, 1500))
            {
                ++iterations;
                sumVals += e.Value;
            }

            Assert.AreEqual (500, iterations);
            Assert.AreEqual (-374750, sumVals, "Sum of values not correct");
        }


        [TestMethod]
        public void UnitRdx_GetFrom()
        {
            var btree = new RankedDictionary<int,int>();

            for (int i = 1; i <= 1000; ++i)
                btree.Add (i, -i);

            int firstKey = -1;
            int iterations = 0;
            foreach (var e in btree.GetFrom (501))
            {
                if (iterations == 0)
                    firstKey = e.Key;
                ++iterations;
            }

            Assert.AreEqual (501, firstKey);
            Assert.AreEqual (500, iterations);
        }

        [TestMethod]
        public void UnitRdx_GetFromMissingVal()
        {

            var btree = new RankedDictionary<int,int>();

            for (int i = 0; i < 1000; i += 2)
                btree.Add (i, -i);

            for (int i = 1; i < 999; i += 2)
            {
                bool isFirst = true;
                foreach (var x in btree.GetFrom (i))
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
        public void UnitRdx_GetFromPassedEnd()
        {
            var btree = new RankedDictionary<int,int>();

            for (int i = 0; i < 1000; ++i)
                btree.Add (i, -i);

            int iterations = 0;
            foreach (var x in btree.GetFrom (2000))
                ++iterations;

            Assert.AreEqual (0, iterations, "SkipUntilKey shouldn't find anything");
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRdx_ElementAt1_ArgumentOutOfRange()
        {
            var tree = new RankedDictionary<int,int>();
            KeyValuePair<int,int> pair = tree.ElementAt (-1);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRdx_ElementAt2_ArgumentOutOfRange()
        {
            var tree = new RankedDictionary<int,int>();
            KeyValuePair<int,int> pair = tree.ElementAt (0);
        }

        [TestMethod]
        public void UnitRdx_ElementAt()
        {
            var tree = new RankedDictionary<int,int>();
            tree.Capacity = 4;
            for (int ii = 0; ii <= 800; ii+=2)
                tree.Add (ii, ii+100);

            for (int ii = 0; ii <= 400; ii+=2)
            {
                KeyValuePair<int,int> pair = tree.ElementAt (ii);
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
        public void UnitRdx_IndexOfKey()
        {
            var tree = new RankedDictionary<int,int>();
            tree.Capacity = 5;
            for (int ii = 0; ii < 500; ii+=2)
                tree.Add (ii, ii+1000);

            for (int ii = 0; ii < 500; ii+=2)
            {
                int ix = tree.IndexOfKey (ii);
                Assert.AreEqual (ii/2, ix);
            }

            int iw = tree.IndexOfKey (-1);
            Assert.AreEqual (~0, iw);

            int iy = tree.IndexOfKey (500);
            Assert.AreEqual (~250, iy);
        }


        [TestMethod]
        public void UnitRdx_IndexOfValue()
        {
            var d1 = new RankedDictionary<int,int>();
            for (int ii = 0; ii < 500; ++ii)
                d1.Add (ii, ii+1000);

            var ix1 = d1.IndexOfValue (1400);
            Assert.AreEqual (400, ix1);

            var ix2 = d1.IndexOfValue (88888);
            Assert.AreEqual (-1, ix2);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRdx_RemoveAtA_ArgumentOutOfRange()
        {
            var d1 = new RankedDictionary<int,int>();
            d1.Add (42, 24);
            d1.RemoveAt (-1);
        }

        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void CrashRdx_RemoveAtB_ArgumentOutOfRange()
        {
            var d1 = new RankedDictionary<int,int>();
            d1.RemoveAt (0);
        }

        [TestMethod]
        public void UnitRdx_RemoveAt()
        {
            var d1 = new RankedDictionary<int,int>();
            for (int ii = 0; ii < 5000; ++ii)
                d1.Add (ii, -ii);

            for (int i2 = 4900; i2 >= 0; i2 -= 100)
                d1.RemoveAt (i2);

            for (int i2 = 0; i2 < 5000; ++i2)
                if (i2 % 100 == 0)
                    Assert.IsFalse (d1.ContainsKey (i2));
                else
                    Assert.IsTrue (d1.ContainsKey (i2));
        }


        [TestMethod]
        public void UnitRdx_TryGetValueIndex()
        {
            var tree = new RankedDictionary<int,int>();
            tree.Capacity = 5;
            for (int ii = 0; ii < 500; ii+=2)
                tree.Add (ii, ii+1000);

            for (int ii = 0; ii < 500; ii+=2)
            {
                bool isOk = tree.TryGetValueAndIndex (ii, out int v1, out int i1);

                Assert.IsTrue (isOk);
                Assert.AreEqual (ii/2, i1);
                Assert.AreEqual (ii+1000, v1);
            }

            bool isOkNot = tree.TryGetValueAndIndex (111, out int v2, out int i2);
            Assert.IsFalse (isOkNot);
        }

#endif
        #endregion
    }
}
