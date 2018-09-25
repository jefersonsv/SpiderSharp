using Data.MongoDB;
using Humanizer;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpiderSharp
{
    public abstract partial class SpiderEngine
    {
        public void AddSaveToMongoDBAsyncPipeline(string collection, string primaryKeyField)
        {
            this.AddPipeline(it =>
            {
                try
                {
                    var mongo = new MongoConnection(GlobalSettings.MongoDatabase, GlobalSettings.MongoConnectionString);
                    var pkValue = (string)it[primaryKeyField];
                    mongo.UpdateDefinition(collection, primaryKeyField, pkValue, it.ToString());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                return it;
            });
        }
    }
}