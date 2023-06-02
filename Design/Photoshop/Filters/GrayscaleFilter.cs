using System;

namespace MyPhotoshop
{
    public class GrayscaleFilter : PixelFilter
    {
        public override ParameterInfo[] GetParameters()
        {
            return Array.Empty<ParameterInfo>();
        }

        public override string ToString()
        {
            return "Оттенки серого";
        }

        public override Pixel ProcessPixel(Pixel original, double[] parameters)
        {
            var shade = (original.Red
                        + original.Green
                        + original.Blue) / 3;
            return new Pixel(shade, shade, shade);
        }
    }
}