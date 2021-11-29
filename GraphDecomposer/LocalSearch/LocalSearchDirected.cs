using GraphDecomposer.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphDecomposer.LocalSearch
{
    public class LocalSearchDirected : LocalSearchBase
    {
        public LocalSearchDirected(Graph z, Graph w, int attemptLimit, TestInput input) : base(z, w, attemptLimit, input)
        {

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

        public override bool DoLocalSearch()
        {

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


            ArrayUtils.ShuffleArray(edgesToTry);
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
