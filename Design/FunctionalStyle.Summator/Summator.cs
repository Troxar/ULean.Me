using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FP
{
    public class Summator
    {
        private readonly ISumFormatter formatter;
        private readonly Func<DataSource> openDatasource;
        private readonly string outputFilename;
        
        public Summator(Func<DataSource> openDatasource, ISumFormatter formatter, string outputFilename)
        {
            this.openDatasource = openDatasource;
            this.formatter = formatter;
            this.outputFilename = outputFilename;
        }

        public void Process()
        {
            var res = SumRecords(openDatasource(), formatter)
                .AfterEvery(100, c => Console.WriteLine("processed {0} items", c));
            File.WriteAllLines(outputFilename, res);
        }

        public static IEnumerable<string> SumRecords(DataSource dataSource, ISumFormatter formatter)
        {
            return dataSource
                .ReadIntRecords(16)
                .Select(args => formatter.Format(args, args.Sum()));
        }
    }

    public static class DataSourceExtensions
    {
        public static IEnumerable<string[]> ReadRecords(this DataSource data)
        {
            return Enumeration.RepeatUntilNull(data.NextRecord);
        }

        public static IEnumerable<int[]> ReadIntRecords(this DataSource data, int radix)
        {
            return data.ReadRecords()
                .Select(record => record.Select(f => Convert.ToInt32(f, radix))
                .ToArray());
        }
    }

    public static class Enumeration
    {
        public static IEnumerable<T> RepeatUntilNull<T>(Func<T> get)
        {
            return Repeat(get).TakeWhile(i => i != null);
        }

        public static IEnumerable<T> Repeat<T>(Func<T> get)
        {
            while (true) yield return get();
            // ReSharper disable once IteratorNeverReturns
        }

        public static IEnumerable<T> AfterEvery<T>(
            this IEnumerable<T> items,
            int period,
            Action<int> afterNth)
        {
            var n = 0;
            foreach (var item in items)
            {
                n++;
                yield return item;
                if (n % period == 0) afterNth(n);
            }
        }
    }
}
