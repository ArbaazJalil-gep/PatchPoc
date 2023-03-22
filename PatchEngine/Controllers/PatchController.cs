using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using PatchEngine.Core;
using System.Dynamic;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PatchEngine.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatchController : ControllerBase
    {
        [HttpPost]
        [Route("Generate")]
        public string Generate([FromBody] object obj)
        {
            var objParsed = JObject.Parse(obj.ToString());

            var left = JToken.Parse(objParsed["left"].ToString());
            var right = JToken.Parse(objParsed["right"].ToString());
            var differences = new JsonComparer().Compare(left, right, "");
            return Newtonsoft.Json.JsonConvert.SerializeObject(differences); ;
        }


        [HttpPost]
        [Route("Merge")]
        public string Merge([FromBody] object obj)
        {
            var objParsed = JObject.Parse(obj.ToString());
            var left = JToken.Parse(objParsed["left"].ToString());
            var selected = JToken.Parse(objParsed["selected"].ToString());
            var userSelectedDifferences = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Difference>>(selected.ToString());
            // Merge the JSON objects based on the user-selected differences
            JToken mergedJson = new JsonComparer().Merge(left, userSelectedDifferences);
            return Newtonsoft.Json.JsonConvert.SerializeObject(mergedJson); ;
            //return Newtonsoft.Json.JsonConvert.SerializeObject(patches);
        }
    }
}
