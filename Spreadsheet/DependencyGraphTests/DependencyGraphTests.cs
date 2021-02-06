using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;


namespace DevelopmentTests
{
    /// <summary>
    ///This is a test class for DependencyGraphTest and is intended
    ///to contain all DependencyGraphTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DependencyGraphTest
    {

        /// <summary>
        ///Empty graph should contain nothing
        ///</summary>
        [TestMethod()]
        public void SimpleEmptyTest()
        {
            DependencyGraph t = new DependencyGraph();
            Assert.AreEqual(0, t.Size);
        }


        /// <summary>
        ///Empty graph should contain nothing
        ///</summary>
        [TestMethod()]
        public void SimpleEmptyRemoveTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "y");
            Assert.AreEqual(1, t.Size);
            t.RemoveDependency("x", "y");
            Assert.AreEqual(0, t.Size);
        }


        /// <summary>
        ///Empty graph should contain nothing
        ///</summary>
        [TestMethod()]
        public void EmptyEnumeratorTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "y");
            IEnumerator<string> e1 = t.GetDependees("y").GetEnumerator();
            Assert.IsTrue(e1.MoveNext());
            Assert.AreEqual("x", e1.Current);
            IEnumerator<string> e2 = t.GetDependents("x").GetEnumerator();
            Assert.IsTrue(e2.MoveNext());
            Assert.AreEqual("y", e2.Current);
            t.RemoveDependency("x", "y");
            Assert.IsFalse(t.GetDependees("y").GetEnumerator().MoveNext());
            Assert.IsFalse(t.GetDependents("x").GetEnumerator().MoveNext());
        }


        /// <summary>
        ///Replace on an empty DG shouldn't fail
        ///</summary>
        [TestMethod()]
        public void SimpleReplaceTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "y");
            Assert.AreEqual(t.Size, 1);
            t.RemoveDependency("x", "y");
            t.ReplaceDependents("x", new HashSet<string>());
            t.ReplaceDependees("y", new HashSet<string>());
        }



        ///<summary>
        ///It should be possibe to have more than one DG at a time.
        ///</summary>
        [TestMethod()]
        public void StaticTest()
        {
            DependencyGraph t1 = new DependencyGraph();
            DependencyGraph t2 = new DependencyGraph();
            t1.AddDependency("x", "y");
            Assert.AreEqual(1, t1.Size);
            Assert.AreEqual(0, t2.Size);
        }




        /// <summary>
        ///Non-empty graph contains something
        ///</summary>
        [TestMethod()]
        public void SizeTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("a", "c");
            t.AddDependency("c", "b");
            t.AddDependency("b", "d");
            Assert.AreEqual(4, t.Size);
        }


        /// <summary>
        ///Non-empty graph contains something
        ///</summary>
        [TestMethod()]
        public void EnumeratorTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("a", "c");
            t.AddDependency("c", "b");
            t.AddDependency("b", "d");

            IEnumerator<string> e = t.GetDependees("a").GetEnumerator();
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("b").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            String s1 = e.Current;
            Assert.IsTrue(e.MoveNext());
            String s2 = e.Current;
            Assert.IsFalse(e.MoveNext());
            Assert.IsTrue(((s1 == "a") && (s2 == "c")) || ((s1 == "c") && (s2 == "a")));

            e = t.GetDependees("c").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("a", e.Current);
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("d").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("b", e.Current);
            Assert.IsFalse(e.MoveNext());
        }




        /// <summary>
        ///Non-empty graph contains something
        ///</summary>
        [TestMethod()]
        public void ReplaceThenEnumerate()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "b");
            t.AddDependency("a", "z");
            t.ReplaceDependents("b", new HashSet<string>());
            t.AddDependency("y", "b");
            t.ReplaceDependents("a", new HashSet<string>() { "c" });
            t.AddDependency("w", "d");
            t.ReplaceDependees("b", new HashSet<string>() { "a", "c" });
            t.ReplaceDependees("d", new HashSet<string>() { "b" });

            IEnumerator<string> e = t.GetDependees("a").GetEnumerator();
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("b").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            String s1 = e.Current;
            Assert.IsTrue(e.MoveNext());
            String s2 = e.Current;
            Assert.IsFalse(e.MoveNext());
            Assert.IsTrue(((s1 == "a") && (s2 == "c")) || ((s1 == "c") && (s2 == "a")));

            e = t.GetDependees("c").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("a", e.Current);
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("d").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("b", e.Current);
            Assert.IsFalse(e.MoveNext());
        }



        /// <summary>
        ///Using lots of data
        ///</summary>
        [TestMethod()]
        public void StressTest()
        {
            // Dependency graph
            DependencyGraph t = new DependencyGraph();

            // A bunch of strings to use
            const int SIZE = 200;
            string[] letters = new string[SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                letters[i] = ("" + (char)('a' + i));
            }

            // The correct answers
            HashSet<string>[] dents = new HashSet<string>[SIZE];
            HashSet<string>[] dees = new HashSet<string>[SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                dents[i] = new HashSet<string>();
                dees[i] = new HashSet<string>();
            }

            // Add a bunch of dependencies
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 1; j < SIZE; j++)
                {
                    t.AddDependency(letters[i], letters[j]);
                    dents[i].Add(letters[j]);
                    dees[j].Add(letters[i]);
                }
            }

            // Remove a bunch of dependencies
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 4; j < SIZE; j += 4)
                {
                    t.RemoveDependency(letters[i], letters[j]);
                    dents[i].Remove(letters[j]);
                    dees[j].Remove(letters[i]);
                }
            }

            // Add some back
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 1; j < SIZE; j += 2)
                {
                    t.AddDependency(letters[i], letters[j]);
                    dents[i].Add(letters[j]);
                    dees[j].Add(letters[i]);
                }
            }

            // Remove some more
            for (int i = 0; i < SIZE; i += 2)
            {
                for (int j = i + 3; j < SIZE; j += 3)
                {
                    t.RemoveDependency(letters[i], letters[j]);
                    dents[i].Remove(letters[j]);
                    dees[j].Remove(letters[i]);
                }
            }

            // Make sure everything is right
            for (int i = 0; i < SIZE; i++)
            {
                Assert.IsTrue(dents[i].SetEquals(new HashSet<string>(t.GetDependents(letters[i]))));
                Assert.IsTrue(dees[i].SetEquals(new HashSet<string>(t.GetDependees(letters[i]))));
            }
        }


        [TestMethod()]
        public void _SimpleAddTest()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("a", "b");
            dg.AddDependency("b", "c");
            Assert.AreEqual(2, dg.Size);
        }

        [TestMethod()]
        public void _SizeOfDependees()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("b", "a");
            dg.AddDependency("c", "a");
            dg.AddDependency("d", "a");
            dg.AddDependency("b", "a");
            dg.AddDependency("a", "z");
            Assert.AreEqual(4, dg.Size);
            Assert.AreEqual(3, dg["a"]);
            Assert.AreEqual(0, dg["b"]);
        }

        [TestMethod()]
        public void _HasDependents()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("a", "b");
            Assert.IsTrue(dg.HasDependents("a"));
            Assert.IsFalse(dg.HasDependents("b"));
            Assert.IsFalse(dg.HasDependents("c"));
        }


        [TestMethod()]
        public void _HasDependees()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("a", "b");
            Assert.IsTrue(dg.HasDependees("b"));
            Assert.IsFalse(dg.HasDependees("a"));
            Assert.IsFalse(dg.HasDependees("c"));
        }

        [TestMethod()]
        public void _CheckDependency()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("a", "b");
            dg.AddDependency("b", "a");
            Assert.AreEqual(2, dg.Size);
            Assert.IsTrue(dg.HasDependents("a"));
            Assert.IsTrue(dg.HasDependents("b"));
            Assert.IsTrue(dg.HasDependees("a"));
            Assert.IsTrue(dg.HasDependees("b"));
        }

        [TestMethod()]
        public void _CheckAddToSeeRunAllCodeLines()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("a", "b");
            dg.AddDependency("a", "c");
            dg.AddDependency("a", "b");
            dg.AddDependency("c", "b");
            Assert.AreEqual(3, dg.Size);
            dg.RemoveDependency("a", "b");
            dg.RemoveDependency("z", "b");
            Assert.AreEqual(2, dg.Size);
        }

        [TestMethod()]
        public void _CheckAddToSeeRunAllCodeLines2()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("a", "b");
            dg.AddDependency("c", "d");
            Assert.AreEqual(2, dg.Size);
            dg.AddDependency("a", "c");
            Assert.AreEqual(3, dg.Size);
            dg.AddDependency("z", "d");
            Assert.AreEqual(4, dg.Size);
        }

        [TestMethod()]
        public void _CheckAddToSeeRunAllCodeLines3()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("a", "b");
            dg.AddDependency("c", "b");
            Assert.AreEqual(2, dg.Size);

        }

        [TestMethod()]
        public void _CheckAddToSeeRunAllCodeLines4()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("a", "b");
            dg.AddDependency("c", "b");
            Assert.AreEqual(2, dg.Size);

        }

        [TestMethod()]
        public void LoopDependency()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("b", "a");
            Assert.AreEqual(2, t.Size);
            IEnumerator<string> e1 = t.GetDependents("a").GetEnumerator();
            IEnumerator<string> e2 = t.GetDependees("a").GetEnumerator();
            Assert.IsTrue(e1.MoveNext());
            Assert.IsTrue(e2.MoveNext());
            Assert.AreEqual("b", e1.Current);
            Assert.AreEqual("b", e1.Current);
            Assert.IsFalse(e1.MoveNext());
            Assert.IsFalse(e2.MoveNext());
            IEnumerator<string> e3 = t.GetDependents("b").GetEnumerator();
            IEnumerator<string> e4 = t.GetDependees("b").GetEnumerator();
            Assert.IsTrue(e3.MoveNext());
            Assert.IsTrue(e4.MoveNext());
            Assert.AreEqual("a", e3.Current);
            Assert.AreEqual("a", e4.Current);
            Assert.IsFalse(e3.MoveNext());
            Assert.IsFalse(e4.MoveNext());
        }

        [TestMethod()]
        public void _RemoveDependency()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("a", "c");
            Assert.AreEqual(2, t.Size);
            t.RemoveDependency("b", "a");
            Assert.AreEqual(2, t.Size);
            Assert.IsTrue(t.HasDependents("a"));
            Assert.IsTrue(t.HasDependees("b"));
            t.RemoveDependency("a", "c");
            Assert.IsFalse(t.HasDependees("c"));

        }

        [TestMethod()]
        public void _GetDependentsTest()
        {
            DependencyGraph dg = new DependencyGraph();
            for (int i = 0; i < 10; i++)
            {
                dg.AddDependency("a", i.ToString());
            }
            Assert.AreEqual(10, dg.Size);
            IEnumerator<string> e1 = dg.GetDependents("a").GetEnumerator();
            for (int i = 0; i < 10; i++)
            {
                Assert.IsTrue(e1.MoveNext());
                Assert.AreEqual(i.ToString(), e1.Current);
            }
            dg.AddDependency("a", "9");
            Assert.AreNotEqual(11, dg.Size);
            Assert.IsFalse(e1.MoveNext());
        }

        [TestMethod()]
        public void _GetDependeesTest()
        {
            DependencyGraph dg = new DependencyGraph();
            for (int i = 0; i < 10; i++)
            {
                dg.AddDependency(i.ToString(),"a");
            }
            Assert.AreEqual(10, dg.Size);
            IEnumerator<string> e1 = dg.GetDependees("a").GetEnumerator();
            for (int i = 0; i < 10; i++)
            {
                Assert.IsTrue(e1.MoveNext());
                Assert.AreEqual(i.ToString(), e1.Current);
            }
            dg.AddDependency( "9", "a");
            Assert.AreNotEqual(11, dg.Size);
            Assert.IsFalse(e1.MoveNext());
        }

        [TestMethod()]
        public void _ReplacementDependentsTest()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("b", "d");
            dg.ReplaceDependents("b", new HashSet<string>());
            Assert.IsFalse(dg.HasDependents("b"));
            dg.AddDependency("b", "c");
            Assert.IsTrue(dg.HasDependents("b"));
            IEnumerator<string> e = dg.GetDependents("b").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            String s1 = e.Current;
            Assert.IsFalse(e.MoveNext());
            Assert.IsTrue(s1 == "c");

            dg.ReplaceDependents("b", new HashSet<string> { "d", "e", "f", "g" });
            IEnumerator<string> e2 = dg.GetDependents("b").GetEnumerator();
            Assert.IsTrue(e2.MoveNext());
            String s2 = e2.Current;
            Assert.IsTrue(e2.MoveNext());
            String s3 = e2.Current;
            Assert.IsTrue(e2.MoveNext());
            String s4 = e2.Current;
            Assert.IsTrue(e2.MoveNext());
            String s5 = e2.Current;
            Assert.IsFalse(e2.MoveNext());
            Assert.IsTrue(s2 == "d" && s3 == "e" && s4 == "f" && s5 == "g");

            dg.AddDependency("b", "d");
            Assert.IsFalse(e2.MoveNext());

        }

        [TestMethod()]
        public void _ReplacementDependeesTest()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("a", "b");
            dg.AddDependency("b", "c");
            dg.ReplaceDependees("b", new HashSet<string>());
            Assert.IsTrue(dg.HasDependents("b"));
            Assert.IsFalse(dg.HasDependees("b"));
            dg.AddDependency("1", "b");
            Assert.IsTrue(dg.HasDependees("b"));
            IEnumerator<string> e = dg.GetDependees("b").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            String s1 = e.Current;
            Assert.IsFalse(e.MoveNext());
            Assert.IsTrue(s1 == "1");

            dg.ReplaceDependees("b", new HashSet<string> { "2", "3", "4", "5" });
            IEnumerator<string> e2 = dg.GetDependees("b").GetEnumerator();
            Assert.IsTrue(e2.MoveNext());
            String s2 = e2.Current;
            Assert.IsTrue(e2.MoveNext());
            String s3 = e2.Current;
            Assert.IsTrue(e2.MoveNext());
            String s4 = e2.Current;
            Assert.IsTrue(e2.MoveNext());
            String s5 = e2.Current;
            Assert.IsFalse(e2.MoveNext());
            Assert.IsTrue(s2 == "2" && s3 == "3" && s4 == "4" && s5 == "5");

        }

        [TestMethod()]
        public void _DependentOnItself()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("b", "b");
            IEnumerator<string> e1 = dg.GetDependents("b").GetEnumerator();
            IEnumerator<string> e2 = dg.GetDependees("b").GetEnumerator();
            Assert.IsTrue(e1.MoveNext());
            String s1 = e1.Current;
            Assert.IsFalse(e1.MoveNext());
            Assert.IsTrue(e2.MoveNext());
            String s2 = e2.Current;
            Assert.IsFalse(e2.MoveNext());
            Assert.AreEqual(s1, s2);
            Assert.IsTrue(dg.HasDependents("b"));
            Assert.IsTrue(dg.HasDependees("b"));
        }



    }
}