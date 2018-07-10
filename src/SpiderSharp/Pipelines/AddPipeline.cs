using Humanizer;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpiderSharp
{
    public abstract partial class SpiderEngine
    {
        public void AddPipeline(Action<dynamic> act)
        {
            this.pipelines.Add(act);
        }
    }
}