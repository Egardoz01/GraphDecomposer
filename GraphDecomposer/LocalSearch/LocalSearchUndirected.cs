using GraphDecomposer.DataStructures;
using GraphDecomposer.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphDecomposer.LocalSearch
{
    public class LocalSearchUndirected : LocalSearchBase
    {


        public LocalSearchUndirected(Graph z, Graph w, int attemptLimit, TestInput input, TestConfiguration conf) : base(z, w, attemptLimit, input, conf)
        {
        }

        public override bool DoLocalSearch()
        {
            if (conf.zeroNeighbour)
                return ZeroNeighborhood();
            else if (conf.firstNeighbourhood)
                return FirstNeighborhood();
            else if (conf.secondNeighbour)
                return SecondNeighborhood();
            else if (conf.thirdNeighbourhood)
                return ThirdNeighborhood();

            return false;
        }

        private bool ZeroNeighborhood()
        {

            List<Edge> edgesToTry = new List<Edge>();
            for (int i = 0; i < z.edges.Count; i++)
            {
                var e = z.edges[i];
                if (fixed_edges[e.Id] != 2)
                    edgesToTry.Add(z.edges[i]);
            }

            ArrayUtils.Shuffle(edgesToTry);

            for (int i = 0; i < edgesToTry.Count; i++)
            {
                var e = edgesToTry[i];

                int A = e.from;
                int B = e.to;

                List<int> incidentToAInW = w.GetIncidentVerteces(A);
                List<int> incidentToBInW = w.GetIncidentVerteces(B);

                foreach (var C in incidentToAInW)
                {
                    foreach (var D in incidentToBInW)
                    {
                        var CD = z.GetEdgeBetween(C, D);

                        if (!CD.HasValue)
                            continue;

                        var AC = w.GetEdgeBetween(A, C);
                        var BD = w.GetEdgeBetween(B, D);

                        if (fixed_edges[CD.Value.Id] == 2 || fixed_edges[AC.Value.Id] == 2 || fixed_edges[BD.Value.Id] == 2)
                        {
                            continue;
                        }

                        MoveEdge(z, w, e);
                        MoveEdge(z, w, CD.Value);
                        MoveEdge(w, z, AC.Value);
                        MoveEdge(w, z, BD.Value);

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
                }
            }

            if (brokenVerticses.Count != 0)
            {
                throw new Exception("Bruh");
            }

            z.RestoreEdges();
            w.RestoreEdges();

            if (conf.firstNeighbourhood)
                return FirstNeighborhood();
            if (conf.secondNeighbour)
                return FirstNeighborhood();
            else if (conf.thirdNeighbourhood)
                return ThirdNeighborhood();
            return false;
        }

        private bool FirstNeighborhood()
        {
            List<Edge> edgesToTry = new List<Edge>();
            for (int i = 0; i < z.edges.Count; i++)
            {
                var e = z.edges[i];
                if (fixed_edges[e.Id] == 0)
                    edgesToTry.Add(z.edges[i]);
            }

            ArrayUtils.Shuffle(edgesToTry);
            for (int i = 0; i < edgesToTry.Count; i++)
            {
                var e = edgesToTry[i];
                int attempts = attemptLimit;
                while (attempts-- > 0)
                {
                    MoveEdge(z, w, e);
                    Chain_Edge_Fixing_UnDirected(z, w, e);

                    bool res = TryToFix();

                    if (res)
                        return true;
                }
            }

            if (conf.secondNeighbour)
                return SecondNeighborhood();
            else if (conf.thirdNeighbourhood)
                return ThirdNeighborhood();

            return false;
        }


        private bool TryToFix()
        {
            while (brokenVerticses.Count != 0)
            {
                if (brokenVerticses.Count == 1)
                {
                    if (!conf.noChainFix)
                        throw new Exception("Bruh");
                }

                var vertex = ArrayUtils.GetRandomElement(brokenVerticses);

                if (z.edgesFrom[vertex].Count + z.edgesTo[vertex].Count <= 1)
                {
                    var edges = new List<Edge>();

                    foreach (var x in brokenVerticses)
                    {
                        if (x != vertex && z.edgesFrom[x].Count + z.edgesTo[x].Count <= 1)
                        {
                            foreach (var edge in w.edgesFrom[vertex])
                                if (fixed_edges[edge.Id] == 0 && edge.to == x)
                                    edges.Add(edge);
                            foreach (var edge in w.edgesTo[vertex])
                                if (fixed_edges[edge.Id] == 0 && edge.from == x)
                                    edges.Add(edge);
                        }
                    }

                    if (edges.Count == 0)
                    {
                        foreach (var x in brokenVerticses)
                        {
                            if (x != vertex && z.edgesFrom[x].Count + z.edgesTo[x].Count >= 3)
                            {
                                foreach (var edge in w.edgesFrom[vertex])
                                    if (fixed_edges[edge.Id] == 0 && edge.to == x)
                                        edges.Add(edge);
                                foreach (var edge in w.edgesTo[vertex])
                                    if (fixed_edges[edge.Id] == 0 && edge.from == x)
                                        edges.Add(edge);
                            }
                        }
                    }

                    if (edges.Count == 0)
                    {
                        foreach (var edge in w.edgesFrom[vertex])
                            if (fixed_edges[edge.Id] == 0)
                                edges.Add(edge);
                        foreach (var edge in w.edgesTo[vertex])
                            if (fixed_edges[edge.Id] == 0)
                                edges.Add(edge);
                    }

                    if (edges.Count != 0)
                    {
                        var ed = ArrayUtils.GetRandomElement(edges);
                        MoveEdge(w, z, ed);
                        Chain_Edge_Fixing_UnDirected(w, z, ed);
                    }
                    else
                    {
                        break;
                    }
                }

                if (z.edgesFrom[vertex].Count + z.edgesTo[vertex].Count >= 3)
                {
                    var edges = new List<Edge>();

                    foreach (var x in brokenVerticses)
                    {
                        if (x != vertex && z.edgesFrom[x].Count + z.edgesTo[x].Count >= 3)
                        {
                            foreach (var edge in z.edgesFrom[vertex])
                                if (fixed_edges[edge.Id] == 0 && edge.to == x)
                                    edges.Add(edge);
                            foreach (var edge in z.edgesTo[vertex])
                                if (fixed_edges[edge.Id] == 0 && edge.from == x)
                                    edges.Add(edge);
                        }
                    }


                    if (edges.Count == 0)
                    {
                        foreach (var x in brokenVerticses)
                        {
                            if (x != vertex && z.edgesFrom[x].Count + z.edgesTo[x].Count <= 1)
                            {
                                foreach (var edge in z.edgesFrom[vertex])
                                    if (fixed_edges[edge.Id] == 0 && edge.to == x)
                                        edges.Add(edge);
                                foreach (var edge in z.edgesTo[vertex])
                                    if (fixed_edges[edge.Id] == 0 && edge.from == x)
                                        edges.Add(edge);
                            }
                        }
                    }


                    if (edges.Count == 0)
                    {
                        foreach (var edge in z.edgesFrom[vertex])
                            if (fixed_edges[edge.Id] == 0)
                                edges.Add(edge);
                        foreach (var edge in z.edgesTo[vertex])
                            if (fixed_edges[edge.Id] == 0)
                                edges.Add(edge);
                    }

                    if (edges.Count != 0)
                    {
                        var ed = ArrayUtils.GetRandomElement(edges);
                        MoveEdge(z, w, ed);
                        Chain_Edge_Fixing_UnDirected(z, w, ed);
                    }
                    else
                    {
                        break;
                    }
                }

            }


            if (brokenVerticses.Count == 0)
            {
                if (z.nEdges != w.nEdges)
                {
                    throw new Exception("Bruh");
                }

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
            }

            BackToOriginal();

            return false;
        }


        private bool TryToFixRecursive(int depth)
        {
            if (depth >= conf.recursionDepth)
                return false;


            if (brokenVerticses.Count == 1)
            {
                /*List<int> reallyBroken = new List<int>();
                for (int i = 1; i <= z.nVertices; i++)
                {
                    if (z.edgesFrom[i].Count + z.edgesTo[i].Count != 2)
                    {
                        reallyBroken.Add(i);
                    }
                }*/

                if (!conf.noChainFix)
                    throw new Exception("Bruh");
            }

            var curBroken = new List<int>(brokenVerticses);

            int restZ = movedFromZ.Count;
            int restW = movedFromW.Count;

            foreach (var vertex in curBroken)
            {

                if (z.edgesFrom[vertex].Count + z.edgesTo[vertex].Count <= 1)
                {
                    var edges = new List<Edge>();

                    foreach (var x in brokenVerticses)
                    {
                        if (x != vertex && z.edgesFrom[x].Count + z.edgesTo[x].Count <= 1)
                        {
                            foreach (var edge in w.edgesFrom[vertex])
                                if (fixed_edges[edge.Id] == 0 && edge.to == x)
                                    edges.Add(edge);
                            foreach (var edge in w.edgesTo[vertex])
                                if (fixed_edges[edge.Id] == 0 && edge.from == x)
                                    edges.Add(edge);
                        }
                    }

                    if (edges.Count == 0)
                    {
                        foreach (var x in brokenVerticses)
                        {
                            if (x != vertex && z.edgesFrom[x].Count + z.edgesTo[x].Count >= 3)
                            {
                                foreach (var edge in w.edgesFrom[vertex])
                                    if (fixed_edges[edge.Id] == 0 && edge.to == x)
                                        edges.Add(edge);
                                foreach (var edge in w.edgesTo[vertex])
                                    if (fixed_edges[edge.Id] == 0 && edge.from == x)
                                        edges.Add(edge);
                            }
                        }
                    }

                    if (edges.Count == 0)
                    {
                        foreach (var edge in w.edgesFrom[vertex])
                            if (fixed_edges[edge.Id] == 0)
                                edges.Add(edge);
                        foreach (var edge in w.edgesTo[vertex])
                            if (fixed_edges[edge.Id] == 0)
                                edges.Add(edge);
                    }

                    if (edges.Count != 0)
                    {
                        var ed = ArrayUtils.GetRandomElement(edges);
                        MoveEdge(w, z, ed);
                        Chain_Edge_Fixing_UnDirected(w, z, ed);
                    }
                    else
                    {
                        break;
                    }
                }

                if (z.edgesFrom[vertex].Count + z.edgesTo[vertex].Count >= 3)
                {
                    var edges = new List<Edge>();

                    foreach (var x in brokenVerticses)
                    {
                        if (x != vertex && z.edgesFrom[x].Count + z.edgesTo[x].Count >= 3)
                        {
                            foreach (var edge in z.edgesFrom[vertex])
                                if (fixed_edges[edge.Id] == 0 && edge.to == x)
                                    edges.Add(edge);
                            foreach (var edge in z.edgesTo[vertex])
                                if (fixed_edges[edge.Id] == 0 && edge.from == x)
                                    edges.Add(edge);
                        }
                    }


                    if (edges.Count == 0)
                    {
                        foreach (var x in brokenVerticses)
                        {
                            if (x != vertex && z.edgesFrom[x].Count + z.edgesTo[x].Count <= 1)
                            {
                                foreach (var edge in z.edgesFrom[vertex])
                                    if (fixed_edges[edge.Id] == 0 && edge.to == x)
                                        edges.Add(edge);
                                foreach (var edge in z.edgesTo[vertex])
                                    if (fixed_edges[edge.Id] == 0 && edge.from == x)
                                        edges.Add(edge);
                            }
                        }
                    }


                    if (edges.Count == 0)
                    {
                        foreach (var edge in z.edgesFrom[vertex])
                            if (fixed_edges[edge.Id] == 0)
                                edges.Add(edge);
                        foreach (var edge in z.edgesTo[vertex])
                            if (fixed_edges[edge.Id] == 0)
                                edges.Add(edge);
                    }

                    if (edges.Count != 0)
                    {
                        var ed = ArrayUtils.GetRandomElement(edges);
                        MoveEdge(z, w, ed);
                        Chain_Edge_Fixing_UnDirected(z, w, ed);
                    }
                    else
                    {
                        break;
                    }
                }

                if (brokenVerticses.Count == 0)
                {
                    if (z.nEdges != w.nEdges)
                    {
                        throw new Exception("Bruh");
                    }

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

                    return false;
                }

                bool bResult = TryToFixRecursive(depth + 1);

                if (bResult)
                    return true;

                UnmoveLstMoved(restZ, restW);
            }

            return false;
        }

        private void UnmoveLstMoved(int restZ, int restW)
        {
            for (int i = movedFromZ.Count - 1; i >= restZ; i--)
            {
                var e = movedFromZ[i];
                MoveEdge(w, z, e, false, true);
                fixed_edges[e.Id] = 0;
                movedFromZ.RemoveAt(i);
            }

            for (int i = movedFromW.Count - 1; i >= restW; i--)
            {
                var e = movedFromW[i];
                MoveEdge(z, w, e, false, true);
                fixed_edges[e.Id] = 0;
                movedFromW.RemoveAt(i);
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



        private void Chain_Edge_Fixing_UnDirectedNoRecursion(Graph z, Graph w, Edge initialEdge)
        {
            if (conf.noChainFix)
                return;

            Stack<int> vertices = new Stack<int>();
            vertices.Push(initialEdge.from);
            vertices.Push(initialEdge.to);

            while (vertices.Count > 0)
            {
                int i = vertices.Pop();
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
                            vertices.Push(edge.to);
                        }
                    }

                    for (int ind = 0; ind < w.edgesTo[i].Count; ind++)
                    {
                        var edge = w.edgesTo[i][ind];
                        if (fixed_edges[edge.Id] == 0)
                        {
                            MoveEdge(w, z, edge);
                            vertices.Push(edge.from);
                        }
                    }


                   for (int ind = 0; ind < z.edgesFrom[i].Count; ind++)
                    {
                        var edge = z.edgesFrom[i][ind];
                        if (fixed_edges[edge.Id] == 0)
                        {
                            fixed_edges[edge.Id] = 1;
                            vertices.Push(edge.to);
                        }
                    }

                    for (int ind = 0; ind < z.edgesTo[i].Count; ind++)
                    {
                        var edge = z.edgesTo[i][ind];
                        if (fixed_edges[edge.Id] == 0)
                        {
                            fixed_edges[edge.Id] = 1;
                            vertices.Push(edge.from);
                        }
                    }
               
                }

                cntFixed = 0;

                foreach (var edge in z.edgesFrom[i])
                    if (fixed_edges[edge.Id] != 0)
                        cntFixed++;

                foreach (var edge in z.edgesTo[i])
                    if (fixed_edges[edge.Id] != 0)
                        cntFixed++;

                if (cntFixed == 2)
                {
                    for (int ind = 0; ind < z.edgesFrom[i].Count; ind++)
                    {
                        var edge = z.edgesFrom[i][ind];
                        if (fixed_edges[edge.Id] == 0)
                        {
                            MoveEdge(z, w, edge);
                            vertices.Push(edge.to);
                        }
                    }

                    for (int ind = 0; ind < z.edgesTo[i].Count; ind++)
                    {
                        var edge = z.edgesTo[i][ind];
                        if (fixed_edges[edge.Id] == 0)
                        {
                            MoveEdge(z, w, edge);
                            vertices.Push(edge.from);
                        }
                    }
  
                    for (int ind = 0; ind < w.edgesFrom[i].Count; ind++)
                    {
                        var edge = w.edgesFrom[i][ind];
                        if (fixed_edges[edge.Id] == 0)
                        {
                            fixed_edges[edge.Id] = 1;
                            vertices.Push(edge.to);
                        }
                    }

                    for (int ind = 0; ind < w.edgesTo[i].Count; ind++)
                    {
                        var edge = w.edgesTo[i][ind];
                        if (fixed_edges[edge.Id] == 0)
                        {
                            fixed_edges[edge.Id] = 1;
                             vertices.Push(edge.from);
                        }
                    }
                  
                }

            }
        }



        private void Chain_Edge_Fixing_UnDirected(Graph z, Graph w, Edge e, int dontCheck = -1)
        {

            // Chain_Edge_Fixing_UnDirectedNoRecursion(z, w, e);
            // return;

            if (conf.noChainFix)
                return;

            int i = e.from;
            int j = e.to;
            if (dontCheck != i)
                Chain_For_vertex(z, w, i);
            if (dontCheck != j)
                Chain_For_vertex(z, w, j);

        }


        private void Chain_For_vertex(Graph z, Graph w, int i)
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
                        Chain_Edge_Fixing_UnDirected(w, z, edge, i);
                    }
                }

                for (int ind = 0; ind < w.edgesTo[i].Count; ind++)
                {
                    var edge = w.edgesTo[i][ind];
                    if (fixed_edges[edge.Id] == 0)
                    {
                        MoveEdge(w, z, edge);
                        Chain_Edge_Fixing_UnDirected(w, z, edge, i);
                    }
                }


                for (int ind = 0; ind < z.edgesFrom[i].Count; ind++)
                {
                    var edge = z.edgesFrom[i][ind];
                    if (fixed_edges[edge.Id] == 0)
                    {
                        fixed_edges[edge.Id] = 1;
                        Chain_Edge_Fixing_UnDirected(w, z, edge, i);
                    }
                }

                for (int ind = 0; ind < z.edgesTo[i].Count; ind++)
                {
                    var edge = z.edgesTo[i][ind];
                    if (fixed_edges[edge.Id] == 0)
                    {
                        fixed_edges[edge.Id] = 1;
                        Chain_Edge_Fixing_UnDirected(w, z, edge, i);
                    }
                }

            }
        }



        private bool SecondNeighborhood()
        {

            List<Edge> edgesToTryZ = new List<Edge>();
            List<Edge> edgesToTryW = new List<Edge>();
            for (int i = 0; i < z.edges.Count; i++)
            {
                var e = z.edges[i];
                if (fixed_edges[e.Id] == 0)
                    edgesToTryZ.Add(z.edges[i]);
            }

            for (int i = 0; i < w.edges.Count; i++)
            {
                var e = w.edges[i];
                if (fixed_edges[e.Id] == 0)
                    edgesToTryW.Add(w.edges[i]);
            }

            ArrayUtils.Shuffle(edgesToTryZ);
            for (int i = 0; i < edgesToTryZ.Count; i++)
            {
                for (int j = 0; j < Math.Min(edgesToTryW.Count, 3); j++)
                {
                    var e1 = edgesToTryZ[i];
                    var e2 = edgesToTryW[j];

                    int attempts = attemptLimit;
                    while (attempts-- > 0)
                    {
                        MoveEdge(z, w, e1);
                        MoveEdge(w, z, e2);

                        Chain_Edge_Fixing_UnDirected(z, w, e1);
                        Chain_Edge_Fixing_UnDirected(w, z, e2);

                        bool res = TryToFix();

                        if (res)
                            return true;

                    }
                }
            }
            if (conf.thirdNeighbourhood)
                return ThirdNeighborhood();

            return false;
        }

        private bool ThirdNeighborhood()
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
                Chain_Edge_Fixing_UnDirected(z, w, e);

                //int br = brokenVerticses.Count;
                bool res = TryToFixRecursive(0);

                if (res)
                    return true;

                BackToOriginal();

            }

            z.RestoreEdges();
            w.RestoreEdges();


            if (conf.secondNeighbour)
                return SecondNeighborhood();

            return false;
        }
    }
}
