using System;
using SpreadsheetUtilities;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Formula f1 = new Formula("1+1");
            Formula f2 = new Formula("1+1");
            Console.WriteLine(f1==f2);
        }
    }
}
