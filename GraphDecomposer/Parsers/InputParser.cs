using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GrubiTest
{
    public class InputParser : IEnumerable<ProblemInput>
    {

        private List<ProblemInput> graphs;

        public InputParser(string filePath, int nProblems)
        {
            try
            {
                var lines = File.ReadAllLines(filePath);

                graphs = new List<ProblemInput>();
                int cur = 0;
                for (int i = 0; i < nProblems; i++)
                {
                    ProblemInput input = new ProblemInput();
                    input.x = GraphParser.parce(lines[cur++]);

                    input.y = GraphParser.parce(lines[cur++]);
                    cur++;

                    graphs.Add(input);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Input Parsing exception: "+ ex);
            }

        }

        public IEnumerator<ProblemInput> GetEnumerator()
        {
            foreach (var item in graphs)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
