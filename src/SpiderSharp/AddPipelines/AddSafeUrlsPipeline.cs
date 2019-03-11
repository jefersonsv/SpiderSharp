using Newtonsoft.Json.Linq;

using System;

namespace SpiderSharp
{
    public abstract partial class SpiderEngine
    {
        [Obsolete("Use RunSafeUrlsPipeline in SpiderContext")]
        public void AddSafeUrlsPipeline(string domain, params string[] tokens)
        {
            this.AddPipeline(it =>
            {
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
                it = dyn;

                return dyn;
            });
        }
    }
}