using System;
using Kaos.Collections;

namespace ExampleApp
{
    class RbExample01
    {
        static void Main()
        {
            var crayons = new RankedBag<string> (StringComparer.InvariantCultureIgnoreCase)
            { "red", "yellow", "black", "BLACK" };

            crayons.Add ("blue");

            Console.WriteLine ($"There are {crayons.Count} total crayons:");
            foreach (var crayon in crayons)
                Console.WriteLine ($"  {crayon}");

            Console.WriteLine ($"\nThere are {crayons.GetDistinctCount()} distinct colors:");
            foreach (var crayon in crayons.Distinct())
                Console.WriteLine ($"  {crayon}");

            Console.WriteLine ($"\nGot 'gold' crayon? {crayons.Contains ("gold")}");

            // RetainAll respects cardinality so the oldest 'black' is removed:
            crayons.RetainAll (new string[] { "white", "grey", "Black", "red" });

            Console.WriteLine ("\nAfter RetainAll: ");
            foreach (var crayon in crayons)
                Console.WriteLine ($"  {crayon}");
        }

        /* Output:

        There are 5 total crayons:
          black
          BLACK
          blue
          red
          yellow

        There are 4 distinct colors:
          black
          blue
          red
          yellow

        Got 'gold' crayon? False

        After RetainAll:
          BLACK
          red

        */
    }
}
