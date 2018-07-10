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
                Helpers.Json.Rename((JObject)it, name => name.ToString().Underscore().Replace("_", "-"));
            });
        }
    }
}