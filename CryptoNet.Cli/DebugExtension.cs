using System.Runtime.CompilerServices;

namespace CryptoNet.Cli
{
    public static class Debug
    {
        public static void Assert(bool condition, [CallerArgumentExpression(nameof(condition))] string message = "")
        {
            if (condition)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Assert passed: {message}");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Assert failed: {message}");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }
}
