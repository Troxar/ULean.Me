using System.Drawing;

namespace MyPhotoshop
{
    public class FlipTransformer : ITransformer<FlipParameters>
    {
        public Size ResultSize { get; private set; }

        public void Prepare(Size size, FlipParameters parameters)
        {
            ResultSize = size;
        }

        public Point? MapPoint(Point point)
        {
            return new Point(ResultSize.Width - point.X - 1, point.Y);
        }
    }
}