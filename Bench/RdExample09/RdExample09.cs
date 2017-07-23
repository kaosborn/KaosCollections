//
// Program: RdExample09.cs
// Purpose: Demonstrate FxCore usage.
//
// Usage:
// • To execute from a command prompt run:
//     dotnet BtreeExample09.dll

using System;
using Kaos.Collections;

namespace ExampleApp
{
    class RdExample09
    {
        static void Main()
        {
            var sd = new RankedDictionary<int,int> { { 3, 33 } };
            sd.Add (1,11);
            sd.Add (9,99);
            sd.Add (5,55);
            foreach (var kv in sd)
                Console.WriteLine (kv.Value);
        }
    }
}
