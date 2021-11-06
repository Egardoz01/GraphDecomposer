using GrubiTest.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrubiTest
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
                Console.WriteLine($"Feasible average time {countAverage(solvedTime)}");
                Console.WriteLine($"Feasible time dispersion {countDispersion(solvedTime)}");
                Console.WriteLine($"Feasible average iterations {countAverage(solvedIterations)}");
                Console.WriteLine($"Feasible iterations dispersion {countDispersion(solvedIterations)}");
            }
            if (unsolvedTime.Count > 0)
            {
                Console.WriteLine($"Infeasible average time {countAverage(unsolvedTime)}");
                Console.WriteLine($"Infeasible time dispersion {countDispersion(unsolvedTime)}");
                Console.WriteLine($"Infeasible average iterations {countAverage(unsolvedIterations)}");
                Console.WriteLine($"Infeasible iterations dispersion {countDispersion(unsolvedIterations)}");
            }
        }


        private double countAverage(List<double> v)
        {
           return v.Sum()/v.Count;
        }

        private double countDispersion(List<double> v)
        {
            double M = countAverage(v);

            return v.Select(x => (x - M) * (x - M)).Sum() / v.Count;
        }

    }
}
