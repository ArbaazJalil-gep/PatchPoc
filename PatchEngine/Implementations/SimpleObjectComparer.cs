using Newtonsoft.Json.Linq;
using PatchEngine.Core;
using PatchEngine.Interfaces;

namespace PatchEngine.Implementations
{
    public class SimpleObjectComparer : JsonComparer
    {
        private JObject _left;
        private JObject _right;
        private string _path;
        private JArray _differences;
        public SimpleObjectComparer(JToken left, JToken right, string path)
        {
            _left = (JObject) left;
            _right = (JObject) right;
            _path = path;
            _differences = new JArray();
        }
        public JArray Compare()
        {
            return Compare(_left, _right, _path);
        }
        public JArray Compare(JObject leftObject, JObject rightObject, string path="")
        {
           

            var allKeys = leftObject.Properties().Select(p => p.Name)
                .Union(rightObject.Properties().Select(p => p.Name));

            foreach (string key in allKeys)
            {
                string childPath = path == "" ? key : $"{path}.{key}";

                if (leftObject.TryGetValue(key, out JToken leftValue))
                {
                    if (rightObject.TryGetValue(key, out JToken rightValue))
                    {
                        _differences.Merge(base.Compare(leftValue, rightValue, childPath));
                    }
                    else
                    {
                        _differences.Add(new JObject
                        {
                            ["Path"] = childPath,
                            ["LeftValue"] = leftValue,
                            ["RightValue"] = null
                        });
                    }
                }
                else
                {
                    _differences.Add(new JObject
                    {
                        ["Path"] = childPath,
                        ["LeftValue"] = null,
                        ["RightValue"] = rightObject[key]
                    });
                }

            }
            return _differences;
        }
        public JToken Merge(JToken left, List<Difference> selectedDiffs)
        {
            return null;
        }
    }
}
