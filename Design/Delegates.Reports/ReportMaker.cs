using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Delegates.Reports
{
	public class ReportMaker<TStatsResult>
	{
        private readonly IStatisticsMaker<TStatsResult> _statsMaker;
		private readonly IReporter _reporter;
		
		public ReportMaker(
            IStatisticsMaker<TStatsResult> statsMaker,
            IReporter reporter)
		{
            _statsMaker = statsMaker;
			_reporter = reporter;
		}

		public string MakeReport(IEnumerable<Measurement> measurements)
		{
            var data = measurements.ToList();
            var result = new StringBuilder();
			result.Append(_reporter.MakeCaption(_statsMaker.Caption));
			result.Append(_reporter.BeginList());
			result.Append(_reporter.MakeItem("Temperature",
                _statsMaker.MakeStatistics(data.Select(z => z.Temperature)).ToString()));
			result.Append(_reporter.MakeItem("Humidity",
                _statsMaker.MakeStatistics(data.Select(z => z.Humidity)).ToString()));
			result.Append(_reporter.EndList());
			return result.ToString();
		}
	}

	public static class ReportMakerHelper
	{
		private static string MakeReport<TStatsResult>(
            IStatisticsMaker<TStatsResult> statsMaker,
            IReporter reporter,
            IEnumerable<Measurement> data)
		{
			return new ReportMaker<TStatsResult>(
                statsMaker,
                reporter).MakeReport(data);
		}

        public static string MeanAndStdHtmlReport(IEnumerable<Measurement> measurements)
		{
            return MakeReport(
                new MeanAndStdStatisticsMaker(),
                new HtmlReporter(),
                measurements);
		}

		public static string MedianMarkdownReport(IEnumerable<Measurement> measurements)
		{
            return MakeReport(
                new MedianStatisticsMaker(),
                new MarkdownReporter(),
                measurements);
        }

		public static string MeanAndStdMarkdownReport(IEnumerable<Measurement> measurements)
		{
            return MakeReport(
                new MeanAndStdStatisticsMaker(),
                new MarkdownReporter(),
                measurements);
        }

		public static string MedianHtmlReport(IEnumerable<Measurement> measurements)
		{
            return MakeReport(
                new MedianStatisticsMaker(),
                new HtmlReporter(),
                measurements);
        }
	}

	public interface IStatisticsMaker<TResult>
	{
        string Caption { get; }
        TResult MakeStatistics(IEnumerable<double> data);
    }

    public class MeanAndStdStatisticsMaker : IStatisticsMaker<MeanAndStd>
    {
        public string Caption => "Mean and Std";

        public MeanAndStd MakeStatistics(IEnumerable<double> data)
        {
            var list = data.ToList();
            var mean = list.Average();
            var std = Math.Sqrt(list.Select(z => Math.Pow(z - mean, 2)).Sum() / (list.Count - 1));

            return new MeanAndStd
            {
                Mean = mean,
                Std = std
            };
        }
    }

    public class MedianStatisticsMaker : IStatisticsMaker<double>
    {
        public string Caption => "Median";

        public double MakeStatistics(IEnumerable<double> data)
        {
            var list = data.OrderBy(z => z).ToList();
            if (list.Count % 2 == 0)
                return (list[list.Count / 2] + list[list.Count / 2 - 1]) / 2;
            return list[list.Count / 2];
        }
    }

	public interface IReporter
    {
        string MakeCaption(string caption);
        string BeginList();
        string MakeItem(string valueType, string entry);
        string EndList();
    }

    public class HtmlReporter : IReporter
    {
        public string BeginList() => "<ul>";
        public string EndList() => "</ul>";
        public string MakeCaption(string caption) => $"<h1>{caption}</h1>";
        public string MakeItem(string valueType, string entry) => $"<li><b>{valueType}</b>: {entry}";
    }

    public class MarkdownReporter : IReporter
    {
        public string BeginList() => string.Empty;
        public string EndList() => string.Empty;
        public string MakeCaption(string caption) => $"## {caption}\n\n";
        public string MakeItem(string valueType, string entry) => $" * **{valueType}**: {entry}\n\n";
    }
}