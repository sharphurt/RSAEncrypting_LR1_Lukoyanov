using System;
using System.Threading.Tasks;

namespace RSAEncrypting_LR1_Lukoyanov.RSA
{
    public interface ICryptographer
    {
        Task<BigInt.BigInt[]> Encrypt(string text, PublicKey publicKey, IProgress<double> progress);

        Task<string> Decrypt(BigInt.BigInt[] encrypt, PrivateKey privateKey, IProgress<double> progress);
    }
}