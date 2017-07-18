//
// File: TestInit.cs
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
    public class TS0
    {
        public int K0 { get; set; }
        public TS0 (int k0) { this.K0 = k0; }
    }

    public class TS1 : IComparable<TS1>, IComparable
    {
        public int K1 { get; private set; }
        public TS1 (int k1) { this.K1 = k1; }

        public int CompareTo (TS1 other) { return this.K1 - other.K1; }
        public int CompareTo (object ob) { return this.K1 - ((TS1)ob).K1; }
    }


    [TestClass]
    public partial class Test_Btree
    {
#if TEST_SORTEDDICTIONARY
        SortedDictionary<int,int> tree1;
        SortedDictionary<string,int> tree2;
        SortedDictionary<string,int?> tree3;
        SortedDictionary<int,string> tree4;
        SortedSet<int> setI;
        SortedSet<string> setS;
        SortedSet<TS1> setTS1;
#else
        RankedDictionary<int,int> tree1;
        RankedDictionary<string,int> tree2;
        RankedDictionary<string,int?> tree3;
        RankedDictionary<int,string> tree4;
        RankedSet<int> setI;
        RankedSet<string> setS;
        RankedSet<TS1> setTS1;
#endif
        ICollection<KeyValuePair<string,int>> genCol2;
        ICollection<string> genKeys2;
        ICollection<int> genValues2;

        System.Collections.IDictionary objCol1, objCol2, objCol3, objCol4;


        // Must not contain value 50.
        static int[] keys = new int[] { 12, 28, 15, 18, 14, 19, 25 };
        static int[] iVals2 = new int[] { 10, 28, 14, 50 };
        static int[] iVals3 = new int[] { 13, 22, 51, 22, 33 };
        static int[] iVals4 = new int[] { 14, 15, 19 };

        public void Setup() { Setup (5); }

        public void Setup (int order)
        {
#if TEST_SORTEDDICTIONARY
            tree1 = new SortedDictionary<int,int>();
            tree2 = new SortedDictionary<string,int>();
            tree3 = new SortedDictionary<string,int?>();
            tree4 = new SortedDictionary<int,string>();
            setI = new SortedSet<int>();
            setS = new SortedSet<string>();
            setTS1 = new SortedSet<TS1>();
#else
            tree1 = new RankedDictionary<int,int> (order);
            tree2 = new RankedDictionary<string,int> (order);
            tree3 = new RankedDictionary<string,int?> (order);
            tree4 = new RankedDictionary<int,string> (order);
            setI = new RankedSet<int>(order);
            setS = new RankedSet<string>(order);
            setTS1 = new RankedSet<TS1>(order);
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
