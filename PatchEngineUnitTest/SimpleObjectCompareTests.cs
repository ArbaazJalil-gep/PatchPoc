using Newtonsoft.Json.Linq;
using PatchEngine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatchEngineUnitTest
{
    public class SimpleObjectCompareTests
    {
        [Fact]
        public void SimpleObjectSuperNestedObjectTest()
        {
            //nested object has simple arrays

            // Arrange
            var objParsed = JObject.Parse("{\"left\":{\"level1\":{\"level2a\":{\"level3a\":{\"level4a\":{\"level5a\":\"value1\",\"level5b\":\"value2\"},\"level4b\":{\"level5c\":\"value3\",\"level5d\":\"value4\"}},\"level3b\":{\"level4c\":{\"level5e\":\"value5\",\"level5f\":\"value6\"},\"level4d\":{\"level5g\":\"value7\",\"level5h\":\"value8\"}}},\"level2b\":{\"level3c\":{\"level4e\":{\"level5i\":\"value9\",\"level5j\":\"value10\"},\"level4f\":{\"level5k\":\"value11\",\"level5l\":\"value12\"}},\"level3d\":{\"level4g\":{\"level5m\":\"value13\",\"level5n\":\"value14\"},\"level4h\":{\"level5o\":\"value15\",\"level5p\":\"value16\"}}}}},\"right\":{\"level1\":{\"level2a\":{\"level3a\":{\"level4a\":{\"level5a\":\"value1\",\"level5b\":\"value2\"},\"level4b\":{\"level5c\":\"value3\",\"level5d\":\"value4\"}},\"level3b\":{\"level4c\":{\"level5e\":\"value5\",\"level5f\":\"value6\"},\"level4d\":{\"level5g\":\"value7000\",\"level5h\":\"value8\"}}},\"level2b\":{\"level3c\":{\"level4e\":{\"level5i\":\"value9\",\"level5j\":\"value10\"},\"level4f\":{\"level5k\":\"value11\",\"level5l\":\"value12\"}},\"level3d\":{\"level4g\":{\"level5m\":\"value13\",\"level5n\":\"value14\"},\"level4h\":{\"level5o\":\"value15\",\"level5p\":\"value16\"}},\"level2extra\":\"righthandside\"}}}}");

            var left = JToken.Parse(objParsed["left"].ToString());
            var right = JToken.Parse(objParsed["right"].ToString());

            // Act
            JToken result = new JsonComparer().Compare(left, right, "");
            JToken expectedResult = JToken.Parse("[{\"Path\":\"level1.level2a.level3b.level4d.level5g\",\"LeftValue\":\"value7\",\"RightValue\":\"value7000\"},{\"Path\":\"level1.level2b.level2extra\",\"LeftValue\":null,\"RightValue\":\"righthandside\"}]");

            Assert.True(JToken.DeepEquals(result, expectedResult), "The result of the compare is incorrect.");

        }

        [Fact]
        public void SimpleObjectSuperNestedObjectWithSuperNestedArraysTest()  
        {
            //nested object has simple arrays

            // Arrange
            var objParsed = JObject.Parse("{\"left\":{\"level1\":{\"level2a\":{\"level3a\":{\"level4a\":{\"level5a\":\"value1\",\"level5b\":\"value2\"},\"level4b\":{\"level5c\":\"value3\",\"level5d\":\"value4\"}},\"level3b\":{\"level4c\":{\"level5e\":\"value5\",\"level5f\":\"value6\"},\"level4d\":{\"level5g\":\"value7\",\"level5h\":[[[[[1,2],[3,4]],[[5,6],[7,8]]],[[[9,10],[11,12]],[[13,14],[15,16]]]],[[[[17,18],[19,20]],[[21,22],[23,24]]],[[[25,26],[27,28]],[[29,30],[31,32]]]]]}}},\"level2b\":{\"level3c\":{\"level4e\":{\"level5i\":\"value9\",\"level5j\":\"value10\"},\"level4f\":{\"level5k\":\"value11\",\"level5l\":\"value12\"}},\"level3d\":{\"level4g\":{\"level5m\":\"value13\",\"level5n\":\"value14\"},\"level4h\":{\"level5o\":\"value15\",\"level5p\":\"value16\"}}}}},\"right\":{\"level1\":{\"level2a\":{\"level3a\":{\"level4a\":{\"level5a\":\"value1\",\"level5b\":\"value2\"},\"level4b\":{\"level5c\":\"value3\",\"level5d\":\"value4\"}},\"level3b\":{\"level4c\":{\"level5e\":\"value5\",\"level5f\":\"value6\"},\"level4d\":{\"level5g\":\"value7000\",\"level5h\":[[[[[1,2],[3000,4]],[[5,6],[7,8]]],[[[9,10],[11,12]],[[13000,14],[15,16]]]],[[[[17,18],[19,20]],[[21,22],[2300,24]]],[[[25,26],[27,28]],[[29,30],[31,32]]]]]}}},\"level2b\":{\"level3c\":{\"level4e\":{\"level5i\":\"value9\",\"level5j\":\"value10\"},\"level4f\":{\"level5k\":\"value11\",\"level5l\":\"value12\"}},\"level3d\":{\"level4g\":{\"level5m\":\"value13\",\"level5n\":\"value14\"},\"level4h\":{\"level5o\":\"value15\",\"level5p\":\"value16\"}},\"level2extra\":\"righthandside\"}}}}");

            var left = JToken.Parse(objParsed["left"].ToString());
            var right = JToken.Parse(objParsed["right"].ToString());

            // Act
            JToken result = new JsonComparer().Compare(left, right, "");
            JToken expectedResult = JToken.Parse("[{\"Path\":\"level1.level2a.level3b.level4d.level5g\",\"LeftValue\":\"value7\",\"RightValue\":\"value7000\"},{\"Path\":\"level1.level2a.level3b.level4d.level5h[0][0][0][1][0]\",\"LeftValue\":3,\"RightValue\":3000},{\"Path\":\"level1.level2a.level3b.level4d.level5h[0][1][1][0][0]\",\"LeftValue\":13,\"RightValue\":13000},{\"Path\":\"level1.level2a.level3b.level4d.level5h[1][0][1][1][0]\",\"LeftValue\":23,\"RightValue\":2300},{\"Path\":\"level1.level2b.level2extra\",\"LeftValue\":null,\"RightValue\":\"righthandside\"}]");

            Assert.True(JToken.DeepEquals(result, expectedResult), "The result of the compare is incorrect.");

        }

        [Fact]
        public void SimpleObjectSuperNestedObjectWithSuperNestedArraysTest2()  
        {
            //nested object has simple arrays

            // Arrange
            var objParsed = JObject.Parse("{\"left\":{\"level1\":{\"level2a\":{\"level3a\":{\"level4a\":{\"level5a\":\"value1\",\"level5b\":\"value2\"},\"level4b\":{\"level5c\":\"value3\",\"level5d\":\"value4\"}},\"level3b\":{\"level4c\":{\"level5e\":\"value5\",\"level5f\":\"value6\"},\"level4d\":{\"level5g\":\"value7\",\"level5h\":[[[[[1,2],[3,4]],[[5,6],[7,8]]],[[[9,10],[11,12]],[[13,14],[15,16]]]],[[[[17,18],[19,20]],[[21,22],[23,24]]],[[[25,26],[27,28]],[[29,30],[31,{\"level1a\":{\"level2a\":{\"level3a\":\"level 3\"}}}]]]]]}}},\"level2b\":{\"level3c\":{\"level4e\":{\"level5i\":\"value9\",\"level5j\":\"value10\"},\"level4f\":{\"level5k\":\"value11\",\"level5l\":\"value12\"}},\"level3d\":{\"level4g\":{\"level5m\":\"value13\",\"level5n\":\"value14\"},\"level4h\":{\"level5o\":\"value15\",\"level5p\":\"value16\"}}}}},\"right\":{\"level1\":{\"level2a\":{\"level3a\":{\"level4a\":{\"level5a\":\"value1\",\"level5b\":\"value2\"},\"level4b\":{\"level5c\":\"value3\",\"level5d\":\"value4\"}},\"level3b\":{\"level4c\":{\"level5e\":\"value5\",\"level5f\":\"value6\"},\"level4d\":{\"level5g\":\"value7\",\"level5h\":[[[[[1,2],[3,4]],[[5,6],[7,8]]],[[[9,10],[11,12]],[[13,14],[15,16]]]],[[[[17,18],[19,20]],[[21,22],[23,24]]],[[[25,26],[27,28]],[[29,30],[31,{\"level1a\":{\"level2a\":{\"level3a\":\"LEVEL 3\"}}}]]]]]}}},\"level2b\":{\"level3c\":{\"level4e\":{\"level5i\":\"value9\",\"level5j\":\"value10\"},\"level4f\":{\"level5k\":\"value11\",\"level5l\":\"value12\"}},\"level3d\":{\"level4g\":{\"level5m\":\"value13\",\"level5n\":\"value14\"},\"level4h\":{\"level5o\":\"value15\",\"level5p\":\"value16\"}}}}}}");

            var left = JToken.Parse(objParsed["left"].ToString());
            var right = JToken.Parse(objParsed["right"].ToString());

            // Act
            JToken result = new JsonComparer().Compare(left, right, "");
            JToken expectedResult = JToken.Parse("[{\"Path\":\"level1.level2a.level3b.level4d.level5h[1][1][1][1][1].level1a.level2a.level3a\",\"LeftValue\":\"level 3\",\"RightValue\":\"LEVEL 3\"}]");

            Assert.True(JToken.DeepEquals(result, expectedResult), "The result of the compare is incorrect.");

        }

        [Fact]
        public void SimpleObjectSuperNestedObjectWithSuperNestedArraysTest3()
        {
            //nested object has simple arrays

            // Arrange
            var objParsed = JObject.Parse("{\"left\":{\"level1\":{\"level2a\":{\"level3a\":{\"level4a\":{\"level5a\":\"value1\",\"level5b\":\"value2\"},\"level4b\":{\"level5c\":\"value3\",\"level5d\":\"value4\"}},\"level3b\":{\"level4c\":{\"level5e\":\"value5\",\"level5f\":\"value6\"},\"level4d\":{\"level5g\":\"value7\",\"level5h\":[[[[[1,2],[3,4]],[[5,6],[7,8]]],[[[9,10],[11,12]],[[13,14],[15,16]]]],[[[[17,18],[19,20]],[[21,22],[23,24]]],[[[25,26],[27,28]],[[29,30],[{\"Id\":1,\"widgetManager\":{\"Id\":\"widgetManager1\",\"layout\":\"12\",\"widgets\":[{\"Id\":\"widget-16784476843\",\"title\":\"First\",\"behaviour\":{\"Id\":\"000\",\"isVisible\":true,\"isDraggable\":\"yes\"}},{\"Id\":\"widget-000000001\",\"title\":\"Should GetMerged\",\"behaviour\":{\"Id\":\"behaviour1\",\"isVisible\":true,\"isDraggable\":\"no\",\"extraPropertyInLeft\":\"yep i am extra\"}}]},\"children\":[{\"isVisible\":true,\"type\":\"text\",\"Id\":\"widget-16784476843-field-16784479250\",\"label\":\"Field Name\",\"behaviour\":{\"Id\":\"behavior1\",\"autoRender\":false},\"attributes\":{\"Id\":\"attributes1\",\"disable\":false,\"textAlign\":\"left\"}}],\"breadCrumb\":{\"Id\":\"breadCrumb1\",\"navigation\":{\"Id\":\"navigation1\",\"sections\":[{\"Id\":\"sections1\",\"routePath\":\"#\",\"title\":\"untitled\",\"emitEvent\":false}]},\"heading\":{\"Id\":\"heading1\",\"title\":\"Untitled\",\"secondaryTitle\":\"SecondTitle\"}}}]]]]]}}},\"level2b\":{\"level3c\":{\"level4e\":{\"level5i\":\"value9\",\"level5j\":\"value10\"},\"level4f\":{\"level5k\":\"value11\",\"level5l\":\"value12\"}},\"level3d\":{\"level4g\":{\"level5m\":\"value13\",\"level5n\":\"value14\"},\"level4h\":{\"level5o\":\"value15\",\"level5p\":\"value16\"}}}}},\"right\":{\"level1\":{\"level2a\":{\"level3a\":{\"level4a\":{\"level5a\":\"value1\",\"level5b\":\"value2\"},\"level4b\":{\"level5c\":\"value3\",\"level5d\":\"value4\"}},\"level3b\":{\"level4c\":{\"level5e\":\"value5\",\"level5f\":\"value6\"},\"level4d\":{\"level5g\":\"value7\",\"level5h\":[[[[[1,2],[3,4]],[[5,6],[7,8]]],[[[9,10],[11,12]],[[13,14],[15,16]]]],[[[[17,18],[19,20]],[[21,22],[23,24]]],[[[25,26],[27,28]],[[29,30],[{\"Id\":1,\"widgetManager\":{\"Id\":\"widgetManager1\",\"layout\":\"13\",\"widgets\":[{\"Id\":\"widget-16784476843\",\"title\":\"A\",\"behaviour\":{\"Id\":\"000\",\"isVisible\":true,\"isDraggable\":\"yes\"}},{\"Id\":\"widget-22224476843\",\"title\":\"B\",\"behaviour\":{\"Id\":\"behaviour2\",\"isVisible\":true,\"isDraggable\":\"no\"}}]},\"children\":[{\"isVisible\":true,\"type\":\"text\",\"Id\":\"widget-16784476843-field-16784479250\",\"label\":\"New Field Name\",\"behaviour\":{\"Id\":\"behavior1\",\"autoRender\":true},\"attributes\":{\"Id\":\"attributes1\",\"disable\":false,\"textAlign\":\"right\"}}],\"breadCrumb\":{\"Id\":\"breadCrumb1\",\"navigation\":{\"Id\":\"navigation1\",\"sections\":[{\"Id\":\"sections1\",\"routePath\":\"#\",\"title\":\"SomeTitle\",\"emitEvent\":false}]},\"heading\":{\"Id\":\"heading1\",\"title\":\"cool heading\"}},\"toolbar\":{\"Id\":\"t1\",\"toolbarName\":\"Fancy Toolbar\"}}]]]]]}}},\"level2b\":{\"level3c\":{\"level4e\":{\"level5i\":\"value9\",\"level5j\":\"value10\"},\"level4f\":{\"level5k\":\"value11\",\"level5l\":\"value12\"}},\"level3d\":{\"level4g\":{\"level5m\":\"value13\",\"level5n\":\"value14\"},\"level4h\":{\"level5o\":\"value15\",\"level5p\":\"value16\"}}}}}}");

            var left = JToken.Parse(objParsed["left"].ToString());
            var right = JToken.Parse(objParsed["right"].ToString());

            // Act
            JToken result = new JsonComparer().Compare(left, right, "");
            JToken expectedResult = JToken.Parse("[{\"Path\":\"level1.level2a.level3b.level4d.level5h[1][1][1][1]:1.widgetManager.layout\",\"LeftValue\":\"12\",\"RightValue\":\"13\"},{\"Path\":\"level1.level2a.level3b.level4d.level5h[1][1][1][1]:1.widgetManager.widgets:widget-16784476843.title\",\"LeftValue\":\"First\",\"RightValue\":\"A\"},{\"Path\":\"level1.level2a.level3b.level4d.level5h[1][1][1][1]:1.widgetManager.widgets:widget-000000001\",\"LeftValue\":{\"Id\":\"widget-000000001\",\"title\":\"Should GetMerged\",\"behaviour\":{\"Id\":\"behaviour1\",\"isVisible\":true,\"isDraggable\":\"no\",\"extraPropertyInLeft\":\"yep i am extra\"}},\"RightValue\":null},{\"Path\":\"level1.level2a.level3b.level4d.level5h[1][1][1][1]:1.widgetManager.widgets:widget-22224476843\",\"LeftValue\":null,\"RightValue\":{\"Id\":\"widget-22224476843\",\"title\":\"B\",\"behaviour\":{\"Id\":\"behaviour2\",\"isVisible\":true,\"isDraggable\":\"no\"}}},{\"Path\":\"level1.level2a.level3b.level4d.level5h[1][1][1][1]:1.children:widget-16784476843-field-16784479250.label\",\"LeftValue\":\"Field Name\",\"RightValue\":\"New Field Name\"},{\"Path\":\"level1.level2a.level3b.level4d.level5h[1][1][1][1]:1.children:widget-16784476843-field-16784479250.behaviour.autoRender\",\"LeftValue\":false,\"RightValue\":true},{\"Path\":\"level1.level2a.level3b.level4d.level5h[1][1][1][1]:1.children:widget-16784476843-field-16784479250.attributes.textAlign\",\"LeftValue\":\"left\",\"RightValue\":\"right\"},{\"Path\":\"level1.level2a.level3b.level4d.level5h[1][1][1][1]:1.breadCrumb.navigation.sections:sections1.title\",\"LeftValue\":\"untitled\",\"RightValue\":\"SomeTitle\"},{\"Path\":\"level1.level2a.level3b.level4d.level5h[1][1][1][1]:1.breadCrumb.heading.title\",\"LeftValue\":\"Untitled\",\"RightValue\":\"cool heading\"},{\"Path\":\"level1.level2a.level3b.level4d.level5h[1][1][1][1]:1.breadCrumb.heading.secondaryTitle\",\"LeftValue\":\"SecondTitle\",\"RightValue\":null},{\"Path\":\"level1.level2a.level3b.level4d.level5h[1][1][1][1]:1.toolbar\",\"LeftValue\":null,\"RightValue\":{\"Id\":\"t1\",\"toolbarName\":\"Fancy Toolbar\"}}]");

            Assert.True(JToken.DeepEquals(result, expectedResult), "The result of the compare is incorrect.");

        }
    }
}
