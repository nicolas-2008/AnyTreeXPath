using System;
using System.Collections.Generic;
using System.Linq;
using AnyTreeXPath;
using AnyTreeXPath.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace UnitTests
{
    [TestClass]
    public class JsonTests
    {
        [TestMethod]
        public void QueryObjectItems()
        {
            var json = JObject.Parse(@"
                { 
                    book:   'C# in a Nutshell', 
                    author: 'Joseph Albahari',
                    chapters: ['Chapter 1 Introducing C# and the .NET Framework', 'Chapter 2 C# Language Basics', '...']
                }");

            var navigator = new JObjectXPathNavigator(json);
            var queryResult = navigator.Select("/*")
                .GetResult<JProperty>()
                .ToList();

            Assert.AreEqual(3, queryResult.Count);
            Assert.AreEqual("book", queryResult[0].Name);
            Assert.AreEqual("author", queryResult[1].Name);
            Assert.AreEqual("chapters", queryResult[2].Name);
        }
        
        [TestMethod]
        public void QueryArrayItems()
        {
            var json = JObject.Parse(@"
                {
                  books: [ 
                    { name: 'C# in a Nutshell',         author: 'Joseph Albahari' }, 
                    { name: 'Programming WCF Services', author: 'Juval Lowy' } 
                  ]
                }");

            var navigator = new JObjectXPathNavigator(json);
            var queryResult = navigator.Select("/books/*")
                .GetResult<JObject>()
                .ToList();

            Assert.AreEqual(2, queryResult.Count);
            Assert.AreEqual("C# in a Nutshell", queryResult[0].GetValue("name").ToString());
            Assert.AreEqual("Programming WCF Services", queryResult[1].GetValue("name").ToString());
        }

        [TestMethod]
        public void QueryArrayItemByPropertyValue()
        {
            var json = JObject.Parse(@"
                {
                  books: [ 
                    { name: 'C# in a Nutshell',         author: 'Joseph Albahari' }, 
                    { name: 'Programming WCF Services', author: 'Juval Lowy' } 
                  ]
                }");

            var navigator = new JObjectXPathNavigator(json);
            var queryResult = navigator.Select("/books/*[@name='Programming WCF Services']")
                .GetResult<JObject>()
                .ToList();

            Assert.AreEqual(1, queryResult.Count);
            Assert.AreEqual("Programming WCF Services", queryResult[0].GetValue("name").ToString());
            Assert.AreEqual("Juval Lowy", queryResult[0].GetValue("author").ToString());
        }

        [TestMethod]
        public void QueryArrayItemByIndex()
        {
            var json = JObject.Parse(@"
                {
                  books: [ 
                    { name: 'C# in a Nutshell',         author: 'Joseph Albahari' }, 
                    { name: 'Programming WCF Services', author: 'Juval Lowy' } 
                  ]
                }");

            var navigator = new JObjectXPathNavigator(json);
            var queryResult = navigator.Select("/books/_2")
                .GetResult<JObject>()
                .ToList();

            Assert.AreEqual(1, queryResult.Count);
            Assert.AreEqual(queryResult[0].GetValue("name").ToString(), "Programming WCF Services");
            Assert.AreEqual(queryResult[0].GetValue("author").ToString(), "Juval Lowy" );
        }
        
        [TestMethod]
        public void QueryPropertyByName()
        {
            var json = JObject.Parse(@"
                { 
                    book:   'C# in a Nutshell', 
                    author: 'Joseph Albahari' 
                }");

            var navigator = new JObjectXPathNavigator(json);
            var queryResult = navigator.Select("/book")
                                     .GetResult<JProperty>()
                                     .ToList();

            Assert.AreEqual(1, queryResult.Count);
            Assert.AreEqual("book", queryResult[0].Name);
            Assert.AreEqual("C# in a Nutshell", queryResult[0].Value.ToString());
        }

        [TestMethod]
        public void QueryPropertyByContent()
        {
            var json = JObject.Parse(@"
                { 
                    book:   'C# in a Nutshell', 
                    author: 'Joseph Albahari' 
                }");

            var navigator = new JObjectXPathNavigator(json);
            var queryResult = navigator.Select("/author[text()='Joseph Albahari']")
                .GetResult<JProperty>()
                .ToList();

            Assert.AreEqual(1, queryResult.Count);
            Assert.AreEqual("author", queryResult[0].Name);
            Assert.AreEqual("Joseph Albahari", queryResult[0].Value.ToString());
        }
        
        [TestMethod]
        public void QueryPropertyValue()
        {
            var json = JObject.Parse(@"
                { 
                    book:   'C# in a Nutshell', 
                    author: 'Joseph Albahari' 
                }");

            var navigator = new JObjectXPathNavigator(json);
            var queryResult = navigator.Select("/book/text()")
                .GetResult<string>()
                .ToList();

            Assert.AreEqual(1, queryResult.Count);
            Assert.AreEqual("C# in a Nutshell", queryResult[0]);
        }
        
        
        [TestMethod]
        public void QueryInnerLevelItems()
        {
            var json = JObject.Parse(@"
                {
                  books: [ 
                    { name: 'C# in a Nutshell',         author: 'Joseph Albahari' }, 
                    { name: 'Programming WCF Services', author: 'Juval Lowy' } 
                  ]
                }");

            var navigator = new JObjectXPathNavigator(json);
            var queryResult = navigator.Select("/books/*/name/text()")
                .GetResult<string>()
                .ToList();

            Assert.AreEqual(2, queryResult.Count);
            Assert.AreEqual("C# in a Nutshell", queryResult[0]);
            Assert.AreEqual("Programming WCF Services", queryResult[1]);
        }

        [TestMethod]
        public void QueryAllItems()
        {
            var json = JObject.Parse(@"
                { 
                    book:   'C# in a Nutshell', 
                    author: 'Joseph Albahari',
                    chapters: ['Chapter 1 Introducing C# and the .NET Framework', 'Chapter 2 C# Language Basics', '...']
                }");

            var navigator = new JObjectXPathNavigator(json);
            var queryResult = navigator.Select("//*")
                .GetResult<JToken>()
                .ToList();

            Assert.AreEqual(6, queryResult.Count);
            Assert.AreEqual("book", ((JProperty)queryResult[0]).Name);
            Assert.AreEqual("author", ((JProperty)queryResult[1]).Name);
            Assert.AreEqual("chapters", ((JProperty)queryResult[2]).Name);
            Assert.AreEqual("Chapter 1 Introducing C# and the .NET Framework", ((JValue)queryResult[3]).Value);
            Assert.AreEqual("Chapter 2 C# Language Basics", ((JValue)queryResult[4]).Value);
            Assert.AreEqual("...", ((JValue)queryResult[5]).Value);
        }
    }
}
