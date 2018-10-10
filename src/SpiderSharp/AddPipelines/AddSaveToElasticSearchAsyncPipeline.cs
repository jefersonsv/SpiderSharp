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
        public void AddSaveToElasticSearchAsyncPipeline(string index, string primaryKeyField)
        {
            this.AddPipeline(it =>
            {
                var elastic = new ElasticConnection(index, GlobalSettings.ElasticSearchConnectionString);
                JObject obj = JObject.Parse(it.ToString());
                var id = (string)it[primaryKeyField];
                elastic.Index(id, obj.ToString());

                return it;
            });
        }
    }
}