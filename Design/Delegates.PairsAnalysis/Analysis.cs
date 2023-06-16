using System;
using System.Collections.Generic;
using System.Linq;

namespace Delegates.PairsAnalysis
{
    public static class Analysis
    {
        public static int FindMaxPeriodIndex(params DateTime[] data)
        {
            return data
                .Pairs()
                .MaxIndex(x => (x.Item2 - x.Item1).TotalSeconds);
        }

        public static double FindAverageRelativeDifference(params double[] data)
        {
            return data
                .Pairs()
                .Average(x => (x.Item2 - x.Item1) / x.Item1);
        }
    }

    public static class Extensions
    {
        public static IEnumerable<Tuple<T, T>> Pairs<T>(this IEnumerable<T> source)
        {
            T prev = default;
            int counter = 0;

            foreach (T value in source)
            {
                if (counter > 0)
                    yield return Tuple.Create(prev, value);
                prev = value;
                counter++;
            }

            if (counter < 2)
                throw new InvalidOperationException("Not enough items in collection");
        }

        public static int MaxIndex<TSource, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, TResult> selector)
                where TResult : IComparable
        {
            TResult max = default;
            int bestIndex = 0;
            int counter = 0;

            foreach (TSource pair in source)
            {
                TResult value = selector(pair);
                if (counter == 0 || value.CompareTo(max) > 0)
                {
                    max = value;
                    bestIndex = counter;
                }
                counter++;
            }

            if (counter == 0)
                throw new InvalidOperationException("Collection is empty");

            return bestIndex;
        }

        // required only for successful compilation on the site
        public static int MaxIndex<T>(this IEnumerable<T> source)
            where T : IComparable
        {
            return source.MaxIndex(x => x);
        }
    }
}