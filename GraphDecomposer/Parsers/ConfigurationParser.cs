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
            var conf = array.ToObject<List<TestConfiguration>>();

            return conf;
        }

    }
}
