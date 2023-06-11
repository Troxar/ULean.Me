using System.Drawing;

namespace MyPhotoshop
{
    public interface ITransformer<TParameters>
    {
        void Prepare(Size size, TParameters parameters);
        Size ResultSize { get; }
        Point? MapPoint(Point point);
    }
}