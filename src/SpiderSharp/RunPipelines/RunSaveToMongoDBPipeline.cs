using DataFoundation.MongoDB;
using Humanizer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SpiderSharp
{
    public partial class SpiderContext
    {
        public async Task RunSaveToMongoDBAsyncPipelineAsync(string collection, string primaryKeyField)
        {
            var it = this.Data;
            
            try
            {
                var mongo = new MongoConnection(GlobalSettings.MongoDatabase, GlobalSettings.MongoConnectionString);
                var pkValue = (string)it[primaryKeyField];
                await mongo.UpdateDefinitionAsync(collection, primaryKeyField, pkValue, it.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}