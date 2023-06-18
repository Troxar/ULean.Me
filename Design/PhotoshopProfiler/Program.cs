using MyPhotoshop;
using System.Diagnostics;

namespace Profiler
{
    internal class Program
    {
        private static void Test(Action<double[], LighteningParameters> action, int count)
        {
            var args = new double[] { 0 };
            var parameters = new LighteningParameters();
            action(args, parameters);

            var sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < count; i++)
                action(args, parameters);

            sw.Stop();

            Console.WriteLine(sw.ElapsedMilliseconds * 1000d / count);
        }

        public static void Main(string[] args)
        {
            int count = 100000;
            Test((values, parameters) => parameters.Parse(values), count);
            Test((values, parameters) => parameters.Coefficient = values[0], count);
        }
    }
}