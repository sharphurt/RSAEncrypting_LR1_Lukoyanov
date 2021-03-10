using System;

namespace RSAEncrypting_LR1_Lukoyanov
{
    public static class IntExtensions
    {
        public static bool IsPrime(this int value)
        {
            for (var i = 2; i <= Math.Sqrt(value); ++i)
                if (value % i == 0)
                    return false;

            return true;
        }
    }
}