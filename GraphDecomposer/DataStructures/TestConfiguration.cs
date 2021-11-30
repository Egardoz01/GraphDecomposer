using System;
using System.Collections.Generic;
using System.Text;

namespace GraphDecomposer.DataStructures
{
    public struct TestConfiguration
    {
        public string model;
        public bool directed;
        public int nVertices;
        public int nTests;
        public string testFile;
        public int solvable;
        public int unsolvable;
        public bool secondNeighbour;
    }
}
