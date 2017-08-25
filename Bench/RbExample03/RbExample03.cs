using System;
using System.Linq;
using Kaos.Collections;

namespace ExampleApp
{
    class RbExample03
    {
        static double Median (RankedBag<int> vals)
        {
            if (vals.Count == 0) return 0D;
            if (vals.Count % 2 == 1) return (double) vals.ElementAt (vals.Count/2);
            return (vals.ElementAt (vals.Count/2-1) + vals.ElementAt (vals.Count/2)) / 2D;
        }

        static double Mean (RankedBag<int> vals) => vals.Sum() / (double) vals.Count;
        static double Variance (RankedBag<int> vals) => vals.Sum (x => Math.Pow (Mean (vals) - x, 2)) / vals.Count;
        static double StandardDeviation (RankedBag<int> vals) => Math.Sqrt (Variance (vals));

        static void Main()
        {
            var scores = new RankedBag<int> (new int[] { 2, 5, 4, 4, 5, 4, 7, 9 });

            var mean = Mean (scores);
            var stddev = StandardDeviation (scores);

            Console.WriteLine ("Count = " + scores.Count + ", median = " + Median (scores) + ", mean = " + Mean (scores));
            Console.WriteLine ("Variance = " + Variance (scores));
            Console.WriteLine ("Standard deviation = " + StandardDeviation (scores));

            Console.WriteLine ("\nLow score: " + scores.Min);
            Console.WriteLine ("High score: " + scores.Max);

            Console.Write ("Scores within 1 standard deviation:");
            foreach (var score in scores.ElementsBetween ((int) (mean-stddev+0.5), (int) (mean+stddev+0.5)))
                Console.Write (" " + score);
            Console.WriteLine();
        }
    }
}
