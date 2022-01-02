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


                BackToOriginal();

            }

            return false;
        }

        private void MoveEdge(Graph z, Graph w, Edge e, bool countMovements = true, bool countBroken = true)
        {
            fixed_edges[e.Id] = 1;
            z.Remove(e, false);
            w.Add(e, false);


            if (countBroken)
            {
                Graph g = z.GraphId == 1 ? z : w;
                if (g.edgesFrom[e.from].Count + g.edgesTo[e.from].Count != 2)
                {
                    if (!brokenVerticses.Contains(e.from))
                        brokenVerticses.Add(e.from);
                }
                else
                    brokenVerticses.Remove(e.from);

                if (g.edgesFrom[e.to].Count + g.edgesTo[e.to].Count != 2)
                {
                    if (!brokenVerticses.Contains(e.to))
                        brokenVerticses.Add(e.to);
                }
                else
                    brokenVerticses.Remove(e.to);

            }

            if (countMovements)
            {
                if (z.GraphId == 1)
                    movedFromZ.Add(e);
                else
                    movedFromW.Add(e);
            }

        }

        private void BackToOriginal()
        {
            foreach (var e in movedFromZ)
            {
                MoveEdge(w, z, e, false, false);
                fixed_edges[e.Id] = 0;
            }
            foreach (var e in movedFromW)
            {
                MoveEdge(z, w, e, false, false);
                fixed_edges[e.Id] = 0;
            }

            movedFromZ.Clear();
            movedFromW.Clear();
            brokenVerticses.Clear();
        }
    }
}
