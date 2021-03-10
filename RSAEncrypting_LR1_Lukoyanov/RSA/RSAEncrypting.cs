namespace RSAEncrypting_LR1_Lukoyanov.RSA
{
    public static class RsaEncrypting
    {
        public static BigInt.BigInt EncryptNumber(int number, PublicKey publicKey) =>
            BigInt.BigInt.Pow(number, publicKey.E) % publicKey.N;
        
        public static int DecryptNumber(BigInt.BigInt number, PrivateKey privateKey) =>
            int.Parse((BigInt.BigInt.Pow(number, privateKey.D) % privateKey.N).ToString());
        
    }
}