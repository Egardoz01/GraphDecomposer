using System;
using System.Collections.Generic;
using System.Text;

namespace GrubiTest.DataStructures
{
    public struct SolverResult
    {
        public int iterationsCnt;
        public bool solutionExistance;
        public long millisecondsElapsed;
        public SolverResult(int iterCnt, bool solution)
        {
            iterationsCnt = iterCnt;
            solutionExistance = solution;
            millisecondsElapsed = 0;
        }
    }
}
