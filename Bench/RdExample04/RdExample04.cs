//
// Program: RdExample04.cs
// Purpose: Exercise BtreeDictionary with a supplied comparer.
//

using System;
using System.Collections.Generic;
using Kaos.Collections;

namespace ExampleApp
{
    class RdExample04
    {
        static void Main()
        {
            var tree1 = new RankedDictionary<string,int> (StringComparer.InvariantCultureIgnoreCase);
            tree1.Add ("AAA", 0);
            tree1.Add ("bbb", 1);
            tree1.Add ("CCC", 2);
            tree1.Add ("ddd", 3);

            Console.WriteLine ("Case insensitive:");
            foreach (KeyValuePair<string,int> pair in tree1)
                Console.WriteLine (pair.Key);
            Console.WriteLine();

            var tree2 = new RankedDictionary<string,int> (StringComparer.Ordinal);
            tree2.Add ("AAA", 0);
            tree2.Add ("bbb", 2);
            tree2.Add ("CCC", 1);
            tree2.Add ("ddd", 3);

            Console.WriteLine ("Case sensitive:");
            foreach (KeyValuePair<string,int> pair in tree2)
                Console.WriteLine (pair.Key);
        }
    }
}
