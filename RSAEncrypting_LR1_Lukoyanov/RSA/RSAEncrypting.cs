using System;
using System.Linq;
using System.Threading.Tasks;

namespace RSAEncrypting_LR1_Lukoyanov.RSA
{
    public static class RsaEncrypting
    {
        private static BigInt.BigInt EncryptNumber(int number, PublicKey publicKey) =>
            BigInt.BigInt.ModOfPower(new BigInt.BigInt(number), publicKey.E, publicKey.N);

        private static int DecryptNumber(BigInt.BigInt number, PrivateKey privateKey) =>
            int.Parse(BigInt.BigInt.ModOfPower(number, privateKey.D, privateKey.N).ToString());

        public static async Task<BigInt.BigInt[]> EncryptText(string text, PublicKey publicKey,
            IProgress<double> progress)
        {
            var textBytes = StringUtils.ConvertToAsciiBytes(text);
            var encrypted = new BigInt.BigInt[textBytes.Length];

            await Task.Run(() =>
            {
                for (var i = 0; i < textBytes.Length; i++)
                {
                    encrypted[i] = EncryptNumber(textBytes[i], publicKey);
                    progress?.Report(Math.Round(100.0 / textBytes.Length * i));
                }
                progress?.Report(100);
            });

            return encrypted;
        }

        public static async Task<string> DecryptText(BigInt.BigInt[] encrypted, PrivateKey privateKey,
            IProgress<double> progress)
        {
            var decrypted = new byte[encrypted.Length];
            await Task.Run(() =>
            {
                for (var i = 0; i < encrypted.Length; i++)
                {
                    decrypted[i] = (byte) DecryptNumber(encrypted[i], privateKey);
                    progress?.Report(Math.Round(100.0 / encrypted.Length * i));
                }
                progress?.Report(100);
            });
            
            return StringUtils.ConvertToString(decrypted);
        }
    }
}