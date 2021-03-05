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
        [TestCase("508274117379", "261586535666", "769860653045")]
        [TestCase("390887621301", "913330187028", "1304217808329")]
        [TestCase("140258438736", "134379501732", "274637940468")]
        [TestCase("651412133636", "733015581700", "1384427715336")]
        [TestCase("368854515297", "340776457502", "709630972799")]
        public void PositiveNumbersSumTest(string firstTerm, string secondTerm, string result)
        {
            var a = new BigInt(firstTerm);
            var b = new BigInt(secondTerm);
    
            var sum = a + b;

            Assert.True(sum == new BigInt(result));
        }
    }
}