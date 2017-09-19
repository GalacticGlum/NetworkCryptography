using System;

namespace Sandbox
{
    internal static class Program
    {
        private const string TestMessage = "this is a test message";

        private static void Main(string[] args)
        {
            new DesCryptographicMethod(new byte[] { 14, 14, 14 });

            TestCryptographicMethod<CaeserCryptographicMethod>();
            Console.ReadLine();
        }

        private static void TestCryptographicMethod<T>() where T : ICryptographicMethod, new()
        {
            ICryptographicMethod cryptographic = new T();
            AssertTest(typeof(T).Name, cryptographic.Decrypt(cryptographic.Encrypt(TestMessage)));
        }

        private static void AssertTest(string testName, string messageAfter)
        {
            ConsoleColor color = Console.ForegroundColor;
            bool hasPassed = messageAfter == TestMessage;
            string status = hasPassed ? "passed" : "failed";

            Console.ForegroundColor = hasPassed ? ConsoleColor.Green : ConsoleColor.Red;
            Console.WriteLine($"{testName} {status}. Returned: \"{messageAfter}\"");

            Console.ForegroundColor = color;
        }
    }
}
