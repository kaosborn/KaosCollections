//
// File: TestBtree.cs
//
// Run BtreeDictionary's suite against SortedDictionary to demonstrate identical behavior
// between the two API's.  To perform baseline test against SortedDictionary, add the
// TEST_SORTEDDICTIONARY compilation symbol.
//

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#if ! TEST_SORTEDDICTIONARY
using Kaos.Collections;
#endif

namespace CollectionsTest
{
    [TestClass]
    public partial class Test_Btree
    {
#if TEST_SORTEDDICTIONARY
        SortedDictionary<int,int> tree1;
        SortedDictionary<string,int> tree2;
        SortedDictionary<string,int?> tree3;
        SortedDictionary<int,string> tree4;
#else
        RankedDictionary<int,int> tree1;
        RankedDictionary<string,int> tree2;
        RankedDictionary<string,int?> tree3;
        RankedDictionary<int,string> tree4;
#endif
        ICollection<KeyValuePair<string,int>> genCol2;
        ICollection<string> genKeys2;
        ICollection<int> genValues2;

        System.Collections.IDictionary objCol1, objCol2, objCol3, objCol4;


        // Must not contain value 50.
        static int[] keys = new int[] { 12, 28, 15, 18, 14, 19, 25 };

        public void Setup() { Setup (5); }

        public void Setup (int order)
        {
#if TEST_SORTEDDICTIONARY
            tree1 = new SortedDictionary<int,int>();
            tree2 = new SortedDictionary<string,int>();
            tree3 = new SortedDictionary<string,int?>();
            tree4 = new SortedDictionary<int,string>();
#else
            tree1 = new RankedDictionary<int,int> (order);
            tree2 = new RankedDictionary<string,int> (order);
            tree3 = new RankedDictionary<string,int?> (order);
            tree4 = new RankedDictionary<int,string> (order);
#endif

            Type treeType = tree1.GetType();

            // For testing explicit implementations.
            genCol2 = (ICollection<KeyValuePair<string,int>>) tree2;
            genKeys2 = (ICollection<string>) tree2.Keys;
            genValues2 = (ICollection<int>) tree2.Values;
            objCol1 = (System.Collections.IDictionary) tree1;
            objCol2 = (System.Collections.IDictionary) tree2;
            objCol3 = (System.Collections.IDictionary) tree3;
            objCol4 = (System.Collections.IDictionary) tree4;
        }
    }
}
