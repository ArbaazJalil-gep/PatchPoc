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
        public Patcher patcher { get; set; } = new Patcher();

        // GET api/<PatchController>/5

        [HttpPost]
        public string Post([FromBody] object obj)
        {
            var objParsed = JObject.Parse(obj.ToString());
            var payload = new Payload(objParsed["before"], objParsed["after"]);

           var patches =  JsonPatchGenerator.GeneratePatch(payload.before, payload.after);

            //var patches = patcher.GenerateJsonPatches(payload.before, payload.after);
            //var result = patcher.ApplyPatchesToObjectChatGpt(payload.before, patches);
            //dynamic resultD = new ExpandoObject();
            //resultD.result = result;
            //resultD.patches = patches;

            return Newtonsoft.Json.JsonConvert.SerializeObject(patches);

        }

        [HttpPost]
        [Route("Generate")]
        public string Generate([FromBody] object obj)
        {
            var objParsed = JObject.Parse(obj.ToString());
            
            var left = JToken.Parse(objParsed["left"].ToString());
            var right = JToken.Parse(objParsed["right"].ToString());
            //var patches = JsonPatchGeneratorGpt4.CompareJTokens(left, right);

            // var patches = JsonPatchGeneratorGpt4.CompareJTokens(left, right);



             
           var differences = JsonComparer.Compare(left, right,"");

            return Newtonsoft.Json.JsonConvert.SerializeObject(differences); ;
            //return Newtonsoft.Json.JsonConvert.SerializeObject(patches);

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
            JToken mergedJson = JsonComparer.Merge(left, userSelectedDifferences);
           
           return Newtonsoft.Json.JsonConvert.SerializeObject(mergedJson); ;
            //return Newtonsoft.Json.JsonConvert.SerializeObject(patches);

        }
    }
}
