using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace PatchEngine.Controllers
{


    public static class JsonPatchGenerator
    {
        public static JArray GeneratePatch(JToken before, JToken after)
        {
            JArray patch = new JArray();

            // Find items that were added or modified
            if (after.Type == JTokenType.Object)
            {
                foreach (var item in after)
                {
                    JToken beforeValue;
                    if (((JObject)before).TryGetValue(item.Path, out beforeValue))
                    {
                        if (!JToken.DeepEquals(item, beforeValue))

                        {
                            patch.Add(new JObject
                            {
                                { "op", "replace" },
                                { "path", item.Path },
                                { "value", ((Newtonsoft.Json.Linq.JProperty)item).Value }
                            });
                        }
                    }
                    else
                    {
                        patch.Add(new JObject
                    {
                        { "op", "add" },
                        { "path", item.Path },
                        { "value", ((Newtonsoft.Json.Linq.JProperty)item).Value }
                    });
                    }
                }

                // Find items that were removed
                if (before.Type == JTokenType.Object)
                {
                    foreach (var item in before)
                    {
                        if (after[item.Path] == null)
                        {
                            patch.Add(new JObject
                        {
                            { "op", "remove" },
                            { "path", item.Path }
                        });
                        }
                    }
                }
            }
            else if (after.Type == JTokenType.Array)
            {
                for (int i = 0; i < after.Count(); i++)
                {
                    JToken beforeValue = (i < before.Count()) ? before[i] : null;
                    JToken afterValue = after[i];
                    JArray arrayPatch = GeneratePatch(beforeValue, afterValue);
                    foreach (var item in arrayPatch)
                    {
                        patch.Add(item);
                    }
                }
            }

            return patch;
        }
    }

}
