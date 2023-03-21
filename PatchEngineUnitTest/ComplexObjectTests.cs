using Newtonsoft.Json.Linq;
using PatchEngine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatchEngineUnitTest
{
    public class ComplexObjectTests
    {
        [Fact]
        public void TestComplexObectCompare()
        {
            // Arrange
            var objParsed = JObject.Parse("{\n    \"left\": {\n        \"name\": \"arbaaz\",\n        \"phone\": \"9004715915\",\n        \"details\":{\n            \"education\":{\n                \"highschool\":\"ICSE\",\n                \"subjects\":[\"phy\",\"chem\",\"math\"]\n            }\n        }\n    },\n    \"right\": {\n        \"name\": \"ARBAAZ\",\n        \"address\": \"Mumbai\",\n        \"details\":{\n            \"education\":{\n                \"highschool\":\"CBSE\",\n                \"subjects\":[\"phy\",\"Bio\",\"math\"]\n            }\n        }\n    }\n}\n");

            var left = JToken.Parse(objParsed["left"].ToString());
            var right = JToken.Parse(objParsed["right"].ToString());

            // Act
            JToken result = new JsonComparer().Compare(left, right, "");
            JToken expectedResult = JToken.Parse("[\n    {\n        \"Path\": \"name\",\n        \"LeftValue\": \"arbaaz\",\n        \"RightValue\": \"ARBAAZ\"\n    },\n    {\n        \"Path\": \"phone\",\n        \"LeftValue\": \"9004715915\",\n        \"RightValue\": null\n    },\n    {\n        \"Path\": \"details.education.highschool\",\n        \"LeftValue\": \"ICSE\",\n        \"RightValue\": \"CBSE\"\n    },\n    {\n        \"Path\": \"details.education.subjects[1]\",\n        \"LeftValue\": \"chem\",\n        \"RightValue\": \"Bio\"\n    },\n    {\n        \"Path\": \"address\",\n        \"LeftValue\": null,\n        \"RightValue\": \"Mumbai\"\n    }\n]");

            Assert.True(JToken.DeepEquals(result, expectedResult), "The result of the compare is incorrect.");

        }

        [Fact]
        public void TestComplexObectCompare2()
        {
            //nested object has array with identity objects 

            // Arrange
            var objParsed = JObject.Parse("{\n    \"left\": {\n        \"name\": \"arbaaz\",\n        \"phone\": \"9004715915\",\n        \"details\":{\n            \"education\":{\n                \"highschool\":\"ICSE\",\n                \"subjects\":[{\"Id\":\"phy\", \"fullname\":\"physics\"}]\n            }\n        }\n    },\n    \"right\": {\n        \"name\": \"ARBAAZ\",\n        \"address\": \"Mumbai\",\n        \"details\":{\n            \"education\":{\n                \"highschool\":\"CBSE\",\n                \"subjects\":[{\"Id\":\"phy\", \"fullname\":\"Physics\"  },{\"Id\":\"math\", \"fullname\":\"Mathmatics\"  } ],\n                \"extraPropOnRight\":\"yep\"\n            }\n        }\n    }\n}\n");

            var left = JToken.Parse(objParsed["left"].ToString());
            var right = JToken.Parse(objParsed["right"].ToString());

            // Act
            JToken result = new JsonComparer().Compare(left, right, "");
            JToken expectedResult = JToken.Parse("[\n    {\n        \"Path\": \"name\",\n        \"LeftValue\": \"arbaaz\",\n        \"RightValue\": \"ARBAAZ\"\n    },\n    {\n        \"Path\": \"phone\",\n        \"LeftValue\": \"9004715915\",\n        \"RightValue\": null\n    },\n    {\n        \"Path\": \"details.education.highschool\",\n        \"LeftValue\": \"ICSE\",\n        \"RightValue\": \"CBSE\"\n    },\n    {\n        \"Path\": \"details.education.subjects:phy.fullname\",\n        \"LeftValue\": \"physics\",\n        \"RightValue\": \"Physics\"\n    },\n    {\n        \"Path\": \"details.education.subjects:math\",\n        \"LeftValue\": null,\n        \"RightValue\": {\n            \"Id\": \"math\",\n            \"fullname\": \"Mathmatics\"\n        }\n    },\n    {\n        \"Path\": \"details.education.extraPropOnRight\",\n        \"LeftValue\": null,\n        \"RightValue\": \"yep\"\n    },\n    {\n        \"Path\": \"address\",\n        \"LeftValue\": null,\n        \"RightValue\": \"Mumbai\"\n    }\n]");

            Assert.True(JToken.DeepEquals(result, expectedResult), "The result of the compare is incorrect.");

        }

        [Fact]
        public void TestComplexObectCompare3()
        {
            //nested object has simple arrays

            // Arrange
            var objParsed = JObject.Parse("{\n    \"left\": {\n        \"name\": \"arbaaz\",\n        \"phone\": \"9004715915\",\n        \"details\":{\n            \"education\":{\n                \"highschool\":\"ICSE\",\n                \"subjects\":[{ \"fullname\":\"physics\"}]\n            }\n        }\n    },\n    \"right\": {\n        \"name\": \"ARBAAZ\",\n        \"address\": \"Mumbai\",\n        \"details\":{\n            \"education\":{\n                \"highschool\":\"CBSE\",\n                \"subjects\":[{ \"fullname\":\"Physics\"  },{ \"fullname\":\"Mathmatics\"  } ],\n                \"extraPropOnRight\":\"yep\"\n            }\n        }\n    }\n}\n");

            var left = JToken.Parse(objParsed["left"].ToString());
            var right = JToken.Parse(objParsed["right"].ToString());

            // Act
            JToken result = new JsonComparer().Compare(left, right, "");
            JToken expectedResult = JToken.Parse("[\n    {\n        \"Path\": \"name\",\n        \"LeftValue\": \"arbaaz\",\n        \"RightValue\": \"ARBAAZ\"\n    },\n    {\n        \"Path\": \"phone\",\n        \"LeftValue\": \"9004715915\",\n        \"RightValue\": null\n    },\n    {\n        \"Path\": \"details.education.highschool\",\n        \"LeftValue\": \"ICSE\",\n        \"RightValue\": \"CBSE\"\n    },\n    {\n        \"Path\": \"details.education.subjects[0]\",\n        \"LeftValue\": {\n            \"fullname\": \"physics\"\n        },\n        \"RightValue\": {\n            \"fullname\": \"Physics\"\n        }\n    },\n    {\n        \"Path\": \"details.education.subjects[1]\",\n        \"LeftValue\": null,\n        \"RightValue\": {\n            \"fullname\": \"Mathmatics\"\n        }\n    },\n    {\n        \"Path\": \"details.education.extraPropOnRight\",\n        \"LeftValue\": null,\n        \"RightValue\": \"yep\"\n    },\n    {\n        \"Path\": \"address\",\n        \"LeftValue\": null,\n        \"RightValue\": \"Mumbai\"\n    }\n]");

            Assert.True(JToken.DeepEquals(result, expectedResult), "The result of the compare is incorrect.");

        }
    }
}
