using GraphDecomposer.DataStructures;
using GraphDecomposer.LocalSearch;
using GraphDecomposer.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            int gorubiTime = 0;
            int localSearchTime = 0;
            Stopwatch gurubiTimer = new Stopwatch();
            Stopwatch LinearSearchTimer = new Stopwatch();

            while (true)
            {
                iterationsCnt++;

                gurubiTimer.Start();
                model.Optimize();
                gurubiTimer.Stop();

                double totalHours = timer.ElapsedMilliseconds / (1000 * 60 * 60);
                bool hasSol = model.SolCount > 0;
                if (!hasSol || totalHours>=2)
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

                    gurubiTimer.Start();
                    foreach (var cicle in zSubCicles)
                        addCicleConstr(cicle);
                    foreach (var cicle in wSubCicles)
                        addCicleConstr(cicle);
                    gurubiTimer.Stop();

                    if (zSubCicles.Count == 0 && wSubCicles.Count == 0)
                    {
                        return new SolverResult(iterationsCnt, true, z, w, gurubiTimer.ElapsedMilliseconds, LinearSearchTimer.ElapsedMilliseconds);// yes solution
                    }

                    z.GraphId = 1;
                    w.GraphId = 2;
                    if (conf.directed)
                        localSearch = new LocalSearchDirected(z, w, 1, testInput, conf);
                    else
                        localSearch = new LocalSearchUndirected(z, w, 1, testInput, conf);

                    LinearSearchTimer.Start();
                    repeat = localSearch.DoLocalSearch();
                    LinearSearchTimer.Stop();

                    z = localSearch.z;
                    w = localSearch.w;
                }
            }
        }
    }
}
