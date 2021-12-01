using GraphDecomposer.DataStructures;
using GraphDecomposer.Parsers;
using GraphDecomposer.Results;
using GraphDecomposer.Solvers;
using Gurobi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace GraphDecomposer
{
    class Program
    {
        static void Main(string[] args)
        {
            //DoConfigurationTesting();
            //TestDFDOptimized1();
            //TestDFDOptimized01();
            TestDFDOptimized012();
           // TestDFDOptimized0();
            // TestDFD();
            //TestMTZ();
        }




        static void DoConfigurationTesting()
        {
            var conf = ConfigurationParser.GetConfiguration("configuration.json");
            SolverDFD solverDFD = new SolverDFD();
            SolverDFDOptimized solverDFD_LS = new SolverDFDOptimized();
            SolverMTZ solverMTZ = new SolverMTZ();
            foreach (var test in conf)
            {
                if (test.model == "dfd")
                    DoTest(solverDFD, test);
                else if (test.model == "dfd+ls")
                    DoTest(solverDFD_LS, test);
                else
                    DoTest(solverMTZ, test);
            }
        }

        static void TestDFDOptimized01()
        {
            var conf = ConfigurationParser.GetConfiguration("configuration.json");
            SolverDFDOptimized solver = new SolverDFDOptimized();

            for (int i = 0; i < conf.Count; i++)
            {
                var test = conf[i];
                test.model = "dfd+ls 0-1";
                test.firstNeighbourhood = true;
                test.zeroNeighbour = true;
                DoTest(solver, test);
            }
        }

        static void TestDFDOptimized012()
        {
            var conf = ConfigurationParser.GetConfiguration("configuration.json");
            SolverDFDOptimized solver = new SolverDFDOptimized();

            for (int i = 0; i < conf.Count; i++)
            {
                var test = conf[i];
                test.model = "dfd+ls 0-1-2";
                test.firstNeighbourhood = true;
                test.zeroNeighbour = true;
                test.secondNeighbour = true;
                DoTest(solver, test);
            }
        }

        static void TestDFDOptimized0()
        {
            var conf = ConfigurationParser.GetConfiguration("configuration.json");
            SolverDFDOptimized solver = new SolverDFDOptimized();

            for (int i = 0; i < conf.Count; i++)
            {
                var test = conf[i];
                test.model = "dfd+ls 0";
                test.zeroNeighbour = true;
                DoTest(solver, test);
            }
        }



        static void TestDFDOptimized1()
        {
            var conf = ConfigurationParser.GetConfiguration("configuration.json");
            SolverDFDOptimized solver = new SolverDFDOptimized();

            for (int i = 0; i < conf.Count; i++)
            {
                var test = conf[i];
                test.firstNeighbourhood = true;
                test.model = "dfd+ls 1";
                DoTest(solver, test);
            }
        }

        static void TestDFD()
        {
            var conf = ConfigurationParser.GetConfiguration("configuration.json");
            SolverDFD solver = new SolverDFD();
            for (int i = 0; i < conf.Count; i++)
            {
                var test = conf[i];
                test.model = "dfd";
                DoTest(solver, test);
            }
        }

        static void TestMTZ()
        {
            var conf = ConfigurationParser.GetConfiguration("configuration.json");
            SolverMTZ solver = new SolverMTZ();
            for (int i = 0; i < conf.Count; i++)
            {
                var test = conf[i];
                test.model = "mtz";
                DoTest(solver, test);
            }
        }


        static void DoTest(ISolver solver, TestConfiguration conf)
        {
            InputParser input = new InputParser(conf.testFile, conf.nTests, conf.directed);

            ResultAnalyses results = new ResultAnalyses(conf);

            int cnt = 1;
            foreach (var p in input)
            {
                Console.WriteLine("#running test " + cnt++);

                Stopwatch timeTracker = new Stopwatch();
                timeTracker.Start();

                var res = solver.SolveTest(p, conf);
                timeTracker.Stop();

                res.millisecondsElapsed = timeTracker.ElapsedMilliseconds;

                res.InitialZ = p.x;
                res.InitialW = p.y;
                results.addResult(res);

                /*  if (res.solutionExistance)
                  {
                      Console.WriteLine("                     Initial Cicle 1:" + String.Join(',', p.x.FindCycle()));
                      Console.WriteLine("                     Initial Cicle 2:" + String.Join(',', p.y.FindCycle()));

                      Console.WriteLine("                     Result Cicle 1:" + String.Join(',', res.z.FindCycle()));
                      Console.WriteLine("                     Result Cicle 2:" + String.Join(',', res.w.FindCycle()));
                  }
                  else
                  {
                      Console.WriteLine("No solution");
                  }
                */
            }

            results.printResults();
        }

    }
}
