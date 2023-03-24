using Newtonsoft.Json.Linq;
using PatchEngine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatchEngineUnitTest
{
    public class ComplexObjectMergeTests
    {
        [Fact]
        public void TestMergeComplex()
        {
            // Arrange
            var objParsed = JObject.Parse("{\"left\":{\"level1\":{\"level2a\":{\"level3a\":{\"level4a\":{\"level5a\":\"value1\",\"level5b\":\"value2\"},\"level4b\":{\"level5c\":\"value3\",\"level5d\":\"value4\"}},\"level3b\":{\"level4c\":{\"level5e\":\"value5\",\"level5f\":\"value6\"},\"level4d\":{\"level5g\":\"value7\",\"level5h\":[[[[[1,2],[3,4]],[[5,6],[7,8]]],[[[9,10],[11,12]],[[13,14],[15,16]]]],[[[[17,18],[19,20]],[[21,22],[23,24]]],[[[25,26],[27,28]],[[29,30],[31,32]]]]]}}},\"level2b\":{\"level3c\":{\"level4e\":{\"level5i\":\"value9\",\"level5j\":\"value10\"},\"level4f\":{\"level5k\":\"value11\",\"level5l\":\"value12\"}},\"level3d\":{\"level4g\":{\"level5m\":\"value13\",\"level5n\":\"value14\"},\"level4h\":{\"level5o\":\"value15\",\"level5p\":\"value16\"}}}}},\"selected\":[{\"Path\":\"level1.level2a.level3b.level4d.level5g\",\"LeftValue\":\"value7\",\"RightValue\":\"value7000\"},{\"Path\":\"level1.level2a.level3b.level4d.level5h[0][0][0][1][0]\",\"LeftValue\":3,\"RightValue\":3000},{\"Path\":\"level1.level2a.level3b.level4d.level5h[0][1][1][0][0]\",\"LeftValue\":13,\"RightValue\":13000},{\"Path\":\"level1.level2a.level3b.level4d.level5h[1][0][1][1][0]\",\"LeftValue\":23,\"RightValue\":2300},{\"Path\":\"level1.level2b.level2extra\",\"LeftValue\":null,\"RightValue\":\"righthandside\"}]}");

            var left = JToken.Parse(objParsed["left"].ToString());

            var selected = JToken.Parse(objParsed["selected"].ToString());
            var userSelectedDifferences = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Difference>>(selected.ToString());


            // Merge the JSON objects based on the user-selected differences
            JToken result = new JsonComparer().Merge(left, userSelectedDifferences);



            JToken expectedResult = JToken.Parse("{\"level1\":{\"level2a\":{\"level3a\":{\"level4a\":{\"level5a\":\"value1\",\"level5b\":\"value2\"},\"level4b\":{\"level5c\":\"value3\",\"level5d\":\"value4\"}},\"level3b\":{\"level4c\":{\"level5e\":\"value5\",\"level5f\":\"value6\"},\"level4d\":{\"level5g\":\"value7000\",\"level5h\":[[[[[1,2],[3000,4]],[[5,6],[7,8]]],[[[9,10],[11,12]],[[13000,14],[15,16]]]],[[[[17,18],[19,20]],[[21,22],[2300,24]]],[[[25,26],[27,28]],[[29,30],[31,32]]]]]}}},\"level2b\":{\"level3c\":{\"level4e\":{\"level5i\":\"value9\",\"level5j\":\"value10\"},\"level4f\":{\"level5k\":\"value11\",\"level5l\":\"value12\"}},\"level3d\":{\"level4g\":{\"level5m\":\"value13\",\"level5n\":\"value14\"},\"level4h\":{\"level5o\":\"value15\",\"level5p\":\"value16\"}},\"level2extra\":\"righthandside\"}}}");
            // Assert
            Assert.True(JToken.DeepEquals(result, expectedResult), "The result of the merge is incorrect.");

        }
    }
}
