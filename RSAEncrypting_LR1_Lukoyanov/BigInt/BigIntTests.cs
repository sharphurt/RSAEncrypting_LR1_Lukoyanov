using System;
using NUnit.Framework;

namespace RSAEncrypting_LR1_Lukoyanov.BigInt
{
    [TestFixture]
    public class BigIntTests
    {
        private void OperationTest(string first, string second, string result, Func<RSAEncrypting_LR1_Lukoyanov.BigInt.BigInt, RSAEncrypting_LR1_Lukoyanov.BigInt.BigInt, RSAEncrypting_LR1_Lukoyanov.BigInt.BigInt> func)
        {
            var a = new BigInt(first);
            var b = new BigInt(second);
            var r = func(a, b);
            Assert.True(r == new BigInt(result), $"Result is {r}");
        }

        [TestCase("373351898", "22242218987747101", "22242219361098999")]
        [TestCase("76264", "668554717734", "668554793998")]
        [TestCase("264523115614", "657282219887", "921805335501")]
        public void PositiveNumbersAdditionTest(string a, string b, string result) =>
            OperationTest(a, b, result, BigInt.Add);

        [TestCase("-564508", "-277041158412", "-277041722920")]
        [TestCase("-961428656203", "-355352731734", "-1316781387937")]
        [TestCase("-1303996751177117", "-354670522247", "-1304351421699364")]
        public void NegativeNumbersAdditionTest(string a, string b, string result) =>
            OperationTest(a, b, result, BigInt.Add);

        [TestCase("-6982427152952915", "537132162176", "-6981890020790739")]
        [TestCase("114389357256", "-566272303396", "-451882946140")]
        [TestCase("2945541596428113", "-804204726037", "2944737391702076")]
        public void PositiveWithNegativeNumbersAdditionTest(string a, string b, string result) =>
            OperationTest(a, b, result, BigInt.Add);

        [TestCase("680353957225", "156269694756", "524084262469")]
        [TestCase("9417831233798163", "140824048150", "9417690409750013")]
        [TestCase("907763926954791", "833864581", "907763093090210")]
        public void PositiveNumbersSubtractionWithPositiveResultTest(string a, string b, string result) =>
            OperationTest(a, b, result, BigInt.Sub);

        [TestCase("246841642", "99246340857053", "-99246094015411")]
        [TestCase("779310694956", "865409862226", "-86099167270")]
        [TestCase("765398045", "650773379169769", "-650772613771724")]
        public void PositiveNumbersSubtractionWithNegativeResultTest(string a, string b, string result) =>
            OperationTest(a, b, result, BigInt.Sub);

        [TestCase("-96257428842955", "504256139", "-96257933099094")]
        [TestCase("-40374278", "95346962764712", "-95347003138990")]
        [TestCase("60450518", "-83758422519287", "83758482969805")]
        public void PositiveWithNegativeNumberSubtractionTest(string a, string b, string result) =>
            OperationTest(a, b, result, BigInt.Sub);
        
        [TestCase("337435024824", "648458527447", "218812619306412930344328")]
        [TestCase("854052199010127", "181667316", "155153370718067628909132")]
        [TestCase("48733084", "219700348582626", "10706675542306393798584")]
        public void PositiveNumbersMultiplicationTest(string a, string b, string result) =>
            OperationTest(a, b, result, BigInt.DefaultMultiplication);

        [TestCase("337435024824", "648458527447", "218812619306412930344328")]
        [TestCase("854052199010127", "181667316", "155153370718067628909132")]
        [TestCase("48733084", "219700348582626", "10706675542306393798584")]
        public void PositiveNumbersKaratsubaMultiplicationTest(string a, string b, string result) =>
            OperationTest(a, b, result, BigInt.KaratsubaMultiplication);

        [TestCase("-170009390880219", "-6279312672", "1067542122513160400835168")]
        [TestCase("-8452134940", "-35385773834565", "299085335406064616201100")]
        [TestCase("-86200735561435224414", "-34107843654934", "2940121211469742033175496268358676")]
        public void NegativeNumbersMultiplicationTest(string a, string b, string result) =>
            OperationTest(a, b, result, BigInt.KaratsubaMultiplication);

        [TestCase("87515074", "-11746782888946", "-1028020573788042972004")]
        [TestCase("989678353967768", "-7508610631", "-7431109409872963836141608")]
        [TestCase("-416532428020962", "5242874658", "-2183827311106310762580996")]
        public void PositiveWithNegativeNumbersMultiplicationTest(string a, string b, string result) =>
            OperationTest(a, b, result, BigInt.KaratsubaMultiplication);

        [TestCase("567491202040867", "669969525", "847040")]
        [TestCase("793485891480702", "943855842", "840685")]
        [TestCase("14949101481534", "831977559", "17968")]
        public void PositiveNumbersDivisionTest(string a, string b, string result) =>
            OperationTest(a, b, result, BigInt.Div);

        [TestCase("1234", "45", "27")]
        [TestCase("-13547924586232047449092847", "-7069120881602", "1916493551764")]
        [TestCase("-27677441069003275177528084", "-961966731486276447139494", "28")]
        [TestCase("-34180623080682025523935209", "-68682297474100378675", "497662")]
        public void NegativeNumbersDivisionTest(string a, string b, string result) =>
            OperationTest(a, b, result, BigInt.Div);


        [TestCase("-16645927926600509040026869", "883018282001805561", "-18851170")]
        [TestCase("78722611001381133015934480", "-731822566392073670", "-107570625")]
        [TestCase("87850140823630", "-965251350836769424", "0")]
        public void PositiveWithNegativeNumbersDivisionTest(string a, string b, string result) =>
            OperationTest(a, b, result, BigInt.Div);


        [TestCase("235517282517573", "221150240", "17175973")]
        [TestCase("361851169680067", "54702242139", "50540172721")]
        [TestCase("348091531494017", "6362305761482863276", "348091531494017")]
        public void PositiveNumbersModTest(string a, string b, string result) =>
            OperationTest(a, b, result, BigInt.Mod);
    }
}