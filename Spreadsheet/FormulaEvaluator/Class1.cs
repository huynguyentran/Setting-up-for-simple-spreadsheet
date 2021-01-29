using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace FormulaEvaluator
{
    public delegate int Lookup(String v);


    public static class Evaluator
    {



        public static int Evaluate(string input, Lookup variable)
        {
            string[] substrings = Regex.Split(input, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");
            Stack<int> valueStack = new Stack<int>();
            Stack<string> operatorStack = new Stack<string>();
            foreach (string t in substrings)
            {
                //Extension method 
                if (t.Equals(""))
                {
                    continue;
                }
                if (t.Equals(" "))
                {
                    continue;
                }

                if (int.TryParse(t, out int number))
                {
                    if (operatorStack.IsOnTop("*") || operatorStack.IsOnTop("/"))
                    {
                        valueStackEmptyCheck(valueStack.Count);
                        int v = Calculator(number, valueStack.Pop(), operatorStack.Pop());
                        valueStack.Push(v);
                        continue;
                    }
                    else
                    {
                        valueStack.Push(number);
                        continue;
                    }


                }

                if (t.Equals("+") || t.Equals("-"))
                {
                    if (operatorStack.IsOnTop("+") || operatorStack.IsOnTop("-"))
                    {
                        stackThrowLessThan2(valueStack.Count);
                        int value = Calculator(valueStack.Pop(), valueStack.Pop(), operatorStack.Pop());
                        valueStack.Push(value);
                    }
                    operatorStack.Push(t);
                    continue;
                }

                if (t.Equals("*") || t.Equals("/"))
                {
                    operatorStack.Push(t);
                    continue;
                }

                if (t.Equals("("))
                {
                    operatorStack.Push(t);
                    continue;
                }

                if (t.Equals(")"))
                {
                    if (operatorStack.IsOnTop("+") || operatorStack.IsOnTop("-"))
                    {
                        stackThrowLessThan2(valueStack.Count);
                        int v = Calculator(valueStack.Pop(), valueStack.Pop(), operatorStack.Pop());
                        valueStack.Push(v);
                    }

                    if (!operatorStack.IsOnTop("("))
                    {
                        throw new ArgumentException("No ( found.");
                    }
                    operatorStack.Pop();

                    if (operatorStack.IsOnTop("*") || operatorStack.IsOnTop("/"))
                    {
                        stackThrowLessThan2(valueStack.Count);
                        int v = Calculator(valueStack.Pop(), valueStack.Pop(), operatorStack.Pop());
                        valueStack.Push(v);
                    }
                    continue;
                }


                if (VariablesVerification(t))
                {
                    if (operatorStack.IsOnTop("*") || operatorStack.IsOnTop("/"))
                    {
                        valueStackEmptyCheck(valueStack.Count);
                        try
                        {
                            int v = Calculator(variable(t), valueStack.Pop(), operatorStack.Pop());
                            valueStack.Push(v);
                            continue;
                        }

                        catch
                        {
                            throw new ArgumentException("Can not find variable.");
                        }

                    }
                    else
                    {
                        valueStack.Push(variable(t));
                    }
                }




            }

            if (operatorStack.Count == 0)
            {
                return valueStack.Pop();
            }
            else if (operatorStack.Count == 1)
            {
                if (operatorStack.IsOnTop("+") || operatorStack.IsOnTop("-"))
                {
                    stackThrowLessThan2(valueStack.Count);
                    return Calculator(valueStack.Pop(), valueStack.Pop(), operatorStack.Pop());
                }

                throw new ArgumentException("Invalid: operator remains with no values");
            }
            else
            {
                throw new ArgumentException("Invalid: operator remains with no values");
            }
        }


        private static int Calculator(int val1, int val2, string op)
        {
            switch (op)
            {
                case "*":
                    return val2 * val1;
                case "/":
                    if (val1 == 0)
                    {
                        throw new ArgumentException("Can not divide by 0");
                    }
                    return val2 / val1;
                case "+":
                    return val2 + val1;
                case "-":
                    return val2 - val1;
            }
            return 0;
        }

        public static bool VariablesVerification(string v)
        {
            Regex regex = new Regex(@"\b[a-zA-Z]+\d+\b");
            bool boo = regex.IsMatch(v);
            if (boo == false)
            {
                throw new ArgumentException("Invalid variable");
            }

            return regex.IsMatch(v);
        }

        private static void stackThrowLessThan2(int e)
        {
            if (e < 2)
            {
                throw new ArgumentException("Invalid: The value stack contains fewer than 2 values.");
            }
        }

        private static void valueStackEmptyCheck(int e)
        {
            if (e == 0)
            {
                throw new ArgumentException("Invalid: the value stack is empty.");
            }
        }


    }

    static class StackExtentions
    {
        public static bool IsOnTop<T>(this Stack<T> stack, string str)
        {


            if (stack.Count < 1)
            {
                return false;
            }
            return stack.Peek().Equals(str);
        }
    }





}
