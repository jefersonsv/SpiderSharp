using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace SpiderSharp
{
    public partial class SpiderContext
    {
        public void RunCleanUrlQueryStringPipeline(params string[] tokens)
        {
            JObject json = JObject.FromObject(this.Data);

            foreach (var token in tokens)
            {
                JToken jtoken = json.SelectToken(token);

                if (jtoken.Type == JTokenType.Array)
                {
                    JArray jarr = (JArray) jtoken;

                    for (int i=0; i<jarr.Count; i++)
                    {
                        if (jarr[i].Type == JTokenType.String)
                        {
                            jarr[i] = SpiderSharp.Helpers.Url.ReplaceQueryString(jarr[i].ToString(), string.Empty);
                        }
                    }
                }
                else if (jtoken.Type == JTokenType.String)
                {
                    string link = jtoken.Value<string>().ToString().ToLower();

                    JValue property = (JValue)jtoken;
                    property.Value = SpiderSharp.Helpers.Url.ReplaceQueryString(link, string.Empty);
                }
            }

            this.Data = json;
        }
    }
}