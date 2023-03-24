using Newtonsoft.Json.Linq;
using PatchEngine.Core;
using PatchEngine.Core.Extentions;
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
            string op = "replace";
            for (int leftIndex = 0; leftIndex < _leftArray.Count; leftIndex++)
            {
                string childPath = "";

                if (_leftArray.isArrayWithoutId() || _leftArray.isMixedArray()) // check array item instead
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
                    if (_leftArray.isArrayWithoutId() || _leftArray.isMixedArray()) // check array item instead
                    {
                        if (rightIndex != leftIndex)
                        {
                            continue;
                        }
                    }

                    if (_leftArray.isArrayWithoutId() || _leftArray.isMixedArray()) // simple array
                    {
                        if (rightIndex > leftIndex) continue;
                        if (_leftArray[leftIndex]?.ToString() == _rightArray[rightIndex]?.ToString())
                        {
                            foundMatch = true;
                            visitedRightItems.Add(rightIndex);
                            if (doesPathExist(childPath)) continue;
                            // _differences.Merge(base.Compare(_leftArray[leftIndex], _rightArray[rightIndex], childPath));
                            break;
                        }
                        else if (_leftArray[leftIndex].Type == JTokenType.Array && _rightArray[rightIndex].Type == JTokenType.Array) // if left and right element are both array (nested array detected)
                        {
                            visitedRightItems.Add(rightIndex); // not sure if this make any difference or even if it should be here
                            _differences.Merge(base.Compare(_leftArray[leftIndex], _rightArray[rightIndex], childPath));
                            nestedArrayCase = true;
                            break;
                        }
                        else if (_leftArray[leftIndex].Type == JTokenType.Object && _rightArray[rightIndex].Type == JTokenType.Object)// if Object
                        {
                            visitedRightItems.Add(rightIndex); // not sure if this make any difference or even if it should be here
                            _differences.Merge(base.Compare(_leftArray[leftIndex], _rightArray[rightIndex], childPath));
                            nestedArrayCase = true;
                            break;
                        }
                        //else if (_leftArray[leftIndex].Type !=_rightArray[rightIndex].Type )// if Object
                        //{
                        //    visitedRightItems.Add(rightIndex); // not sure if this make any difference or even if it should be here
                        //    _differences.Merge(base.Compare(_leftArray[leftIndex], _rightArray[rightIndex], childPath));
                        //    nestedArrayCase = true;
                        //    break;
                        //}
                        //last three else are same
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
                    if (_leftArray.isArrayWithoutId() || _leftArray.isMixedArray()) // check array item instead
                    {
                        op = _rightArray.Count < leftIndex + 1 ? "remove" : "replace";
                    }
                    else
                    {
                        op = "remove";
                    }
                    if (_leftArray.isArrayWithoutId() || _leftArray.isMixedArray()) // simple array
                    {

                        if (doesPathExist($"{path}[{leftIndex}]")) continue;


                        _differences.Add(new JObject
                        {
                            ["Path"] = $"{path}[{leftIndex}]",
                            ["LeftValue"] = _leftArray[leftIndex],
                            ["RightValue"] = _rightArray.Count < leftIndex + 1 ? null : _rightArray[leftIndex],
                            ["Op"] = op
                        });
                    }
                    else
                    {
                        _differences.Add(new JObject
                        {
                            ["Path"] = $"{path}:{_leftArray[leftIndex]["Id"]}",
                            ["LeftValue"] = _leftArray[leftIndex],
                            ["RightValue"] = null,
                            ["Op"] = op
                        });
                    }

                }
            }
            int maxCount = 0;


            if (_leftArray.isArrayWithoutId() || _leftArray.isMixedArray()) // simple array
            {
                maxCount = _rightArray.Count;
                if (_rightArray.Count > _leftArray.Count)
                {
                    maxCount = _leftArray.Count;
                }
            }

            for (int rightIndex = maxCount; rightIndex < _rightArray.Count; rightIndex++)
            {
                var childPath = $"{path}"; //used to be $"{path}[{rightIndex}]"; if you have to add hyphen (-) add it here
                if (childPath == "") childPath = "/";
                op = "add";
                if (_leftArray.isArrayWithoutId() || _leftArray.isMixedArray()) // simple array
                {
                    if (doesPathExist(childPath)) continue;
                    _differences.Add(new JObject
                    {
                        ["Path"] = $"{childPath}",
                        ["LeftValue"] = _leftArray.Count < rightIndex + 1 ? null : _leftArray[rightIndex],
                        ["RightValue"] = _rightArray[rightIndex],
                        ["Op"] = op
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
                            ["RightValue"] = _rightArray[rightIndex],
                            ["Op"] = op
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
