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
        // Initiating dependents and dependees dictionray. 
        private Dictionary<string, HashSet<string>> dependents;
        private Dictionary<string, HashSet<string>> dependees;
        private int numOfPairs;

        /// <summary>
        /// Creates an empty DependencyGraph.
        /// </summary>
        public DependencyGraph()
        {
            dependents = new Dictionary<string, HashSet<string>>();
            dependees = new Dictionary<string, HashSet<string>>();
            this.numOfPairs = 0;
        }


        /// <summary>
        /// The number of ordered pairs in the DependencyGraph.
        /// </summary>
        public int Size
        {
            get
            {
                return this.numOfPairs;
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
            get { 
                //If dependees list does not contains s, return 0.
                if (!dependees.ContainsKey(s))
                {
                    return 0;
                }
                return dependees[s].Count; 
            }
        }


        /// <summary>
        /// Reports whether dependents(s) is non-empty.
        /// </summary>
        public bool HasDependents(string s)
        {
            //Using the private method to check the condition. 
            if (checkDependents(s))
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
            //Using the private method to check the condition.
            if (checkDependees(s))
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
            //If the string s does not have any dependents, return an empty HashSet.
            if (!HasDependents(s))
            {
                return new HashSet<string>();
            }
            //This is temporary.
            return new HashSet<string>(dependents[s]);
        }

        /// <summary>
        /// Enumerates dependees(s).
        /// </summary>
        public IEnumerable<string> GetDependees(string s)
        {
            //If the string s does not have any dependees, return an empty HashSet.
            if (!HasDependees(s))
            {
                return new HashSet<string>();
            }
            return new HashSet<String>(dependees[s]);
        }



        /// <summary>
        /// <para>Adds the ordered pair (s,t), if it doesn't exist</para>
        /// 
        /// <para>This should be thought of as:</para>   
        /// 
        ///   t depends on s 
        ///
        ///   
        /// 
        /// </summary>
        /// <param name="s"> s must be evaluated first. T depends on S</param>
        /// <param name="t"> t cannot be evaluated until s is</param>        /// 
        public void AddDependency(string s, string t)
        {        
            //Checking if the dependency graph already has s and t
            if (dependents.ContainsKey(s) && dependees.ContainsKey(t))
            {
                //If either the new dependency is unique, increases the number of pairs.Otherwises, return. 
                if(dependents[s].Add(t) || dependees[t].Add(s))
                {
                    dependents[s].Add(t);
                    dependees[t].Add(s);
                    this.numOfPairs++;
                    return;
                }
                return;
            }

            //Checking if dependency graph already has t
            if (dependees.ContainsKey(t) && !dependents.ContainsKey(s))
            {
                dependees[t].Add(s);
                //Calling a private method to insert a new s
                createNewDependents(s, t);
                this.numOfPairs++;
                return;
            }

            //Checking if dependency graph already has s
            if (dependents.ContainsKey(s) && !dependees.ContainsKey(t))
            {
                dependents[s].Add(t);
                //Calling a private method to insert a new t
                createNewDependees(s, t);
                this.numOfPairs++;
                return;


            }

            //Else, creates both new elements into the dependency graph. 
            createNewDependents(s, t);
            createNewDependees(s, t);
            this.numOfPairs++;
            return;
        }


        /// <summary>
        /// Removes the ordered pair (s,t), if it exists
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        public void RemoveDependency(string s, string t)
        {
            //If the dependency graph has valid s->t pair
            if (HasDependents(s) && HasDependees(t))
            {
                dependees[t].Remove(s);
                dependents[s].Remove(t);
                this.numOfPairs--;
            }
        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (s,r).  Then, for each
        /// t in newDependents, adds the ordered pair (s,t).
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            //If s has depedents, then we remove all elements in it and then insert. Else, we need to create a new depedents list for s
            if (HasDependents(s))
            {
                foreach (string t in new HashSet<string>(dependents[s]))
                {
                    RemoveDependency(s, t);
                }
            }
 
            foreach (string t in newDependents)
            {
                AddDependency(s, t);
            }
        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (r,s).  Then, for each 
        /// t in newDependees, adds the ordered pair (t,s).
        /// </summary>
        public void ReplaceDependees(string s, IEnumerable<string> newDependees)
        {
            //If t has depedees, then we remove all elements in it and then insert. Else, we need to create a new depedees list for t
            if (HasDependees(s))
            {
                foreach (string t in new HashSet<string>(dependees[s]))
                {
                    RemoveDependency(t, s);
                }
            }
          
            foreach (string t in newDependees)
            {
                AddDependency(t, s);
            }
        }

        /// <summary>
        /// A private method to check if whether s has dependents or not.
        /// Return true if dependents list does not contain s or s does not have
        /// any dependents. 
        /// </summary>
        private bool checkDependents(string s)
        {
          if (!dependents.ContainsKey(s) || dependents[s].Count == 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// A private method to check if whether s has dependees or not.
        /// Return true if dependees list does not contain s or s does not have
        /// any dependees. 
        /// </summary>
        private bool checkDependees(string s)
        {
            if (!dependees.ContainsKey(s) || dependees[s].Count == 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// A private method to add into the dependents dictionary key s.
        /// This key s has value of a HashSet that contains t
        /// </summary>
        private void createNewDependents(string s, string t)
        {
            HashSet<string> newDependents = new HashSet<string>();
            newDependents.Add(t);
            dependents.Add(s, newDependents);
        }

        /// <summary>
        /// A private method to add into the dependees dictionary key t.
        /// This key t has value of a HashSet that contains s
        /// </summary>
        private void createNewDependees(string s, string t)
        {
            HashSet<string> newDependees = new HashSet<string>();
            newDependees.Add(s);
            dependees.Add(t, newDependees);
        }


    }

}
