using System;

namespace MyPhotoshop
{
    public class GrayscaleFilter : IFilter
    {
        public ParameterInfo[] GetParameters()
        {
            return Array.Empty<ParameterInfo>();
        }

        public override string ToString()
        {
            return "Оттенки серого";
        }

        public Photo Process(Photo original, double[] parameters)
        {
            var result = new Photo(original.Width, original.Height);
            
            for (int x = 0; x < result.Width; x++)
                for (int y = 0; y < result.Height; y++)
                    result[x, y] = ProcessPixel(original[x, y], parameters);

            return result;
        }

        private Pixel ProcessPixel(Pixel original, double[] parameters)
        {
            var shade = (original.Red
                        + original.Green
                        + original.Blue) / 3;
            return new Pixel(shade, shade, shade);
        }
    }
}