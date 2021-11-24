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

            return g;
        }
        public void Add(Edge e, bool addToEdgesArray = true)
        {
            if (addToEdgesArray)
                edges.Add(e);

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

        public void Remove(Edge e, bool addToEdgesArray=true)
        {
            if(addToEdgesArray)
                edges.Remove(e);

            nEdges--;
            edgesFrom[e.from].Remove(e);
            edgesTo[e.to].Remove(e);
        }

        public string FindCycle()
        {
            return this.directed ? FindCycleDirected() : FindCycleUnDirected();
        }

        public string FindCycleDirected()
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

            return String.Join(' ', cicles[0].ToArray());
        }



        public string FindCycleUnDirected()
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
                    used[i] = true;
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
                            }
                        }

                        foreach (var ed in edgesTo[x])
                        {

                            int next = ed.from;
                            if (!used[next])
                            {
                                used[next] = true;
                                cur.Add(next);
                                cnt++;
                            }
                        }
                    }
                    cicles.Add(cur);
                }
            }

            return String.Join(' ', cicles[0].ToArray());
        }


        List<bool> usedEdges;
        List<bool> usedVertices;
        List<Edge> currentCicle;

        public List<List<Edge>> findSubCicles()
        {
            List<List<Edge>> cicles = new List<List<Edge>>();

             usedEdges = new List<bool>();
             usedVertices = new List<bool>();

            for (int i = 0; i <= this.nEdges*2; i++)
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
                    }
                }

            }

        }
    }
}
