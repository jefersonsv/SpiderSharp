using Newtonsoft.Json.Linq;

using System;

namespace SpiderSharp
{
    public partial class SpiderContext
    {
        public void RunEmbedMetadataPipeline(string exceptionProperty = "exception", string urlProperty = "url", string nowProperty = "now", string spiderName = "spider")
        {
            JObject obj = JObject.FromObject(this.Data);

            if (this.Error != null)
                obj.Add(exceptionProperty, JObject.FromObject(this.Error));

            if (!string.IsNullOrEmpty(this.Url))
                obj.Add(urlProperty, this.Url);

            obj.Add(spiderName, this.Spider);
            obj.Add(nowProperty, DateTime.Now);

            this.Data = obj;
        }
    }
}