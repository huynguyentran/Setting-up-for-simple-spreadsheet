using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using System;
using System.Collections.Generic;

namespace FormulaTests
{
    [TestClass()]
    public class FormulaTests
    {

        // Normalizer tests
        [TestMethod(), Timeout(2000)]
        [TestCategory("1")]
        public void TestNormalizerGetVars()
        {
            Formula f = new Formula("2+x1", s => s.ToUpper(), s => true);
            HashSet<string> vars = new HashSet<string>(f.GetVariables());

            Assert.IsTrue(vars.SetEquals(new HashSet<string> { "X1" }));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("2")]
        public void TestNormalizerEquals()
        {
            Formula f = new Formula("2+x1", s => s.ToUpper(), s => true);
            Formula f2 = new Formula("2+X1", s => s.ToUpper(), s => true);

            Assert.IsTrue(f.Equals(f2));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("3")]
        public void TestNormalizerToString()
        {
            Formula f = new Formula("2+x1", s => s.ToUpper(), s => true);
            Formula f2 = new Formula(f.ToString());

            Assert.IsTrue(f.Equals(f2));
        }

        // Validator tests
        [TestMethod(), Timeout(2000)]
        [TestCategory("4")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestValidatorFalse()
        {
            Formula f = new Formula("2+x1", s => s, s => false);
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("5")]
        public void TestValidatorX1()
        {
            Formula f = new Formula("2+x", s => s, s => (s == "x"));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("6")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestValidatorX2()
        {
            Formula f = new Formula("2+y1", s => s, s => (s == "x"));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("7")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestValidatorX3()
        {
            Formula f = new Formula("2+x1", s => s, s => (s == "x"));
        }


        // Simple tests that return FormulaErrors
        [TestMethod(), Timeout(2000)]
        [TestCategory("8")]
        public void TestUnknownVariable()
        {
            Formula f = new Formula("2+X1");
            Assert.IsInstanceOfType(f.Evaluate(s => { throw new ArgumentException("Unknown variable"); }), typeof(FormulaError));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("9")]
        public void TestDivideByZero()
        {
            Formula f = new Formula("5/0");
            Assert.IsInstanceOfType(f.Evaluate(s => 0), typeof(FormulaError));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("10")]
        public void TestDivideByZeroVars()
        {
            Formula f = new Formula("(5 + X1) / (X1 - 3)");
            Assert.IsInstanceOfType(f.Evaluate(s => 3), typeof(FormulaError));
        }


        // Tests of syntax errors detected by the constructor
        [TestMethod(), Timeout(2000)]
        [TestCategory("11")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestSingleOperator()
        {
            Formula f = new Formula("+");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("12")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestExtraOperator()
        {
            Formula f = new Formula("2+5+");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("13")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestExtraCloseParen()
        {
            Formula f = new Formula("2+5*7)");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("14")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestExtraOpenParen()
        {
            Formula f = new Formula("((3+5*7)");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("15")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestNoOperator()
        {
            Formula f = new Formula("5x");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("16")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestNoOperator2()
        {
            Formula f = new Formula("5+5x");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("17")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestNoOperator3()
        {
            Formula f = new Formula("5+7+(5)8");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("18")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestNoOperator4()
        {
            Formula f = new Formula("5 5");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("19")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestDoubleOperator()
        {
            Formula f = new Formula("5 + + 3");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("20")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestEmpty()
        {
            Formula f = new Formula("");
        }

        // Some more complicated formula evaluations
        [TestMethod(), Timeout(2000)]
        [TestCategory("21")]
        public void TestComplex1()
        {
            Formula f = new Formula("y1*3-8/2+4*(8-9*2)/14*x7");
            Assert.AreEqual(5.14285714285714, (double)f.Evaluate(s => (s == "x7") ? 1 : 4), 1e-9);
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("22")]
        public void TestRightParens()
        {
            Formula f = new Formula("x1+(x2+(x3+(x4+(x5+x6))))");
            Assert.AreEqual(6, (double)f.Evaluate(s => 1), 1e-9);
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("23")]
        public void TestLeftParens()
        {
            Formula f = new Formula("((((x1+x2)+x3)+x4)+x5)+x6");
            Assert.AreEqual(12, (double)f.Evaluate(s => 2), 1e-9);
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("53")]
        public void TestRepeatedVar()
        {
            Formula f = new Formula("a4-a4*a4/a4");
            Assert.AreEqual(0, (double)f.Evaluate(s => 3), 1e-9);
        }

        // Test of the Equals method
        [TestMethod(), Timeout(2000)]
        [TestCategory("24")]
        public void TestEqualsBasic()
        {
            Formula f1 = new Formula("X1+X2");
            Formula f2 = new Formula("X1+X2");
            Assert.IsTrue(f1.Equals(f2));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("25")]
        public void TestEqualsWhitespace()
        {
            Formula f1 = new Formula("X1+X2");
            Formula f2 = new Formula(" X1  +  X2   ");
            Assert.IsTrue(f1.Equals(f2));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("26")]
        public void TestEqualsDouble()
        {
            Formula f1 = new Formula("2+X1*3.00");
            Formula f2 = new Formula("2.00+X1*3.0");
            Assert.IsTrue(f1.Equals(f2));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("27")]
        public void TestEqualsComplex()
        {
            Formula f1 = new Formula("1e-2 + X5 + 17.00 * 19 ");
            Formula f2 = new Formula("   0.0100  +     X5+ 17 * 19.00000 ");
            Assert.IsTrue(f1.Equals(f2));
        }


        [TestMethod(), Timeout(2000)]
        [TestCategory("28")]
        public void TestEqualsNullAndString()
        {
            Formula f = new Formula("2");
            Assert.IsFalse(f.Equals(null));
            Assert.IsFalse(f.Equals(""));
        }


        // Tests of == operator
        [TestMethod(), Timeout(2000)]
        [TestCategory("29")]
        public void TestEq()
        {
            Formula f1 = new Formula("2");
            Formula f2 = new Formula("2");
            Assert.IsTrue(f1 == f2);
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("30")]
        public void TestEqFalse()
        {
            Formula f1 = new Formula("2");
            Formula f2 = new Formula("5");
            Assert.IsFalse(f1 == f2);
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("31")]
        public void TestEqNull()
        {
            Formula f1 = new Formula("2");
            Formula f2 = new Formula("2");
            Assert.IsFalse(null == f1);
            Assert.IsFalse(f1 == null);
            Assert.IsTrue(f1 == f2);
        }


        // Tests of != operator
        [TestMethod(), Timeout(2000)]
        [TestCategory("32")]
        public void TestNotEq()
        {
            Formula f1 = new Formula("2");
            Formula f2 = new Formula("2");
            Assert.IsFalse(f1 != f2);
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("33")]
        public void TestNotEqTrue()
        {
            Formula f1 = new Formula("2");
            Formula f2 = new Formula("5");
            Assert.IsTrue(f1 != f2);
        }


        // Test of ToString method
        [TestMethod(), Timeout(2000)]
        [TestCategory("34")]
        public void TestString()
        {
            Formula f = new Formula("2*5");
            Assert.IsTrue(f.Equals(new Formula(f.ToString())));
        }


        // Tests of GetHashCode method
        [TestMethod(), Timeout(2000)]
        [TestCategory("35")]
        public void TestHashCode()
        {
            Formula f1 = new Formula("2*5");
            Formula f2 = new Formula("2*5");
            Assert.IsTrue(f1.GetHashCode() == f2.GetHashCode());
        }

        // Technically the hashcodes could not be equal and still be valid,
        // extremely unlikely though. Check their implementation if this fails.
        [TestMethod(), Timeout(2000)]
        [TestCategory("36")]
        public void TestHashCodeFalse()
        {
            Formula f1 = new Formula("2*5");
            Formula f2 = new Formula("3/8*2+(7)");
            Assert.IsTrue(f1.GetHashCode() != f2.GetHashCode());
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("37")]
        public void TestHashCodeComplex()
        {
            Formula f1 = new Formula("2 * 5 + 4.00 - _x");
            Formula f2 = new Formula("2*5+4-_x");
            Assert.IsTrue(f1.GetHashCode() == f2.GetHashCode());
        }


        // Tests of GetVariables method
        [TestMethod(), Timeout(2000)]
        [TestCategory("38")]
        public void TestVarsNone()
        {
            Formula f = new Formula("2*5");
            Assert.IsFalse(f.GetVariables().GetEnumerator().MoveNext());
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("39")]
        public void TestVarsSimple()
        {
            Formula f = new Formula("2*X2");
            List<string> actual = new List<string>(f.GetVariables());
            HashSet<string> expected = new HashSet<string>() { "X2" };
            Assert.AreEqual(actual.Count, 1);
            Assert.IsTrue(expected.SetEquals(actual));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("40")]
        public void TestVarsTwo()
        {
            Formula f = new Formula("2*X2+Y3");
            List<string> actual = new List<string>(f.GetVariables());
            HashSet<string> expected = new HashSet<string>() { "Y3", "X2" };
            Assert.AreEqual(actual.Count, 2);
            Assert.IsTrue(expected.SetEquals(actual));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("41")]
        public void TestVarsDuplicate()
        {
            Formula f = new Formula("2*X2+X2");
            List<string> actual = new List<string>(f.GetVariables());
            HashSet<string> expected = new HashSet<string>() { "X2" };
            Assert.AreEqual(actual.Count, 1);
            Assert.IsTrue(expected.SetEquals(actual));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("42")]
        public void TestVarsComplex()
        {
            Formula f = new Formula("X1+Y2*X3*Y2+Z7+X1/Z8");
            List<string> actual = new List<string>(f.GetVariables());
            HashSet<string> expected = new HashSet<string>() { "X1", "Y2", "X3", "Z7", "Z8" };
            Assert.AreEqual(actual.Count, 5);
            Assert.IsTrue(expected.SetEquals(actual));
        }

        // Tests to make sure there can be more than one formula at a time
        [TestMethod(), Timeout(2000)]
        [TestCategory("43")]
        public void TestMultipleFormulae()
        {
            Formula f1 = new Formula("2 + a1");
            Formula f2 = new Formula("3");
            Assert.AreEqual(2.0, f1.Evaluate(x => 0));
            Assert.AreEqual(3.0, f2.Evaluate(x => 0));
            Assert.IsFalse(new Formula(f1.ToString()) == new Formula(f2.ToString()));
            IEnumerator<string> f1Vars = f1.GetVariables().GetEnumerator();
            IEnumerator<string> f2Vars = f2.GetVariables().GetEnumerator();
            Assert.IsFalse(f2Vars.MoveNext());
            Assert.IsTrue(f1Vars.MoveNext());
        }

        // Repeat this test to increase its weight
        [TestMethod(), Timeout(2000)]
        [TestCategory("44")]
        public void TestMultipleFormulaeB()
        {
            TestMultipleFormulae();
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("45")]
        public void TestMultipleFormulaeC()
        {
            TestMultipleFormulae();
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("46")]
        public void TestMultipleFormulaeD()
        {
            TestMultipleFormulae();
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("47")]
        public void TestMultipleFormulaeE()
        {
            TestMultipleFormulae();
        }

        // Stress test for constructor
        [TestMethod(), Timeout(2000)]
        [TestCategory("48")]
        public void TestConstructor()
        {
            Formula f = new Formula("(((((2+3*X1)/(7e-5+X2-X4))*X5+.0005e+92)-8.2)*3.14159) * ((x2+3.1)-.00000000008)");
        }

        // This test is repeated to increase its weight
        [TestMethod(), Timeout(2000)]
        [TestCategory("49")]
        public void TestConstructorB()
        {
            Formula f = new Formula("(((((2+3*X1)/(7e-5+X2-X4))*X5+.0005e+92)-8.2)*3.14159) * ((x2+3.1)-.00000000008)");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("50")]
        public void TestConstructorC()
        {
            Formula f = new Formula("(((((2+3*X1)/(7e-5+X2-X4))*X5+.0005e+92)-8.2)*3.14159) * ((x2+3.1)-.00000000008)");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("51")]
        public void TestConstructorD()
        {
            Formula f = new Formula("(((((2+3*X1)/(7e-5+X2-X4))*X5+.0005e+92)-8.2)*3.14159) * ((x2+3.1)-.00000000008)");
        }

        // Stress test for constructor
        [TestMethod(), Timeout(2000)]
        [TestCategory("52")]
        public void TestConstructorE()
        {
            Formula f = new Formula("(((((2+3*X1)/(7e-5+X2-X4))*X5+.0005e+92)-8.2)*3.14159) * ((x2+3.1)-.00000000008)");
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
