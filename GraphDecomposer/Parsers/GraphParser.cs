﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace GrubiTest
{
    public static class GraphParser
    {
        public static Graph parce(string s)
        {
            Graph g = new Graph();

            var vertices = s.Trim().Split(' ').Select(x => int.Parse(x)).ToList();
            validateInput(vertices);

            g.nVertices = vertices.Count();
            g.nEdges = vertices.Count();

            g.edges = new List<Edge>();

            for (int i = 1; i < vertices.Count(); i++)
            {
                g.edges.Add(new Edge(i, vertices[i - 1], vertices[i]));
            }

            g.edges.Add(new Edge(vertices.Count, vertices[vertices.Count - 1], vertices[0]));

            return g;
        }


        private static void validateInput(List<int> vertices)
        {
            HashSet<int> used = new HashSet<int>();

            for (int i = 0; i < vertices.Count(); i++)
            {
                if (used.Contains(vertices[i]))
                {
                    throw new Exception("Invalid input");
                }

                if(vertices[i]<=0 || vertices[i] > vertices.Count())
                {
                    throw new Exception("Invalid input");
                }

                used.Add(vertices[i]);
            }
        }

    }
}

