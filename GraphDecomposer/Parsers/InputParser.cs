using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GraphDecomposer
{
    public class InputParser : IEnumerable<TestInput>
    {

        private List<TestInput> graphs;

        public InputParser(string filePath, int nProblems, bool directed)
        {
            try
            {
                var lines = File.ReadAllLines(filePath);

                graphs = new List<TestInput>();
                int cur = 0;
                for (int i = 0; i < nProblems; i++)
                {
                    TestInput input = new TestInput();
                    input.x = GraphParser.parce(lines[cur++], directed);

                    input.y = GraphParser.parce(lines[cur++], directed);
                    cur++;

                    graphs.Add(input);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Input Parsing exception: "+ ex);
            }

        }

        public IEnumerator<TestInput> GetEnumerator()
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
