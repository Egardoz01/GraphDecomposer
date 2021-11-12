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
        public List<List<int>> edgesFrom;
        public List<List<int>> edgesTo;


        public string FindCycle()
        {
            edgesFrom = new List<List<int>>();
            for (int i = 0; i <= nVertices; i++)
                edgesFrom.Add(new List<int>());
            foreach (var e in edges)
            {
                edgesFrom[e.from].Add(e.to);
            }

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
                        int next = edgesFrom[x][0];
                        used[next] = true;
                        cur.Add(next);
                        cnt++;
                    }
                    cicles.Add(cur);
                }
            }

            return String.Join(' ', cicles[0].ToArray());
        }

    }
}
