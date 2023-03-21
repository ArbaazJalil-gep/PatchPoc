using Newtonsoft.Json.Linq;
using PatchEngine.Core;
using PatchEngine.Interfaces;

namespace PatchEngine.Implementations
{
    public class ArrayWithIdentityObjects : JsonComparer
    {
        private JArray _leftArray;
        private JArray _rightArray;
        private string _path;
        private JArray _differences;

        public ArrayWithIdentityObjects(JToken left, JToken right, string path)
        {
            _leftArray = (JArray)left;
            _rightArray = (JArray)right;
            _path = path;
            _differences = new JArray();
        }

        public JArray Compare()
        {
            return Compare(_leftArray, _rightArray, _path);
        }
        public JArray Compare(JToken left, JToken right, string path)
        {
            var visitedRightItems = new HashSet<int>();

            for (int leftIndex = 0; leftIndex < _leftArray.Count; leftIndex++)
            {
                string childPath = "";
                if (JArrayExtentions.isArrayWithoutId(_leftArray))
                {
                    childPath = $"{path}[{leftIndex}]"?.ToString();
                }
                else
                {
                    childPath = $"{path}:{_leftArray[leftIndex]["Id"]?.ToString()}";
                }

                bool foundMatch = false;
                bool nestedArrayCase = false;
                for (int rightIndex = 0; rightIndex < _rightArray.Count; rightIndex++)
                {
                    if (visitedRightItems.Contains(rightIndex))
                    {
                        continue;
                    }

                    if (_leftArray.isArrayWithoutId()) // simple array
                    {
                        if (_leftArray[leftIndex]?.ToString() == _rightArray[rightIndex]?.ToString())
                        {
                            foundMatch = true;
                            visitedRightItems.Add(rightIndex);
                            if (doesPathExist(childPath)) continue;
                           // _differences.Merge(base.Compare(_leftArray[leftIndex], _rightArray[rightIndex], childPath));
                            break;
                        }
                        else if(_leftArray[leftIndex].Type == JTokenType.Array  && _rightArray[rightIndex].Type == JTokenType.Array) // if left and right element are both array (nested array detected)
                        {
                            visitedRightItems.Add(rightIndex); // not sure if this make any difference or even if it should be here
                            _differences.Merge(base.Compare(_leftArray[leftIndex], _rightArray[rightIndex], childPath));
                            nestedArrayCase = true;
                            break;
                        }
                    }
                    else if (_leftArray[leftIndex]["Id"]?.ToString() == _rightArray[rightIndex]["Id"]?.ToString())
                    {

                        foundMatch = true;
                        visitedRightItems.Add(rightIndex);
                        _differences.Merge(base.Compare(_leftArray[leftIndex], _rightArray[rightIndex], childPath));
                        break;
                    }
                }
                if (nestedArrayCase) continue;
                if (!foundMatch)
                {
                    if (_leftArray.isArrayWithoutId()) // simple array
                    {

                        if (doesPathExist($"{path}[{leftIndex}]")) continue;

                        _differences.Add(new JObject
                        {
                            ["Path"] = $"{path}[{leftIndex}]",
                            ["LeftValue"] = _leftArray[leftIndex],
                            ["RightValue"] = _rightArray.Count < leftIndex + 1 ? null : _rightArray[leftIndex]
                        });
                    }
                    else
                    {
                        _differences.Add(new JObject
                        {
                            ["Path"] = $"{path}:{_leftArray[leftIndex]["Id"]}",
                            ["LeftValue"] = _leftArray[leftIndex],
                            ["RightValue"] = null
                        });
                    }

                }
            }
            int maxCount = 0;
           

            if (_leftArray.isArrayWithoutId()) // simple array
            {
                maxCount = _rightArray.Count;
                if (_rightArray.Count > _leftArray.Count)
                {
                    maxCount = _leftArray.Count;
                }
            }
            
                for (int rightIndex = maxCount; rightIndex < _rightArray.Count; rightIndex++)
            {
                var childPath = $"{path}[{rightIndex}]";

                if (_leftArray.isArrayWithoutId()) // simple array
                {
                    if (doesPathExist(childPath)) continue;
                    _differences.Add(new JObject
                    {
                        ["Path"] = $"{path}[{rightIndex}]",
                        ["LeftValue"] = _leftArray.Count < rightIndex + 1 ? null : _leftArray[rightIndex],
                        ["RightValue"] = _rightArray[rightIndex],
                    }); ;
                }
                else
                {
                    if (!visitedRightItems.Contains(rightIndex))
                    {
                        _differences.Add(new JObject
                        {
                            ["Path"] = $"{path}:{_rightArray[rightIndex]["Id"]}",
                            ["LeftValue"] = null,
                            ["RightValue"] = _rightArray[rightIndex]
                        });
                    }

                }


            }
            return _differences;
        }

        private bool doesPathExist(string path)
        {
            bool pathExists = false;
            foreach (var item in _differences)
            {
                if (item["Path"]?.ToString() == path?.ToString())
                {
                    pathExists = true;
                    break;
                }
            }

            return pathExists;
        }

        public JToken Merge(JToken left, List<Difference> selectedDiffs)
        {
            return null;
        }
    }
}
