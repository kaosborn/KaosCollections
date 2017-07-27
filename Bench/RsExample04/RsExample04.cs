using System;
using Kaos.Collections;

namespace ExampleApp
{
    class RsExample04
    {
        static string Text<T> (System.Collections.Generic.IEnumerable<T> data)
        { return "{ " + String.Join (" ", data) + " }"; }

        static void Main()
        {
            var set = new RankedSet<int>(new int[] { 3, 5, 7 });
            var arg = new int[] { 5, 7, 9 };

            var ew = new RankedSet<int> (set);
            var iw = new RankedSet<int> (set);
            var se = new RankedSet<int> (set);
            var uw = new RankedSet<int> (set);

            ew.ExceptWith (arg);
            iw.IntersectWith (arg);
            se.SymmetricExceptWith (arg);
            uw.UnionWith (arg);

            Console.WriteLine (Text(set) + " ExceptWith " + Text(arg) + " = " + Text(ew));
            Console.WriteLine (Text(set) + " IntersectWith " + Text(arg) + " = " + Text(iw));
            Console.WriteLine (Text(set) + " SymmetricExceptWith " + Text(arg) + " = " + Text(se));
            Console.WriteLine (Text(set) + " UnionWith " + Text(arg) + " = " + Text(uw));
        }

        /* Output:

        { 3 5 7 } ExceptWith { 5 7 9 } = { 3 }
        { 3 5 7 } IntersectWith { 5 7 9 } = { 5 7 }
        { 3 5 7 } SymmetricExceptWith { 5 7 9 } = { 3 9 }
        { 3 5 7 } UnionWith { 5 7 9 } = { 3 5 7 9 }

        */
    }
}
