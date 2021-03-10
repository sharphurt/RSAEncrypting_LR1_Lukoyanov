using System;
using System.Text;
using System.Threading.Tasks;

namespace RSAEncrypting_LR1_Lukoyanov.RSA
{
    public class AsciiCryptographer : ICryptographer
    {
        public async Task<BigInt.BigInt[]> Encrypt(string text, PublicKey publicKey, IProgress<double> progress)
        {
            var textBytes = StringUtils.ConvertToBytes(text, Encoding.ASCII);
            var encrypted = new BigInt.BigInt[textBytes.Length];

            await Task.Run(() =>
            {
                for (var i = 0; i < textBytes.Length; i++)
                {
                    encrypted[i] = RsaEncrypting.EncryptNumber(textBytes[i], publicKey);
                    progress?.Report(100.0 / textBytes.Length * i);
                }
                progress?.Report(100);
            });
            
            
            return encrypted;
        }

        public async Task<string> Decrypt(BigInt.BigInt[] encrypt, PrivateKey privateKey, IProgress<double> progress)
        {
            var decrypted = new byte[encrypt.Length];
            await Task.Run(() =>
            {
                for (var i = 0; i < encrypt.Length; i++)
                {
                    decrypted[i] = (byte) RsaEncrypting.DecryptNumber(encrypt[i], privateKey);
                    progress?.Report(Math.Round(100.0 / encrypt.Length * i));
                }
                progress?.Report(100);
            });
            return StringUtils.ConvertBytesToString(decrypted, Encoding.ASCII);
        }
    }
}