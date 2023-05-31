using System;

namespace Incapsulation.RationalNumbers
{
    public class Rational
    {
        public int Numerator { get; }
        public int Denominator { get; }
        public bool IsNan { get; }

        public Rational(int num, int den = 1, bool isNan = false)
        {
            var tors = CheckValues(num, den);
            Numerator = tors.Numerator;
            Denominator = tors.Denominator;
            IsNan = isNan || den == 0;
        }

        private (int Numerator, int Denominator) CheckValues(int num, int den)
        {
            if (num == 0)
                return (num, 1);

            while (num % 2 == 0 && den % 2 == 0)
            {
                num /= 2;
                den /= 2;
            }

            if (num % 2 == 0)
            {
                var quotient = num / 2;
                if (den % quotient == 0)
                {
                    num = 2;
                    den /= quotient;
                }
            }

            return CheckNegativeDenominator(num, den);
        }

        private (int Numerator, int Denominator) CheckNegativeDenominator(int num, int den)
        {
            return den < 0
                ? (-num, -den)
                : (num, den);
        }

        public static Rational operator +(Rational left, Rational right)
        {
            return new Rational(
                left.Numerator * right.Denominator + right.Numerator * left.Denominator,
                left.Denominator * right.Denominator);
        }

        public static Rational operator -(Rational left, Rational right)
        {
            return new Rational(
                left.Numerator * right.Denominator - right.Numerator * left.Denominator,
                left.Denominator * right.Denominator);
        }

        public static Rational operator *(Rational left, Rational right)
        {
            return new Rational(
                left.Numerator * right.Numerator,
                left.Denominator * right.Denominator);
        }

        public static Rational operator /(Rational left, Rational right)
        {
            return new Rational(
                left.Numerator * right.Denominator,
                right.Numerator * left.Denominator,
                left.IsNan || right.IsNan);
        }

        public static implicit operator double(Rational rational) 
        {
            return rational.IsNan
                ? double.NaN
                : (double)rational.Numerator / rational.Denominator;
        }

        public static implicit operator Rational(int number)
        {
            return new Rational(number);
        }

        public static explicit operator int(Rational rational)
        {
            if (rational.IsNan)
                return 0;
            if (rational.Numerator % rational.Denominator != 0)
                throw new InvalidCastException();
            return rational.Numerator / rational.Denominator;
        }
    }
}