using Newtonsoft.Json.Linq;

namespace PatchEngine.Core.Extentions
{
    public static class JArrayExtentions
    {
        public static JArray MergeRegularArrays(this JArray arr1, JArray arr2)
        {
            var merged = new JArray();
            merged.Merge(arr1, new JsonMergeSettings
            {
                MergeArrayHandling = MergeArrayHandling.Union
            });
            merged.Merge(arr2, new JsonMergeSettings
            {
                MergeArrayHandling = MergeArrayHandling.Union
            });
            return merged;
        }

        public static bool isArrayWithoutId(this JArray leftArray)
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
        public static bool isMixedArray(this JArray leftArray)
        {
            bool hasIdentityObject = false;
            bool hasNonIdentityObject = false;

            foreach (var item in leftArray)
            {
                if (item is JObject obj && obj.ContainsKey("Id"))
                {
                      hasIdentityObject = true;
                }
                else
                {
                    hasNonIdentityObject = true;
                }
            }

            return hasIdentityObject && hasNonIdentityObject;
        }

        
    }
}
