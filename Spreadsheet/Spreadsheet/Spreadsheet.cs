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

        private Dictionary<string, Cell> spreadsheet = new Dictionary<string, Cell>();
        private DependencyGraph graph = new DependencyGraph();

        public override object GetCellContents(string name)
        {
            if (!nameValidation(name))
            {
                throw new InvalidNameException();
            }

            if (!spreadsheet.ContainsKey(name))
            {
                return "";
            }

            return spreadsheet[name].getContent();


        }

        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            return new List<string>(spreadsheet.Keys);

        }

        public override IList<string> SetCellContents(string name, double number)
        {
            if (!nameValidation(name))
            {
                throw new InvalidNameException();
            }


            if (spreadsheet.ContainsKey(name))
            {
                if (spreadsheet[name].getContent().GetType() == typeof(Formula) && graph.HasDependees(name))
                {
                    graph.ReplaceDependees(name, new HashSet<string>());
                }

                spreadsheet[name].setContent(number);
            }
            else
            {
                spreadsheet.Add(name, new Cell(number));
            }

            return new List<string>(GetCellsToRecalculate(name));
        }

        public override IList<string> SetCellContents(string name, string text)
        {

            if (text is null)
            {
                throw new ArgumentNullException();
            }

            if (!nameValidation(name))
            {
                throw new InvalidNameException();
            }



            if (spreadsheet.ContainsKey(name))
            {
                if (spreadsheet[name].getContent().GetType() == typeof(Formula) && graph.HasDependees(name))
                {
                    graph.ReplaceDependees(name, new HashSet<string>());

                }
                spreadsheet[name].setContent(text);
            }

            else
            {
                spreadsheet.Add(name, new Cell(text));

            }

            if (text.Equals(""))
            {
                spreadsheet.Remove(name);
            }

            return new List<string>(GetCellsToRecalculate(name));
        }

        public override IList<string> SetCellContents(string name, Formula formula)
        {
            //Do we need this because formula already throws null.
            if (formula is null)
            {
                throw new ArgumentNullException();
            }

            if (!nameValidation(name))
            {
                throw new InvalidNameException();
            }

            object original = GetCellContents(name);

            if (spreadsheet.ContainsKey(name))
            {
                if (spreadsheet[name].getContent().GetType() == typeof(Formula) && graph.HasDependees(name))
                {
                    graph.ReplaceDependees(name, new HashSet<string>());
                }

                spreadsheet[name].setContent(formula);

                foreach (string token in formula.GetVariables())
                {
                    graph.AddDependency(token, name);
                }
            }
            else
            {
                spreadsheet.Add(name, new Cell(formula));
                foreach (string token in formula.GetVariables())
                {
                    graph.AddDependency(token, name);
                }
            }
            try
            {
                IEnumerable<string> list = GetCellsToRecalculate(name);
                return new List<string>(list);
            }
            catch
            {

                if (original.GetType() == typeof(string))
                {
                    SetCellContents(name, (string)original);
                }
                else if (original.GetType() == typeof(double))
                {
                    SetCellContents(name, (double)original);
                }
                else 
                {
                    SetCellContents(name, (Formula)original);
                }
         
                throw new CircularException();
            }

        }

        
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            return graph.GetDependents(name);
        }



        private bool nameValidation(string name)
        {
            String varPattern = @"^[a-zA-Z_](?: [a-zA-Z_]|\d)*$";
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

        public object content;
        // public double value;

        //public Cell()
        //{
        //    content = null;
        //}
        public Cell(Formula _content)
        {
            content = _content;
            //   value = _value;
        }
        public Cell(string _content)
        {
            content = _content;
            //   value = _value;
        }
        public Cell(double _content)
        {
            content = _content;
            //   value = _value;
        }

        public object getContent()
        {
            //if (content is null)
            //{
            //    return "";
            //}
            return content;
        }

        //public bool isEmpty()
        //{
        //    if (content is null || content.Equals(""))
        //    {
        //        return true;
        //    }
        //    return false;
        //}

        public void setContent(object obj)
        {
            content = obj;
        }

    }

}
