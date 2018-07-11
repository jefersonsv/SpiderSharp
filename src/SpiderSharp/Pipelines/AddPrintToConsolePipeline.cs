using Humanizer;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpiderSharp
{
    public abstract partial class SpiderEngine
    {
        public void AddPrintToConsolePipeline(params string[] fields)
        {
            this.AddPipeline(it =>
            {
                if (fields == null || fields.Count() == 0)
                {
                    JObject obj = JObject.FromObject(it);
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