using Newtonsoft.Json.Linq;
using PatchEngine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatchEngineUnitTest
{
    public class SimpleObjectMergeTests
    {

        [Fact]
        public void TestMerge6()
        {

            // Arrange
            JsonComparer obj = new JsonComparer();
            var objParsed = JObject.Parse("{\"left\":{\"level1\":{\"level2a\":{\"level3a\":{\"level4a\":{\"level5a\":\"value1\",\"level5b\":\"value2\"},\"level4b\":{\"level5c\":\"value3\",\"level5d\":\"value4\"}},\"level3b\":{\"level4c\":{\"level5e\":\"value5\",\"level5f\":\"value6\"},\"level4d\":{\"level5g\":\"value7\",\"level5h\":\"value8\"}}},\"level2b\":{\"level3c\":{\"level4e\":{\"level5i\":\"value9\",\"level5j\":\"value10\"},\"level4f\":{\"level5k\":\"value11\",\"level5l\":\"value12\"}},\"level3d\":{\"level4g\":{\"level5m\":\"value13\",\"level5n\":\"value14\"},\"level4h\":{\"level5o\":\"value15\",\"level5p\":\"value16\"}}}}},\"selected\":[{\"Path\":\"level1.level2a.level3b.level4d.level5g\",\"LeftValue\":\"value7\",\"RightValue\":\"value7000\"},{\"Path\":\"level1.level2b.level2extra\",\"LeftValue\":null,\"RightValue\":\"righthandside\"}]}");

            var left = JToken.Parse(objParsed["left"].ToString());
            var selectedDiffs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Difference>>(objParsed["selected"].ToString());
            // Act
            JToken result = obj.Merge(left, selectedDiffs);

            // Act

            JToken expectedResult = JToken.Parse("{\"level1\":{\"level2a\":{\"level3a\":{\"level4a\":{\"level5a\":\"value1\",\"level5b\":\"value2\"},\"level4b\":{\"level5c\":\"value3\",\"level5d\":\"value4\"}},\"level3b\":{\"level4c\":{\"level5e\":\"value5\",\"level5f\":\"value6\"},\"level4d\":{\"level5g\":\"value7000\",\"level5h\":\"value8\"}}},\"level2b\":{\"level3c\":{\"level4e\":{\"level5i\":\"value9\",\"level5j\":\"value10\"},\"level4f\":{\"level5k\":\"value11\",\"level5l\":\"value12\"}},\"level3d\":{\"level4g\":{\"level5m\":\"value13\",\"level5n\":\"value14\"},\"level4h\":{\"level5o\":\"value15\",\"level5p\":\"value16\"}},\"level2extra\":\"righthandside\"}}}");

            Assert.True(JToken.DeepEquals(result, expectedResult), "The result of the compare is incorrect.");

        }
    }
}
