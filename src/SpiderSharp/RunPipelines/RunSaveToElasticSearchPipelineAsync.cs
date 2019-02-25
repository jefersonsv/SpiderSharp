using DataFoundation.ElasticSearch;
using Humanizer;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SpiderSharp
{
    public partial class SpiderContext
    {
        public async Task RunSaveToElasticSearchPipelineAsync(string index, string primaryKeyField = null)
        {
            var it = this.Data;

            var elastic = new ElasticConnection(index, GlobalSettings.ElasticSearchConnectionString);
            JObject obj = new JObject(it);
            var json = obj.ToString();

            if (primaryKeyField == null)
            {
                var ret = await elastic.IndexAsync(json);
                if (!ret.Success)
                    Log.Error(ret.Body);
            }
            else
            {
                var id = (string)it[primaryKeyField];
                var ret = await elastic.IndexAsync(id, json);
                if (!ret.Success)
                {
                    Log.Error($"{id} -> {ret.Body}");
                }
            }
        }
    }
}