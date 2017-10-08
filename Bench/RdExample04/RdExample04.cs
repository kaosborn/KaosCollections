using System;
using Kaos.Collections;

namespace ExampleApp
{
    class RdExample04
    {
        static void Main()
        {
            var dary1 = new RankedDictionary<string,int> (StringComparer.InvariantCultureIgnoreCase);
            dary1.Add ("AAA", 0);
            dary1.Add ("bbb", 1);
            dary1.Add ("CCC", 2);
            dary1.Add ("ddd", 3);

            Console.WriteLine ("Comparer is case insensitive:");
            foreach (System.Collections.Generic.KeyValuePair<string,int> pair in dary1)
                Console.WriteLine (pair.Key);
            Console.WriteLine();

            var dary2 = new RankedDictionary<string,int> (StringComparer.Ordinal);
            dary2.Add ("AAA", 0);
            dary2.Add ("bbb", 2);
            dary2.Add ("CCC", 1);
            dary2.Add ("ddd", 3);

            Console.WriteLine ("Comparer is case sensitive:");
            foreach (System.Collections.Generic.KeyValuePair<string,int> pair in dary2)
                Console.WriteLine (pair.Key);
        }

        /* Output:

        Comparer is case insensitive:
        AAA
        bbb
        CCC
        ddd

        Comparer is case sensitive:
        AAA
        CCC
        bbb
        ddd

        */
    }
}
