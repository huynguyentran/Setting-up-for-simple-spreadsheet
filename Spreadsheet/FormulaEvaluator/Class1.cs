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

        /*
         * A method that evaluates integer arithmetic expressions written using standard infix notation.
         * This method will take in a string input and a delegate from the user. The delegate will have to 
         * find the correct variables and turn the variables into int numbers. 
         * 
         *@param    String input        The string input that contains the integer arithmetic
         *          Lookup variable     The user own delegate to look up the variables
         *
         *@Return   int                 The result number of the arithmetic expressions
         * 
         *@throws                       Invalid integer arithmetic
         * 
         */
        public static int Evaluate(string input, Lookup variable)
        {
            //Splits up the string input and put the tokens into an array. 
            string[] substrings = Regex.Split(input, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");

            //Creates an operator stack and a value stack. 
            Stack<int> valueStack = new Stack<int>();
            Stack<string> operatorStack = new Stack<string>();
            foreach (string t in substrings)
            {
                //In case of null and empty space, the method will continue to the next token of the substrings array. 
                if (t.Equals(""))
                {
                    continue;
                }
                if (t.Equals(" "))
                {
                    continue;
                }

                //If token is a number. 
                if (int.TryParse(t, out int number))
                {
                    //If * or / is on top of the operatorStack. Extension IsOnTop was created to simplied the logic of the method.
                    if (operatorStack.IsOnTop("*") || operatorStack.IsOnTop("/"))
                    {
                        //The private helper method will throws if it is a bad experssion. 
                        valueStackEmptyCheck(valueStack.Count);
                        /* Performs the calculation between the token and the number that already in the value stack. 
                         * The calculation is peformed as follow: 
                         * The value on top of the value stack (operator on top of the operator stack) token.
                         */
                        int v = Calculator(number, valueStack.Pop(), operatorStack.Pop());
                        //Pushes the new value into the value stack.
                        valueStack.Push(v);
                        //Moves to the next token.
                        continue;
                    }
                    else
                    {
                        //Else, pushes the number into the value stack and moves to the next token.
                        valueStack.Push(number);
                        continue;
                    }
                }

                //If token is a + or -
                if (t.Equals("+") || t.Equals("-"))
                {
                    //If + or - is on top of the operatorStack.
                    if (operatorStack.IsOnTop("+") || operatorStack.IsOnTop("-"))
                    {
                        //The private helper method will throws if it is a bad experssion. 
                        stackThrowLessThan2(valueStack.Count);
                        /* Performs the calculation between the numbers that already in the . 
                        * The calculation is peformed as follow:
                        * The value on top of the value stack (operator on top of the operator stack) The second top value of the value stack.
                        */
                        int value = Calculator(valueStack.Pop(), valueStack.Pop(), operatorStack.Pop());
                        //Pushes the new value into the value stack.
                        valueStack.Push(value);
                    }
                    //Pushes the token into the token stack.
                    operatorStack.Push(t);
                    continue;
                }

                //If token is a * or /
                if (t.Equals("*") || t.Equals("/"))
                {
                    //Pushes the token into the token stack.
                    operatorStack.Push(t);
                    continue;
                }

                //If token is a ( 
                if (t.Equals("("))
                {
                    //Pushes the token into the token stack.
                    operatorStack.Push(t);
                    continue;
                }


                //If token is )
                if (t.Equals(")"))
                {
                    //If + or - is on top of the operatorStack. 
                    if (operatorStack.IsOnTop("+") || operatorStack.IsOnTop("-"))
                    {
                        //Performs the same calculation as above.
                        stackThrowLessThan2(valueStack.Count);
                        int v = Calculator(valueStack.Pop(), valueStack.Pop(), operatorStack.Pop());
                        valueStack.Push(v);
                    }
                    //If there is no ( after the calculation, throws an error. 
                    if (!operatorStack.IsOnTop("("))
                    {
                        throw new ArgumentException("Found ) without proper (");
                    }
                    operatorStack.Pop();

                    //If / or * is on top of the operatorStack. 
                    if (operatorStack.IsOnTop("*") || operatorStack.IsOnTop("/"))
                    {
                        //Performs the same calculation as above.
                        stackThrowLessThan2(valueStack.Count);
                        int v = Calculator(valueStack.Pop(), valueStack.Pop(), operatorStack.Pop());
                        valueStack.Push(v);
                    }
                    continue;
                }


                //If token is a variable
                if (VariablesVerification(t))
                {
                    if (operatorStack.IsOnTop("*") || operatorStack.IsOnTop("/"))
                    {
                        //Throws invalid expression.
                        valueStackEmptyCheck(valueStack.Count);
                        //
                        try
                        {
                            //Replaces the token with the correct int and calculates the function.
                            int v = Calculator(variable(t), valueStack.Pop(), operatorStack.Pop());
                            valueStack.Push(v);
                            continue;
                        }

                        catch
                        {
                            //If the variable does not exist, throws 
                            throw new ArgumentException("Can not find variable.");
                        }

                    }
                    else
                    {
                        //Else pushes the variable onto the value stack.
                        valueStack.Push(variable(t));
                    }
                }
            }

            //If the condition is met and the expression is valid, returns the final value.
            if (operatorStack.Count == 0)
            {
                valueStackEmptyCheck(valueStack.Count);
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


        /*
         * A private helper method to perform simple calculation.
         * 
         * @param   int val1    The first value
         *          int va2     The second value
         *          string op   The operator
         *          
         *@Return   int value   The new value after calculation.
         * 
         *@throws   if there is a division by 0.
         * 
         */
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

        /*
      * A private helper to help identify the pattern of the variable.
      * variables consisting of one or more letters followed by one or more digits. 
      * Letters can be lowercase or uppercase.
      * 
      * @param  string v    variable we needs to identify.
      * 
      * @Return bool        If the variable is valid, returns true. Otherwise returns false.   
      * 
      * @throws             If the variable is invalid.
      * 
      */
        private static bool VariablesVerification(string v)
        {

            Regex regex = new Regex(@"\b[a-zA-Z]+\d+\b");
            bool boo = regex.IsMatch(v);
            if (boo == false)
            {
                throw new ArgumentException("Invalid variable");
            }

            return regex.IsMatch(v);
        }

        /*
         *A private method to throw if there is invalid expression.
         *
         *@throws             If the expression is invalid.
         */
        private static void stackThrowLessThan2(int e)
        {
            if (e < 2)
            {
                throw new ArgumentException("Invalid expression.");
            }
        }

        /*
        *A private method to throw if there is invalid expression.
        *
        *@throws             If the expression is invalid.
        */
        private static void valueStackEmptyCheck(int e)
        {
            if (e == 0)
            {
                throw new ArgumentException("Invalid expression.");
            }
        }


    }

    static class StackExtentions
    {

        /*
         * This method will check the string on top of the stack.
         * 
         * @param   String str  The string on top of the stack. 
         * @Return  bool        Returns true if the string str is on top of the stack. Otherwise returns false
         */
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
