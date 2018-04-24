//
// Program: RxBench01.cs
// Purpose: Demonstrate FxCore usage of all classes.
//
// Usage:
// • To execute from a command prompt run:
//     dotnet RxBench01.dll

using System;
using Kaos.Collections;

namespace BenchApp
{
    class RxBench01
    {
        static void Main()
        {
            var rs = new RankedSet<int> { 3,5,7 };
            Console.WriteLine ("Set items must be distinct:");
            foreach (var item in rs)
                Console.WriteLine (item);

            var rd = new RankedDictionary<int,int> { {3,0}, {1,11}, {5,0}, {9,99} };
            Console.WriteLine ("\nDictionary keys must be distinct:");
            foreach (var kv in rd)
                Console.WriteLine (kv);
            Console.WriteLine ("Just the keys: { " + String.Join (",", rd.Keys) + " }");
            Console.WriteLine ("Just the values: { " + String.Join (",", rd.Values) + " }");

            var rb = new RankedBag<int> { 1,2,2,3 };
            Console.WriteLine ("\nBag items may repeat:");
            foreach (var bx in rb)
                Console.WriteLine (bx);

            var rm = new RankedMap<int,int> { {2,0}, {4,0}, {4,44}, {6,66} };
            Console.WriteLine ("\nMap keys may repeat:");
            foreach (var kv in rm)
                Console.WriteLine (kv);
            Console.WriteLine ("Just the keys: { " + String.Join (",", rm.Keys) + " }");
            Console.WriteLine ("Just the values: { " + String.Join (",", rm.Values) + " }");
        }

        /* Output:

        Set items must be distinct:
        3
        5
        7

        Dictionary keys must be distinct:
        [1, 11]
        [3, 0]
        [5, 0]
        [9, 99]
        Just the keys: { 1,3,5,9 }
        Just the values: { 11,0,0,99 }

        Bag items may repeat:
        1
        2
        2
        3

        Map keys may repeat:
        [2, 0]
        [4, 0]
        [4, 44]
        [6, 66]
        Just the keys: { 2,4,4,6 }
        Just the values: { 0,0,44,66 }

        */
    }
}
