using GraphDecomposer.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphDecomposer
{
    public class ResultAnalyses
    {
        private List<SolverResult> solverResults;
        private readonly TestConfiguration conf;
        public ResultAnalyses(TestConfiguration conf)
        {
            this.conf = conf;
            solverResults = new List<SolverResult>();
        }

        public void addResult(SolverResult res)
        {
            solverResults.Add(res);
        }

        public void printResults()
        {
            List<double> solvedTime = new List<double>();
            List<double> solvedIterations = new List<double>();
            List<double> unsolvedIterations = new List<double>();
            List<double> unsolvedTime = new List<double>();

            foreach (var res in solverResults)
            {
                if (res.solutionExistance)
                {
                    solvedTime.Add(res.millisecondsElapsed / 1000.0);
                    solvedIterations.Add(res.iterationsCnt);
                }
                else
                {
                    unsolvedTime.Add(res.millisecondsElapsed / 1000.0);
                    unsolvedIterations.Add(res.iterationsCnt);
                }
            }

            Console.WriteLine($"Test File {conf.testFile}");
            Console.WriteLine((conf.directed ? "directed" : "undirected") + " graph");
            Console.WriteLine($"Vertex amount {conf.nVertices}");
            Console.WriteLine($"Tests amount {conf.nTests}");

            if (solvedTime.Count>0)
            {
                Console.WriteLine($"Feasible Tests count: {solvedTime.Count}");
                Console.WriteLine($"Feasible M(time) {countM(solvedTime)}");
                Console.WriteLine($"Feasible Sd(time) {countSd(solvedTime)}");
                Console.WriteLine($"Feasible M(iterations) {countM(solvedIterations)}");
                Console.WriteLine($"Feasible Sd(iterations) {countSd(solvedIterations)}");
            }
            if (unsolvedTime.Count > 0)
            {
                Console.WriteLine($"Infeasible Tests count: {unsolvedTime.Count}");
                Console.WriteLine($"Infeasible M(time) {countM(unsolvedTime)}");
                Console.WriteLine($"Infeasible Sd(time) {countSd(unsolvedTime)}");
                Console.WriteLine($"Infeasible M(iterations) {countM(unsolvedIterations)}");
                Console.WriteLine($"Infeasible Sd(iterations) {countSd(unsolvedIterations)}");
            }

            Console.WriteLine($"");
        }


        private double countM(List<double> v)
        {
           return v.Sum()/v.Count;
        }

        private double countSd(List<double> v)
        {
            double M = countM(v);

            return Math.Sqrt(v.Select(x => (x - M) * (x - M)).Sum() / v.Count);
        }

    }
}
