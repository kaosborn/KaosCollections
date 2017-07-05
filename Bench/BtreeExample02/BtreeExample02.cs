//
// Program: BtreeExample02.cs
// Purpose: Trivial example of iterating over subcollections Keys, Values.
//

using System;
using System.Collections.Generic;
using Kaos.Collections;

namespace ExampleApp
{
    class BtreeExample02
    {
        static void Main()
        {
            var tree = new BtreeDictionary<int,int>();

            tree.Add (36, 360);
            tree.Add (12, 120);

            ICollection<int> keyList = tree.Keys;
            ICollection<int> valList = tree.Values;

            Console.WriteLine ("Keys:");
            foreach (int n in keyList)
                Console.WriteLine (n);

            Console.WriteLine ("Values:");
            foreach (int v in tree.Values)
                Console.WriteLine (v);
        }
    }
}
