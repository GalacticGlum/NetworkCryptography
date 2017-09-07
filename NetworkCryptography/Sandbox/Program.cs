using System;

namespace Sandbox
{
    internal static class Program
    {
        private const string TestMessage = "this is a test message";

        private static void Main(string[] args)
        {
            TestCaeserCryptographicMethod();

            Console.ReadLine();
        }

        private static void TestCaeserCryptographicMethod()
        {
            CaeserCryptographicMethod caeserCryptographic = new CaeserCryptographicMethod();
            AssertTest("Caeser Cipher", caeserCryptographic.Decrypt(caeserCryptographic.Encrypt(TestMessage)));
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
