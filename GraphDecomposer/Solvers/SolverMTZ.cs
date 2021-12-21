using GraphDecomposer.DataStructures;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace GraphDecomposer.Solvers
{
    public class SolverMTZ : ISolver
    {
        public SolverMTZ_Directed directedSolver;
        public SolverMTZ_UnDirected undirectedSolver;
        public SolverMTZ()
        {
            directedSolver = new SolverMTZ_Directed();
            undirectedSolver = new SolverMTZ_UnDirected();
        }
        public SolverResult SolveTest(TestInput input, TestConfiguration conf, Stopwatch timer)
        {
            if (conf.directed)
                return directedSolver.SolveTest(input, conf, timer);
            else
                return undirectedSolver.SolveTest(input, conf, timer);
        }
    }
}
