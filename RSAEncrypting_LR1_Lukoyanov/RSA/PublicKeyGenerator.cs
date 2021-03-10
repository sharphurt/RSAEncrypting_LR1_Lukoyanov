namespace RSAEncrypting_LR1_Lukoyanov.RSA
{
    public static class PublicKeyGenerator
    {
        public static PublicKey Generate(int p, int q)
        {
            var n = p * q;
            var phi = (p - BigInt.BigInt.One) * (q - BigInt.BigInt.One);
            BigInt.BigInt e = 2;
            
            while (BigInt.BigInt.GreatestCommonDivisor(e, phi) != BigInt.BigInt.One)
                e += BigInt.BigInt.One;

            return new PublicKey(e, n, phi);
        }
    }
}