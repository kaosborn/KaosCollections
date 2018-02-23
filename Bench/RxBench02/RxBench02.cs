//
// Program: RxBench02.cs
// Purpose: Shows library source compiled into the same assembly as the executable.
//

using System.Collections.Generic;
using Kaos.Collections;
[assembly: System.Reflection.AssemblyVersion ("0.1.0.0")]

namespace BenchApp
{
    class RxBench02
    {
        static void Main()
        {
            var rs = new RankedSet<int>();

            // Here is an example of why 'private protected' is better than 'internal'.
            // Kaos.Collections v4.1 and earlier it was internal and would have compiled for RxBench02.
            // After v4.1, as private protected it is not accessible.

            // No longer compiles for single-assembly projects:
            // var kc = rs.maxKeyCount;

            // Asides:

            // Here is a curiosity where the BCL allows creation of an enumerator without a target class.
            // This default constructor is not emulated by Kaos.Collections.
            var strangeEnumerator = new SortedSet<int>.Enumerator();
        }
    }
}
