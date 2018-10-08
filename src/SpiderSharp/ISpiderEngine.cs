using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SpiderSharp
{
    public interface ISpiderEngine
    {
        Task RunAsync();
        void AddBag(string key, string value);
        void SetUrl(string url);
    }
}