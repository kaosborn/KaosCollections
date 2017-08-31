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

        public override string ToString() => "[ " + Age + ", " + Name + " ]";
    }


    class RbExample02
    {
        static int Median (RankedBag<Person> rs) => rs.ElementAt (rs.Count/2).Age;

        static void Main()
        {
            var persons = new Person[]
            {
                new Person ("Jimi Hendrix", 27), new Person ("Amy Winehouse", 27),
                new Person ("Janis Joplin", 27), new Person ("Jim Morrison", 27),
                new Person ("Kurt Cobain", 27), new Person ("John Lennon", 40),
                new Person ("Keith Moon", 32), new Person ("Randy Rhoads", 25)
            };

            var rockStars = new RankedBag<Person> (persons, new PersonAgeComparer());

            Console.WriteLine ("Dead rock stars:");
            foreach (var rs in rockStars)
                Console.WriteLine ("  " + rs);

            Console.WriteLine ("\nMedian age of death: " + Median (rockStars));

            Console.WriteLine ("\nDied at 27:");
            foreach (var rs27 in rockStars.ElementsBetween (new Person ("", 26), new Person ("zz", 27)))
                Console.WriteLine ("  " + rs27);
        }

        /* Output:

        Dead rock stars:
          [ 25, Randy Rhoads ]
          [ 27, Amy Winehouse ]
          [ 27, Janis Joplin ]
          [ 27, Jim Morrison ]
          [ 27, Jimi Hendrix ]
          [ 27, Kurt Cobain ]
          [ 32, Keith Moon ]
          [ 40, John Lennon ]

        Median age of death: 27

        Died at 27:
          [ 27, Amy Winehouse ]
          [ 27, Janis Joplin ]
          [ 27, Jim Morrison ]
          [ 27, Jimi Hendrix ]
          [ 27, Kurt Cobain ]

        */
    }
}
