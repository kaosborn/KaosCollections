using System;
using Kaos.Collections;

namespace ExampleApp
{
    class RmExample01
    {
        static void Main()
        {
            #region Ctor1
            // Keys will compare case insensitive with this constructor.
            var fmt = new RankedMap<string,string> (StringComparer.InvariantCultureIgnoreCase);
            #endregion

            fmt.Add ("csv", "Comma Separated Values");
            fmt.Add ("DAT", "dBase II data file");
            fmt.Add ("dat", "Sony Digital Audio Tape");

            #region Add
            fmt.Add ("qt", "QuickTime movie clip");
            fmt.Add ("jpeg", "JPEG bitmap image format");
            fmt.Add ("sln", "Blend for Visual Studio");
            fmt.Add ("sln", "Visual Studio 2017");
            #endregion

            #region ContainsKey
            if (fmt.ContainsKey ("jpeg"))
                fmt.Add ("jpg", "JPEG bitmap image format");
            #endregion

            #region GetCount
            if (fmt.Keys.GetCount ("sln") > 1)
                fmt.Add ("sln", "Visual Studio Version Selector");
            #endregion

            #region Etor
            // When you use foreach to enumerate map elements,
            // each element is retrieved as a KeyValuePair.
            Console.WriteLine ("All map elements:");
            foreach (System.Collections.Generic.KeyValuePair<string,string> pair in fmt)
                Console.WriteLine ("  Key = {0}, Value = {1}", pair.Key, pair.Value);
            #endregion

            #region GetDistinctCount
            Console.WriteLine ("\nDistinct format count: " + fmt.Keys.GetDistinctCount());
            Console.WriteLine ("Total format count: " + fmt.Count);
            #endregion

            #region Remove2
            Console.WriteLine ("\nRemove all sln format occurrences...");
            int removed = fmt.Remove ("sln", Int32.MaxValue);
            Console.WriteLine ("Items removed: " + removed);
            #endregion

            #region Remove1
            Console.WriteLine ("Remove qt format...");
            fmt.Remove ("qt");
            #endregion

            #region Keys
            // To get the keys alone, use the Keys property.
            RankedMap<string,string>.KeyCollection keys = fmt.Keys;

            // The items of a KeyCollection have the type specified for the map keys.
            Console.WriteLine ("\nKeys:");
            foreach (string x in keys)
                Console.WriteLine ("  " + x);
            #endregion

            #region Values
            // To get the values alone, use the Values property.
            RankedMap<string,string>.ValueCollection values = fmt.Values;

            Console.WriteLine ("\nValues:");
            foreach (string x in values)
                Console.WriteLine ("  " + x);
            #endregion
        }

        /* Output:

        All map elements:
          Key = csv, Value = Comma Separated Values
          Key = DAT, Value = dBase II data file
          Key = dat, Value = Sony Digital Audio Tape
          Key = jpeg, Value = JPEG bitmap image format
          Key = jpg, Value = JPEG bitmap image format
          Key = qt, Value = QuickTime movie clip
          Key = sln, Value = Blend for Visual Studio
          Key = sln, Value = Visual Studio 2017
          Key = sln, Value = Visual Studio Version Selector

        Distinct format count: 6
        Total format count: 9

        Remove all sln format occurrences...
        Items removed: 3
        Remove qt format...

        Keys:
          csv
          DAT
          dat
          jpeg
          jpg

        Values:
          Comma Separated Values
          dBase II data file
          Sony Digital Audio Tape
          JPEG bitmap image format
          JPEG bitmap image format

        */
    }
}
