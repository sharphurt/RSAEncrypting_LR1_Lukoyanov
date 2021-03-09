using System.Linq;


namespace RSAEncrypting_LR1_Lukoyanov
{
    public static class RSAEncrypting
    {
        private static BigInt EncryptNumber(int number, PublicKey publicKey) =>
            BigInt.ModOfPower(new BigInt(number), publicKey.E, publicKey.N);

        private static int DecryptNumber(BigInt number, PrivateKey privateKey) =>
            int.Parse(BigInt.ModOfPower(number, privateKey.D, privateKey.N).ToString());
        
        public static BigInt[] EncryptText(string text, PublicKey publicKey)
        {
            var textBytes = StringUtils.ConvertToAsciiBytes(text);
            return textBytes.Select(b => EncryptNumber(b, publicKey)).ToArray();
        }

        public static string DecryptText(BigInt[] encrypted, PrivateKey privateKey)
        {
            var decryptedBytes = encrypted.Select(n => (byte) DecryptNumber(n, privateKey)).ToArray();
            return StringUtils.ConvertToString(decryptedBytes);
        }
    }
}