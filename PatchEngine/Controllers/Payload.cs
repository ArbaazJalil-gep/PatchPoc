using Newtonsoft.Json.Linq;

namespace PatchEngine.Controllers
{
    public class Payload
    {
        public Payload(JToken? before, JToken? after)
        {
            this.before = before;
            this.after = after;
        }
        public JToken? before { get; set; }
        public JToken? after { get; set; }
    }
}
