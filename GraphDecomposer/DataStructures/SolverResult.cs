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
        public long millisecondsGorubi;
        public long millisecondsLinearSearch;
        public bool TimeLimit;
        public Graph z, w, q;
        public Graph InitialZ, InitialW;
        public bool five_graphs_task;
        public SolverResult(int iterCnt, bool solution, Graph z, Graph w)
        {
            millisecondsGorubi = 0;
            millisecondsLinearSearch = 0;
            iterationsCnt = iterCnt;
            solutionExistance = solution;
            millisecondsElapsed = 0;
            this.z = z;
            this.w = w;
            InitialW = null;
            InitialZ = null;
            TimeLimit = false;
            this.q = null;
            five_graphs_task = false;
        }


        public SolverResult(int iterCnt, Graph z, Graph w, Graph q)
        {
            millisecondsGorubi = 0;
            millisecondsLinearSearch = 0;
            iterationsCnt = iterCnt;
            solutionExistance = z != null && w != null && q != null;
            millisecondsElapsed = 0;
            this.z = z;
            this.w = w;
            this.q = q;
            InitialW = null;
            InitialZ = null;
            TimeLimit = false;
            five_graphs_task = true;
        }


        public SolverResult(int iterCnt, bool solution, Graph z, Graph w, long millisecondsGorubi, long millisecondsLinearSearch)
        {
            this.millisecondsGorubi = millisecondsGorubi;
            this.millisecondsLinearSearch = millisecondsLinearSearch;
            iterationsCnt = iterCnt;
            solutionExistance = solution;
            millisecondsElapsed = 0;
            this.z = z;
            this.w = w;
            InitialW = null;
            InitialZ = null;
            TimeLimit = false;
            q = null;
            five_graphs_task = false;
        }
    }
}
