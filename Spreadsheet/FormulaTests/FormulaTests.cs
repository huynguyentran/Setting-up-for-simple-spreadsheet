using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using System;
using System.Collections.Generic;

namespace FormulaTests
{
    [TestClass()]
    public class FormulaTests
    {

        [TestMethod()]
        public void _ComplexEquals()
        {
            Formula f1 = new Formula("1e-3 + 1.0");
            Formula f2 = new Formula("0.001 + 1.0");
            Assert.IsTrue(f1.Equals(f2));
        }


        [TestMethod()]
        public void _ComplexEquals2()
        {
            Formula f1 = new Formula("1e10 + 7.0000");
            Formula f2 = new Formula("10000000000 + 7");
            Assert.IsTrue(f1.Equals(f2));
            Assert.AreEqual(f1.GetHashCode(), f2.GetHashCode());
        }

        [TestMethod()]
        public void _SimpleConstructorEquals()
        {
            Formula f1 = new Formula("1.0 + 1.0");
            Formula f2 = new Formula("1.0 + 1.0");
            Assert.IsTrue(f1 == f2);
            Assert.AreEqual(f1.GetHashCode(), f2.GetHashCode());
        }

        [TestMethod()]
        public void _SimpleEquals1()
        {
            Formula f1 = new Formula("1.0 + 1.0");
            String f2 = "1.0 + 1.0";
            Assert.IsFalse(f1.Equals(f2));
            Assert.AreNotEqual(f1.GetHashCode(), f2.GetHashCode());
        }


        [TestMethod()]
        public void _SimpleEquals2()
        {
            Formula f1 = new Formula("1000.0");
            Formula f2 = new Formula("1000.000000000000000000000000000000000000000000000000000000002");
            Assert.IsTrue(f1.Equals(f2));
        }


        [TestMethod()]
        public void _SimpleEquals3()
        {
            Formula f1 = new Formula("1.0 + 1.0");
            Formula f2 = null;
            Assert.IsFalse(f1.Equals(f2));
        }

        [TestMethod()]
        public void _SimpleEquals4()
        {
            Formula f1 = new Formula("1.0 + 1.0");
            Formula f2 = new Formula("1.0 + 1.0");
            Assert.IsTrue(f1.Equals(f2));
        }


        [TestMethod()]
        public void _SimpleHashCodeEquals()
        {
            Formula f1 = new Formula("1.0 + 1.0");
            Formula f2 = new Formula("1.0 + 1.0");
            Assert.IsTrue(f1 == f2);
            Assert.AreEqual(f1.GetHashCode(), f2.GetHashCode());
        }

        [TestMethod()]
        public void _SimpleHashCodeEquals2()
        {
            Formula f1 = new Formula("x1+1", s=> s.ToLower(),s=> true );
            Formula f2 = new Formula("X1+1", s=> s.ToLower(),s => true);
            Formula f3 = new Formula("X1+ x2", s => s.ToLower(), s => true);
            Assert.IsTrue(f1 == f2);
            Assert.AreEqual(f1.GetHashCode(), f2.GetHashCode());
            Assert.AreNotEqual(f1.GetHashCode(), f3.GetHashCode());
        }

        [TestMethod()]
        public void _SimpleHashCodeNotEquals()
        {
            Formula f1 = new Formula("x1 + 1.0");
            Formula f2 = new Formula("2.0 + 1.0");
            Assert.IsTrue(f1 != f2);
            Assert.AreNotEqual(f1.GetHashCode(), f2.GetHashCode());
        }

        [TestMethod()]
        public void _SimpleConstructorCompareNull1()
        {
            Formula f1 = null;
            Formula f2 = null;
            Assert.IsTrue(f1 == f2);
        }

        [TestMethod()]
        public void _SimpleConstructorCompareNull2()
        {
            Formula f1 = null;
            Formula f2 = new Formula("1.0 + 1.0");
            Assert.IsFalse(f1 == f2);
            Assert.IsTrue(f1 != f2);
        }

        [TestMethod()]
        public void _SimpleConstructorCompareNull3()
        {
            Formula f1 = new Formula("1.0 + 1.0");
            Formula f2 = null;
            Assert.IsFalse(f1 == f2);
            Assert.IsTrue(f1 != f2);
        }

        [TestMethod()]
        public void _SimpleConstructorCompareOp()
        {
            Formula f1 = new Formula("1.0 + 1.0");
            Formula f2 = new Formula("1.0 + 1.0");
            Assert.IsTrue(f1 == f2);
            Assert.IsFalse(f1 != f2);
        }

        [TestMethod()]
        public void _SimpleConstructorNotEquals()
        {
            Formula f1 = new Formula("2.0 + 1.0");
            Formula f2 = new Formula("1.0 + 1.0");
            Assert.IsFalse(f1 == f2);
            Assert.IsFalse(f1.Equals(f2));
        }

        [TestMethod()]
        public void _Evaluate()
        {
            Formula f1 = new Formula("(5+3)*4");
            double val = (double)f1.Evaluate(x => 0);
            Assert.AreEqual(32, val, 1e-9);
        }

        [TestMethod()]
        public void _Evaluate2()
        {
            Formula f1 = new Formula("((10+2)*(15-5))/(40+8)");
            double val = (double)f1.Evaluate(x => 0);
            Assert.AreEqual(2.5, val, 1e-9);
        }

        [TestMethod()]
        public void _Evaluate3()
        {
            Formula f1 = new Formula("((10+2)*(15-5))/((60-20)+4*2)");
            double val = (double)f1.Evaluate(x => 0);
            Assert.AreEqual(2.5, val, 1e-9);
        }

        [TestMethod()]
        public void _Evaluate4()
        {
            Formula f1 = new Formula("4+2+3+2");
            double val = (double)f1.Evaluate(x => 0);
            Assert.AreEqual(11, val, 1e-9);
        }


        [TestMethod()]
        public void _Evaluate5()
        {
            Formula f1 = new Formula("(4+2)");
            double val = (double)f1.Evaluate(x => 0);
            Assert.AreEqual(6, val, 1e-9);
        }


        [TestMethod()]
        public void _Evaluate6()
        {
            Formula f1 = new Formula("x1+x2+50", s => s.ToUpper(), s => s == "X2" || s == "X1");
            double val = (double)f1.Evaluate(x =>
            {
                if (x == "X1")
                {
                    return 1;
                }
                return 2;


            });
            Assert.AreEqual(53, val, 1e-9);
        }

        [TestMethod()]
        public void _Evaluate7()
        {
            Formula f1 = new Formula("x1/x2+5*(1+1)", s => s.ToUpper(), s => s == "X2" || s == "X1");
            double val = (double)f1.Evaluate(x =>
            {
                if (x == "X1")
                {
                    return 1;
                }
                return 2;


            });
            Assert.AreEqual(10.5, val, 1e-9);
        }

        [TestMethod()]
        public void _Evaluate8()
        {
            Formula f1 = new Formula("x1/x2+x1*x2+x3", s => s.ToLower(), s => s == "x2" || s == "x1" || s == "x3");
            double val = (double)f1.Evaluate(x =>
             {
                 if (x == "x1")
                 {
                     return 1;
                 }
                 if (x == "x2")
                     return 2;
                 if (x == "x3")
                     return 3;

                 else throw new ArgumentException("can not find" + x);
             });

            Assert.AreEqual(5.5, val, 1e-9);
        }

        [TestMethod()]
        public void _Evaluate9()
        {
            Formula f1 = new Formula("x1/x2+x1*x2+x3", s => s.ToUpper(), s => s == "X2" || s == "X1" || s == "X3");
           
            var a = (FormulaError)f1.Evaluate(x =>
            {
                if (x == "X1")
                {
                    return 1;
                }
                if (x == "X2")
                    return 2;

                else throw new ArgumentException("can not find " + x);
            });
            Assert.IsInstanceOfType(a, typeof(FormulaError));
            Assert.IsNotNull(a.Reason);
            Assert.AreEqual("can not find X3", a.Reason);
        }


        [TestMethod()]
        public void _Evaluate10()
        {
            Formula f1 = new Formula("(x1+2)/4-(x2-(x3*5))*4-100", s => s.ToLower(), s => true);

           double val = (double)f1.Evaluate(x =>
            {
                if (x == "x1")
                {
                    return 1;
                }
                if (x == "x2")
                    return 2;
                if (x == "x3")
                    return 3;

                else throw new ArgumentException("can not find " + x);
            });
            Assert.AreEqual(-47.25, val, 1e-9);
        }

        [TestMethod()]
        public void _EvaluateDivideByZero()
        {
            Formula f1 = new Formula("1/0");
            Assert.IsInstanceOfType(f1.Evaluate(s => 0), typeof(FormulaError));
            var a = (FormulaError)f1.Evaluate(s => 0);
            Assert.IsNotNull(a.Reason);
            Assert.AreEqual("can not divide by 0", a.Reason);

        }

        [TestMethod()]
        public void _EvaluateDivideByZero2()
        {
            Formula f1 = new Formula("1/x1");
            var a = (FormulaError)f1.Evaluate(x =>
            {
                if (x == "x1")
                {
                    return 0;
                }
                else throw new ArgumentException("can not find " + x);
            });
            Assert.IsInstanceOfType(a, typeof(FormulaError));
            Assert.IsNotNull(a.Reason);
            Assert.AreEqual("can not divide by 0", a.Reason);

        }

        [TestMethod()]
        public void _GetVariablesTest()
        {
            Formula f1 = new Formula("x+y+x+2");
            IEnumerator<string> e = f1.GetVariables().GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            String s1 = e.Current;
            Assert.IsTrue(e.MoveNext());
            String s2 = e.Current;
            Assert.IsFalse(e.MoveNext());
            Assert.IsTrue(((s1 == "x") && (s2 == "y")) || ((s1 == "y") && (s2 == "x")));
        }


        [TestMethod()]
        public void _GetVariablesTest2()
        {
            Formula f1 = new Formula("x1+x2+x3+2", s => s.ToUpper(), s => s == "X1" || s == "X2" || s == "X3");
            IEnumerator<string> e = f1.GetVariables().GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            String s1 = e.Current;
            Assert.IsTrue(e.MoveNext());
            String s2 = e.Current;
            Assert.IsTrue(e.MoveNext());
            String s3 = e.Current;
            Assert.IsFalse(e.MoveNext());
            Assert.IsTrue(s1 == "X1" && s2 == "X2" && s3 == "X3");
        }



        [TestMethod()]
        public void _ThrowOpenParenthesis1()
        {
            try { Formula f1 = new Formula("(1/2"); }
            catch (FormulaFormatException e)
            {
                Assert.AreEqual("There exists open parenthesis without an appropriate close parenthesis.", e.Message);
            }

        }

        [TestMethod()]
        public void _ThrowOpenParenthesis2()
        {
            try { Formula f1 = new Formula("1+2+((5-2)"); }
            catch (FormulaFormatException e)
            {
                Assert.AreEqual("There exists open parenthesis without an appropriate close parenthesis.", e.Message);
            }

        }


        [TestMethod()]
        public void _ThrowCloseParenthesis1()
        {
            try { Formula f1 = new Formula("(1+2))"); }
            catch (FormulaFormatException e)
            {
                Assert.AreEqual("There exists close parenthesis without an appropriate open parenthesis.", e.Message);
            }

        }

        [TestMethod()]
        public void _ThrowCloseParenthesis2()
        {
            try { Formula f1 = new Formula("1+2)+2"); }
            catch (FormulaFormatException e)
            {
                Assert.AreEqual("There exists close parenthesis without an appropriate open parenthesis.", e.Message);
            }

        }

        [TestMethod()]
        public void _ThrowNotVariable()
        {
            try { Formula f1 = new Formula("12+@1+2"); }
            catch (FormulaFormatException e)
            {
                Assert.AreEqual("There exists unidentified token in the formula.", e.Message);
            }

        }

        [TestMethod()]
        public void _ThrowEmpty()
        {
            try { Formula f1 = new Formula(""); }
            catch (FormulaFormatException e)
            {
                Assert.AreEqual("There are less than one token in the formula.", e.Message);
            }

        }

        [TestMethod()]
        public void _ThrowFirst()
        {
            try { Formula f1 = new Formula("+52-80"); }
            catch (FormulaFormatException e)
            {
                Assert.AreEqual("Invalid first token.", e.Message);
            }

        }


        [TestMethod()]
        public void _ThrowLast()
        {
            try { Formula f1 = new Formula("52-80-"); }
            catch (FormulaFormatException e)
            {
                Assert.AreEqual("Invalid last token.", e.Message);
            }

        }


        [TestMethod()]
        public void _ThrowOpenPara1()
        {
            try { Formula f1 = new Formula("()"); }
            catch (FormulaFormatException e)
            {
                Assert.AreEqual("Invalid formula: there should be a number, a variable, or a open parenthesis after an operator or an open parenthesis.", e.Message);
            }

        }

        [TestMethod()]
        public void _ThrowOpenPara2()
        {
            try { Formula f1 = new Formula("(+3"); }
            catch (FormulaFormatException e)
            {
                Assert.AreEqual("Invalid formula: there should be a number, a variable, or a open parenthesis after an operator or an open parenthesis.", e.Message);
            }

        }

        [TestMethod()]
        public void _ThrowUnidentifiedVariable()
        {
            try { Formula f1 = new Formula("123+^abcd+152"); }
            catch (FormulaFormatException e)
            {
                Assert.AreEqual("There exists unidentified token in the formula.", e.Message);
            }

        }

        [TestMethod()]
        public void _ThrowInvalidVariable()
        {
            try { Formula f1 = new Formula("x1/x2+x1*x2", s => s.ToUpper(), s => s == "x2" || s == "x1"); }
            catch (FormulaFormatException e)
            {
                Assert.AreEqual("Variables are not allowed by the validation.", e.Message);
            }

        }

        [TestMethod()]
        public void _ThrowInvalidVariable2()
        {
            try { Formula f1 = new Formula("((x1/x2)+3)-(x1*x2)/x3", s => s.ToUpper(), s => s == "x2" || s == "x1"); }
            catch (FormulaFormatException e)
            {
                Assert.AreEqual("Variables are not allowed by the validation.", e.Message);
            }

        }

        [TestMethod()]
        public void _ThrowInvalidNormalization()
        {
            try { Formula f1 = new Formula("x1/x2+x1*x2", s => "$", s => true); }
            catch (FormulaFormatException e)
            {
                Assert.AreEqual("Invalid variables returned by the noramlizer.", e.Message);
            }

        }

        [TestMethod()]
        public void _ThrowInvalidNormalization2()
        {
            try { Formula f1 = new Formula("((x1/x2)+3)-(x1*x2)/x3", s => "$", s => true); }
            catch (FormulaFormatException e)
            {
                Assert.AreEqual("Invalid variables returned by the noramlizer.", e.Message);
            }




        }

        [TestMethod()]
        public void _ThrowInvalidFormula()
        {
            try { Formula f1 = new Formula("123+54(-12", s => s, s => true); }
            catch (FormulaFormatException e)
            {
                Assert.AreEqual("Invalid formula: there should be an operator or a close parenthesis after a number, a variable, or a close parenthesis.", e.Message);
            }

        }

    }
}
