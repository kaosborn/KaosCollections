using System;
using Kaos.Collections;

namespace ExampleApp
{
    class RdExample02
    {
        static void Main()
        {
            var d2 = new RankedDictionary<int,int>()
            { [36] = 360, [12] = 120 };

            Console.WriteLine ("Keys:");
            foreach (var key in d2.Keys)
                Console.WriteLine (key);

            Console.WriteLine ("Values:");
            foreach (var val in d2.Values)
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
