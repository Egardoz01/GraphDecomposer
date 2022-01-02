using GraphDecomposer.DataStructures;
using GraphDecomposer.Solvers;
using Gurobi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace GraphDecomposer
{
    public class SolverDFD : ISolver
    {
        private GRBEnv env;
        protected Multigraph multiGraph;
        protected GRBModel model;
        protected List<GRBVar> variables;
        protected TestConfiguration conf;
        protected TestInput testInput;
        int ciclesCnt = 0;
        protected Stopwatch timer;
        public SolverDFD()
        {

            env = new GRBEnv(true);
            env.Set("LogFile", "mip1.log");
            env.OutputFlag = 0;
            env.Start();
        }


        public SolverResult SolveTest(TestInput input, TestConfiguration conf, Stopwatch timer)
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


            var res = doIterations();

            model.Dispose();


            return res;
        }

        protected virtual SolverResult doIterations()
        {
            int iterationsCnt = 0;
            while (true)
            {
                iterationsCnt++;

                model.Optimize();

                double totalHours = timer.ElapsedMilliseconds / (1000 * 60 * 60);
                bool hasSol = model.SolCount > 0;
                if (!hasSol || totalHours >= 2)
                    return new SolverResult(iterationsCnt, false, null, null);// no solution


                List<Edge> z_Edges = new List<Edge>();
                List<Edge> w_Edges = new List<Edge>();

                for (int i=1; i<=multiGraph.nEdges; i++)
                {
                    var v = variables[i];
                    if (v.X == 0)
                        z_Edges.Add(multiGraph.edges[i-1]);
                    else
                        w_Edges.Add(multiGraph.edges[i-1]);
                }

                Graph z = new Graph(multiGraph.nVertices, z_Edges, conf.directed);
                Graph w = new Graph(multiGraph.nVertices, w_Edges, conf.directed);

          



                var zSubCicles = z.findSubCicles();
                var wSubCicles = w.findSubCicles();
                if (zSubCicles.Count == 0 && wSubCicles.Count == 0)
                    return new SolverResult(iterationsCnt, true,z,w);// yes solution

                foreach (var cicle in zSubCicles)
                    addCicleConstr(cicle);
                foreach (var cicle in wSubCicles)
                    addCicleConstr(cicle);


            }
        }


       
       

        private void addVariables()
        {
            variables = new List<GRBVar>();
            variables.Add(new GRBVar());
            foreach (var edge in multiGraph.edges)
            {
                variables.Add(model.AddVar(0.0, 1, 0.0, GRB.BINARY, "e" + edge.Id));
            }
        }

        private void addVariablesConstr()
        {
            for (int i = 1; i <= multiGraph.nVertices; i++)
            {
                if (!conf.directed)
                {
                    GRBLinExpr expr = new GRBLinExpr();

                    foreach (var edge in multiGraph.edgesFrom[i])
                    {
                        expr.Add(1 * variables[edge.Id]);
                    }

                    model.AddConstr(expr == 2, $"V{i} constr");
                }
                else
                {
                    GRBLinExpr expr1 = new GRBLinExpr();

                    foreach (var edge in multiGraph.edgesFrom[i])
                    {
                        expr1.Add(1 * variables[edge.Id]);
                    }

                    model.AddConstr(expr1 == 1, $"V{i} 1_constr");

                    GRBLinExpr expr2 = new GRBLinExpr();

                    foreach (var edge in multiGraph.edgesTo[i])
                    {
                        expr2.Add(1 * variables[edge.Id]);
                    }

                    model.AddConstr(expr2 == 1, $"V{i} 2_constr");
                }
            }
        }

        protected void addCicleConstr(List<Edge> cicle)
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
            model.AddConstr(expr >= E_s.Count - S.Count + 1, "cicle constr " + ciclesCnt++);
        }

        private void addXGraphConstr()
        {
            GRBLinExpr xExpr = new GRBLinExpr();
            foreach (var edge in multiGraph.xEdges)
            {
                int index = edge.Id;
                xExpr.Add(1 * variables[index]);
            }
            model.AddConstr(xExpr <= multiGraph.xEdges.Count - 1, "X Edges constr");
        }

        private void addYGraphConstr()
        {
            GRBLinExpr yExpr = new GRBLinExpr();
            foreach (var edge in multiGraph.yEdges)
            {
                int index = edge.Id;
                yExpr.Add(1 * variables[index]);
            }
            model.AddConstr(yExpr <= multiGraph.yEdges.Count - 1, "X Edges constr");
        }




    }
}
