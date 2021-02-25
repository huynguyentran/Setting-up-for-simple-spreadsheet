// Huy Nguyen (u1315096) 
// 2/19/2021
// PS4 3500

// Implementing methods for AbstractSpreadsheet to create afunctional spreadsheet.

using SpreadsheetUtilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace SS
{
    public class Spreadsheet : AbstractSpreadsheet
    {

        // Regex regex = new Regex(@"\b[a-zA-Z]+\d+\b");

        private Dictionary<string, Cell> spreadsheet;
        private DependencyGraph graph;

        /// <summary>
        /// A zero-argument constructor that creates an empty spreadsheet.
        /// 
        /// </summary>
        public Spreadsheet() : base(s => true, s => s, "default")
        {
            spreadsheet = new Dictionary<string, Cell>();
            graph = new DependencyGraph();
        }

        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version)
        {
            spreadsheet = new Dictionary<string, Cell>();
            graph = new DependencyGraph();
        }


        public Spreadsheet(string filename, Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version)
        {
            spreadsheet = new Dictionary<string, Cell>();
            graph = new DependencyGraph();
            try
            {
                using (XmlReader reader = XmlReader.Create(filename))
                {
                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            switch (reader.Name)
                            {
                                case "spreadsheet":
                                    Console.WriteLine("spreadsheet found: " + reader["version"]);
                                    break;
                                case "cell":
                                    Console.Write("Found cell:");
                                    break;
                                case "name":
                                    Console.Write("\tName = ");
                                    reader.Read();
                                    Console.WriteLine(reader.Value);
                                    break;
                                case "content":
                                    Console.Write("\tName = ");
                                    reader.Read();
                                    Console.WriteLine(reader.Value);
                                    break;
                            }
                        }
                        else
                        {
                            if (reader.Name == "cell")
                                Console.WriteLine("end of cells");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new SpreadsheetReadWriteException(e.Message);
            }

        }

        public override bool Changed
        {
            get; protected set;
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
        public override object GetCellContents(string name)
        {
            if (!variableValidator(name))
            {
                throw new InvalidNameException();
            }
            string newName = Normalize(name);
            if (!spreadsheet.ContainsKey(newName))
            {
                return "";
            }
            return spreadsheet[newName].getContent();
        }

        public override object GetCellValue(string name)
        {
            if (!variableValidator(name))
            {
                throw new InvalidNameException();
            }
            string newName = Normalize(name);
            if (!spreadsheet.ContainsKey(newName))
            {
                return "";
            }
            return returnValue(newName);
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

        public override string GetSavedVersion(string filename)
        {
            //Takes in filename, read it, find the spreadsheetelement and get attribute
            throw new NotImplementedException();
        }

        public override void Save(string filename)
        {
            //Github example XM
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  ";
            try
            {
                using (XmlWriter writer = XmlWriter.Create(filename, settings))
                {

                    writer.WriteStartDocument();
                    writer.WriteStartElement("spreadsheet");
                    // This adds an attribute to the Nation element
                    writer.WriteAttributeString("version", this.Version);
                    foreach (KeyValuePair<string, Cell> entry in spreadsheet)
                    {
                        writer.WriteStartElement("cell");
                        writer.WriteStartElement("name", entry.Key);
                        if (entry.Value.getContent() is string)
                        {
                            writer.WriteStartElement("content", (string)entry.Value.getContent());
                        }
                        else if (entry.Value.getContent() is double)
                        {
                            writer.WriteStartElement("content", entry.Value.getContent().ToString());
                        }
                        else
                        {
                            writer.WriteStartElement("content", "=" + entry.Value.getContent().ToString());
                        }
                        writer.WriteEndElement();
                        writer.WriteEndElement();
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                    writer.WriteEndDocument();

                    this.Changed = false;

                }
            }

            catch (Exception e)
            {
                throw new SpreadsheetReadWriteException(e.Message);
            }
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
        protected override IList<string> SetCellContents(string name, double number)
        {
            //if (!nameValidation(name))
            //{
            //    throw new InvalidNameException();
            //}


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
            this.Changed = true;
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
        protected override IList<string> SetCellContents(string name, string text)
        {

            //if (text is null)
            //{
            //    throw new ArgumentNullException();
            //}

            //if (!nameValidation(name))
            //{
            //    throw new InvalidNameException();
            //}

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
            this.Changed = true;
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
        protected override IList<string> SetCellContents(string name, Formula formula)
        {

            //if (formula is null)
            //{
            //    throw new ArgumentNullException();
            //}

            //if (!variableValidator(name))
            //{
            //    throw new InvalidNameException();
            //}

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
                this.Changed = true;
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
                this.Changed = false;
                throw new CircularException();
            }

        }


        public override IList<string> SetContentsOfCell(string name, string content)
        {
            if (content is null)
            {
                throw new ArgumentNullException();
            }

            if (!variableValidator(name))
            {
                throw new InvalidNameException();
            }

            String newName = Normalize(name);

            if (double.TryParse(content, out double num))
            {
                return SetCellContents(newName, num);
            }

            else if (content.Substring(0, 1).Equals("="))
            {
                String formula = content.Substring(1);
                Formula f = new Formula(formula);
                return SetCellContents(newName, f);
            }


            return SetCellContents(newName, content);

        }



        /// <summary>
        /// A protected method to get direct dependents.
        /// 
        /// </summary>
        /// <param name="name">The cell name</param>
        /// <returns>A list of dependents.</returns>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            if (!variableValidator(name))
            {
                throw new InvalidNameException();
            }
            string newName = Normalize(name);
            return graph.GetDependents(newName);
        }


        /// <summary>
        /// A private method to validiate the name of the cell. Returns true if 
        /// the name is valid. Otherwises return false. 
        /// 
        /// </summary>
        /// <param name="name">The cell name</param>
        /// <returns>true/false.</returns>
        //private bool nameValidation(string name)
        //{
        //    String varPattern = @"^[a-zA-Z_](?: [a-zA-Z_]|\d)*$";
        //    Regex regVar = new Regex(varPattern, RegexOptions.IgnorePatternWhitespace);
        //    if (name is null || !regVar.IsMatch(name))
        //    {
        //        return false;
        //    }
        //    return true;
        //}

        private bool variableValidator(string variable)
        {
            // ^[a-zA-Z]+\d+$
            Regex regex = new Regex(@"^[a-zA-Z]+\d+$");
            if (variable is null || !regex.IsMatch(variable) || !IsValid(variable) || !IsValid(Normalize(variable)))
            {
                return false;
            }
            return true;
        }

        private double lookup(string name)
        {
            if (spreadsheet[name].getContent() is double)
            {
                return (double)spreadsheet[name].getContent();
            }

            else if (spreadsheet[name].getContent() is Formula)
            {
                Formula f = (Formula)spreadsheet[name].getContent();
                var value = f.Evaluate(lookup);
                if (value is double)
                {
                    return (double)value;
                }
                else
                {
                    throw new ArgumentException("invalid.");
                }
            }

            else throw new ArgumentException("invalid.");

        }

        private object returnValue(string name)
        {
            if (spreadsheet[name].getContent() is string || spreadsheet[name].getContent() is double)
            {
                return spreadsheet[name].getContent();
            }
            Formula f = (Formula)spreadsheet[name].getContent();
            return f.Evaluate(lookup);
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
