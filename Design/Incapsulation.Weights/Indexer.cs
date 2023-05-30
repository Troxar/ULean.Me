using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incapsulation.Weights
{
	public class Indexer
	{
		private readonly double[] _data;
		private readonly int _start;

		public int Length { get; }

        public Indexer(double[] data, int start, int length)
		{
			_data = data;
			_start = CheckStart(start, data);
			Length = CheckLength(start, length, data);
		}

		public double this[int i]
		{
			get { return _data[_start + CheckIndex(i)]; }
			set { _data[_start + CheckIndex(i)] = value; }
		}

		private int CheckStart(int start, double[] data)
		{
			if (start < 0 || start > data.Length)
				throw new ArgumentException(nameof(start));
			return start;
        }

        private int CheckLength(int start, int length, double[] data)
        {
            if (length < 0 || start + length > data.Length)
                throw new ArgumentException(nameof(length));
            return length;
        }

		private int CheckIndex(int i)
		{
			if (i < 0 || i >= Length)
				throw new IndexOutOfRangeException();
			return i;
		}
    }
}