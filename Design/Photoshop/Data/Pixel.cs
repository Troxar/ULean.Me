﻿using System.Drawing;

namespace MyPhotoshop
{
    public struct Pixel
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

        public Pixel(Color color) : this()
        {
            const double rate = 255;
            Red = color.R / rate;
            Green = color.G / rate;
            Blue = color.B / rate;
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