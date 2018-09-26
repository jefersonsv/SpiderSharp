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
        public void RunEmbedException(string propertyName = "__exception")
        {
            if (this.HasError)
            {
                JObject obj = JObject.FromObject(this.Data);
                obj.Add(propertyName, JObject.FromObject(this.Error));

                this.Data = obj;
            }
        }
    }
}