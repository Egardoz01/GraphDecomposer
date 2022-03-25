using GraphDecomposer.DataStructures;
using GraphDecomposer.Parsers;
using GraphDecomposer.Results;
using GraphDecomposer.Solvers;
using Gurobi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace GraphDecomposer
{
    class Program
    {
        static void Main(string[] args)
        {
            // TestDFDOptimized12();
            var conf = ConfigurationParser.GetConfiguration("configuration.json");
            //var conf2 = ConfigurationParser.GetAllTestsConfiguration("allTests");
            DoConfigurationTesting(conf);
            // TestDFDOptimized3();
            // TestDFDOptimized1(conf);
            //TestDFDOptimized13_1(conf);
            //TestDFDOptimized13_2(conf);
            //TestDFDOptimized13_3(conf);
            //TestDFDOptimized13NoChainFix(conf);

            //TestDFD_LS_Directed(conf);
            //TestDFD_5Cycles(conf);
            // TestMTZ(conf2);
        }




        static void DoConfigurationTesting(List<TestConfiguration> conf)
        {
            SolverDFD solverDFD = new SolverDFD();
            SolverDFDOptimized solverDFD_LS = new SolverDFDOptimized();
            SolverMTZ solverMTZ = new SolverMTZ();

            SolverDFD_5_cycles solver5_Cycles = new SolverDFD_5_cycles();

            foreach (var test in conf)
            {
                if (test.runTest == false)
                    continue;
                if (test.model == "dfj")
                    DoTest_5_cycles(solverDFD, solver5_Cycles, test);
                else if (test.model == "dfj_ls")
                    DoTest_5_cycles(solverDFD_LS, solver5_Cycles, test);
                if (test.model == "mtz")
                    DoTest_5_cycles(solverMTZ, solver5_Cycles, test);
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
                test.zeroNeighbourhood = true;
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
                test.zeroNeighbourhood = true;
                test.secondNeighbour = true;
                DoTest(solver, test);
            }
        }

        static void TestDFDOptimized12()
        {
            var conf = ConfigurationParser.GetConfiguration("configuration.json");
            SolverDFDOptimized solver = new SolverDFDOptimized();

            for (int i = 0; i < conf.Count; i++)
            {
                var test = conf[i];
                test.model = "dfd+ls 1 2";
                test.firstNeighbourhood = true;
                test.secondNeighbour = true;
                DoTest(solver, test);
            }
        }

        static void TestDFDOptimized2()
        {
            var conf = ConfigurationParser.GetConfiguration("configuration.json");
            SolverDFDOptimized solver = new SolverDFDOptimized();

            for (int i = 0; i < conf.Count; i++)
            {
                var test = conf[i];
                test.model = "dfd+ls 2";
                test.secondNeighbour = true;
                DoTest(solver, test);
            }
        }

        static void TestDFDOptimized02()
        {
            var conf = ConfigurationParser.GetConfiguration("configuration.json");
            SolverDFDOptimized solver = new SolverDFDOptimized();

            for (int i = 0; i < conf.Count; i++)
            {
                var test = conf[i];
                test.model = "dfd+ls 0-2";
                test.zeroNeighbourhood = true;
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
                test.zeroNeighbourhood = true;
                DoTest(solver, test);
            }
        }



        static void TestDFDOptimized1(List<TestConfiguration> conf)
        {
            SolverDFDOptimized solver = new SolverDFDOptimized();

            for (int i = 0; i < conf.Count; i++)
            {
                var test = conf[i];
                test.firstNeighbourhood = true;
                test.model = "dfd+ls 1";
                if (test.nVertices > 3000)
                    DoTest(solver, test);
            }
        }


        static void TestDFDOptimized13_1(List<TestConfiguration> conf)
        {
            SolverDFDOptimized solver = new SolverDFDOptimized();

            for (int i = 0; i < conf.Count; i++)
            {
                var test = conf[i];
                test.recursionDepth = 7;
                test.thirdNeighborhoodType = 1;
                test.firstNeighbourhood = true;
                test.thirdNeighbourhood = true;
                test.model = "dfd+ls 1 3_1";
                DoTest(solver, test);
            }
        }

        static void TestDFDOptimized13_2(List<TestConfiguration> conf)
        {
            SolverDFDOptimized solver = new SolverDFDOptimized();

            for (int i = 0; i < conf.Count; i++)
            {
                var test = conf[i];
                test.recursionDepth = 5;
                test.thirdNeighborhoodType = 2;
                test.firstNeighbourhood = true;
                test.thirdNeighbourhood = true;
                test.model = "dfd+ls 1 3_2";
                DoTest(solver, test);
            }
        }

        static void TestDFDOptimized13_3(List<TestConfiguration> conf)
        {
            SolverDFDOptimized solver = new SolverDFDOptimized();

            for (int i = 0; i < conf.Count; i++)
            {
                var test = conf[i];
                test.recursionDepth = 7;
                test.thirdNeighborhoodType = 3;
                test.firstNeighbourhood = true;
                test.thirdNeighbourhood = true;
                test.model = "dfd+ls 1 3_3";
                DoTest(solver, test);
            }
        }

        static void TestDFDOptimized13NoChainFix(List<TestConfiguration> conf)
        {
            SolverDFDOptimized solver = new SolverDFDOptimized();

            for (int i = 0; i < conf.Count; i++)
            {

                var test = conf[i];
                test.recursionDepth = 7;
                test.firstNeighbourhood = true;
                test.thirdNeighborhoodType = 3;
                test.thirdNeighbourhood = true;
                test.noChainFix = true;
                test.model = "dfd+ls 1 3_3 no fix";
                if (test.nVertices > 1500)
                    DoTest(solver, test);
            }
        }

        static void TestDFDOptimized013()
        {
            var conf = ConfigurationParser.GetConfiguration("configuration.json");
            SolverDFDOptimized solver = new SolverDFDOptimized();

            for (int i = 0; i < conf.Count; i++)
            {
                var test = conf[i];
                test.zeroNeighbourhood = true;
                test.firstNeighbourhood = true;
                test.thirdNeighbourhood = true;
                test.model = "dfd+ls 0 1 3";
                DoTest(solver, test);
            }
        }

        static void TestDFDOptimized03()
        {
            var conf = ConfigurationParser.GetConfiguration("configuration.json");
            SolverDFDOptimized solver = new SolverDFDOptimized();

            for (int i = 0; i < conf.Count; i++)
            {
                var test = conf[i];
                test.zeroNeighbourhood = true;
                test.zeroNeighbourhood = true;
                test.thirdNeighbourhood = true;
                test.model = "dfd+ls 0 1 3";
                DoTest(solver, test);
            }
        }


        static void TestDFDOptimized3()
        {
            var conf = ConfigurationParser.GetConfiguration("configuration.json");
            SolverDFDOptimized solver = new SolverDFDOptimized();

            for (int i = 0; i < conf.Count; i++)
            {
                var test = conf[i];
                test.thirdNeighbourhood = true;
                test.recursionDepth = 7;
                test.model = "dfd+ls 3";
                DoTest(solver, test);
            }
        }

        static void TestDFDOptimized23()
        {
            var conf = ConfigurationParser.GetConfiguration("configuration.json");
            SolverDFDOptimized solver = new SolverDFDOptimized();

            for (int i = 0; i < conf.Count; i++)
            {
                var test = conf[i];
                test.secondNeighbour = true;
                test.thirdNeighbourhood = true;
                test.model = "dfd+ls 2 3";
                DoTest(solver, test);
            }
        }

        static void TestDFD(List<TestConfiguration> conf)
        {
            SolverDFD solver = new SolverDFD();
            for (int i = 0; i < conf.Count; i++)
            {
                var test = conf[i];
                test.model = "dfj";
                test.directed = false;
                DoTest(solver, test);
            }
        }


        static void TestDFD_5Cycles(List<TestConfiguration> conf)
        {
            SolverDFD solver1 = new SolverDFD();
            SolverDFD_5_cycles solver2 = new SolverDFD_5_cycles();
            for (int i = 0; i < conf.Count; i++)
            {
                var test = conf[i];
                test.model = "dfj";
                test.directed = true;
                DoTest_5_cycles(solver1, solver2, test);
            }
        }

        static void TestDFD_Directed(List<TestConfiguration> conf)
        {
            SolverDFD solver = new SolverDFD();
            for (int i = 0; i < conf.Count; i++)
            {
                var test = conf[i];
                test.model = "dfj";
                test.directed = true;
                if (!test.testFile.Contains("bi"))
                    DoTest(solver, test);
            }
        }


        static void TestDFD_LS_Directed(List<TestConfiguration> conf)
        {
            SolverDFDOptimized solver = new SolverDFDOptimized();
            for (int i = 0; i < conf.Count; i++)
            {
                var test = conf[i];
                test.model = "dfj+Ls";
                test.directed = true;
                if (!test.testFile.Contains("bi"))
                    DoTest(solver, test);
            }
        }

        static void DoTest(ISolver solver, TestConfiguration conf)
        {
            InputParser input = new InputParser(conf.testFile, conf.nTests, conf.directed);

            ResultAnalyses results = new ResultAnalyses(conf);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            int cnt = 1;
            Console.WriteLine(conf.model + (conf.directed ? " directed " : " undirected ") + conf.testFile);

            foreach (var p in input)
            {


                double totalSeconds = timer.ElapsedMilliseconds / (1000);
                if (conf.PackOfTestTimeout > 0)
                {
                    if (totalSeconds >= conf.PackOfTestTimeout)
                        break;
                }
                Console.WriteLine("#running test " + cnt++);

                Stopwatch timeTracker = new Stopwatch();
                timeTracker.Start();

                var res = solver.SolveTest(p, conf, timer);
                timeTracker.Stop();

                res.millisecondsElapsed = timeTracker.ElapsedMilliseconds;

                res.InitialZ = p.x;
                res.InitialW = p.y;
                results.addResult(res);


            }

            results.printResults();
        }

        static void DoTest_5_cycles(ISolver solver, ISolver solver5Cycles, TestConfiguration conf)
        {
            InputParser input = new InputParser(conf.testFile, conf.nTests, conf.directed);

            ResultAnalyses results = new ResultAnalyses(conf);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            int cnt = 1;
            Console.WriteLine(conf.model + (conf.directed ? " directed " : " undirected ") + conf.testFile);

            foreach (var p in input)
            {
                double totalSeconds = timer.ElapsedMilliseconds / (1000);
                if (conf.PackOfTestTimeout > 0)
                {
                    if (totalSeconds >= conf.PackOfTestTimeout)
                        break;
                }
                Console.WriteLine("#running test " + cnt++);

                Stopwatch timeTracker = new Stopwatch();
                timeTracker.Start();

                var res = solver.SolveTest(p, conf, timer);

                if (conf.FiveCyclesSearch)
                {
                    if (!res.solutionExistance)
                    {
                        res = solver5Cycles.SolveTest(p, conf, timer);
                    }
                }

                timeTracker.Stop();
                res.millisecondsElapsed = timeTracker.ElapsedMilliseconds;
                res.InitialZ = p.x;
                res.InitialW = p.y;

                results.addResult(res);
            }

            results.printResults();
        }


    }
}
