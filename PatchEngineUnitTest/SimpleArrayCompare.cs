using Newtonsoft.Json.Linq;
using PatchEngine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatchEngineUnitTest
{
    public class SimpleArrayCompare
    {
        [Fact]
        public void TestCompare()
        {
            // Arrange
            var objParsed = JObject.Parse("{\n    \"left\": [\"geo\",[\"phy\",\"cm\"],\"a\",[\"C\"]],\n    \"right\":[\"geo\",[\"phy\",\"chem\", \"bio\"],\"b\",[\"c\",\"D\"]]\n}\n");

            var left = JToken.Parse(objParsed["left"].ToString());
            var right = JToken.Parse(objParsed["right"].ToString());

            // Act
            JToken result = new JsonComparer().Compare(left, right, "");
            JToken expectedResult = JToken.Parse("[\n    {\n        \"Path\": \"[1][1]\",\n        \"LeftValue\": \"cm\",\n        \"RightValue\": \"chem\"\n    },\n    {\n        \"Path\": \"[1][2]\",\n        \"LeftValue\": null,\n        \"RightValue\": \"bio\"\n    },\n    {\n        \"Path\": \"[2]\",\n        \"LeftValue\": \"a\",\n        \"RightValue\": \"b\"\n    },\n    {\n        \"Path\": \"[3][0]\",\n        \"LeftValue\": \"C\",\n        \"RightValue\": \"c\"\n    },\n    {\n        \"Path\": \"[3][1]\",\n        \"LeftValue\": null,\n        \"RightValue\": \"D\"\n    }\n]");

            Assert.True(JToken.DeepEquals(result, expectedResult), "The result of the compare is incorrect.");

        }
        [Fact]
        public void TestCompare2_SuperNestedSimpleArrays()
        {
            // Arrange
            var objParsed = JObject.Parse("{\"left\":[[[[[1,2],[3,4]],[[5,6],[7,8]]],[[[9,10],[11,12]],[[13,14],[15,16]]]],[[[[17,18],[19,20]],[[21,22],[23,24]]],[[[25,26],[27,28]],[[29,30],[31,32]]]]],\"right\":[[[[[1,2],[30,4]],[[5,6],[7,8]]],[[[9,10],[11,12]],[[13,14],[150,16]]]],[[[[17,18],[19,20]],[[21,22],[23,24]]],[[[25,26],[27,280]],[[29,30],[31,32]]]]]}");

            var left = JToken.Parse(objParsed["left"].ToString());
            var right = JToken.Parse(objParsed["right"].ToString());

            // Act
            JToken result = new JsonComparer().Compare(left, right, "");
            JToken expectedResult = JToken.Parse("[\n    {\n        \"Path\": \"[0][0][0][1][0]\",\n        \"LeftValue\": 3,\n        \"RightValue\": 30\n    },\n    {\n        \"Path\": \"[0][1][1][1][0]\",\n        \"LeftValue\": 15,\n        \"RightValue\": 150\n    },\n    {\n        \"Path\": \"[1][1][0][1][1]\",\n        \"LeftValue\": 28,\n        \"RightValue\": 280\n    }\n]");

            Assert.True(JToken.DeepEquals(result, expectedResult), "The result of the compare is incorrect.");

        }
    }
}
