using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Kaos.Collections;

namespace ExampleApp
{
    [Serializable]
    public class ExamComparer : Comparer<Exam>
    {
        public override int Compare (Exam x, Exam y)
        { return x.Score - y.Score; }
    }

    [Serializable]
    public class Exam : ISerializable
    {
        public int Score { get; private set; }
        public string Name { get; private set; }

        public Exam (int score, string name)
        { this.Score = score; this.Name = name; }

        protected Exam (SerializationInfo info, StreamingContext context)
        {
            this.Score = (int) info.GetValue ("Score", typeof (int));
            this.Name = (String) info.GetValue ("Name", typeof (String));
        }

        public virtual void GetObjectData (SerializationInfo info, StreamingContext context)
        {
            info.AddValue ("Score", Score, typeof (int));
            info.AddValue ("Name", Name, typeof (String));
        }

        public override string ToString() => Score + ", " + Name;
    }


    class RbExample05
    {
        static void Main()
        {
            var bag1 = new RankedBag<Exam> (new ExamComparer());
            bag1.Add (new Exam (5, "Jack"));
            bag1.Add (new Exam (2, "Ned"));
            bag1.Add (new Exam (2, "Betty"));
            bag1.Add (new Exam (3, "Paul"));
            bag1.Add (new Exam (5, "John"));

            Console.WriteLine ("Where comparer returns equality, items will retain sequence added:");
            foreach (var item in bag1)
                Console.WriteLine ("  " + item);

            string fileName = "Exams.bin";
            IFormatter formatter = new BinaryFormatter();

            SerializePersons (fileName, bag1, formatter);
            Console.WriteLine ("\nWrote " + bag1.Count + " items to file '" + fileName + "'.");
            Console.WriteLine ();

            RankedBag<Exam> bag2 = DeserializePersons (fileName, formatter);
            Console.WriteLine ("Read back " + bag2.Count + " items:");

            foreach (var p2 in bag2)
                Console.WriteLine ("  " + p2);
        }

        static void SerializePersons (string fn, RankedBag<Exam> set, IFormatter formatter)
        {
            using (var fs = new FileStream (fn, FileMode.Create))
            { formatter.Serialize (fs, set); }
        }

        static RankedBag<Exam> DeserializePersons (string fn, IFormatter formatter)
        {
            using (var fs = new FileStream (fn, FileMode.Open))
            { return (RankedBag<Exam>) formatter.Deserialize (fs); }
        }

        /* Output:

        Where comparer returns equality, items will retain sequence added:
          2, Ned
          2, Betty
          3, Paul
          5, Jack
          5, John

        Wrote 5 items to file 'Exams.bin'.

        Read back 5 items:
          2, Ned
          2, Betty
          3, Paul
          5, Jack
          5, John

        */
    }
}
