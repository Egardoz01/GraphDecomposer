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
        public static readonly string outputFile;

        static ResultAnalyses()
        {
            outputFile = "results\\result_" + DateTime.Now.ToString("yyyy'-'MM'-'dd'-'HH'-'mm'-'ss") + ".csv";
            string title = "Model;Test file;Graph type;Vertex amount;F count;F M(time);F Sd(time);F M(iterations);F Sd(iterations);Inf Tests count;Inf M(time);Inf Sd(time);Inf M(iterations);Inf Sd(iterations)";
            File.WriteAllText(outputFile, title);

        }
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
            List<double> gurubiTime = new List<double>();
            List<double> localSearchTime = new List<double>();

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
                gurubiTime.Add(res.millisecondsGorubi / 1000.0);
                localSearchTime.Add(res.millisecondsLinearSearch / 1000.0);
            }
            List<string> lines = new List<string>();
            List<string> linesCSV = new List<string>();


            lines.Add($"Model {conf.model}");
            lines.Add($"Test File {conf.testFile}");
            lines.Add((conf.directed ? "directed" : "undirected") + " graph");
            lines.Add($"Vertex amount {conf.nVertices}");
            lines.Add($"Tests amount {conf.nTests}");
            lines.Add($"Average Double Edges Amount { doubleEdgesCnt / conf.nTests }");
            lines.Add($"M(gorubi working time) { countM(gurubiTime)}");
            lines.Add($"Sd(gurubi working time) { countSd(gurubiTime)}");
            lines.Add($"M(local search working time) { countM(localSearchTime)}");
            lines.Add($"Sd(local search working time) { countSd(localSearchTime)}");

            linesCSV.Add(conf.model);
            linesCSV.Add(Path.GetFileName(conf.testFile));
            linesCSV.Add((conf.directed ? "directed" : "undirected"));
            linesCSV.Add(conf.nVertices.ToString());



            if (solvedTime.Count > 0)
            {
                lines.Add($"Feasible Tests count: {solvedTime.Count}");
                lines.Add($"Feasible M(time) {countM(solvedTime)}");
                lines.Add($"Feasible Sd(time) {countSd(solvedTime)}");
                lines.Add($"Feasible M(iterations) {countM(solvedIterations)}");
                lines.Add($"Feasible Sd(iterations) {countSd(solvedIterations)}");

                linesCSV.Add(solvedTime.Count.ToString());
                linesCSV.Add(countM(solvedTime).ToString());
                linesCSV.Add(countSd(solvedTime).ToString());
                linesCSV.Add(countM(solvedIterations).ToString());
                linesCSV.Add(countSd(solvedIterations).ToString());
            }
            if (unsolvedTime.Count > 0)
            {
                lines.Add($"Infeasible Tests count: {unsolvedTime.Count}");
                lines.Add($"Infeasible M(time) {countM(unsolvedTime)}");
                lines.Add($"Infeasible Sd(time) {countSd(unsolvedTime)}");
                lines.Add($"Infeasible M(iterations) {countM(unsolvedIterations)}");
                lines.Add($"Infeasible Sd(iterations) {countSd(unsolvedIterations)}");

                linesCSV.Add(unsolvedTime.Count.ToString());
                linesCSV.Add(countM(unsolvedTime).ToString());
                linesCSV.Add(countSd(unsolvedTime).ToString());
                linesCSV.Add(countM(unsolvedIterations).ToString());
                linesCSV.Add(countSd(unsolvedIterations).ToString());
            }

            lines.Add($"");
            lines.Add($"");

            foreach (var s in lines)
            {
                Console.WriteLine(s);
            }

            File.AppendAllLines("results.txt", lines);
            File.AppendAllText(outputFile,"\n"+ String.Join(';', linesCSV.ToArray()));
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
