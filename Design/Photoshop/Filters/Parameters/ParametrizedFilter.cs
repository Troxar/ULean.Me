namespace MyPhotoshop
{
    public abstract class ParametrizedFilter<TParameters> : IFilter
        where TParameters : IParameters, new()
    {
        private IParametersHandler<TParameters> _handler = new ExpressionsParametersHandler<TParameters>();

        public ParameterInfo[] GetParameters()
        {
            return _handler.GetDescription();
        }

        public abstract Photo Process(Photo photo, TParameters parameters);

        public Photo Process(Photo photo, double[] values)
        {
            var parameters = _handler.CreateParameters(values);
            return Process(photo, parameters);
        }
    }
}