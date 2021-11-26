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
        public Graph z, w;
        public Graph InitialZ, InitialW; 
        public SolverResult(int iterCnt, bool solution, Graph z, Graph w)
        {
            iterationsCnt = iterCnt;
            solutionExistance = solution;
            millisecondsElapsed = 0;
            this.z = z;
            this.w = w;
            InitialW = null;
            InitialZ = null;
        }
    }
}
