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
                {
                    var pixel = original[x, y];
                    var shade = (pixel.Red
                        + pixel.Green
                        + pixel.Blue) / 3;
                    result[x, y] = new Pixel(shade, shade, shade);
                }

            return result;
        }
    }
}