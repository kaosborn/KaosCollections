using System;
using Kaos.Collections;

namespace ExampleApp
{
    public class PersonAgeComparer : System.Collections.Generic.Comparer<Person>
    {
        public override int Compare (Person x, Person y) =>
            x.Age != y.Age ? x.Age - y.Age : String.Compare (x.Name, y.Name);
    }

    public class Person
    {
        public String Name { get; private set; }
        public int Age { get; private set; }
        public Person (string name, int age)
        { this.Name = name; this.Age = age; }
        public override string ToString () => "[ " + Age + ", " + Name + " ]";
    }


    class RbExample02
    {
        static float Mean (RankedBag<Person> rs)
        {
            float sum = 0;
            foreach (var p in rs) sum += p.Age;
            return sum / rs.Count;
        }

        static int Median (RankedBag<Person> rs)
        {
            return rs.ElementAt(rs.Count/2).Age;
        }

        static void Main()
        {
            var persons = new Person[]
            { new Person ("Jimi Hendrix", 27), new Person ("Amy Winehouse", 27),
            new Person ("Janis Joplin", 27), new Person ("Jim Morrison", 27),
            new Person ("Kurt Cobain", 27), new Person ("John Lennon", 40),
            new Person ("Keith Moon", 32), new Person ("Randy Rhoads", 25) };

            var rs = new RankedBag<Person> (persons, new PersonAgeComparer());

            Console.WriteLine ("Dead rock stars:");
            foreach (var x1 in rs)
                Console.WriteLine (x1);

            var seekMoon = new Person ("Keith aMoonx", 32);

            var moonIx = rs.IndexOf (seekMoon);
            Console.WriteLine ("moonIx = " + moonIx + ", ~moonIx = " + ~moonIx);

            Console.WriteLine ("\nMedian age of death: " + Median (rs));
            Console.WriteLine ("Average age of death: " + Mean (rs));

            Console.WriteLine ("Died at 27:");
            foreach (var p in rs.ElementsBetween (new Person ("", 26), new Person ("zz", 27)))
                Console.WriteLine ("  " + p);
        }
    }
}
