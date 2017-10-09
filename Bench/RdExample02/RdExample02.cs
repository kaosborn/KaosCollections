using System;
using Kaos.Collections;

namespace ExampleApp
{
    class RdExample02
    {
        static void Main()
        {
            var dary = new RankedDictionary<int,int>()
            { [36] = 360, [12] = 120 };

            Console.WriteLine ("Keys:");
            foreach (var key in dary.Keys)
                Console.WriteLine (key);

            Console.WriteLine ("\nValues:");
            foreach (var val in dary.Values)
                Console.WriteLine (val);
        }

        /* Output:

        Keys:
        12
        36

        Values:
        120
        360

        */
    }
}
