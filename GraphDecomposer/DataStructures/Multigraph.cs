using System;
using System.Collections.Generic;
using System.Text;

namespace GraphDecomposer.DataStructures
{
    public class Multigraph : Graph
    {
        public HashSet<Edge> xEdges;
        public HashSet<Edge> yEdges;

        public Multigraph(Graph x, Graph y, bool directed)
        {
            this.directed = directed;
            if (x.nVertices != y.nVertices)
                throw new Exception("Invalid input");

            this.nVertices = x.nVertices;
            this.nEdges = x.nEdges + y.nEdges;
            this.edges = new List<Edge>();
            this.edgesFrom = new List<List<Edge>>();
            this.edgesTo = new List<List<Edge>>();
            xEdges = new HashSet<Edge>();
            yEdges = new HashSet<Edge>();

            for (int i = 0; i <= this.nVertices; i++)
            {
                this.edgesFrom.Add(new List<Edge>());
                this.edgesTo.Add(new List<Edge>());
            }

            for (int i = 0; i < x.edges.Count; i++)
            {
                int id = i + 1;

                var edge = new Edge(id, x.edges[i].from, x.edges[i].to);

                this.edges.Add(edge);
                this.edgesFrom[x.edges[i].from].Add(edge);

                if(!directed)
                    this.edgesFrom[x.edges[i].to].Add(edge);
                else
                    this.edgesTo[x.edges[i].to].Add(edge);

                xEdges.Add(edge);
            }


            for (int i = 0; i < y.edges.Count; i++)
            {
                int id = x.edges.Count + i + 1;
                var edge = new Edge(id, y.edges[i].from, y.edges[i].to);
                this.edges.Add(edge);
                this.edgesFrom[y.edges[i].from].Add(edge);

                if (!directed)
                    this.edgesFrom[y.edges[i].to].Add(edge);
                else
                    this.edgesTo[y.edges[i].to].Add(edge);

                yEdges.Add(edge);
            }

            for (int i = 0; i < y.edges.Count; i++)
                xEdges.Remove(y.edges[i]);

            for (int i = 0; i < x.edges.Count; i++)
                yEdges.Remove(x.edges[i]);
        }
    }
}
