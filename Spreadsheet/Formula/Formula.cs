// Skeleton written by Joe Zachary for CS 3500, September 2013
// Read the entire skeleton carefully and completely before you
// do anything else!

// Version 1.1 (9/22/13 11:45 a.m.)

// Change log:
//  (Version 1.1) Repaired mistake in GetTokens
//  (Version 1.1) Changed specification of second constructor to
//                clarify description of how validation works

// (Daniel Kopta) 
// Version 1.2 (9/10/17) 

// Change log:
//  (Version 1.2) Changed the definition of equality with regards
//                to numeric tokens


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  The allowed symbols are non-negative numbers written using double-precision 
    /// floating-point syntax (without unary preceeding '-' or '+'); 
    /// variables that consist of a letter or underscore followed by 
    /// zero or more letters, underscores, or digits; parentheses; and the four operator 
    /// symbols +, -, *, and /.  
    /// 
    /// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
    /// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable; 
    /// and "x 23" consists of a variable "x" and a number "23".
    /// 
    /// Associated with every formula are two delegates:  a normalizer and a validator.  The
    /// normalizer is used to convert variables into a canonical form, and the validator is used
    /// to add extra restrictions on the validity of a variable (beyond the standard requirement 
    /// that it consist of a letter or underscore followed by zero or more letters, underscores,
    /// or digits.)  Their use is described in detail in the constructor and method comments.
    /// </summary>
    public class Formula
    {
        private string validFormula;
        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer is the identity function, and the associated validator
        /// maps every string to true.  
        /// </summary>
        public Formula(String formula) :
            this(formula, s => s, s => true)
        {

        }

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically incorrect,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer and validator are the second and third parameters,
        /// respectively.  
        /// 
        /// If the formula contains a variable v such that normalize(v) is not a legal variable, 
        /// throws a FormulaFormatException with an explanatory message. 
        /// 
        /// If the formula contains a variable v such that isValid(normalize(v)) is false,
        /// throws a FormulaFormatException with an explanatory message.
        /// 
        /// Suppose that N is a method that converts all the letters in a string to upper case, and
        /// that V is a method that returns true only if a string consists of one letter followed
        /// by one digit.  Then:
        /// 
        /// new Formula("x2+y3", N, V) should succeed
        /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
        /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
        /// </summary>
        public Formula(String formula, Func<string, string> normalize, Func<string, bool> isValid)
        {
            if (Formula.GetTokens(formula).Count() < 1)
            {
                throw new FormulaFormatException("There are less than one token in the formula");
            }


            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);
            int leftPara = 0;
            int rightPara = 0;
            String prev = "";
            Regex reg = new Regex(pattern);
            foreach (string token in Formula.GetTokens(formula))
            {

                if (!reg.IsMatch(token))
                {
                    throw new FormulaFormatException("There are illegal token in the formula");
                }

                String leftAndOp = String.Format("({0}) | ({1})", lpPattern, opPattern);
                Regex regLeftOp = new Regex(leftAndOp);
                if (regLeftOp.IsMatch(prev))
                {
                    String patternAfterLeft = String.Format("({0}) | ({1})| ({2})",
                                                                doublePattern, varPattern, lpPattern);
                    Regex regAfterLeftAndOp = new Regex(patternAfterLeft);
                    if (!regAfterLeftAndOp.IsMatch(token))
                    {
                        throw new FormulaFormatException("Invalid formula");
                    }
                    prev = token;
                    continue;
                }

                String numVarRight = String.Format("({0}) | ({1})| ({2})", doublePattern, varPattern, rpPattern);
                Regex regNumVarRight = new Regex(numVarRight);
                if (regNumVarRight.IsMatch(prev))
                {
                    String patternAfterLeft = String.Format("({0}) | ({1})",
                                                                opPattern, rpPattern);
                    Regex regAfterNumVarRight = new Regex(patternAfterLeft);
                    if (!regAfterNumVarRight.IsMatch(token))
                    {
                        throw new FormulaFormatException("Invalid formula");
                    }
                    prev = token;
                    continue;
                }

                if (token.Equals("("))
                {
                    leftPara++;
                    prev = token;
                    continue;
                }
                if (token.Equals(")"))
                {
                    rightPara++;
                    if (rightPara > leftPara)
                    {
                        throw new FormulaFormatException("There exists closing parenthesis without an open parenthesis");
                    }
                    prev = token;
                    continue;
                }

                prev = token;

            }

            if (leftPara != rightPara)
            {
                throw new FormulaFormatException("There exists extra parenthesis");
            }
            String patternForFirst = String.Format("({0}) | ({1}) | ({2})",
                                                        lpPattern, varPattern, doublePattern);
            Regex reg2 = new Regex(patternForFirst);
            if (!reg2.IsMatch(Formula.GetTokens(formula).First()))
            {
                throw new FormulaFormatException("Invalid first token.");
            }


            String patternForLast = String.Format("({0}) | ({1}) | ({2})",
                                                     rpPattern, varPattern, doublePattern);
            Regex reg3 = new Regex(patternForFirst);
            if (!reg3.IsMatch(Formula.GetTokens(formula).First()))
            {
                throw new FormulaFormatException("Invalid last token.");
            }




            validFormula = formula;

        }

        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables.  When a variable symbol v needs to be determined, it should be looked up
        /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to 
        /// the constructor.)
        /// 
        /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters 
        /// in a string to upper case:
        /// 
        /// new Formula("x+7", N, s => true).Evaluate(L) is 11
        /// new Formula("x+7").Evaluate(L) is 9
        /// 
        /// Given a variable symbol as its parameter, lookup returns the variable's value 
        /// (if it has one) or throws an ArgumentException (otherwise).
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.  
        /// The Reason property of the FormulaError should have a meaningful explanation.
        ///
        /// This method should never throw an exception.
        /// </summary>
        public object Evaluate(Func<string, double> lookup)
        {

            Stack<double> valueStack = new Stack<double>();
            Stack<string> operatorStack = new Stack<string>();
            foreach (string t in Formula.GetTokens(validFormula))
            {
                //if (t.Equals("") || t.Equals(" "))
                //{
                //    continue;
                //}

                if (double.TryParse(t, out double number))
                {
                    if (operatorStack.IsOnTop("*") || operatorStack.IsOnTop("/"))
                    {
                        valueStackEmptyCheck(valueStack.Count);
                        double v = Calculator(number, valueStack.Pop(), operatorStack.Pop());
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

                        double value = Calculator(valueStack.Pop(), valueStack.Pop(), operatorStack.Pop());

                        valueStack.Push(value);
                    }
                    //Pushes the token into the token stack.
                    operatorStack.Push(t);
                    continue;
                }

                //If token is a * or /
                if (t.Equals("*") || t.Equals("/") || t.Equals("("))
                {
                    //Pushes the token into the token stack.
                    operatorStack.Push(t);
                    continue;
                }

                ////If token is a ( 
                //if (t.Equals("("))
                //{
                //    //Pushes the token into the token stack.
                //    operatorStack.Push(t);
                //    continue;
                //}


                //If token is )
                if (t.Equals(")"))
                {

                    //If + or - is on top of the operatorStack. 
                    if (operatorStack.IsOnTop("+") || operatorStack.IsOnTop("-"))
                    {
                        //Performs the same calculation as above.
                        stackThrowLessThan2(valueStack.Count);
                        double v = Calculator(valueStack.Pop(), valueStack.Pop(), operatorStack.Pop());
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
                        double v = Calculator(valueStack.Pop(), valueStack.Pop(), operatorStack.Pop());
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
                            double v = Calculator(lookup(t), valueStack.Pop(), operatorStack.Pop());
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
                        valueStack.Push(lookup(t));
                    }
                }
            }
            if (operatorStack.Count == 0)
            {
                valueStackEmptyCheck(valueStack.Count);
                return valueStack.Pop();
            }
            else if (operatorStack.Count == 1)
            {
                if ((operatorStack.IsOnTop("+") || operatorStack.IsOnTop("-")) && valueStack.Count == 2)
                {
                    return Calculator(valueStack.Pop(), valueStack.Pop(), operatorStack.Pop());
                }

                throw new ArgumentException("Invalid: operator remains with no values");
            }
            else
            {
                throw new ArgumentException("Invalid: operator remains with no values");
            }
        }


        private static double Calculator(double val1, double val2, string op)
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


        private static void stackThrowLessThan2(double count)
        {
            if (count < 2)
            {
                throw new ArgumentException("Invalid expression.");
            }
        }

        private static void valueStackEmptyCheck(double count)
        {
            if (count == 0)
            {
                throw new ArgumentException("Invalid expression.");
            }
        }

        /// <summary>
        /// Enumerates the normalized versions of all of the variables that occur in this 
        /// formula.  No normalization may appear more than once in the enumeration, even 
        /// if it appears more than once in this Formula.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
        /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
        /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
        /// </summary>
        public IEnumerable<String> GetVariables()
        {
            //HashSet<string> set = HashSet<string>set
            // foreach (string token in GetTokens)
            // if (token is variable) 
            // set.add(token)
            //return set
            return null;
        }

        /// <summary>
        /// Returns a string containing no spaces which, if passed to the Formula
        /// constructor, will produce a Formula f such that this.Equals(f).  All of the
        /// variables in the string should be normalized.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
        /// new Formula("x + Y").ToString() should return "x+Y"
        /// </summary>
        public override string ToString()
        {
            return null;
        }

        /// <summary>
        /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
        /// whether or not this Formula and obj are equal.
        /// 
        /// Two Formulae are considered equal if they consist of the same tokens in the
        /// same order.  To determine token equality, all tokens are compared as strings 
        /// except for numeric tokens and variable tokens.
        /// Numeric tokens are considered equal if they are equal after being "normalized" 
        /// by C#'s standard conversion from string to double, then back to string. This 
        /// eliminates any inconsistencies due to limited floating point precision.
        /// Variable tokens are considered equal if their normalized forms are equal, as 
        /// defined by the provided normalizer.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///  
        /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
        /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
        /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
        /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
        /// </summary>
        public override bool Equals(object obj)
        {
            //|| obj != Formula
            if (obj == null)
            {
                return false;
            }
            return false;
        }

        /// <summary>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return true.  If one is
        /// null and one is not, this method should return false.
        /// </summary>
        public static bool operator ==(Formula f1, Formula f2)
        {

            return f1.ToString() == f2.ToString();
        }

        /// <summary>
        /// Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return false.  If one is
        /// null and one is not, this method should return true.
        /// </summary>
        public static bool operator !=(Formula f1, Formula f2)
        {
            return false;
        }

        /// <summary>
        /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
        /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two 
        /// randomly-generated unequal Formulae have the same hash code should be extremely small.
        /// </summary>
        public override int GetHashCode()
        {
            //String has a very good hashcode algorithm
            return 0;
        }

        /// <summary>
        /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
        /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }

        }
    }

    /// <summary>
    /// Used to report syntactic errors in the argument to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// Used as a possible return value of the Formula.Evaluate method.
    /// </summary>
    public struct FormulaError
    {
        /// <summary>
        /// Constructs a FormulaError containing the explanatory reason.
        /// </summary>
        /// <param name="reason"></param>
        public FormulaError(String reason)
            : this()
        {
            Reason = reason;
        }

        /// <summary>
        ///  The reason why this FormulaError was created.
        /// </summary>
        public string Reason { get; private set; }
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

