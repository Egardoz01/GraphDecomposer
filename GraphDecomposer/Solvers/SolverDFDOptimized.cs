using GraphDecomposer.DataStructures;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphDecomposer.Solvers
{
    public class SolverDFDOptimized : SolverDFD
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

                int cntRepeat = 0;
                while (repeat)
                {
                    cntRepeat++;

                    var zSubCicles = z.findSubCicles();
                    var wSubCicles = w.findSubCicles();

                    foreach (var cicle in zSubCicles)
                        addCicleConstr(cicle);
                    foreach (var cicle in wSubCicles)
                        addCicleConstr(cicle);

                    if (zSubCicles.Count == 0 && wSubCicles.Count == 0)
                    {
                        var s1 = z.FindCycle();
                        var s2 = w.FindCycle();

                        var s3 = testInput.y.FindCycle();
                        var s4 = testInput.x.FindCycle();

                        if (s1 != s3 && s2 != s3 && s1 != s4 && s2 != s4)
                            return new SolverResult(iterationsCnt, true);// yes solution
                        else
                            break;
                    }

                    if (conf.directed)
                        repeat = LocalSearchDirected(ref z, ref w);
                    else
                        repeat = LocalSearchUnDirected(ref z, ref w, 20);
                }

                int a = cntRepeat;

            }
        }


        List<int> fixed_edges;



        private void MoveEdge(Graph z, Graph w, Edge e)
        {
            z.Remove(e, false);
            w.Add(e, false);
        }

        private void Chain_Edge_Fixing_UnDirected(Graph z, Graph w, Edge e)
        {
            fixed_edges[e.Id] = 1;

            int i = e.from;
            int j = e.to;

            Chain_Fo_vertex(z, w, i);
            Chain_Fo_vertex(z, w, j);

        }

        private void Chain_Fo_vertex(Graph z, Graph w, int i)
        {
            int cntFixed = 0;

            foreach (var edge in w.edgesFrom[i])
                if (fixed_edges[edge.Id] != 0)
                    cntFixed++;

            foreach (var edge in w.edgesTo[i])
                if (fixed_edges[edge.Id] != 0)
                    cntFixed++;

            if (cntFixed == 2)
            {
                for (int ind = 0; ind < w.edgesFrom[i].Count; ind++)
                {
                    var edge = w.edgesFrom[i][ind];
                    if (fixed_edges[edge.Id] == 0)
                    {
                        MoveEdge(w, z, edge);
                        Chain_Edge_Fixing_UnDirected(w, z, edge);
                    }
                }

                for (int ind = 0; ind < w.edgesTo[i].Count; ind++)
                {
                    var edge = w.edgesTo[i][ind];
                    if (fixed_edges[edge.Id] == 0)
                    {
                        MoveEdge(w, z, edge);
                        Chain_Edge_Fixing_UnDirected(w, z, edge);
                    }
                }

                for (int ind = 0; ind < z.edgesFrom[i].Count; ind++)
                {
                    var edge = z.edgesFrom[i][ind];
                    if (fixed_edges[edge.Id] == 0)
                    {
                        Chain_Edge_Fixing_UnDirected(w, z, edge);
                    }
                }

                for (int ind = 0; ind < z.edgesTo[i].Count; ind++)
                {
                    var edge = z.edgesTo[i][ind];
                    if (fixed_edges[edge.Id] == 0)
                    {
                        Chain_Edge_Fixing_UnDirected(w, z, edge);
                    }
                }

            }
        }


        private T GetRandomElement<T>(List<T> list)
        {
            int ind = r.Next(0, list.Count - 1);

            return list[ind];
        }

        private bool LocalSearchUnDirected(ref Graph z, ref Graph w, int attemptLimit)
        {
            fixed_edges = new List<int>();
            for (int i = 0; i <= multiGraph.nEdges; i++)
            {
                fixed_edges.Add(0);
            }

            int cntFIxed = 0;
            foreach (Edge e in z.edges)
            {
                var same_edge = w.edges.FindAll(x => x.GetHashCode() == e.GetHashCode());

                if (same_edge.Count > 0)
                {
                    cntFIxed++;
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

            var c11 = z.findSubCicles();
            var c21 = w.findSubCicles();
            int before = c11.Count + c21.Count;

            ShuffleArray(edgesToTry);
            int cntEdgesTested = 0;
            for (int i = edgesToTry.Count - 1; i >= 0; i--)
            {
                var e = edgesToTry[i];

                cntEdgesTested++;

                while (attemptLimit-- > 0)
                {
                    MoveEdge(z, w, e);
                    Chain_Edge_Fixing_UnDirected(z, w, e);

                    while (true)
                    {
                        List<int> verticses = new List<int>();
                        for (int vertexI = 1; vertexI <= z.nVertices; vertexI++)
                        {
                            if (z.edgesFrom[vertexI].Count + z.edgesTo[vertexI].Count != 2)
                            {
                                verticses.Add(vertexI);
                            }
                        }
                        if (verticses.Count == 0)
                            break;


                        var vertex = GetRandomElement(verticses);

                        if (z.edgesFrom[vertex].Count + z.edgesTo[vertex].Count == 1)
                        {
                            var edges = new List<Edge>();

                            foreach (var edge in w.edgesFrom[vertex])
                                if (fixed_edges[edge.Id] == 0)
                                    edges.Add(edge);
                            foreach (var edge in w.edgesTo[vertex])
                                if (fixed_edges[edge.Id] == 0)
                                    edges.Add(edge);

                            if (edges.Count != 0)
                            {
                                var ed = GetRandomElement(edges);
                                MoveEdge(w, z, ed);
                                Chain_Edge_Fixing_UnDirected(w, z, ed);
                            }
                        }

                        if (z.edgesFrom[vertex].Count + z.edgesTo[vertex].Count == 3)
                        {
                            var edges = new List<Edge>();

                            foreach (var edge in z.edgesFrom[vertex])
                                if (fixed_edges[edge.Id] == 0)
                                    edges.Add(edge);
                            foreach (var edge in z.edgesTo[vertex])
                                if (fixed_edges[edge.Id] == 0)
                                    edges.Add(edge);

                            if (edges.Count != 0)
                            {
                                var ed = GetRandomElement(edges);
                                MoveEdge(z, w, ed);
                                Chain_Edge_Fixing_UnDirected(z, w, ed);
                            }
                        }

                    }

                    var a = z.findSubCicles();
                    var b = w.findSubCicles();
                    if (a.Count + b.Count < before)
                    {
                        z.RestoreEdges();
                        w.RestoreEdges();
                        return true;
                    }




                    w = optimalW.Copy();
                    z = optimalZ.Copy();

                    for (int j = 0; j < fixed_edges.Count; j++)
                        if (fixed_edges[j] != 2)
                            fixed_edges[j] = 0;
                }
            }

            return false;
        }




        private void ShuffleArray<T>(List<T> z)
        {
            for (int i = 0; i < z.Count; i++)
            {
                int j = r.Next(0, z.Count - 1);
                T tmp = z[i];
                z[i] = z[j];
                z[j] = tmp;
            }
        }

        private void Chain_Edge_Fixing_Directed(Graph z, Graph w, Edge e)
        {
            fixed_edges[e.Id] = 1;
            z.Remove(e, false);
            w.Add(e, false);

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

            var c11 = z.findSubCicles();
            var c21 = w.findSubCicles();
            int before = c11.Count + c21.Count;


            ShuffleArray(edgesToTry);
            for (int i = edgesToTry.Count - 1; i >= 0; i--)
            {
                var e = edgesToTry[i];

                if (fixed_edges[e.Id] != 0)
                    continue;

                Chain_Edge_Fixing_Directed(z, w, e);

                var c1 = z.findSubCicles();
                var c2 = w.findSubCicles();

                if (c1.Count + c2.Count < before)
                {
                    w.RestoreEdges();
                    z.RestoreEdges();
                    before = c1.Count + c2.Count;
                    optimalW = w.Copy();
                    optimalZ = z.Copy();
                    return true;
                }

                w = optimalW.Copy();
                z = optimalZ.Copy();


                for (int j = 0; j < fixed_edges.Count; j++)
                    if (fixed_edges[j] != 2)
                        fixed_edges[j] = 0;
            }

            return false;
        }

    }
}
