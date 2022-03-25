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
        public bool zeroNeighbourhood;
        public bool firstNeighbourhood;
        public bool thirdNeighbourhood;
        public bool noChainFix;
        public int recursionDepth;
        public int thirdNeighborhoodType;
        public bool runTest;
        public int PackOfTestTimeout;
        public int SingleTestTimeout;
        public bool FiveCyclesSearch;
    }

   
}
