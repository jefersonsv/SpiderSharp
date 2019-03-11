using DataFoundation.ElasticSearch;

using Newtonsoft.Json.Linq;

using System;

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