using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RSAEncrypting_LR1_Lukoyanov.RSA;

namespace RSAEncrypting_LR1_Lukoyanov
{
    internal static class Program
    {
        private static int P;
        private static int _q;

        private static readonly ICryptographer Cryptographer = new AsciiCryptographer();

        public static async Task Main(string[] args)
        {
            Console.WriteLine("RSA Text Encrypting/Decrypting Util (demonstration)");
            Console.WriteLine(
                "Util can encrypt and decrypt text only in ASCII encoding (UTF-8/UTF-16 support coming soon)");
            bool n1Successfully;
            bool n2Successfully;
            do
            {
                string input;
                var isInputCorrect = false;

                do
                {
                    Console.Write("Choose two prime numbers to use RSA algorithm: ");
                    
                    input = Console.ReadLine() ?? string.Empty;
                    if (input == "q")
                        return;

                    if (string.IsNullOrEmpty(input.Trim()) || input.Split().Length != 2)
                        Console.WriteLine("Input is incorrect. Try again or enter 'q' to exit");
                    else
                        isInputCorrect = true;
                } while (!isInputCorrect);

                n1Successfully = int.TryParse(input.Split()[0], out P);
                n2Successfully = int.TryParse(input.Split()[1], out _q);

                if (!n1Successfully || !n2Successfully)
                    Console.WriteLine("Input is incorrect. Try again or enter 'q' to exit");

                if (n1Successfully && !P.IsPrime())
                {
                    Console.WriteLine("First number isn't prime! Try again");
                    n1Successfully = false;
                }

                if (n2Successfully && !_q.IsPrime())
                {
                    Console.WriteLine("Second number isn't prime! Try again");
                    n2Successfully = false;
                }
            } while (!n1Successfully || !n2Successfully);

            while (true)
            {
                Console.WriteLine(
                    "\nChoose the option:\n1. Encrypt and decrypt entered text\n2. Encrypt adn decrypt text file\nType 'q' to exit");

                bool isSuccessfullyParsed;
                do
                {
                    Console.Write("Enter the option number: ");
                    var optionNumber = Console.ReadLine();

                    if (optionNumber == "q")
                        return;

                    isSuccessfullyParsed = int.TryParse(optionNumber, out var parsedOptionNumber);

                    switch (parsedOptionNumber)
                    {
                        case 1:
                            await ConsoleInputEncrypting();
                            break;
                        case 2:
                            await ProcessTextFile();
                            break;
                        default:
                            Console.WriteLine("Incorrect option number. Try again");
                            break;
                    }
                } while (!isSuccessfullyParsed);
            }
        }

        private static async Task ConsoleInputEncrypting()
        {
            Console.WriteLine("Enter text to encrypt/decrypt: ");
            var text = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrEmpty(text.Trim()))
            {
                Console.WriteLine("Input is empty");
                return;
            }

            var publicKey = PublicKeyGenerator.Generate(P, _q);
            Console.WriteLine($"\nPublic key: {publicKey}");
            
            Console.WriteLine("Encrypting...");
            var (encrypt, encryptionTime) = await EncryptingTask(text, publicKey);
            Console.WriteLine($"\nEncryption time: {encryptionTime}");
            Console.WriteLine($"\nEncrypted bytes:\n{StringUtils.CollectionToReadable(encrypt)}\n");

            var privateKey = PrivateKeyGenerator.Generate(publicKey);
            Console.WriteLine($"Private key: {privateKey}");

            Console.WriteLine("Decrypting...");
            var (decrypt, decryptionTime) = await DecryptingTask(encrypt, privateKey);

            Console.WriteLine($"\nDecryption time: {decryptionTime}\n");
            Console.WriteLine($"Decrypted text:\n{decrypt}");

            Console.WriteLine($"\nTotal time passed: {encryptionTime + decryptionTime}");
        }

        private static async Task ProcessTextFile()
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

            var publicKey = PublicKeyGenerator.Generate(P, _q);
            Console.WriteLine($"\nPublic key: {publicKey}");
            
            Console.WriteLine("Encrypting...");
            var (encrypt, encryptionTime) = await EncryptingTask(text, publicKey);
            Console.WriteLine($"\nEncryption time: {encryptionTime}\n");

            var encryptedFilePath =
                $"{Path.GetDirectoryName(filePath) ?? "C:\\"}\\{Path.GetFileNameWithoutExtension(filePath)}_encrypted.txt";
            using (var writer = new StreamWriter(encryptedFilePath))
                await writer.WriteAsync(StringUtils.CollectionToReadable(encrypt));

            Console.WriteLine($"Encrypted file saved at {encryptedFilePath}\n");

            var privateKey = PrivateKeyGenerator.Generate(publicKey);
            Console.WriteLine($"Private key: {privateKey}");

            Console.WriteLine("Decrypting...");
            var (decrypt, decryptionTime) = await DecryptingTask(encrypt, privateKey);

            var decryptedFilePath =
                $"{Path.GetDirectoryName(filePath) ?? "C:\\"}\\{Path.GetFileNameWithoutExtension(filePath)}_decrypted.txt";
            using (var writer = new StreamWriter(decryptedFilePath))
                await writer.WriteAsync(decrypt);

            Console.WriteLine($"\nDecryption time: {decryptionTime}\n");
            Console.WriteLine($"Decrypted file saved at {decryptedFilePath}\n");

            Console.WriteLine($"Total time passed: {encryptionTime + decryptionTime}");
        }


        private static async Task<(BigInt.BigInt[] encryption, TimeSpan timePassed)> EncryptingTask(string text,
            PublicKey publicKey)
        {
            var startTime = DateTime.Now;
            var progressBar = new ProgressBar();
            var progress = new Progress<double>(progressBar.Report);
            var encrypted = await Cryptographer.Encrypt(text, publicKey, progress);
            progressBar.Dispose();
            return (encrypted, DateTime.Now - startTime);
        }

        private static async Task<(string decryption, TimeSpan timePassed)> DecryptingTask(BigInt.BigInt[] encrypt,
            PrivateKey privateKey)
        {
            var startTime = DateTime.Now;
            var progressBar = new ProgressBar();
            var progress = new Progress<double>(progressBar.Report);
            var decrypt = await Cryptographer.Decrypt(encrypt, privateKey, progress);
            progressBar.Dispose();
            return (decrypt, DateTime.Now - startTime);
        }
    }
}