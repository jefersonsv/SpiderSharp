using System;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Nest;

namespace Data.ElasticSearch
{
    public class ElasticConnection
    {
        Uri node;
        string index;

        /// <summary>
        /// https://www.elastic.co/guide/en/elasticsearch/client/net-api/current/elasticsearch-net.html
        /// </summary>
        public ElasticLowLevelClient LowClient { get; private set; }

        /// <summary>
        /// https://www.elastic.co/guide/en/elasticsearch/client/net-api/current/nest.html
        /// </summary>
        public ElasticClient HighClient { get; private set; }

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

        //public ElasticConnection(string index)
        //{
        //    node = new Uri("http://127.0.0.1:9200");
        //    var config = new ConnectionConfiguration(node);
        //    LowClient = new ElasticLowLevelClient(config);
        //    HighClient = new ElasticClient(node);

        //    this.index = index;
        //}

        public ElasticConnection(string index, string elasticSearchConnectionString)
        {
            node = new Uri(elasticSearchConnectionString);
            var config = new ConnectionConfiguration(node);
            LowClient = new ElasticLowLevelClient(config);
            HighClient = new ElasticClient(node);
            this.index = index;
        }

        public void DeleteById(string id)
        {
            var doc = new Nest
                .DocumentPath<dynamic>(new Nest.Id(id))
                .Index(this.index)
                .Type(this.index);

            Nest.IDeleteResponse resp = this.HighClient.Delete<dynamic>(doc);
        }

        public async Task DeleteByIdAsync(string id)
        {
            var doc = new Nest
                .DocumentPath<dynamic>(new Nest.Id(id))
                .Index(this.index)
                .Type(this.index);

            Nest.IDeleteResponse resp = await this.HighClient.DeleteAsync<dynamic>(doc);
        }

        public dynamic QueryAll(int limit = 10)
        {
            var searchResponse = this.HighClient.Search<dynamic>(s => s
                    .Index(this.index)
                    .Type(this.index)
                    .Size(limit)
                    .Query(q => q
                        .MatchAll()
                    )
                );

            return searchResponse;
        }

        public async Task<dynamic> QueryAllAsync(int limit = 10, DateTime belowOf)
        {
            var searchResponse = await this.HighClient.SearchAsync<dynamic>(s => s
                    .Index(this.index)
                    .Type(this.index)
                    .Size(limit)
                    .Query(q => q
                        .MatchAll()
                    )
                );

            return searchResponse;
        }

        public void Index(string id, string json)
        {
            PostData post = PostData.String(json);
            var res = LowClient.Index<Elasticsearch.Net.StringResponse>(index, index, id, post);
        }

        public async Task<StringResponse> IndexAsync(string id, string json)
        {
            PostData post = PostData.String(json);
            return await LowClient.IndexAsync<Elasticsearch.Net.StringResponse>(index, index, id, post);
        }

        public void Index(string json)
        {
            PostData post = PostData.String(json);
            var res = LowClient.Index<Elasticsearch.Net.StringResponse>(index, index, post);
        }

        public async Task<StringResponse> IndexAsync(string json)
        {
            PostData post = PostData.String(json);
            return await LowClient.IndexAsync<Elasticsearch.Net.StringResponse>(index, index, post);
        }
    }
}
