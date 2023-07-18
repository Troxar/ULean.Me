using System;
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
            using (var input = openDatasource())
            using (var writer = new StreamWriter(outputFilename))
            {
                var c = 0;
                while (true)
                {
                    string[] record = input.NextRecord();
                    if (record == null) break;
                    c++;
                    var nums = record.Select(part => Convert.ToInt32(part, 16)).ToArray();
                    var sum = nums.Sum();
                    var text = formatter.Format(nums, sum);
                    writer.WriteLine(text);
                    if (c % 100 == 0)
                        Console.WriteLine("processed {0} items", c);
                }
            }
        }
    }
}
