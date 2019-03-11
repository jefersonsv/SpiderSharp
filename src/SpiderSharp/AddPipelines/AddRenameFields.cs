using Newtonsoft.Json.Linq;

using System;

namespace SpiderSharp
{
    public abstract partial class SpiderEngine
    {
        [Obsolete("Use RunRenameFields in SpiderContext")]
        public void AddRenameFields(string from, string to)
        {
            this.AddPipeline(it =>
            {
                var jobject = JObject.FromObject(it);
                Helpers.Json.RenameProperty(jobject, from, to);
                it = jobject;

                return it;
            });
        }
    }
}