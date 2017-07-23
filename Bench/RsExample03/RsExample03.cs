// Program: RsExample03.cs

using System;
using Kaos.Collections;

namespace ExampleApp
{
    class RsExample03
    {
        static string Text (RankedSet<int> set)
        { return "{ " + String.Join (" ", set) + " }"; }

        static void Main()
        {
            var set1 = new RankedSet<int>( new int[] { 3, 5, 7 });
            var set2 = new RankedSet<int>( new int[] { 5, 7, 9 });
            var set3 = new RankedSet<int>( new int[] { 3, 5, 9 });
            var set4 = new RankedSet<int>( new int[] { 5, 7 });
            var set5 = new RankedSet<int>( new int[] { 1, 9 });

            var ew = new RankedSet<int> (set1);
            var iw = new RankedSet<int> (set1);
            var sew = new RankedSet<int> (set1);
            var uw = new RankedSet<int> (set1);

            bool isSub1 = set4.IsSubsetOf (set1);
            bool isSub2 = set4.IsSubsetOf (set4);
            bool isSub3 = set4.IsSubsetOf (set3);
            bool isSup1 = set1.IsSupersetOf (set4);
            bool isSup2 = set4.IsSupersetOf (set4);
            bool isSup3 = set3.IsSupersetOf (set4);
            bool isPSub1 = set4.IsProperSubsetOf (set1);
            bool isPSub2 = set4.IsProperSubsetOf (set4);
            bool isPSup1 = set1.IsProperSupersetOf (set4);
            bool isPSup2 = set4.IsProperSupersetOf (set4);
            bool isOlap1 = set1.Overlaps (set2);
            bool isOlap2 = set1.Overlaps (set5);

            ew.ExceptWith (set2);
            iw.IntersectWith (set2);
            sew.SymmetricExceptWith (set2);
            uw.UnionWith (set2);

            Console.WriteLine (Text(set4) + " IsSubsetOf " + Text(set1) + " = " + isSub1);
            Console.WriteLine (Text(set4) + " IsSubsetOf " + Text(set4) + " = " + isSub2);
            Console.WriteLine (Text(set4) + " IsSubsetOf " + Text(set3) + " = " + isSub3);
            Console.WriteLine ();

            Console.WriteLine (Text(set1) + " IsSupersetOf " + Text(set4) + " = " + isSup1);
            Console.WriteLine (Text(set4) + " IsSupersetOf " + Text(set4) + " = " + isSup2);
            Console.WriteLine (Text(set3) + " IsSupersetOf " + Text(set4) + " = " + isSup3);
            Console.WriteLine ();

            Console.WriteLine (Text(set4) + " IsProperSubsetOf " + Text(set1) + " = " + isPSub1);
            Console.WriteLine (Text(set4) + " IsProperSubsetOf " + Text(set4) + " = " + isPSub2);
            Console.WriteLine ();

            Console.WriteLine (Text(set1) + " IsProperSupersetOf " + Text(set4) + " = " + isPSup1);
            Console.WriteLine (Text(set4) + " IsProperSupersetOf " + Text(set4) + " = " + isPSup2);
            Console.WriteLine ();

            Console.WriteLine (Text(set1) + " Overlaps " + Text(set2) + " = " + isOlap1);
            Console.WriteLine (Text(set1) + " Overlaps " + Text(set5) + " = " + isOlap2);
            Console.WriteLine ();

            Console.WriteLine (Text(set1) + " ExceptWith " + Text(set2) + " = " + Text(ew));
            Console.WriteLine ();
            Console.WriteLine (Text(set1) + " IntersectWith " + Text(set2) + " = " + Text(iw));
            Console.WriteLine ();
            Console.WriteLine (Text(set1) + " SymmetricExceptWith " + Text(set2) + " = " + Text(sew));
            Console.WriteLine ();
            Console.WriteLine (Text(set1) + " UnionWith " + Text(set3) + " = " + Text(uw));
        }
    }
}
