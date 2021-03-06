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
            Assert.True(sum == new BigInt(result));
        }
        
        [TestCase("373351060898", "228987747101", "602338807999")]
        [TestCase("515163236966", "983768945586", "1498932182552")]
        [TestCase("264523115614", "657282219887", "921805335501")]
        public void PositiveNumbersAdditionTest(string a, string b, string result) => AdditionTest(a, b, result);

        [TestCase("-937640468489", "-866539114375", "-1804179582864")]
        [TestCase("-961428656203", "-355352731734", "-1316781387937")]
        [TestCase("-247230724255", "-239667040148", "-486897764403")]
        public void NegativeNumbersAdditionTest(string a, string b, string result) => AdditionTest(a, b, result);

        [TestCase("991751949353", "-612455577642", "379296371711")]
        [TestCase("114389357256", "-566272303396", "-451882946140")]
        [TestCase("850932131180", "-147887437561", "703044693619")]
        [TestCase("314877628376", "-118974295998", "195903332378")]
        [TestCase("868121603029", "-901542663525", "-33421060496")]
        public void PositiveWithNegativeNumbersAdditionTest(string a, string b, string result) => AdditionTest(a, b, result);

        [TestCase("680353957225", "156269694756", "524084262469")]
        [TestCase("720728994561", "175425285778", "545303708783")]
        [TestCase("958267186706", "185743546914", "772523639792")]
        public void PositiveNumbersSubtractionWithPositiveResultTest(string a, string b, string result) =>
            SubtractionTest(a, b, result);


        [TestCase("346438802141", "544442125116", "-198003322975")]
        [TestCase("779310694956", "865409862226", "-86099167270")]
        [TestCase("262325668273", "625961113402", "-363635445129")]
        public void PositiveNumbersSubtractionWithNegativeResultTest(string a, string b, string result) =>
            SubtractionTest(a, b, result);

        [TestCase("538475591777", "538475591777", "0")]
        [TestCase("704782808812", "704782808812", "0")]
        [TestCase("983664262544", "983664262544", "0")]
        public void PositiveNumbersSubtractionWithZeroResultTest(string a, string b, string result) =>
            SubtractionTest(a, b, result);
        
        
    }
}