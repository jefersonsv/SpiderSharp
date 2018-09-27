using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace SpiderSharp
{
    public partial class SpiderContext
    {
        public dynamic Data { get; set;  }
        public Exception Error { get; set;  }
        public bool HasError { get { return this.Error != null; } }

        public string Url { get; set; }

        public SpiderContext()
        {
            //dynamic json = new ExpandoObject();
            //this.Data = JObject.FromObject(new ExpandoObject());
            this.Data = new ExpandoObject();
        }
    }
}
