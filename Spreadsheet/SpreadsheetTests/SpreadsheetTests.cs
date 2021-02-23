// Huy Nguyen (u1315096) 
// 2/19/2021
// PS4 3500 

//Tests for spreadsheet class.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using System.Collections.Generic;
using System;
using System.Linq;

namespace SS
{
    [TestClass]
    public class SpreadsheetTests
    {
        [TestMethod]
        public void SimpleGetCellContentTest()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            string a1 = "a1";
            Assert.AreEqual(sheet.GetCellContents(a1), "");
            sheet.SetCellContents(a1, "s");
            Assert.AreEqual("s", sheet.GetCellContents(a1));
        }

        [TestMethod]
        public void SimpleGetCellContentTest2()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            string a1 = "a1";
            sheet.SetCellContents(a1, 24.323);
            Assert.AreEqual(24.323, sheet.GetCellContents(a1));
        }


        [TestMethod]
        public void SimpleGetCellContentTest3()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            string a1 = "a1";
            Formula f1 = new Formula("1.0 + 1.0");
            sheet.SetCellContents(a1, f1);
            Assert.AreEqual(f1, sheet.GetCellContents(a1));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetCellContentThrow()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.GetCellContents("@13");
        }


        [TestMethod]
        public void GetCellContentDoesNotExist()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            Assert.AreEqual("", sheet.GetCellContents("_a2"));
            List<string> actualList = new List<string>(sheet.GetNamesOfAllNonemptyCells());
            Assert.AreEqual(0, actualList.Count);
        }

        [TestMethod]
        public void SimpleGetNameOfNoneEmptyCellsTest()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();

            Formula f1 = new Formula("1.0 + 1.0");
            sheet.SetCellContents("a1", f1);
            sheet.SetCellContents("a2", 20);
            sheet.SetCellContents("a3", "something cool");
            sheet.SetCellContents("a4", "");

            List<string> expectedList = new List<string>();
            expectedList.Add("a1");
            expectedList.Add("a2");
            expectedList.Add("a3");

            List<string> actualList = new List<string>(sheet.GetNamesOfAllNonemptyCells());
            CollectionAssert.AreEqual(expectedList, actualList);
        }

        [TestMethod]
        public void SimpleGetNameOfNoneEmptyCellsTest2()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            int size = 1000;
            List<string> expectedList = new List<string>();
            for (int i = 0; i < size; i++)
            {
                sheet.SetCellContents("a" + i, i);
                expectedList.Add("a" + i);
            }
            List<string> actualList = new List<string>(sheet.GetNamesOfAllNonemptyCells());
            CollectionAssert.AreEqual(expectedList, actualList);
            Assert.AreEqual(1000, actualList.Count);

            List<string> emptyList = new List<string>();
            for (int i = 0; i < size; i++)
            {
                sheet.SetCellContents("a" + i, "");
            }
            actualList = new List<string>(sheet.GetNamesOfAllNonemptyCells());
            CollectionAssert.AreEqual(emptyList, actualList);
            Assert.AreEqual(0, actualList.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellContentForDoubleThrow()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("@13", 20);
        }

        [TestMethod]
        public void TestSetCellContentForDouble()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            Formula f1 = new Formula("a2 + 3");
            sheet.SetCellContents("a1", f1);

            List<string> list = new List<string>(sheet.GetNamesOfAllNonemptyCells());
            List<string> expectedList = new List<string>();
            expectedList.Add("a1");
            CollectionAssert.AreEqual(expectedList, list);
            Assert.AreEqual(f1, sheet.GetCellContents("a1"));

            List<string> list2 = new List<string>(sheet.SetCellContents("a1", f1));
            CollectionAssert.AreEqual(expectedList, list2);

            sheet.SetCellContents("a2", "");
            List<string> expectedList2 = new List<string>();
            expectedList2.Add("a2");
            expectedList2.Add("a1");

            list2 = new List<string>(sheet.SetCellContents("a2", ""));
            CollectionAssert.AreEqual(expectedList2, list2);
            sheet.SetCellContents("a1", 20);
            Assert.AreNotEqual(f1, sheet.GetCellContents("a1"));
            Assert.AreEqual(20.0, sheet.GetCellContents("a1"));


        }

        [TestMethod]
        public void TestSetCellContentForDouble2()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();

            sheet.SetCellContents("a1", 13.0);

            List<string> list = new List<string>(sheet.GetNamesOfAllNonemptyCells());
            List<string> expectedList = new List<string>();
            expectedList.Add("a1");
            CollectionAssert.AreEqual(expectedList, list);
            Assert.AreEqual(13.0, sheet.GetCellContents("a1"));

            sheet.SetCellContents("a1", 20);
            Assert.AreNotEqual(13.0, sheet.GetCellContents("a1"));
            Assert.AreEqual(20.0, sheet.GetCellContents("a1"));
        }

        [TestMethod]
        public void TestSetCellContentForDouble3()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();

            sheet.SetCellContents("a1", "a");

            List<string> list = new List<string>(sheet.GetNamesOfAllNonemptyCells());
            List<string> expectedList = new List<string>();
            expectedList.Add("a1");
            CollectionAssert.AreEqual(expectedList, list);
            Assert.AreEqual("a", sheet.GetCellContents("a1"));

            sheet.SetCellContents("a1", 20);
            Assert.AreNotEqual("a", sheet.GetCellContents("a1"));
            Assert.AreEqual(20.0, sheet.GetCellContents("a1"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSetCellContentForStringThrow()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            string s = null;
            sheet.SetCellContents("_12312asd23", s);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellContentForStringThrow2()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            string s = "test";
            sheet.SetCellContents("^12asd23", s);
        }

        [TestMethod]
        public void TestSetCellContentForString()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("a1", 19);
            List<string> list = new List<string>(sheet.GetNamesOfAllNonemptyCells());
            List<string> expectedList = new List<string>();
            expectedList.Add("a1");
            CollectionAssert.AreEqual(expectedList, list);
            Assert.AreEqual(19.0, sheet.GetCellContents("a1"));
            sheet.SetCellContents("a1", "test");
            Assert.AreNotEqual(19, sheet.GetCellContents("a1"));
            Assert.AreEqual("test", sheet.GetCellContents("a1"));
        }

        [TestMethod]
        public void TestSetCellContentForStringAndRemoveDependency()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            Formula f1 = new Formula("b1+1");
            sheet.SetCellContents("a1", f1);
            List<string> list = new List<string>(sheet.GetNamesOfAllNonemptyCells());
            List<string> expectedList = new List<string>();
            expectedList.Add("a1");
            CollectionAssert.AreEqual(expectedList, list);

            List<string> listB = new List<string>(sheet.SetCellContents("b1", 1));
            List<string> expectedList2 = new List<string>();
            expectedList2.Add("a1");
            expectedList2.Add("b1");
            CollectionAssert.AreEquivalent(expectedList2, listB);

            List<string> listA = new List<string>(sheet.SetCellContents("a1", "s"));
            CollectionAssert.AreEquivalent(expectedList, listA);
            listB = new List<string>(sheet.SetCellContents("b1", 1));
            expectedList2.Remove("a1");
            CollectionAssert.AreEquivalent(expectedList2, listB);
        }

        [TestMethod]
        public void TestSetCellContentSimpleDependency()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            Formula f1 = new Formula("b1+1");
            Formula f2 = new Formula("c1+2");
            sheet.SetCellContents("c1", 1);
            sheet.SetCellContents("a1", f1);
            sheet.SetCellContents("b1", f2);
            List<string> list = new List<string>(sheet.GetNamesOfAllNonemptyCells());
            List<string> expectedList = new List<string>();
            expectedList.Add("a1");
            expectedList.Add("b1");
            expectedList.Add("c1");
            CollectionAssert.AreEquivalent(expectedList, list);

            List<string> list2 = new List<string>(sheet.SetCellContents("c1", 1));
            List<string> list3 = new List<string>(sheet.SetCellContents("a1", f1));
            List<string> list4 = new List<string>(sheet.SetCellContents("b1", f2));

            List<string> expectedList2 = new List<string>();
            expectedList2.Add("a1");

            List<string> expectedList3 = new List<string>();
            expectedList3.Add("a1");
            expectedList3.Add("b1");

            CollectionAssert.AreEquivalent(expectedList, list2);
            CollectionAssert.AreEquivalent(expectedList2, list3);
            CollectionAssert.AreEquivalent(expectedList3, list4);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellContentFormulaThrow()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            Formula f1 = new Formula("A1+b2+_c3");
            sheet.SetCellContents("_2%!@", f1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSetCellContentFormulaThrow2()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            Formula f1 = null;
            sheet.SetCellContents("_", f1);
        }

        [TestMethod]
        public void TestSetCellContentFormulaReplaceOtherFormula()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("a1", "b");
            Formula f1 = new Formula("a2+3");
            List<string> list = new List<string>(sheet.SetCellContents("a1", f1));
            List<string> expected = new List<string>();
            expected.Add("a1");
            expected.Add("a2");
            List<string> list2 = new List<string>(sheet.SetCellContents("a2", 1));
            CollectionAssert.AreEquivalent(expected, list2);
        }

        [TestMethod]
        public void TestSetCellContentFormulaThrow3()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            try
            {
               
                sheet.SetCellContents("a1", "f");
                Formula f1 = new Formula("a1+3");
                sheet.SetCellContents("a1", f1);
            }
            catch(CircularException)
            {
                Assert.AreEqual("f", sheet.GetCellContents("a1"));
            }

            try
            {
                sheet.SetCellContents("a1", "");
                Formula f1 = new Formula("a1+3");
                sheet.SetCellContents("a1", f1);
            }
            catch (CircularException)
            {
                Assert.AreEqual("", sheet.GetCellContents("a1"));
            }
            try
            {
                sheet.SetCellContents("a1", 20);
                Formula f1 = new Formula("a1+3");
                sheet.SetCellContents("a1", f1);
            }
            catch (CircularException)
            {
                Assert.AreEqual(20.0, sheet.GetCellContents("a1"));
            }
            try
            {
                Formula f1 = new Formula("a2+1");
                sheet.SetCellContents("a1",f1);
                Formula f2 = new Formula("a1+3");
                sheet.SetCellContents("a1", f2);
            }
            catch (CircularException)
            {
                Formula f1 = new Formula("a2+1");
                Assert.AreEqual(f1, sheet.GetCellContents("a1"));
            }
        }

        [TestMethod]
        public void TestComplexityDependency()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            Formula f1 = new Formula("b+1");
            Formula f2 = new Formula("c+1");
            Formula f3 = new Formula("d+1");
            Formula f4 = new Formula("e+1");
            Formula f5 = new Formula("e1+1");
            sheet.SetCellContents("a", f1);
            sheet.SetCellContents("b", f2);
            sheet.SetCellContents("c", f3);
            sheet.SetCellContents("d", f4);
            sheet.SetCellContents("e", f5);

            List<string> expected =new List<string>();
            expected.Add("a");
            CollectionAssert.AreEquivalent(expected, new List<string>(sheet.SetCellContents("a", f1)));
            expected.Add("b");
            CollectionAssert.AreEquivalent(expected, new List<string>(sheet.SetCellContents("b", f2)));
            expected.Add("c");
            CollectionAssert.AreEquivalent(expected, new List<string>(sheet.SetCellContents("c", f3)));
            expected.Add("d");
            CollectionAssert.AreEquivalent(expected, new List<string>(sheet.SetCellContents("d", f4)));
            expected.Add("e");
            CollectionAssert.AreEquivalent(expected, new List<string>(sheet.SetCellContents("e",f5)));
            try
            {
                Formula f6 = new Formula("a+1");
                sheet.SetCellContents("e", f6);
            }
            catch
            {
                Assert.AreEqual(f5, sheet.GetCellContents("e"));
                CollectionAssert.AreEquivalent(expected, new List<string>(sheet.SetCellContents("e", f5)));
                expected.Add("a");
                CollectionAssert.AreNotEquivalent(expected, new List<string>(sheet.SetCellContents("e", f5)));
            }

        }


        [TestMethod]
        public void DependencyTest()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            int size = 1000;
            List<string> expectedList = new List<string>();
            for (int i = 0; i < size; i++)
            {
                int j = i + 1;
                sheet.SetCellContents("a" + i, new Formula("a"+ j + " + 1"));
                expectedList.Add("a" + i);
            }
            List<string> actualList = new List<string>(sheet.GetNamesOfAllNonemptyCells());
            CollectionAssert.AreEqual(expectedList, actualList);
            CollectionAssert.AreEquivalent(expectedList, new List<string>(sheet.SetCellContents("a999", 1)));
            Assert.AreEqual(1000, actualList.Count);
        }

        [TestMethod]
        public void DependencyTest2()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            Formula f1 = new Formula("a1+1");
            List<string> expected = new List<string>();
            expected.Add("a1");
            expected.Add("b1");
            sheet.SetCellContents("b1", f1);
            CollectionAssert.AreEquivalent(expected,new List<string>(sheet.SetCellContents("a1", 1)));
            Formula f2 = new Formula("c1+1");
            sheet.SetCellContents("b1", f2);
            expected.Remove("b1");
            CollectionAssert.AreEquivalent(expected, new List<string>(sheet.SetCellContents("a1", 1)));

            List<string> expected2 = new List<string>();
            expected2.Add("c1");
            expected2.Add("b1");
            CollectionAssert.AreEquivalent(expected2, new List<string>(sheet.SetCellContents("c1", 1)));
        }

        [TestMethod]
        public void GetContentNull()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.GetCellContents(null);

        }

    }
}
