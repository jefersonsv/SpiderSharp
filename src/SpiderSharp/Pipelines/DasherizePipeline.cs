﻿using Humanizer;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpiderSharp
{
    public static partial class Pipelines
    {
        internal static void DasherizePipeline(ref dynamic item)
        {
            JObject json = (JObject)item;

            var renamedJson = Helpers.Json.CloneRenaming(json, name => name.Underscore().Replace("_", "-"));

            dynamic dyn = renamedJson;
            item = dyn;
        }
    }
}