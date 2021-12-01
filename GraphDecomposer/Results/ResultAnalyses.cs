using GraphDecomposer.DataStructures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GraphDecomposer.Results
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

            double doubleEdgesCnt = 0;
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

                doubleEdgesCnt += res.InitialZ.FindDoubleEdges(res.InitialW).Count;
            }
            List<string> lines = new List<string>(); ;

            lines.Add($"Model {conf.model}");
            lines.Add($"Test File {conf.testFile}");
            lines.Add((conf.directed ? "directed" : "undirected") + " graph");
            lines.Add($"Vertex amount {conf.nVertices}");
            lines.Add($"Tests amount {conf.nTests}");
            lines.Add($"Average Double Edges Amount { doubleEdgesCnt / conf.nTests }");


            if (solvedTime.Count > 0)
            {
                lines.Add($"Feasible Tests count: {solvedTime.Count}");
                lines.Add($"Feasible M(time) {countM(solvedTime)}");
                lines.Add($"Feasible Sd(time) {countSd(solvedTime)}");
                lines.Add($"Feasible M(iterations) {countM(solvedIterations)}");
                lines.Add($"Feasible Sd(iterations) {countSd(solvedIterations)}");
            }
            if (unsolvedTime.Count > 0)
            {
                lines.Add($"Infeasible Tests count: {unsolvedTime.Count}");
                lines.Add($"Infeasible M(time) {countM(unsolvedTime)}");
                lines.Add($"Infeasible Sd(time) {countSd(unsolvedTime)}");
                lines.Add($"Infeasible M(iterations) {countM(unsolvedIterations)}");
                lines.Add($"Infeasible Sd(iterations) {countSd(unsolvedIterations)}");
            }

            lines.Add($"");
            lines.Add($"");

            foreach (var s in lines)
            {
                Console.WriteLine(s);
            }

            File.AppendAllLines("results.txt", lines);

        }


        private double countM(List<double> v)
        {
            return v.Sum() / v.Count;
        }

        private double countSd(List<double> v)
        {
            double M = countM(v);

            return Math.Sqrt(v.Select(x => (x - M) * (x - M)).Sum() / v.Count);
        }

    }
}
