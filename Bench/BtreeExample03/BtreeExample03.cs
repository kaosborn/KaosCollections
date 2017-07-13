//
// Program: BtreeExample03.cs
// Purpose: Demonstrate LINQ usage and range query.
//

using System;
using System.Linq;
using System.Collections.Generic;
using Kaos.Collections;

namespace ExampleApp
{
    class BtreeExample03
    {
        static void Main()
        {
            var towns = new RankedDictionary<string,int>();

            // Load sample data.
            towns.Add ("Albany", 43600);
            towns.Add ("Bandon", 2960);
            towns.Add ("Corvallis", 54462);
            towns.Add ("Damascus", 10539);
            towns.Add ("Elkton", 195);
            towns.Add ("Florence", 8466);
            towns.Add ("Glide", 1795);
            towns.Add ("Jacksonville", 2235);
            towns.Add ("Lebanon", 13140);
            towns.Add ("Lookingglass", 855);
            towns.Add ("Medford", 75180);
            towns.Add ("Powers", 689);
            towns.Add ("Riddle", 1020);
            towns.Add ("Roseburg", 20480);
            towns.Add ("Scio", 710);
            towns.Add ("Talent", 6066);
            towns.Add ("Umatilla", 6906);
            towns.Add ("Winston", 5379);
            towns.Add ("Yamhill", 820);

            // Here's a typical LINQ-To-Objects operation.
            double avg = towns.Average (x => x.Value);
            Console.WriteLine ("Average population of all towns = {0:f0}", avg);

            // Lambda expression
            IEnumerable<KeyValuePair<string,int>> r1 = towns.Where (t => t.Key.CompareTo ("E") < 0);

            Console.WriteLine ("\nTowns A-D:");
            foreach (KeyValuePair<string,int> e in r1)
                Console.WriteLine (e.Key);

            // LINQ range: O(n)
            IEnumerable<KeyValuePair<string,int>> r2 = towns.SkipWhile (t => t.Key.CompareTo ("E") < 0).TakeWhile (t => t.Key.CompareTo ("J") < 0);

            Console.WriteLine ("\nTowns E-G:");
            foreach (KeyValuePair<string,int> e in r2)
                Console.WriteLine (e.Key);

            //
            // Use the BetweenKeys iterator to query range.
            // Unlike LINQ SkipWhile and TakeWhile, this will perform an optimized (partial scan) lookup.
            //

            // BtreeDictionary range operator: O(log n)
            IEnumerable<KeyValuePair<string,int>> r3 = towns.BetweenKeys ("K", "M");

            Console.WriteLine ("\nTowns K-L:");
            foreach (KeyValuePair<string,int> town in r3)
                Console.WriteLine (town.Key);

            // BtreeDictionary range operator without upper limit: O(log n)
            IEnumerable<KeyValuePair<string,int>> r4 = towns.SkipUntilKey ("M");

            Console.WriteLine ("\nTowns M-R:");
            foreach (KeyValuePair<string,int> town in r4)
                // This avoids the issue in the last example where a town named "M" would be included.
                if (town.Key.CompareTo ("S") >= 0)
                    break;
                else
                    Console.WriteLine (town.Key);

            // BtreeDictionary range operator without upper limit: O(log n)
            IEnumerable<KeyValuePair<string,int>> r5 = towns.SkipUntilKey ("T");

            Console.WriteLine ("\nTowns T-Z:");
            foreach (KeyValuePair<string,int> town in r5)
                Console.WriteLine (town.Key);
        }

        /* Output:

        Average population of all towns = 13447

        Towns A-D:
        Albany
        Bandon
        Corvallis
        Damascus

        Towns E-G:
        Elkton
        Florence
        Glide

        Towns K-L:
        Lebanon
        Lookingglass

        Towns M-R:
        Medford
        Powers
        Riddle
        Roseburg

        Towns T-Z:
        Talent
        Umatilla
        Winston
        Yamhill

         */
    }
}
