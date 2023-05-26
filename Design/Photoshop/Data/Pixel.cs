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

        public Pixel()
        {

        }

        public Pixel(Color color)
        {
            Red = color.R / 255f;
            Green = color.G / 255f;
            Blue = color.B / 255f;
        }

        public static Pixel operator *(Pixel left, double right) => new Pixel
        {
            Red = left.Red * right,
            Green = left.Green * right,
            Blue = left.Blue * right
        };

        private static double CheckChannelValue(string channel, double value)
        {
            if (value < 0 || value > 1)
                throw new InvalidChannelValueException($"Wrong {channel} channel value {value} (the value must be between 0 and 1)");
            return value;
        }
    }
}