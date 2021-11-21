using GraphDecomposer;
using GraphDecomposer.Parsers;
using GraphDecomposer.Solvers;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace TestProject
{
    public class SolverMTZ_Directed_Tests
    {
        [Fact]
        public void DirectedTest()
        {
            var conf = ConfigurationParser.GetConfiguration("testsConfiguration.json");
            SolverMTZ_Directed solver = new SolverMTZ_Directed();
            foreach (var test in conf)
            {
                if (test.directed)
                    Tester.DoTest(solver, test);
            }

        }
    }
}
