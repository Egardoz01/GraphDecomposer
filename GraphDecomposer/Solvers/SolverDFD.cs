using GraphDecomposer.DataStructures;
using GraphDecomposer.Solvers;
using Gurobi;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphDecomposer
{
    public class SolverDFD : ISolver
    {
        private GRBEnv env;
        private Multigraph multiGraph;
        private GRBModel model;
        private List<GRBVar> variables;
        private TestConfiguration conf;
        int ciclesCnt = 0;
        private Random r;
        public SolverDFD()
        {
            r = new Random();
            env = new GRBEnv(true);
            env.Set("LogFile", "mip1.log");
            env.OutputFlag = 0;
            env.Start();
        }


        public SolverResult SolveTest(TestInput input, TestConfiguration conf)
        {

            this.conf = conf;

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

        private SolverResult doIterations()
        {
            int iterationsCnt = 0;
            while (true)
            {
                iterationsCnt++;

                model.Optimize();

                bool hasSol = model.SolCount > 0;

                if (!hasSol)
                    return new SolverResult(iterationsCnt, false);// no solution

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

                bool repeat = true;


                // while (repeat)
                // {
                var zSubCicles = z.findSubCicles();
                var wSubCicles = w.findSubCicles();
                if (zSubCicles.Count == 0 && wSubCicles.Count == 0)
                    return new SolverResult(iterationsCnt, true);// yes solution

                foreach (var cicle in zSubCicles)
                    addCicleConstr(cicle);
                foreach (var cicle in wSubCicles)
                    addCicleConstr(cicle);

                //  if (conf.directed)
                //  repeat = LocalSearchDirected(z, w);
                // }

            }
        }
        List<int> colors;

        private void Chain_Edge_Fixing_Directed(Graph z, Graph w, Edge e)
        {
            colors[e.Id] = 1;
            z.Remove(e);
            w.Add(e);

            foreach (var edge in w.edgesFrom[e.from])
            {
                if (colors[edge.Id] != 0)
                {
                    Chain_Edge_Fixing_Directed(w, z, e);
                }
            }

            foreach (var edge in w.edgesTo[e.to])
            {
                if (colors[edge.Id] != 0)
                {
                    Chain_Edge_Fixing_Directed(w, z, e);
                }
            }



        }

        private bool LocalSearchDirected(Graph z, Graph w)
        {

            colors = new List<int>();//0 1->z 2->w
            List<bool> checked_edges = new List<bool>();
            for (int i = 1; i <= multiGraph.nEdges; i++)
                colors.Add(0);

            foreach (Edge e in z.edges)
            {
                var same_edge = w.edges.FindAll(x => x.from == e.from && x.to == e.to);

                if (same_edge.Count > 0)
                {
                    colors[e.Id] = 2;
                    colors[same_edge[0].Id] = 2;
                }
            }
            Graph optimalZ = z.Copy();
            Graph optimalW = w.Copy();

            bool repeat = true;
            while (repeat)
            {
                ShuffleArray(z.edges);
                repeat = false;
                foreach (Edge e in z.edges)
                {
                    if (colors[e.Id] != 0 && !checked_edges[e.Id])
                        continue;
                    checked_edges[e.Id] = true;
                    repeat = true;
                    var c11 = z.findSubCicles();
                    var c21 = z.findSubCicles();

                    Chain_Edge_Fixing_Directed(z, w, e);

                    var c1 = z.findSubCicles();
                    var c2 = z.findSubCicles();
                    if (c1.Count + c2.Count < c11.Count + c21.Count)
                    {
                        return true;
                    }
                }

                for (int i = 0; i < colors.Count; i++)
                    if (colors[i] != 2)
                        colors[i] = 1;
            }

            return false;
        }

        private void ShuffleArray(List<Edge> z)
        {
            for (int i = 0; i < z.Count; i++)
            {
                int j = r.Next(0, z.Count - 1);
                Edge tmp = z[i];
                z[i] = z[j];
                z[j] = tmp;
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

        private void addCicleConstr(List<Edge> cicle)
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
