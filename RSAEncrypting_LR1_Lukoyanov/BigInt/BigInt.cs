using System;
using System.Collections.Generic;
using System.Linq;

namespace RSAEncrypting_LR1_Lukoyanov.BigInt
{
    public class BigInt
    {
        private readonly List<int> _digits = new List<int>();

        public static BigInt Zero => new BigInt(0);
        public static BigInt One => new BigInt(1);

        public int Size => _digits.Count;
        public Sign Sign { get; } = Sign.Plus;

        #region Constructors
        
        /// <summary>
        /// Конструктор BigInt из существующего объекта класса
        /// </summary>
        /// <param name="copy">Объект класса BigInt</param>
        private BigInt(BigInt copy)
        {
            _digits = copy._digits;
            Sign = copy.Sign;
        }

        /// <summary>
        /// Конструктор положительного BigInt из коллекции цифр
        /// </summary>
        /// <param name="digits">Коллекция цифр числа</param>
        private BigInt(IEnumerable<int> digits)
        {
            Sign = Sign.Plus;
            _digits = digits.ToList();
            RemoveLeadingZeros();
        }

        /// <summary>
        /// Конструктор BigInt из коллекции цифр с указанием знака
        /// </summary>
        /// <param name="sign">Знак числа</param>
        /// <param name="digits">Коллекция, содержащая цифры числа</param>
        private BigInt(Sign sign, IEnumerable<int> digits)
        {
            Sign = sign;
            _digits = digits.ToList();
            RemoveLeadingZeros();
        }

        /// <summary>
        /// Конструктор BigInt из строкового представления числа
        /// </summary>
        /// <param name="str">Строковое представление числа</param>
        /// <exception cref="ArgumentException">Число имеет неверный формат</exception>
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
        
        /// <summary>
        /// Конструктор BigInt из числа типа Int32
        /// </summary>
        /// <param name="x">Число для представления в BigInt</param>
        private BigInt(int x)
        {
            if (x < 0)
                Sign = Sign.Minus;

            _digits.AddRange(GetDigits((uint) Math.Abs(x)));
        }

        #endregion

        #region Ariphmetic Operations
        
        /// <summary>
        /// Сложение двух чисел (столбиком)
        /// </summary>
        /// <param name="a">Первое слагаемое</param>
        /// <param name="b">Второе слагаемое</param>
        /// <returns></returns>
        public static BigInt Add(BigInt a, BigInt b)
        {
            if (a.Sign != b.Sign)
                return Subtraction(a, b);

            var digits = new List<int>();
            var maxLength = Math.Max(a.Size, b.Size);
            var tens = 0;
            for (var i = 0; i < maxLength; i++)
            {
                var units = (a.GetDigitFromEnd(i) + b.GetDigitFromEnd(i) + tens) % 10;
                tens = (a.GetDigitFromEnd(i) + b.GetDigitFromEnd(i) + tens) / 10;
                digits.Add(units);
            }

            if (tens > 0)
                digits.Add(tens);
            digits.Reverse();
            
            return new BigInt(a.Sign, digits);
        }

        /// <summary>
        /// Операция вычитания двух чисел (столбиком)
        /// </summary>
        /// <param name="a">Уменьшаемое </param>
        /// <param name="b">Вычитаемое</param>
        /// <returns>Разность уменьшаемого и вычитаемого</returns>
        public static BigInt Sub(BigInt a, BigInt b) => a + -b;

        /// <summary>
        /// Вычитание из большего числа меньшего (столбиком)
        /// </summary>
        /// <param name="a">Первое число</param>
        /// <param name="b">Второе число</param>
        /// <returns>Разность чисел A и B</returns>
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
                var units = (max.GetDigitFromEnd(i) - min.GetDigitFromEnd(i) - debt + 10) % 10;
                debt = max.GetDigitFromEnd(i) - min.GetDigitFromEnd(i) - debt < 0 ? 1 : 0;

                digits.Add(units);
            }

            digits.Reverse();

            return new BigInt(max.Sign, digits);
        }

        #endregion

        #region Multiplicative Operations

        /// <summary>
        /// Умножение двух чисел (Алгоритм Карацубы)
        /// Рекурсивный алгоритм умножения двух чисел подходит для умножения больших чисел
        /// Гораздо эффективнее умножения "столбиком", т.к. имеет сложность O(n^log_2(3))
        /// </summary>
        /// <param name="x">Первый множитель</param>
        /// <param name="y">Второй множитель</param>
        /// <returns>Произвдение двух множителей</returns>
        public static BigInt KaratsubaMultiplication(BigInt x, BigInt y)
        {
            var ux = new BigInt(Sign.Plus, x._digits);
            var uy = new BigInt(Sign.Plus, y._digits);
            var sign = x.Sign == y.Sign ? Sign.Plus : Sign.Minus;

            var maxSize = Math.Max(x.Size, y.Size);

            if (maxSize < 2)
                return DefaultMultiplication(x, y);

            maxSize = maxSize / 2 + maxSize % 2;

            var m = Power10(One, maxSize);

            var b = ux % m;
            var a = ux / m;
            var c = uy / m;
            var d = uy % m;

            var z0 = a * c;
            var z1 = b * d;
            var z2 = (a + b) * (c + d);

            var result = DefaultMultiplication(Power10(One, maxSize * 2), z0) + z1 +
                         DefaultMultiplication(z2 - z1 - z0, m);
            return new BigInt(sign, result._digits);
        }

        private static BigInt Power10(BigInt number, int power) => new BigInt(number.Sign,
            number._digits.Concat(Enumerable.Repeat(0, power)).ToList());

        /// <summary>
        /// Умножение "столбиком" двух чисел
        /// </summary>
        /// <param name="a">Первый множитель</param>
        /// <param name="b">Второй множитель</param>
        /// <returns>Произведение двух множителей</returns>
        public static BigInt DefaultMultiplication(BigInt a, BigInt b)
        {
            var result = Zero;
            for (var i = 0; i < b.Size; i++)
            {
                var d = b.GetDigitFromEnd(b.Size - i - 1);
                var buf = MultiplyOnDigit(a, d);
                buf = Power10(buf, b.Size - i - 1);
                result += buf;
            }

            return new BigInt(a.Sign == b.Sign ? Sign.Plus : Sign.Minus, result._digits);
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

        /// <summary>
        /// Целочисленное деление двух чисел
        /// </summary>
        /// <param name="dividend">Делимое</param>
        /// <param name="divider">Делитель</param>
        /// <returns>Целая часть от деления делимого на делитель</returns>
        /// <exception cref="DivideByZeroException">Исключение при попытке деления на 0</exception>
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
                    if (DefaultMultiplication(b, new BigInt(j)) <= new BigInt(left))
                        times = j;
                    else
                        break;
                }

                result.Add(times);

                if (i < a.Size)
                    left = (Power10(new BigInt(left) - DefaultMultiplication(b, new BigInt(times)), 1) +
                            new BigInt(a.GetDigitFromEnd(a.Size - i - 1)))._digits;
            }

            var answer = new BigInt(sign, result);
            return answer;
        }
        
        /// <summary>
        /// Остаток от деления
        /// </summary>
        /// <param name="a">Делимое</param>
        /// <param name="b">Делитель</param>
        /// <returns>Всегда неотрицательный остаток от деления делимого на делитель</returns>
        public static BigInt Mod(BigInt a, BigInt b)
        {
            if (Abs(a) < Abs(b))
                return a;

            var div = a / b;
            if (DefaultMultiplication(a, b) < Zero)
                div -= One;
            return a - DefaultMultiplication(b, div);
        }

        /// <summary>
        /// Возведение в степень
        /// </summary>
        /// <param name="n">Основание степени</param>
        /// <param name="pow">Показатель степени</param>
        /// <returns>Возвращает число N в степени Pow </returns>
        public static BigInt Pow(BigInt n, BigInt pow)
        {
            if (pow < Zero)
                return Zero;
            
            var number = new BigInt(n);
            var b = new BigInt(1);
            var map = DecToBin(pow);
            for (var i = map.Size - 1; i >= 0; i--)
            {
                b *= b;
                if (map.GetDigitFromEnd(i) == 1)
                    b *= number;
            }

            return b;
        }

        #endregion
        
        #region Extended Operations

        /// <summary>
        /// Абсолютное значение числа
        /// </summary>
        /// <param name="n">Число, абсолютное значение которого необходимо вернуть</param>
        /// <returns>Неотрицательное число, являющееся абсолютным значением числа N</returns>
        private static BigInt Abs(BigInt n) => new BigInt(Sign.Plus, n._digits);

        /// <summary>
        /// Число, обратное по модулю (Расширенный алгоритм Евклида)
        /// </summary>
        /// <param name="a">Число, для которого необходимо найти обратное</param>
        /// <param name="n">Модуль, по которому находится обратное</param>
        /// <returns>Возвращает число, обратное A по модулю N (такое, что A * X mod N = 1)</returns>
        public static BigInt Inverse(BigInt a, BigInt n)
        {
            BigInt i = n, v = Zero, d = One;
            while (a > Zero)
            {
                var t = i / a;
                var x = a;
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

        /// <summary>
        /// Наибольший общий делитель двух чисел (алгоритм Евклида)
        /// </summary>
        /// <param name="a">Первое число</param>
        /// <param name="b">Второе число</param>
        /// <returns>Возвращает наибольший общий делитель двух переданных чисел</returns>
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

        #endregion
        
        #region Compare Operations
        
        /// <summary>
        /// Сравнение двух чисел
        /// </summary>
        /// <param name="a">Первое число</param>
        /// <param name="b">Второе число</param>
        /// <param name="ignoreSign">Необязательный параметр. Если <value>true</value>, то при сравнении чисел знак будет игнорироваться</param>
        /// <returns></returns>
        public static int Comparison(BigInt a, BigInt b, bool ignoreSign = false)
        {
            var first = a.Size == 1 && a._digits[0] == 0 ? Zero : a;
            var second = b.Size == 1 && b._digits[0] == 0 ? Zero : b;
            
            return CompareSign(first, second, ignoreSign);
        }

        private static int CompareSign(BigInt a, BigInt b, bool ignoreSign = false)
        {
            if (ignoreSign)
                return CompareSize(a, b);

            if (a.Sign < b.Sign)
                return -1;
            return a.Sign > b.Sign ? 1 : CompareSize(a, b);
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
            for (var i = maxLength; i >= 0; i--)
            {
                if (a.GetDigitFromEnd(i) < b.GetDigitFromEnd(i))
                    return -1;
                if (a.GetDigitFromEnd(i) > b.GetDigitFromEnd(i))
                    return 1;
            }

            return 0;
        }

        #endregion

        #region Service Operations
        
        /// <summary>
        /// Перевод числа в двоичную систему счисления
        /// </summary>
        /// <param name="dec">Число для перевода в двоичную СС</param>
        /// <returns>Число, являющееся представлением переданного в двоичной СС</returns>
        private static BigInt DecToBin(BigInt dec)
        {
            var digits = new List<int>();
            while (dec != Zero)
            {
                digits.Add((dec % new BigInt(2)).GetDigitFromEnd(0));
                dec /= new BigInt(2);
            }

            digits.Reverse();

            return new BigInt(dec.Sign, digits);
        }

        private int GetDigitFromEnd(int i) => i < Size ? _digits[Size - i - 1] : 0;
        
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

        private static IEnumerable<int> GetDigits(uint num) => num.ToString().Select(c => c - '0');
        
        #endregion
        
        #region Operators

        public static BigInt operator -(BigInt a) => new BigInt(a.Sign == Sign.Plus ? Sign.Minus : Sign.Plus, a._digits);

        public static BigInt operator +(BigInt a, BigInt b) => Add(a, b);

        public static BigInt operator -(BigInt a, BigInt b) => Sub(a, b);

        public static BigInt operator *(BigInt a, BigInt b) => KaratsubaMultiplication(a, b);

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

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_digits != null ? _digits.GetHashCode() : 0) * 397) ^ (int) Sign;
            }
        }

        public override string ToString() =>
            this == Zero ? "0" : $"{_digits[0] * (int) Sign}{string.Join("", _digits.Skip(1))}";

        #endregion
    }
}