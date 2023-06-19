using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MyPhotoshop
{
    public class ExpressionsParametersHandler<TParameters> : IParametersHandler<TParameters>
        where TParameters : IParameters, new()
    {
        private static readonly PropertyInfo[] _properties;
        private static readonly ParameterInfo[] _parameters;
        private static Func<double[], TParameters> _parser;

        static ExpressionsParametersHandler()
        {
            var properties = typeof(TParameters).GetProperties();
            _properties = properties
                .Where(x => x.GetCustomAttribute<ParameterInfo>() != null)
                .ToArray();
            _parameters = properties
                .Select(x => x.GetCustomAttribute<ParameterInfo>())
                .Where(x => x != null)
                .ToArray();

            // values => new LighteningParameters { Coefficient = values[0] };
            var arg = Expression.Parameter(typeof(double[]), "values");
            var bindings = new List<MemberBinding>();
            for (int i = 0; i < _properties.Length; i++)
                bindings.Add(Expression.Bind(_properties[i],
                    Expression.ArrayIndex(arg, Expression.Constant(i))));
            var body = Expression.MemberInit(
                Expression.New(typeof(TParameters).GetConstructor(new Type[0])), bindings);
            var lambda = Expression.Lambda<Func<double[], TParameters>>(body, arg);
            _parser = lambda.Compile();
        }

        public TParameters CreateParameters(double[] values)
        {
            return _parser(values);
        }

        public ParameterInfo[] GetDescription()
        {
            return _parameters;
        }
    }
}