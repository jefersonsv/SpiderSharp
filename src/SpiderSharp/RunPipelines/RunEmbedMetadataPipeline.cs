using Data.ElasticSearch;
using Humanizer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpiderSharp
{
    public partial class SpiderContext
    {
        public void RunEmbedMetadataPipeline(string exceptionProperty = "__exception", string urlProperty = "__url", string nowProperty = "__now", string spiderName = "__spider")
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