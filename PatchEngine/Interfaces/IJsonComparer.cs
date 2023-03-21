using Newtonsoft.Json.Linq;
using PatchEngine.Core;

namespace PatchEngine.Interfaces
{
    public interface IJsonComparer
    {
        public JArray Compare(JToken left, JToken right, string path);
        public JToken Merge(JToken left, List<Difference> selectedDiffs,int level);
    }
}
