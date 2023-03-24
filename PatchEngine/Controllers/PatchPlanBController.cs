using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using PatchEngine.Core;
using static Patcher;

namespace PatchEngine.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatchPlanBController : ControllerBase
    {
        [HttpPost]
        [Route("Generate")]
        public string Generate([FromBody] object obj)
        {
            var objParsed = JObject.Parse(obj.ToString());
            var left = JToken.Parse(objParsed["left"].ToString());
            var right = JToken.Parse(objParsed["right"].ToString());
            List<JsonPatchOperation> patchOperations = GenerateOptimizedJsonPatch(left, right);
            return Newtonsoft.Json.JsonConvert.SerializeObject(patchOperations);
        }

        [HttpPost]
        [Route("Merge")]
        public string Merge([FromBody] object obj)
        {
            var objParsed = JObject.Parse(obj.ToString());
            var left = JToken.Parse(objParsed["left"].ToString());
            var selected = JToken.Parse(objParsed["selected"].ToString());
            var userSelectedDifferences = Newtonsoft.Json.JsonConvert.DeserializeObject<List<JsonPatchOperation>>(selected.ToString());
            // Merge the JSON objects based on the user-selected differences
            JToken mergedJson = ApplyPatch(left, userSelectedDifferences);
            return Newtonsoft.Json.JsonConvert.SerializeObject(mergedJson); ;
            //return Newtonsoft.Json.JsonConvert.SerializeObject(patches);
        }
    }
}
