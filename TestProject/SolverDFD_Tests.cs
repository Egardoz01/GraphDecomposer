using GraphDecomposer;
using GraphDecomposer.DataStructures;
using GraphDecomposer.Parsers;
using GraphDecomposer.Solvers;
using System;
using System.IO;
using Xunit;

namespace TestProject
{
    public class SolverDFD_Tests
    {
        [Fact]
        public void UndirectedTests()
        {
            var conf = ConfigurationParser.GetConfiguration("testsConfiguration.json");
            SolverDFD solver = new SolverDFD();
            foreach(var test in conf)
            {
                if(!test.directed)
                    Tester.DoTest(solver, test);
            }
        }


        [Fact]
        public void DirectedTest()
        {
            var conf = ConfigurationParser.GetConfiguration("testsConfiguration.json");
            SolverDFD solver = new SolverDFD();
            foreach (var test in conf)
            {
                if (test.directed)
                    Tester.DoTest(solver, test);
            }

        }


    }
}
