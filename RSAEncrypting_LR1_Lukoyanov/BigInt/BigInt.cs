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

        public BigInt(BigInt copy)
        {
            _digits = copy._digits;
            Sign = copy.Sign;
        }

        public BigInt(IEnumerable<int> digits)
        {
            _digits = digits.ToList();
            RemoveLeadingZeros();
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

        public BigInt(int x)
        {
            if (x < 0)
                Sign = Sign.Minus;

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

        public int GetDigit(int i) => i < Size ? _digits[Size - i - 1] : 0;

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

        public static BigInt Add(BigInt a, BigInt b)
        {
            if (a.Sign != b.Sign)
                return Subtraction(a, b);

            var digits = new List<int>();
            var maxLength = Math.Max(a.Size, b.Size);
            var tens = 0;
            for (var i = 0; i < maxLength; i++)
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

        public static BigInt Sub(BigInt a, BigInt b) => a + -b;

        private static BigInt Subtraction(BigInt a, BigInt b)
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
            for (var i = 0; i < maxLength; i++)
            {
                var units = (max.GetDigit(i) - min.GetDigit(i) - debt + 10) % 10;
                debt = max.GetDigit(i) - min.GetDigit(i) - debt < 0 ? 1 : 0;

                digits.Add(units);
            }

            digits.Reverse();

            return new BigInt(max.Sign, digits);
        }

        private static BigInt MultiplyOnDigit(BigInt a, int b)
        {
            switch (b)
            {
                case 0:
                    return Zero;
                case 1:
                    return new BigInt(a);
            }

            var result = Zero;
            for (var i = 0; i < b; i++)
                result += a;

            return result;
        }

        public static BigInt KaratsubaMultiplication(BigInt x, BigInt y)
        {
            var maxSize = Math.Max(x.Size, y.Size);

            if (maxSize < 2)
                return x * y;

            maxSize = maxSize / 2 + maxSize % 2;

            var m = Power10(One, maxSize);

            var b = x % m;
            var a = x / m;
            var c = y / m;
            var d = y % m;

            var z0 = KaratsubaMultiplication(a, c);
            var z1 = KaratsubaMultiplication(b, d);
            var z2 = KaratsubaMultiplication(a + b, c + d);


            return Power10(One, maxSize * 2) * z0 + z1 + (z2 - z1 - z0) * m;
        }


        private static BigInt Power10(BigInt number, int power) => new BigInt(number.Sign,
            number._digits.Concat(Enumerable.Repeat(0, power)).ToList());

        public static BigInt Mul(BigInt a, BigInt b)
        {
            var result = Zero;
            for (var i = 0; i < b.Size; i++)
            {
                var d = b.GetDigit(b.Size - i - 1);
                var buf = MultiplyOnDigit(a, d);
                buf = Power10(buf, b.Size - i - 1);
                result += buf;
            }

            result.Sign = a.Sign == b.Sign ? Sign.Plus : Sign.Minus;
            return result;
        }

        public static BigInt Div(BigInt dividend, BigInt divider)
        {
            var sign = dividend.Sign == divider.Sign ? Sign.Plus : Sign.Minus;
            var a = Abs(dividend);
            var b = Abs(divider);
            if (b == Zero)
                throw new DivideByZeroException();
            if (a < b)
                return Zero;

            var left = a._digits.GetRange(0, b.Size);
            var result = new List<int>();

            for (var i = b.Size; i <= a.Size; i++)
            {
                var times = 0;
                for (var j = 0; j <= 9; j++)
                {
                    if (b * new BigInt(j) <= new BigInt(left))
                        times = j;
                    else
                        break;
                }

                result.Add(times);

                if (i < a.Size)
                    left = (Power10(new BigInt(left) - b * new BigInt(times), 1) +
                            new BigInt(a.GetDigit(a.Size - i - 1)))._digits;
            }

            var answer = new BigInt(sign, result);

            return answer;
        }

        private static BigInt Abs(BigInt n) => new BigInt(Sign.Plus, n._digits);

        public static BigInt Mod(BigInt a, BigInt b)
        {
            if (Abs(a) < Abs(b))
                return a;

            var div = a / b;
            if (a * b < Zero)
                div -= One;
            return a - b * div;
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
            for (var i = maxLength;
                i >= 0;
                i--)
            {
                if (a.GetDigit(i) < b.GetDigit(i))
                    return -1;
                if (a.GetDigit(i) > b.GetDigit(i))
                    return 1;
            }

            return 0;
        }
        
        public static BigInt Inverse(BigInt a, BigInt n)
        {
            BigInt i = n, v = Zero, d = One;
            while (a > Zero)
            {
                BigInt t = i / a, x = a;
                a = i % x;
                i = x;
                x = d;
                d = v - t * x;
                v = x;
            }

            v %= n;
            if (v < Zero) v = (v + n) % n;
            return v;
        }

        public static BigInt GreatestCommonDivisor(BigInt a, BigInt b)
        {
            var x = new BigInt(a);
            var y = new BigInt(b);
            while (x != Zero && y != Zero)
            {
                if (x > y)
                    x %= y;
                else
                    y %= x;
            }

            return x + y;
        }


        public static BigInt ModOfPower(BigInt number, BigInt exponent, BigInt modulus) =>
            Pow(number, exponent) % modulus;


        public static BigInt Pow(BigInt n, BigInt pow)
        {
            var number = new BigInt(n);
            var b = new BigInt(1);
            var map = DecToBin(pow);
            for (var i = map.Size - 1; i >= 0; i--)
            {
                b = KaratsubaMultiplication(b, b);
                if (map.GetDigit(i) == 1)
                    b = KaratsubaMultiplication(b, number);
            }
            
            return b;
        }


        public static BigInt DecToBin(BigInt dec)
        {
            var digits = new List<int>();
            while (dec != Zero)
            {
                digits.Add((dec % new BigInt("2")).GetDigit(0));
                dec /= new BigInt("2");
            }

            digits.Reverse();

            return new BigInt(dec.Sign, digits);
        }

        public static BigInt operator -(BigInt a)
        {
            a.Sign = a.Sign == Sign.Plus ? Sign.Minus : Sign.Plus;
            return a;
        }

        public static BigInt operator +(BigInt a, BigInt b) => Add(a, b);

        public static BigInt operator -(BigInt a, BigInt b) => a + -b;

        public static BigInt operator *(BigInt a, BigInt b) => Mul(a, b);

        public static BigInt operator /(BigInt a, BigInt b) => Div(a, b);

        public static BigInt operator %(BigInt a, BigInt b) => Mod(a, b);

        public static bool operator <(BigInt a, BigInt b) => Comparison(a, b) < 0;

        public static bool operator >(BigInt a, BigInt b) => Comparison(a, b) > 0;

        public static bool operator <=(BigInt a, BigInt b) => Comparison(a, b) <= 0;

        public static bool operator >=(BigInt a, BigInt b) => Comparison(a, b) >= 0;

        public static bool operator ==(BigInt a, BigInt b) => Comparison(a, b) == 0;

        public static bool operator !=(BigInt a, BigInt b) => Comparison(a, b) != 0;

        public static implicit operator BigInt(int i) => new BigInt(i);
        
        public static implicit operator BigInt(byte b) => new BigInt(b);

        public override bool Equals(object obj) => obj is BigInt && this == (BigInt) obj;
    }
}