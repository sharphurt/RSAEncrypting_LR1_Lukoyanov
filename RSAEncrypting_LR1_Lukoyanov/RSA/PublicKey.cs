namespace RSAEncrypting_LR1_Lukoyanov.RSA
{
    public class PublicKey
    {
        public BigInt.BigInt E { get; }
        public BigInt.BigInt N { get; }
        public BigInt.BigInt Phi { get; }

        public PublicKey(BigInt.BigInt e, BigInt.BigInt n, BigInt.BigInt phi)
        {
            E = e;
            N = n;
            Phi = phi;
        }

        public override string ToString() => $"(E: {E}, N: {N})";
    }
}