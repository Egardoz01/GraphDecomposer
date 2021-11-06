using GrubiTest.DataStructures;
using System;
using System.Collections.Generic;
using System.Text;

namespace GrubiTest.Solvers
{
    public interface ISolver
    {
        SolverResult Solve(ProblemInput input);
    }
}
