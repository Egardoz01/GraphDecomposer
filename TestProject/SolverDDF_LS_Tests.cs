using GraphDecomposer;
using GraphDecomposer.Parsers;
using GraphDecomposer.Solvers;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace TestProject
{
    public static class SolverDDF_LS_Tests
    {
        [Fact]
        public static void DirectedTest()
        {
            var conf = ConfigurationParser.GetConfiguration("testsConfiguration.json");
            SolverDFDOptimized solver = new SolverDFDOptimized();
            foreach (var test in conf)
            {
                if (test.directed)
                    Tester.DoTest(solver, test);
            }

        }

    }
}
