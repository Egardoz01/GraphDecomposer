using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphDecomposer
{
    public class Graph
    {
        public int nVertices;
        public int nEdges;
        public List<Edge> edges;
        public List<List<Edge>> edgesFrom;
        public List<List<Edge>> edgesTo;
        public bool directed;
        public int GraphId;
        public Graph()
        {

        }

        public Graph(int nVertices, List<Edge> edges, bool directed)
        {
            this.directed = directed;
            this.nVertices = nVertices;
            this.nEdges = edges.Count;
            this.edges = new List<Edge>();
            foreach (var e in edges)
                this.edges.Add(e);

            edgesFrom = new List<List<Edge>>();
            edgesTo = new List<List<Edge>>();
            for (int i = 0; i <= nVertices; i++)
            {
                edgesTo.Add(new List<Edge>());
                edgesFrom.Add(new List<Edge>());
            }

            foreach (var e in edges)
            {
                edgesFrom[e.from].Add(e);
                edgesTo[e.to].Add(e);
            }

        }

        public Graph Copy()
        {
            Graph g = new Graph(nVertices, edges, directed);
            g.GraphId = this.GraphId;
            return g;
        }
        public void Add(Edge e, bool addToEdgesArray = true)
        {
            if (addToEdgesArray)
            {
                if (edges.FindAll(x => x.Id == e.Id).Count > 0)
                {

                    throw new Exception("Bruh");
                }
                edges.Add(e);
            }
            nEdges++;
            edgesFrom[e.from].Add(e);
            edgesTo[e.to].Add(e);
        }

        public void RestoreEdges()
        {
            edges = new List<Edge>();
            foreach (var edgesFromVertex in edgesFrom)
            {
                foreach (var edge in edgesFromVertex)
                    edges.Add(edge);
            }

        }

        public void Remove(Edge e, bool addToEdgesArray = true)
        {
            if (addToEdgesArray)
            {
                if (!edges.Contains(e))
                {
                    throw new Exception("Bruh");
                }
                edges.Remove(e);
            }
            nEdges--;
            edgesFrom[e.from].RemoveAll(x => x.Id == e.Id);
            edgesTo[e.to].RemoveAll(x => x.Id == e.Id);
        }

        public int[] FindCycle()
        {
            return this.directed ? FindCycleDirected() : FindCycleUnDirected();
        }

        public bool checkCicle(string s)
        {
            var verts = s.Split().Select(x => int.Parse(x)).ToList();

            if (verts.Count != this.nVertices)
                return false;

            verts.Add(verts[0]);
            for (int i = 1; i <= nVertices; i++)
            {
                int u = verts[i];
                int v = verts[i - 1];
                var c1 = edgesFrom[u].FindAll(x => x.to == v);
                var c2 = edgesTo[u].FindAll(x => x.from == v);
                if (c1.Count + c2.Count == 0)
                {

                    FindCycle();
                    return false;
                }
            }


            return true;
        }


        public int[] FindCycleDirected()
        {
            List<List<int>> cicles = new List<List<int>>();
            List<bool> used = new List<bool>();
            for (int i = 0; i <= nVertices; i++)
                used.Add(false);
            for (int i = 1; i <= nVertices; i++)
            {
                List<int> cur = new List<int>();
                if (!used[i])
                {
                    cur.Add(i);
                    int cnt = 1;
                    while (cnt < nVertices)
                    {
                        int x = cur[cnt - 1];
                        int next = edgesFrom[x][0].to;
                        used[next] = true;
                        cur.Add(next);
                        cnt++;
                    }
                    cicles.Add(cur);
                }
            }

            // return String.Join(' ', cicles[0].ToArray());
            return cicles[0].ToArray();
        }



        public int[] FindCycleUnDirected()
        {
            List<List<int>> cicles = new List<List<int>>();
            List<bool> used = new List<bool>();
            for (int i = 0; i <= nVertices; i++)
                used.Add(false);

            List<int> cur = new List<int>();

            cur.Add(1);
            used[1] = true;
            int cnt = 1;
            while (cnt < nVertices)
            {
                int x = cur[cnt - 1];
                foreach (var ed in edgesFrom[x])
                {

                    int next = ed.to;
                    if (!used[next])
                    {
                        used[next] = true;
                        cur.Add(next);
                        cnt++;
                        break;
                    }
                }
                x = cur[cnt - 1];
                foreach (var ed in edgesTo[x])
                {

                    int next = ed.from;
                    if (!used[next])
                    {
                        used[next] = true;
                        cur.Add(next);
                        cnt++;
                        break;
                    }
                }
            }
            cicles.Add(cur);



            //return String.Join(' ', cicles[0].ToArray());
            return cicles[0].ToArray();
        }


        List<bool> usedEdges;
        List<bool> usedVertices;
        List<Edge> currentCicle;

        public List<List<Edge>> findSubCicles()
        {
            List<List<Edge>> cicles = new List<List<Edge>>();

            usedEdges = new List<bool>();
            usedVertices = new List<bool>();

            for (int i = 0; i <= this.nEdges * 2; i++)
                usedEdges.Add(false);

            for (int i = 0; i <= this.nVertices; i++)
                usedVertices.Add(false);

            for (int i = 1; i <= this.nVertices; i++)
            {
                if (!usedVertices[i])
                {
                    currentCicle = new List<Edge>();
                    dfs(i);
                    if (currentCicle.Count < edges.Count && currentCicle.Count > 0)
                    {
                        var last = currentCicle[currentCicle.Count - 1];
                        if (last.to != i && last.from != i)
                        {
                            throw new Exception("Bruh");
                        }
                        cicles.Add(new List<Edge>(currentCicle));
                    }
                }
            }
            return cicles;
        }



        private void dfs(int x)
        {
            usedVertices[x] = true;
            foreach (var edge in this.edgesFrom[x])
            {
                int y;
                y = edge.to;

                if (!usedEdges[edge.Id])
                {
                    usedEdges[edge.Id] = true;
                    currentCicle.Add(edge);
                    dfs(y);
                    break;
                }
            }

            if (!this.directed)
            {
                foreach (var edge in this.edgesTo[x])
                {
                    int y;
                    y = edge.from;

                    if (!usedEdges[edge.Id])
                    {
                        usedEdges[edge.Id] = true;
                        currentCicle.Add(edge);
                        dfs(y);
                        break;
                    }
                }
            }
        }
    }
}
