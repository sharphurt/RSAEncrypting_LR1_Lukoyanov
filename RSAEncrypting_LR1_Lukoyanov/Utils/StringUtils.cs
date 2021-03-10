using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSAEncrypting_LR1_Lukoyanov
{
    public static class StringUtils
    {
        public static byte[] ConvertToBytes(string text, Encoding encoding) =>
            Encoding.Convert(Encoding.Default, encoding, Encoding.Default.GetBytes(text).ToArray());

        public static string ConvertBytesToString(byte[] bytes, Encoding encoding)
        {

          //  var e = encoding.GetString(bytes);
            var chars = new char[encoding.GetCharCount(bytes, 0, bytes.Length)];
            encoding.GetChars(bytes, 0, bytes.Length, chars, 0);
            return new string(chars);
        }

        public static string GetDifference(string s1, string s2)
        {
            var set1 = s1.Split(' ').Distinct().ToArray();
            var set2 = s2.Split(' ').Distinct().ToArray();

            var diff = set2.Count() > set1.Count() ? set2.Except(set1).ToList() : set1.Except(set2).ToList();
            return diff.Any() ? string.Join("\n", diff) : "There's no difference";
        }

        public static string CollectionToReadable(IEnumerable<object> arr) =>
            $"[{string.Join(", ", arr.Select(b => b.ToString()))}]";
    }
}