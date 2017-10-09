using System;
using Kaos.Collections;

namespace ExampleApp
{
    public class CharMap : IComparable<CharMap>
    {
        public char Ch { get; private set; }
        public int Pos { get; private set; }
        public CharMap (char ch, int pos) { Ch = ch; Pos = pos; }
        public int CompareTo (CharMap other) => Ch.CompareTo (other.Ch);
        public override string ToString() => Ch + " :: " + Pos;
    }

    class RbExample02
    {
        static void Main()
        {
            var map = new RankedBag<CharMap>();
            string s1 = "this is it";

            for (int pos = 0; pos < s1.Length; ++pos)
                if (! Char.IsWhiteSpace (s1[pos]))
                    map.Add (new CharMap (s1[pos], pos));

            foreach (var mapItem in map)
                Console.WriteLine (mapItem);
        }

        /* Output:

        h :: 1
        i :: 2
        i :: 5
        i :: 8
        s :: 3
        s :: 6
        t :: 0
        t :: 9

        */
    }
}
