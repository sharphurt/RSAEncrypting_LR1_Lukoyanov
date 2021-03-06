using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace RSAEncrypting_LR1_Lukoyanov
{
    public class BigInt
    {
        private readonly List<int> _digits = new List<int>();

        public static BigInt Zero => new BigInt(0);
        public static BigInt One => new BigInt(1);

        public int Size => _digits.Count;

        public Sign Sign { get; private set; } = Sign.Plus;

        public BigInt(IEnumerable<int> digits)
        {
            _digits = digits.ToList();
            RemoveLeadingZeros();
            _digits.Reverse();
        }

        public BigInt(Sign sign, List<int> digits)
        {
            Sign = sign;
            _digits = digits;
            RemoveLeadingZeros();
        }

        public BigInt(string str)
        {
            if (str.StartsWith("-"))
            {
                Sign = Sign.Minus;
                str = str.Substring(1);
            }
            else if (str.StartsWith("+"))
            {
                Sign = Sign.Plus;
                str = str.Substring(1);
            }
            else if (!char.IsDigit(str[0]))
                throw new ArgumentException("Invalid number format");

            if (str.Any(c => !char.IsDigit(c)))
                throw new ArgumentException("Invalid number format");

            foreach (var c in str)
                _digits.Add(c - '0');

            RemoveLeadingZeros();
        }

        public BigInt(uint x)
        {
            _digits.AddRange(GetDigits(x));
        }

        public BigInt(int x)
        {
            if (x < 0)
            {
                Sign = Sign.Minus;
            }

            _digits.AddRange(GetDigits((uint) Math.Abs(x)));
        }

        private IEnumerable<int> GetDigits(uint num) => num.ToString().Select(c => c - '0');

        private void RemoveLeadingZeros()
        {
            if (Size < 2)
                return;

            var end = 0;
            for (; end < Size; end++)
                if (_digits[end] != 0)
                    break;
            
            _digits.RemoveRange(0, end);
        }

        public static BigInt Exp(int val, int exp)
        {
            var bigInt = Zero;
            bigInt.SetDigit(exp, val);
            bigInt.RemoveLeadingZeros();

            return bigInt;
        }


        public int GetDigit(int i) => i < Size ? _digits[i] : 0;

        public void SetDigit(int i, int b)
        {
            while (_digits.Count <= i)
            {
                _digits.Add(0);
            }

            _digits[i] = b;
        }

        public override string ToString() =>
            this == Zero ? "0" : $"{_digits[0] * (int) Sign}{string.Join("", _digits.Skip(1))}";

        private static BigInt Add(BigInt a, BigInt b)
        {
            var digits = new List<int>();
            var maxLength = Math.Max(a.Size, b.Size);
            var tens = 0;
            for (var i = maxLength - 1; i >= 0; i--)
            {
                var units = (a.GetDigit(i) + b.GetDigit(i) + tens) % 10;
                tens = (a.GetDigit(i) + b.GetDigit(i) + tens) / 10;
                digits.Add(units);
            }

            if (tens > 0)
                digits.Add(tens);

            digits.Reverse();

            return new BigInt(a.Sign, digits);
        }

        private static BigInt Subtract(BigInt a, BigInt b)
        {
            var digits = new List<int>();

            var max = Zero;
            var min = Zero;

            var comparison = Comparison(a, b, true);

            switch (comparison)
            {
                case -1:
                    min = a;
                    max = b;
                    break;
                case 0:
                    return Zero;
                case 1:
                    min = b;
                    max = a;
                    break;
            }

            var maxLength = Math.Max(a.Size, b.Size);

            var debt = 0;
            for (var i = maxLength - 1; i >= 0; i--)
            {
                var units = (max.GetDigit(i) - min.GetDigit(i) - debt + 10) % 10;
                debt = max.GetDigit(i) - min.GetDigit(i) - debt < 0 ? 1 : 0;

                digits.Add(units);
            }

            digits.Reverse();

            return new BigInt(max.Sign, digits);
        }

        private static BigInt Multiply(BigInt a, BigInt b)
        {
            var retValue = Zero;

            for (var i = 0; i < a.Size; i++)
            {
                for (int j = 0, carry = 0; j < b.Size || carry > 0; j++)
                {
                    var cur = retValue.GetDigit(i + j) + a.GetDigit(i) * b.GetDigit(j) + carry;
                    retValue.SetDigit(i + j, (int) (cur % 10));
                    carry = cur / 10;
                }
            }

            retValue.Sign = a.Sign == b.Sign ? Sign.Plus : Sign.Minus;
            return retValue;
        }

        private static BigInt Div(BigInt a, BigInt b)
        {
            var retValue = Zero;
            var curValue = Zero;

            for (var i = a.Size - 1; i >= 0; i--)
            {
                curValue += Exp(a.GetDigit(i), i);

                var x = 0;
                var l = 0;
                var r = 10;
                while (l <= r)
                {
                    var m = (l + r) / 2;
                    var cur = b * Exp((int) m, i);
                    if (cur <= curValue)
                    {
                        x = m;
                        l = m + 1;
                    }
                    else
                    {
                        r = m - 1;
                    }
                }

                retValue.SetDigit(i, (int) (x % 10));
                var t = b * Exp((int) x, i);
                curValue = curValue - t;
            }

            retValue.RemoveLeadingZeros();

            retValue.Sign = a.Sign == b.Sign ? Sign.Plus : Sign.Minus;
            return retValue;
        }

        private static BigInt Mod(BigInt a, BigInt b)
        {
            var retValue = Zero;

            for (var i = a.Size - 1; i >= 0; i--)
            {
                retValue += Exp(a.GetDigit(i), i);

                var x = 0;
                var l = 0;
                var r = 10;

                while (l <= r)
                {
                    var m = (l + r) >> 1;
                    var cur = b * Exp((int) m, i);
                    if (cur <= retValue)
                    {
                        x = m;
                        l = m + 1;
                    }
                    else
                    {
                        r = m - 1;
                    }
                }

                retValue -= b * Exp((int) x, i);
            }

            retValue.RemoveLeadingZeros();

            retValue.Sign = a.Sign == b.Sign ? Sign.Plus : Sign.Minus;
            return retValue;
        }

        private static int Comparison(BigInt a, BigInt b, bool ignoreSign = false)
        {
            return CompareSign(a, b, ignoreSign);
        }

        private static int CompareSign(BigInt a, BigInt b, bool ignoreSign = false)
        {
            if (!ignoreSign)
            {
                if (a.Sign < b.Sign)
                    return -1;
                if (a.Sign > b.Sign)
                    return 1;
            }

            return CompareSize(a, b);
        }

        private static int CompareSize(BigInt a, BigInt b)
        {
            if (a.Size < b.Size)
                return -1;
            return a.Size > b.Size ? 1 : CompareDigits(a, b);
        }

        private static int CompareDigits(BigInt a, BigInt b)
        {
            var maxLength = Math.Max(a.Size, b.Size);
            for (var i = 0; i < maxLength; i++)
            {
                if (a.GetDigit(i) < b.GetDigit(i))
                    return -1;
                if (a.GetDigit(i) > b.GetDigit(i))
                    return 1;
            }

            return 0;
        }

        // унарный минус(изменение знака числа)
        public static BigInt operator -(BigInt a)
        {
            a.Sign = a.Sign == Sign.Plus ? Sign.Minus : Sign.Plus;
            return a;
        }

        //сложение
        public static BigInt operator +(BigInt a, BigInt b) => a.Sign == b.Sign
            ? Add(a, b)
            : Subtract(a, b);

        //вычитание
        public static BigInt operator -(BigInt a, BigInt b) => a + -b;

        //умножение
        public static BigInt operator *(BigInt a, BigInt b) => Multiply(a, b);

        //целочисленное деление(без остатка)
        public static BigInt operator /(BigInt a, BigInt b) => Div(a, b);

        //остаток от деления
        public static BigInt operator %(BigInt a, BigInt b) => Mod(a, b);

        public static bool operator <(BigInt a, BigInt b) => Comparison(a, b) < 0;

        public static bool operator >(BigInt a, BigInt b) => Comparison(a, b) > 0;

        public static bool operator <=(BigInt a, BigInt b) => Comparison(a, b) <= 0;

        public static bool operator >=(BigInt a, BigInt b) => Comparison(a, b) >= 0;

        public static bool operator ==(BigInt a, BigInt b) => Comparison(a, b) == 0;

        public static bool operator !=(BigInt a, BigInt b) => Comparison(a, b) != 0;

        public override bool Equals(object obj) => !(obj is BigInt) ? false : this == (BigInt) obj;
    }
}