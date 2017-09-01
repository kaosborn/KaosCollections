using System;
using Kaos.Collections;

namespace ExampleApps
{
    class RbExample01
    {
        static void Main()
        {
            var crayons = new RankedBag<string>() { "red", "blue", "yellow", "black", "black" };

            Console.WriteLine ("There are " + crayons.Count + " total crayon colors:");
            foreach (var crayon in crayons)
                Console.WriteLine ("  " + crayon);

            Console.WriteLine ("\nThere are " + crayons.GetDistinctCount() + " distinct colors:");
            foreach (var crayon in crayons.Distinct())
                Console.WriteLine ("  " + crayon);

            if (! crayons.Contains ("thistle"))
                Console.WriteLine ("\nDoes not contain 'thistle'.");

            crayons.RetainAll (new string[] { "white", "grey", "black" });
            crayons.Add ("silver");

            Console.WriteLine ("\nAfter RetainAll+Add: ");
            foreach (var crayon in crayons)
                Console.WriteLine ("  " + crayon);
        }

        /* Output:

        There are 5 total crayon colors:
          black
          black
          blue
          red
          yellow

        There are 4 distinct colors:
          black
          blue
          red
          yellow

        Does not contain 'thistle'.

        After RetainAll+Add:
          black
          silver

         */
    }
}
