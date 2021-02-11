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

        /// <summary>
        /// A test to see if the add method and size method work properly.
        /// </summary>
        [TestMethod()]
        public void _SimpleAddTest()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("a", "b");
            dg.AddDependency("b", "c");
            Assert.AreEqual(2, dg.Size);
        }

        /// <summary>
        /// A test to see if the size of dependees.
        /// </summary>
        [TestMethod()]
        public void _SizeOfDependees()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("b", "a");
            dg.AddDependency("c", "a");
            dg.AddDependency("d", "a");
            //Repeat should not increase the dependees list. 
            dg.AddDependency("b", "a");
            dg.AddDependency("a", "z");
            Assert.AreEqual(4, dg.Size);
            Assert.AreEqual(3, dg["a"]);
            Assert.AreEqual(0, dg["b"]);
        }

        /// <summary>
        /// A test to check HasDependents method. 
        /// </summary>
        [TestMethod()]
        public void _HasDependents()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("a", "b");
            Assert.IsTrue(dg.HasDependents("a"));
            Assert.IsFalse(dg.HasDependents("b"));
            Assert.IsFalse(dg.HasDependents("c"));
        }

        /// <summary>
        /// A test to check HasDependees method. 
        /// </summary>
        [TestMethod()]
        public void _HasDependees()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("a", "b");
            Assert.IsTrue(dg.HasDependees("b"));
            Assert.IsFalse(dg.HasDependees("a"));
            Assert.IsFalse(dg.HasDependees("c"));
        }

        /// <summary>
        /// A test to check cycle dependency.
        /// </summary>
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
            Assert.AreEqual(1, dg["a"]);
            Assert.AreEqual(1, dg["b"]);
        }

        /// <summary>
        /// A series of tests to check all possible AddDependency
        /// </summary>
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

        /// <summary>
        /// A series of tests to check all possible AddDependency
        /// </summary>
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

        /// <summary>
        /// A series of tests to check all possible AddDependency
        /// </summary>
        [TestMethod()]
        public void _CheckAddToSeeRunAllCodeLines3()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("a", "b");
            dg.AddDependency("c", "b");
            Assert.AreEqual(2, dg.Size);

        }

        /// <summary>
        /// A series of tests to check all possible AddDependency
        /// </summary>
        [TestMethod()]
        public void _CheckAddToSeeRunAllCodeLines4()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("a", "b");
            dg.AddDependency("c", "b");
            Assert.AreEqual(2, dg.Size);

        }

        /// <summary>
        /// A more in depth test to check a cycle dependency.
        /// </summary>
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

        /// <summary>
        /// A test to check RemoveDependency method.
        /// </summary>
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
            Assert.IsTrue(t.HasDependees("c"));
            t.RemoveDependency("a", "c");
            Assert.IsFalse(t.HasDependees("c"));

        }

        /// <summary>
        /// A test to check GetDependents method to see if
        /// it returns the correct list.
        /// </summary>
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

        /// <summary>
        /// A test to check GetDependees method to see if
        /// it returns the correct list.
        /// </summary>
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


        /// <summary>
        /// A test to check ReplaceDependents method.
        /// </summary>
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

        /// <summary>
        /// A test to check a large dependees 
        /// </summary>
        [TestMethod()]
        public void _ReplacementLargeDependeesTest()
        {
            DependencyGraph dg = new DependencyGraph();
            for (int i = 0; i < 1000; i++)
            {
                dg.AddDependency(i.ToString(), "a");
            }
            Assert.AreEqual(1000, dg["a"]);
            IEnumerator<string> e = dg.GetDependees("a").GetEnumerator();
            for (int i = 0; i < 1000; i++)
            {
                Assert.IsTrue(e.MoveNext());
                String s = e.Current;
                Assert.IsTrue(s == i.ToString());
            }

            HashSet<string> set = new HashSet<string>();
            for (int i = 0; i < 1000; i++)
            {
                set.Add(i.ToString());
            }

            dg.ReplaceDependees("b", set);
            Assert.AreEqual(1000, dg["b"]);
            IEnumerator<string> e2 = dg.GetDependees("b").GetEnumerator();
            for (int i = 0; i < 1000; i++)
            {
                Assert.IsTrue(e2.MoveNext());
                String s = e2.Current;
                Assert.IsTrue(s == i.ToString());
            }
        }

        /// <summary>
        /// A test to check a large dependents 
        /// </summary>
        [TestMethod()]
        public void _ReplacementLargeDependentsTest()
        {
            DependencyGraph dg = new DependencyGraph();
            for (int i = 0; i < 1000; i++)
            {
                dg.AddDependency( "a",i.ToString());
            }
            Assert.AreEqual(0, dg["a"]);
            IEnumerator<string> e = dg.GetDependents("a").GetEnumerator();
            for (int i = 0; i < 1000; i++)
            {
                Assert.IsTrue(e.MoveNext());
                String s = e.Current;
                Assert.IsTrue(s == i.ToString());
            }

            HashSet<string> set = new HashSet<string>();
            for (int i = 0; i < 1000; i++)
            {
                set.Add(i.ToString());
            }

            dg.ReplaceDependents("b", set);
            Assert.AreEqual(0, dg["b"]);
            IEnumerator<string> e2 = dg.GetDependents("b").GetEnumerator();
            for (int i = 0; i < 1000; i++)
            {
                Assert.IsTrue(e2.MoveNext());
                String s = e2.Current;
                Assert.IsTrue(s == i.ToString());
            }
        }

        /// <summary>
        /// A test to check ReplaceDependees method.
        /// </summary>
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

        /// <summary>
        /// A test to check valid dependency on oneself
        /// </summary>
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

        /// <summary>
        /// A test to if remove dependency affect the dependee list in a corret way
        /// </summary>
        [TestMethod()]
        public void _EmptyDependeesList()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("a", "b");
            dg.RemoveDependency("a", "b");
            Assert.AreEqual(0, dg["b"]);
        
        }

        /// <summary>
        /// A test to check null element
        /// </summary>
        [TestMethod()]
        public void _addingNull()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("a", null);
            dg.AddDependency("b", null);
            dg.AddDependency(null, null);
            Assert.AreEqual(0, dg.Size);
        }


        /// <summary>
        ///This is a test class for DependencyGraphTest and is intended
        ///to contain all DependencyGraphTest Unit Tests
        ///</summary>
    

            // ************************** TESTS ON EMPTY DGs ************************* //

            /// <summary>
            ///Empty graph should contain nothing
            ///</summary>
            [TestMethod(), Timeout(2000)]
            [TestCategory("1")]
            public void TestZeroSize()
            {
                DependencyGraph t = new DependencyGraph();
                Assert.AreEqual(0, t.Size);
            }

            /// <summary>
            ///Empty graph should contain nothing
            ///</summary>
            [TestMethod(), Timeout(2000)]
            [TestCategory("2")]
            public void TestNoDepends()
            {
                DependencyGraph t = new DependencyGraph();
                Assert.IsFalse(t.HasDependees("x"));
                Assert.IsFalse(t.HasDependents("x"));
            }

            /// <summary>
            ///Empty graph should contain nothing
            ///</summary>
            [TestMethod(), Timeout(2000)]
            [TestCategory("3")]
            public void TestEmptyEnumerator()
            {
                DependencyGraph t = new DependencyGraph();
                Assert.IsFalse(t.GetDependees("x").GetEnumerator().MoveNext());
                Assert.IsFalse(t.GetDependents("x").GetEnumerator().MoveNext());
            }

            /// <summary>
            ///Empty graph should contain nothing
            ///</summary>
            [TestMethod(), Timeout(2000)]
            [TestCategory("4")]
            public void TestEmptyIndexer()
            {
                DependencyGraph t = new DependencyGraph();
                Assert.AreEqual(0, t["x"]);
            }

            /// <summary>
            ///Removing from an empty DG shouldn't fail
            ///</summary>
            [TestMethod(), Timeout(2000)]
            [TestCategory("5")]
            public void TestRemoveFromEmpty()
            {
                DependencyGraph t = new DependencyGraph();
                t.RemoveDependency("x", "y");
            }

            /// <summary>
            ///Replace on an empty DG shouldn't fail
            ///</summary>
            [TestMethod(), Timeout(2000)]
            [TestCategory("6")]
            public void TestReplaceEmpty()
            {
                DependencyGraph t = new DependencyGraph();
                t.ReplaceDependents("x", new HashSet<string>());
                t.ReplaceDependees("y", new HashSet<string>());
            }


            // ************************ MORE TESTS ON EMPTY DGs *********************** //

            /// <summary>
            ///Empty graph should contain nothing
            ///</summary>
            [TestMethod(), Timeout(2000)]
            [TestCategory("7")]
            public void TestAddRemoveEmpty()
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
            [TestMethod(), Timeout(2000)]
            [TestCategory("8")]
            public void TestAddRemoveEmpty2()
            {
                DependencyGraph t = new DependencyGraph();
                t.AddDependency("x", "y");
                Assert.IsTrue(t.HasDependees("y"));
                Assert.IsTrue(t.HasDependents("x"));
                t.RemoveDependency("x", "y");
                Assert.IsFalse(t.HasDependees("y"));
                Assert.IsFalse(t.HasDependents("x"));
            }

            /// <summary>
            ///Empty graph should contain nothing
            ///</summary>
            [TestMethod(), Timeout(2000)]
            [TestCategory("9")]
            public void TestComplexAddRemoveEmpty()
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
            ///Empty graph should contain nothing
            ///</summary>
            [TestMethod(), Timeout(2000)]
            [TestCategory("10")]
            public void TestAddRemoveIndexerEmpty()
            {
                DependencyGraph t = new DependencyGraph();
                t.AddDependency("x", "y");
                Assert.AreEqual(1, t["y"]);
                t.RemoveDependency("x", "y");
                Assert.AreEqual(0, t["y"]);
            }

            /// <summary>
            ///Removing from an empty DG shouldn't fail
            ///</summary>
            [TestMethod(), Timeout(2000)]
            [TestCategory("11")]
            public void TestRemoveTwice()
            {
                DependencyGraph t = new DependencyGraph();
                t.AddDependency("x", "y");
                Assert.AreEqual(t.Size, 1);
                t.RemoveDependency("x", "y");
                t.RemoveDependency("x", "y");
            }

            /// <summary>
            ///Replace on an empty DG shouldn't fail
            ///</summary>
            [TestMethod(), Timeout(2000)]
            [TestCategory("12")]
            public void TestRemoveReplace()
            {
                DependencyGraph t = new DependencyGraph();
                t.AddDependency("x", "y");
                Assert.AreEqual(t.Size, 1);
                t.RemoveDependency("x", "y");
                t.ReplaceDependents("x", new HashSet<string>());
                t.ReplaceDependees("y", new HashSet<string>());
            }


            // ********************** Making Sure that Static Variables Weren't Used ****************** //
            ///<summary>
            ///It should be possibe to have more than one DG at a time.  This test is
            ///repeated because I want it to be worth more than 1 point.
            ///</summary>
            [TestMethod(), Timeout(2000)]
            [TestCategory("13")]
            public void TestStatic1()
            {
                DependencyGraph t1 = new DependencyGraph();
                DependencyGraph t2 = new DependencyGraph();
                t1.AddDependency("x", "y");
                Assert.AreEqual(1, t1.Size);
                Assert.AreEqual(0, t2.Size);
            }

            // Increase the weight of this test by running it multiple times
            [TestMethod(), Timeout(2000)]
            [TestCategory("14")]
            public void TestStatic2()
            {
                TestStatic1();
            }

            [TestMethod(), Timeout(2000)]
            [TestCategory("15")]
            public void TestStatic3()
            {
                TestStatic1();
            }

            [TestMethod(), Timeout(2000)]
            [TestCategory("16")]
            public void TestStatic4()
            {
                TestStatic1();
            }

            [TestMethod(), Timeout(2000)]
            [TestCategory("17")]
            public void TestStatic5()
            {
                TestStatic1();
            }

            /**************************** SIMPLE NON-EMPTY TESTS ****************************/

            /// <summary>
            ///Non-empty graph contains something
            ///</summary>
            [TestMethod(), Timeout(2000)]
            [TestCategory("18")]
            public void TestSimpleSize()
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
            [TestMethod(), Timeout(2000)]
            [TestCategory("19")]
            public void TestSimpleIndexer()
            {
                DependencyGraph t = new DependencyGraph();
                t.AddDependency("a", "b");
                t.AddDependency("a", "c");
                t.AddDependency("c", "b");
                t.AddDependency("b", "d");
                Assert.AreEqual(2, t["b"]);
            }

            /// <summary>
            ///Non-empty graph contains something
            ///</summary>
            [TestMethod(), Timeout(2000)]
            [TestCategory("20")]
            public void TestSimpleHasDeps()
            {
                DependencyGraph t = new DependencyGraph();
                t.AddDependency("a", "b");
                t.AddDependency("a", "c");
                t.AddDependency("c", "b");
                t.AddDependency("b", "d");
                Assert.IsTrue(t.HasDependents("a"));
                Assert.IsFalse(t.HasDependees("a"));
                Assert.IsTrue(t.HasDependents("b"));
                Assert.IsTrue(t.HasDependees("b"));
            }

            /// <summary>
            ///Non-empty graph contains something
            ///</summary>
            [TestMethod(), Timeout(2000)]
            [TestCategory("21")]
            public void TestSimpleEnumerator()
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
            [TestMethod(), Timeout(2000)]
            [TestCategory("22")]
            public void TestSimpleEnumerator2()
            {
                DependencyGraph t = new DependencyGraph();
                t.AddDependency("a", "b");
                t.AddDependency("a", "c");
                t.AddDependency("c", "b");
                t.AddDependency("b", "d");

                IEnumerator<string> e = t.GetDependents("a").GetEnumerator();
                Assert.IsTrue(e.MoveNext());
                String s1 = e.Current;
                Assert.IsTrue(e.MoveNext());
                String s2 = e.Current;
                Assert.IsFalse(e.MoveNext());
                Assert.IsTrue(((s1 == "b") && (s2 == "c")) || ((s1 == "c") && (s2 == "b")));

                e = t.GetDependents("b").GetEnumerator();
                Assert.IsTrue(e.MoveNext());
                Assert.AreEqual("d", e.Current);
                Assert.IsFalse(e.MoveNext());

                e = t.GetDependents("c").GetEnumerator();
                Assert.IsTrue(e.MoveNext());
                Assert.AreEqual("b", e.Current);
                Assert.IsFalse(e.MoveNext());

                e = t.GetDependents("d").GetEnumerator();
                Assert.IsFalse(e.MoveNext());
            }

            /// <summary>
            ///Non-empty graph contains something
            ///</summary>
            [TestMethod(), Timeout(2000)]
            [TestCategory("23")]
            public void TestDuplicatesSize()
            {
                DependencyGraph t = new DependencyGraph();
                t.AddDependency("a", "b");
                t.AddDependency("a", "c");
                t.AddDependency("a", "b");
                t.AddDependency("c", "b");
                t.AddDependency("b", "d");
                t.AddDependency("c", "b");
                Assert.AreEqual(4, t.Size);
            }

            /// <summary>
            ///Non-empty graph contains something
            ///</summary>
            [TestMethod(), Timeout(2000)]
            [TestCategory("24")]
            public void TestDuplicatesIndexer()
            {
                DependencyGraph t = new DependencyGraph();
                t.AddDependency("a", "b");
                t.AddDependency("a", "c");
                t.AddDependency("a", "b");
                t.AddDependency("c", "b");
                t.AddDependency("b", "d");
                t.AddDependency("c", "b");
                Assert.AreEqual(2, t["b"]);
            }

            /// <summary>
            ///Non-empty graph contains something
            ///</summary>
            [TestMethod(), Timeout(2000)]
            [TestCategory("25")]
            public void TestDuplicatesDeps()
            {
                DependencyGraph t = new DependencyGraph();
                t.AddDependency("a", "b");
                t.AddDependency("a", "c");
                t.AddDependency("a", "b");
                t.AddDependency("c", "b");
                t.AddDependency("b", "d");
                t.AddDependency("c", "b");
                Assert.IsTrue(t.HasDependents("a"));
                Assert.IsFalse(t.HasDependees("a"));
                Assert.IsTrue(t.HasDependents("b"));
                Assert.IsTrue(t.HasDependees("b"));
            }

            /// <summary>
            ///Non-empty graph contains something
            ///</summary>
            [TestMethod(), Timeout(2000)]
            [TestCategory("26")]
            public void TestDuplicatesEnumerator()
            {
                DependencyGraph t = new DependencyGraph();
                t.AddDependency("a", "b");
                t.AddDependency("a", "c");
                t.AddDependency("a", "b");
                t.AddDependency("c", "b");
                t.AddDependency("b", "d");
                t.AddDependency("c", "b");

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
            [TestMethod(), Timeout(2000)]
            [TestCategory("27")]
            public void TestDuplicatesEnumerator2()
            {
                DependencyGraph t = new DependencyGraph();
                t.AddDependency("a", "b");
                t.AddDependency("a", "c");
                t.AddDependency("a", "b");
                t.AddDependency("c", "b");
                t.AddDependency("b", "d");
                t.AddDependency("c", "b");

                IEnumerator<string> e = t.GetDependents("a").GetEnumerator();
                Assert.IsTrue(e.MoveNext());
                String s1 = e.Current;
                Assert.IsTrue(e.MoveNext());
                String s2 = e.Current;
                Assert.IsFalse(e.MoveNext());
                Assert.IsTrue(((s1 == "b") && (s2 == "c")) || ((s1 == "c") && (s2 == "b")));

                e = t.GetDependents("b").GetEnumerator();
                Assert.IsTrue(e.MoveNext());
                Assert.AreEqual("d", e.Current);
                Assert.IsFalse(e.MoveNext());

                e = t.GetDependents("c").GetEnumerator();
                Assert.IsTrue(e.MoveNext());
                Assert.AreEqual("b", e.Current);
                Assert.IsFalse(e.MoveNext());

                e = t.GetDependents("d").GetEnumerator();
                Assert.IsFalse(e.MoveNext());
            }


            /// <summary>
            ///Non-empty graph contains something
            ///</summary>
            [TestMethod(), Timeout(2000)]
            [TestCategory("28")]
            public void TestComplexAddRemove()
            {
                DependencyGraph t = new DependencyGraph();
                t.AddDependency("x", "y");
                t.AddDependency("a", "b");
                t.AddDependency("a", "c");
                t.AddDependency("a", "d");
                t.AddDependency("c", "b");
                t.RemoveDependency("a", "d");
                t.AddDependency("e", "b");
                t.AddDependency("b", "d");
                t.RemoveDependency("e", "b");
                t.RemoveDependency("x", "y");
                Assert.AreEqual(4, t.Size);
            }

            /// <summary>
            ///Non-empty graph contains something
            ///</summary>
            [TestMethod(), Timeout(2000)]
            [TestCategory("29")]
            public void TestAddRemoveIndexer()
            {
                DependencyGraph t = new DependencyGraph();
                t.AddDependency("x", "y");
                t.AddDependency("a", "b");
                t.AddDependency("a", "c");
                t.AddDependency("a", "d");
                t.AddDependency("c", "b");
                t.RemoveDependency("a", "d");
                t.AddDependency("e", "b");
                t.AddDependency("b", "d");
                t.RemoveDependency("e", "b");
                t.RemoveDependency("x", "y");
                Assert.AreEqual(2, t["b"]);
            }

            /// <summary>
            ///Non-empty graph contains something
            ///</summary>
            [TestMethod(), Timeout(2000)]
            [TestCategory("30")]
            public void TestAddRemoveDeps()
            {
                DependencyGraph t = new DependencyGraph();
                t.AddDependency("x", "y");
                t.AddDependency("a", "b");
                t.AddDependency("a", "c");
                t.AddDependency("a", "d");
                t.AddDependency("c", "b");
                t.RemoveDependency("a", "d");
                t.AddDependency("e", "b");
                t.AddDependency("b", "d");
                t.RemoveDependency("e", "b");
                t.RemoveDependency("x", "y");
                Assert.IsTrue(t.HasDependents("a"));
                Assert.IsFalse(t.HasDependees("a"));
                Assert.IsTrue(t.HasDependents("b"));
                Assert.IsTrue(t.HasDependees("b"));
            }


            /// <summary>
            ///Non-empty graph contains something
            ///</summary>
            [TestMethod(), Timeout(2000)]
            [TestCategory("31")]
            public void TestComplexEnumerator()
            {
                DependencyGraph t = new DependencyGraph();
                t.AddDependency("x", "y");
                t.AddDependency("a", "b");
                t.AddDependency("a", "c");
                t.AddDependency("a", "d");
                t.AddDependency("c", "b");
                t.RemoveDependency("a", "d");
                t.AddDependency("e", "b");
                t.AddDependency("b", "d");
                t.RemoveDependency("e", "b");
                t.RemoveDependency("x", "y");

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
            [TestMethod(), Timeout(2000)]
            [TestCategory("32")]
            public void TestComplexEnumerator2()
            {
                DependencyGraph t = new DependencyGraph();
                t.AddDependency("x", "y");
                t.AddDependency("a", "b");
                t.AddDependency("a", "c");
                t.AddDependency("a", "d");
                t.AddDependency("c", "b");
                t.RemoveDependency("a", "d");
                t.AddDependency("e", "b");
                t.AddDependency("b", "d");
                t.RemoveDependency("e", "b");
                t.RemoveDependency("x", "y");

                IEnumerator<string> e = t.GetDependents("a").GetEnumerator();
                Assert.IsTrue(e.MoveNext());
                String s1 = e.Current;
                Assert.IsTrue(e.MoveNext());
                String s2 = e.Current;
                Assert.IsFalse(e.MoveNext());
                Assert.IsTrue(((s1 == "b") && (s2 == "c")) || ((s1 == "c") && (s2 == "b")));

                e = t.GetDependents("b").GetEnumerator();
                Assert.IsTrue(e.MoveNext());
                Assert.AreEqual("d", e.Current);
                Assert.IsFalse(e.MoveNext());

                e = t.GetDependents("c").GetEnumerator();
                Assert.IsTrue(e.MoveNext());
                Assert.AreEqual("b", e.Current);
                Assert.IsFalse(e.MoveNext());

                e = t.GetDependents("d").GetEnumerator();
                Assert.IsFalse(e.MoveNext());
            }

            /// <summary>
            /// Replace on an empty graph results in a non-empty graph
            /// </summary>
            [TestMethod(), Timeout(2000)]
            [TestCategory("33")]
            public void TestEmptyReplaceDependees()
            {
                DependencyGraph dg = new DependencyGraph();

                dg.ReplaceDependees("b", new HashSet<string> { "a" });

                Assert.AreEqual(1, dg.Size);
                Assert.IsTrue(new HashSet<string> { "b" }.SetEquals(dg.GetDependents("a")));
            }

            /// <summary>
            ///Non-empty graph contains something
            ///</summary>
            [TestMethod(), Timeout(2000)]
            [TestCategory("34")]
            public void TestComplexReplace()
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
                Assert.AreEqual(4, t.Size);
            }

            /// <summary>
            ///Non-empty graph contains something
            ///</summary>
            [TestMethod(), Timeout(2000)]
            [TestCategory("35")]
            public void TestComplexReplaceIndexer()
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
                Assert.AreEqual(2, t["b"]);
            }

            /// <summary>
            ///Non-empty graph contains something
            ///</summary>
            [TestMethod(), Timeout(2000)]
            [TestCategory("36")]
            public void TestComplexReplace2()
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
                Assert.IsTrue(t.HasDependents("a"));
                Assert.IsFalse(t.HasDependees("a"));
                Assert.IsTrue(t.HasDependents("b"));
                Assert.IsTrue(t.HasDependees("b"));
            }

            /// <summary>
            ///Non-empty graph contains something
            ///</summary>
            [TestMethod(), Timeout(2000)]
            [TestCategory("37")]
            public void TestComplexReplaceEnumerator()
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
            ///Non-empty graph contains something
            ///</summary>
            [TestMethod(), Timeout(2000)]
            [TestCategory("38")]
            public void TestComplexReplaceEnumerator2()
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

                IEnumerator<string> e = t.GetDependents("a").GetEnumerator();
                Assert.IsTrue(e.MoveNext());
                String s1 = e.Current;
                Assert.IsTrue(e.MoveNext());
                String s2 = e.Current;
                Assert.IsFalse(e.MoveNext());
                Assert.IsTrue(((s1 == "b") && (s2 == "c")) || ((s1 == "c") && (s2 == "b")));

                e = t.GetDependents("b").GetEnumerator();
                Assert.IsTrue(e.MoveNext());
                Assert.AreEqual("d", e.Current);
                Assert.IsFalse(e.MoveNext());

                e = t.GetDependents("c").GetEnumerator();
                Assert.IsTrue(e.MoveNext());
                Assert.AreEqual("b", e.Current);
                Assert.IsFalse(e.MoveNext());

                e = t.GetDependents("d").GetEnumerator();
                Assert.IsFalse(e.MoveNext());
            }


            // ************************** STRESS TESTS REPEATED MULTIPLE TIMES ******************************** //
            /// <summary>
            ///Using lots of data
            ///</summary>
            [TestMethod(), Timeout(2000)]
            [TestCategory("39")]
            public void StressTest1()
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

            [TestMethod(), Timeout(2000)]
            [TestCategory("40")]
            public void StressTest2()
            {
                StressTest1();
            }
            [TestMethod(), Timeout(2000)]
            [TestCategory("41")]
            public void StressTest3()
            {
                StressTest1();
            }


            // ********************************** ANOTHER STESS TEST, REPEATED ******************** //
            /// <summary>
            ///Using lots of data with replacement
            ///</summary>
            [TestMethod(), Timeout(2000)]
            [TestCategory("42")]
            public void StressTest8()
            {
                // Dependency graph
                DependencyGraph t = new DependencyGraph();

                // A bunch of strings to use
                const int SIZE = 400;
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
                    for (int j = i + 2; j < SIZE; j += 3)
                    {
                        t.RemoveDependency(letters[i], letters[j]);
                        dents[i].Remove(letters[j]);
                        dees[j].Remove(letters[i]);
                    }
                }

                // Replace a bunch of dependents
                for (int i = 0; i < SIZE; i += 2)
                {
                    HashSet<string> newDents = new HashSet<String>();
                    for (int j = 0; j < SIZE; j += 5)
                    {
                        newDents.Add(letters[j]);
                    }
                    t.ReplaceDependents(letters[i], newDents);

                    foreach (string s in dents[i])
                    {
                        dees[s[0] - 'a'].Remove(letters[i]);
                    }

                    foreach (string s in newDents)
                    {
                        dees[s[0] - 'a'].Add(letters[i]);
                    }

                    dents[i] = newDents;
                }

                // Make sure everything is right
                for (int i = 0; i < SIZE; i++)
                {
                    Assert.IsTrue(dents[i].SetEquals(new HashSet<string>(t.GetDependents(letters[i]))));
                    Assert.IsTrue(dees[i].SetEquals(new HashSet<string>(t.GetDependees(letters[i]))));
                }
            }

            [TestMethod(), Timeout(2000)]
            [TestCategory("43")]
            public void StressTest9()
            {
                StressTest8();
            }
            [TestMethod(), Timeout(2000)]
            [TestCategory("44")]
            public void StressTest10()
            {
                StressTest8();
            }


            // ********************************** A THIRD STESS TEST, REPEATED ******************** //
            /// <summary>
            ///Using lots of data with replacement
            ///</summary>
            [TestMethod(), Timeout(2000)]
            [TestCategory("45")]
            public void StressTest15()
            {
                // Dependency graph
                DependencyGraph t = new DependencyGraph();

                // A bunch of strings to use
                const int SIZE = 1000;
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

                for (int i = 0; i < SIZE; i++)
                {
                    Assert.IsTrue(dents[i].SetEquals(new HashSet<string>(t.GetDependents(letters[i]))));
                    Assert.IsTrue(dees[i].SetEquals(new HashSet<string>(t.GetDependees(letters[i]))));
                }

                // Remove a bunch of dependencies
                for (int i = 0; i < SIZE; i++)
                {
                    for (int j = i + 2; j < SIZE; j += 3)
                    {
                        t.RemoveDependency(letters[i], letters[j]);
                        dents[i].Remove(letters[j]);
                        dees[j].Remove(letters[i]);
                    }
                }

                for (int i = 0; i < SIZE; i++)
                {
                    Assert.IsTrue(dents[i].SetEquals(new HashSet<string>(t.GetDependents(letters[i]))));
                    Assert.IsTrue(dees[i].SetEquals(new HashSet<string>(t.GetDependees(letters[i]))));
                }

                // Replace a bunch of dependees
                for (int i = 0; i < SIZE; i += 2)
                {
                    HashSet<string> newDees = new HashSet<String>();
                    for (int j = 0; j < SIZE; j += 9)
                    {
                        newDees.Add(letters[j]);
                    }
                    t.ReplaceDependees(letters[i], newDees);

                    foreach (string s in dees[i])
                    {
                        dents[s[0] - 'a'].Remove(letters[i]);
                    }

                    foreach (string s in newDees)
                    {
                        dents[s[0] - 'a'].Add(letters[i]);
                    }

                    dees[i] = newDees;
                }

                // Make sure everything is right
                for (int i = 0; i < SIZE; i++)
                {
                    Assert.IsTrue(dents[i].SetEquals(new HashSet<string>(t.GetDependents(letters[i]))));
                    Assert.IsTrue(dees[i].SetEquals(new HashSet<string>(t.GetDependees(letters[i]))));
                }
            }

            [TestMethod(), Timeout(2000)]
            [TestCategory("46")]
            public void StressTest16()
            {
                StressTest15();
            }
            [TestMethod(), Timeout(2000)]
            [TestCategory("47")]
            public void StressTest17()
            {
                StressTest15();
            }

        

    }
}