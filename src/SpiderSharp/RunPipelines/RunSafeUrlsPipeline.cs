using Newtonsoft.Json.Linq;
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
            var it = this.Data;
            JObject json = JObject.FromObject(it);

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
                            var concat = prop.Value.ToString();
                            if (concat.StartsWith("/"))
                            {
                                concat = concat.Substring(1);
                            }

                            link = domain + concat;

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
            this.Data = dyn;
        }
    }
}