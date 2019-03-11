using Newtonsoft.Json.Linq;

using System.Linq;

namespace SpiderSharp
{
    public partial class SpiderContext
    {
        public void RunHtmlDecodePipeline()
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