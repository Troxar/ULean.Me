using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Memory.Timers
{
    public class Timer : IDisposable
    {
        private readonly Timer _parent;
        private readonly TextWriter _writer;
        private readonly string _name;
        private readonly Stopwatch _timer;
        public  readonly List<TimerResult> NestedResults = new List<TimerResult>();

        private Timer(string name)
        {
            _name = name;
            _timer = new Stopwatch();
            _timer.Start();
        }

        private Timer(TextWriter writer, string name)
            : this(name)
        {
            _writer = writer;
        }

        private Timer(Timer parent, string name)
            : this(name)
        {
            _parent = parent;
        }

        public Timer StartChildTimer(string name)
        {
            return new Timer(this, name);
        }

        public void Dispose()
        {
            _timer.Stop();

            var result = new TimerResult(_name, _timer.ElapsedMilliseconds, NestedResults);
            if (_writer != null)
                _writer.Write(TimerResultReportGenerator.Generate(result));
            else
                _parent.NestedResults.Add(result);
        }

        public static Timer Start(TextWriter writer, string name = "")
        {
            return new Timer(writer, name);
        }
    }

    public class TimerResult
    {
        public string Name { get; }
        public long Result { get; }
        public List<TimerResult> Nested { get; }

        public TimerResult(string name, long result, List<TimerResult> nested)
        {
            Name = name;
            Result = result;
            Nested = nested;
        }
    }

    public static class TimerResultReportGenerator
    {
        public static string Generate(TimerResult timerResult)
        {
            var sb = new StringBuilder();
            Generate(sb, timerResult, 0);
            return sb.ToString();
        }

        private static void Generate(StringBuilder sb, TimerResult timerResult, int level)
        {
            var name = string.IsNullOrEmpty(timerResult.Name) ? "*" : timerResult.Name;
            sb.Append(FormatReportLine(name, level, timerResult.Result));

            if (timerResult.Nested.Any())
            {
                var sum = 0L;

                foreach (var nestedResult in timerResult.Nested)
                {
                    Generate(sb, nestedResult, level + 1);
                    sum += nestedResult.Result;
                }

                sb.Append(FormatReportLine("Rest", level + 1, timerResult.Result - sum));
            }
        }

        private static string FormatReportLine(string timerName, int level, long value)
        {
            var intro = new string(' ', level * 4) + timerName;
            return $"{intro,-20}: {value}\n";
        }
    }
}
