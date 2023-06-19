using MyPhotoshop;
using System.Diagnostics;

namespace Profiler
{
    internal class Program
    {
        private static void Test(Func<double[], IParameters> method, int count, string description)
        {
            var args = new double[] { 0 };
            method(args);

            var sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < count; i++)
                method(args);

            sw.Stop();

            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine($"{description}: {sw.ElapsedMilliseconds * 1000d / count} µs");
        }

        public static void Main(string[] args)
        {
            var simpleHandler = new SimpleParametersHandler<LighteningParameters>();
            var staticHandler = new StaticParametersHandler<LighteningParameters>();

            int count = 100000;
            Test(values => new LighteningParameters { Coefficient = values[0] }, count, "Direct assignment: ");
            Test(values => simpleHandler.CreateParameters(values), count, "Simple handler");
            Test(values => staticHandler.CreateParameters(values), count, "Static handler");
        }
    }
}