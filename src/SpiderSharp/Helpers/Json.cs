using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Linq;

namespace SpiderSharp.Helpers
{
    public static class Json
    {
        public static bool IsJson(string content)
        {
            return TryParse(content) != null;
        }

        public static JToken TryParse(string content)
        {
            try
            {
                // check if json
                return JToken.Parse(content);
            }
            catch
            {
            }

            return null;
        }

        public static JToken CloneRenaming(JToken json, Func<string, string> map)
        {
            var prop = json as JProperty;
            if (prop != null)
            {
                return new JProperty(map(prop.Name), CloneRenaming(prop.Value, map));
            }

            var arr = json as JArray;
            if (arr != null)
            {
                var cont = arr.Select(el => CloneRenaming(el, map));
                return new JArray(cont);
            }

            var o = json as JObject;
            if (o != null)
            {
                var cont = o.Properties().Select(el => CloneRenaming(el, map));
                return new JObject(cont);
            }

            return json;
        }

        public static JToken CloneRenaming(JToken json, Dictionary<string, string> map)
        {
            return CloneRenaming(json, name => map.ContainsKey(name) ? map[name] : name);
        }

        public static void Rename(JToken item, Func<string, string> map)
        {
            JObject json = (JObject)item;
            foreach (JProperty it in json.DescendantsAndSelf().Where(w => w.Type == JTokenType.Property).ToList())
            {
                if (it.Type == JTokenType.Property)
                {
                    var oldFieldName = it.Name;
                    var newFiledName = map(it.Name);
                    if (oldFieldName != newFiledName)
                    {
                        //it.Parent.Add(new JProperty(map(it.Name), it.Value));
                        //it.Remove();
                        it.Replace(new JProperty(map(it.Name), it.Value));
                    }
                }
            }
        }

        public static void RenameProperty(JToken item, string from, string to)
        {
            JObject json = (JObject)item;
            var prop = json.Property(from);
            prop.Replace(new JProperty(to, prop.Value));
        }
    }
}