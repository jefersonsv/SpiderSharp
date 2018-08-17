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

namespace HttpRequester
{
    public class ChromeHeadlessClient
    {
        string chromeExecutable;

        // https://stackoverflow.com/questions/4291912/process-start-how-to-get-the-output
        public ChromeHeadlessClient()
        {
            TryFindChromeExecutable();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetContentAsync(string url)
        {
            //await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);
            using (var cli = new CliWrap.Cli(this.chromeExecutable))
            {
                var args = $"--headless --disable-gpu --dump-dom {url}";

                cli.Settings.Encoding = new CliWrap.Models.EncodingSettings()
                {
                    StandardOutput = System.Text.Encoding.UTF8
                };

                // Execute
                var output = await cli.ExecuteAsync(args);
                
                // Throw an exception if CLI reported an error
                // output.ThrowIfError();

                return output.StandardOutput;
                // Extract output
                //var code = output.ExitCode;
                //var stdOut = output.StandardOutput;
                //var stdErr = output.StandardError;
                //var startTime = output.StartTime;
                //var exitTime = output.ExitTime;
                //var runTime = output.RunTime;
            }
        }

        static void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            //* Do your stuff with the output (write to console/log/StringBuilder)
            Console.WriteLine(outLine.Data);
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
