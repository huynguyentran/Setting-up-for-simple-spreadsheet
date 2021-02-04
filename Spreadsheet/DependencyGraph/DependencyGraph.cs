// Skeleton implementation written by Joe Zachary for CS 3500, September 2013.
// Version 1.1 (Fixed error in comment for RemoveDependency.)
// Version 1.2 - Daniel Kopta 
//               (Clarified meaning of dependent and dependee.)
//               (Clarified names in solution/project structure.)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpreadsheetUtilities
{

    /// <summary>
    /// (s1,t1) is an ordered pair of strings
    /// t1 depends on s1; s1 must be evaluated before t1
    /// 
    /// A DependencyGraph can be modeled as a set of ordered pairs of strings.  Two ordered pairs
    /// (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 equals t2.
    /// Recall that sets never contain duplicates.  If an attempt is made to add an element to a 
    /// set, and the element is already in the set, the set remains unchanged.
    /// 
    /// Given a DependencyGraph DG:
    /// 
    ///    (1) If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
    ///        (The set of things that depend on s)    
    ///        
    ///    (2) If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
    ///        (The set of things that s depends on) 
    //
    // For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    //     dependents("a") = {"b", "c"}
    //     dependents("b") = {"d"}
    //     dependents("c") = {}
    //     dependents("d") = {"d"}
    //     dependees("a") = {}
    //     dependees("b") = {"a"}
    //     dependees("c") = {"a"}
    //     dependees("d") = {"b", "d"}
    /// </summary>
    public class DependencyGraph
    {
        // some functiosn dictiaonry 

        //private List<Dictionary<string, List<string>>> dg;
        private Dictionary<string, List<string>> dependents;
        private Dictionary<string, List<string>> dependees;
        private int numberOfPair;

        /// <summary>
        /// Creates an empty DependencyGraph.
        /// </summary>
        public DependencyGraph()
        {
            //dg = new List<Dictionary<string, List<string>>>();
            dependents = new Dictionary<string, List<string>>();
            dependees = new Dictionary<string, List<string>>();
            this.numberOfPair = 0;
        }


        /// <summary>
        /// The number of ordered pairs in the DependencyGraph.
        /// </summary>
        public int Size
        {
            get
            {
                return this.numberOfPair;
            }
        }


        /// <summary>
        /// The size of dependees(s).
        /// This property is an example of an indexer.  If dg is a DependencyGraph, you would
        /// invoke it like this:
        /// dg["a"]
        /// It should return the size of dependees("a")
        /// </summary>
        public int this[string s]
        {
            get { return dependees[s].Count; }
        }


        /// <summary>
        /// Reports whether dependents(s) is non-empty.
        /// </summary>
        public bool HasDependents(string s)
        {
            if (dependents[s].Count == 0)
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// Reports whether dependees(s) is non-empty.
        /// </summary>
        public bool HasDependees(string s)
        {
 
            if (dependees[s].Count == 0)
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// Enumerates dependents(s).
        /// </summary>
        public IEnumerable<string> GetDependents(string s)
        {

            if (dependents[s].Count == 0)
            {
                return new List<string>();
            }
            //This is temporary.
            return new List<string>(dependents[s]);
        }

        /// <summary>
        /// Enumerates dependees(s).
        /// </summary>
        public IEnumerable<string> GetDependees(string s)
        {
           
            if (dependees[s].Count == 0)
            {
                return new List<string>();
            }
            return new List<String>(dependees[s]);
        }


        /// <summary>
        /// <para>Adds the ordered pair (s,t), if it doesn't exist</para>
        /// 
        /// <para>This should be thought of as:</para>   
        /// 
        ///   t depends on s
        ///
        /// </summary>
        /// <param name="s"> s must be evaluated first. T depends on S</param>
        /// <param name="t"> t cannot be evaluated until s is</param>        /// 
        public void AddDependency(string s, string t)
        {
       
             //use set ( ordered, reverse relationship is fine. HashSet)

            if (!dependents.ContainsKey(s)) // Checking for the key 
            {
                //If do not have any depedents, creates a new list
                List<string> newDependents = new List<string>();
                newDependents.Add(t);
                dependents.Add(s, newDependents);
            }
            else
            {
                dependents[s].Add(t);
            }

            if (!dependees.ContainsKey(t))
            {
                List<string> newDependees = new List<string>();
                newDependees.Add(s);
                dependees.Add(t, newDependees);
            }
            else
            {
                dependees[t].Add(s);
            }
            this.numberOfPair++;

        }


        /// <summary>
        /// Removes the ordered pair (s,t), if it exists
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        public void RemoveDependency(string s, string t)
        {
            dependees[t].Remove(s);
            dependents[s].Remove(t);
            this.numberOfPair--;
            //If containsKey("s")
        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (s,r).  Then, for each
        /// t in newDependents, adds the ordered pair (s,t).
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        //use remove then add

        {
            //for each depenencee in s 
            // Remove 
            //foreach (string str in newDependets)
            // AdddDependency(s, str)



        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (r,s).  Then, for each 
        /// t in newDependees, adds the ordered pair (t,s).
        /// </summary>
        public void ReplaceDependees(string s, IEnumerable<string> newDependees)
        {

            //Remove first
            //foreach (string str in newDependees)
            // AdddDependency(str, s)
        }

    }

}
