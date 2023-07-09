using System.Collections.Generic;
using System.Linq;

namespace SRP.ControlDigit
{
    public static class Extensions
    {
        // Вспомогательные методы-расширения поместите в этот класс.
        // Они должны быть понятны и потенциально полезны вне контекста задачи расчета контрольных разрядов.

        public static char ToChar(this int value)
        {
            return (char)('0' | value);
        }

        public static int DiffToRate(this int sum, int rate)
        {
            int result = sum % rate;
            if (result != 0)
                result = rate - result;
            return result;
        }

        public static IEnumerable<int> ToDigits(this long number)
        {
            do
            {
                int digit = (int)(number % 10);
                yield return digit;
                number /= 10;
            }
            while (number > 0);
        }

        public static char ToIsbn10Result(this int number)
        {
            return number == 10 ? 'X' : number.ToChar();
        }
    }

    public static class ControlDigitAlgo
    {
        public static int Upc(long number)
        {
            int factor = 3;
            return number.ToDigits().Sum(digit =>
                {
                    int f = factor;
                    factor = 4 - factor;
                    return f * digit;
                }).DiffToRate(10);
        }

        public static char Isbn10(long number)
        {
            int factor = 2;
            return number.ToDigits().Sum(digit =>
                {
                    int f = factor++;
                    return digit * f;
                }).DiffToRate(11).ToIsbn10Result();
        }

        public static int Luhn(long number)
        {
            bool odd = false;
            return number.ToDigits().Sum(digit =>
                {
                    odd = !odd;
                    int value = (odd ? 2 : 1) * digit;
                    return value > 9 ? value - 9 : value;
                }).DiffToRate(10);
        }
    }
}