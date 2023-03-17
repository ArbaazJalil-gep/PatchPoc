using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

public class Patcher
{
    public JArray GenerateJsonPatches(JToken original, JToken modified)
    {
        var patches = new JArray();

        Compare(original, modified, patches, "");

        return patches;
    }

    private void Compare(JToken originalToken, JToken modifiedToken, JArray patches, string path)
    {
        if (JToken.DeepEquals(originalToken, modifiedToken))
        {
            return;
        }

        if (originalToken.Type != modifiedToken.Type)
        {
            AddReplacePatch(patches, path, modifiedToken);
            return;
        }

        switch (originalToken.Type)
        {
            case JTokenType.Object:
                CompareObjects((JObject)originalToken, (JObject)modifiedToken, patches, path);
                break;
            case JTokenType.Array:
                CompareArrays((JArray)originalToken, (JArray)modifiedToken, patches, path);
                break;
            default:
                AddReplacePatch(patches, path, modifiedToken);
                break;
        }
    }

    private void CompareObjects(JObject originalObject, JObject modifiedObject, JArray patches, string path)
    {
        foreach (var pair in modifiedObject)
        {
            var propertyName = pair.Key;
            var originalPropertyValue = originalObject.Property(propertyName);
            var modifiedPropertyValue = pair.Value;

            if (originalPropertyValue == null)
            {
                AddAddPatch(patches, $"{path}/{propertyName}", modifiedPropertyValue);
            }
            else
            {
                Compare(originalPropertyValue.Value, modifiedPropertyValue, patches, $"{path}/{propertyName}");
            }
        }

        foreach (var pair in originalObject)
        {
            if (modifiedObject.Property(pair.Key) !=null && !modifiedObject.Property(pair.Key).HasValues)
            {
                AddRemovePatch(patches, $"{path}/{pair.Key}");
            }
        }
    }

    private void CompareArrays(JArray originalArray, JArray modifiedArray, JArray patches, string path)
    {
        var minLength = Math.Min(originalArray.Count, modifiedArray.Count);

        for (var i = 0; i < minLength; i++)
        {
            Compare(originalArray[i], modifiedArray[i], patches, $"{path}/{i}");
        }

        if (originalArray.Count > modifiedArray.Count)
        {
            for (var i = modifiedArray.Count; i < originalArray.Count; i++)
            {
                AddRemovePatch(patches, $"{path}/{i}");
            }
        }
        else if (originalArray.Count < modifiedArray.Count)
        {
            for (var i = originalArray.Count; i < modifiedArray.Count; i++)
            {
                AddAddPatch(patches, $"{path}/{i}", modifiedArray[i]);
            }
        }
    }

    private void AddAddPatch(JArray patches, string path, JToken value)
    {
        patches.Add(new JObject
        {
            { "op", "add" },
            { "path", path },
            { "value", value }
        });
    }

    private void AddRemovePatch(JArray patches, string path)
    {
        patches.Add(new JObject
        {
            { "op", "remove" },
            { "path", path }
        });
    }

    private void AddReplacePatch(JArray patches, string path, JToken value)
    {
        patches.Add(new JObject
        {
            { "op", "replace" },
            { "path", path },
            { "value", value }
        });
    }
    public JToken ApplyPatchesToObject(JToken obj, JArray patches)
    {
        foreach (JObject patch in patches)
        {
            string path = ConvertJsonPointerToSelectToken(patch["path"].ToString());
            JToken value = patch["value"];

            switch (patch["op"].ToString())
            {
                case "add":
                    JToken currentObj = obj.SelectToken(path);
                    if (currentObj != null)
                    {
                        JProperty currentProp = (JProperty)currentObj;
                        JArray parent = (JArray)currentProp.Value;

                        int index = int.Parse(path.Split('/').Last());
                        if (index < parent.Count)
                        {
                            currentProp.AddAfterSelf(new JProperty(path.Split('/').Last(), value));
                        }
                        else
                        {
                            parent.Add(value);
                        }
                    }
                    break;

                case "remove":
                    obj.SelectToken(path)?.Parent.Remove();
                    break;

                case "replace":
                    obj.SelectToken(path)?.Replace(value);
                    break;
            }
        }

        return obj;
    }


    public JToken ApplyPatchesToObjectChatGpt(JToken obj, JArray patches)
    {
        foreach (JObject patch in patches)
        {
            string path = ConvertJsonPointerToSelectToken(patch["path"].ToString());
            JToken value = patch["value"];

            switch (patch["op"].ToString())
            {
                case "add":
                    JToken currentObj = obj.SelectToken(path);
                    if (currentObj != null)
                    {
                        JProperty currentProp = (JProperty)currentObj;
                        JArray parent = (JArray)currentProp.Value;

                        string indexString = Regex.Match(path, @"\d+").Value;
                        int index = int.Parse(indexString);

                        if (index < parent.Count)
                        {
                            currentProp.AddAfterSelf(new JProperty(indexString, value));
                        }
                        else
                        {
                            parent.Add(value);
                        }
                    }
                    else
                    {
                        string[] pathSegments = path.Split('/');
                        string indexString =  Regex.Match(pathSegments.Last(), @"\d+").Value;
                        int index = int.Parse(indexString);
                        string propName = pathSegments.Last().Split(".")[1]; 
                        JArray parent = (JArray)obj.SelectToken(string.Join("/", pathSegments.Take(pathSegments.Length - 1)));

                        // check if an object already exists at the given index
                        JToken existingObj = parent.ElementAtOrDefault(index);
                        if (existingObj != null)
                        {
                           
                            if (hasIdProperty(parent))
                            {
                                // if an object already exists, and has Id property that matches current id , add the new property to it
                                JObject existingJObj = (JObject)existingObj;
                                existingJObj.Add(new JProperty(propName, value));
                            }


                            
                        }
                        else
                        {
                            // if an object does not exist, create a new object and add the property to it
                            JObject newJObj = new JObject();
                            newJObj.Add(new JProperty(propName, value));
                            parent.Insert(index, newJObj);
                        }
                    }
                    break;

                case "remove":
                    obj.SelectToken(path)?.Parent.Remove();
                    break;

                case "replace":
                    obj.SelectToken(path)?.Replace(value);
                    break;
            }
        }

        return obj;
    }

    private static bool hasIdProperty(JToken parent)
    {
        var matchingItem = parent?.FirstOrDefault(x => x["Id"]?.ToString() == "1001");

        return matchingItem != null;
    }
    private static string ConvertJsonPointerToSelectToken(string path)
    {
        var parts = path.Split('/');
        var result = new StringBuilder();

        foreach (var part in parts)
        {
            if (string.IsNullOrEmpty(part)) continue;
            if (int.TryParse(part, out var index))
            {
                result.Append($"[{index}]");
            }
            else
            {
                if (result.Length > 0) result.Append('.');
                result.Append(part);
            }
        }

        return result.ToString();
    }



}
