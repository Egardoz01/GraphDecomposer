using GraphDecomposer.DataStructures;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphDecomposer.Solvers
{
    class SolverDFDOptimized : SolverDFD
    {
        private Random r;
        public SolverDFDOptimized() : base()
        {
            r = new Random();
        }


        protected override SolverResult doIterations()
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

                for (int i = 1; i <= multiGraph.nEdges; i++)
                {
                    var v = variables[i];
                    if (v.X == 0)
                        z_Edges.Add(multiGraph.edges[i - 1]);
                    else
                        w_Edges.Add(multiGraph.edges[i - 1]);
                }

                Graph z = new Graph(multiGraph.nVertices, z_Edges, conf.directed);
                Graph w = new Graph(multiGraph.nVertices, w_Edges, conf.directed);

                bool repeat = true;


                while (repeat)
                {
                    var zSubCicles = z.findSubCicles();
                    var wSubCicles = w.findSubCicles();
                    if (zSubCicles.Count == 0 && wSubCicles.Count == 0)
                    {
                        var s1 = z.FindCycle();
                        var s2 = w.FindCycle();

                        var s3 = testInput.y.FindCycle();
                        var s4 = testInput.x.FindCycle();

                        if(s1!=s3 && s2!=s3 && s1!=s4 && s2!=s4)
                            return new SolverResult(iterationsCnt, true);// yes solution
                    }
                    foreach (var cicle in zSubCicles)
                        addCicleConstr(cicle);
                    foreach (var cicle in wSubCicles)
                        addCicleConstr(cicle);

                    if (conf.directed)
                        repeat = LocalSearchDirected(ref z, ref w);
                }

            }
        }


        List<int> fixed_edges;

        private void Chain_Edge_Fixing_Directed(Graph z, Graph w, Edge e)
        {
            fixed_edges[e.Id] = 1;
            z.Remove(e);
            w.Add(e);

            for (int i = 0; i < w.edgesFrom[e.from].Count; i++)
            {
                var edge = w.edgesFrom[e.from][i];
                if (fixed_edges[edge.Id] == 0)
                {
                    Chain_Edge_Fixing_Directed(w, z, edge);
                }
            }

            for (int i = 0; i < w.edgesTo[e.to].Count; i++)
            {
                var edge = w.edgesTo[e.to][i];
                if (fixed_edges[edge.Id] == 0)
                {
                    Chain_Edge_Fixing_Directed(w, z, edge);
                }
            }
        }

        private bool LocalSearchDirected(ref Graph z, ref Graph w)
        {

            fixed_edges = new List<int>();
            for (int i = 0; i <= multiGraph.nEdges; i++)
            {
                fixed_edges.Add(0);
            }

            foreach (Edge e in z.edges)
            {
                var same_edge = w.edges.FindAll(x => x.from == e.from && x.to == e.to);

                if (same_edge.Count > 0)
                {
                    fixed_edges[e.Id] = 2;
                    fixed_edges[same_edge[0].Id] = 2;
                }
            }
            Graph optimalZ = z.Copy();
            Graph optimalW = w.Copy();


            List<Edge> edgesToTry = new List<Edge>();
            for (int i = 0; i < z.edges.Count; i++)
            {
                var e = z.edges[i];
                if (fixed_edges[e.Id] == 0)
                    edgesToTry.Add(z.edges[i]);
            }


            bool repeat = true;
            while (repeat)
            {
                ShuffleArray(edgesToTry);
                repeat = false;
                for (int i = edgesToTry.Count - 1; i >= 0; i--)
                {
                    var e = edgesToTry[i];
                    edgesToTry.RemoveAt(i);

                    if (fixed_edges[e.Id] != 0)
                        continue;

                    repeat = true;
                    var c11 = z.findSubCicles();
                    var c21 = w.findSubCicles();
                    Chain_Edge_Fixing_Directed(z, w, e);

                    if (z.edges.Count != w.edges.Count)
                    {
                        throw new Exception("Bruh");
                    }


                    var c1 = z.findSubCicles();
                    var c2 = w.findSubCicles();

                
                    if (c1.Count + c2.Count < c11.Count + c21.Count)
                    {
                        return true;
                    }
                    else
                    {
                        w = optimalW.Copy();
                        z = optimalZ.Copy();
                    }

                    for (int j = 0; j < fixed_edges.Count; j++)
                        if (fixed_edges[j] != 2)
                            fixed_edges[j] = 0;

                }


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



    }
}
