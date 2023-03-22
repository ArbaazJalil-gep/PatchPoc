using Newtonsoft.Json.Linq;
using PatchEngine.Core.Extentions;
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
                JToken parent = merged.GetParent(diff.Path);
                string propertyName = "";
                 if (diff.Path.isArrayIndexSegmet())
                {
                    propertyName = diff.Path.GetLastArrayIndexSegment(); //get index [index]
                }
                else
                {
                    propertyName = diff.Path.GetPropertyName();
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
                            int index = diff.Path.ReturnIndexValue(level + 1);
                            
                            if (parentArray.Count > index)
                            {
                                if (parent[index].Type == JTokenType.Array)
                                {
                                    parent[index] = Merge(parent[index], new List<Difference>() { diff }, level + 1);
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
    }
}
