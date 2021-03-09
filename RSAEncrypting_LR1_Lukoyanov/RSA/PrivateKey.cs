namespace RSAEncrypting_LR1_Lukoyanov.RSA
{
    public class PrivateKey
    {
        public BigInt.BigInt D { get; }
        public BigInt.BigInt N { get; }

        public PrivateKey(BigInt.BigInt d, BigInt.BigInt n)
        {
            D = d;
            N = n;
        }
    }
}