using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace AngleSharp.Dom
{
    public static class AngleSharpExtension
    {
        public static JObject GetJson(this IElement doc, bool addPath = false)
        {
            JObject obj = new JObject();
            obj.Add(new JProperty(doc?.NodeName, IElementToJObject(doc)));

            if (addPath)
            {
                var idx = 0;
                JObject path = new JObject();
                foreach (JToken item in obj.DescendantsAndSelf())
                {
                    if (item.Type == JTokenType.Property)
                    {
                        var prop = (JProperty)item;
                        if (!prop.Name.StartsWith("@") && !prop.Name.StartsWith("#"))
                        {
                            idx++;
                            path.Add(prop.Name + "_" + idx, item.Path.ToLower().Replace(".", " > "));
                        }
                    }
                }

                obj.Add("PATH", path);
            }

            return obj;
        }

        static JObject IElementToJObject(this IElement node)
        {
            JObject obj = new JObject();

            // child
            node?.ChildNodes.ToList().ForEach(a =>
            {
                // add childs
                if (a.NodeType == NodeType.Element)
                {
                    var no = IElementToJObject((IElement)a);
                    if (no.Children().Count() > 0)
                    {
                        // verifiy if is unique or array
                        if (node.ChildNodes.Any(w => w.NodeName == a.NodeName))
                        {
                            // is array

                            // verify if is first
                            if (obj.ContainsKey(a.NodeName))
                            {
                                // second, third
                                JArray arr = obj[a.NodeName] as JArray;
                                arr.Add(no);
                            }
                            else
                            {
                                // first
                                obj.Add(new JProperty(a.NodeName, new JArray(no)));
                            }
                        }
                        else
                        {
                            // is unique
                            obj.Add(new JProperty(a.NodeName, no));
                        }
                    }
                }
                else if (a.NodeType == NodeType.Text)
                {
                    if (!string.IsNullOrWhiteSpace(a.TextContent))
                    {
                        var ct = System.Web.HttpUtility.HtmlDecode(a.TextContent.Trim());
                        if (!obj.ContainsKey(a.NodeName))
                        {
                            obj.Add(new JProperty(a.NodeName, ct));
                        }
                        else
                        {
                            obj.Add(new JProperty(a.NodeName + "_" + obj.Count, ct));
                        }
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }
            });

            // add attributes
            node?.Attributes.ToList().ForEach(a => {

                if (!string.IsNullOrWhiteSpace(a.Value))
                {
                    string ct = System.Web.HttpUtility.HtmlDecode(a.Value.ToString());

                    // verifiy if is unique or array
                    if (node.Attributes.Any(w => w.Name == a.Name))
                    {
                        // is array

                        // verify if is first
                        if (obj.ContainsKey($"@{a.Name}"))
                        {
                            // second, third
                            JArray arr = obj[$"@{a.Name}"] as JArray;
                            arr.Add(ct);
                        }
                        else
                        {
                            // first
                            obj.Add(new JProperty($"@{a.Name}", new JArray(ct)));
                        }
                    }
                    else
                    {
                        // is single
                        obj.Add(new JProperty($"@{a.Name}", ct));
                    }
                }
            });

            return obj;
        }
    }
}
