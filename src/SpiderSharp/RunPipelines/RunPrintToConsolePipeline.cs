using Humanizer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpiderSharp
{
    public partial class SpiderContext
    {
        public void RunPrintToConsolePipeline(params string[] fields)
        {
            var it = this.Data;

            if (fields == null || fields.Count() == 0)
            {
                JToken obj = JToken.FromObject(it);
                Console.WriteLine(obj);
            }
            else
            {
                foreach (var item in fields)
                {
                    if (it[item] != null)
                        Console.WriteLine($"{item} => {it[item].Value}");
                }
            }
        }
    }
}