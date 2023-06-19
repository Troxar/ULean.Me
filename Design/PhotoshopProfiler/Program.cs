using MyPhotoshop;
using System.Diagnostics;

namespace Profiler
{
    internal class Program
    {
        private static void Test(Func<double[], IParameters> method, int count)
        {
            var args = new double[] { 0 };
            method(args);

            var sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < count; i++)
                method(args);

            sw.Stop();

            Console.WriteLine(sw.ElapsedMilliseconds * 1000d / count);
        }

        public static void Main(string[] args)
        {
            var simpleHandler = new SimpleParametersHandler<LighteningParameters>();
            int count = 100000;
            Test(values => simpleHandler.CreateParameters(values), count);
            Test(values => new LighteningParameters { Coefficient = values[0] }, count);
        }
    }
}