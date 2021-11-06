using System;
using System.Collections.Generic;
using System.Text;

namespace GrubiTest.DataStructures
{
    public class Multigraph : Graph
    {
        public List<List<int>> edgesFrom;
        public List<List<int>> edgesTo;
        public HashSet<Edge> xEdges;
        public HashSet<Edge> yEdges;
        public bool directed;
        public Multigraph(Graph x, Graph y, bool directed)
        {
            this.directed = directed;
            if (x.nVertices != y.nVertices)
                throw new Exception("Invalid input");

            this.nVertices = x.nVertices;
            this.nEdges = x.nEdges + y.nEdges;
            this.edges = new List<Edge>();
            this.edgesFrom = new List<List<int>>();
            this.edgesTo = new List<List<int>>();
            xEdges = new HashSet<Edge>();
            yEdges = new HashSet<Edge>();

            for (int i = 0; i <= this.nVertices; i++)
            {
                this.edgesFrom.Add(new List<int>());
                this.edgesTo.Add(new List<int>());
            }

            for (int i = 0; i < x.edges.Count; i++)
            {
                int id = i + 1;
                this.edges.Add(new Edge(id, x.edges[i].from, x.edges[i].to));
                this.edgesFrom[x.edges[i].from].Add(id);

                if(!directed)
                    this.edgesFrom[x.edges[i].to].Add(id);
                else
                    this.edgesTo[x.edges[i].to].Add(id);

                xEdges.Add(this.edges[id-1]);
            }


            for (int i = 0; i < y.edges.Count; i++)
            {
                int id = x.edges.Count + i + 1;
                this.edges.Add(new Edge(id, y.edges[i].from, y.edges[i].to));
                this.edgesFrom[y.edges[i].from].Add(id);

                if (!directed)
                    this.edgesFrom[y.edges[i].to].Add(id);
                else
                    this.edgesTo[y.edges[i].to].Add(id);

                yEdges.Add(this.edges[id - 1]);
            }

            for (int i = 0; i < y.edges.Count; i++)
                xEdges.Remove(y.edges[i]);

            for (int i = 0; i < x.edges.Count; i++)
                yEdges.Remove(x.edges[i]);
        }
    }
}
