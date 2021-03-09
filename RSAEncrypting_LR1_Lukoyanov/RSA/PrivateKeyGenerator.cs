namespace RSAEncrypting_LR1_Lukoyanov
{
    public static class PrivateKeyGenerator
    {
        public static PrivateKey Generate(PublicKey publicKey)
        {
            var d = BigInt.Inverse(publicKey.E, publicKey.Phi);
            return new PrivateKey(d, publicKey.N);
        }
    }
}