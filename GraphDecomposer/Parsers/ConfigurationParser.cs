using GraphDecomposer.DataStructures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft;
using Newtonsoft.Json.Linq;

namespace GraphDecomposer.Parsers
{
    public static class ConfigurationParser
    {
        public static List<TestConfiguration> GetConfiguration(string filename)
        {
            string json = File.ReadAllText(filename);
            dynamic dinamic = JObject.Parse(json);
            JArray array = dinamic["tests"];
            bool directed = dinamic["directed"];
            string model = dinamic["model"];
            string dir = dinamic["TestsDirectory"];
            int singleTestTimeout = dinamic["SingleTestTimeout"];
            int PackOfTestTimeout = dinamic["PackOfTestTimeout"];
            bool FiveCyclesSearch = dinamic["FiveCyclesSearch"];

            bool run = false;

            string startWith = dinamic["StartWith"];
            if (startWith == "")
                run = true;
            List<TestConfiguration> conf;
            if (dir == "")
                conf = array.ToObject<List<TestConfiguration>>();
            else
                conf = GetAllTestsConfiguration(dir);

            for (int i = 0; i < conf.Count; i++)
            {
                var a = conf[i];
                a.directed = directed;
                a.model = model;
                a.runTest = run;
                a.SingleTestTimeout = singleTestTimeout;
                a.PackOfTestTimeout = PackOfTestTimeout;
                a.FiveCyclesSearch = FiveCyclesSearch;

                if (a.testFile.Contains(startWith))
                    run = true;


                conf[i] = a;
            }
            return conf;
        }


        public static List<TestConfiguration> GetAllTestsConfiguration(string dir)
        {
            List<TestConfiguration> conf = new List<TestConfiguration>();
            var files = Directory.GetFiles(dir);
            for (int i = 0; i < files.Length; i++)
            {
                var file = files[i];

                TestConfiguration test = new TestConfiguration();
                test.directed = false;
                test.nTests = 100;
                test.testFile = file;
                test.nVertices = getNVertices(file);
                conf.Add(test);
            }


            conf.Sort((a, b) =>
            {
                return a.nVertices - b.nVertices;
            });

            return conf;
        }


        private static int getNVertices(string s)
        {
            var digits = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            int l = s.IndexOfAny(digits);
            int r = l;
            var digitsList = new List<char>(digits);
            while (digitsList.Contains(s[r]))
                r++;

            string d = s.Substring(l, r - l);

            return int.Parse(d);
        }

    }
}
