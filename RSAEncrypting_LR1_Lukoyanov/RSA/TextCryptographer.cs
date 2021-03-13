using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSAEncrypting_LR1_Lukoyanov.RSA
{
    public class TextCryptographer : ICryptographer<string>
    {
        private readonly Dictionary<char, int> _alphabet =
            "абвгдеёжзийклмнопрстуфхцчшщъыьэюяabcdefghijklmnopqrstuvwxyz1234567890 "
                .Select((value, index) => new {value, index}).ToDictionary(pair => pair.value, pair => pair.index);
        
        public async Task<BigInt.BigInt[]> Encrypt(string text, PublicKey publicKey, IProgress<double> progress = default)
        {
            var textBytes = StringUtils.ConvertToNumbers(text.ToLower(), _alphabet);
            var encrypted = new BigInt.BigInt[textBytes.Length];

            await Task.Run(() =>
            {
                for (var i = 0; i < textBytes.Length; i++)
                {
                    encrypted[i] = RsaEncrypting.EncryptNumber(textBytes[i], publicKey);
                    progress?.Report(100.0 / text.Length * i);
                }

                progress?.Report(100);
            });

            return encrypted;
        }

        public async Task<string> Decrypt(BigInt.BigInt[] encrypt, PrivateKey privateKey, IProgress<double> progress = default)
        {
            var decrypted = new int[encrypt.Length];
            await Task.Run(() =>
            {
                for (var i = 0; i < encrypt.Length; i++)
                {
                    decrypted[i] = (byte) RsaEncrypting.DecryptNumber(encrypt[i], privateKey);
                    progress?.Report(Math.Round(100.0 / encrypt.Length * i));
                }

                progress?.Report(100);
            });
            return StringUtils.ConvertNumbersToString(decrypted, _alphabet);
        }
    }
}