using System.Linq;
using NUnit.Framework;

namespace RSAEncrypting_LR1_Lukoyanov
{
    [TestFixture]
    public class BigIntTests
    {
        private void AdditionTest(string first, string second, string result)
        {
            var a = new BigInt(first);
            var b = new BigInt(second);
            var sum = a + b;
            Assert.True(sum == new BigInt(result));
        }

        private void SubtractionTest(string first, string second, string result)
        {
            var a = new BigInt(first);
            var b = new BigInt(second);
            var sum = a - b;
            Assert.True(sum == new BigInt(result), $"Result is {sum}");
        }

        private void MultiplicationTest(string first, string second, string result)
        {
            var a = new BigInt(first);
            var b = new BigInt(second);
            var r = a * b;
            Assert.True(r == new BigInt(result), $"Result is {r}");
        }
        
        [TestCase("373351898", "22242218987747101", "22242219361098999")]
        [TestCase("76264", "668554717734", "668554793998")]
        [TestCase("264523115614", "657282219887", "921805335501")]
        public void PositiveNumbersAdditionTest(string a, string b, string result) => AdditionTest(a, b, result);

        [TestCase("-564508", "-277041158412", "-277041722920")]
        [TestCase("-961428656203", "-355352731734", "-1316781387937")]
        [TestCase("-1303996751177117", "-354670522247", "-1304351421699364")]
        public void NegativeNumbersAdditionTest(string a, string b, string result) => AdditionTest(a, b, result);

        [TestCase("-6982427152952915", "537132162176", "-6981890020790739")]
        [TestCase("114389357256", "-566272303396", "-451882946140")]
        [TestCase("2945541596428113", "-804204726037", "2944737391702076")]
        public void PositiveWithNegativeNumbersAdditionTest(string a, string b, string result) => AdditionTest(a, b, result);

        [TestCase("680353957225", "156269694756", "524084262469")]
        [TestCase("9417831233798163", "140824048150", "9417690409750013")]
        [TestCase("907763926954791", "833864581", "907763093090210")]
        public void PositiveNumbersSubtractionWithPositiveResultTest(string a, string b, string result) =>
            SubtractionTest(a, b, result);


        [TestCase("246841642", "99246340857053", "-99246094015411")]
        [TestCase("779310694956", "865409862226", "-86099167270")]
        [TestCase("765398045", "650773379169769", "-650772613771724")]
        public void PositiveNumbersSubtractionWithNegativeResultTest(string a, string b, string result) =>
            SubtractionTest(a, b, result);
        
        [TestCase("337435024824", "648458527447", "218812619306412930344328")]
        [TestCase("854052199010127", "181667316", "155153370718067628909132")]
        [TestCase("48733084", "219700348582626", "10706675542306393798584")]
        public void PositiveNumbersMultiplicationTest(string a, string b, string result) =>
            MultiplicationTest(a, b, result);

    }
}