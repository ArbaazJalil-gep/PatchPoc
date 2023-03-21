using Newtonsoft.Json.Linq;
using PatchEngine.Core;
using PatchEngine.Interfaces;

namespace PatchEngine.Implementations
{
    public class SimpleArrayComparer : IJsonComparer
    {
        public JArray Compare(JToken left, JToken right, string path)
        {
            return null;
        }
       

        public JToken Merge(JToken left, List<Difference> selectedDiffs, int level)
        {
            throw new NotImplementedException();
        }
    }
}
