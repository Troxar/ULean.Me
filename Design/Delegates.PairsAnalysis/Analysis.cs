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
                .ProcessItems(pair => (pair.Item2 - pair.Item1).TotalSeconds)
                .MaxIndex();
        }

        public static double FindAverageRelativeDifference(params double[] data)
        {
            return data
                .Pairs()
                .ProcessItems(pair => (pair.Item2 - pair.Item1) / pair.Item1)
                .Average();
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

        public static int MaxIndex<T>(this IEnumerable<T> source)
            where T : IComparable
        {
            T max = default;
            int bestIndex = 0;
            int counter = 0;

            foreach (T value in source)
            {
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

        public static IEnumerable<TResult> ProcessItems<TSource, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, TResult> processor)
        {
            foreach (TSource value in source)
                yield return processor(value);
        }
    }
}