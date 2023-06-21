using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Reflection.Randomness
{
    public class FromDistribution : Attribute
    {
        public Type Type { get; private set; }
        public double[] Parameters { get; private set; }

        public FromDistribution(Type type, params double[] parameters)
        {
            Type = type;
            Parameters = parameters;
        }
    }

    public class Generator<TResult>
        where TResult : new()
    {
        private static readonly IContinuousDistribution[] _distributions;
        private static readonly Func<double[], TResult> _generator;

        static Generator()
        {
            var attributedProperties = typeof(TResult)
                .GetProperties()
                .Select(property =>
                {
                    var attribute = property.GetCustomAttribute<FromDistribution>();
                    return (property, attribute);
                })
                .Where(item => item.attribute != null)
                .ToArray();

            _distributions = new IContinuousDistribution[attributedProperties.Length];
            var properties = new PropertyInfo[attributedProperties.Length];

            for (int i = 0; i < attributedProperties.Length; i++)
            {
                _distributions[i] = CreateDistribution(attributedProperties[i].attribute);
                properties[i] = attributedProperties[i].property;
            }

            _generator = CreateGenerator(properties);
        }

        private static IContinuousDistribution CreateDistribution(FromDistribution fromDistribution)
        {
            // () => new Distribution(parameters[0], parameters[1], ...)

            var resultType = fromDistribution.Type;
            var arguments = new Expression[fromDistribution.Parameters.Length];
            var types = new Type[fromDistribution.Parameters.Length];

            for (int i = 0; i < fromDistribution.Parameters.Length; i++)
            {
                arguments[i] = Expression.Constant(fromDistribution.Parameters[i]);
                types[i] = fromDistribution.Parameters[i].GetType();
            }

            var ctor = resultType.GetConstructor(types);
            if (ctor is null)
                throw new ArgumentException($"No suitable ctor for type {resultType.FullName}");

            var body = Expression.MemberInit(
                Expression.New(ctor, arguments));
            var lambda = Expression.Lambda<Func<IContinuousDistribution>>(body);

            return lambda.Compile().Invoke();
        }

        private static Func<double[], TResult> CreateGenerator(PropertyInfo[] properties)
        {
            // values => new Distribution { Property1 = values[0], Property2 = values[1], ... };

            var arg = Expression.Parameter(typeof(double[]), "values");
            var bindings = new List<MemberBinding>();
            for (int i = 0; i < properties.Length; i++)
                bindings.Add(Expression.Bind(properties[i],
                    Expression.ArrayIndex(arg, Expression.Constant(i))));
            var body = Expression.MemberInit(
                Expression.New(typeof(TResult).GetConstructor(new Type[0])), bindings);
            var lambda = Expression.Lambda<Func<double[], TResult>>(body, arg);
            return lambda.Compile();
        }

        public TResult Generate(Random random)
        {
            var values = _distributions
                .Select(x => x.Generate(random))
                .ToArray();

            return _generator(values);
        }
    }
}