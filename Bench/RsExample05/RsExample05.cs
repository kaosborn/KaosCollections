using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
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

        protected Person (SerializationInfo info, StreamingContext context)
        {
            this.First = (String) info.GetValue ("First", typeof (String));
            this.Last = (String) info.GetValue ("Last", typeof (String));
        }

        public virtual void GetObjectData (SerializationInfo info, StreamingContext context)
        {
            info.AddValue ("First", First, typeof (String));
            info.AddValue ("Last", Last, typeof (String));
        }

        public override string ToString() => Last + ", " + First;
    }


    class RsExample05
    {
        static void Main()
        {
            var set1 = new RankedSet<Person> (new PersonComparer());
            set1.Add (new Person ("Hugh", "Mann"));
            set1.Add (new Person ("Hammond", "Egger"));

            string fileName = "Persons.bin";
            IFormatter formatter = new BinaryFormatter();

            SerializePersons (fileName, set1, formatter);
            Console.WriteLine ($"Wrote {set1.Count} items to file '{fileName}'.");
            Console.WriteLine ();

            RankedSet<Person> set2 = DeserializePersons (fileName, formatter);
            Console.WriteLine ($"Read back {set2.Count} items:");

            foreach (var p2 in set2)
                Console.WriteLine (p2);
        }

        public static void SerializePersons (string fn, RankedSet<Person> set, IFormatter formatter)
        {
            using (var fs = new FileStream (fn, FileMode.Create))
            { formatter.Serialize (fs, set); }
        }

        static RankedSet<Person> DeserializePersons (string fn, IFormatter formatter)
        {
            using (var fs = new FileStream (fn, FileMode.Open))
            { return (RankedSet<Person>) formatter.Deserialize (fs); }
        }

        /* Output:

        Wrote 2 items to file 'Persons.bin'.

        Read back 2 items:
        Egger, Hammond
        Mann, Hugh

        */
    }
}
