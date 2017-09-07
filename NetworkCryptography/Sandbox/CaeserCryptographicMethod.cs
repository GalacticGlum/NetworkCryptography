namespace Sandbox
{
    public class CaeserCryptographicMethod : ICryptographicMethod
    {
        private const int ShiftOffset = 3;

        private static string Shift(string message, int offset)
        {
            char[] result = new char[message.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = (char)(message[i] + offset);
            }

            return new string(result);
        }

        public string Encrypt(string message)
        {
            return Shift(message, ShiftOffset);
        }

        public string Decrypt(string encryptedMessage)
        {
            return Shift(encryptedMessage, -ShiftOffset);
        }
    }
}
