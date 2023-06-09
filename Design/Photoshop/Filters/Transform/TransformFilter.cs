using System;
using System.Drawing;

namespace MyPhotoshop
{
    public class TransformFilter<TParameters> : ParametrizedFilter<TParameters>
        where TParameters : IParameters, new()
    {
        private string _name;
        private Func<Size, TParameters, Size> _sizeTransform;
        private Func<Point, Size, TParameters, Point?> _pointTransform;

        public TransformFilter(string name, 
            Func<Size, TParameters, Size> sizeTransform,
            Func<Point, Size, TParameters, Point?> pointTransform)
        {
            _name = name;
            _sizeTransform = sizeTransform;
            _pointTransform = pointTransform;
        }

        public override string ToString() => _name;

        public override Photo Process(Photo original, TParameters parameters)
        {
            var originalSize = new Size(original.Width, original.Height);
            var size = _sizeTransform(originalSize, parameters);
            var result = new Photo(size.Width, size.Height);

            for (int x = 0; x < result.Width; x++)
                for (int y = 0; y < result.Height; y++)
                {
                    var originalPoint = new Point(x, y);
                    var point = _pointTransform(originalPoint, originalSize, parameters);
                    if (point.HasValue)
                        result[x, y] = original[point.Value.X, point.Value.Y];
                }

            return result;
        }
    }
}