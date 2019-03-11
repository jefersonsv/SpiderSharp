using System;

namespace SpiderSharp
{
    public abstract partial class SpiderEngine
    {
        [Obsolete]
        public void AddPipeline(Func<dynamic, dynamic> act)
        {
            this.pipelines.Add(act);
        }
    }
}