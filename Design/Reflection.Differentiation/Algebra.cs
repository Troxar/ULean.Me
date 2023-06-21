using System;
using System.Linq.Expressions;
 
namespace Reflection.Differentiation
{
    public static class Algebra
    {
        public static Expression<Func<double, double>> Differentiate(Expression<Func<double, double>> expression)
        {
            return Expression.Lambda<Func<double, double>>(
                Differentiate(expression.Body), 
                expression.Parameters);
        }

        private static Expression Differentiate(Expression body)
        {
            if (body is ConstantExpression constantExpression)
                return Differentiate(constantExpression);
            if (body is ParameterExpression parameterExpression)
                return Differentiate(parameterExpression);
            if (body is BinaryExpression binaryExpression)
                return Differentiate(binaryExpression);
            if (body is MethodCallExpression methodCallExpression)
                return Differentiate(methodCallExpression);
            throw new ArgumentException($"{body} expression is not supported");
        }

        private static Expression Differentiate(ConstantExpression _)
        {
            return Expression.Constant(0d);
        }

        private static Expression Differentiate(ParameterExpression _)
        {
            return Expression.Constant(1d);
        }

        private static Expression Differentiate(BinaryExpression expression)
        {
            if (expression.NodeType == ExpressionType.Add)
                return Expression.Add(
                    Differentiate(expression.Left),
                    Differentiate(expression.Right));

            if (expression.NodeType == ExpressionType.Multiply)
                return Expression.Add(
                    Expression.Multiply(Differentiate(expression.Left), expression.Right),
                    Expression.Multiply(expression.Left, Differentiate(expression.Right)));

            throw new ArgumentException($"{expression} expression is not supported");
        }

        private static Expression Differentiate(MethodCallExpression expression)
        {
            if (expression.Method.Name == "Sin")
            {
                var argument = expression.Arguments[0];
                return Expression.Multiply(
                    Expression.Call(typeof(Math).GetMethod("Cos", new[] { typeof(double) }), argument),
                    Differentiate(argument));
            }

            if (expression.Method.Name == "Cos")
            {
                var argument = expression.Arguments[0];
                return Expression.Multiply(
                    Expression.Negate(
                        Expression.Call(typeof(Math).GetMethod("Sin", new[] { typeof(double) }), argument)),
                    Differentiate(argument));
            }

            throw new ArgumentException($"{expression.Method.Name} method is not supported");
        }
    }
}