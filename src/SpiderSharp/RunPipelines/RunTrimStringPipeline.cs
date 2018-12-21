using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace SpiderSharp
{
    public partial class SpiderContext
    {
        public void RunTrimStringPipeline(params string[] tokens)
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
                            jarr[i] = jarr[i].ToString().Trim();
                        }
                    }
                }
                else if (jtoken.Type == JTokenType.String)
                {
                    string link = jtoken.Value<string>().ToString();

                    JValue property = (JValue)jtoken;
                    property.Value = link.Trim();
                }
            }

            this.Data = json;
        }
    }
}