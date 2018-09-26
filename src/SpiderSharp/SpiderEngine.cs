using EnsureThat;
using Humanizer;
using Newtonsoft.Json.Linq;
using Data.MongoDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;

namespace SpiderSharp
{
    public abstract partial class SpiderEngine
    {
        public DownloaderMiddleware downloader = null;
        readonly List<Func<dynamic, dynamic>> pipelines = new List<Func<dynamic, dynamic>>();
        private string sourceCode;
        private string url;
        public string Cookies { get; set; }
        protected Nodes node;
        protected JToken Json { get; set; }
        public string SpiderName { get; private set; }
        protected dynamic result = null;

        protected SpiderContext ct;


        protected SpiderEngine()
        {
            this.SpiderName = this.GetType().Name.Underscore().Replace("_", "-");
        }

        public void SetDownloader(DownloaderMiddleware requester)
        {
            this.downloader = requester;
        }

        protected virtual void After(dynamic jObject)
        {
            // Executed after Run()
            foreach (Func<dynamic, dynamic> item in pipelines)
            {
                Console.WriteLine("Executing Pipeline > " + item.Method.Name.ToString());
                jObject = item(jObject);
            }
        }

        protected virtual void Before()
        {
            // Executed before Run()
            System.Console.WriteLine("Executing Middlewares");

            Ensure.That(url).IsNotNullOrWhiteSpace();
            // Ensure.That(downloader).IsNotNull();

            if (downloader == null)
            {
                downloader = new DownloaderMiddleware();
                downloader.HttpProvider = GlobalSettings.HttpProvider ?? HttpRequester.EnumHttpProvider.HttpClient;
                downloader.UseRedisCache = GlobalSettings.UseRedisCache ?? false;
            }

            sourceCode = this.downloader.RunAsync(url).Result;
            Cookies = this.downloader.Cookies;
        }

        protected virtual string FollowPage()
        {
            return null;
        }

        // Implement to execute code
        protected abstract IEnumerable<SpiderContext> OnRun();
        
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

                // Verify if is Html or Json
                try
                {
                    // check if json
                    this.Json = JToken.Parse(sourceCode);
                }
                catch (Exception ex)
                {

                }

                try
                {
                    this.node = new Nodes(sourceCode);
                }
                catch (Exception ex)
                {

                }

                System.Console.WriteLine("Running... " + this.SpiderName);
                result = new ExpandoObject();
                ct = new SpiderContext();
                var obj = OnRun();

                foreach (var item in obj)
                {
                    ct = new SpiderContext();

                    if (item.Error == null)
                        //After(item.Data);
                        SuccessPipeline(item);
                    else
                        ErrorPipeline(item);
                }

                System.Console.WriteLine("Finish... " + this.SpiderName);

                if (HasFollowPage())
                    this.SetUrl(FollowPage());
            } while (this.HasFollowPage());
        }

        protected virtual void SuccessPipeline(SpiderContext context)
        {
            Console.WriteLine("Done Item");
        }

        protected virtual void ErrorPipeline(SpiderContext context)
        {
            Console.WriteLine(context.Error.ToString());
        }

        public void SetUrl(string url)
        {
            this.url = url;
        }
    }
}