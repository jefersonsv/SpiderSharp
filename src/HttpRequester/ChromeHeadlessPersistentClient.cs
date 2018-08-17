using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Linq;
using Microsoft.Win32;
using System.Diagnostics;
using System.Threading.Tasks;
using PuppeteerSharp;
using System.Dynamic;
using Microsoft.AspNetCore.Routing;

namespace HttpRequester
{
    public class ChromeHeadlessPersistentClient
    {
        string chromeExecutable;
        string url;
        Browser browser;
        public Page page;

        // https://stackoverflow.com/questions/4291912/process-start-how-to-get-the-output
        public ChromeHeadlessPersistentClient()
        {
            TryFindChromeExecutable();

            //await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);

            // Starting
            browser = Puppeteer.LaunchAsync(new LaunchOptions
            {
                ExecutablePath = this.chromeExecutable,
                Headless = true,
                Args = new string[] { "--incognito", "--disable-extensions", "--safe-plugins", "--disable-translate" }
            }).Result;

            page = browser.PagesAsync().Result.First();
        }

        public async Task<Response> GoToAsync(string url)
        {
            this.url = url;
            return await page.GoToAsync(url);
        }

        public void WaitForSelector(string selector)
        {
            page.WaitForSelectorAsync(selector).Wait();
        }

        public async Task<string> GoAndGetContentAsync(string url)
        {
            await GoToAsync(url);
            return await GetContentAsync();
        }

        public async Task<string> GetContentAsync()
        {
            return await page.GetContentAsync();
        }

        public async Task TypeAsync(string selector, string text)
        {
            await page.WaitForSelectorAsync(selector);
            await page.TypeAsync(selector, text);
        }

        public async Task ClickAsync(string selector)
        {
            await page.WaitForSelectorAsync(selector);
            await page.ClickAsync(selector);
        }

        public void WaitNetworks()
        {
            NavigationOptions n = new NavigationOptions();
            n.WaitUntil = new WaitUntilNavigation[] { WaitUntilNavigation.Networkidle2 };

            page.WaitForNavigationAsync(n).Wait();
        }

        public IEnumerable<KeyValuePair<string, string>> PagesContent()
        {
            //var userAgent = this.browser.GetUserAgentAsync().Result;
            //var version = this.browser.GetVersionAsync().Result;
            //var last = this.browser.Targets().ToList().Last().PageAsync().Result;

            var pages = this.browser.PagesAsync().Result;
            foreach (var item in pages)
            {
                yield return new KeyValuePair<string, string>(item.Url, item.GetContentAsync().Result);
            }
        }

        public async Task<Newtonsoft.Json.Linq.JToken> ListCookiesAsync()
        {
            var cookies = await page.GetCookiesAsync(url);

            var obj = cookies.Select(s => new
            {
                Domain = s.Domain,
                Expires = s.Expires,
                HttpOnly = s.HttpOnly,
                Name = s.Name,
                Path = s.Path,
                Secure = s.Secure,
                Session = s.Session,
                Size = s.Size,
                Url = s.Url,
                Value = s.Value
            });

            return Newtonsoft.Json.Linq.JArray.FromObject(obj);
        }

        string GetChromeExecutableFromRegistry()
        {
            bool hasRegistry = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            if (hasRegistry)
            {
                var keyName = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe";
                return (string)Registry.GetValue(keyName, "Path", null);
            }

            return null;
        }

        public void TryFindChromeExecutable()
        {
            // https://stackoverflow.com/questions/17736215/universal-path-to-chrome-exe
            // %LOCALAPPDATA%
            // %programfiles(x86)%

            var paths = new string[]
            {
                Path.Combine(Environment.GetEnvironmentVariable("LOCALAPPDATA"), @"Google\Chrome", "chrome.exe"),
                Path.Combine(Environment.GetEnvironmentVariable("programfiles(x86)"), @"Google\Chrome\Application", "chrome.exe"),
                Path.Combine(Environment.GetEnvironmentVariable("programfiles"), @"Google\Chrome\Application", "chrome.exe"),
                GetChromeExecutableFromRegistry()
            };

            var idx = paths.ToList().FindIndex(w => File.Exists(w));
            this.chromeExecutable = idx > -1 ? paths[idx] : null;
        }

        public void SetChromeExecutable(string chromeFilename)
        {
            this.chromeExecutable = chromeFilename;
        }
    }
}
