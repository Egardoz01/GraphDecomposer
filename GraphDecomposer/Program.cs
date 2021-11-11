﻿using GraphDecomposer.DataStructures;
using GraphDecomposer.Parsers;
using GraphDecomposer.Solvers;
using Gurobi;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GraphDecomposer
{
    class Program
    {
        static void Main(string[] args)
        {
           // TestDFD();
            TestMTZ();
        }


        static void TestDFD()
        {
            var conf = ConfigurationParser.GetConfiguration("configuration.json");
            SolverDFD solver = new SolverDFD();
            foreach (var test in conf)
            {
                DoTest(solver, test);
            }
        }

        static void TestMTZ()
        {
            var conf = ConfigurationParser.GetConfiguration("configuration.json");
            SolverMTZ solver = new SolverMTZ();
            foreach (var test in conf)
            {
                DoTest(solver, test);
            }
        }


        static void DoTest(ISolver solver, TestConfiguration conf)
        {
            InputParser input = new InputParser(conf.testFile, conf.nTests);

            ResultAnalyses results = new ResultAnalyses(conf);

            foreach (var p in input)
            {
                Stopwatch timeTracker = new Stopwatch();
                timeTracker.Start();
      
                var res = solver.SolveTest(p, conf);
                timeTracker.Stop();

                res.millisecondsElapsed = timeTracker.ElapsedMilliseconds;

                results.addResult(res);
            }

            results.printResults();
        }

    }
}
