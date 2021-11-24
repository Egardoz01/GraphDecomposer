using System;
using System.Collections.Generic;
using System.Text;

namespace GraphDecomposer
{
    public struct Edge
    {
        public int Id;
        public int from;
        public int to;
        public bool directed;
        private int _hashCode;
        public Edge(int id, int from, int to, bool directed)
        {
            this.Id = id;
            this.from = from;
            this.to = to;
            this.directed = directed;
            _hashCode = CountHashCode(from, to, directed);
        }


        private static int CountHashCode(int from, int to, bool directed)
        {
            int v1 = from, v2 = to;

            if (!directed)
            {
                if (v1 > v2)
                {
                    int cc = v1;
                    v1 = v2;
                    v2 = cc;
                }
            }

            return (v1 + " " + v2).GetHashCode();
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override bool Equals(object obj)
        {
            return this.GetHashCode() == obj.GetHashCode();
        }

        public override string ToString()
        {
            return $"{Id}  {from} -> {to}";
        }

    }
}
