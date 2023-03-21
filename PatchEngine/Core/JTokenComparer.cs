using Newtonsoft.Json.Linq;
using PatchEngine.Implementations;
using PatchEngine.Interfaces;
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

    public class JsonComparer : IJsonComparer
    {
        public JArray Compare(JToken left, JToken right, string path = "")
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

                differences.Merge(new SimpleObjectComparer(left, right, path).Compare());
            }
            else if (left.Type == JTokenType.Array)
            {

                differences.Merge(new ArrayWithIdentityObjects(left, right, path).Compare());
            }
            else if (!JToken.DeepEquals(left, right)) // simple values
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



        public JToken Merge(JToken left, List<Difference> selectedDiffs, int level = 0)
        {
            JToken merged = left.DeepClone();
            int idx = 0;
            foreach (var diff in selectedDiffs)
            {
                JToken parent = GetParent(merged, diff.Path);
                string propertyName = "";
                if (isArrayIndexSegmet(diff.Path))
                {
                    propertyName = GetLastArrayIndexSegment(diff.Path); //get index [index]
                }
                else
                {
                    propertyName = GetPropertyName(diff.Path);
                }

                if (parent != null)
                {
                    if (parent.Type == JTokenType.Object)
                    {
                        JObject parentObject = (JObject)parent;
                        parentObject[propertyName] = diff.RightValue;
                    }
                    else if (parent.Type == JTokenType.Array)
                    {
                        if (((JArray)parent).isArrayWithoutId()) // simple array
                        {
                            //leftValue RightValue path


                            JArray parentArray = (JArray)parent;
                          
                             
                                 int  index = diff.Path.ReturnIndexValue(level + 1);
                             
                           

                          
                            if (parentArray.Count > index)
                            {
                                if(parent[index].Type == JTokenType.Array)
                                {
                                    parent[index] = Merge(parent[index], new List<Difference>() { diff }, level+1);
                                }
                                else
                                {
                                    parentArray[index] = diff.RightValue;
                                }
                            }
                            else
                            {
                                parentArray.Add(diff.RightValue);
                            }
                                

                        }
                        else
                        {
                            JArray parentArray = (JArray)parent;
                            int index = parentArray.IndexOf(parentArray.FirstOrDefault(x => x["Id"]?.ToString() == propertyName));

                            if (index >= 0)
                            {
                                parentArray[index] = diff.RightValue.HasValues ? diff.RightValue : diff.LeftValue;
                            }
                            else
                            {
                                parentArray.Add(diff.RightValue);
                            }
                        }

                    }
                }
                idx++;
            }

            return merged;
        }


        private static JToken GetParent(JToken token, string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            var p = path.Split(".").ToList();

            if (p.Last().Contains(":"))
            {
                var l = p.Last().Split(':')[1];
                var f = p.Last().Split(':')[0];


                p[p.Count - 1] = f;
                //p.Add(l);
            }
            else if (isArrayIndexSegmet(p.Last()))
            {
                p[p.Count - 1] = RemoveIndexFromString(p.Last()); //removing [index] from last segment
            }
            else
            {
                if (p.Count > 1)
                {
                    var secondlast = p[p.Count - 2];
                    if (secondlast.Contains(":")) p.RemoveAt(p.Count - 1);
                    else
                        p.RemoveAt(p.Count - 1);
                }
                else
                {
                    return SelectTokenByPath(token, "/"); // return root object
                }

            }
            string tokenPath = String.Join(".", p.ToArray());
            if (tokenPath == null || tokenPath == "") tokenPath = "/";
            return SelectTokenByPath(token, tokenPath);
        }

        private static string RemoveIndexFromString(string input)
        {
            // Find the position of the last opening square bracket '['
            int lastOpenBracket = input.LastIndexOf('[');

            // Check if the last opening square bracket '[' was found
            if (lastOpenBracket != -1)
            {
                // Return the string without the index part
                return input.Substring(0, lastOpenBracket);
            }

            // If the input string does not contain an index, return the input string unchanged
            return input;
        }

        private static bool isArrayIndexSegmet(string segment)
        {
            return segment.Contains("[") && segment.Contains("]");
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
                    currentToken = root;// return took if there is no parent
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
        private static string GetLastArrayIndexSegment(string path)
        {
            string[] parts = path.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            string lastPart = parts.LastOrDefault();

            return isArrayIndexSegmet(lastPart) ? lastPart.RemoveIndex() : lastPart;
        }

    }



}
