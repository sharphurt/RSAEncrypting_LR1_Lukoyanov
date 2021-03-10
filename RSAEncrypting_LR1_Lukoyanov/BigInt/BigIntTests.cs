using System;
using NUnit.Framework;

namespace RSAEncrypting_LR1_Lukoyanov.BigInt
{
    [TestFixture]
    public class BigIntTests
    {
        private static void OperationTest(string first, string second, string result, Func<BigInt, BigInt, BigInt> func)
        {
            var a = new BigInt(first);
            var b = new BigInt(second);
            var r = func(a, b);
            Assert.True(r == new BigInt(result), $"Result is {r}");
        }

        [TestCase("373351898", "22242218987747101", "22242219361098999")]
        [TestCase("76264", "668554717734", "668554793998")]
        [TestCase("264523115614", "657282219887", "921805335501")]
        [TestCase("-564508", "-277041158412", "-277041722920")]
        [TestCase("-961428656203", "-355352731734", "-1316781387937")]
        [TestCase("-1303996751177117", "-354670522247", "-1304351421699364")]
        [TestCase("-6982427152952915", "537132162176", "-6981890020790739")]
        [TestCase("114389357256", "-566272303396", "-451882946140")]
        [TestCase("2945541596428113", "-804204726037", "2944737391702076")]
        public void AdditionTest(string a, string b, string result) =>
            OperationTest(a, b, result, BigInt.Add);

        [TestCase("680353957225", "156269694756", "524084262469")]
        [TestCase("-96257428842955", "504256139", "-96257933099094")]
        [TestCase("60450518", "-83758422519287", "83758482969805")]
        [TestCase("246841642", "99246340857053", "-99246094015411")]
        [TestCase("779310694956", "865409862226", "-86099167270")]
        [TestCase("765398045", "650773379169769", "-650772613771724")]
        public void SubtractionTest(string a, string b, string result) =>
            OperationTest(a, b, result, BigInt.Sub);

        [TestCase("0", "0", "0")]
        [TestCase("12472948294", "0", "0")]
        [TestCase("9438429044", "1", "9438429044")]
        [TestCase("337435024824", "648458527447", "218812619306412930344328")]
        [TestCase("87515074", "-11746782888946", "-1028020573788042972004")]
        [TestCase("-416532428020962", "5242874658", "-2183827311106310762580996")]
        [TestCase("-0", "-0", "0")]
        [TestCase("-12472948294", "0", "0")]
        [TestCase("9438429044", "-1", "-9438429044")]
        [TestCase("-170009390880219", "-6279312672", "1067542122513160400835168")]
        [TestCase("-8452134940", "-35385773834565", "299085335406064616201100")]
        [TestCase("-86200735561435224414", "-34107843654934", "2940121211469742033175496268358676")]
        public void DefaultMultiplicationTest(string a, string b, string result) =>
            OperationTest(a, b, result, BigInt.DefaultMultiplication);

        [TestCase("0", "0", "0")]
        [TestCase("12472948294", "0", "0")]
        [TestCase("9438429044", "1", "9438429044")]
        [TestCase("337435024824", "648458527447", "218812619306412930344328")]
        [TestCase("87515074", "-11746782888946", "-1028020573788042972004")]
        [TestCase("-416532428020962", "5242874658", "-2183827311106310762580996")]
        [TestCase("-0", "-0", "0")]
        [TestCase("-12472948294", "0", "0")]
        [TestCase("9438429044", "-1", "-9438429044")]
        [TestCase("-170009390880219", "-6279312672", "1067542122513160400835168")]
        [TestCase("-8452134940", "-35385773834565", "299085335406064616201100")]
        [TestCase("-86200735561435224414", "-34107843654934", "2940121211469742033175496268358676")]
        public void KaratsubaMultiplicationTest(string a, string b, string result) =>
            OperationTest(a, b, result, BigInt.KaratsubaMultiplication);

        [TestCase("-16645927926600509040026869", "883018282001805561", "-18851170")]
        [TestCase("78722611001381133015934480", "-731822566392073670", "-107570625")]
        [TestCase("87850140823630", "-965251350836769424", "0")]
        [TestCase("793485891480702", "943855842", "840685")]
        [TestCase("1234", "45", "27")]
        [TestCase("-13547924586232047449092847", "-7069120881602", "1916493551764")]
        public void DivisionTest(string a, string b, string result) =>
            OperationTest(a, b, result, BigInt.Div);


        [TestCase("235517282517573", "221150240", "17175973")]
        [TestCase("361851169680067", "54702242139", "50540172721")]
        [TestCase("348091531494017", "6362305761482863276", "348091531494017")]
        public void ModTest(string a, string b, string result) =>
            OperationTest(a, b, result, BigInt.Mod);

        [TestCase("1830184925627198302978", "3921821", "824103")]
        [TestCase("2408490402825726383127198302978", "4029048129472949", "1968106216971570")]
        [TestCase("8376327546273912271983029", "27432", "13229")]
        public void InvertingNumberTest(string number, string modulo, string result) =>
            OperationTest(number, modulo, result, BigInt.Inverse);
        
        [TestCase("531", "37", "67374767577675456090964712472670484305696898563133409832099875890530065117852060233269000630135939011")]
        [TestCase("218", "94", "6529958163664865318517555711970764270146878908402790237793722905845018438374614250241672431880016418454326340956829724578560982344596628673088753944481901021050751991370621051952091577350959887262859055660023054728167424")]
        [TestCase("849", "20", "37857663344776540371681891561864232691176600687505316008001")]
        [TestCase("1209204729748", "0", "1")]
        [TestCase("134890187496287389248", "1", "134890187496287389248")]
        [TestCase("-8", "2", "64")]
        [TestCase("8", "-2", "0")]
        public void PowerTest(string a, string b, string result) =>
            OperationTest(a, b, result, BigInt.Pow);
    }
}