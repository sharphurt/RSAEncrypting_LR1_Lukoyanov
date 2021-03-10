using System;
using NUnit.Framework;

namespace RSAEncrypting_LR1_Lukoyanov.RSA
{
    [TestFixture]
    public class RsaTests
    {
        private const int P = 11;
        private const int Q = 13;

        private static readonly PublicKey PublicKey = PublicKeyGenerator.Generate(P, Q);
        private static readonly PrivateKey PrivateKey = PrivateKeyGenerator.Generate(PublicKey);

        [TestCase(31, 125)]
        [TestCase(32, 98)]
        [TestCase(33, 110)]
        [TestCase(34, 122)]
        [TestCase(343, 73)]
        [TestCase(35, 139)]
        [TestCase(21, 109)]
        [TestCase(9, 48)]
        public void NumberEncryptionTest(int number, int encrypted) => 
            Assert.AreEqual((BigInt.BigInt) encrypted, RsaEncrypting.EncryptNumber(number, PublicKey));

        [TestCase(18, 112)]
        [TestCase(3, 16)]
        [TestCase(330, 99)]
        [TestCase(19, 72)]
        [TestCase(343, 8)]
        [TestCase(67, 89)]
        [TestCase(43, 43)]
        [TestCase(12, 12)]
        public void NumberDecryptionTest(int encrypted, int original) =>
            Assert.AreEqual(original, RsaEncrypting.DecryptNumber(encrypted, PrivateKey));
    }
}