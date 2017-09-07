using System;

namespace Sandbox
{
    internal class Program
    {
        private const string TestMessage = "this is a test message";

        private static void Main(string[] args)
        {
            string encrypt = caeserCryptographic.Encrypt(Console.ReadLine());
            Console.WriteLine(encrypt);
            Console.WriteLine(caeserCryptographic.Decrypt(encrypt));
        }

        private void TestCaeserCryptographicMethod()
        {
            CaeserCryptographicMethod caeserCryptographic = new CaeserCryptographicMethod();
            AssertTest("Caeser Cipher", caeserCryptographic.Decrypt(caeserCryptographic.Encrypt(TestMessage)));
        }

        private static void AssertTest(string testName, string messageAfter)
        {
            ConsoleColor color = Console.ForegroundColor;

            if (messageAfter != TestMessage)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{testName} failed. Returned: {messageAfter}");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{testName} passed! Returned: {messageAfter}");
            }

            Console.ForegroundColor = color;
        }
    }
}
