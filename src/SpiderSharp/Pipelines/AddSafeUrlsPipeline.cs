using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpiderSharp
{
    public abstract partial class SpiderEngine
    {
        public void AddSafeUrlsPipeline(dynamic item, string domain, params string[] tokens)
        {
            JObject json = (JObject)item;

            foreach (var token in tokens)
            {
                JProperty prop = json.Property(token);

                if (prop != null && prop.HasValues)
                {
                    string link = prop.Value.ToString().ToLower();

                    if (!string.IsNullOrEmpty(link))
                    {
                        domain = domain.EndsWith("/") ? domain : domain + "/";

                        // add schema/domain/port if needed
                        if (!link.StartsWith(domain.ToLower().Trim()))
                        {
                            link = domain + prop.Value.ToString();

                            Uri uri = null;
                            if (Uri.TryCreate(link, UriKind.RelativeOrAbsolute, out uri))
                            {
                                prop.Value = uri.ToString();
                            }
                        }
                    }
                }
            }

            dynamic dyn = json;
            item = dyn;
        }
    }
}