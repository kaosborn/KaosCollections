using System;
using Kaos.Collections;

namespace ExampleApp
{
    class RsExample03
    {
        static string Text<T> (System.Collections.Generic.IEnumerable<T> items)
            => "{ " + String.Join (" ", items) + " }";

        static void Main()
        {
            var set1 = new RankedSet<int> { 3, 5, 7 };
            var set2 = new RankedSet<int> { 5, 7 };
            var set3 = new RankedSet<int> { 1, 9 };
            var set4 = new RankedSet<int> { 5, 9 };
            var arg5 = new int[] { 5, 9, 9 };

            bool isSub1 = set2.IsSubsetOf (set1);
            bool isSub2 = set2.IsSubsetOf (set2);
            bool isSub3 = set4.IsSubsetOf (set2);
            bool isSup1 = set1.IsSupersetOf (set2);
            bool isSup2 = set2.IsSupersetOf (set2);
            bool isSup3 = set2.IsSupersetOf (set4);
            bool isPSub1 = set2.IsProperSubsetOf (set1);
            bool isPSub2 = set2.IsProperSubsetOf (set2);
            bool isPSup1 = set1.IsProperSupersetOf (set2);
            bool isPSup2 = set2.IsProperSupersetOf (set2);
            bool isOlap1 = set1.Overlaps (set4);
            bool isOlap2 = set1.Overlaps (set3);
            bool isEq1 = set4.SetEquals (set4);
            bool isEq2 = set4.SetEquals (set3);
            bool isEq3 = set4.SetEquals (arg5);

            Console.WriteLine ($"{Text(set2)} IsSubsetOf {Text(set1)} = {isSub1}");
            Console.WriteLine ($"{Text(set2)} IsSubsetOf {Text(set2)} = {isSub2}");
            Console.WriteLine ($"{Text(set4)} IsSubsetOf {Text(set2)} = {isSub3}");
            Console.WriteLine ();

            Console.WriteLine ($"{Text(set1)} IsSupersetOf {Text(set2)} = {isSup1}");
            Console.WriteLine ($"{Text(set2)} IsSupersetOf {Text(set2)} = {isSup2}");
            Console.WriteLine ($"{Text(set2)} IsSupersetOf {Text(set4)} = {isSup3}");
            Console.WriteLine ();

            Console.WriteLine ($"{Text(set2)} IsProperSubsetOf {Text(set1)} = {isPSub1}");
            Console.WriteLine ($"{Text(set2)} IsProperSubsetOf {Text(set2)} = {isPSub2}");
            Console.WriteLine ();

            Console.WriteLine ($"{Text(set1)} IsProperSupersetOf {Text(set2)} = {isPSup1}");
            Console.WriteLine ($"{Text(set2)} IsProperSupersetOf {Text(set2)} = {isPSup2}");
            Console.WriteLine ();

            Console.WriteLine ($"{Text(set1)} Overlaps {Text(set4)} = {isOlap1}");
            Console.WriteLine ($"{Text(set1)} Overlaps {Text(set3)} = {isOlap2}");
            Console.WriteLine ();

            Console.WriteLine ($"{Text(set4)} SetEquals {Text(set4)} = {isEq1}");
            Console.WriteLine ($"{Text(set4)} SetEquals {Text(set3)} = {isEq2}");
            Console.WriteLine ($"{Text(set4)} SetEquals {Text(arg5)} = {isEq3}");
            Console.WriteLine ();
        }

        /* Output:

        { 5 7 } IsSubsetOf { 3 5 7 } = True
        { 5 7 } IsSubsetOf { 5 7 } = True
        { 5 9 } IsSubsetOf { 5 7 } = False

        { 3 5 7 } IsSupersetOf { 5 7 } = True
        { 5 7 } IsSupersetOf { 5 7 } = True
        { 5 7 } IsSupersetOf { 5 9 } = False

        { 5 7 } IsProperSubsetOf { 3 5 7 } = True
        { 5 7 } IsProperSubsetOf { 5 7 } = False

        { 3 5 7 } IsProperSupersetOf { 5 7 } = True
        { 5 7 } IsProperSupersetOf { 5 7 } = False

        { 3 5 7 } Overlaps { 5 9 } = True
        { 3 5 7 } Overlaps { 1 9 } = False

        { 5 9 } SetEquals { 5 9 } = True
        { 5 9 } SetEquals { 1 9 } = False
        { 5 9 } SetEquals { 5 9 9 } = True

        */
    }
}
