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
                if (spreadsheet[name].GetType() == typeof(Formula) && graph.HasDependents(name))
                {
                    graph.ReplaceDependents(name, new HashSet<string>());
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
                if (spreadsheet[name].GetType() == typeof(Formula) && graph.HasDependents(name))
                {
                    graph.ReplaceDependents(name, new HashSet<string>());
                    //if (spreadsheet.ContainsKey(text))
                    //{
                    //    graph.AddDependency(name, text);
                    //    //Check for circular here
                    //}

                }
                spreadsheet[name].setContent(text);
            }

            else
            {
                spreadsheet.Add(name, new Cell(text));
                //if (nameValidation(text))
                //{
                //    if ((spreadsheet[name].getContent().Equals("")))
                //    {
                //        spreadsheet.Add(text, null);
                //    }
                //    graph.AddDependency(name, text);

                //}
            }

            return new List<string>(GetCellsToRecalculate(name));
        }

        public override IList<string> SetCellContents(string name, Formula formula)
        {
            if (formula is null)
            {
                throw new ArgumentNullException();
            }

            if (!nameValidation(name))
            {
                throw new InvalidNameException();
            }

            //Checking circular.
            foreach (string token in Formula.GetTokens(formula))
            {
                if (nameValidation(token))
                {
                    if (token.Equals(name) || GetCellsToRecalculate(token).Contains(name))
                    {
                        throw new CircularException();
                    }
                }
            }


            if (spreadsheet.ContainsKey(name))
            {
                if (spreadsheet[name].GetType() == typeof(Formula) && graph.HasDependents(name))
                {
                    graph.ReplaceDependents(name, new HashSet<string>());
                }
                
                foreach (string token in Formula.GetTokens(formula))
                {
                    if (nameValidation(token))
                    {
                        //Do we actually need this ?
                        if (!spreadsheet.ContainsKey(token))
                        {
                            spreadsheet.Add(token, null);
                        }
                        graph.AddDependency(name, token);
                      
                    }
                }

                spreadsheet[name].setContent(formula);

            }
            else
            {
                spreadsheet.Add(name, new Cell(formula));
                foreach (string token in Formula.GetTokens(formula))
                {
                    if (nameValidation(token))
                    {
                        //Do we actually need this ?
                        if (!spreadsheet.ContainsKey(token))
                        {
                            spreadsheet.Add(token, null);
                        }
                        graph.AddDependency(name, token);

                    }
                }
            }

            return new List<string>(GetCellsToRecalculate(name));
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
