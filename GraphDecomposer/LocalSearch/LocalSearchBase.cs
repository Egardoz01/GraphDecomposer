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
        public LocalSearchBase(Graph z, Graph w, int attemptLimit, TestInput input)
        {
            this.z = z;
            this.w = w;
            this.attemptLimit = attemptLimit;
            this.OriginalZ = input.x;
            this.OriginalW = input.y;

            Init();
        }

        private void Init()
        {
            fixed_edges = new List<int>();

            for (int i = 0; i <= z.nVertices * 2; i++)
            {
                fixed_edges.Add(0);
            }

            int cntFIxed = 0;
            foreach (Edge e in z.edges)
            {
                var same_edge = w.edges.FindAll(x => x.GetHashCode() == e.GetHashCode());

                if (same_edge.Count > 0)
                {
                    cntFIxed++;
                    fixed_edges[e.Id] = 2;
                    fixed_edges[same_edge[0].Id] = 2;
                }
            }

            optimalZ = z.Copy();
            optimalW = w.Copy();

            var c11 = z.findSubCicles();
            var c21 = w.findSubCicles();
            before = c11.Count + c21.Count;
        }

        protected bool checkOriginalCicles()
        {
            return !(z.CheckEqualCicle(OriginalW) || z.CheckEqualCicle(OriginalZ) || w.CheckEqualCicle(OriginalZ) || w.CheckEqualCicle(OriginalW));
        }

        public abstract bool DoLocalSearch();

    }
}
