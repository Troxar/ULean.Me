using System;
using System.Linq;
using System.Reflection;

namespace MyPhotoshop
{
    public class StaticParametersHandler<TParameters> : IParametersHandler<TParameters>
        where TParameters : IParameters, new()
    {
        private static readonly PropertyInfo[] _properties;
        private static readonly ParameterInfo[] _parameters;

        static StaticParametersHandler()
        {
            var properties = typeof(TParameters).GetProperties();
            _properties = properties
                .Where(x => x.GetCustomAttribute<ParameterInfo>() != null)
                .ToArray();
            _parameters = properties
                .Select(x => x.GetCustomAttribute<ParameterInfo>())
                .Where(x => x != null)
                .ToArray();
        }

        public TParameters CreateParameters(double[] values)
        {
            if (_properties.Length != values.Length)
                throw new InvalidOperationException();

            var parameters = new TParameters();
            for (int i = 0; i < _properties.Length; i++)
                _properties[i].SetValue(parameters, values[i]);

            return parameters;
        }

        public ParameterInfo[] GetDescription()
        {
            return _parameters;
        }
    }
}