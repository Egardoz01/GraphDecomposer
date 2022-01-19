﻿using GraphDecomposer.DataStructures;
using Gurobi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace GraphDecomposer.Solvers
{
    class SolverDFD_5_cycles : SolverDFD
    {
        int iterationsCnt;
        public SolverDFD_5_cycles() : base() { }

        public override SolverResult SolveTest(TestInput input, TestConfiguration conf, Stopwatch timer)
        {
            testInput = input;
            this.conf = conf;
            this.timer = timer;
            ciclesCnt = 0;

            model = new GRBModel(env);

            multiGraph = new Multigraph(input.x, input.y, conf.directed);

            addVariables();

            addVariablesConstr();

            addXGraphConstr();

            addYGraphConstr();

            iterationsCnt = 0;

           // model.Write("model_dfj_5_cycles_log.lp");

            Graph z=null, w=null, q=null;
            z = doIterations();
            if (z != null)
                w = doIterations();
            if (w != null)
                q = doIterations();

            model.Dispose();


            return new SolverResult(iterationsCnt, z, w, q);
        }


        private Graph doIterations()
        {
            while (true)
            {
                iterationsCnt++;

                model.Optimize();

                double totalseconds = timer.ElapsedMilliseconds / (1000);
                bool hasSol = model.SolCount > 0;
                if (!hasSol || conf.PackOfTestTimeout > 0 && totalseconds >= conf.PackOfTestTimeout)
                    return null;

                List<Edge> z_Edges = new List<Edge>();

                for (int i = 1; i <= multiGraph.nEdges; i++)
                {
                    var v = variables[i];
                    if (v.X == 1)
                        z_Edges.Add(multiGraph.edges[i - 1]);
                }

                Graph z = new Graph(multiGraph.nVertices, z_Edges, conf.directed);

                var zSubCicles = z.findSubCicles(true);

                foreach (var cicle in zSubCicles)
                    addCicleConstr(cicle);

                if (zSubCicles.Count == 1)
                    return z;// yes solution
            }
        }

        private new void addCicleConstr(List<Edge> cicle)
        {
            HashSet<int> S = new HashSet<int>();

            foreach (var edge in cicle)
            {
                S.Add(edge.from);
                S.Add(edge.to);
            }

            List<Edge> E_s = new List<Edge>();

            foreach (var edge in multiGraph.edges)
            {
                if (S.Contains(edge.from) && S.Contains(edge.to))
                    E_s.Add(edge);
            }

            GRBLinExpr expr = new GRBLinExpr();
            foreach (var edge in E_s)
            {
                int index = edge.Id;
                expr.AddTerm(1, variables[index]);
            }

            model.AddConstr(expr <= S.Count - 1, "cicle constr " + ciclesCnt++);
        }


    }
}
