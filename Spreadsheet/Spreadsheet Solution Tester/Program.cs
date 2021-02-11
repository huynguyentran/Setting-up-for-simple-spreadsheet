using FormulaEvaluator;
using System;

namespace Spreadsheet_Solution_Tester
{

    class Program
    {


        static int SimpleLookup(string s)
        {
            return 7;
        }

        static int AdvanceLookup(string s)
        {
            if (s == "a1")
            {
                return 1;
            }

            if (s == "A7")
            {
                return 2;
            }

            if (s == "aa1")
            {
                return 3;
            }
            else throw new ArgumentException("unknow variable");

        }

        static void Main(string[] args)
        {


        
            //Normal test, if the test is correct, the statement will print true.
            Console.WriteLine("If the test is correct, the statement will print True.");
            Console.WriteLine("Test 1:");
            Console.WriteLine((Evaluator.Evaluate("1+a111", SimpleLookup)) == 8);
            Console.WriteLine("Test 2:");
            Console.WriteLine((Evaluator.Evaluate("1+aaa111", SimpleLookup)) == 8);
            Console.WriteLine("Test 3:");
            Console.WriteLine((Evaluator.Evaluate("1+aaa11111", SimpleLookup)) == 8);
            Console.WriteLine("Test 4:");
            Console.WriteLine((Evaluator.Evaluate("1+aaaa1", SimpleLookup)) == 8);
            Console.WriteLine("Test 5:");
            Console.WriteLine((Evaluator.Evaluate("  1   +  2  + a111 ", SimpleLookup)) == 10);
            Console.WriteLine("Test 6:");
            Console.WriteLine((Evaluator.Evaluate("(1+2)*A7", AdvanceLookup)) == 6);
            Console.WriteLine("Test 7:");
            Console.WriteLine((Evaluator.Evaluate("aa1/7", AdvanceLookup)) == 0);
            Console.WriteLine("Test 8:");
            Console.WriteLine((Evaluator.Evaluate("((1+2))", SimpleLookup)) == 3);
            Console.WriteLine("Test 9:");
            Console.WriteLine("Test 10:");
            Console.WriteLine((Evaluator.Evaluate("1+2*5", SimpleLookup)) == 11);
            Console.WriteLine("Test 11:");
            Console.WriteLine((Evaluator.Evaluate("(((1+2)*5)+3)/2", SimpleLookup)) == 9);
     


            Console.WriteLine("");
            Console.WriteLine("If the test is correct, the statement will print Passed.");
            Console.WriteLine("Test 1:");
            try
            {
                Console.WriteLine(Evaluator.Evaluate("++", SimpleLookup));
                Console.WriteLine("Failed");
            }
            catch
            {
                Console.WriteLine("Passed");
            }
            Console.WriteLine("Test 2:");
            try
            {
                Console.WriteLine(Evaluator.Evaluate("(1+2", SimpleLookup));
                Console.WriteLine("Failed");
            }
            catch
            {
                Console.WriteLine("Passed");
            }
            Console.WriteLine("Test 3:");
            try
            {
                Console.WriteLine(Evaluator.Evaluate("5/0", SimpleLookup));
                Console.WriteLine("Failed");
            }
            catch
            {
                Console.WriteLine("Passed");
            }
            Console.WriteLine("Test 4:");
            try
            {
                Console.WriteLine(Evaluator.Evaluate("1+2s", SimpleLookup));
                Console.WriteLine("Failed");
            }
            catch
            {
                Console.WriteLine("Passed");
            }
            Console.WriteLine("Test 5:");
            try
            {
                Console.WriteLine(Evaluator.Evaluate("1+2)", SimpleLookup));
                Console.WriteLine("Failed");
            }
            catch
            {
                Console.WriteLine("Passed");
            }
            Console.WriteLine("Test 6:");
            try
            {
                Console.WriteLine(Evaluator.Evaluate("(((())))", SimpleLookup));
                Console.WriteLine("Failed");
            }
            catch
            {
                Console.WriteLine("Passed");
            }
            Console.WriteLine("Test 7:");
            try
            {
                Console.WriteLine(Evaluator.Evaluate("a111+3", AdvanceLookup));
                Console.WriteLine("Failed");
            }
            catch
            {
                Console.WriteLine("Passed");
            }
            Console.WriteLine("Test 8:");
            try
            {
                Console.WriteLine(Evaluator.Evaluate("5+7+(5)8",SimpleLookup));
                Console.WriteLine("Failed");
            }
            catch
            {
                Console.WriteLine("Passed");
            }

            Console.Read();




        }
    }
}
