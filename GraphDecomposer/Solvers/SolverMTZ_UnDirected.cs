using GraphDecomposer.DataStructures;
using Gurobi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace GraphDecomposer.Solvers
{
    public class SolverMTZ_UnDirected : ISolver
    {
        private GRBEnv env;
        private Multigraph multiGraph;
        private GRBModel model;
        private List<GRBVar> w_1;
        private List<GRBVar> w_2;
        private List<GRBVar> z_1;
        private List<GRBVar> z_2;
        private TestConfiguration conf;
        private List<GRBVar> alphaVars;
        private List<GRBVar> betaVars;
        int ciclesCnt = 0;
        private Stopwatch timer;
        public SolverMTZ_UnDirected()
        {
            env = new GRBEnv(true);
            env.Set("LogFile", "mip1.log");
            env.OutputFlag = 0;
            env.Start();
        }

        public SolverResult SolveTest(TestInput input, TestConfiguration conf, Stopwatch timer)
        {
            this.conf = conf;
            this.timer = timer;
            ciclesCnt = 0;

            model = new GRBModel(env);

            multiGraph = new Multigraph(input.x, input.y, true);

            addVariables();

            addVariablesConstr();

            addXGraphConstr();

            addYGraphConstr();

            addOrderConstrs();

            if (conf.SingleTestTimeout > 0)
                model.Parameters.TimeLimit = conf.SingleTestTimeout;

         //   model.Write("model_mtz_undirected_log.lp");

            model.Optimize();

            SolverResult res = new SolverResult();
            res.iterationsCnt = 1;
            res.solutionExistance = model.SolCount > 0;


            if (model.Status == 9)//Time_Limit
            {
                res.TimeLimit = true;
            }

            if (res.solutionExistance)
            {
                var ans = GetSolution(input);
                /*
                Console.WriteLine("X: " + String.Join(",", input.x.FindCycle()));
                Console.WriteLine("Y: " + String.Join(",", input.y.FindCycle()));
                Console.WriteLine("W: " + ans[0]);
                Console.WriteLine("Z: " + ans[1]);
                */
            }
            
            return res;
        }


        private string[] GetSolution(TestInput input)
        {

            var w_edges = new List<Edge>();

            var z_edges = new List<Edge>();



            for (int i = 1; i < w_1.Count; i++)
            {
                if (w_1[i].X == 1)
                    w_edges.Add(new Edge(i, multiGraph.edges[i - 1].from, multiGraph.edges[i - 1].to, this.conf.directed));

               // Console.WriteLine(w_1[i].VarName + ": " + w_1[i].X);
            }

            for (int i = 1; i < w_2.Count; i++)
            {
                if (w_2[i].X == 1)
                    w_edges.Add(new Edge(i, multiGraph.edges[i - 1].to, multiGraph.edges[i - 1].from, this.conf.directed));

               // Console.WriteLine(w_2[i].VarName + ": " + w_2[i].X);
            }


            for (int i = 1; i < z_1.Count; i++)
            {
                if (z_1[i].X == 1)
                    z_edges.Add(new Edge(i, multiGraph.edges[i - 1].from, multiGraph.edges[i - 1].to, this.conf.directed));

              //  Console.WriteLine(z_1[i].VarName + ": " + z_1[i].X);
            }

            for (int i = 1; i < z_2.Count; i++)
            {
                if (z_2[i].X == 1)
                    z_edges.Add(new Edge(i, multiGraph.edges[i - 1].to, multiGraph.edges[i - 1].from, this.conf.directed));


             //   Console.WriteLine(z_2[i].VarName + ": " + z_2[i].X);
            }

            var w = new Graph(multiGraph.nVertices, w_edges, false);
            var z = new Graph(multiGraph.nVertices, z_edges, false);



            List<int> be = new List<int>();
            for (int i = 1; i <= w.nVertices; i++)
            {
                be.Add((int)betaVars[i].X);

              //  Console.WriteLine(betaVars[i].VarName + ": " + betaVars[i].X);
            }

            List<int> al = new List<int>();
            for (int i = 1; i <= z.nVertices; i++)
            {
                al.Add((int)alphaVars[i].X);

              //  Console.WriteLine(alphaVars[i].VarName + ": " + alphaVars[i].X);
            }

            var res = new string[2];
            res[0] = String.Join(",", w.FindCycle());
            res[1] = String.Join(",", z.FindCycle());

            bool b1 = input.x.CheckEqualCicle(w);
            bool b2 = input.x.CheckEqualCicle(z);

            bool b3 = input.y.CheckEqualCicle(w);
            bool b4 = input.y.CheckEqualCicle(z);

            if (b1 || b2 || b3 || b4)
            {
                throw new Exception("Bruh");
            }

            return res;
        }


        private void addOrderConstrs()
        {
            int n = multiGraph.nVertices;
            foreach (var edge in multiGraph.edges)
            {

                int i = edge.from;
                int j = edge.to;
                if (j < 2 || i < 2)
                    continue;
                model.AddConstr(alphaVars[i] - alphaVars[j] + z_1[edge.Id] * n <= n - 1, "constr al 1");
                model.AddConstr(alphaVars[j] - alphaVars[i] + z_2[edge.Id] * n <= n - 1, "constr al 2");

                model.AddConstr(betaVars[i] - betaVars[j] + w_1[edge.Id] * n <= n - 1, "constr bet 1");
                model.AddConstr(betaVars[j] - betaVars[i] + w_2[edge.Id] * n <= n - 1, "constr bet 2");
            }
        }

        private void addVariables()
        {
            w_1 = new List<GRBVar>();
            w_2 = new List<GRBVar>();
            w_1.Add(new GRBVar());
            w_2.Add(new GRBVar());
            foreach (var edge in multiGraph.edges)
            {
                w_1.Add(model.AddVar(0.0, 1, 0.0, GRB.BINARY, "w1_" + edge.Id));
                w_2.Add(model.AddVar(0.0, 1, 0.0, GRB.BINARY, "w2_" + edge.Id));
            }

            z_1 = new List<GRBVar>();
            z_2 = new List<GRBVar>();
            z_1.Add(new GRBVar());
            z_2.Add(new GRBVar());
            foreach (var edge in multiGraph.edges)
            {
                z_1.Add(model.AddVar(0.0, 1, 0.0, GRB.BINARY, "z1_" + edge.Id));
                z_2.Add(model.AddVar(0.0, 1, 0.0, GRB.BINARY, "z2_" + edge.Id));
            }


            alphaVars = new List<GRBVar>();
            alphaVars.Add(new GRBVar());
            betaVars = new List<GRBVar>();
            betaVars.Add(new GRBVar());

            int n = multiGraph.nVertices;
            for (int i = 1; i <= multiGraph.nVertices; i++)
            {
                alphaVars.Add(model.AddVar(2, n, 0.0, GRB.INTEGER, "alpha " + i));
                betaVars.Add(model.AddVar(2, n, 0.0, GRB.INTEGER, "beta  " + i));
            }
        }

        private void addVariablesConstr()
        {
            for (int i = 1; i <= multiGraph.nVertices; i++)
            {
                GRBLinExpr expr1 = new GRBLinExpr();

                foreach (var edge in multiGraph.edgesFrom[i])
                {
                    expr1.Add(1 * w_1[edge.Id]);
                }

                foreach (var edge in multiGraph.edgesTo[i])
                {
                    expr1.Add(1 * w_2[edge.Id]);
                }

                model.AddConstr(expr1 == 1, $"V{i} w2_constr");

                GRBLinExpr expr2 = new GRBLinExpr();

                foreach (var edge in multiGraph.edgesTo[i])
                {
                    expr2.Add(1 * w_1[edge.Id]);
                }

                foreach (var edge in multiGraph.edgesFrom[i])
                {
                    expr2.Add(1 * w_2[edge.Id]);
                }

                model.AddConstr(expr2 == 1, $"V{i} w2_constr");

                GRBLinExpr expr3 = new GRBLinExpr();

                foreach (var edge in multiGraph.edgesFrom[i])
                {
                    expr3.Add(1 * z_1[edge.Id]);
                }

                foreach (var edge in multiGraph.edgesTo[i])
                {
                    expr3.Add(1 * z_2[edge.Id]);
                }

                model.AddConstr(expr3 == 1, $"V{i} z2_constr");


                GRBLinExpr expr4 = new GRBLinExpr();

                foreach (var edge in multiGraph.edgesTo[i])
                {
                    expr4.Add(1 * z_1[edge.Id]);
                }

                foreach (var edge in multiGraph.edgesFrom[i])
                {
                    expr4.Add(1 * z_2[edge.Id]);
                }

                model.AddConstr(expr4 == 1, $"V{i} z2_constr");
            }

            foreach (var ed in multiGraph.edges)
            {

                model.AddConstr(w_1[ed.Id] + z_1[ed.Id] + w_2[ed.Id] + z_2[ed.Id] == 1, $"Es{ed.Id} z+w_constr");
            }
        }

        private void addXGraphConstr()
        {
            GRBLinExpr xw1Expr = new GRBLinExpr();
            foreach (var edge in multiGraph.xEdges)
            {
                xw1Expr.Add(w_1[edge.Id]);
            }
            model.AddConstr(xw1Expr <= multiGraph.xEdges.Count - 1, "Xw1 Edges constr");


            GRBLinExpr xw2Expr = new GRBLinExpr();
            foreach (var edge in multiGraph.xEdges)
            {
                xw2Expr.Add(w_2[edge.Id]);
            }
            model.AddConstr(xw2Expr <= multiGraph.xEdges.Count - 1, "Xw2 Edges constr");


            GRBLinExpr xz1Expr = new GRBLinExpr();
            foreach (var edge in multiGraph.xEdges)
            {
                xz1Expr.Add(z_1[edge.Id]);
            }
            model.AddConstr(xz1Expr <= multiGraph.xEdges.Count - 1, "Xz1 Edges constr");

            GRBLinExpr xz2Expr = new GRBLinExpr();
            foreach (var edge in multiGraph.xEdges)
            {
                xz2Expr.Add(z_2[edge.Id]);
            }
            model.AddConstr(xz2Expr <= multiGraph.xEdges.Count - 1, "Xz2 Edges constr");

        }

        private void addYGraphConstr()
        {
            GRBLinExpr yw1Expr = new GRBLinExpr();
            foreach (var edge in multiGraph.yEdges)
            {
                yw1Expr.Add(w_1[edge.Id]);
            }
            model.AddConstr(yw1Expr <= multiGraph.yEdges.Count - 1, "Yw1 Edges constr");

            GRBLinExpr yw2Expr = new GRBLinExpr();
            foreach (var edge in multiGraph.yEdges)
            {
                yw2Expr.Add(w_2[edge.Id]);
            }
            model.AddConstr(yw2Expr <= multiGraph.yEdges.Count - 1, "Yw2 Edges constr");

            GRBLinExpr yz1Expr = new GRBLinExpr();
            foreach (var edge in multiGraph.yEdges)
            {
                yz1Expr.Add(z_1[edge.Id]);
            }
            model.AddConstr(yz1Expr <= multiGraph.yEdges.Count - 1, "Yz1 Edges constr");

            GRBLinExpr yz2Expr = new GRBLinExpr();
            foreach (var edge in multiGraph.yEdges)
            {
                yz2Expr.Add(z_2[edge.Id]);
            }
            model.AddConstr(yz2Expr <= multiGraph.yEdges.Count - 1, "Yz2 Edges constr");
        }
    }
}
