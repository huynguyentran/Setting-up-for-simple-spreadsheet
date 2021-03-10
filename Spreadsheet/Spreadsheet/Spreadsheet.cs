// Huy Nguyen (u1315096) 
// 2/16/2021
// PS5 3500

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
            this.Changed = false;
        }


        /// <summary>
        /// A  constructor that creates an empty spreadsheet that takes in 
        /// isValid func, normalize func, and a string to indicates version.
        /// 
        /// </summary>
        /// <param name="isValid">The function to validate the variables</param>
        /// <param name="normalize">The function to normalize the variables</param>
        /// <param name="version">The strings indicates the spreadsheet version</param>
        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version)
        {
            spreadsheet = new Dictionary<string, Cell>();
            graph = new DependencyGraph();
            this.Changed = false;
        }

        /// <summary>
        /// A  constructor that creates an empty spreadsheet that takes in 
        /// a string filepath, isValid func, normalize func, and a string 
        /// to indicates version.
        /// 
        /// </summary>
        /// <param name="isValid">The function to validate the variables</param>
        /// <param name="normalize">The function to normalize the variables</param>
        /// <param name="version">The strings indicates the spreadsheet version</param>
        /// <param name="filePath">The strings indicates the file path</param>
        public Spreadsheet(string filePath, Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version)
        {
            spreadsheet = new Dictionary<string, Cell>();
            graph = new DependencyGraph();
            try
            {
                using (XmlReader reader = XmlReader.Create(filePath))
                {
                    String name = "";
                    String content = "";
                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            switch (reader.Name)
                            {
                                case "spreadsheet":
                                    if (!reader["version"].Equals(version))
                                    {
                                        throw new SpreadsheetReadWriteException("Not the same version");
                                    }
                                    this.Version = reader["version"];
                                    break;
                                case "cell":
                                    break;
                                case "name":
                                    reader.Read();
                                    name = reader.Value;
                                    break;
                                case "contents":
                                    reader.Read();
                                    content = reader.Value;
                                    SetContentsOfCell(name, content);
                                    break;
                            }
                        }

                    }
                }
                this.Changed = false;
            }
            catch (Exception e)
            {
                throw new SpreadsheetReadWriteException(e.Message);
            }

        }


        /// <summary>
        /// Indicating whether the spreadsheet has been changed or not.
        /// 
        /// </summary>
        public override bool Changed
        {
            get; protected set;
        }

        /// <summary>
        /// A method that returns the content of the cell. The content can be
        /// a Formula, a string, or a double. If the cell does not have any
        /// content, or the cell doesn't exist yet in the spreadsheet, returns 
        /// an empty string.
        /// 
        /// </summary>
        /// <param name="name">The string cell name</param>
        /// <throw> InvalidNameException() if the name is not valid.</throw>
        /// <returns>the content of the cell(obj).</returns>
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

        /// <summary>
        /// A method that returns the value of the cell. The content can be
        /// a FormulaError, a string, or a double. If the cell does not have any
        /// value, or the cell doesn't exist yet in the spreadsheet, returns 
        /// an empty string.
        /// 
        /// </summary>
        /// <param name="name">The string cell name</param>
        /// <throw> InvalidNameException() if the name is not valid.</throw>
        /// <returns>the value of the cell(obj).</returns>
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
            return spreadsheet[newName].getValue();
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
        /// A method that returns the version of the spreadsheet from XML.
        /// 
        /// </summary>
        /// <param name="filename">The filepath</param>
        /// <throw> SpreadsheetReadWriteException() if there are error when finding the version.</throw>
        /// <returns>the version string.</returns>
        public override string GetSavedVersion(string filename)
        {
            try
            {
                using (XmlReader reader = XmlReader.Create(filename))
                {
                    while (reader.Read())
                    {
                        if (!(reader["version"] is null) && reader.Name.Equals("spreadsheet") )
                        {
                            return reader["version"];
                        }
                    }
                    throw new SpreadsheetReadWriteException("Can not find version information");
                }
            }
            catch (Exception e)
            {
                throw new SpreadsheetReadWriteException(e.Message);
            }
        }

        /// <summary>
        /// A method that saves the spreadsheet information into a XML
        /// 
        /// </summary>
        /// <param name="filename">The filepath</param>
        /// <throw> SpreadsheetReadWriteException() if there are error when saving the spreadsheet.</throw>
        public override void Save(string filename)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  ";
            try
            {
                using (XmlWriter writer = XmlWriter.Create(filename, settings))
                {

                    writer.WriteStartDocument();
                    writer.WriteStartElement("spreadsheet");
                    writer.WriteAttributeString("version", this.Version);
                    foreach (KeyValuePair<string, Cell> entry in spreadsheet)
                    {
                        writer.WriteStartElement("cell");
                        writer.WriteElementString("name", entry.Key);

                        if (entry.Value.getContent() is string)
                        {
                            writer.WriteElementString("contents", (string)entry.Value.getContent());
                        }
                        else if (entry.Value.getContent() is double)
                        {
                            writer.WriteElementString("contents", entry.Value.getContent().ToString());
                        }
                        else
                        {
                            writer.WriteElementString("contents", "=" + entry.Value.getContent().ToString());
                        }
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
                this.Changed = true;
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
        /// A method that sets the conent of the named Cell to a Formula, a string 
        /// or a double. The content is depended by the structure of the parameter
        /// string content.
        /// The method returns a list consisting of name plus the names of 
        /// all other cells whose value depends, directly or indirectly, on 
        /// the named cell. The method also updates the values of all dependents 
        /// of the name cell.
        /// 
        /// </summary>
        /// <param name="name">The string cell name</param>
        /// <param name="Formula">The Formula content</param>
        /// <throw> ArgumentNullException() if the text is null.</throw>
        /// <throw> CircularException() if the Formula creates Circular.</throw>
        /// <returns>A list of name that depended directly and indirectly on the named Cell.</retu
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
            List<string> list = null;

            if (double.TryParse(content, out double num))
            {
                list = new List<string>(SetCellContents(newName, num));
            }

            else if (content.Length > 0 && content.Substring(0, 1).Equals("="))
            {
                String formula = content.Substring(1);
                Formula f = new Formula(formula, this.Normalize, this.IsValid);
                list = new List<string>(SetCellContents(newName, f));
            }

            else
            {
                list = new List<string>(SetCellContents(newName, content));
            }

            foreach (string str in list)
            {
                if (!spreadsheet.ContainsKey(str))
                {
                    continue;
                }
                else if (spreadsheet[str].getContent() is Formula f)
                {
                    spreadsheet[str].setValue(f.Evaluate(lookup));
                }
                else
                {
                    spreadsheet[str].setValue(spreadsheet[str].getContent());
                }
            }
            return list;

        }

        /// <summary>
        /// A protected method to get direct dependents.
        /// 
        /// </summary>
        /// <param name="name">The cell name</param>
        /// <returns>A list of dependents.</returns>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
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
        private bool variableValidator(string variable)
        {
            Regex regex = new Regex(@"^[a-zA-Z]+\d+$");
            if (variable is null || !regex.IsMatch(variable) || !IsValid(variable) || !IsValid(Normalize(variable)))
            {
                return false;
            }
            if (Normalize(variable) is null || !regex.IsMatch(Normalize(variable)))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// A private method to look up the value of the cell.
        /// 
        /// </summary>
        /// <param name="name">The cell name</param>
        /// <returns>The value of cell if it is double.</returns>
        /// <throw> ArgumentException if the cell doesn't contain a double.</throw>
        private double lookup(string name)
        {
            if (spreadsheet.ContainsKey(name) && spreadsheet[name].getValue() is double)
            {
                return (double)spreadsheet[name].getValue();
            }
            else throw new ArgumentException("invalid.");
        }

    }

    /// <summary>
    /// A class represented a Cell that holds a content object and a value 
    /// 
    /// </summary>
    class Cell
    {

        private object content;
        private object value;


        /// <summary>
        /// A constructor to initializing the cell with content(Formula)
        /// 
        /// </summary>
        /// <param name="_content">The object content(Formula)</param>
        public Cell(Formula _content)
        {
            content = _content;
        }

        /// <summary>
        /// A constructor to initializing the cell with content(string)
        /// 
        /// </summary>
        /// <param name="_content">The object content(string)</param>
        public Cell(string _content)
        {
            content = _content;
        }

        /// <summary>
        /// A constructor to initializing the cell with content(double)
        /// 
        /// </summary>
        /// <param name="_content">The object content(double)</param>
        public Cell(double _content)
        {
            content = _content;
        }

        /// <summary>
        /// A method that would return the content of the cell.
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
        /// <param name="obj">The object content(object)</param>
        public void setContent(object obj)
        {
            content = obj;
        }

        /// <summary>
        /// A method to change the vale of the cell.
        /// 
        /// </summary>
        /// <param name="obj">The object content(object)</param>
        public void setValue(object obj)
        {
            value = obj;
        }

        /// <summary>
        /// A method that would return the value of the cell.
        /// 
        /// </summary>
        /// <returns>The content of the cell.</returns>
        public object getValue()
        {
            return value;
        }
    }

}
