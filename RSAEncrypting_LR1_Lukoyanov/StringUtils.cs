using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSAEncrypting_LR1_Lukoyanov
{
    public static class StringUtils
    {
        public static byte[] ConvertToAsciiBytes(string text)
            => Encoding.Convert(Encoding.UTF8, Encoding.ASCII, Encoding.UTF8.GetBytes(text)).ToArray();

        public static string ConvertToString(byte[] bytes)
        {
            var asciiChars = new char[Encoding.ASCII.GetCharCount(bytes, 0, bytes.Length)];
            Encoding.ASCII.GetChars(bytes, 0, bytes.Length, asciiChars, 0);
            return new string(asciiChars);
        }
        
        public static string GetDifference(string s1, string s2)
        {
            var set1 = s1.Split(' ').Distinct().ToArray();
            var set2 = s2.Split(' ').Distinct().ToArray();

            var diff = set2.Count() > set1.Count() ? set2.Except(set1).ToList() : set1.Except(set2).ToList();
            return diff.Any() ? string.Join("\n", diff) : "There's no difference";
        }

        public static string CollectionToReadable(IEnumerable<object> arr) => $"[{string.Join(", ", arr.Select(b => b.ToString()))}]";
    }
}