using System;
using Elasticsearch.Net;
using Nest;

namespace Data.ElasticSearch
{
    public class ElasticConnection
    {
        Uri node;
        string index;
        ElasticLowLevelClient client;

        public static void CreateIndex(string elasticSearchConnectionString, string index, int shards, int replicas)
        {
            CreateIndexRequest req = new CreateIndexRequest(index);
            req.Settings = new IndexSettings
            {
                NumberOfReplicas = replicas,
                NumberOfShards = shards
            };

            var client = new Nest.ElasticClient(new Uri(elasticSearchConnectionString));
            var res = client.CreateIndex(req);
        }

        public static void DeleteIndex(string elasticSearchConnectionString, string index)
        {
            var req = new DeleteIndexRequest(index);

            var client = new Nest.ElasticClient(new Uri(elasticSearchConnectionString));
            var res = client.DeleteIndex(req);
        }

        public ElasticConnection(string index)
        {
            node = new Uri("http://127.0.0.1:9200");
            var config = new ConnectionConfiguration(node);
            client = new ElasticLowLevelClient(config);
            this.index = index;
        }

        public ElasticConnection(string index, string elasticSearchConnectionString)
        {
            node = new Uri(elasticSearchConnectionString);
            var config = new ConnectionConfiguration(node);
            client = new ElasticLowLevelClient(config);
            this.index = index;
        }

        public void Index(string type, string id, string json)
        {
            PostData post = PostData.String(json);
            var res = client.Index<Elasticsearch.Net.StringResponse>(index, type, id, post);
        }
    }
}
