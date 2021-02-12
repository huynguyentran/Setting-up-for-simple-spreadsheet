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
        public void _SimpleConstructorEquals()
        {
            Formula f1 = new Formula("1.0 + 1.0");
            Formula f2 = new Formula("1.0 + 1.0");
            Assert.IsTrue(f1 == f2);
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
        public void _SimpleConstructorNull1()
        {
            Formula f1 = null;
            Formula f2 = null;
            Assert.IsTrue(f1 == f2);
        }

        [TestMethod()]
        public void _SimpleConstructorNull2()
        {
            Formula f1 = null;
            Formula f2 = new Formula("1.0 + 1.0");
            Assert.IsFalse(f1 == f2);
            Assert.IsTrue(f1 != f2);
        }

        [TestMethod()]
        public void _SimpleConstructorNull3()
        {
            Formula f1 = new Formula("1.0 + 1.0");
            Formula f2 = null;
            Assert.IsFalse(f1 == f2);
            Assert.IsTrue(f1 != f2);
        }


        [TestMethod()]
        public void _SimpleConstructorNotEquals()
        {
            Formula f1 = new Formula("2.0 + 1.0");
            Formula f2 = new Formula("1.0 + 1.0");
            Assert.IsFalse(f1 == f2);
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
            Formula f1 = new Formula("((10+2)*(15-5))/48");
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
        public void _EvaluateDivideBy0()
        {
            Formula f1 = new Formula("1/0");
            Assert.IsInstanceOfType(f1.Evaluate(s => 0), typeof(FormulaError));
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
        [ExpectedException(typeof(FormulaFormatException), "There exists open parenthesis without an appropriate close parenthesis")]
        public void _ThrowOpenParenthesis()
        {
            Formula f1 = new Formula("(1/2");
        }



        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException), "Invalid formula: there should be an operator or a closing parenthesis after a number, a variable, or a closing parenthesis.")]
        public void _ThrowOpenParenthesis2()
        {
            Formula f1 = new Formula("1+2(-2");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException), "There exists closing parenthesis without an appropriate open parenthesis.")]
        public void _ThrowCloseParenthesis()
        {
            Formula f1 = new Formula("(1+2))");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException), "There exists closing parenthesis without an appropriate open parenthesis.")]
        public void _ThrowCloseParenthesis2()
        {
            Formula f1 = new Formula("1+2)+2");
        }



        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException), "There exists unidentified token in the formula.")]
        public void _ThrowUnidentifiedToken()
        {
            Formula f1 = new Formula("@1+2");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException), "There are less than one token in the formula.")]
        public void _ThrowEmpty()
        {
            Formula f1 = new Formula("");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException), "Invalid first token.")]
        public void _ThrowFirst()
        {
            Formula f1 = new Formula("+52-80");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException), "Invalid last token.")]
        public void _ThrowLast()
        {
            Formula f1 = new Formula("52-80-");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException), "Invalid formula: there should be a number, a variable, or a opening parenthesis after an operator or an opening parenthesis.")]
        public void _ThrowOpenPara1()
        {
            Formula f1 = new Formula("()");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException), "Invalid formula: there should be a number, a variable, or a opening parenthesis after an operator or an opening parenthesis.")]
        public void _ThrowOpenPara2()
        {
            Formula f1 = new Formula("(+");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException), "There exists unidentified token in the formula.")]
        public void _ThrowUnidentifiedVariable()
        {
            Formula f1 = new Formula("123+^abcd+152");
        }
        //Throw but not showing where it throw, the later message does not matter.
    }
}
