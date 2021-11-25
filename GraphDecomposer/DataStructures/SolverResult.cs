using System;
using System.Collections.Generic;
using System.Text;

namespace GraphDecomposer.DataStructures
{
    public struct SolverResult
    {
        public int iterationsCnt;
        public bool solutionExistance;
        public long millisecondsElapsed;
        Graph z, w;
        public SolverResult(int iterCnt, bool solution, Graph z, Graph w)
        {
            iterationsCnt = iterCnt;
            solutionExistance = solution;
            millisecondsElapsed = 0;
            this.z = z;
            this.w = w;
        }
    }
}
