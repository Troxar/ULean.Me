namespace MyPhotoshop
{
    public abstract class ParametrizedFilter<TParameters> : IFilter
        where TParameters : IParameters, new()
    {
        public ParameterInfo[] GetParameters()
        {
            return new TParameters().GetDescription();
        }

        public abstract Photo Process(Photo photo, TParameters parameters);

        public Photo Process(Photo photo, double[] values)
        {
            var parameters = new TParameters();
            parameters.Parse(values);
            return Process(photo, parameters);
        }
    }
}