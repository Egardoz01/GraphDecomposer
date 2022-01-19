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
            string title = "Model;Test file;Tests passed;Graph type;Vertex amount;F count;F M(time);F Sd(time);F M(iterations);F Sd(iterations);Inf Tests count;Inf M(time);Inf Sd(time);Inf M(iterations);Inf Sd(iterations); F_5_C count;F_5_C M(time);F_5_C Sd(time);F_5_C M(iterations);F_5_C Sd(iterations);Inf_5_C Tests count;Inf_5_C M(time);Inf_5_C Sd(time);Inf_5_C M(iterations);Inf_5_C Sd(iterations); cnt_z; cnt_w; cnt_q; Time Limit Tests Count";
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

            List<double> fg_solvedTime = new List<double>();
            List<double> fg_solvedIterations = new List<double>();
            List<double> fg_unsolvedIterations = new List<double>();
            List<double> fg_unsolvedTime = new List<double>();
            int cnt_z = 0, cnt_w = 0, cnt_q = 0;

            double doubleEdgesCnt = 0;
            int tl_cnt = 0;
            foreach (var res in solverResults.FindAll(x => x.five_graphs_task == false))
            {
                if (res.solutionExistance)
                {
                    solvedTime.Add(res.millisecondsElapsed / 1000.0);
                    solvedIterations.Add(res.iterationsCnt);
                }
                else
                {


                    if (res.TimeLimit == true)
                        tl_cnt++;
                    else
                    {
                        unsolvedTime.Add(res.millisecondsElapsed / 1000.0);
                        unsolvedIterations.Add(res.iterationsCnt);
                    }
                }

                doubleEdgesCnt += res.InitialZ.FindDoubleEdges(res.InitialW).Count;
                gurubiTime.Add(res.millisecondsGorubi / 1000.0);
                localSearchTime.Add(res.millisecondsLinearSearch / 1000.0);
            }

            foreach (var res in solverResults.FindAll(x => x.five_graphs_task == true))
            {

                if (res.q != null)
                    cnt_q++;
                else if (res.w != null)
                    cnt_w++;
                else if (res.z != null)
                    cnt_z++;

                if (res.solutionExistance)
                {
                    fg_solvedTime.Add(res.millisecondsElapsed / 1000.0);
                    fg_solvedIterations.Add(res.iterationsCnt);
                }
                else
                {
                    if (res.TimeLimit == true)
                        tl_cnt++;
                    else
                    {
                        fg_unsolvedTime.Add(res.millisecondsElapsed / 1000.0);
                        fg_unsolvedIterations.Add(res.iterationsCnt);
                    }
                }

                doubleEdgesCnt += res.InitialZ.FindDoubleEdges(res.InitialW).Count;
            }



            List<string> lines = new List<string>();
            List<string> linesCSV = new List<string>();


            lines.Add($"Model {conf.model}");
            lines.Add($"Test File {conf.testFile}");
            lines.Add((conf.directed ? "directed" : "undirected") + " graph");
            lines.Add($"Vertex amount {conf.nVertices}");
            lines.Add($"Tests amount {conf.nTests}");
            lines.Add($"Tests passed { solverResults.Count}");
            lines.Add($"Average Double Edges Amount { (solverResults.Count > 0 ? doubleEdgesCnt / solverResults.Count : '0')}");
            lines.Add($"M(gorubi working time) { countM(gurubiTime)}");
            lines.Add($"Sd(gurubi working time) { countSd(gurubiTime)}");
            lines.Add($"M(local search working time) { countM(localSearchTime)}");
            lines.Add($"Sd(local search working time) { countSd(localSearchTime)}");
            lines.Add($"Time Limit tests: {tl_cnt}");




            linesCSV.Add(conf.model);
            linesCSV.Add(Path.GetFileName(conf.testFile));
            linesCSV.Add(solvedIterations.Count.ToString());
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
            else
            {
                linesCSV.Add(" ");
                linesCSV.Add(" ");
                linesCSV.Add(" ");
                linesCSV.Add(" ");
                linesCSV.Add(" ");
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
            else
            {
                linesCSV.Add(" ");
                linesCSV.Add(" ");
                linesCSV.Add(" ");
                linesCSV.Add(" ");
                linesCSV.Add(" ");
            }



            if (fg_solvedTime.Count > 0)
            {
                lines.Add($"5 cycles Feasible Tests count: {fg_solvedTime.Count}");
                lines.Add($"5 cycles Feasible M(time) {countM(fg_solvedTime)}");
                lines.Add($"5 cycles Feasible Sd(time) {countSd(fg_solvedTime)}");
                lines.Add($"5 cycles Feasible M(iterations) {countM(fg_solvedIterations)}");
                lines.Add($"5 cycles Feasible Sd(iterations) {countSd(fg_solvedIterations)}");

                linesCSV.Add(fg_solvedTime.Count.ToString());
                linesCSV.Add(countM(fg_solvedTime).ToString());
                linesCSV.Add(countSd(fg_solvedTime).ToString());
                linesCSV.Add(countM(fg_solvedIterations).ToString());
                linesCSV.Add(countSd(fg_solvedIterations).ToString());
            }
            else
            {
                linesCSV.Add(" ");
                linesCSV.Add(" ");
                linesCSV.Add(" ");
                linesCSV.Add(" ");
                linesCSV.Add(" ");
            }

            if (fg_unsolvedTime.Count > 0)
            {
                lines.Add($"5 cycles Infeasible Tests count: {fg_unsolvedTime.Count}");
                lines.Add($"5 cycles Infeasible M(time) {countM(fg_unsolvedTime)}");
                lines.Add($"5 cycles Infeasible Sd(time) {countSd(fg_unsolvedTime)}");
                lines.Add($"5 cycles Infeasible M(iterations) {countM(fg_unsolvedIterations)}");
                lines.Add($"5 cycles Infeasible Sd(iterations) {countSd(fg_unsolvedIterations)}");

                linesCSV.Add(fg_unsolvedTime.Count.ToString());
                linesCSV.Add(countM(fg_unsolvedTime).ToString());
                linesCSV.Add(countSd(fg_unsolvedTime).ToString());
                linesCSV.Add(countM(fg_unsolvedIterations).ToString());
                linesCSV.Add(countSd(fg_unsolvedIterations).ToString());

                lines.Add($"5 cycles cnt_z: {cnt_z}");
                lines.Add($"5 cycles cnt_w {cnt_w}");
                lines.Add($"5 cycles cnt_q {cnt_q}");

                linesCSV.Add(cnt_z.ToString());
                linesCSV.Add(cnt_w.ToString());
                linesCSV.Add(cnt_q.ToString());

            }
            else
            {
                linesCSV.Add(" ");
                linesCSV.Add(" ");
                linesCSV.Add(" ");
                linesCSV.Add(" ");
                linesCSV.Add(" ");

                linesCSV.Add(" ");
                linesCSV.Add(" ");
                linesCSV.Add(" ");
            }


            linesCSV.Add(tl_cnt.ToString());

            lines.Add($"");
            lines.Add($"");

            foreach (var s in lines)
            {
                Console.WriteLine(s);
            }

            File.AppendAllLines("results.txt", lines);
            File.AppendAllText(outputFile, "\n" + String.Join(';', linesCSV.ToArray()));
        }


        private double countM(List<double> v)
        {
            if (v.Count == 0)
                return 0;
            return v.Sum() / v.Count;
        }

        private double countSd(List<double> v)
        {
            if (v.Count == 0)
                return 0;

            double M = countM(v);

            return Math.Sqrt(v.Select(x => (x - M) * (x - M)).Sum() / v.Count);
        }

    }
}
