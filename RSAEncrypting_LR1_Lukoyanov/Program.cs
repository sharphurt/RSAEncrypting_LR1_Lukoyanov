using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using RSAEncrypting_LR1_Lukoyanov.RSA;

namespace RSAEncrypting_LR1_Lukoyanov
{
    internal static class Program
    {
        private const int P = 11;
        private const int Q = 13;

        public static async Task Main(string[] args)
        {
            Console.WriteLine("RSA Text Encrypting/Decrypting Util (demonstration)");

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

            var publicKey = PublicKeyGenerator.Generate(P, Q);
            var (encrypt, encryptionTime) = await EncryptingTask(text, publicKey);
            
            Console.WriteLine($"\nEncryption time: {encryptionTime.ToString()}");
            Console.WriteLine($"\nEncrypted bytes:\n{StringUtils.CollectionToReadable(encrypt)}\n");
            
            var privateKey = PrivateKeyGenerator.Generate(publicKey);
            var (decrypt, decryptionTime) = await DecryptingTask(encrypt, privateKey);
            
            Console.WriteLine($"\nDecryption time: {decryptionTime.ToString()}\n");
            Console.WriteLine($"Decrypted text:\n{decrypt}");
            
            Console.WriteLine($"\nTotal time passed: {(encryptionTime + decryptionTime).ToString()}");
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

            Console.WriteLine("\nEncrypting...");
            var publicKey = PublicKeyGenerator.Generate(P, Q);
            var (encrypt, encryptionTime) = await EncryptingTask(text, publicKey);

            Console.WriteLine($"\nEncryption time: {encryptionTime.ToString()}\n");

            var encryptedFilePath = $"{Path.GetDirectoryName(filePath) ?? "C:\\"}\\{Path.GetFileNameWithoutExtension(filePath)}_encrypted.txt";
            using (var writer = new StreamWriter(encryptedFilePath))
                await writer.WriteAsync(StringUtils.CollectionToReadable(encrypt));

            Console.WriteLine($"Encrypted file saved at {encryptedFilePath}\n");
            
            Console.WriteLine("Decrypting...");
            var privateKey = PrivateKeyGenerator.Generate(publicKey);
            var (decrypt, decryptionTime) = await DecryptingTask(encrypt, privateKey);
            
            var decryptedFilePath = $"{Path.GetDirectoryName(filePath) ?? "C:\\"}\\{Path.GetFileNameWithoutExtension(filePath)}_decrypted.txt";
            using (var writer = new StreamWriter(decryptedFilePath))
                await writer.WriteAsync(decrypt);

            Console.WriteLine($"\nDecryption time: {decryptionTime.ToString()}\n");
            Console.WriteLine($"Decrypted file saved at {decryptedFilePath}\n");
            
            Console.WriteLine($"Total time passed: {(encryptionTime + decryptionTime).ToString()}");
        }


        private static async Task<(BigInt.BigInt[] encrypted, TimeSpan timePassed)> EncryptingTask(string text,
            PublicKey publicKey)
        {
            var startTime = DateTime.Now;
            var progressBar = new ProgressBar();
            var progress = new Progress<double>(percent => progressBar.Report(percent / 100.0));
            var encrypted = await RsaEncrypting.EncryptText(text, publicKey, progress);
            progressBar.Dispose();
            return (encrypted, DateTime.Now - startTime);
        }

        private static async Task<(string decrypt, TimeSpan timePassed)> DecryptingTask(BigInt.BigInt[] encrypt,
            PrivateKey privateKey)
        {
            var startTime = DateTime.Now;
            var progressBar = new ProgressBar();
            var progress = new Progress<double>(percent => progressBar.Report(percent / 100.0));
            var decrypt = await RsaEncrypting.DecryptText(encrypt, privateKey, progress);
            progressBar.Dispose();
            return (decrypt, DateTime.Now - startTime);
        }
    }
}