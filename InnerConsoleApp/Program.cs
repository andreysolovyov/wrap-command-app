using System;

namespace InnerConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Program run.");
            Console.WriteLine("Arguments passes: {0}", string.Join(";", args));
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
