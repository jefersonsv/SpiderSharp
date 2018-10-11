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
        public void RunSafeUrlsPipeline(string domain, params string[] tokens)
        {
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
                            if (!jarr[i].ToString().ToLower().StartsWith(domain.ToLower()))
                            {
                                jarr[i] = $"{domain}{jarr[i].ToString()}";
                            }
                        }
                    }
                }
                else if (jtoken.Type == JTokenType.String)
                {
                    string link = jtoken.Value<string>().ToString().ToLower();

                    if (!string.IsNullOrEmpty(link))
                    {
                        domain = domain.EndsWith("/") ? domain : domain + "/";

                        // add schema/domain/port if needed
                        if (!link.StartsWith(domain.ToLower().Trim()))
                        {
                            var concat = jtoken.Value<string>().ToString();
                            if (concat.StartsWith("/"))
                            {
                                concat = concat.Substring(1);
                            }

                            link = domain + concat;

                            Uri uri = null;
                            if (Uri.TryCreate(link, UriKind.RelativeOrAbsolute, out uri))
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