using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnyTreeXPath;
using AnyTreeXPath.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace UnitTests
{
    [TestClass]
    public class JsonTests
    {
        private static JObject CreateTestJObject()
        {
            var json = JObject.Parse(@"
{
    test1: 'x3',
    test2: [ '1', '2', 'x3' ],
    test3: { 
             'x1':'1', 
             'x2':'2', 
             'x3':31 
           },
    test4: { 
             'x1':'1', 
             'x2':'2', 
             'x3':32 
           },
    test5: { 
             'x1':'1', 
             'x2': [
                     'a', 
                     'b', 
                     ['innerArrItem0', 'innerArrItem1']
                   ], 
             'x3':33.3, 
             'x4':4.0 
           },
}
            ");

            return json;
        }

        [TestMethod]
        public void ComplexQuery()
        {
            var json = CreateTestJObject();

            var navigator = new AnyTreeXPathNavigator(new JObjectXPathElement(json));
            var result = navigator.Select("//*[starts-with(name(),'x')]")
                                  .GetResult<JToken>()
                                  .ToList();
        }
    }
}
