using GraphDecomposer.DataStructures;
using GraphDecomposer.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphDecomposer.LocalSearch
{
    public class LocalSearchDirected : LocalSearchBase
    {

        public LocalSearchDirected(Graph z, Graph w, int attemptLimit, TestInput input, TestConfiguration conf) : base(z, w, attemptLimit, input, conf)
        {

        }

        private void Chain_Edge_Fixing_Directed(Graph z, Graph w, Edge e)
        {
            for (int i = 0; i < w.edgesFrom[e.from].Count; i++)
            {
                var edge = w.edgesFrom[e.from][i];
                if (fixed_edges[edge.Id] == 0)
                {
                    MoveEdge(w, z, edge);
                    Chain_Edge_Fixing_Directed(w, z, edge);
                }
            }

            for (int i = 0; i < w.edgesTo[e.to].Count; i++)
            {
                var edge = w.edgesTo[e.to][i];
                if (fixed_edges[edge.Id] == 0)
                {
                    MoveEdge(w, z, edge);
                    Chain_Edge_Fixing_Directed(w, z, edge);
                }
            }
        }

        public override bool DoLocalSearch()
        {
            List<Edge> edgesToTry = new List<Edge>();
            for (int i = 0; i < z.edges.Count; i++)
            {
                var e = z.edges[i];
                if (fixed_edges[e.Id] == 0)
                    edgesToTry.Add(z.edges[i]);
            }


            ArrayUtils.Shuffle(edgesToTry);
            for (int i = edgesToTry.Count - 1; i >= 0; i--)
            {
                var e = edgesToTry[i];

                MoveEdge(z, w, e);
                Chain_Edge_Fixing_Directed(z, w, e);

                var a = z.findSubCicles();
                var b = w.findSubCicles();

                if (a.Count + b.Count < before)
                {
                    if (a.Count + b.Count != 0 || checkOriginalCicles())
                    {
                        z.RestoreEdges();
                        w.RestoreEdges();
                        return true;
                    }
                }

              //  w = optimalW.Copy();
               // z = optimalZ.Copy();  doto some fix


                for (int j = 0; j < fixed_edges.Count; j++)
                    if (fixed_edges[j] != 2)
                        fixed_edges[j] = 0;
            }

            //return LocalSearchDirectedSecondNeighborhood();
            return false;
        }

        private void MoveEdge(Graph z, Graph w, Edge e)
        {
            fixed_edges[e.Id] = 1;
            z.Remove(e, false);
            w.Add(e, false);
        }

        private bool LocalSearchDirectedSecondNeighborhood()
        {
            List<Edge> edgesToTryZ = new List<Edge>();
            for (int i = 0; i < z.edges.Count; i++)
            {
                var e = z.edges[i];
                if (fixed_edges[e.Id] == 0)
                    edgesToTryZ.Add(z.edges[i]);
            }

            List<Edge> edgesToTryW = new List<Edge>();
            for (int i = 0; i < w.edges.Count; i++)
            {
                var e = w.edges[i];
                if (fixed_edges[e.Id] == 0)
                    edgesToTryW.Add(w.edges[i]);
            }


            ArrayUtils.Shuffle(edgesToTryZ);
            for (int i = 0; i < edgesToTryZ.Count; i++)
            {
                for (int j = 0; j < edgesToTryW.Count; j++)
                {

                    var e1 = edgesToTryZ[i];
                    var e2 = edgesToTryW[j];

                    MoveEdge(z, w, e1);

                    Chain_Edge_Fixing_Directed(z, w, e1);
                    MoveEdge(w, z, e2);
                    Chain_Edge_Fixing_Directed(w, z, e2);

                    var a = z.findSubCicles();
                    var b = w.findSubCicles();

                    if (a.Count + b.Count < before)
                    {
                        if (a.Count + b.Count != 0 || checkOriginalCicles())
                        {
                            z.RestoreEdges();
                            w.RestoreEdges();
                            return true;
                        }
                    }

                    w = optimalW.Copy();
                    z = optimalZ.Copy();

                    for (int g = 0; g < fixed_edges.Count; g++)
                        if (fixed_edges[g] != 2)
                            fixed_edges[g] = 0;
                }
            }

            return false;
        }

    }
}
