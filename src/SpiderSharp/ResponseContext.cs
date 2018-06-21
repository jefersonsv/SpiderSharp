using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace SpiderSharp
{
    public sealed class ResponseContext
    {
        public dynamic Data { get; }
        public Exception Error { get; }
        public bool HasError { get { return this.Error != null; } }

        public ResponseContext()
        {
            this.Data = JObject.FromObject(new ExpandoObject());
        }

        public ResponseContext(dynamic data)
        {
            //this.Data = JObject.FromObject(data);
            this.Data = data;
        }

        public ResponseContext(dynamic data, Exception exception)
        {
            this.Data = data;
            this.Error = exception;
        }
    }
}
