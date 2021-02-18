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
        private DependencyGraph graph;
        public override object GetCellContents(string name)
        {
            if (!nameValidation(name))
            {
                throw new InvalidNameException();
            }

            if (spreadsheet[name].GetType() == typeof(Formula))
            {

                return (Formula)spreadsheet[name].getContent();
            }

            if (spreadsheet[name].GetType() == typeof(double))
            {

                return (double)spreadsheet[name].getContent();
            }

            return (string)spreadsheet[name].getContent();
            // throw new NotImplementedException();
        }

        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {

            //List<string> nonEmptyCells = new List<string>();

            //foreach (KeyValuePair<string, Cell> entry in spreadsheet)
            //{
            //    //if (!spreadsheet[entry.Key].Equals(""))
            //    //{
            //        nonEmptyCells.Add(entry.Key);
            //   // }
            //}
            //Remove every empty cell 
            return new List<string>(spreadsheet.Keys);
        }

        public override IList<string> SetCellContents(string name, double number)
        {
            if (!nameValidation(name))
            {
                throw new InvalidNameException();
            }
           
            spreadsheet[name].setContent(number);

            return new List<string>(GetCellsToRecalculate(name));

            //HashSet<string> list = new HashSet<string>();
            //list.Add(name);
            //list.Union(graph.GetDependents(name));
            //foreach(string e in graph.GetDependents(name))
            //{
            //    list.Union(graph.GetDependents(e));
            //}
            //return new List<string>(list);
            // throw new NotImplementedException();
        }

        public override IList<string> SetCellContents(string name, string text)
        {


            if (!nameValidation(name))
            {
                throw new InvalidNameException();
            }

            //if (text.Equals(""))
            //{
            //    spreadsheet[name].setContent(text);
            //    spreadsheet.Remove(name);
            //    HashSet<string> list2 = new HashSet<string>();

            //    list2.Add(name);

            //    list2.Union(graph.GetDependents(name));

            //    foreach (string e in graph.GetDependents(name))
            //    {
            //        list2.Union(graph.GetDependents(e));
            //    }
            //    return new List<string>(list2);
            //}

            spreadsheet[name].setContent(text);

            return new List<string>(GetCellsToRecalculate(name));

            //HashSet<string> list = new HashSet<string>();
            //list.Add(name);
            //list.Union(graph.GetDependents(name));
            //foreach (string e in graph.GetDependents(name))
            //{
            //    list.Union(graph.GetDependents(e));
            //}
            //return new List<string>(list);
            // throw new NotImplementedException();
        }

        public override IList<string> SetCellContents(string name, Formula formula)
        {

            if (!nameValidation(name))
            {
                throw new InvalidNameException();
            }

            spreadsheet[name].setContent(formula);
            return new List<string>(GetCellsToRecalculate(name));

            //if (formula.ToString().Contains(name))
            //{
            //    graph.AddDependency(name, formula.ToString);
            //}
            //spreadsheet[name].setContent(formula);
            //HashSet<string> list = new HashSet<string>();
            //list.Add(name);
            //list.Union(GetDirectDependents(name));
            //foreach (string e in graph.GetDependents(name))
            //{
            //    list.Union(GetDirectDependents(e));
            //}
            //return new List<string>(list);
            // throw new NotImplementedException();
        }

        protected override IEnumerable<string> GetDirectDependents(string name)
        {

            if (!nameValidation(name))
            {
                throw new InvalidNameException();
            }
            return graph.GetDependents(name);
          //  throw new NotImplementedException();
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
        // public double value;

        public Cell()
        {
            content = null;
        }
        public Cell(Object _content)
        {
            content = _content;
            //   value = _value;
        }

        public object getContent()
        {
            if (content is null)
            {
                return "";
            }
            return content;
        }

        public void setContent(object obj)
        {
            content = obj;
        }

    }

}
