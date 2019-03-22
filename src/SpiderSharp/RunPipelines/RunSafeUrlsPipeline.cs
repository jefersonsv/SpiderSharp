using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace SpiderSharp
{
    public partial class SpiderContext
    {
        public void RunSafeUrlsPipeline(string baseUrl, params string[] tokens)
        {
            var baseUrlSlashed = baseUrl.EndsWith("/") ? baseUrl : baseUrl + "/";

            JObject json = JObject.FromObject(this.Data);

            foreach (var token in tokens)
            {
                JToken jtoken = json.SelectToken(token);

                if (jtoken == null)
                {
                    Log.Warning($"Token {token} don't is on context");
                    continue;
                }

                if (jtoken.Type == JTokenType.Array)
                {
                    JArray jarr = (JArray) jtoken;

                    for (int i=0; i<jarr.Count; i++)
                    {
                        if (jarr[i].Type == JTokenType.String)
                        {
                            if (!jarr[i].ToString().ToLower().StartsWith(baseUrlSlashed.ToLower()))
                            {
                                jarr[i] = $"{baseUrlSlashed}{jarr[i].ToString().TrimStart('/')}";
                            }
                        }
                    }
                }
                else if (jtoken.Type == JTokenType.String)
                {
                    string link = jtoken.ToString().ToLower();

                    if (!string.IsNullOrEmpty(link))
                    {
                        // add schema/domain/port if needed
                        if (!link.StartsWith(baseUrlSlashed.ToLower().Trim()))
                        {
                            var concat = jtoken.ToString().TrimStart('/');
                            if (Uri.TryCreate(baseUrlSlashed + concat, UriKind.RelativeOrAbsolute, out Uri uri))
                            {
                                JValue property = (JValue)jtoken;
                                property.Value = uri.ToString();
                            }
                            else
                            {
                                throw new Exception($"Cannot parse uri {uri}");
                            }
                        }
                    }
                }
            }

            this.Data = json;
        }
    }
}