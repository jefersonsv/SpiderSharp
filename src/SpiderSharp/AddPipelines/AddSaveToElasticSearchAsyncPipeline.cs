using Data.ElasticSearch;
using Humanizer;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpiderSharp
{
    
    public abstract partial class SpiderEngine
    {

        [Obsolete("Use RunSaveToElasticSearchAsyncPipeline in SpiderContext")]
        public void AddSaveToElasticSearchAsyncPipeline(string type, string primaryKeyField)
        {
            this.AddPipeline(it =>
            {
                var elastic = new ElasticConnection(GlobalSettings.ElasticSearchIndex, GlobalSettings.ElasticSearchConnectionString);
                JObject obj = JObject.Parse(it.ToString());
                var id = (string)it[primaryKeyField];
                elastic.Index(id, obj.ToString());

                return it;
            });
        }
    }
}