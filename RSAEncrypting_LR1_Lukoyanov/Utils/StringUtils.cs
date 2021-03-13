using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSAEncrypting_LR1_Lukoyanov
{
    public static class StringUtils
    {
        public static int[] ConvertToNumbers(string text, Dictionary<char, int> alphabet) =>
            text.Where(alphabet.ContainsKey).Select(symbol => alphabet[symbol]).ToArray();

        public static string ConvertNumbersToString(IEnumerable<int> numbers, Dictionary<char, int> alphabet)
        {
            return new string(numbers.Select(n => alphabet.FirstOrDefault(x => x.Value == n).Key).ToArray());
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