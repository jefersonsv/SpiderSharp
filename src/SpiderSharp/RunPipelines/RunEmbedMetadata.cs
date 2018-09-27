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
        public void RunEmbedMetadata(string exceptionProperty = "__exception", string urlProperty = "__url", string nowProperty = "__now")
        {
            JObject obj = JObject.FromObject(this.Data);

            if (this.Error != null)
                obj.Add(exceptionProperty, JObject.FromObject(this.Error));

            if (!string.IsNullOrEmpty(this.Url))
                obj.Add(urlProperty, this.Url);

            obj.Add(nowProperty, DateTime.Now);

            this.Data = obj;
        }
    }
}