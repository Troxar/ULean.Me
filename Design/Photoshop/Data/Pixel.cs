using System.Drawing;

namespace MyPhotoshop
{
    public class Pixel
    {
        private double _red;
        public double Red
        {
            get { return _red; }
            set { _red = CheckChannelValue(nameof(Red), value); }
        }

        private double _green;
        public double Green
        {
            get { return _green; }
            set { _green = CheckChannelValue(nameof(Green), value); }
        }

        private double _blue;
        public double Blue
        {
            get { return _blue; }
            set { _blue = CheckChannelValue(nameof(Blue), value); }
        }

        private static double CheckChannelValue(string channel, double value)
        {
            if (value < 0 || value > 1)
                throw new InvalidChannelValueException($"Wrong {channel} channel value {value} (the value must be between 0 and 1)");
            return value;
        }

        public void Fill(Color color)
        {
            double rate = 255f;
            Red = color.R / rate;
            Green = color.G / rate;
            Blue = color.B / rate;
        }
    }
}