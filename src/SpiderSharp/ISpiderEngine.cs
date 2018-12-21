using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SpiderSharp
{
    public interface ISpiderEngine
    {
        Task<bool> RunAsync();
        void AddBag(string key, object value);
        void SetUrl(string url);
        void SetNofollow();
    }
}