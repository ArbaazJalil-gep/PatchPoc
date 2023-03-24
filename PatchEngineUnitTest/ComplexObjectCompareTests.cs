using Newtonsoft.Json.Linq;
using PatchEngine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatchEngineUnitTest
{
    public class ComplexObjectCompareTests
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
            JToken expectedResult = JToken.Parse("[\n    {\n        \"Path\": \"name\",\n        \"LeftValue\": \"arbaaz\",\n        \"RightValue\": \"ARBAAZ\",\n        \"Op\": \"replace\"\n    },\n    {\n        \"Path\": \"phone\",\n        \"LeftValue\": \"9004715915\",\n        \"RightValue\": null,\n        \"Op\": \"remove\"\n    },\n    {\n        \"Path\": \"details.education.highschool\",\n        \"LeftValue\": \"ICSE\",\n        \"RightValue\": \"CBSE\",\n        \"Op\": \"replace\"\n    },\n    {\n        \"Path\": \"details.education.subjects[1]\",\n        \"LeftValue\": \"chem\",\n        \"RightValue\": \"Bio\",\n        \"Op\": \"replace\"\n    },\n    {\n        \"Path\": \"address\",\n        \"LeftValue\": null,\n        \"RightValue\": \"Mumbai\",\n        \"Op\": \"add\"\n    }\n]");

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
            JToken expectedResult = JToken.Parse("[{\"Path\":\"name\",\"LeftValue\":\"arbaaz\",\"RightValue\":\"ARBAAZ\",\"Op\":\"replace\"},{\"Path\":\"phone\",\"LeftValue\":\"9004715915\",\"RightValue\":null,\"Op\":\"remove\"},{\"Path\":\"details.education.highschool\",\"LeftValue\":\"ICSE\",\"RightValue\":\"CBSE\",\"Op\":\"replace\"},{\"Path\":\"details.education.subjects:phy.fullname\",\"LeftValue\":\"physics\",\"RightValue\":\"Physics\",\"Op\":\"replace\"},{\"Path\":\"details.education.subjects:math\",\"LeftValue\":null,\"RightValue\":{\"Id\":\"math\",\"fullname\":\"Mathmatics\"},\"Op\":\"add\"},{\"Path\":\"details.education.extraPropOnRight\",\"LeftValue\":null,\"RightValue\":\"yep\",\"Op\":\"add\"},{\"Path\":\"address\",\"LeftValue\":null,\"RightValue\":\"Mumbai\",\"Op\":\"add\"}]");
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
            JToken expectedResult = JToken.Parse("[{\"Path\":\"name\",\"LeftValue\":\"arbaaz\",\"RightValue\":\"ARBAAZ\",\"Op\":\"replace\"},{\"Path\":\"phone\",\"LeftValue\":\"9004715915\",\"RightValue\":null,\"Op\":\"remove\"},{\"Path\":\"details.education.highschool\",\"LeftValue\":\"ICSE\",\"RightValue\":\"CBSE\",\"Op\":\"replace\"},{\"Path\":\"details.education.subjects[0].fullname\",\"LeftValue\":\"physics\",\"RightValue\":\"Physics\",\"Op\":\"replace\"},{\"Path\":\"details.education.subjects\",\"LeftValue\":null,\"RightValue\":{\"fullname\":\"Mathmatics\"},\"Op\":\"add\"},{\"Path\":\"details.education.extraPropOnRight\",\"LeftValue\":null,\"RightValue\":\"yep\",\"Op\":\"add\"},{\"Path\":\"address\",\"LeftValue\":null,\"RightValue\":\"Mumbai\",\"Op\":\"add\"}]");
            Assert.True(JToken.DeepEquals(result, expectedResult), "The result of the compare is incorrect.");

        }
        

    }
}
