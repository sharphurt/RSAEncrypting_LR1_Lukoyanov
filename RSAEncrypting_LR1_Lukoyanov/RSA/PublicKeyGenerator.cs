namespace RSAEncrypting_LR1_Lukoyanov
{
    public static class PublicKeyGenerator
    {
        public static PublicKey Generate(int p, int q)
        {
            var n = p * q;
            var phi = (p - BigInt.One) * (q - BigInt.One);
            var e = BigInt.One;
            
            while (BigInt.GreatestCommonDivisor(e, phi) != BigInt.One)
                e += BigInt.One;

            return new PublicKey(e, n, phi);
        }
    }
}