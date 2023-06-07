using System;

namespace MyPhotoshop
{
    public class GrayscaleFilter : PixelFilter<GrayscaleParameters>
    {
        public override string ToString()
        {
            return "Оттенки серого";
        }

        public override Pixel ProcessPixel(Pixel original, GrayscaleParameters parameters)
        {
            var shade = (original.Red
                        + original.Green
                        + original.Blue) / 3;
            return new Pixel(shade, shade, shade);
        }
    }
}