using Humanizer;

using Newtonsoft.Json.Linq;

namespace SpiderSharp
{
    public partial class SpiderContext
    {
        public void RunDasherizePipeline()
        {
            var it = this.Data;
            JObject obj = JObject.FromObject(it);
            Helpers.Json.Rename(obj, name => name.ToString().Underscore().Replace("_", "-"));
            this.Data = obj;
        }
    }
}