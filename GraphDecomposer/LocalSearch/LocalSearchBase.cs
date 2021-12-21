using GraphDecomposer.DataStructures;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphDecomposer.LocalSearch
{
    public abstract class LocalSearchBase
    {
        public Graph z { get; protected set; }
        public Graph w { get; protected set; }

        protected List<int> brokenVerticses;
        protected List<int> fixed_edges;
        protected Graph optimalZ;
        protected Graph optimalW;
        protected int attemptLimit;
        protected int before;
        protected Graph OriginalZ;
        protected Graph OriginalW;
        protected TestConfiguration conf;
        protected List<Edge> movedFromZ;
        protected List<Edge> movedFromW;

        public LocalSearchBase(Graph z, Graph w, int attemptLimit, TestInput input, TestConfiguration conf)
        {
            this.z = z;
            this.w = w;
            this.attemptLimit = attemptLimit;
            this.OriginalZ = input.x;
            this.OriginalW = input.y;
            this.conf = conf;
            Init();
        }

        private void Init()
        {
            fixed_edges = new List<int>();
            brokenVerticses = new List<int>();
            movedFromZ = new List<Edge>();
            movedFromW = new List<Edge>();

            for (int i = 0; i <= z.nVertices * 2; i++)
            {
                fixed_edges.Add(0);
            }
            var doubleEdges = z.FindDoubleEdges(w);

            foreach (var e in doubleEdges)
            {
                fixed_edges[e.Id] = 2;
            }


            var c11 = z.findSubCicles();
            var c21 = w.findSubCicles();

            before = c11.Count + c21.Count;

        }

        protected bool checkOriginalCicles()
        {
            return !z.CheckEqualCicle(OriginalW) && !z.CheckEqualCicle(OriginalZ);

            //return !z.CheckGrahphsAreTheSameUndirected(OriginalW) && !z.CheckGrahphsAreTheSameUndirected(OriginalZ);

        }

        public abstract bool DoLocalSearch();

    }
}
