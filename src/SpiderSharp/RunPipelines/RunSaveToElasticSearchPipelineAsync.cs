using Data.ElasticSearch;
using Humanizer;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SpiderSharp
{
    public partial class SpiderContext
    {
        public async Task RunSaveToElasticSearchPipelineAsync(string primaryKeyField = null)
        {
            var it = this.Data;

            var elastic = new ElasticConnection(GlobalSettings.ElasticSearchIndex, GlobalSettings.ElasticSearchConnectionString);
            JObject obj = JObject.Parse(it.ToString());

            if (primaryKeyField == null)
            {
                var ret = await elastic.IndexAsync(obj.ToString());
                Log.Verbose(ret.Body);
            }
            else
            {
                var id = (string)it[primaryKeyField];
                var ret = await elastic.IndexAsync(id, obj.ToString());
                Log.Verbose(ret.Body);
            }
        }
    }
}