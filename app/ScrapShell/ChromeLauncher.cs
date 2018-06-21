using Microsoft.Win32;
using System;
using System.Diagnostics;

namespace ScrapShell
{
    internal static class ChromeLauncher
    {
        private const string ChromeAppKey = @"\Software\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe";

        private static string ChromeAppFileName
        {
            get
            {
                return (string)(Registry.GetValue("HKEY_LOCAL_MACHINE" + ChromeAppKey, "", null) ??
                                    Registry.GetValue("HKEY_CURRENT_USER" + ChromeAppKey, "", null));
            }
        }

        public static void OpenLink(string url)
        {
            string chromeAppFileName = ChromeAppFileName;
            if (string.IsNullOrEmpty(chromeAppFileName))
            {
                throw new TypeAccessException("Could not find chrome.exe!");
            }
            Process.Start(chromeAppFileName, "--disable-javascript " + url);
        }
    }
}