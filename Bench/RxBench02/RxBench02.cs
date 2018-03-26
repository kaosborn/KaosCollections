//
// Program: RxBench02.cs
// Purpose:
// • Exercise library source compiled into the same assembly as the executable.
// • Demonstrate BCL curiosities.
//

using System;
using System.Collections.Generic;
using Kaos.Collections;
[assembly: System.Reflection.AssemblyVersion ("0.1.0.0")]

namespace BenchApp
{
    class RxBench02
    {
        static void Main()
        {
            // Here is an example of why 'private protected' is better than 'internal'.
            // Kaos.Collections v4.1 and earlier it was internal and would have compiled for RxBench02.
            // After v4.1, as private protected it is not accessible.

            // Commented out assignment no longer compiles for single-assembly projects:
            var rs = new RankedSet<int>();
         // var kc = rs.maxKeyCount;

            // Here is a curiosity where the BCL allows creation of an enumerator without a target class.
            // This default constructor is not emulated by Kaos.Collections.
            var strangeEnumerator = new SortedSet<int>.Enumerator();

            // Another curiosity: BCL enumerators remain active after disposing.
            // The KaosCollections library emulates this seemingly wrong behavior.

            var ss = new SortedSet<int> { 3,4,5 };
            SortedSet<int>.Enumerator ssEtor = ss.GetEnumerator();
            bool ssNext1 = ssEtor.MoveNext();
            ssEtor.Dispose();
            bool ssNext2 = ssEtor.MoveNext();

            Console.WriteLine ("Dispose() then MoveNext() = " + ssNext2);
            Console.WriteLine ("Dispose() then Current = " + ssEtor.Current);
        }

        /* Output:

        Dispose() then MoveNext() = True
        Dispose() then Current = 4

        */
    }
}
