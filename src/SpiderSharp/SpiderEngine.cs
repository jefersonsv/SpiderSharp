using EnsureThat;
using Humanizer;
using Newtonsoft.Json.Linq;
using Data.Mongo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpiderSharp
{
    public abstract class SpiderEngine
    {
        readonly List<Action<dynamic>> pipelines = new List<Action<dynamic>>();
        private string sourceCode;
        private string url;
        protected Nodes node;
        public string SpiderName { get; private set; }

        protected SpiderEngine()
        {
            this.SpiderName = this.GetType().Name.Underscore().Replace("_", "-");
        }

        protected virtual void After(dynamic jObject)
        {
            // Executed after Run()
            foreach (Action<dynamic> item in pipelines)
            {
                item(jObject);
            }
        }

        protected virtual void Before()
        {
            // Executed before Run()
            System.Console.WriteLine("Running Middlewares");

            // System.Console.WriteLine("Before execute");
            Ensure.That(url).IsNotNullOrWhiteSpace();

            // Getting Url
            DownloaderMiddleware download = new DownloaderMiddleware()
            {
                RedisConnectrionstring = GlobalSettings.RedisConnectionString,
                UseRedisCache = GlobalSettings.UseRedisCache
            };

            sourceCode = download.RunAsync(url).Result;
        }

        protected abstract string FollowPage();

        // Implement to execute code
        protected abstract IEnumerable<dynamic> OnRun();

        public void AddDasherizePipeline()
        {
            this.AddPipeline(it =>
            {
                Helpers.Json.Rename((JObject)it, name => name.ToString().Underscore().Replace("_", "-"));
            });
        }

        public void AddPipeline(Action<dynamic> act)
        {
            this.pipelines.Add(act);
        }

        public void AddPrintToConsolePipeline(params string[] fields)
        {
            this.AddPipeline(it =>
            {
                if (fields == null || fields.Count() == 0)
                {
                    JObject obj = JObject.FromObject(it);
                    Console.WriteLine(obj);
                }
                else
                {
                    foreach (var item in fields)
                    {
                        if (it[item] != null)
                            Console.WriteLine($"{item} => {it[item].Value}");
                    }
                }
            });
        }

        public void AddRenameFields(string from, string to)
        {
            this.AddPipeline(it =>
            {
                Helpers.Json.RenameProperty((JObject)it, from, to);
            });
        }

        public void AddSaveToMongoAsyncPipeline(string collection, string primaryKeyField)
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
            });
        }

        public string GetUrl()
        {
            return this.url;
        }

        public bool HasFollowPage()
        {
            return !string.IsNullOrEmpty(this.FollowPage());
        }

        public void Run()
        {
            do
            {
                System.Console.WriteLine("Starting... " + this.SpiderName);
                Before();

                this.node = new Nodes(sourceCode);

                System.Console.WriteLine("Running... " + this.SpiderName);
                var obj = OnRun();

                foreach (var item in obj)
                {
                    After(item);
                }

                System.Console.WriteLine("Stopping... " + this.SpiderName);

                if (HasFollowPage())
                    this.SetUrl(FollowPage());
            } while (this.HasFollowPage());
        }

        public void SetUrl(string url)
        {
            this.url = url;
        }
    }
}