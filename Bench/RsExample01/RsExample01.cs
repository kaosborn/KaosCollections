using System;
using Kaos.Collections;

namespace ChartApp
{
    class RsExample01
    {
        static bool IsPolynymous (string name) => name.Contains (" ");

        static void Main()
        {
            var musicians = new RankedSet<string> (StringComparer.InvariantCultureIgnoreCase);

            foreach (var m1 in new string[] { "Falco", "k.d. lang", "Madonna", "Tom Petty",
                                              "Joni Mitchell", "Grimes", "Warren Zevon" })
                musicians.Add (m1);

            Console.WriteLine ("Candidates:");
            foreach (var item in musicians)
                Console.WriteLine ("  " + item);

            musicians.Remove ("Falco");
            musicians.RemoveWhere (IsPolynymous);
            musicians.RemoveRange (1, musicians.Count-1);

            Console.WriteLine ("\nFavorite:");
            foreach (var item in musicians)
                Console.WriteLine ("  " + item);
        }

        /* Output:

        Candidates:
          Falco
          Grimes
          Joni Mitchell
          k.d. lang
          Madonna
          Tom Petty
          Warren Zevon

        Favorite:
          Grimes

        */
    }
}
