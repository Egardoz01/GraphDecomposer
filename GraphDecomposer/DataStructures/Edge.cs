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

        public Edge(int id, int from, int to)
        {
            this.Id = id;
            this.from = from;
            this.to = to;
        }

        public override int GetHashCode()
        {
            int v1=from, v2=to;
            if (from > to)
            {
                int cc = from;
                from = to;
                to = cc;
            }    
            return (from + " " + to).GetHashCode(); 
        }

        public override bool Equals(object obj)
        {
            return this.GetHashCode() == obj.GetHashCode();
        }

    }
}
