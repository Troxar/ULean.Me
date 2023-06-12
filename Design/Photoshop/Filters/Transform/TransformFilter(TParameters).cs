using System.Drawing;

namespace MyPhotoshop
{
    public class TransformFilter<TParameters> : ParametrizedFilter<TParameters>
        where TParameters : IParameters, new()
    {
        private readonly string _name;
        private readonly ITransformer<TParameters> _transformer;

        public TransformFilter(string name, ITransformer<TParameters> transformer)
        {
            _name = name;
            _transformer = transformer;
        }

        public override string ToString() => _name;

        public override Photo Process(Photo original, TParameters parameters)
        {
            var originalSize = new Size(original.Width, original.Height);
            _transformer.Prepare(originalSize, parameters);
            var result = new Photo(_transformer.ResultSize.Width, _transformer.ResultSize.Height);

            for (int x = 0; x < result.Width; x++)
                for (int y = 0; y < result.Height; y++)
                {
                    var originalPoint = new Point(x, y);
                    var point = _transformer.MapPoint(originalPoint);
                    if (point.HasValue)
                        result[x, y] = original[point.Value.X, point.Value.Y];
                }

            return result;
        }
    }
}