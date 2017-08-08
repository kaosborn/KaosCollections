using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Kaos.Collections;

namespace ExampleApp
{
    [Serializable]
    public class PersonComparer : Comparer<Person>
    {
        public override int Compare (Person x, Person y)
        { return x==null? (y==null? 0 : -1) : (y==null? 1 : String.Compare (x.ToString(), y.ToString())); }
    }

    [Serializable]
    public class Person : ISerializable
    {
        public string First { get; private set; }
        public string Last { get; private set; }

        public Person (string first, string last)
        { this.First = first; this.Last = last; }

        public Person (SerializationInfo info, StreamingContext context)
        {
            this.First = (string) info.GetValue ("First", typeof (String));
            this.Last = (string) info.GetValue ("Last", typeof (String));
        }

        public void GetObjectData (SerializationInfo info, StreamingContext context)
        {
            info.AddValue ("First", First, typeof (String));
            info.AddValue ("Last", Last, typeof (String));
        }

        public override string ToString() => Last + ", " + First;
    }


    class RdExample05
    {
        static void Main()
        {
            IFormatter formatter = new BinaryFormatter();
            var set1 = new RankedDictionary<Person,string> (new PersonComparer());

            set1.Add (new Person ("Hugh", "Mann"), "B+");
            set1.Add (new Person ("Hammond", "Egger"), "C-");

            SerializePersons ("Persons.bin", set1, formatter);
            Console.WriteLine ("Wrote " + set1.Count + " key/value pairs.");
            Console.WriteLine ();

            RankedDictionary<Person,string> set2 = DeserializePersons ("Persons.bin", formatter);
            Console.WriteLine ("Read back:");

            foreach (var kv in set2)
                Console.WriteLine (kv);
        }

        public static void SerializePersons (string fn, RankedDictionary<Person,string> set, IFormatter formatter)
        {
            using (var fs = new FileStream (fn, FileMode.Create))
            { formatter.Serialize (fs, set); }
        }

        static RankedDictionary<Person,string> DeserializePersons (string fn, IFormatter formatter)
        {
            using (var fs = new FileStream (fn, FileMode.Open))
            { return (RankedDictionary<Person,string>) formatter.Deserialize (fs); }
        }

        /* Output:

        Wrote 2 key/value pairs.

        Read back:
        [Egger, Hammond, C-]
        [Mann, Hugh, B+]

        */
    }
}
