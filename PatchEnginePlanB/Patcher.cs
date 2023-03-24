using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
public static class Patcher
{
    public class JsonPatchOperation
    {
        public string Operation { get; set; }
        public string Path { get; set; }
        public JToken Value { get; set; }

        public override string ToString()
        {
            return $"Operation: {Operation}, Path: {Path}, Value: {Value}";
        }
    }

    public static List<JsonPatchOperation> GenerateOptimizedJsonPatch(JToken source, JToken target)
    {
        var patchOperations = new List<JsonPatchOperation>();
        GenerateOptimizedJsonPatchInternal(source, target, patchOperations, "");
        return patchOperations;
    }

    private static void GenerateOptimizedJsonPatchInternal(JToken source, JToken target, List<JsonPatchOperation> patchOperations, string currentPath)
    {
        if (source.Type != target.Type)
        {
            patchOperations.Add(new JsonPatchOperation { Operation = "replace", Path = currentPath, Value = target });
            return;
        }

        switch (source.Type)
        {
            case JTokenType.Object:
                var sourceObject = (JObject)source;
                var targetObject = (JObject)target;
                var allKeys = new HashSet<string>(sourceObject.Properties().Select(p => p.Name));
                allKeys.UnionWith(targetObject.Properties().Select(p => p.Name));

                foreach (var key in allKeys)
                {
                    if (sourceObject.TryGetValue(key, out var sourceValue))
                    {
                        if (targetObject.TryGetValue(key, out var targetValue))
                        {
                            GenerateOptimizedJsonPatchInternal(sourceValue, targetValue, patchOperations, $"{currentPath}/{key}");
                        }
                        else
                        {
                            patchOperations.Add(new JsonPatchOperation { Operation = "remove", Path = $"{currentPath}/{key}" });
                        }
                    }
                    else
                    {
                        patchOperations.Add(new JsonPatchOperation { Operation = "add", Path = $"{currentPath}/{key}", Value = targetObject[key] });
                    }
                }
                break;

            case JTokenType.Array:
                var sourceArray = (JArray)source;
                var targetArray = (JArray)target;

                // Check if both source and target arrays contain objects with an "Id" property
                bool containsIdProperty = sourceArray.All(x => x.Type == JTokenType.Object && x["Id"] != null) &&
                                          targetArray.All(x => x.Type == JTokenType.Object && x["Id"] != null);

                if (containsIdProperty)
                {
                    var sourceIdMap = sourceArray.ToDictionary(x => x["Id"].ToString(), x => x);
                    var targetIdMap = targetArray.ToDictionary(x => x.Type == JTokenType.Object && x["Id"] != null ? x["Id"].ToString() : Guid.NewGuid().ToString(), x => x);

                    foreach (var kvp in sourceIdMap)
                    {
                        if (targetIdMap.ContainsKey(kvp.Key))
                        {
                            GenerateOptimizedJsonPatchInternal(kvp.Value, targetIdMap[kvp.Key], patchOperations, $"{currentPath}/{targetArray.IndexOf(targetIdMap[kvp.Key])}");
                        }
                        else
                        {
                            patchOperations.Add(new JsonPatchOperation { Operation = "remove", Path = $"{currentPath}/{sourceArray.IndexOf(kvp.Value)}" });
                        }
                    }

                    foreach (var kvp in targetIdMap)
                    {
                        if (!sourceIdMap.ContainsKey(kvp.Key))
                        {
                            patchOperations.Add(new JsonPatchOperation { Operation = "add", Path = $"{currentPath}/-", Value = kvp.Value });
                        }
                    }
                }
                else
                {
                    var minLength = Math.Min(sourceArray.Count, targetArray.Count);
                    for (int i = 0; i < minLength; i++)
                    {
                        GenerateOptimizedJsonPatchInternal(sourceArray[i], targetArray[i], patchOperations, $"{currentPath}/{i}");
                    }

                    if (sourceArray.Count > targetArray.Count)
                    {
                        for (int i = targetArray.Count; i < sourceArray.Count; i++)
                        {
                            patchOperations.Add(new JsonPatchOperation { Operation = "remove", Path = $"{currentPath}/{targetArray.Count}" });
                        }
                    }
                    else if (targetArray.Count > sourceArray.Count)
                    {
                        for (int i = sourceArray.Count; i < targetArray.Count; i++)
                        {
                            patchOperations.Add(new JsonPatchOperation { Operation = "add", Path = $"{currentPath}/-", Value = targetArray[i] });
                        }
                    }
                }
                break;




            case JTokenType.Null:
                // Both tokens are null, no need to create a patch operation.
                break;

            default:
                if (!JToken.DeepEquals(source, target))
                {
                    patchOperations.Add(new JsonPatchOperation { Operation = "replace", Path = currentPath, Value = target });
                }
                break;
        }


    }
    public static JToken ApplyPatch(JToken source, List<JsonPatchOperation> patchOperations)
    {
        var patched = source.DeepClone();

        foreach (var operation in patchOperations)
        {
            var pathSegments = operation.Path.Split('/').Skip(1).ToArray();
            var path = string.Join("/", pathSegments);

            switch (operation.Operation)
            {
                case "add":
                case "replace":
                    JToken parentToken = GetParentToken(patched, pathSegments);
                    if (pathSegments.Last() == "-")
                    {
                        ((JArray)parentToken).Add(operation.Value);
                    }
                    else
                    {
                        int index = int.TryParse(pathSegments.Last(), out int tempIndex) ? tempIndex : -1;
                        if (index >= 0)
                        {
                            ((JArray)parentToken)[index] = operation.Value;
                        }
                        else
                        {
                            parentToken[pathSegments.Last()] = operation.Value;
                        }
                    }
                    break;
                case "remove":
                    JToken tokenToRemove = GetTokenByPath(patched, pathSegments);
                    if (tokenToRemove != null)
                    {
                        tokenToRemove.Remove();
                    }
                    break;
                default:
                    throw new InvalidOperationException($"Unknown patch operation: {operation.Operation}");
            }
        }

        return patched;
    }


    private static JToken GetParentToken(JToken root, string[] pathSegments)
    {
        JToken currentToken = root;
        for (int i = 0; i < pathSegments.Length - 1; i++)
        {
            if (int.TryParse(pathSegments[i], out int index))
            {
                // Handle array index
                currentToken = currentToken[index];
            }
            else
            {
                // Handle object property
                currentToken = currentToken[pathSegments[i]];
            }
        }
        return currentToken;
    }


    private static JToken GetTokenByPath(JToken root, string[] pathSegments)
    {
        JToken currentToken = root;
        for (int i = 0; i < pathSegments.Length; i++)
        {
            if (int.TryParse(pathSegments[i], out int index))
            {
                // Handle array index
                currentToken = currentToken[index];
            }
            else
            {
                // Handle object property
                currentToken = currentToken[pathSegments[i]];
            }
        }
        return currentToken;
    }




}
