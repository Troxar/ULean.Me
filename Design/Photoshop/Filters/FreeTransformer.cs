using System;
using System.Drawing;

namespace MyPhotoshop
{
    public class FreeTransformer : ITransformer<EmptyParameters>
    {
        private readonly Func<Size, Size> _sizeTransformer;
        private readonly Func<Point, Size, Point> _pointTransformer;

        public Size OriginalSize { get; private set; }

        public FreeTransformer(Func<Size, Size> sizeTransformer,
            Func<Point, Size, Point> pointTransformer)
        {
            _sizeTransformer = sizeTransformer;
            _pointTransformer = pointTransformer;
        }

        public void Prepare(Size size, EmptyParameters parameters)
        {
            OriginalSize = size;
            ResultSize = _sizeTransformer(size);
        }

        public Size ResultSize { get; private set; }

        public Point? MapPoint(Point point)
        {
            return _pointTransformer(point, OriginalSize);
        }
    }
}