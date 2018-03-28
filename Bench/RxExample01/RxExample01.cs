using System;
using Kaos.Collections;

namespace RxExample01
{
    class ExampleApp
    {
        static void Main()
        {
            Console.WriteLine ("RankedBag: ");

            #region RbSkip

            var bag = new RankedSet<int>() { 295 };
            for (var i = 2; i < 500; i += 2) bag.Add (i);

            foreach (int x in bag.Skip (100).SkipWhile (i => i%2==0).Skip (100))
                Console.Write ($"{x} ");

            // Output: 494 496 498 

            #endregion

            Console.WriteLine ("\nRankedSet: ");

            #region RsSkip

            var set = new RankedSet<int>() { 295 };
            for (int i = 2; i < 500; i += 2) set.Add (i);

            foreach (var x in set.Skip (100).SkipWhile (i => i%2==0).Skip (100))
                Console.Write ($"{x} ");

            // Output: 494 496 498 

            #endregion

            Console.WriteLine ("\nRankedDictionary: ");

            #region RdSkip

            var dary = new RankedDictionary<int,int>() { {295,-295} };
            for (int i = 2; i < 500; i += 2) dary.Add (i,-i);

            foreach (var x in dary.Skip (100).SkipWhile (kv => kv.Key%2==0).Skip (100))
                Console.Write ($"{x} ");

            // Output: [494, -494] [496, -496] [498, -498] 

            #endregion


            Console.WriteLine ("\nRankedDictionary.Keys: ");

            #region RdkSkip

            var daryk = new RankedMap<int,int>() { {295,-295} };
            for (int i = 2; i < 500; i += 2) daryk.Add (i,-i);

            foreach (var x in daryk.Keys.Skip (100).SkipWhile (k => k%2==0).Skip (100))
                Console.Write ($"{x} ");

            // Output: 494 496 498 

            #endregion

            Console.WriteLine ("\nRankedDictionary.Values: ");

            #region RdvSkip

            var daryv = new RankedMap<int,int>() { { 295,-295 } };
            for (int i = 2; i < 500; i += 2) daryv.Add (i,-i);

            foreach (var x in daryv.Values.Skip (100).SkipWhile (k => k%2==0).Skip (100))
                Console.Write ($"{x} ");

            // Output: -494 -496 -498 

            #endregion

            Console.WriteLine ("\nRankedMap: ");

            #region RmSkip

            var map = new RankedMap<int,int>() { {295,-295} };
            for (int i = 2; i < 500; i += 2) map.Add (i,-i);

            foreach (var x in map.Skip (100).SkipWhile (kv => kv.Key%2==0).Skip (100))
                Console.Write ($"{x} ");

            // Output: [494, -494] [496, -496] [498, -498] 

            #endregion

            Console.WriteLine ("\nRankedMap.Keys: ");

            #region RmkSkip

            var mapk = new RankedMap<int,int>() { {295,-295} };
            for (int i = 2; i < 500; i += 2) mapk.Add (i,-i);

            foreach (var x in mapk.Keys.Skip (100).SkipWhile (k => k%2==0).Skip (100))
                Console.Write ($"{x} ");

            // Output: 494 496 498 

            #endregion

            Console.WriteLine ("\nRankedMap.Values: ");

            #region RmvSkip

            var mapv = new RankedMap<int,int>() { { 295,-295 } };
            for (int i = 2; i < 500; i += 2) mapv.Add (i,-i);

            foreach (var x in mapv.Values.Skip (100).SkipWhile (k => k%2==0).Skip (100))
                Console.Write ($"{x} ");

            // Output: -494 -496 -498 

            #endregion

            Console.WriteLine();
        }
    }
}
