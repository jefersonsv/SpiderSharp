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
using System.Diagnostics;

namespace SpiderSharp
{
    public abstract partial class SpiderEngine
    {
        public DownloaderMiddleware Downloader = new DownloaderMiddleware();
        readonly List<Func<dynamic, dynamic>> pipelines = new List<Func<dynamic, dynamic>>();
        string sourceCode;
        string url;
        bool nofollow;
        public string Cookies { get; private set; }
        protected Nodes node;
        protected JToken Json { get; set; }
        public string SpiderName { get; private set; }

        private Dictionary<string, object> ViewBag = new Dictionary<string, object>();

        public void AddBag(string key, object value)
        {
            if (ViewBag.ContainsKey(key))
                ViewBag[key] = value;
            else
                ViewBag.Add(key, value);

            if (ct != null)
                ct.Bag = JObject.FromObject(ViewBag);
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
            this.Downloader = requester;
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

            sourceCode = await this.Downloader.RunAsync(url);
            Cookies = this.Downloader.Cookies;
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


        public dynamic Fetch(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
#if DEBUG
                Debugger.Break();
#endif
                ct.Error = ex;
            }

            return ct;
        }


        string DebugFile;

        public async Task RunAsync()
        {
            try
            {
                SetupBeforeRun();

                var hasNextPage = false;
                do
                {
                    System.Console.WriteLine("Starting... " + this.SpiderName);
                    await BeforeAsync();

                    this.Json = SpiderSharp.Helpers.Json.TryParse(sourceCode);
                    this.node = SpiderSharp.Helpers.Html.TryParse(sourceCode);

#if DEBUG
                    if (this.node != null)
                    {
                        DebugFile = System.IO.Path.GetTempFileName() + ".html";
                        System.IO.File.WriteAllText(DebugFile, sourceCode);

                        Log.Debug($"Debug: {DebugFile}");
                    }
#endif

                    System.Console.WriteLine("Running... " + this.SpiderName);

                    ct = new SpiderContext();
                    ct.Url = this.url;
                    ct.Spider = this.SpiderName;
                    ct.Bag = JObject.FromObject(ViewBag);

                    var obj = OnRun();

                    foreach (var item in obj)
                    {
                        ct = new SpiderContext();
                        ct.Url = this.url;
                        ct.Spider = this.SpiderName;
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

                            Log.Error(ex, "Error looping spider engine");
                        }
                    }

                    System.Console.WriteLine("Finish... " + this.SpiderName);

                    if (!nofollow)
                    {
                        var nextPage = this.FollowPage();
                        hasNextPage = !string.IsNullOrEmpty(nextPage);
                        if (hasNextPage)
                        {
                            this.SetUrl(nextPage);
                        }
                    }

                } while (hasNextPage);

            }
            catch (Exception ex)
            {
                var ct = new SpiderContext();
                ct.Url = this.url;
                ct.Spider = this.SpiderName;
                ct.Bag = JObject.FromObject(ViewBag);

                await ErrorPipelineAsync(ct);
            }
        }

        protected async virtual Task SuccessPipelineAsync(SpiderContext context)
        {
            Log.Debug("Sucess Pipeline");
        }

        protected async virtual Task ErrorPipelineAsync(SpiderContext context)
        {
            Log.Debug("Error Pipeline");
        }

        protected virtual void SetupBeforeRun()
        {
            Log.Debug("Setup for run");
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