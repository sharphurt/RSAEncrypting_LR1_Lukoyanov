namespace RSAEncrypting_LR1_Lukoyanov.RSA
{
    public static class RsaEncrypting
    {
        public static BigInt.BigInt EncryptNumber(int number, PublicKey publicKey) =>
            BigInt.BigInt.ModOfPower(new BigInt.BigInt(number), publicKey.E, publicKey.N);

        public static int DecryptNumber(BigInt.BigInt number, PrivateKey privateKey) =>
            int.Parse(BigInt.BigInt.ModOfPower(number, privateKey.D, privateKey.N).ToString());
        
    }
}