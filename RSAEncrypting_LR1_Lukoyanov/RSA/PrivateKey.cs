namespace RSAEncrypting_LR1_Lukoyanov
{
    public class PrivateKey
    {
        public BigInt D { get; }
        public BigInt N { get; }

        public PrivateKey(BigInt d, BigInt n)
        {
            D = d;
            N = n;
        }
    }
}