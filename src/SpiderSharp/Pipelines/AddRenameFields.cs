using Humanizer;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpiderSharp
{
    public abstract partial class SpiderEngine
    {
        public void AddRenameFields(string from, string to)
        {
            this.AddPipeline(it =>
            {
                Helpers.Json.RenameProperty((JObject)it, from, to);
            });
        }
    }
}