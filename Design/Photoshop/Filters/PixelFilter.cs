using System;

namespace MyPhotoshop
{
    public class PixelFilter<TParameters> : ParametrizedFilter<TParameters>
        where TParameters : IParameters, new()
    {
        private string _name;
        private Func<Pixel, TParameters, Pixel> _processor;

        public PixelFilter(string name, Func<Pixel, TParameters, Pixel> processor)
        {
            _name = name;
            _processor = processor;
        }

        public override string ToString() => _name;

        public override Photo Process(Photo original, TParameters parameters)
        {
            var result = new Photo(original.Width, original.Height);

            for (int x = 0; x < result.Width; x++)
                for (int y = 0; y < result.Height; y++)
                    result[x, y] = _processor(original[x, y], parameters);

            return result;
        }
    }
}