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
    }
}