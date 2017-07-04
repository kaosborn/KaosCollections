﻿//
// Program: BtreeBench02.cs
// Purpose: Perform comparison benchmarks on BtreeDictionary & SortedDictionary.
//
// Usage notes:
// - For valid results, run Release build outside Visual Studio.
// - Adjust reps to change test duration.
//

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Kaos.Collections;

namespace BenchApp
{
    public class Exercise<T> where T : IDictionary<long,long>, new()
    {
        // Break up 's' iterations into 'f' chunks.
        long reps = 5000000;
        long f = 5;

        IDictionary<long,long> tree;

        public Exercise() { tree = new T(); }

        public void Clear() { tree.Clear(); }

        public long RunInsert()
        {
            long k = 0;
            for (long i = 0; i < reps / f; ++i)
                for (long j = i; j < reps; j += reps / f)
                {
                    tree.Add (j, -j);
                    ++k;
                }
            return k;
        }

        public void RunSeek()
        {
            long sum = 0;
            for (long i = 0; i < reps / f; ++i)
                for (long j = i; j < reps; j += reps / f)
                    sum += tree[j];
        }

        public long RunPairIterator()
        {
            long sum = 0;
            foreach (KeyValuePair<long,long> pair in tree)
                sum += pair.Value;
            return sum;
        }

        public long RunKeyIterator()
        {
            long sum = 0;
            foreach (long key in tree.Keys)
                sum += key;
            return sum;
        }

        public void RunInsertSequential()
        {
            for (long j = 0; j < reps; ++j)
                tree.Add (j, -j);
        }
    }


    class BtreeBench02
    {
        static void BenchSuite<T> (string title, Exercise<T> sort)
            where T : IDictionary<long,long>, new()
        {
            long result;
            Stopwatch watch = new Stopwatch();

            Console.WriteLine();
            Console.WriteLine ("Running suite for {0}:", title);

            watch.Reset(); watch.Start();
            sort.RunInsert();
            Console.WriteLine ("Random insert: time={0}ms", watch.ElapsedMilliseconds);

            watch.Reset(); watch.Start();
            sort.RunSeek();
            Console.WriteLine ("-Seek: time={0}ms", watch.ElapsedMilliseconds);

            watch.Reset(); watch.Start();
            result = sort.RunPairIterator();
            Console.WriteLine ("-Pair iterator: time={0}ms result={1}", watch.ElapsedMilliseconds, result);

            sort.Clear();
            watch.Reset(); watch.Start();
            sort.RunInsertSequential();
            Console.WriteLine ("Sequential insert: time={0}ms", watch.ElapsedMilliseconds);

            watch.Reset(); watch.Start();
            sort.RunSeek();
            Console.WriteLine ("-Seek time={0}ms", watch.ElapsedMilliseconds);

            watch.Reset(); watch.Start();
            result = sort.RunPairIterator();
            Console.WriteLine ("-Pair iterator: time={0}ms result={1}", watch.ElapsedMilliseconds, result);

            sort.Clear();
        }

        static void Main()
        {
            var b1 = new Exercise<BtreeDictionary<long,long>>();
            var b2 = new Exercise<SortedDictionary<long,long>>();

            BenchSuite ("BtreeDictionary", b1);
            BenchSuite ("SortedDictionary", b2);

            BenchSuite ("BtreeDictionary", b1);
            BenchSuite ("SortedDictionary", b2);
        }
    }
}
