using Xunit;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using PatchEngine.Core;

namespace PatchEngineUnitTest
{


    public class SimpleArrayMergeTests
    {
        [Fact]
        public void TestMerge()
        {
            // Arrange
            JToken left = JToken.Parse("[1,2,3]");
            List<Difference> selectedDiffs = new List<Difference>
        {
            new Difference
            {
                Path = "[2]",
                LeftValue = JToken.Parse("3"),
                RightValue = JToken.Parse("5")
            },
            new Difference
            {
                Path = "[3]",
                LeftValue = null,
                RightValue = JToken.Parse("6")
            }
        };

            // Instantiate the class containing the Merge method.
            // Replace "YourClassName" with the actual class name.
            JsonComparer obj = new JsonComparer();

            // Act
            JToken result = obj.Merge(left, selectedDiffs);

            // Assert
            JToken expectedResult = JToken.Parse("[1,2,5,6]");
            Assert.True(JToken.DeepEquals(result, expectedResult), "The result of the merge is incorrect.");
        }

        [Fact]
        public void TestMerge2()
        {
            // Arrange
            JToken left = JToken.Parse("[1,2,3,13,14]");
            List<Difference> selectedDiffs = new List<Difference>
        {
            new Difference
            {
                Path = "[2]",
                LeftValue = JToken.Parse("3"),
                RightValue = null
            },
            new Difference
            {
                Path = "[3]",
                LeftValue = 13,
                RightValue = null
            },
             new Difference
            {
                Path = "[4]",
                LeftValue = 14,
                RightValue = null
            }
        };

            // Instantiate the class containing the Merge method.
            // Replace "YourClassName" with the actual class name.
            JsonComparer obj = new JsonComparer();

            // Act
            JToken result = obj.Merge(left, selectedDiffs);

            // Assert
            JToken expectedResult = JToken.Parse("[1,2,null,null,null]");
            Assert.True(JToken.DeepEquals(result, expectedResult), "The result of the merge is incorrect.");
        }


        [Fact]
        public void TestMerge3()
        {
            // Arrange
            JToken left = JToken.Parse("[1,2,3,13,14]");
            List<Difference> selectedDiffs = new List<Difference>
        {
            new Difference
            {
                Path = "[2]",
                LeftValue = JToken.Parse("3"),
                RightValue = null
            }
        };

            // Instantiate the class containing the Merge method.
            // Replace "YourClassName" with the actual class name.
            JsonComparer obj = new JsonComparer();

            // Act
            JToken result = obj.Merge(left, selectedDiffs);

            // Assert
            JToken expectedResult = JToken.Parse("[1,2,null,13,14]");
            Assert.True(JToken.DeepEquals(result, expectedResult), "The result of the merge is incorrect.");
        }


        [Fact]
        public void TestMerge4()
        {
            // Arrange
            JToken left = JToken.Parse("[1,2,3,13,14]");
            List<Difference> selectedDiffs = new List<Difference>
        {
            new Difference
            {
                Path = "[2]",
                LeftValue = JToken.Parse("3"),
                RightValue = JToken.Parse("[1,2,3]") 
            }
        };

            // Instantiate the class containing the Merge method.
            // Replace "YourClassName" with the actual class name.
            JsonComparer obj = new JsonComparer();

            // Act
            JToken result = obj.Merge(left, selectedDiffs);

            // Assert
            JToken expectedResult = JToken.Parse("[1,2,[1,2,3],13,14]");
            Assert.True(JToken.DeepEquals(result, expectedResult), "The result of the merge is incorrect.");
        }


       

        [Fact]
        public void TestMerge5()
        {

            // Arrange
            JsonComparer obj = new JsonComparer();
            var objParsed = JObject.Parse("{\n      \"left\":{\n        \"name\": \"arbaaz\",\n        \"phone\": \"9004715915\",\n        \"details\":{\n            \"education\":{\n                \"highschool\":\"ICSE\",\n                \"subjects\":[\"phy\",\"chem\",\"math\"]\n            }\n        }\n    },\n    \"selected\":[\n    {\n        \"Path\": \"name\",\n        \"LeftValue\": \"arbaaz\",\n        \"RightValue\": \"ARBAAZ\"\n    },\n    {\n        \"Path\": \"phone\",\n        \"LeftValue\": \"9004715915\",\n        \"RightValue\": \"9004715910\"\n    },\n    {\n        \"Path\": \"details.education.highschool\",\n        \"LeftValue\": \"ICSE\",\n        \"RightValue\": \"CBSE\"\n    },\n    {\n        \"Path\": \"details.education.subjects[1]\",\n        \"LeftValue\": \"chem\",\n        \"RightValue\": \"Bio\"\n    },\n    {\n        \"Path\": \"address\",\n        \"LeftValue\": null,\n        \"RightValue\": \"Mumbai\"\n    }\n]\n}");

            var left = JToken.Parse(objParsed["left"].ToString());
            var selectedDiffs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Difference>>(objParsed["selected"].ToString());
            // Act
            JToken result = obj.Merge(left, selectedDiffs);

            // Act

            JToken expectedResult = JToken.Parse("{\n    \"name\": \"ARBAAZ\",\n    \"phone\": \"9004715910\",\n    \"details\": {\n        \"education\": {\n            \"highschool\": \"CBSE\",\n            \"subjects\": [\n                \"phy\",\n                \"Bio\",\n                \"math\"\n            ]\n        }\n    },\n    \"address\": \"Mumbai\"\n}");

            Assert.True(JToken.DeepEquals(result, expectedResult), "The result of the compare is incorrect.");

        }



        [Fact]
        public void TestMerge6()
        {

            // Arrange
            JsonComparer obj = new JsonComparer();
            var objParsed = JObject.Parse("{\n      \"left\":[\"geo\",\"phy\"],\n    \"selected\":[{\"Path\":\"[1]\",\"LeftValue\":\"phy\",\"RightValue\":[\"Phy\",\"chem\",\"bio\"]}]\n}");

            var left = JToken.Parse(objParsed["left"].ToString());
            var selectedDiffs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Difference>>(objParsed["selected"].ToString());
            // Act
            JToken result = obj.Merge(left, selectedDiffs);

            // Act

            JToken expectedResult = JToken.Parse("[\"geo\",[\"Phy\",\"chem\",\"bio\"]]");

            Assert.True(JToken.DeepEquals(result, expectedResult), "The result of the compare is incorrect.");

        }


        [Fact]
        public void TestMerge6_SuperNestedSimpleArrays()
        {

            // Arrange
            JsonComparer obj = new JsonComparer();
            var objParsed = JObject.Parse("{\"left\":[[[[[1,2],[3,4]],[[5,6],[7,8]]],[[[9,10],[11,12]],[[13,14],[15,16]]]],[[[[17,18],[19,20]],[[21,22],[23,24]]],[[[25,26],[27,28]],[[29,30],[31,32]]]]],\"selected\":[{\"Path\":\"[0][0][0][1][0]\",\"LeftValue\":3,\"RightValue\":30},{\"Path\":\"[0][1][1][1][0]\",\"LeftValue\":15,\"RightValue\":150},{\"Path\":\"[1][1][0][1][1]\",\"LeftValue\":28,\"RightValue\":280}]}");

            var left = JToken.Parse(objParsed["left"].ToString());
            var selectedDiffs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Difference>>(objParsed["selected"].ToString());
            // Act
            JToken result = obj.Merge(left, selectedDiffs);

            // Act

            JToken expectedResult = JToken.Parse("[[[[[1,2],[30,4]],[[5,6],[7,8]]],[[[9,10],[11,12]],[[13,14],[150,16]]]],[[[[17,18],[19,20]],[[21,22],[23,24]]],[[[25,26],[27,280]],[[29,30],[31,32]]]]]");

            Assert.True(JToken.DeepEquals(result, expectedResult), "The result of the compare is incorrect.");

        }


        [Fact]
        public void TestMerge7()
        {

            // Arrange
            JsonComparer obj = new JsonComparer();
            var objParsed = JObject.Parse("{\"left\":[1,2,3,{\"Id\":\"10005\",\"name\":\"Charlie\"}],\"selected\":[{\"Path\":\"[3].name\",\"LeftValue\":\"Charlie\",\"RightValue\":\"Charlie!!\"}]}");

            var left = JToken.Parse(objParsed["left"].ToString());
            var selectedDiffs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Difference>>(objParsed["selected"].ToString());
            // Act
            JToken result = obj.Merge(left, selectedDiffs);

            // Act

            JToken expectedResult = JToken.Parse("[1,2,3,{\"Id\":\"10005\",\"name\":\"Charlie!!\"}]");

            Assert.True(JToken.DeepEquals(result, expectedResult), "The result of the compare is incorrect.");

        }

    }



}