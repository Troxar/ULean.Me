using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Text;

namespace StructBenchmarking
{
    public class Benchmark : IBenchmark
    {
        public double MeasureDurationInMs(ITask task, int repetitionCount)
        {
            GC.Collect();                   // Эти две строчки нужны, чтобы уменьшить вероятность того,
            GC.WaitForPendingFinalizers();  // что Garbadge Collector вызовется в середине измерений
                                            // и как-то повлияет на них.

            task.Run();                     // warming call

            var watch = new Stopwatch();
            watch.Start();

            for (int i = 0; i < repetitionCount; i++)
                task.Run();

            watch.Stop();

            return watch.ElapsedMilliseconds / (double)repetitionCount;
        }
    }

    [TestFixture]
    public class RealBenchmarkUsageSample
    {
        [Test]
        public void StringConstructorFasterThanStringBuilder()
        {
            int count = 10000;
            char letter = 'a';

            var sbCtor = new StringBuilderConstructor(count, letter);
            var stringCtor = new StringConstructor(count, letter);

            int repetitionCount = 1000;

            var benchmark = new Benchmark();
            double durationSb = benchmark.MeasureDurationInMs(sbCtor, repetitionCount);
            double durationString = benchmark.MeasureDurationInMs(stringCtor, repetitionCount);

            Assert.Less(durationString, durationSb);
            Assert.AreEqual(sbCtor.Result, stringCtor.Result);
        }
    }

    public class StringBuilderConstructor : ITask
    {
        int _count;
        char _letter;

        public string Result { get; private set; }

        public StringBuilderConstructor(int count, char letter)
        {
            _count = count;
            _letter = letter;
        }

        public void Run()
        {
            var sb = new StringBuilder();

            for (int i = 0; i < _count; i++)
                sb.Append(_letter);

            Result = sb.ToString();
        }
    }

    public class StringConstructor : ITask
    {
        int _count;
        char _letter;

        public string Result { get; private set; }

        public StringConstructor(int count, char letter)
        {
            _count = count;
            _letter = letter;
        }

        public void Run()
        {
            Result = new string(_letter, _count);
        }
    }
}