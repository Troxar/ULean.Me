using System;
using System.Linq;
using System.Reflection;

namespace MyPhotoshop
{
    public static class ParametersExtensions
    {
        public static ParameterInfo[] GetDescription(this IParameters parameters)
        {
            return parameters
                .GetType()
                .GetProperties()
                .Select(x => x.GetCustomAttribute<ParameterInfo>())
                .Where(x => x != null)
                .ToArray();
        }

        public static void Parse(this IParameters parameters, double[] values)
        {
            if (values is null)
                throw new NullReferenceException(nameof(values));

            var properties = parameters
                .GetType()
                .GetProperties()
                .Where(x => x.GetCustomAttribute<ParameterInfo>() != null)
                .ToArray();

            if (properties.Length != values.Length)
                throw new InvalidOperationException();

            for (int i = 0; i < properties.Length; i++)
                properties[i].SetValue(parameters, values[i]);
        }
    }
}