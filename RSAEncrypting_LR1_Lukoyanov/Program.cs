using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RSAEncrypting_LR1_Lukoyanov.RSA;

namespace RSAEncrypting_LR1_Lukoyanov
{
    internal static class Program
    {
        private static int _p;
        private static int _q;

        private static readonly ICryptographer Cryptographer = new AsciiCryptographer();

        public static async Task Main(string[] args)
        {
            // Доступные локализации: RU-ru, En-US
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("RU-ru");
            
            Console.WriteLine(Strings.ProgramName);
            Console.WriteLine(Strings.ProgramDescription);
            bool n1Successfully;
            bool n2Successfully;
            do
            {
                string input;
                var isInputCorrect = false;

                do
                {
                    Console.Write(Strings.ChoosePrimes);
                    
                    input = Console.ReadLine() ?? string.Empty;
                    if (input == "q")
                        return;

                    if (string.IsNullOrEmpty(input.Trim()) || input.Split().Length != 2)
                        Console.WriteLine(Strings.IncorrectInput);
                    else
                        isInputCorrect = true;
                } while (!isInputCorrect);

                n1Successfully = int.TryParse(input.Split()[0], out _p);
                n2Successfully = int.TryParse(input.Split()[1], out _q);

                if (!n1Successfully || !n2Successfully)
                    Console.WriteLine(Strings.IncorrectInput);

                if (n1Successfully && !_p.IsPrime())
                {
                    Console.WriteLine(Strings.FirstNumberIsntPrime);
                    n1Successfully = false;
                }

                if (n2Successfully && !_q.IsPrime())
                {
                    Console.WriteLine(Strings.SecondNumberIsntPrime);
                    n2Successfully = false;
                }
            } while (!n1Successfully || !n2Successfully);

            while (true)
            {
                Console.WriteLine(Strings.AvailableOptions);

                bool isSuccessfullyParsed;
                do
                {
                    Console.Write(Strings.EnterOptionNumber);
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
                            Console.WriteLine(Strings.IncorrectOption);
                            break;
                    }
                } while (!isSuccessfullyParsed);
            }
        }

        private static async Task ConsoleInputEncrypting()
        {
            Console.WriteLine(Strings.TextToEncryptDecrypt);
            var text = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrEmpty(text.Trim()))
            {
                Console.WriteLine(Strings.EmptyInput);
                return;
            }

            var publicKey = PublicKeyGenerator.Generate(_p, _q);
            Console.WriteLine(Strings.PublicKey, publicKey);
            
            Console.WriteLine(Strings.EncryptingProcess);
            var (encrypt, encryptionTime) = await EncryptingTask(text, publicKey);
            Console.WriteLine(Strings.EncryptionTime, encryptionTime);
            Console.WriteLine(Strings.EncryptedBytes, StringUtils.CollectionToReadable(encrypt));

            var privateKey = PrivateKeyGenerator.Generate(publicKey);
            Console.WriteLine(Strings.PrivateKey, privateKey);

            Console.WriteLine(Strings.DecryptingProcess);
            var (decrypt, decryptionTime) = await DecryptingTask(encrypt, privateKey);

            Console.WriteLine(Strings.DecryptionTime, decryptionTime);
            Console.WriteLine(Strings.DecryptedText, decrypt);

            Console.WriteLine(Strings.TotalTime, encryptionTime + decryptionTime);
        }

        private static async Task ProcessTextFile()
        {
            Console.Write(Strings.FilePath);
            var filePath = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrEmpty(filePath.Trim()) || !File.Exists(filePath))
            {
                Console.WriteLine(Strings.InvalidFilePath);
                return;
            }

            var text = string.Join(" ", File.ReadAllLines(filePath));

            if (string.IsNullOrEmpty(text.Trim()))
            {
                Console.WriteLine(Strings.NoContentToEncrypt);
                return;
            }

            var publicKey = PublicKeyGenerator.Generate(_p, _q);
            Console.WriteLine(Strings.PublicKey, publicKey);
            
            Console.WriteLine(Strings.EncryptingProcess);
            var (encrypt, encryptionTime) = await EncryptingTask(text, publicKey);
            Console.WriteLine(Strings.EncryptionTime, encryptionTime);

            var encryptedFilePath =
                $"{Path.GetDirectoryName(filePath) ?? "C:\\"}\\{Path.GetFileNameWithoutExtension(filePath)}_encrypted.txt";
            using (var writer = new StreamWriter(encryptedFilePath))
                await writer.WriteAsync(StringUtils.CollectionToReadable(encrypt));

            Console.WriteLine(Strings.EncryptionFilePath, encryptedFilePath);

            var privateKey = PrivateKeyGenerator.Generate(publicKey);
            Console.WriteLine(Strings.PrivateKey, privateKey);

            Console.WriteLine(Strings.DecryptingProcess);
            var (decrypt, decryptionTime) = await DecryptingTask(encrypt, privateKey);

            var decryptedFilePath =
                $"{Path.GetDirectoryName(filePath) ?? "C:\\"}\\{Path.GetFileNameWithoutExtension(filePath)}_decrypted.txt";
            using (var writer = new StreamWriter(decryptedFilePath))
                await writer.WriteAsync(decrypt);

            Console.WriteLine(Strings.DecryptionTime, decryptionTime);
            Console.WriteLine(Strings.DecryptionFilePath, decryptedFilePath);

            Console.WriteLine(Strings.TotalTime, encryptionTime + decryptionTime);
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