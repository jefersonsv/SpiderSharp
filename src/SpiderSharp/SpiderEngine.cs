using EnsureThat;
using Humanizer;
using Newtonsoft.Json.Linq;
using Data.MongoDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Serilog;

namespace SpiderSharp
{
    public abstract partial class SpiderEngine
    {
        public DownloaderMiddleware downloader = null;
        readonly List<Func<dynamic, dynamic>> pipelines = new List<Func<dynamic, dynamic>>();
        string sourceCode;
        string url;
        bool nofollow;

        public string Cookies { get; set; }
        protected Nodes node;
        protected JToken Json { get; set; }
        public string SpiderName { get; private set; }

        private Dictionary<string, object> ViewBag = new Dictionary<string, object>();

        public void AddBag(string key, string value)
        {
            ViewBag.Add(key, value);
            if (ct != null)
                ct.Bag[key] = value;
        }

        protected SpiderContext ct;

        public SpiderEngine()
        {
            this.SpiderName = this.GetType().Name.Underscore().Replace("_", "-");
        }

        public SpiderEngine(string url)
        {
            this.SpiderName = this.GetType().Name.Underscore().Replace("_", "-");
            this.SetUrl(url);
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

        protected async virtual Task BeforeAsync()
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
                downloader.RedisConnectrionstring = GlobalSettings.RedisConnectionString ?? null;
            }

            sourceCode = await this.downloader.RunAsync(url);
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
            if (this.nofollow)
                return false;

            return !string.IsNullOrEmpty(this.FollowPage());
        }

        public dynamic Fetch(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                ct.Error = ex;
            }

            return ct;
        }

        public async Task RunAsync()
        {
            do
            {
                System.Console.WriteLine("Starting... " + this.SpiderName);
                await BeforeAsync();

                this.Json = SpiderSharp.Helpers.Json.TryParse(sourceCode);
                this.node = SpiderSharp.Helpers.Html.TryParse(sourceCode);

                System.Console.WriteLine("Running... " + this.SpiderName);
                
                ct = new SpiderContext();
                ct.Url = this.url;
                ct.Bag = JObject.FromObject(ViewBag);

                var obj = OnRun();

                foreach (var item in obj)
                {
                    ct = new SpiderContext();
                    ct.Url = this.url;
                    ct.Bag = JObject.FromObject(ViewBag);

                    try
                    {
                        // After(item.Data);
                        if (item.Error == null)
                            await SuccessPipelineAsync(item);
                        else
                            await ErrorPipelineAsync(item);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(JsonConvert.SerializeObject(this));
                        Console.WriteLine(JsonConvert.SerializeObject(item));
                        Console.WriteLine(JsonConvert.SerializeObject(ex));

                        throw;
                    }
                }

                System.Console.WriteLine("Finish... " + this.SpiderName);

                if (HasFollowPage())
                    this.SetUrl(FollowPage());
            } while (this.HasFollowPage());
        }

        protected async virtual Task SuccessPipelineAsync(SpiderContext context)
        {
            Log.Debug("Sucess Pipeline");
        }

        protected async virtual Task ErrorPipelineAsync(SpiderContext context)
        {
            Log.Debug("Erros Pipeline");
        }

        public void SetUrl(string url)
        {
            this.url = url;
        }

        public void SetNofollow()
        {
            this.nofollow = true;
        }
    }
}