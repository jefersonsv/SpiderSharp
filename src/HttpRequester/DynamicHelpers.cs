using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using Microsoft.AspNetCore.Routing;

namespace HttpRequester
{
    public static class DynamicHelpers
    {
        /// <summary>
        /// https://stackoverflow.com/questions/5120317/dynamic-anonymous-type-in-razor-causes-runtimebinderexception
        /// </summary>
        /// <param name="anonymousObject"></param>
        /// <returns></returns>
        public static ExpandoObject ToExpando(this object anonymousObject)
        {
            IDictionary<string, object> anonymousDictionary = new RouteValueDictionary(anonymousObject);
            IDictionary<string, object> expando = new ExpandoObject();
            foreach (var item in anonymousDictionary)
                expando.Add(item);

            return (ExpandoObject)expando;
        }
    }
}
