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

// (Huy Nguyen / u1315096)
// Version 1.5 (2/12/2021)

// Change log:
//  (Version 1.5) Implemented, and tested the Formula constructor 
//                and the class' methods 
 


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
        //A private string that represents the valid formula after allowed from the constructor.
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
                throw new FormulaFormatException("There are less than one token in the formula.");
            }

            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            String spacePattern = @"\s+";
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            if (checkForFirst(Formula.GetTokens(formula).First()) == false)
            {
                throw new FormulaFormatException("Invalid first token.");
            }

            if (checkForLast(Formula.GetTokens(formula).Last()) == false)
            {
                throw new FormulaFormatException("Invalid last token.");
            }

            int leftPara = 0;
            int rightPara = 0;
            String prev = "";
            String newString = "";
            Regex regValid = new Regex(pattern, RegexOptions.IgnorePatternWhitespace);
            Regex regVar = new Regex(varPattern, RegexOptions.IgnorePatternWhitespace);
            foreach (string token in Formula.GetTokens(formula))
            {

                if (!regValid.IsMatch(token))
                {
                    throw new FormulaFormatException("There exists unidentified token in the formula.");
                }

                String leftAndOp = "+-*/(";
                if (leftAndOp.Contains(prev) && !prev.Equals(""))
                {
                    if (token.Equals("("))
                    {
                        leftPara++;
                        prev = token;
                        newString += token;
                        continue;
                    }

                    if (double.TryParse(token, out double num))
                    {
                        String newDoub = num.ToString();
                        newString += newDoub;
                        prev = newDoub;
                        continue;
                    }

                    if (regVar.IsMatch(token))
                    {
                        if (!regVar.IsMatch(normalize(token)))
                        {
                            throw new FormulaFormatException("Invalid variable returns by the noramlizer.");
                        }

                        String normalizedVar = normalize(token);

                        if (!isValid(normalizedVar))
                        {
                            throw new FormulaFormatException("Variables are not allowed by the validation.");
                        }
                        prev = normalizedVar;
                        newString += normalizedVar;
                        continue;
                    }

                    throw new FormulaFormatException("Invalid formula: there should be a number, a variable, or a opening parenthesis after an operator or an opening parenthesis.");

                }

                if (double.TryParse(prev, out double d) || regVar.IsMatch(prev) || prev.Equals(")"))
                {
                    String rightAndOp = "+-*/)";
                    if (!rightAndOp.Contains(token))
                    {
                        throw new FormulaFormatException("Invalid formula: there should be an operator or a closing parenthesis after a number, a variable, or a closing parenthesis.");
                    }

                    if (token.Equals(")"))
                    {
                        rightPara++;
                        if (rightPara > leftPara)
                        {
                            throw new FormulaFormatException("There exists closing parenthesis without an appropriate open parenthesis.");
                        }
                        prev = token;
                        newString += token;
                        continue;
                    }

                    prev = token;
                    newString += token;
                    continue;
                }



                if (token.Equals("("))
                {
                    leftPara++;
                    prev = token;
                    newString += token;
                    continue;
                }



                if (regVar.IsMatch(token))
                {
                    if (!regVar.IsMatch(normalize(token)))
                    {
                        throw new FormulaFormatException("Invalid variable returns by the noramlizer.");
                    }

                    String normalizedVar = normalize(token);

                    if (!isValid(normalize(token)))
                    {
                        throw new FormulaFormatException("Variables are not allowed by the validation.");
                    }
                    prev = normalizedVar;
                    newString += normalizedVar;
                    continue;
                }




                //The only thing left is number, so we check it down here.
                double number = double.Parse(token);
                String newDouble = number.ToString();
                newString += newDouble;
                prev = newDouble;
                continue;
 
            }

            //The the number of parenthesises are not equal to each other
            if (leftPara != rightPara)
            {
                throw new FormulaFormatException("There exists open parenthesis without an appropriate close parenthesis.");
            }

            validFormula = newString;
        }

        /// <summary>
        /// A private helper method to help check the first token of formula
        /// </summary>
        private bool checkForFirst(string s)
        {
            String lpPattern = @"\(";
            String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            String pattern = string.Format("({0}) | ({1}) | ({2})", lpPattern, varPattern, doublePattern);
            Regex reg = new Regex(pattern, RegexOptions.IgnorePatternWhitespace);
            if (reg.IsMatch(s))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// A private helper method to help check the last token of formula
        /// </summary>
        private bool checkForLast(string s)
        {
            String rpPattern = @"\)";
            String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            String pattern = String.Format("({0}) | ({1}) | ({2})",
                                                   rpPattern, varPattern, doublePattern);

            Regex reg = new Regex(pattern, RegexOptions.IgnorePatternWhitespace);
            if (reg.IsMatch(s))
            {
                return true;
            }
            return false;

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
            string[] substrings = Regex.Split(validFormula, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");
            Stack<double> valueStack = new Stack<double>();
            Stack<string> operatorStack = new Stack<string>();
            try
            {
                foreach (string t in substrings)
                {
                    if (t.Equals("") || t.Equals(" "))
                    {
                        continue;
                    }

                    if (double.TryParse(t, out double number))
                    {

                        if (operatorStack.IsOnTop("*") || operatorStack.IsOnTop("/"))
                        {
                            double v = Calculator(number, valueStack.Pop(), operatorStack.Pop());
                            valueStack.Push(v);
                            continue;
                        }
                   
                            valueStack.Push(number);
                            continue;
                        
                    }

                    if (t.Equals("+") || t.Equals("-"))
                    {

                        if (operatorStack.IsOnTop("+") || operatorStack.IsOnTop("-"))
                        {
                            double value = Calculator(valueStack.Pop(), valueStack.Pop(), operatorStack.Pop());
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
                            double v = Calculator(valueStack.Pop(), valueStack.Pop(), operatorStack.Pop());
                            valueStack.Push(v);
                        }


                        operatorStack.Pop();



                        if (operatorStack.IsOnTop("*") || operatorStack.IsOnTop("/"))
                        {


                            double v = Calculator(valueStack.Pop(), valueStack.Pop(), operatorStack.Pop());
                            valueStack.Push(v);
                        }
                        continue;
                    }

                    if (VariablesVerification(t))
                    {
                        if (operatorStack.IsOnTop("*") || operatorStack.IsOnTop("/"))
                        {
                            double v = Calculator(lookup(t), valueStack.Pop(), operatorStack.Pop());
                            valueStack.Push(v);
                            continue;
                        }
                            valueStack.Push(lookup(t));
                    }
                }

                if (operatorStack.Count == 1)
                {
                    return Calculator(valueStack.Pop(), valueStack.Pop(), operatorStack.Pop());
                }

                return valueStack.Pop();
            }
            catch (ArgumentException e)
            {
                return new FormulaError(e.Message);
            }

        }

        /// <summary>
        /// A private helper method to perform simple calculation.
        /// </summary>
        private static double Calculator(double val1, double val2, string op)
        {
            switch (op)
            {
                case "*":
                    return val2 * val1;
                case "/":
                    if (val1 == 0)
                    {
                        throw new ArgumentException("can not divide by 0");
                    }
                    return val2 / val1;
                case "-":
                    return val2 - val1;
            }

            return val2 + val1;
        }

        /// <summary>
        /// A private helper method to verify if the token is a variable.
        /// </summary>
        private static bool VariablesVerification(string v)
        {
            Regex regex = new Regex(@"[a-zA-Z_](?: [a-zA-Z_]|\d)*", RegexOptions.IgnorePatternWhitespace);
            bool boo = regex.IsMatch(v);
            return regex.IsMatch(v);
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
            HashSet<string> set = new HashSet<string>();
            Regex reg = new Regex(@"[a-zA-Z_](?: [a-zA-Z_]|\d)*");

            foreach (string token in Formula.GetTokens(validFormula))
            {
                if (reg.IsMatch(token))
                {
                    set.Add(token);
                }
            }

            return new HashSet<string>(set);
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
            return validFormula;
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

            if (obj == null || !(obj is Formula))
            {
                return false;
            }

            return this == (Formula)obj;
        }

        /// <summary>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return true.  If one is
        /// null and one is not, this method should return false.
        /// </summary>
        public static bool operator ==(Formula f1, Formula f2)
        {
            // Make a foreach loop to compare token by token or for(int i = 0; i < formula.count; i++){}
            if (f1 is null && f2 is null)
            {
                return true;
            }

            if (!(f1 is null) && f2 is null)
            {
                return false;
            }

            if (f1 is null && !(f2 is null))
            {
                return false;
            }

            return f1.ToString().Equals(f2.ToString());
        }

        /// <summary>
        /// Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return false.  If one is
        /// null and one is not, this method should return true.
        /// </summary>
        public static bool operator !=(Formula f1, Formula f2)
        {
            return !(f1 == f2);

        }

        /// <summary>
        /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
        /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two 
        /// randomly-generated unequal Formulae have the same hash code should be extremely small.
        /// </summary>
        public override int GetHashCode()
        {
            return validFormula.GetHashCode();
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

