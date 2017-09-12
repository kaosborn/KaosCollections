using System;
using System.Collections.Generic;
using Kaos.Collections;

namespace ExampleApp
{
    class RdExample01
    {
        static void Main()
        {
            // ISO 3166-1 country codes:
            var cc = new RankedDictionary<string,string>();

            cc.Add ("TO", "Tonga");
            cc.Add ("DD", "German Democratic Republic");
            cc.Add ("CH", "Switzerland");
            cc.Add ("RU", "Burundi");

            // The Add method throws an exception if the new key is
            // already in the dictionary.
            try
            {
                cc.Add ("DD", "East Germany");
            }
            catch (ArgumentException)
            {
                Console.WriteLine ("An element with Key = 'DD' already exists.");
            }

            // The Item property is another name for the indexer,
            // so you can omit its name when accessing elements.
            Console.WriteLine ("For key = 'CH', value = {0}.", cc["CH"]);

            // The indexer can be used to change the value associated with a key.
            cc["RU"] = "Russian Federation";
            Console.WriteLine ("For key = 'RU', value = {0}.", cc["RU"]);

            // If a key does not exist, setting the indexer for that key
            // adds a new key/value pair.
            cc["SS"] = "South Sudan";

            // The indexer throws an exception if the supplied key is
            // not in the dictionary.
            try
            {
                Console.WriteLine ("For key = 'ZZ', value = {0}.", cc["ZZ"]);
            }
            catch (KeyNotFoundException)
            {
                Console.WriteLine ("Key = 'ZZ' is not found.");
            }

            // When a program often has to try keys that are usually not in the
            // dictionary, TryGetValue can be a more efficient way to get values.
            string value = "";
            if (cc.TryGetValue ("ZZ", out value))
                Console.WriteLine ("For key = 'ZZ', value = {0}.", value);
            else
                Console.WriteLine ("Key = 'ZZ' is not found.");

            // ContainsKey can be used to test keys before inserting them.
            if (! cc.ContainsKey ("GG"))
            {
                cc.Add ("GG", "Guernsey");
                Console.WriteLine ("Value added for key = 'GG': {0}", cc["GG"]);
            }

            // When you use foreach to enumerate dictionary elements,
            // the elements are retrieved as KeyValuePair instances.
            Console.WriteLine();
            foreach (KeyValuePair<string,string> pair in cc)
                Console.WriteLine ("Key = {0}, Value = {1}", pair.Key, pair.Value);

            // To get the values alone, use the Values property.
            RankedDictionary<string,string>.ValueCollection vals = cc.Values;

            // The elements of the ValueCollection are strongly typed
            // with the type that was specified for dictionary values.
            Console.WriteLine();
            foreach(string val in vals)
                Console.WriteLine ("Value = {0}", val);

            // To get the keys alone, use the Keys property.
            RankedDictionary<string,string>.KeyCollection keys = cc.Keys;

            // The elements of the KeyCollection are strongly typed
            // with the type that was specified for dictionary keys.
            Console.WriteLine();
            foreach (string key in keys)
                Console.WriteLine("Key = {0}", key);

            // Use the Remove method to remove a key/value pair.
            Console.WriteLine ("\nRemoving 'DD'.");
            cc.Remove ("DD");

            Console.WriteLine ("\nChecking if 'DD' exists:");
            if (! cc.ContainsKey ("DD"))
                Console.WriteLine ("  Key 'DD' not found.");
        }

        /* Output:

        An element with Key = 'DD' already exists.
        For key = 'CH', value = Switzerland.
        For key = 'RU', value = Russian Federation.
        Key = 'ZZ' is not found.
        Key = 'ZZ' is not found.
        Value added for key = 'GG': Guernsey

        Key = CH, Value = Switzerland
        Key = DD, Value = German Democratic Republic
        Key = GG, Value = Guernsey
        Key = RU, Value = Russian Federation
        Key = SS, Value = South Sudan
        Key = TO, Value = Tonga

        Value = Switzerland
        Value = German Democratic Republic
        Value = Guernsey
        Value = Russian Federation
        Value = South Sudan
        Value = Tonga

        Key = CH
        Key = DD
        Key = GG
        Key = RU
        Key = SS
        Key = TO

        Removing 'DD'.

        Checking if 'DD' exists:
          Key 'DD' not found.

        */
    }
}
