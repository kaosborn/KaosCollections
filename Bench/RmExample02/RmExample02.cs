using System;
using Kaos.Collections;

namespace ExampleApp
{
    class RmExample02
    {
        static void Main()
        {
            var map = new RankedMap<char,int>();
            var input = "this is it";

            for (int pos = 0; pos < input.Length; ++pos)
                if (! Char.IsWhiteSpace (input[pos]))
                    map.Add (input[pos], pos);

            foreach (var kv in map)
                Console.WriteLine (kv);
        }

        /* Output:

        [h, 1]
        [i, 2]
        [i, 5]
        [i, 8]
        [s, 3]
        [s, 6]
        [t, 0]
        [t, 9]

        */
    }
}
