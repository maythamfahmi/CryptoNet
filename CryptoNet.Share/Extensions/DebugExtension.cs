// <copyright file="Program.cs" company="NextBix" year="2021">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Maytham Fahmi</author>
// <date>17-12-2021 12:18:44</date>
// <summary>part of CryptoNet project</summary>

using System.Runtime.CompilerServices;

namespace CryptoNet.Share.Extensions
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

                throw new InvalidOperationException(message);
            }
        }
    }
}
