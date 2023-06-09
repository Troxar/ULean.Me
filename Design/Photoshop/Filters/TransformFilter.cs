using System;
using System.Drawing;

namespace MyPhotoshop
{
    public class TransformFilter : ParametrizedFilter<TransformParameters>
    {
        private string _name;
        private Func<Size, Size> _sizeTransform;
        private Func<Point, Size, Point> _pointTransform;

        public TransformFilter(string name, 
            Func<Size, Size> sizeTransform,
            Func<Point, Size, Point> pointTransform)
        {
            _name = name;
            _sizeTransform = sizeTransform;
            _pointTransform = pointTransform;
        }

        public override string ToString() => _name;

        public override Photo Process(Photo original, TransformParameters parameters)
        {
            var originalSize = new Size(original.Width, original.Height);
            var size = _sizeTransform(originalSize);
            var result = new Photo(size.Width, size.Height);

            for (int x = 0; x < result.Width; x++)
                for (int y = 0; y < result.Height; y++)
                {
                    var point = _pointTransform(new Point(x, y), originalSize);
                    result[x, y] = original[point.X, point.Y];
                }

            return result;
        }
    }
}