using System.Numerics;
using NetworkCryptography.Core.DataStructures;

namespace NetworkCryptography.Core
{
    public class RsaCryptographicMethod 
    {
        private readonly RsaKeySet keys;

        public RsaCryptographicMethod(RsaKeySet keys)
        {
            this.keys = keys;
        }

        public int[] Encrypt(string plaintext)
        {
            int[] ciphertext = new int[plaintext.Length];
            for (int i = 0; i < ciphertext.Length; i++)
            {
                BigInteger result = BigInteger.ModPow((int)plaintext[i], keys.PublicExponent, keys.Modulus);
                ciphertext[i] = (int) result;
            }

            return ciphertext;
        }

        public string Decrypt(int[] ciphertext)
        {
            string plaintext = string.Empty;
            foreach (int value in ciphertext)
            {
                BigInteger result = BigInteger.ModPow(value, keys.PrivateExponent, keys.Modulus);
                plaintext += (char) result;
            }

            return plaintext;
        }
    }
}
