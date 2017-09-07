namespace Sandbox
{
    public interface ICryptographicMethod
    {
        string Encrypt(string message);
        string Decrypt(string encryptedMessage);
    }
}
