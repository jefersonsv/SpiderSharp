using System;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MONGO = MongoDB.Driver;

namespace Data.MongoDB
{
    public class MongoConnection
    {
        private MongoClient client;
        private IMongoDatabase DB;

        public MongoConnection(string database, string mongodbConnectionString)
        {
            var settings = GetSettingsBy(mongodbConnectionString);
            client = new MONGO.MongoClient(settings);

            DB = client.GetDatabase(database);
        }

        private MongoClientSettings GetSettingsBy(string url)
        {
            // Verify if schema is typed
            if (!Uri.CheckSchemeName(url) && !url.Contains("://"))
                url = $"mongodb://{url}";

            // https://docs.mongodb.com/manual/reference/connection-string/
            var mongoUrl = new MongoUrl(url);
            return MongoClientSettings.FromUrl(mongoUrl);
        }

        public void UpdateDefinition(string collection, string pkField, string pkValue, string json)
        {
            //self.db[self.mongo_collection].update({'unique-id': uniqueid}, {"$set" : dicionary, "$inc": { 'counter': 1}}, upsert=True)
            var coll = DB.GetCollection<BsonDocument>(collection);
            var now = DateTime.UtcNow;

            var filter = Builders<BsonDocument>.Filter
                .Eq(pkField, pkValue);

            var update = Builders<BsonDocument>.Update
                .SetOnInsert("inserted", now)
                .Set("updated", now)
                .Inc("counter", 1);

            foreach (var item in BsonDocument.Parse(json).ToDictionary())
            {
                update = update.Set(item.Key, item.Value);
            }

            // http://www.fourthbottle.com/2015/07/update-details-into-mongodb-using-csharp-and-.Net.html
            var upt = coll.UpdateOneAsync(filter, update, new UpdateOptions() { IsUpsert = true });
            upt.Wait();
        }
    }
}