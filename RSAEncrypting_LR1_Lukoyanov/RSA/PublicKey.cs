namespace RSAEncrypting_LR1_Lukoyanov
{
    public class PublicKey
    {
        public BigInt E { get; }
        public BigInt N { get; }
        public BigInt Phi { get; }

        public PublicKey(BigInt e, BigInt n, BigInt phi)
        {
            E = e;
            N = n;
            Phi = phi;
        }
    }
}