//
// File: TestBtreeGenerics.cs
//

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kaos.Collections;

namespace CollectionsTest
{
    public partial class Test_Btree
    {
        [TestMethod]
        public void Test_Inheritance()
        {
            Setup();

            Assert.IsTrue (tree2 is System.Collections.Generic.IDictionary<string,int>);
            Assert.IsTrue (tree2 is System.Collections.Generic.ICollection<KeyValuePair<string,int>>);
            Assert.IsTrue (tree2 is System.Collections.Generic.IEnumerable<KeyValuePair<string,int>>);
            Assert.IsTrue (tree2 is System.Collections.IEnumerable);
            Assert.IsTrue (tree2 is System.Collections.IDictionary);
            Assert.IsTrue (tree2 is System.Collections.ICollection);

            Assert.IsTrue (tree2.Keys is System.Collections.Generic.ICollection<string>);
            Assert.IsTrue (tree2.Keys is System.Collections.Generic.IEnumerable<string>);
            Assert.IsTrue (tree2.Keys is System.Collections.IEnumerable);
            Assert.IsTrue (tree2.Keys is System.Collections.ICollection);

            Assert.IsTrue (tree2.Values is System.Collections.Generic.ICollection<int>);
            Assert.IsTrue (tree2.Values is System.Collections.Generic.IEnumerable<int>);
            Assert.IsTrue (tree2.Values is System.Collections.IEnumerable);
            Assert.IsTrue (tree2.Values is System.Collections.ICollection);

            Assert.IsTrue (tree2 is System.Collections.Generic.IReadOnlyDictionary<string, int>);
            Assert.IsTrue (tree2 is System.Collections.Generic.IReadOnlyCollection<KeyValuePair<string, int>>);
            Assert.IsTrue (tree2.Keys is System.Collections.Generic.IReadOnlyCollection<string>);
            Assert.IsTrue (tree2.Values is System.Collections.Generic.IReadOnlyCollection<int>);
        }


        [TestMethod]
        public void Test_Ctor0()
        {
            Setup();
            Assert.AreEqual (0, tree1.Count);
        }


        [TestMethod]
        public void Test_Ctor2A()
        {
#if TEST_SORTEDDICTIONARY
            var tree = new SortedDictionary<string, int> (StringComparer.OrdinalIgnoreCase);
#else
            var tree = new BtreeDictionary<string, int> (StringComparer.OrdinalIgnoreCase);
#endif

            tree.Add ("AAA", 0);
            tree.Add ("bbb", 1);
            tree.Add ("CCC", 2);
            tree.Add ("ddd", 3);

            int actualPosition = 0;
            foreach (KeyValuePair<string, int> pair in tree)
            {
                Assert.AreEqual (actualPosition, pair.Value);
                ++actualPosition;
            }

            Assert.AreEqual (4, actualPosition);
        }


        [TestMethod]
        public void Test_Ctor2B()
        {
#if TEST_SORTEDDICTIONARY
            var tree = new SortedDictionary<string, int> (StringComparer.Ordinal);
#else
            var tree = new BtreeDictionary<string, int> (StringComparer.Ordinal);
#endif
            tree.Add ("AAA", 0);
            tree.Add ("CCC", 1);
            tree.Add ("bbb", 2);
            tree.Add ("ddd", 3);

            int actualPosition = 0;
            foreach (KeyValuePair<string, int> pair in tree)
            {
                Assert.AreEqual (actualPosition, pair.Value);
                ++actualPosition;
            }

            Assert.AreEqual (4, actualPosition);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void Test_Ctor_ArgumentNullException()
        {
            IDictionary<int, int> listArg = null;
#if TEST_SORTEDDICTIONARY
            IDictionary<int, int> idx = new SortedDictionary<int, int> (listArg);
#else
            IDictionary<int, int> idx = new BtreeDictionary<int, int> (listArg);
#endif
        }


        [TestMethod]
        public void Test_Ctor_IDictionary()
        {
            Setup();

            IDictionary<string, int> sl = new SortedList<string,int>();
            sl.Add ("Gremlin", 1);
            sl.Add ("Pacer", 2);

#if TEST_SORTEDDICTIONARY
            var tree = new SortedDictionary<string, int> (sl);
#else
            var tree = new BtreeDictionary<string, int> (sl);
#endif

            Assert.AreEqual (1, tree["Gremlin"]);
            Assert.AreEqual (2, tree["Pacer"]);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void Test_Add_ArgumentNullException()
        {
            Setup();
            tree2.Add (null, 0);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void Test_Add_ArgumentException_Entry_Already_Exists()
        {
            Setup();
            tree2.Add ("foo", 1);
            tree2.Add ("foo", 2);
        }


        [TestMethod]
        public void Test_AddCount()
        {
            Setup();

            tree1.Add (12, 120);
            tree1.Add (18, 180);

            Assert.AreEqual (2, tree1.Count);
        }


        [TestMethod]
        public void Test_Clear()
        {
            Setup();

            tree1.Add (41, 410);
            Assert.AreEqual (1, tree1.Count);

            tree1.Clear();
            Assert.AreEqual (0, tree1.Count);

            int actualCount = 0;
            foreach (KeyValuePair<int, int> pair in tree1)
                ++actualCount;

            Assert.AreEqual (0, actualCount);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void Test_ContainsKey_ArgumentNullException()
        {
            Setup();

            tree2.Add ("gamma", 3);
            bool result = tree2.ContainsKey (null);
        }


        [TestMethod]
        public void Test_ContainsKey()
        {
            Setup();

            int key1 = 26;
            int key2 = 36;
            tree1.Add (key1, key1 * 10);

            Assert.IsTrue (tree1.ContainsKey (key1));
            Assert.IsFalse (tree1.ContainsKey (key2));
        }


        [TestMethod]
        public void Test_ContainsValue()
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
        [ExpectedException (typeof (ArgumentNullException))]
        public void Test_CopyTo_ArgumentNullException()
        {
            Setup();
            var target = new KeyValuePair<int, int>[keys.Length];
            tree1.CopyTo (null, -1);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentOutOfRangeException))]
        public void Test_CopyTo_ArgumentOutOfRangeException_1()
        {
            Setup();
            var target = new KeyValuePair<int, int>[keys.Length];
            tree1.CopyTo (target, -1);
        }


        // MS docs incorrectly state ArgumentOutOfRangeException for this case.
        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void Test_CopyTo_ArgumentException_1()
        {
            Setup();
            for (int key = 1; key < 10; ++key)
                tree1.Add (key, key + 1000);

            var target = new KeyValuePair<int, int>[10];
            tree1.CopyTo (target, 25);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void Test_CopyTo_ArgumentException_2()
        {
            Setup();
            for (int key = 1; key < 10; ++key)
                tree1.Add (key, key + 1000);

            var target = new System.Collections.Generic.KeyValuePair<int, int>[4];
            tree1.CopyTo (target, 2);
        }


        [TestMethod]
        public void Test_CopyTo()
        {
            Setup();
            int offset = 1;
            int size = 20;

            for (int i = 0; i < size; ++i)
                tree1.Add (i + 1000, i + 10000);

            var pairs = new KeyValuePair<int, int>[size + offset];

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
        public void Test_GetEnumerator()
        {
            Setup();

            for (int i = 0; i < keys.Length; ++i)
                tree1.Add (keys[i], keys[i] + 1000);

            int actualCount = 0;

            foreach (KeyValuePair<int, int> pair in tree1)
            {
                ++actualCount;
                Assert.AreEqual (pair.Key + 1000, pair.Value);
            }

            Assert.AreEqual (keys.Length, actualCount);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void Test_Remove_ArgumentNullException()
        {
            Setup();
            tree2.Add ("delta", 4);

            bool isRemoved = tree2.Remove ((string) null);
        }


        [TestMethod]
        public void Test_Remove()
        {
            Setup();

            foreach (int key in keys)
                tree1.Add (key, key + 1000);

            int c0 = tree1.Count;
            bool isRemoved1 = tree1.Remove (keys[3]);
            bool isRemoved2 = tree1.Remove (keys[3]);

            Assert.IsTrue (isRemoved1);
            Assert.IsFalse (isRemoved2);
            Assert.AreEqual (c0 - 1, tree1.Count);
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void Test_TryGetValue_ArgumentNullException()
        {
            Setup();

            int resultValue;
            tree2.TryGetValue (null, out resultValue);
        }


        [TestMethod]
        public void Test_TryGetValue_For_Unfound_Key_int()
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
        public void Test_TryGetValue_For_Unfound_Key_String()
        {
#if TEST_SORTEDDICTIONARY
            var sd = new SortedDictionary<string, int> (StringComparer.Ordinal);
#else
            var sd = new BtreeDictionary<string, int> (StringComparer.Ordinal);
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
        public void Test_TryGetValue()
        {
            Setup();

            foreach (int key in keys)
                tree1.Add (key, key + 1000);

            int resultValue;
            tree1.TryGetValue (keys[0], out resultValue);

            Assert.AreEqual (keys[0] + 1000, resultValue);

            tree1.TryGetValue (50, out resultValue);
            Assert.AreEqual (default (int), resultValue);
        }


        [TestMethod]
        public void Test_Obj_GetEnumerator()
        {
            Setup();
            var aa = (System.Collections.IEnumerable) tree1;
            var bb = aa.GetEnumerator();
        }

        // ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- -----

        [TestMethod]
        public void Test_Comparer()
        {
            Setup();

            IComparer<string> result = tree2.Comparer;

            Assert.AreEqual (Comparer<string>.Default, result);
        }


        [TestMethod]
        public void Test_Count()
        {
            Setup();

            for (int i = 0; i < keys.Length; ++i)
            {
                Assert.AreEqual (i, tree1.Count);
                tree1.Add (keys[i], keys[i] * 10);
            }
            Assert.AreEqual (keys.Length, tree1.Count);

            for (int i = 0; i < keys.Length; ++i)
            {
                tree1.Remove (keys[i]);
                Assert.AreEqual (keys.Length - i - 1, tree1.Count);
            }
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void Test_Item_ArgumentNullException_1()
        {
            Setup();
            tree2[null] = 42;
        }


        [TestMethod]
        [ExpectedException (typeof (ArgumentNullException))]
        public void Test_Item_ArgumentNullException_2()
        {
            Setup();
            int x = tree2[null];
        }


        [TestMethod]
        [ExpectedException (typeof (KeyNotFoundException))]
        public void Test_Item_KeyNotFoundException_1()
        {
            Setup();
            tree2.Add ("pi", 9);

            int x = tree2["omicron"];
        }


        [TestMethod]
        [ExpectedException (typeof (KeyNotFoundException))]
        public void Test_Item_KeyNotFoundException_2()
        {
            Setup();
            tree1.Add (23, 230);

            int val = tree1[9];
        }


        [TestMethod]
        public void Test_Item()
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


        // ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- -----


        [TestMethod]
        [ExpectedException (typeof (ArgumentException))]
        public void Test_ICollection_Add_Pair_ArgumentException_Entry_Already_Exists()
        {
            Setup();
            var p1 = new KeyValuePair<string, int> ("beta", 1);
            var p2 = new KeyValuePair<string, int> (null, 98);
            var p3 = new KeyValuePair<string, int> (null, 99);

            genCol2.Add (p1);

            // Adding a null key is allowed here!
            genCol2.Add (p2);

            int result2 = tree2.Count;
            Assert.AreEqual (2, result2);

            // Should bomb on the second null key.
            genCol2.Add (p3);
        }


        [TestMethod]
        public void Test_ICollection_Add_Pair()
        {
            Setup();
            var p1 = new KeyValuePair<int, int> (17, 170);
            genCol1.Add (p1);

            Assert.AreEqual (1, tree1.Count);
            Assert.IsTrue (tree1.ContainsKey (17));
        }


        [TestMethod]
        public void Test_ICollection_Contains_Pair()
        {
            Setup();
            KeyValuePair<string, int> pair0 = new KeyValuePair<string, int> (null, 0);
            KeyValuePair<string, int> pair1 = new KeyValuePair<string, int> ("alpha", 1);
            KeyValuePair<string, int> pair2 = new KeyValuePair<string, int> ("delta", 4);
            KeyValuePair<string, int> pair3 = new KeyValuePair<string, int> ("zed", 99);

            tree2.Add (pair1.Key, pair1.Value);

            Assert.IsTrue (genCol2.Contains (pair1));
            Assert.IsFalse (genCol2.Contains (pair2));
            Assert.IsFalse (genCol2.Contains (pair0));
            Assert.IsFalse (genCol2.Contains (pair3));
        }


        [TestMethod]
        public void Test_ICollection_GetEnumerator()
        {
            Setup();
            foreach (int k in keys)
                tree1.Add (k, k + 100);

            int actualCount = 0;
            foreach (KeyValuePair<int, int> pair in genCol1)
            {
                Assert.AreEqual (pair.Key + 100, pair.Value);
                ++actualCount;
            }

            Assert.AreEqual (keys.Length, actualCount);
        }


        [TestMethod]
        public void Test_IEnumerablePair_GetEnumerator()
        {
            Setup();
            foreach (int k in keys)
                tree1.Add (k, k + 100);

            var x = (IEnumerable<KeyValuePair<int, int>>) tree1;

            int actualCount = 0;
            foreach (KeyValuePair<int, int> pair in x)
            {
                Assert.AreEqual (pair.Key + 100, pair.Value);
                ++actualCount;
            }

            Assert.AreEqual (keys.Length, actualCount);
        }


        [TestMethod]
        public void Test_ICollection_IsReadonly()
        {
            Setup();
            Assert.IsFalse (genCol1.IsReadOnly);
            Assert.IsFalse (genCol2.IsReadOnly);
        }


        [TestMethod]
        public void Test_IDictionary_Keys()
        {
            Setup();
            tree2.Add ("alpha", 1);
            tree2.Add ("beta", 2);
            var genId = (IDictionary<string, int>) tree2;
            int count = genId.Keys.Count;
            Assert.AreEqual (2, count);
        }


        [TestMethod]
        public void Test_ICollection_Remove_Pair()
        {
            Setup();

            KeyValuePair<string, int> pair0 = new KeyValuePair<string, int> (null, 0);
            KeyValuePair<string, int> pair1 = new KeyValuePair<string, int> ("ten", 10);
            KeyValuePair<string, int> pair2 = new KeyValuePair<string, int> ("ten", 100);
            KeyValuePair<string, int> pair3 = new KeyValuePair<string, int> ("twenty", 20);

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

            // Nothing removed here because value doesn't match.
            Assert.IsFalse (isRemoved);
            isRemoved = genCol2.Remove (pair2);
            Assert.AreEqual (2, tree2.Count);

            isRemoved = genCol2.Remove (pair1);
            Assert.IsTrue (isRemoved);
            Assert.AreEqual (1, tree2.Count);
        }


        [TestMethod]
        public void Test_IDictionary_Values()
        {
            Setup();
            tree2.Add ("alpha", 1);
            tree2.Add ("beta", 2);
            var genId = (IDictionary<string, int>) tree2;
            int count = genId.Values.Count;
            Assert.AreEqual (2, count);
        }

    }
}
