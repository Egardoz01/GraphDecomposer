using GrubiTest.DataStructures;
using GrubiTest.Solvers;
using Gurobi;
using System;
using System.Collections.Generic;
using System.Text;

namespace GrubiTest
{
    public class SolverDFD : ISolver
    {
        private GRBEnv env;
        private Multigraph multiGraph;
        private GRBModel model;
        List<GRBVar> variables;

        int ciclesCnt = 0;
        public SolverDFD()
        {
            env = new GRBEnv(true);
            env.Set("LogFile", "mip1.log");
            env.OutputFlag = 0;
            env.Start();
        }



        public SolverResult Solve(ProblemInput input)
        {
            ciclesCnt = 0;

            model = new GRBModel(env);

            multiGraph = new Multigraph(input.x, input.y);

            addVariables();

            model.SetObjective(variables[0] + variables[1]);

            addVariablesConstr();

            addXGraphConstr();

            addYGraphConstr();


            var res = doIterations();

            /*foreach (var variable in variables)
            {
                Console.WriteLine(variable.VarName + ": " + variable.X);
            }*/

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
                    return new SolverResult(iterationsCnt, false);// yes solution

                List<Edge> z = new List<Edge>();
                List<Edge> w = new List<Edge>();

                foreach (var v in variables)
                {
                    //Console.WriteLine(v.VarName + ":  " + v.X);
                    int ind = int.Parse(v.VarName.Substring(1)) - 1;
                    if (v.X == 0)
                        z.Add(multiGraph.edges[ind]);
                    else
                        w.Add(multiGraph.edges[ind]);
                }

                var zSubCicles = findSubCicles(z);
                var wSubCicles = findSubCicles(w);
                if (zSubCicles.Count == 0 && wSubCicles.Count == 0)
                    return new SolverResult(iterationsCnt, true);// yes solution

                foreach (var cicle in zSubCicles)
                    addCicleConstr(cicle);
                foreach (var cicle in wSubCicles)
                    addCicleConstr(cicle);

            }
        }





        List<bool> usedEdges;
        List<bool> usedVertices;
        List<List<Edge>> g;
        List<Edge> currentCicle;
        private void dfs(int x)
        {
            usedVertices[x] = true;
            foreach (var edge in g[x])
            {
                int y;
                if (edge.from == x)
                {
                    y = edge.to;
                }
                else
                {
                    y = edge.from;
                }

                if (!usedEdges[edge.Id])
                {
                    usedEdges[edge.Id] = true;
                    currentCicle.Add(edge);
                    dfs(y);
                }

            }
        }

        private List<List<Edge>> findSubCicles(List<Edge> edges)
        {
            List<List<Edge>> cicles = new List<List<Edge>>();

            g = new List<List<Edge>>();
            for (int i = 0; i <= multiGraph.nVertices; i++)
                g.Add(new List<Edge>());
            foreach (var edge in edges)
            {
                g[edge.from].Add(edge);
                g[edge.to].Add(edge);
            }
            usedEdges = new List<bool>();
            usedVertices = new List<bool>();
            
            for (int i = 0; i <= multiGraph.nEdges; i++)
                usedEdges.Add(false);

            for (int i = 0; i <= multiGraph.nVertices; i++)
                usedVertices.Add(false);

            for (int i = 1; i <= multiGraph.nVertices; i++)
            {
                if (!usedVertices[i])
                {
                    currentCicle = new List<Edge>();
                    dfs(i);
                    if (currentCicle.Count < edges.Count)
                        cicles.Add(new List<Edge>(currentCicle));
                }
            }

            return cicles;
        }

        private void addVariables()
        {
            variables = new List<GRBVar>();

            foreach (var edge in multiGraph.edges)
            {
                variables.Add(model.AddVar(0.0, 1, 0.0, GRB.BINARY, "e" + edge.Id));
            }
        }

        private void addVariablesConstr()
        {
            for (int i = 1; i <= multiGraph.nVertices; i++)
            {
                GRBLinExpr expr = new GRBLinExpr();


                foreach (int id in multiGraph.adjacencyList[i])
                {
                    expr.Add(1 * variables[id - 1]);
                }

                model.AddConstr(expr == 2, $"V{i} constr");
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
                int index = edge.Id - 1;
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
                int index = edge.Id - 1;
                xExpr.Add(1 * variables[index]);
            }
            model.AddConstr(xExpr <= multiGraph.xEdges.Count - 1, "X Edges constr");
        }

        private void addYGraphConstr()
        {
            GRBLinExpr yExpr = new GRBLinExpr();
            foreach (var edge in multiGraph.yEdges)
            {
                int index = edge.Id - 1;
                yExpr.Add(1 * variables[index]);
            }
            model.AddConstr(yExpr <= multiGraph.yEdges.Count - 1, "X Edges constr");
        }




    }
}
