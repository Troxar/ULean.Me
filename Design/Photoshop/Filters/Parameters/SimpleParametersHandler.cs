using System;
using System.Linq;
using System.Reflection;

namespace MyPhotoshop
{
    public class SimpleParametersHandler<TParameters> : IParametersHandler<TParameters>
        where TParameters : IParameters, new()
    {
        public TParameters CreateParameters(double[] values)
        {
            var parameters = new TParameters();
            var properties = parameters
                .GetType()
                .GetProperties()
                .Where(x => x.GetCustomAttribute<ParameterInfo>() != null)
                .ToArray();

            if (properties.Length != values.Length)
                throw new InvalidOperationException();

            for (int i = 0; i < properties.Length; i++)
                properties[i].SetValue(parameters, values[i]);

            return parameters;
        }

        public ParameterInfo[] GetDescription()
        {
            return typeof(TParameters)
                .GetProperties()
                .Select(x => x.GetCustomAttribute<ParameterInfo>())
                .Where(x => x != null)
                .ToArray();
        }
    }
}