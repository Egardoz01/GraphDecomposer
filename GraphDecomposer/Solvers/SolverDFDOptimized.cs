using GraphDecomposer.DataStructures;
using GraphDecomposer.LocalSearch;
using GraphDecomposer.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphDecomposer.Solvers
{
    public class SolverDFDOptimized : SolverDFD
    {



        public SolverDFDOptimized() : base()
        {
        }


        protected override SolverResult doIterations()
        {
            int iterationsCnt = 0;
            LocalSearchBase localSearch;
            while (true)
            {
                iterationsCnt++;

                model.Optimize();

                bool hasSol = model.SolCount > 0;
                if (!hasSol)
                    return new SolverResult(iterationsCnt, false, null, null);// no solution


                List<Edge> z_Edges = new List<Edge>();
                List<Edge> w_Edges = new List<Edge>();

                for (int i = 1; i <= multiGraph.nEdges; i++)
                {
                    var v = variables[i];
                    if (v.X == 0)
                        z_Edges.Add(multiGraph.edges[i - 1]);
                    else
                        w_Edges.Add(multiGraph.edges[i - 1]);
                }

                Graph z = new Graph(multiGraph.nVertices, z_Edges, conf.directed);
                Graph w = new Graph(multiGraph.nVertices, w_Edges, conf.directed);

                bool repeat = true;

                while (repeat)
                {
                    var zSubCicles = z.findSubCicles();
                    var wSubCicles = w.findSubCicles();

                    foreach (var cicle in zSubCicles)
                        addCicleConstr(cicle);
                    foreach (var cicle in wSubCicles)
                        addCicleConstr(cicle);

                    if (zSubCicles.Count == 0 && wSubCicles.Count == 0)
                    {
                        return new SolverResult(iterationsCnt, true, z, w);// yes solution
                    }

                    z.GraphId = 1;
                    w.GraphId = 2;
                    if (conf.directed)
                        localSearch = new LocalSearchDirected(z, w, 5, testInput);
                    else
                        localSearch = new LocalSearchUndirected(z, w, 5, testInput);

                    repeat = localSearch.DoLocalSearch();
                    z = localSearch.z;
                    w = localSearch.w;
                }
            }
        }
    }
}
