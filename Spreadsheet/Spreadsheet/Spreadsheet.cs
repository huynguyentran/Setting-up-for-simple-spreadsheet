using SpreadsheetUtilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace SS
{
    public class Spreadsheet : AbstractSpreadsheet
    {
        private Dictionary<string, Cell> spreadsheet;

        public override object GetCellContents(string name)
        {

            if (!nameValidation(name))
            {
                throw new InvalidNameException();
            }



            throw new NotImplementedException();
        }

        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {

            List<string> nonEmptyCells = new List<string>();
            foreach (KeyValuePair<string, Cell> entry in spreadsheet)
            {
                if (!spreadsheet[entry.Key].Equals(""))
                {
                    nonEmptyCells.Add(entry.Key);
                }
            }
            return new List<string>(nonEmptyCells);
   
        }

        public override IList<string> SetCellContents(string name, double number)
        {

            if (!nameValidation(name))
            {
                throw new InvalidNameException();
            }

            throw new NotImplementedException();
        }

        public override IList<string> SetCellContents(string name, string text)
        {


            if (!nameValidation(name))
            {
                throw new InvalidNameException();
            }

            throw new NotImplementedException();
        }

        public override IList<string> SetCellContents(string name, Formula formula)
        {

            if (!nameValidation(name))
            {
                throw new InvalidNameException();
            }
            throw new NotImplementedException();
        }

        protected override IEnumerable<string> GetDirectDependents(string name)
        {

            if (!nameValidation(name))
            {
                throw new InvalidNameException();
            }
            throw new NotImplementedException();
        }



        private bool nameValidation(string name)
        {
            String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            Regex regVar = new Regex(varPattern, RegexOptions.IgnorePatternWhitespace);
            if (!regVar.IsMatch(name) || name is null)
            {
                return false;
            }
            return true;
        }
    }


    class Cell
    {

      
        public Object content;
        public double value;

        public Cell(Object _content, double _value)
        {
            content = _content;
            value = _value;
        }

        public object getContent(string name)
        {
            if (content is null )
            {
                return "";
            }

    
            return content;
        }



    }

    //public class Student
    //{
    //    private string major;
    //    public int ID;
    //    public float GPA;


    //    public Student(string _major, int _ID, float _GPA)
    //    {
    //        ID = _ID;
    //        major = _major;
    //        GPA = _GPA;
    //    }

    //    public string GetMajor()
    //    {
    //        return major;
    //    }

    //}
}
