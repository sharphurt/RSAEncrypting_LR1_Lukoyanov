using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace RSAEncrypting_LR1_Lukoyanov
{
    public class BigInt
    {
        private readonly int _sing;
        private readonly List<int> _digits = new List<int>();

        public BigInt()
        {
        }

        public BigInt(int sign, IEnumerable<int> number)
        {
            _sing = sign;
            _digits = number.ToList();
        }

        public BigInt(string number)
        {
            var start = 0;

            if (number[0] == '+')
            {
                _sing = 1;
                start = 1;
            }
            else if (number[0] == '-')
            {
                _sing = -1;
                start = 1;
            }
            else if (char.IsDigit(number[0]))
            {
                _sing = 1;
                start = 0;
            }
            else
                throw new ArgumentException("Incorrect sign");
            
            if (number.Skip(start).Any(c => !char.IsDigit(c)))
                throw new ArgumentException("Invalid input");
            
            _digits.AddRange(number.Substring(start).TrimStart('0').Select(c => c - '0'));
        }

        public BigInt(BigInt parent)
        {
            _sing = parent._sing;
            _digits = new List<int>(parent._digits);
        }

        public static BigInt operator +(BigInt firstTerm, BigInt secondTerm)
        {
            var sumDigits = new List<int>();
            var a = new BigInt(firstTerm);
            var b = new BigInt(secondTerm);
            
            a._digits.InsertRange(0, Enumerable.Repeat(0, Math.Max(a._digits.Count, b._digits.Count) - a._digits.Count));
            b._digits.InsertRange(0, Enumerable.Repeat(0, Math.Max(a._digits.Count, b._digits.Count) - b._digits.Count));
            
            
            if (a._sing * b._sing == 1)
            {
                var tens = 0;
                var units = 0;
             
                for (var i = Math.Max(a._digits.Count, b._digits.Count) - 1; i >= 0; i--)
                {
                    units = (a._digits[i] + b._digits[i] + tens) % 10;
                    tens = (a._digits[i] + b._digits[i] + tens) / 10;
                    sumDigits.Add(units);
                }

                if (tens > 0)
                    sumDigits.Add(tens);
            }

            sumDigits.Reverse();
            
            return new BigInt(1, sumDigits);
        }

        public static bool operator ==(BigInt a, BigInt b)
        {
            if (a is null || b is null)
                throw new ArgumentNullException();
            
            return a._sing == b._sing && a._digits.SequenceEqual(b._digits);
        }
        
        public static bool operator !=(BigInt a, BigInt b)
        {
            if (a is null || b is null)
                throw new ArgumentNullException();

            return a._sing != b._sing || !a._digits.SequenceEqual(b._digits);
        }
    }
}