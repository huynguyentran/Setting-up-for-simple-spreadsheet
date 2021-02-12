using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;


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
            Formula f1 = new Formula("(x+3)*4");
            double val = (double)f1.Evaluate(x => 0);
            Assert.AreEqual(12, val, 1e-9);
        }

        [TestMethod()]
        public void _Evaluate3()
        {
            Formula f1 = new Formula("1/0");
            Assert.IsInstanceOfType(f1.Evaluate(s => 0), typeof(FormulaError));
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException), "There exists open parenthesis without an appropriate close parenthesis")]
        public void _ThrowConstructor()
        {
            Formula f1 = new Formula("(1/2");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException), "There exists open parenthesis without an appropriate close parenthesis")]
        public void _ThrowConstructor()
        {
            Formula f1 = new Formula("(1/2");
        }


    }
}
