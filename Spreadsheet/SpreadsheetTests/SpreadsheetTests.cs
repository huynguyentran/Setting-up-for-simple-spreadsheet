// Huy Nguyen (u1315096) 
// 2/19/2021
// PS4/PS5 3500 

//Tests for spreadsheet class.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Xml;
using System.IO;

namespace SS
{
    //The first 24 tests were PS4 tests that are modified to accomodate the changes in PS5
    [TestClass]
    public class SpreadsheetTests
    {
        [TestMethod]
        public void SimpleGetCellContentTest()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            string a1 = "a1";
            Assert.AreEqual(sheet.GetCellContents(a1), "");
            sheet.SetContentsOfCell(a1, "s");
            Assert.AreEqual("s", sheet.GetCellContents(a1));
        }

        [TestMethod]
        public void SimpleGetCellContentTest2()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            string a1 = "a1";
            sheet.SetContentsOfCell(a1, "24.323");
            Assert.AreEqual(24.323, sheet.GetCellContents(a1));
        }


        [TestMethod]
        public void SimpleGetCellContentTest3()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            string a1 = "a1";
            String f1 = "=1.0 + 1.0";
            Formula formula = new Formula("1 +1");
            sheet.SetContentsOfCell(a1, f1);
            Assert.AreEqual(formula, sheet.GetCellContents(a1));
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
            Assert.AreEqual("", sheet.GetCellContents("a2"));
            List<string> actualList = new List<string>(sheet.GetNamesOfAllNonemptyCells());
            Assert.AreEqual(0, actualList.Count);
        }

        [TestMethod]
        public void SimpleGetNameOfNoneEmptyCellsTest()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();

            String f1 = "=1.0 + 1.0";
            sheet.SetContentsOfCell("a1", f1);
            sheet.SetContentsOfCell("a2", "20");
            sheet.SetContentsOfCell("a3", "something cool");
            sheet.SetContentsOfCell("a4", "");

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
                sheet.SetContentsOfCell("a" + i, "" + i);
                expectedList.Add("a" + i);
            }
            List<string> actualList = new List<string>(sheet.GetNamesOfAllNonemptyCells());
            CollectionAssert.AreEqual(expectedList, actualList);
            Assert.AreEqual(1000, actualList.Count);

            List<string> emptyList = new List<string>();
            for (int i = 0; i < size; i++)
            {
                sheet.SetContentsOfCell("a" + i, "");
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
            sheet.SetContentsOfCell("@13", "20");
        }

        [TestMethod]
        public void TestSetCellContentForDouble()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            String f1 = "=a2 + 3";
            Formula formula = new Formula("a2+3");
            sheet.SetContentsOfCell("a1", f1);

            List<string> list = new List<string>(sheet.GetNamesOfAllNonemptyCells());
            List<string> expectedList = new List<string>();
            expectedList.Add("a1");
            CollectionAssert.AreEqual(expectedList, list);
            Assert.AreEqual(formula, sheet.GetCellContents("a1"));

            List<string> list2 = new List<string>(sheet.SetContentsOfCell("a1", f1));
            CollectionAssert.AreEqual(expectedList, list2);

            sheet.SetContentsOfCell("a2", "");
            List<string> expectedList2 = new List<string>();
            expectedList2.Add("a2");
            expectedList2.Add("a1");

            list2 = new List<string>(sheet.SetContentsOfCell("a2", ""));
            CollectionAssert.AreEqual(expectedList2, list2);
            sheet.SetContentsOfCell("a1", "20");
            Assert.AreNotEqual(f1, sheet.GetCellContents("a1"));
            Assert.AreEqual(20.0, sheet.GetCellContents("a1"));


        }

        [TestMethod]
        public void TestSetCellContentForDouble2()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();

            sheet.SetContentsOfCell("a1", "13.0");

            List<string> list = new List<string>(sheet.GetNamesOfAllNonemptyCells());
            List<string> expectedList = new List<string>();
            expectedList.Add("a1");
            CollectionAssert.AreEqual(expectedList, list);
            Assert.AreEqual(13.0, sheet.GetCellContents("a1"));

            sheet.SetContentsOfCell("a1", "20");
            Assert.AreNotEqual(13.0, sheet.GetCellContents("a1"));
            Assert.AreEqual(20.0, sheet.GetCellContents("a1"));
        }

        [TestMethod]
        public void TestSetCellContentForDouble3()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();

            sheet.SetContentsOfCell("a1", "a");

            List<string> list = new List<string>(sheet.GetNamesOfAllNonemptyCells());
            List<string> expectedList = new List<string>();
            expectedList.Add("a1");
            CollectionAssert.AreEqual(expectedList, list);
            Assert.AreEqual("a", sheet.GetCellContents("a1"));

            sheet.SetContentsOfCell("a1", "20");
            Assert.AreNotEqual("a", sheet.GetCellContents("a1"));
            Assert.AreEqual(20.0, sheet.GetCellContents("a1"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSetCellContentForStringThrow()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            string s = null;
            sheet.SetContentsOfCell("_12312asd23", s);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellContentForStringThrow2()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            string s = "test";
            sheet.SetContentsOfCell("^12asd23", s);
        }

        [TestMethod]
        public void TestSetCellContentForString()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("a1", "19");
            List<string> list = new List<string>(sheet.GetNamesOfAllNonemptyCells());
            List<string> expectedList = new List<string>();
            expectedList.Add("a1");
            CollectionAssert.AreEqual(expectedList, list);
            Assert.AreEqual(19.0, sheet.GetCellContents("a1"));
            sheet.SetContentsOfCell("a1", "test");
            Assert.AreNotEqual(19, sheet.GetCellContents("a1"));
            Assert.AreEqual("test", sheet.GetCellContents("a1"));
        }

        [TestMethod]
        public void TestSetCellContentForStringAndRemoveDependency()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            String f1 = "=b1+1";
            sheet.SetContentsOfCell("a1", f1);
            List<string> list = new List<string>(sheet.GetNamesOfAllNonemptyCells());
            List<string> expectedList = new List<string>();
            expectedList.Add("a1");
            CollectionAssert.AreEqual(expectedList, list);

            List<string> listB = new List<string>(sheet.SetContentsOfCell("b1", "1"));
            List<string> expectedList2 = new List<string>();
            expectedList2.Add("a1");
            expectedList2.Add("b1");
            CollectionAssert.AreEquivalent(expectedList2, listB);

            List<string> listA = new List<string>(sheet.SetContentsOfCell("a1", "s"));
            CollectionAssert.AreEquivalent(expectedList, listA);
            listB = new List<string>(sheet.SetContentsOfCell("b1", "1"));
            expectedList2.Remove("a1");
            CollectionAssert.AreEquivalent(expectedList2, listB);
        }

        [TestMethod]
        public void TestSetContentsOfCellimpleDependency()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            string f1 = "= b1+1";
            string f2 = "=c1+2";
            sheet.SetContentsOfCell("c1", "1");
            sheet.SetContentsOfCell("a1", f1);
            sheet.SetContentsOfCell("b1", f2);
            List<string> list = new List<string>(sheet.GetNamesOfAllNonemptyCells());
            List<string> expectedList = new List<string>();
            expectedList.Add("a1");
            expectedList.Add("b1");
            expectedList.Add("c1");
            CollectionAssert.AreEquivalent(expectedList, list);

            List<string> list2 = new List<string>(sheet.SetContentsOfCell("c1", "1"));
            List<string> list3 = new List<string>(sheet.SetContentsOfCell("a1", f1));
            List<string> list4 = new List<string>(sheet.SetContentsOfCell("b1", f2));

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
            string f1 = "=A1+b2+c3";
            sheet.SetContentsOfCell("_2%!@", f1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSetCellContentFormulaThrow2()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            string f1 = null;
            sheet.SetContentsOfCell("a1", f1);
        }

        [TestMethod]
        public void TestSetCellContentFormulaReplaceOtherFormula()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("a1", "b");
            String f1 = "=a2+3";
            List<string> list = new List<string>(sheet.SetContentsOfCell("a1", f1));
            List<string> expected = new List<string>();
            expected.Add("a1");
            expected.Add("a2");
            List<string> list2 = new List<string>(sheet.SetContentsOfCell("a2", "1"));
            CollectionAssert.AreEquivalent(expected, list2);
        }

        [TestMethod]
        public void TestSetCellContentFormulaThrow3()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            try
            {

                sheet.SetContentsOfCell("a1", "f");
                string f1 = "=a1+3";
                sheet.SetContentsOfCell("a1", f1);
            }
            catch (CircularException)
            {
                Assert.AreEqual("f", sheet.GetCellContents("a1"));
            }

            try
            {
                sheet.SetContentsOfCell("a1", "");
                string f1 = "=a1+3";
                sheet.SetContentsOfCell("a1", f1);
            }
            catch (CircularException)
            {
                Assert.AreEqual("", sheet.GetCellContents("a1"));
            }
            try
            {
                sheet.SetContentsOfCell("a1", "20");
                string f1 = "=a1+3";
                sheet.SetContentsOfCell("a1", f1);
            }
            catch (CircularException)
            {
                Assert.AreEqual(20.0, sheet.GetCellContents("a1"));
            }

            try
            {
                string f1 = "=a2+1";
                sheet.SetContentsOfCell("a1", f1);
                string f2 = "=a1+3";
                sheet.SetContentsOfCell("a1", f2);
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
            string f1 = "=b1+1";
            string f2 = "=c1+1";
            string f3 = "=d1+1";
            string f4 = "=e1+1";
            string f5 = "=e2+1";
            sheet.SetContentsOfCell("a1", f1);
            sheet.SetContentsOfCell("b1", f2);
            sheet.SetContentsOfCell("c1", f3);
            sheet.SetContentsOfCell("d1", f4);
            sheet.SetContentsOfCell("e1", f5);

            List<string> expected = new List<string>();
            expected.Add("a1");
            CollectionAssert.AreEquivalent(expected, new List<string>(sheet.SetContentsOfCell("a1", f1)));
            expected.Add("b1");
            CollectionAssert.AreEquivalent(expected, new List<string>(sheet.SetContentsOfCell("b1", f2)));
            expected.Add("c1");
            CollectionAssert.AreEquivalent(expected, new List<string>(sheet.SetContentsOfCell("c1", f3)));
            expected.Add("d1");
            CollectionAssert.AreEquivalent(expected, new List<string>(sheet.SetContentsOfCell("d1", f4)));
            expected.Add("e1");
            CollectionAssert.AreEquivalent(expected, new List<string>(sheet.SetContentsOfCell("e1", f5)));
            try
            {
                string f6 = "=a1+1";
                sheet.SetContentsOfCell("e1", f6);
            }
            catch
            {
                string f6 = "e2+1";
                Formula formula = new Formula(f6);
                Assert.AreEqual(formula, sheet.GetCellContents("e1"));
                CollectionAssert.AreEquivalent(expected, new List<string>(sheet.SetContentsOfCell("e1", f5)));
                expected.Add("a1");
                CollectionAssert.AreNotEquivalent(expected, new List<string>(sheet.SetContentsOfCell("e1", f5)));
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
                sheet.SetContentsOfCell("a" + i, "= " + "a" + j + " + 1");
                expectedList.Add("a" + i);
            }
            List<string> actualList = new List<string>(sheet.GetNamesOfAllNonemptyCells());
            CollectionAssert.AreEqual(expectedList, actualList);
            Assert.AreEqual(new Formula("a1000 +1"), sheet.GetCellContents("a999"));
            CollectionAssert.AreEquivalent(expectedList, new List<string>(sheet.SetContentsOfCell("a999", "1")));
            Assert.AreEqual(1000, actualList.Count);
            sheet.SetContentsOfCell("a1000", "1");
            Assert.AreEqual(1000.0, sheet.GetCellValue("a0"));
         
        }


        [TestMethod]
        public void DependencyTest2()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            string f1 = "=a1+1";
            List<string> expected = new List<string>();
            expected.Add("a1");
            expected.Add("b1");
            sheet.SetContentsOfCell("b1", f1);
            CollectionAssert.AreEquivalent(expected, new List<string>(sheet.SetContentsOfCell("a1", "1")));
            string f2 = "=c1+1";
            sheet.SetContentsOfCell("b1", f2);
            expected.Remove("b1");
            CollectionAssert.AreEquivalent(expected, new List<string>(sheet.SetContentsOfCell("a1", "1")));

            List<string> expected2 = new List<string>();
            expected2.Add("c1");
            expected2.Add("b1");
            CollectionAssert.AreEquivalent(expected2, new List<string>(sheet.SetContentsOfCell("c1", "1")));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetContentNull()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.GetCellContents(null);

        }


        //NEW PS5 TESTS
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetUtilities.FormulaFormatException))]
        public void TestEmptyFormula()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            string f1 = "=";
            sheet.SetContentsOfCell("a1", f1);
        }

        [TestMethod]
        public void TestSimpleGetValue()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            string f1 = "=a5+a4+a3";
            sheet.SetContentsOfCell("a1", f1);
            sheet.SetContentsOfCell("a5", "4");
            sheet.SetContentsOfCell("a4", "3");
            sheet.SetContentsOfCell("a3", "3");
            Assert.AreEqual(10.0, sheet.GetCellValue("a1"));
        }

        [TestMethod]
        public void TestInsert()
        {
            using (XmlWriter writer = XmlWriter.Create("save.txt"))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "1.2");

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A1");
                writer.WriteElementString("content", "hello");
                writer.WriteEndElement();

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A2");
                writer.WriteElementString("content", "20");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            AbstractSpreadsheet ss = new Spreadsheet("save.txt", s => true, s => s, "1.2");
            Assert.IsFalse(ss.Changed);
            Assert.AreEqual("hello", ss.GetCellValue("A1"));
            Assert.IsFalse(ss.Changed);
            Assert.AreEqual(20.0, ss.GetCellValue("A2"));
            Assert.IsFalse(ss.Changed);
            Assert.AreEqual("1.2", ss.GetSavedVersion("save.txt"));
            Assert.IsFalse(ss.Changed);
            ss.SetContentsOfCell("A3", "new Content");
            Assert.IsTrue(ss.Changed);
        }

        [TestMethod]
        public void TestGetValue()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("a1", "15");
            sheet.SetContentsOfCell("a2", "string");
            sheet.SetContentsOfCell("a3", "=a1+a2");
            Assert.AreEqual(15.0, sheet.GetCellValue("a1"));
            Assert.AreEqual("string", sheet.GetCellValue("a2"));
            object obj = new SpreadsheetUtilities.FormulaError();
            Assert.IsTrue(sheet.GetCellValue("a3") is SpreadsheetUtilities.FormulaError);
            sheet.SetContentsOfCell("a2", "5");
            Assert.AreEqual(20.0, sheet.GetCellValue("a3"));
            Assert.AreEqual("", sheet.GetCellValue("a4"));
            sheet.SetContentsOfCell("a4", "newstring");
            sheet.Save("newSave2.text");
        }


        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetValueNull()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.GetCellValue(null);

        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetValueInvalid()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.GetCellValue("@213");
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void ThrowNoFileFound()
        {
            AbstractSpreadsheet ss = new Spreadsheet("doesntexist.txt", s => true, s => s, "");
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void ThrowGetSaveVersion()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.GetSavedVersion("doesnntexist.txt");
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void ThrowGetSaveVersion2()
        {
            using (XmlWriter writer = XmlWriter.Create("save.txt"))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadSheet");
                writer.WriteAttributeString("version", "1.2");

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A1");
                writer.WriteElementString("content", "hello");
                writer.WriteEndElement();

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A2");
                writer.WriteElementString("content", "20");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            AbstractSpreadsheet ss = new Spreadsheet();
            ss.GetSavedVersion("save.txt");
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void ThrowSave()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("a1", "5");
            ss.SetContentsOfCell("a2", "string");
            ss.SetContentsOfCell("a3", "4");
            ss.SetContentsOfCell("a4", "=a1+a3");
            ss.Save(null);
        }
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void ThrowIlegalName()
        {
            AbstractSpreadsheet ss = new Spreadsheet(null, s => true, s => s, "1.23");

        }
        [TestMethod]
        public void NoFilePathConstructor()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => s == "a1" || s == "a3" || s == "a2", s => s.ToLower(), "1.23");
            ss.SetContentsOfCell("a1", "1");
            ss.SetContentsOfCell("a3", "3");
            ss.SetContentsOfCell("a2", "=a1 +a3");
            Assert.AreEqual(4.0, ss.GetCellValue("a2"));
            Assert.AreEqual(4.0, ss.GetCellValue("a2"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void NormalizeThrow()
        {
            AbstractSpreadsheet sheet = new Spreadsheet(s => true, s => null, "1.23");
            sheet.SetContentsOfCell("a2", "30");

        }
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void NormalizeThrow2()
        {
            AbstractSpreadsheet sheet = new Spreadsheet(s => true, s => "@a23", "1.23");
            sheet.SetContentsOfCell("a2", "30");

        }

        [TestMethod]
        public void ComplexCalculation()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("a1", "1");
            sheet.SetContentsOfCell("a2", "=a1+1");
            Assert.AreEqual(2.0, sheet.GetCellValue("a2"));
            List<string> expected = new List<string>();
            expected.Add("a1");
            expected.Add("a2");
            List<string> list = new List<string>(sheet.SetContentsOfCell("a1", "1"));
            CollectionAssert.AreEqual(expected, list);
            sheet.SetContentsOfCell("a3", "=a2+1");
            Assert.AreEqual(3.0, sheet.GetCellValue("a3"));
            sheet.SetContentsOfCell("a4", "=a3+1");
            Assert.AreEqual(4.0, sheet.GetCellValue("a4"));
            sheet.SetContentsOfCell("a5", "=a4+1");
            Assert.AreEqual(5.0, sheet.GetCellValue("a5"));
            expected.Add("a3");
            expected.Add("a4");
            expected.Add("a5");
            list = new List<string>(sheet.SetContentsOfCell("a1", "1"));
            CollectionAssert.AreEqual(expected, list);
            sheet.SetContentsOfCell("a1", "2");
            Assert.AreEqual(6.0, sheet.GetCellValue("a5"));
        }


        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SaveNonExistPath()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("a1", "1");
            sheet.SetContentsOfCell("a2", "=a1+1");
            sheet.Save("if/you/are/reading/this/,/I/hope/you/have/a/good/day.xml");
        }
        [TestMethod]
        public void TakingInExistingFile()
        {
            using (XmlWriter writer = XmlWriter.Create("test.txt"))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "1.2");

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "X1");
                writer.WriteElementString("content", "hello");
                writer.WriteEndElement();

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "X2");
                writer.WriteElementString("content", "20");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            AbstractSpreadsheet ss = new Spreadsheet("test.txt", s => true, s => s.ToUpper(), "1.2");
            Assert.AreEqual("1.2", ss.GetSavedVersion("test.txt"));
            Assert.IsFalse(ss.Changed);
            ss.SetContentsOfCell("x3", "4");
            List<string> expected = new List<string>();
            expected.Add("X1");
            expected.Add("X2");
            expected.Add("X3");
            CollectionAssert.AreEquivalent(expected, new List<string>(ss.GetNamesOfAllNonemptyCells()));
            Assert.AreEqual(20.0, ss.GetCellValue("X2"));
            ss.SetContentsOfCell("x2", "30");
            Assert.AreEqual(30.0, ss.GetCellValue("X2"));
            Assert.IsTrue(ss.Changed);
            ss.Save("test.txt");
            Assert.IsFalse(ss.Changed);
        }

        [TestMethod]
        public void DefaultVersion()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("a1", "1");
            ss.SetContentsOfCell("a2", "2");
            ss.Save("newSS.txt");
            Assert.AreEqual("default",ss.GetSavedVersion("newSS.txt"));
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void ConstructorThrowWrongVersion()
        {
            using (XmlWriter writer = XmlWriter.Create("wrong.txt"))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "1.2");

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "X1");
                writer.WriteElementString("content", "hello");
                writer.WriteEndElement();

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "X2");
                writer.WriteElementString("content", "20");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            int[] a = new int[] { 1, 2, 3 };
            AbstractSpreadsheet ss = new Spreadsheet("wrong.txt", s => true, s => s.ToUpper(), "1.3");
           
        }


        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void ConstructorWithoutVersion()
        {
            using (XmlWriter writer = XmlWriter.Create("final.txt"))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "X1");
                writer.WriteElementString("content", "hello");
                writer.WriteEndElement();

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "X2");
                writer.WriteElementString("content", "20");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            AbstractSpreadsheet ss = new Spreadsheet();
            Assert.AreEqual("default", ss.GetSavedVersion("final.txt"));

        }


        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void ConstructorWithoutVersion2()
        {
            using (XmlWriter writer = XmlWriter.Create("final2.txt"))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "X1");
                writer.WriteElementString("content", "hello");
                writer.WriteEndElement();

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "X2");
                writer.WriteElementString("content", "20");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            AbstractSpreadsheet ss = new Spreadsheet("final2.txt", s => true, s => s.ToUpper(), null);
        }




    }
}
