using System.Linq;
using NUnit.Framework;

namespace RSAEncrypting_LR1_Lukoyanov
{
    [TestFixture]
    public class BigIntTests
    {
        [TestCase("373351060898", "228987747101", "602338807999")]
        [TestCase("515163236966", "983768945586", "1498932182552")]
        [TestCase("264523115614", "657282219887", "921805335501")]
        [TestCase("304016462019", "688764377059", "992780839078")]
        [TestCase("168844949392", "962174699301", "1131019648693")]
        public void PositiveNumbersSumTest(string firstTerm, string secondTerm, string result)
        {
            var a = new BigInt(firstTerm);
            var b = new BigInt(secondTerm);
    
            var sum = a + b;

            Assert.True(sum == new BigInt(result));
        }
        
        [TestCase("0000171133559480", "832465074731", "1003598634211")]
        [TestCase("523127349456", "000000000000798121408024", "1321248757480")]
        [TestCase("514377470667", "911565507162", "000000001425942977829")]
        [TestCase("000000226980868599", "00000000000608717583363", "835698451962")]
        [TestCase("0750057406853", "0000124208783069", "0000000000874266189922")]
        public void PositiveNumbersWithLeadingZerosSumTest(string firstTerm, string secondTerm, string result)
        {
            var a = new BigInt(firstTerm);
            var b = new BigInt(secondTerm);
    
            var sum = a + b;

            Assert.True(sum == new BigInt(result));
        }
    }
}