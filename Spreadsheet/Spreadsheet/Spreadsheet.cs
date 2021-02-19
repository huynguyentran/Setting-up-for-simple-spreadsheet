// Huy Nguyen (u1315096) 
// 2/19/2021
// PS4 3500

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

        /// <summary>
        /// A method that sets the conent of the named Cell to a double.
        /// The method returns a list consisting of name plus the names of 
        /// all other cells whose value depends, directly or indirectly, on 
        /// the named cell.
        /// 
        /// </summary>
        /// <param name="name">The string cell name</param>
        /// <param name="number">The dobule content</param>
        /// <throw> InvalidNameException() if the name is not valid.</throw>
        /// <returns>A list of name that depended directly and indirectly on the named Cell.</returns>
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

        /// <summary>
        /// A method that returns the list of name of non-empty cells in the spreadsheet.
        /// 
        /// </summary>
        /// <returns>A list of name of non-empty cells in the spreadsheet.</return>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            return new List<string>(spreadsheet.Keys);

        }

        /// <summary>
        /// A method that sets the conent of the named Cell to a double.
        /// The method returns a list consisting of name plus the names of 
        /// all other cells whose value depends, directly or indirectly, on 
        /// the named cell.
        /// 
        /// </summary>
        /// <param name="name">The string cell name</param>
        /// <param name="number">The dobule content</param>
        /// <throw> InvalidNameException() if the name is not valid.</throw>
        /// <returns>A list of name that depended directly and indirectly on the named Cell.</returns>
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

        /// <summary>
        /// A method that sets the conent of the named Cell to a string.
        /// The method returns a list consisting of name plus the names of 
        /// all other cells whose value depends, directly or indirectly, on 
        /// the named cell.
        /// 
        /// </summary>
        /// <param name="name">The string cell name</param>
        /// <param name="text">The string content</param>
        /// <throw> InvalidNameException() if the name is not valid.</throw>
        /// <throw> ArgumentNullException() if the text is null.</throw>
        /// <returns>A list of name that depended directly and indirectly on the named Cell.</returns>
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

        /// <summary>
        /// A method that sets the conent of the named Cell to a Formula.
        /// The method returns a list consisting of name plus the names of 
        /// all other cells whose value depends, directly or indirectly, on 
        /// the named cell.
        /// 
        /// </summary>
        /// <param name="name">The string cell name</param>
        /// <param name="Formula">The Formula content</param>
        /// <throw> ArgumentNullException() if the text is null.</throw>
        /// <throw> CircularException() if the Formula creates Circular.</throw>
        /// <returns>A list of name that depended directly and indirectly on the named Cell.</returns>
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

        /// <summary>
        /// A protected method to get direct dependents.
        /// 
        /// </summary>
        /// <param name="name">The cell name</param>
        /// <returns>A list of dependents.</returns>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            return graph.GetDependents(name);
        }


        /// <summary>
        /// A private method to validiate the name of the cell. Returns true if 
        /// the name is valid. Otherwises return false. 
        /// 
        /// </summary>
        /// <param name="name">The cell name</param>
        /// <returns>true/false.</returns>
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



    /// <summary>
    /// A class represented a Cell that holds a content object. 
    /// 
    /// </summary>
    class Cell
    {

        private object content;

        /// <summary>
        /// A constructor to initializing the cell.
        /// 
        /// </summary>
        /// <param name="_content">The object content(Formula)</param>
        public Cell(Formula _content)
        {
            content = _content;
        }

        /// <summary>
        /// A constructor to initializing the cell.
        /// 
        /// </summary>
        /// <param name="_content">The object content(string)</param>
        public Cell(string _content)
        {
            content = _content;
        }

        /// <summary>
        /// A constructor to initializing the cell.
        /// 
        /// </summary>
        /// <param name="_content">The object content(double)</param>
        public Cell(double _content)
        {
            content = _content;
        }

        /// <summary>
        /// A constructor to initializing the cell.
        /// 
        /// </summary>
        /// <returns>The content of the cell.</returns>
        public object getContent()
        {
            return content;
        }

        /// <summary>
        /// A method to change the content of a cell. 
        /// 
        /// </summary>
        /// <param name="_content">The object content(object)</param>
        public void setContent(object obj)
        {
            content = obj;
        }

    }

}
