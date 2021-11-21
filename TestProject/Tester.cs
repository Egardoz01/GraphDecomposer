using GraphDecomposer;
using GraphDecomposer.DataStructures;
using GraphDecomposer.Solvers;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace TestProject
{
    public static class Tester
    {

       public  static void DoTest(ISolver solver, TestConfiguration conf)
        {
            InputParser input = new InputParser(conf.testFile, conf.nTests);
            int cntSolvable = 0, cntUnsolvable = 0;
            foreach (var p in input)
            {
                var res = solver.SolveTest(p, conf);

                if (res.solutionExistance)
                    cntSolvable++;
                else
                    cntUnsolvable++;

            }

            Assert.Equal(conf.solvable, cntSolvable);
            Assert.Equal(conf.unsolvable, cntUnsolvable);

        }
    }
}
