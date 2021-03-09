using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RSAEncrypting_LR1_Lukoyanov
{
    internal class Program
    {
        private const int P = 11;
        private const int Q = 13;

        public static void Main(string[] args)
        {
            Console.WriteLine("RSA Text Encrypting/Decrypting Util (demonstration)");

            while (true)
            {
                Console.WriteLine(
                    "\nChoose the option:\n1. Encrypt and decrypt entered text\n2. Encrypt adn decrypt text file\nType 'q' to exit");

                var isSuccessfullyParsed = false;
                var parsedOptionNumber = 0;
                do
                {
                    Console.Write("Enter the option number: ");
                    var optionNumber = Console.ReadLine();

                    if (optionNumber == "q")
                        return;

                    isSuccessfullyParsed = int.TryParse(optionNumber, out parsedOptionNumber);

                    switch (parsedOptionNumber)
                    {
                        case 1:
                            ConsoleInputEncrypting();
                            break;
                        case 2:
                            TextFileEncrypting();
                            break;
                        default:
                            Console.WriteLine("Incorrect option number. Try again");
                            break;
                    }
                } while (!isSuccessfullyParsed);
            }
        }

        private static void ConsoleInputEncrypting()
        {
            Console.WriteLine("Enter text to encrypt/decrypt: ");
            var text = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrEmpty(text.Trim()))
            {
                Console.WriteLine("Input is empty");
                return;
            }

            var publicKey = PublicKeyGenerator.Generate(P, Q);
            EncryptDecryptText(text, publicKey);
        }

        private static void TextFileEncrypting()
        {
            Console.Write("Enter file path: ");
            var filePath = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrEmpty(filePath.Trim()) || !File.Exists(filePath))
            {
                Console.WriteLine("File path is invalid or file doesn't exist");
                return;
            }

            var text = string.Join(" ", File.ReadAllLines(filePath));

            if (string.IsNullOrEmpty(text.Trim()))
            {
                Console.WriteLine("File contains no content to encrypt");
                return;
            }

            var publicKey = PublicKeyGenerator.Generate(P, Q);
            EncryptDecryptText(text, publicKey);
        }

        private static void EncryptDecryptText(string text, PublicKey publicKey)
        {
            var encryptionStartTime = DateTime.Now;
            var encrypted = RSAEncrypting.EncryptText(text, publicKey);
            var encryptionTime = DateTime.Now - encryptionStartTime;

            Console.WriteLine($"\nEncrypted bytes: [{string.Join(", ", encrypted.Select(n => n.ToString()))}]");

            Console.WriteLine($"Encryption time: {encryptionTime.ToString()}");

            var decryptionStartTime = DateTime.Now;

            var privateKey = PrivateKeyGenerator.Generate(publicKey);
            var decrypted = RSAEncrypting.DecryptText(encrypted, privateKey);

            var decryptionTime = DateTime.Now - decryptionStartTime;

            var passed = encryptionTime + decryptionTime;

            Console.WriteLine($"\nDecrypted text: {decrypted}");

            Console.WriteLine($"Decryption time: {decryptionTime.ToString()}");

            Console.WriteLine($"Time passed: {passed.ToString()}");

            Console.WriteLine(
                $"\nDifference between original and decrypted: {StringUtils.GetDifference(text, decrypted)}");
        }
    }
}