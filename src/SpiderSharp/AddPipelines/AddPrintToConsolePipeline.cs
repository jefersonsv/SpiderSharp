using Newtonsoft.Json.Linq;

using System;
using System.Linq;

namespace SpiderSharp
{
    public abstract partial class SpiderEngine
    {
        [Obsolete("Use RunPrintToConsolePipeline in SpiderContext")]
        public void AddPrintToConsolePipeline(params string[] fields)
        {
            this.AddPipeline(it =>
            {
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

                return it;
            });
        }
    }
}