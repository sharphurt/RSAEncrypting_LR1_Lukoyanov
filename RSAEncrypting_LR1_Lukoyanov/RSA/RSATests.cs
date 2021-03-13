using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace RSAEncrypting_LR1_Lukoyanov.RSA
{
    [TestFixture]
    public class RsaTests
    {
        private const int P = 7;
        private const int Q = 11;

        private static readonly PublicKey PublicKey = PublicKeyGenerator.Generate(P, Q);
        private static readonly PrivateKey PrivateKey = PrivateKeyGenerator.Generate(PublicKey);

        private static readonly TextCryptographer Cryptographer = new TextCryptographer();
        
        [TestCase(31, 59)]
        [TestCase(51, 72)]
        [TestCase(14, 42)]
        [TestCase(212, 9)]
        [TestCase(343, 7)]
        [TestCase(18, 39)]
        [TestCase(73, 17)]
        [TestCase(9, 37)]
        public void NumberEncryptionTest(int number, int encrypted) => 
            Assert.AreEqual((BigInt.BigInt) encrypted, RsaEncrypting.EncryptNumber(number, PublicKey));

        [TestCase(18, 46)]
        [TestCase(3, 38)]
        [TestCase(330, 22)]
        [TestCase(19, 61)]
        [TestCase(343, 63)]
        [TestCase(68, 19)]
        [TestCase(46, 74)]
        [TestCase(12, 12)]
        public void NumberDecryptionTest(int encrypted, int original) =>
            Assert.AreEqual(original, RsaEncrypting.DecryptNumber(encrypted, PrivateKey));

        [TestCase("hello world", new []{61, 16, 44, 44, 75, 20, 55, 75, 8, 44, 64})]
        [TestCase("The quick brown fox jumps over the lazy dog", new []{24, 61, 16, 20, 14, 4, 13, 7, 43, 20, 34, 8, 75, 55, 18, 20, 3, 75, 56, 20, 70, 4, 45, 27, 72, 20, 75, 54, 16, 8, 20, 24, 61, 16, 20, 44, 33, 9, 29, 20, 64, 75, 74})]
        public void StringEncryptionTest(string toEncrypt, int[] result)
        {
            var encrypted = Cryptographer.Encrypt(toEncrypt, PublicKey);
            Assert.AreEqual(result.Select(x => (BigInt.BigInt) x), encrypted.Result.ToArray());
        }
        
        [TestCase(new []{33, 34, 7, 64, 16, 3, 74, 61, 13, 43, 44, 45, 75, 27, 14, 8, 72, 24, 4, 54, 55, 56, 29, 9}, "abcdefghiklmopqrstuvwxyz")]
        [TestCase(new []{61, 16, 44, 44, 75, 20, 3, 8, 75, 45, 20, 45, 33, 8, 72, 20, 74, 8, 16, 33, 24, 13, 18, 74, 72}, "hello from mars greatings")]
        public void StringDecryptionTest(int[] encrypt, string result)
        {
            var decrypted = Cryptographer.Decrypt(encrypt.Select(x => (BigInt.BigInt) x).ToArray(), PrivateKey);
            Assert.AreEqual(decrypted.Result, result);
        }
    }
}