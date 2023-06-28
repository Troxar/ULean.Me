using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Ddd.Infrastructure
{
    /// <summary>
    /// Базовый класс для всех Value типов.
    /// </summary>
    public abstract class ValueType<T> where T : class
    {
        private static Func<T, string> _toStringGenerator;
        private static Func<T, T, bool> _equalsDecider;
        private static Func<T, int> _hashCodeGenerator;

        private int? _hashCode;

        static ValueType()
        {
            var bindingFlags = BindingFlags.Instance | BindingFlags.Public;
            var publicProperties = typeof(T)
                .GetProperties(bindingFlags)
                .OrderBy(prop => prop.Name)
                .ToArray();

            _toStringGenerator = CreateToStringGenerator(publicProperties);
            _equalsDecider = CreateEqualsDecider(publicProperties);
            _hashCodeGenerator = CreateHashCodeGenerator(publicProperties);
        }

        #region ToStringGenerator

        private static Func<T, string> CreateToStringGenerator(PropertyInfo[] properties)
        {
            // (instance) => "TypeName(Property1: Value1; Property2: Value2; ...)"

            Type type = typeof(T);
            var parameterExpr = Expression.Parameter(type, "instance");

            var joinArgs = new Expression[2];
            joinArgs[0] = Expression.Constant("; ");
            joinArgs[1] = Expression.NewArrayInit(typeof(string),
                GetArrayExpressionForStringGenerator(properties, parameterExpr));

            var joinExpr = Expression.Call(
                typeof(string).GetMethod("Join", new Type[] { typeof(string), typeof(string[]) }),
                joinArgs);

            return Expression.Lambda<Func<T, string>>(
                 GetConcatExpressionForToStringGenerator(type, joinExpr), parameterExpr)
                .Compile();
        }

        private static MethodCallExpression GetConcatExpressionForToStringGenerator(
            Type type, MethodCallExpression joinExpr)
        {
            var concatArgs = new Expression[4];
            concatArgs[0] = Expression.Constant(type.Name);
            concatArgs[1] = Expression.Constant("(");
            concatArgs[2] = joinExpr;
            concatArgs[3] = Expression.Constant(")");

            return Expression.Call(
                typeof(string).GetMethod("Concat",
                    new[] { typeof(string), typeof(string), typeof(string), typeof(string) }),
                concatArgs);
        }

        private static Expression[] GetArrayExpressionForStringGenerator(
            PropertyInfo[] properties, ParameterExpression parameterExpr)
        {
            var arrayArgs = new Expression[properties.Length];
            for (int i = 0; i < properties.Length; i++)
            {
                var propertyExpr = Expression.Property(parameterExpr, properties[i]);

                var concatArgs = new Expression[2];
                concatArgs[0] = Expression.Constant($"{properties[i].Name}: ");
                concatArgs[1] = properties[i].PropertyType.IsValueType
                    ? Expression.Call(propertyExpr, "ToString", null)
                    : (Expression)Expression.Condition(
                        Expression.Equal(propertyExpr, Expression.Constant(null)),
                        Expression.Constant(string.Empty),
                        Expression.Call(propertyExpr, "ToString", null));

                arrayArgs[i] = Expression.Call(
                    typeof(string).GetMethod("Concat", new[] { typeof(string), typeof(string) }),
                    concatArgs);
            }

            return arrayArgs;
        }

        #endregion

        #region EqualsDecider

        private static Func<T, T, bool> CreateEqualsDecider(PropertyInfo[] properties)
        {
            // (left, right) => true && (Equals(left.Property1, right.Property1)
            //      && (Equals(left.Property2, right.Property2)
            //          && ... ))

            Type type = typeof(T);
            var parameterLeftExpr = Expression.Parameter(type, "left");
            var parameterRightExpr = Expression.Parameter(type, "right");

            if (properties.Length == 0)
                return Expression.Lambda<Func<T, T, bool>>(Expression.Constant(true),
                    new[] { parameterLeftExpr, parameterRightExpr })
                    .Compile();

            var andExpr = GetAndExpressionForEqualsDecider(properties,
                parameterLeftExpr, parameterRightExpr);

            return Expression.Lambda<Func<T, T, bool>>(andExpr,
                new[] { parameterLeftExpr, parameterRightExpr })
                .Compile();
        }

        private static BinaryExpression GetAndExpressionForEqualsDecider(
            PropertyInfo[] properties, ParameterExpression parameterLeftExpr,
            ParameterExpression parameterRightExpr)
        {
            BinaryExpression andExpr = null;
            var objectEquals = typeof(object).GetMethod("Equals", new[] { typeof(object), typeof(object) });

            for (int i = 0; i < properties.Length; i++)
            {
                var propertyLeftExpr = Expression.Property(parameterLeftExpr, properties[i]);
                var propertyRightExpr = Expression.Property(parameterRightExpr, properties[i]);
                var equalsExpr = properties[i].PropertyType.IsValueType
                    ? Expression.Call(propertyLeftExpr,
                        properties[i].PropertyType.GetMethod("Equals", new[] { properties[i].PropertyType }),
                        propertyRightExpr)
                    : Expression.Call(
                        objectEquals,
                        new[] { propertyLeftExpr, propertyRightExpr });
                andExpr = i == 0
                    ? Expression.AndAlso(Expression.Constant(true), equalsExpr)
                    : Expression.AndAlso(andExpr, equalsExpr);
            }

            return andExpr;
        }

        #endregion

        #region HashCodeGenerator

        private static Func<T, int> CreateHashCodeGenerator(PropertyInfo[] properties)
        {
            //int seed = 397;
            //_hashCode = seed;
            //foreach (var prop in _publicProperties)
            //    _hashCode = unchecked((_hashCode ^ prop.GetValue(this).GetHashCode()) * seed);

            var parameterExpr = Expression.Parameter(typeof(T), "instance");
            var seedExpr = Expression.Constant(397);

            if (properties.Length == 0)
                return Expression.Lambda<Func<T, int>>(seedExpr,
                    new[] { parameterExpr })
                    .Compile();

            var hashExpr = GetHashExpressionForHashCodeGenerator(
                properties, parameterExpr, seedExpr);

            return Expression.Lambda<Func<T, int>>(hashExpr,
                new[] { parameterExpr })
                .Compile();
        }

        private static Expression GetHashExpressionForHashCodeGenerator(
            PropertyInfo[] properties, ParameterExpression parameterExpr, ConstantExpression seedExpr)
        {
            Expression hashExpr = seedExpr;

            for (int i = 0; i < properties.Length; i++)
            {
                var propertyExpr = Expression.Property(parameterExpr, properties[i]);

                var callExpr = properties[i].PropertyType.IsValueType
                    ? Expression.Call(propertyExpr, "GetHashCode", null)
                    : (Expression)Expression.Condition(
                        Expression.Equal(propertyExpr, Expression.Constant(null)),
                        Expression.Constant(0),
                        Expression.Call(propertyExpr, "GetHashCode", null));

                var xorExpr = Expression.ExclusiveOr(hashExpr, callExpr);
                hashExpr = Expression.Multiply(xorExpr, seedExpr);
            }

            return hashExpr;
        }

        #endregion

        protected bool Equals(ValueType<T> other)
        {
            return _equalsDecider(this as T, other as T);
        }

        public bool Equals(T other)
        {
            return Equals(this, other);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ValueType<T>)obj);
        }

        public override int GetHashCode()
        {
            if (_hashCode.HasValue)
                return _hashCode.Value;

            _hashCode = _hashCodeGenerator(this as T);

            return _hashCode.Value;
        }

        public override string ToString()
        {
            return _toStringGenerator(this as T);
        }
    }
}