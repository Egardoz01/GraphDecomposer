using GraphDecomposer.DataStructures;
using Gurobi;
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace GraphDecomposer.Solvers
{
    public class SolverMTZ_Directed : ISolver
    {
        private GRBEnv env;
        private Multigraph multiGraph;
        private GRBModel model;
        private List<GRBVar> variables;
        private List<GRBVar> alphaVars;
        private List<GRBVar> bettahVars;
        private TestConfiguration conf;
        int ciclesCnt = 0;
        private Stopwatch timer;
        public SolverMTZ_Directed()
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

            multiGraph = new Multigraph(input.x, input.y, conf.directed);

            addVariables();

            addVariablesConstr();

            addXGraphConstr();

            addYGraphConstr();

            addOrderConstrs();

            model.Parameters.TimeLimit = 10 * 60;
            model.Optimize();




            SolverResult res = new SolverResult();
            res.iterationsCnt = 1;
            res.solutionExistance = model.SolCount > 0;

            if (model.Status == 9)//Time_Limit
            {
                res.TimeLimit = true;
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
                if (i < 2 || j < 2)
                    continue;
                model.AddConstr(alphaVars[i] - alphaVars[j] + variables[edge.Id] * n <= n - 1, "constr al 1");

                model.AddConstr(bettahVars[i] - bettahVars[j] + (1 - variables[edge.Id]) * n <= n - 1, "constr al 1");

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

            alphaVars = new List<GRBVar>();
            alphaVars.Add(new GRBVar());
            alphaVars.Add(new GRBVar());
            bettahVars = new List<GRBVar>();
            bettahVars.Add(new GRBVar());
            bettahVars.Add(new GRBVar());

            int n = multiGraph.nVertices;
            for (int i = 2; i <= multiGraph.nVertices; i++)
            {
                alphaVars.Add(model.AddVar(2, n, 0.0, GRB.INTEGER, "alpha " + i));
                bettahVars.Add(model.AddVar(2, n, 0.0, GRB.INTEGER, "bettah " + i));
            }
        }

        private void addVariablesConstr()
        {
            for (int i = 1; i <= multiGraph.nVertices; i++)
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

        private void addXGraphConstr()
        {
            GRBLinExpr xExpr = new GRBLinExpr();
            foreach (var edge in multiGraph.xEdges)
            {
                xExpr.Add(1 * variables[edge.Id]);
            }
            model.AddConstr(xExpr <= multiGraph.xEdges.Count - 1, "X Edges constr");
        }

        private void addYGraphConstr()
        {
            GRBLinExpr yExpr = new GRBLinExpr();
            foreach (var edge in multiGraph.yEdges)
            {
                yExpr.Add(1 * variables[edge.Id]);
            }
            model.AddConstr(yExpr <= multiGraph.yEdges.Count - 1, "X Edges constr");
        }


    }
}
