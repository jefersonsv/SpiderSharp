using Humanizer;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace SpiderSharp
{
    public partial class SpiderContext
    {
        public void RunRenameFieldsPipeline(string from, string to)
        {
            var it = this.Data;
            var jobject = JObject.FromObject(it);
            Helpers.Json.RenameProperty(jobject, from, to);
            it = jobject;
        }
    }
}