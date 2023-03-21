using Newtonsoft.Json.Linq;
using PatchEngine.Core;
using PatchEngine.Interfaces;

namespace PatchEngine.Implementations
{
    public class IdentityObjectComparer : IJsonComparer
    {
        public JArray Compare(JToken left, JToken right, string path)
        {
            return null;
        }
        public JToken Merge(JToken left, List<Difference> selectedDiffs,int level)
        {
            return null;
        }
    }
}
