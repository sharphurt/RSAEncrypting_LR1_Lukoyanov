using System;
using System.Threading.Tasks;

namespace RSAEncrypting_LR1_Lukoyanov.RSA
{
    public interface ICryptographer<T>
    {
        Task<BigInt.BigInt[]> Encrypt(T text, PublicKey publicKey, IProgress<double> progress = default);

        Task<T> Decrypt(BigInt.BigInt[] encrypt, PrivateKey privateKey, IProgress<double> progress = default);
    }
}