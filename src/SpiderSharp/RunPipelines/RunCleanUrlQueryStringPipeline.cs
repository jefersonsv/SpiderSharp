﻿using Newtonsoft.Json.Linq;

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
                    JArray jarr = (JArray)jtoken;

                    for (int i = 0; i < jarr.Count; i++)
                    {
                        if (jarr[i].Type == JTokenType.String)
                        {
                            jarr[i] = SpiderSharp.Helpers.Url.RemoveQueryString(jarr[i].ToString());
                        }
                    }
                }
                else if (jtoken.Type == JTokenType.String)
                {
                    string link = jtoken.Value<string>().ToString();

                    JValue property = (JValue)jtoken;
                    property.Value = SpiderSharp.Helpers.Url.RemoveQueryString(link);
                }
            }

            this.Data = json;
        }
    }
}