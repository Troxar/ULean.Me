using System.Drawing;

namespace MyPhotoshop
{
    public class Pixel
    {
        public double Red;
        public double Green;
        public double Blue;

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
    }
}
