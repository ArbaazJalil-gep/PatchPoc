using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace PatchEngine.Core
{
    public class Difference
    {
        public string Path { get; set; }
        public JToken LeftValue { get; set; }
        public JToken RightValue { get; set; }
    }

    public class JsonComparer
    {
        public static JArray Compare(JToken left, JToken right, string path = "")
        {
            JArray differences = new JArray();

            if (left.Type != right.Type)
            {
                differences.Add(new JObject
                {
                    ["Path"] = path,
                    ["LeftValue"] = left,
                    ["RightValue"] = right
                });
            }
            else if (left.Type == JTokenType.Object)
            {
                var leftObject = (JObject)left;
                var rightObject = (JObject)right;

                var allKeys = leftObject.Properties().Select(p => p.Name)
                    .Union(rightObject.Properties().Select(p => p.Name));

                foreach (string key in allKeys)
                {
                    string childPath = path == "" ? key : $"{path}.{key}";

                    if (leftObject.TryGetValue(key, out JToken leftValue))
                    {
                        if (rightObject.TryGetValue(key, out JToken rightValue))
                        {
                            differences.Merge(Compare(leftValue, rightValue, childPath));
                        }
                        else
                        {
                            differences.Add(new JObject
                            {
                                ["Path"] = childPath,
                                ["LeftValue"] = leftValue,
                                ["RightValue"] = null
                            });
                        }
                    }
                    else
                    {
                        differences.Add(new JObject
                        {
                            ["Path"] = childPath,
                            ["LeftValue"] = null,
                            ["RightValue"] = rightObject[key]
                        });
                    }

                }
            }
            else if (left.Type == JTokenType.Array)
            {
                var leftArray = (JArray)left;
                var rightArray = (JArray)right;

                var visitedRightItems = new HashSet<int>();

                if (isArrayWithoutId(leftArray))
                {
                   // leftArray.MergeRegularArrays()
                }

                for (int leftIndex = 0; leftIndex < leftArray.Count; leftIndex++)
                {
                    string childPath = $"{path}:{leftArray[leftIndex]["Id"]?.ToString()}";



                    bool foundMatch = false;

                    for (int rightIndex = 0; rightIndex < rightArray.Count; rightIndex++)
                    {
                        if (visitedRightItems.Contains(rightIndex))
                        {
                            continue;
                        }

                        if (leftArray[leftIndex]["Id"]?.ToString() == rightArray[rightIndex]["Id"]?.ToString())
                        {

                            foundMatch = true;
                            visitedRightItems.Add(rightIndex);
                            differences.Merge(Compare(leftArray[leftIndex], rightArray[rightIndex], childPath));
                            break;
                        }
                    }

                    if (!foundMatch)
                    {
                        differences.Add(new JObject
                        {
                            ["Path"] = $"{path}:{leftArray[leftIndex]["Id"]}",
                            ["LeftValue"] = leftArray[leftIndex],
                            ["RightValue"] = null
                        });
                    }
                }

                for (int rightIndex = 0; rightIndex < rightArray.Count; rightIndex++)
                {
                   

                    if (!visitedRightItems.Contains(rightIndex))
                    {
                        differences.Add(new JObject
                        {
                            ["Path"] = $"{path}:{rightArray[rightIndex]["Id"]}",
                            ["LeftValue"] = null,
                            ["RightValue"] = rightArray[rightIndex]
                        });
                    }
                }
            }
            else if (!JToken.DeepEquals(left, right))
            {
                differences.Add(new JObject
                {
                    ["Path"] = path,
                    ["LeftValue"] = left,
                    ["RightValue"] = right
                });
            }           

            return differences;
        }

        private static bool isArrayWithoutId(JArray leftArray)
        {
            foreach (var item in leftArray)
            {
                if (item is JObject obj && obj.ContainsKey("Id"))
                {
                    return false;
                }
            }

            return true;
        }

        public static JToken Merge(JToken left , List<Difference> selectedDiffs)
        {
            JToken merged = left.DeepClone();
            foreach (var diff in selectedDiffs)
            {
                JToken parent = GetParent(merged, diff.Path);
                string propertyName = GetPropertyName(diff.Path);

                if (parent != null)
                {
                    if (parent.Type == JTokenType.Object)
                    {
                        JObject parentObject = (JObject)parent;
                        parentObject[propertyName] = diff.RightValue;
                    }
                    else if (parent.Type == JTokenType.Array)
                    {
                        JArray parentArray = (JArray)parent;
                        int index = parentArray.IndexOf(parentArray.FirstOrDefault(x => x["Id"]?.ToString() == propertyName));

                        if (index >= 0)
                        {
                            parentArray[index] = diff.RightValue.HasValues ? diff.RightValue :diff.LeftValue;
                        }
                        else
                        {
                                parentArray.Add(diff.RightValue);
                        }
                    }
                }
            }

            return merged;
        }


        private static JToken GetParent(JToken token, string path)
        {
                if (string.IsNullOrEmpty(path))
            {
                return null;
            }
          var p  = path.Split(".").ToList();
          
            if (p.Last().Contains(":"))
            {
                var l = p.Last().Split(':')[1];
                var f = p.Last().Split(':')[0];


                p[p.Count-1] = f;
                //p.Add(l);
            }
            else
            {
                if(p.Count>1)
                {
                    var secondlast = p[p.Count - 2];
                    if (secondlast.Contains(":")) p.RemoveAt(p.Count - 1);
                    else
                        p.RemoveAt(p.Count - 1);
                }
               
            }
              return  SelectTokenByPath(token, String.Join(".", p.ToArray()));
        }


        public static JToken SelectTokenByPath(JToken root, string path)
        {
            if (root == null || string.IsNullOrEmpty(path))
            {
                return null;
            }

            JToken currentToken = root;
            var parts = path.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var part in parts)
            {
                if (currentToken.Type == JTokenType.Object)
                {
                    currentToken = currentToken[part.Split(':')[0]];
                }

                if (currentToken != null && currentToken.Type == JTokenType.Array && part.Contains(":"))
                {
                    string id = part.Split(':')[1];
                    currentToken = currentToken.FirstOrDefault(x => x.Type == JTokenType.Object && x["Id"]?.ToString() == id);
                }

                if (currentToken == null)
                {
                    break;
                }
            }

            return currentToken;
        }

        private static string GetPropertyName(string path)
        {
            string[] parts = path.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            string lastPart = parts.LastOrDefault();
            return lastPart.Contains(":") ? lastPart.Split(':')[1] : lastPart;
        }

    }



}
