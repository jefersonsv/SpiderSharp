using Humanizer;

using Newtonsoft.Json.Linq;

using System;

namespace SpiderSharp
{
    public abstract partial class SpiderEngine
    {
        [Obsolete("Use RunDasherizePipeline in SpiderContext")]
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