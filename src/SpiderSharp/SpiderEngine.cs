using EnsureThat;
using Humanizer;
using Newtonsoft.Json.Linq;
using DataFoundation.MongoDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Serilog;
using System.Diagnostics;
using AngleSharp.Html.Dom;

namespace SpiderSharp
{
    public abstract partial class SpiderEngine
    {
        public DownloaderMiddleware Downloader = new DownloaderMiddleware(GlobalSettings.HttpProvider ?? HttpRequester.EnumHttpProvider.HttpClient);
        readonly List<Func<dynamic, dynamic>> pipelines = new List<Func<dynamic, dynamic>>();
        string sourceCode;
        string url;
        bool nofollow;
        public string Cookies { get; private set; }

        protected Nodes node;

        protected IHtmlDocument AngleDocument { get; set; }

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

        public async Task<bool> RunAsync()
        {
            Log.Information($"Starting RunAsync ... {this.SpiderName}");
            try
            {
                this.ct = new SpiderContext();
                ct.Url = this.url;
                ct.Spider = this.SpiderName;
                ct.Bag = JObject.FromObject(ViewBag);

                await SetupBeforeRunAsync();

                var hasNextPage = false;
                do
                {
                    await RunDownloaderAsync();

                    Log.Information("Running... " + this.SpiderName);
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
                        hasNextPage = !string.IsNullOrEmpty(nextPage) && nextPage != url;
                        if (hasNextPage)
                        {
                            this.SetUrl(nextPage);
                        }
                    }

                } while (hasNextPage);

                return true;
            }
            catch (Exception ex)
            {
                ct.Data.SpiderEngine = JsonConvert.SerializeObject(this);
                ct.Data.Exception = JsonConvert.SerializeObject(ex);
                //ct.RunEmbedMetadataPipeline();
                Log.Error(ex, ct.Data.ToString());
            }

            return false;
        }

        protected async virtual Task SuccessPipelineAsync(SpiderContext context)
        {
            Log.Debug("Sucess Pipeline");
        }

        protected async virtual Task ErrorPipelineAsync(SpiderContext context)
        {
            Log.Debug("Error Pipeline");
        }

        protected async virtual Task SetupBeforeRunAsync()
        {
            Log.Debug("Setup for run");
        }

        protected async virtual Task RunDownloaderAsync()
        {
            Log.Information($"URL: {url}");
            await BeforeAsync();

            this.Json = SpiderSharp.Helpers.Json.TryParse(sourceCode);
            this.node = SpiderSharp.Helpers.Html.TryParse(sourceCode);
            this.AngleDocument = SpiderSharp.AngleDocument.TryParse(sourceCode);

#if DEBUG
            if (this.node != null)
            {
                Log.Debug($"URL: {url}");
                DebugFile = System.IO.Path.GetTempFileName() + ".html";
                System.IO.File.WriteAllText(DebugFile, sourceCode);

                Log.Debug($"Debug: {DebugFile}");
            }
#endif
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