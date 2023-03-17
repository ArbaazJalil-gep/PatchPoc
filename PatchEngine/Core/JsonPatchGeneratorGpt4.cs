using Newtonsoft.Json.Linq;


namespace PatchEngine.Core
{
    public static class JsonPatchGeneratorGpt4
    {
        public static JToken CompareJTokens(JToken left, JToken right)
        {
            if (left.Type == JTokenType.Object && right.Type == JTokenType.Object)
            {
                JObject differences = new JObject();
                JObject leftObject = (JObject)left;
                JObject rightObject = (JObject)right;

                foreach (var property in leftObject.Properties())
                {
                    if (rightObject.TryGetValue(property.Name, out JToken rightValue))
                    {
                        JToken diff = CompareJTokens(property.Value, rightValue);
                        if (diff != null)
                        {
                            differences[property.Name] = diff;
                        }
                    }
                    else
                    {
                        differences[property.Name] = CreateLeftDifference(property.Value);
                    }
                }

                foreach (var property in rightObject.Properties())
                {
                    if (!leftObject.ContainsKey(property.Name))
                    {
                        differences[property.Name] = CreateRightDifference(property.Value);
                    }
                }

                return differences.Count > 0 ? differences : null;
            }
            else if (left.Type == JTokenType.Array && right.Type == JTokenType.Array)
            {
                JArray differences = new JArray();
                JArray leftArray = (JArray)left;
                JArray rightArray = (JArray)right;
                int maxLength = Math.Max(leftArray.Count, rightArray.Count);

                for (int i = 0; i < maxLength; i++)
                {
                    if (i < leftArray.Count && i < rightArray.Count)
                    {
                        JToken diff = CompareJTokens(leftArray[i], rightArray[i]);
                        differences.Add(diff ?? JToken.FromObject(null));
                    }
                    else if (i < leftArray.Count)
                    {
                        differences.Add(CreateLeftDifference(leftArray[i]));
                    }
                    else
                    {
                        differences.Add(CreateRightDifference(rightArray[i]));
                    }
                }

                return differences;
            }
            else if (!JToken.DeepEquals(left, right))
            {
                JObject difference = new JObject();

                if (left.Type == JTokenType.Object && left["Id"] != null)
                {
                    difference["left"] = new JObject { ["Id"] = left["Id"] };
                }
                else
                {
                    difference["left"] = left;
                }

                if (right.Type == JTokenType.Object && right["Id"] != null)
                {
                    difference["right"] = new JObject { ["Id"] = right["Id"] };
                }
                else
                {
                    difference["right"] = right;
                }

                return difference;
            }

            return null;
        }

        private static JObject CreateLeftDifference(JToken left)
        {
            return new JObject { ["left"] = left };
        }

        private static JObject CreateRightDifference(JToken right)
        {
            return new JObject { ["right"] = right };
        }
    }
}
