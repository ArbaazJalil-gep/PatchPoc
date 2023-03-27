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
        public string Op { get; set; }
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
                    ["RightValue"] = right,
                    ["Op"] = "replace"

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
                    ["RightValue"] = right,
                    ["Op"] = "replace"
                });
            }

            return differences;
        }


        public JToken Merge(JToken left, List<Difference> selectedDiffs, int level = 0)
            {
            JToken merged = left.DeepClone();
            int idx = 0;

            // Sort patches in descending order of indices for array operations
            selectedDiffs = selectedDiffs.OrderByDescending(diff => diff.Path.isArrayIndexSegmet() ? diff.Path.ReturnLastIndexValue() : -1).ToList();

            foreach (var diff in selectedDiffs)
            {
                JToken parent = null;
                var currToken = merged.SelectTokenByPath(diff.Path);
               if (currToken !=null &&  currToken.Type == JTokenType.Array && diff.Op == "add")
                {
                    parent = merged.SelectTokenByPath(diff.Path);
                }
               else
                 parent = merged.GetParent(diff.Path);

                string propertyName = "";

                if (diff.Path.isArrayIndexSegmet())
                {
                    propertyName = diff.Path.GetLastArrayIndexSegment();
                    if (propertyName.Contains(":"))
                    {
                        propertyName = diff.Path.GetPropertyName();
                    }
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

                        if (diff.Op == "remove")
                            parentObject.Remove(propertyName);
                        else
                            parentObject[propertyName] = diff.RightValue;
                    }
                    else if (parent.Type == JTokenType.Array)
                    {
                        if (((JArray)parent).isArrayWithoutId() || ((JArray)parent).isMixedArray())
                        {
                            JArray parentArray = (JArray)parent;
                            int index = diff.Path.ReturnLastIndexValue();

                            if (parentArray.Count > index && index != -1)
                            {
                                if (diff.Op == "remove")
                                {
                                    parentArray.RemoveAt(index);
                                    continue;
                                }

                                if (parent[index].Type == JTokenType.Array)
                                {
                                    if (diff.Op == "replace")
                                        parent[index] = diff.RightValue; //Merge(parent[index], new List<Difference> { diff }, level + 1);
                                    else if (diff.Op == "add")
                                        ((JArray)parent[index]).Add(diff.RightValue);
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
                            int index = parentArray.IndexOf(parentArray.FirstOrDefault(x => x?["Id"]?.ToString() == propertyName));

                            if (index >= 0)
                            {
                                if (diff.Op == "remove")
                                {
                                    parentArray.RemoveAt(index);
                                }
                                else
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
