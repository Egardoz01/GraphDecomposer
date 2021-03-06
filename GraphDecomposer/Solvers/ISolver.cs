using GraphDecomposer.DataStructures;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace GraphDecomposer.Solvers
{
    public interface ISolver
    {
        SolverResult SolveTest(TestInput input, TestConfiguration conf, Stopwatch timer);
    }
}
