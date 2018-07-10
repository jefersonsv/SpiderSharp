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
    public  abstract partial class SpiderEngine
    {
        readonly List<Action<dynamic>> pipelines = new List<Action<dynamic>>();
        private string sourceCode;
        private string url;
        public string Cookies { get; private set; }
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

            Ensure.That(url).IsNotNullOrWhiteSpace();

            // Getting Url
            DownloaderMiddleware download = new DownloaderMiddleware()
            {
                RedisConnectrionstring = GlobalSettings.RedisConnectionString,
                UseRedisCache = GlobalSettings.UseRedisCache,
                HttpProvider = GlobalSettings.HttpProvider,
                DefaultHeaders = GlobalSettings.DefaultHeaders
            };

            sourceCode = download.RunAsync(url).Result;
            Cookies = download.Cookies;
        }

        protected abstract string FollowPage();

        // Implement to execute code
        protected abstract IEnumerable<dynamic> OnRun();
        
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