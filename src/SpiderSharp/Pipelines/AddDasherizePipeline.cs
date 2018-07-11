using Humanizer;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpiderSharp
{
    public abstract partial class SpiderEngine
    {
        public void AddDasherizePipeline()
        {
            this.AddPipeline(it =>
            {
                JObject obj = JObject.FromObject(it);
                Helpers.Json.Rename(obj, name => name.ToString().Underscore().Replace("_", "-"));
                return obj;
            });
        }
    }
}