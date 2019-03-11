using Newtonsoft.Json.Linq;

namespace SpiderSharp
{
    public partial class SpiderContext
    {
        public void RunCleanUrlBookmarkPipeline(params string[] tokens)
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
                            jarr[i] = SpiderSharp.Helpers.Url.ReplaceBookmark(jarr[i].ToString(), string.Empty);
                        }
                    }
                }
                else if (jtoken.Type == JTokenType.String)
                {
                    string link = jtoken.Value<string>().ToString();

                    JValue property = (JValue)jtoken;
                    property.Value = SpiderSharp.Helpers.Url.ReplaceBookmark(link, string.Empty);
                }
            }

            this.Data = json;
        }
    }
}