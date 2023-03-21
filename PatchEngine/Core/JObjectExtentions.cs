using Newtonsoft.Json.Linq;

namespace PatchEngine.Core
{
    public static class JObjectExtentions
    {

        public static JObject CustomMerge(this JObject obj1, JObject obj2)
        {
            // Check if Id property value is the same in both objects
            if (obj1["Id"].Value<string>() == obj2["Id"].Value<string>())
            {
                // Merge the two objects based on Id property
                obj1.Merge(obj2, new JsonMergeSettings
                {
                    MergeArrayHandling = MergeArrayHandling.Union
                });
            }
            else
            {
                // Override obj1 with obj2
                obj1 = obj2;
            }

            return obj1;
        }

        public static JObject MergeRegularObjects(this JObject obj1, JObject obj2)
        {
            var merged = obj1.DeepClone();
            foreach (var property in obj2.Properties())
            {
                if (merged[property.Name] is JObject obj)
                {
                    MergeRegularObjects(obj, property.Value as JObject);
                }
                else
                {
                    merged[property.Name] = property.Value;
                }
            }
            return merged.ToObject<JObject>();
        }
    }
    
}
