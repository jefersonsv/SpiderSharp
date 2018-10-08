using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace SpiderSharp
{
    public partial class SpiderContext
    {
        public void RunHtmlDecode()
        {
            var it = this.Data;
            JObject json = JObject.FromObject(it);

            foreach (var token in json.Properties().Select(p => p.Name))
            {
                JProperty prop = json.Property(token);

                if (prop != null && prop.HasValues && prop.Type == JTokenType.Property && prop.Value.Type == JTokenType.String)
                {
                    prop.Value = System.Web.HttpUtility.HtmlDecode(prop.Value.ToString());
                }
            }

            this.Data = json;
        }
    }
}