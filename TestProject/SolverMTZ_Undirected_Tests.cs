using GraphDecomposer.Parsers;
using GraphDecomposer.Solvers;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace TestProject
{
    public class SolverMTZ_Undirected_Tests
    {
        [Fact]
        public void UndirectedTests()
        {
            var conf = ConfigurationParser.GetConfiguration("testsConfiguration.json");
            SolverNTZ_UnDirected solver = new SolverNTZ_UnDirected();
            foreach (var test in conf)
            {
                if (!test.directed)
                    Tester.DoTest(solver, test);
            }
        }
    }
}
