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
            var objParsed = JObject.Parse("{\"left\":[\"geo\",[\"phy\",\"cm\"],\"a\",[\"C\"]],\"right\":[\"geo\",[\"phy\",\"chem\",\"bio\"],\"b\",[\"c\",\"D\"]]}");

            var left = JToken.Parse(objParsed["left"].ToString());
            var right = JToken.Parse(objParsed["right"].ToString());

            // Act
            JToken result = new JsonComparer().Compare(left, right, "");
            JToken expectedResult = JToken.Parse("[{\"Path\":\"[1][1]\",\"LeftValue\":\"cm\",\"RightValue\":\"chem\",\"Op\":\"replace\"},{\"Path\":\"[1]\",\"LeftValue\":null,\"RightValue\":\"bio\",\"Op\":\"add\"},{\"Path\":\"[2]\",\"LeftValue\":\"a\",\"RightValue\":\"b\",\"Op\":\"replace\"},{\"Path\":\"[3][0]\",\"LeftValue\":\"C\",\"RightValue\":\"c\",\"Op\":\"replace\"},{\"Path\":\"[3]\",\"LeftValue\":null,\"RightValue\":\"D\",\"Op\":\"add\"}]");
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
            JToken expectedResult = JToken.Parse("[{\"Path\":\"[0][0][0][1][0]\",\"LeftValue\":3,\"RightValue\":30,\"Op\":\"replace\"},{\"Path\":\"[0][1][1][1][0]\",\"LeftValue\":15,\"RightValue\":150,\"Op\":\"replace\"},{\"Path\":\"[1][1][0][1][1]\",\"LeftValue\":28,\"RightValue\":280,\"Op\":\"replace\"}]");
            Assert.True(JToken.DeepEquals(result, expectedResult), "The result of the compare is incorrect.");

        }

        [Fact]
        public void TestCompare3()
        {
            // Arrange
            var objParsed = JObject.Parse("{\"left\":[1,2,3,{\"Id\":\"10005\",\"name\":\"Charlie\"}],\"right\":[1,2,3,{\"Id\":\"10005\",\"name\":\"Charlie!!\"}]}");

            var left = JToken.Parse(objParsed["left"].ToString());
            var right = JToken.Parse(objParsed["right"].ToString());

            // Act
            JToken result = new JsonComparer().Compare(left, right, "");
            JToken expectedResult = JToken.Parse("[{\"Path\":\"[3].name\",\"LeftValue\":\"Charlie\",\"RightValue\":\"Charlie!!\",\"Op\":\"replace\"}]");

            Assert.True(JToken.DeepEquals(result, expectedResult), "The result of the compare is incorrect.");

        }
        [Fact]
        public void TestCompare4()
        {
            // Arrange
            var objParsed = JObject.Parse("{\"left\":{\"simpleObject\":{\"level1a\":{\"level2a\":{\"level3a\":\"level 3\"}},\"simpleArray\":[1,2,3,[10,20,[100,200]]]},\"identityObject\":{\"Id\":\"10001\",\"name\":\"Arbaaz\",\"simpleArrayInsideIdentityObject\":[4,5,6,[30,40,[300,400]]],\"nestedObject\":{\"Id\":\"10002\",\"name\":\"NestedIdentityObject\",\"details\":{\"age\":25,\"city\":\"San Francisco\"}}},\"arrayWithIdentityObjects\":[{\"Id\":\"10003\",\"name\":\"Alice\"},{\"Id\":\"10004\",\"name\":\"Bob\",\"simpleArrayInsideIdentityObject\":[7,8,9,[50,60,[500,600]]]}],\"mixedArray\":[1,2,3,{\"mixedArray\":[1,2,3,{\"Id\":\"10005\",\"name\":\"Charlie\"},{\"Id\":\"10006\",\"name\":\"Diana\",\"simpleArrayInsideIdentityObject\":[11,12,13,[70,80,[700,800]]],\"nestedObject\":{\"Id\":\"10007\",\"name\":\"NestedIdentityObject2\",\"details\":{\"age\":30,\"city\":\"New York\"}}},[14,15,16,[90,100,[900,1000]]],{\"level1b\":{\"level2b\":{\"level3b\":\"level 3\"}}}]}]},\"right\":{\"simpleObject\":{\"level1a\":{\"level2a\":{\"level3a\":\"level 3\"}},\"simpleArray\":[1,2,3,[10,20,[100,200]]]},\"identityObject\":{\"Id\":\"10001\",\"name\":\"Arbaaz\",\"simpleArrayInsideIdentityObject\":[4,5,6,[30,40,[300,400]]],\"nestedObject\":{\"Id\":\"10002\",\"name\":\"NestedIdentityObject\",\"details\":{\"age\":25,\"city\":\"San Francisco!!!\"}}},\"arrayWithIdentityObjects\":[{\"Id\":\"10003\",\"name\":\"Alice\"},{\"Id\":\"10004\",\"name\":\"Bob\",\"simpleArrayInsideIdentityObject\":[7,8,9,[50,60,[500]]]}],\"mixedArray\":[1,2,3,{\"mixedArray\":[1,2,3,{\"Id\":\"10005\",\"name\":\"Charlie\"},{\"Id\":\"10006\",\"name\":\"Diana\",\"simpleArrayInsideIdentityObject\":[11,12,13,[70,80,[700111,800]]],\"nestedObject\":{\"Id\":\"10007\",\"name\":\"NestedIdentityObject2\",\"details\":{\"age\":30,\"city\":\"New York\"}}},[14,15,16,[90,100,[900,1000]]],{\"level1b\":{\"level2b\":{\"level3b\":\"level 3\"}}}]}]}}");

            var left = JToken.Parse(objParsed["left"].ToString());
            var right = JToken.Parse(objParsed["right"].ToString());

            // Act
            JToken result = new JsonComparer().Compare(left, right, "");
            JToken expectedResult = JToken.Parse("[{\"Path\":\"identityObject.nestedObject.details.city\",\"LeftValue\":\"San Francisco\",\"RightValue\":\"San Francisco!!!\",\"Op\":\"replace\"},{\"Path\":\"arrayWithIdentityObjects:10004.simpleArrayInsideIdentityObject[3][2][1]\",\"LeftValue\":600,\"RightValue\":null,\"Op\":\"remove\"},{\"Path\":\"mixedArray[3].mixedArray[4].simpleArrayInsideIdentityObject[3][2][0]\",\"LeftValue\":700,\"RightValue\":700111,\"Op\":\"replace\"}]");

            Assert.True(JToken.DeepEquals(result, expectedResult), "The result of the compare is incorrect.");

        }
    }
}
