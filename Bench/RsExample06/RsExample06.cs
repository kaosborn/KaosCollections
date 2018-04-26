using System;
using System.Collections.Generic;
using Kaos.Collections;

namespace ExampleApp
{
    class RsExample06
    {
        static bool IsMononymous (string name) => ! name.Contains (" ");

        static void Main()
        {
            var names1 = new string[] { "Falco", "Nico", "David Bowie", "Tom Petty", "Joni Mitchell", "Warren Zevon" };
            var names2 = new string[] { "Michelangelo", "Rembrandt", "Joni Mitchell", "David Bowie" };

            var musicians = new RankedSet<string> (names1);

            // Remove mononymous items.
            Console.WriteLine ("Remove single names from the set...");
            Console.WriteLine ($"  Count before: {musicians.Count}");
            musicians.RemoveWhere (IsMononymous);
            Console.WriteLine ($"  Count after: {musicians.Count}\n");

            // List names starting with 'J'.
            Console.WriteLine ("Musicians J-T");
            foreach (var name in musicians.ElementsBetween ("J", "U"))
                Console.WriteLine ($"  {name}");

            // Create another RankedSet.
            var painters = new RankedSet<string> (names2);

            // Remove elements in musicians that are also in painters.
            Console.WriteLine ("\nRemove duplicates (of painters) from the musicians...");
            Console.WriteLine ($"  Count before: {musicians.Count}");
            musicians.ExceptWith (painters);
            Console.WriteLine ($"  Count after: {musicians.Count}\n");

            Console.WriteLine ("List of musicians that are not painters:");
            foreach (string name in musicians)
                Console.WriteLine ($"  {name}");

            var comp = RankedSet<string>.CreateSetComparer();

            HashSet<RankedSet<string>> setOfSets = new HashSet<RankedSet<string>> (comp);
            setOfSets.Add (musicians);
            setOfSets.Add (painters);

            Console.WriteLine ("\nAll sets in hash set:");
            foreach (var set in setOfSets)
            {
                Console.WriteLine ($"  {set.Count} items:");
                foreach (var item in set)
                    Console.WriteLine ($"    {item}");
            }

            // Create a 3rd RankedSet.
            var people = new RankedSet<string> { "Tom Petty", "Warren Zevon" };

            // Create a set equality comparer.
            var comparer = RankedSet<string>.CreateSetComparer();

            Console.WriteLine ($"\nSet comparison 1: {comparer.Equals (musicians, people)}");
            Console.WriteLine ($"Set comparison 2: {comparer.Equals (painters, people)}");
        }

        /* Output:

        Remove single names from the set...
          Count before: 6
          Count after: 4

        Musicians J-T
          Joni Mitchell
          Tom Petty

        Remove duplicates (of painters) from the musicians...
          Count before: 4
          Count after: 2

        List of musicians that are not painters:
          Tom Petty
          Warren Zevon

        All sets in bag:
          2 items:
            Tom Petty
            Warren Zevon
          4 items:
            David Bowie
            Joni Mitchell
            Michelangelo
            Rembrandt

        Set comparison 1: True
        Set comparison 2: False

        */
    }
}
