using Data.ElasticSearch;
using Humanizer;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpiderSharp
{
    public partial class SpiderContext
    {
        public void RunSaveToElasticSearchPipeline(string type, string primaryKeyField)
        {
            var it = this.Data;
            try
            {
                var elastic = new ElasticConnection(GlobalSettings.ElasticSearchIndex, GlobalSettings.ElasticSearchConnectionString);
                JObject obj = JObject.Parse(it.ToString());
                var id = (string)it[primaryKeyField];
                elastic.Index(type, id, obj.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}