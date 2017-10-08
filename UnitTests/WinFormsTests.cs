using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using AnyTreeXPath;
using AnyTreeXPath.WinForms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class WinFormsTests
    {
        [TestMethod]
        public void QueryAllElementsByNodeName()
        {
            var form = new SampleForm();

            WinFormsXPathNavigator navigator = new WinFormsXPathNavigator(form);
            var resultList = navigator.Select("//Label")
                                      .GetResult<Control>()
                                      .ToList();

            Assert.AreEqual(6, resultList.Count);
            Assert.IsTrue(resultList.All(c=>c.GetType() == typeof(Label)));
        }

        [TestMethod]
        public void QueryElementByNameAttribute()
        {
            var form = new SampleForm();

            WinFormsXPathNavigator navigator = new WinFormsXPathNavigator(form);
            var resultList = navigator.Select("//TextBox[@Name='textBox3']")
                                      .GetResult<Control>()
                                      .ToList();

            Assert.AreEqual(1, resultList.Count);
            Assert.AreEqual(typeof(TextBox), resultList[0].GetType());
            Assert.AreEqual("textBox3", resultList[0].Name);
        }

        [TestMethod]
        public void QueryElementByIndex()
        {
            var form = new SampleForm();

            WinFormsXPathNavigator navigator = new WinFormsXPathNavigator(form);
            var resultList = navigator.Select("(//TextBox)[1]")
                                          .GetResult<Control>()
                                          .ToList();

            Assert.AreEqual(1, resultList.Count);
            Assert.AreEqual(typeof(TextBox), resultList[0].GetType());
            Assert.AreEqual("firstTextBox", resultList[0].Name);
        }

        [TestMethod]
        public void QueryElementByFullPathAndNameAttribute()
        {
            var form = new SampleForm();

            WinFormsXPathNavigator navigator = new WinFormsXPathNavigator(form);
            var resultList = navigator.Select("/Panel[@Name='panel1']/GroupBox[@Name='groupBox2']/Label[@Name='label4']")
                                          .GetResult<Control>()
                                          .ToList();

            Assert.AreEqual(1, resultList.Count);
            Assert.AreEqual(typeof(Label), resultList[0].GetType());
            Assert.AreEqual("label4", resultList[0].Name);
        }

        [TestMethod]
        public void QueryAttribute()
        {
            var form = new SampleForm();

            WinFormsXPathNavigator navigator = new WinFormsXPathNavigator(form);
            var resultList = navigator.Select("/Panel/GroupBox/@BackColor")
                                          .GetResult<Color>()
                                          .ToList();

            Assert.AreEqual(1, resultList.Count);
            Assert.AreEqual(Color.LightGreen, resultList[0]);
        }

        [TestMethod]
        public void QueryByNonStringAttribute()
        {
            var form = new SampleForm();

            WinFormsXPathNavigator navigator = new WinFormsXPathNavigator(form);
            var resultList = navigator.Select("/Panel/GroupBox[contains(@BackColor, 'LightGreen')]")
                                          .GetResult<Control>()
                                          .ToList();

            Assert.AreEqual(1, resultList.Count);
            Assert.AreEqual("groupBox2", resultList[0].Name);
        }

        [TestMethod]
        public void QueryElementWithNsPrefix()
        {
            var form = new SampleForm();

            WinFormsXPathNavigator navigator = new WinFormsXPathNavigator(new TestElementWithNamespace(form));
            var nsResolver = new XmlNamespaceManager(new NameTable());
            nsResolver.AddNamespace("x1", "http://dummy.com");

            var resultList = navigator.Select("/x1:Panel/GroupBox", nsResolver)
                                          .GetResult<Control>()
                                          .ToList();

            Assert.AreEqual(1, resultList.Count);
        }

        private class TestElementWithNamespace : ControlXPathElement, INamespaceProvider
        {
            public TestElementWithNamespace(Control control) : base(control)
            {
            }

            protected override ControlXPathElement CreateXPathElement(Control control)
            {
                return new TestElementWithNamespace(control);
            }

            public string GetNamespace()
            {
                if (_control.GetType() == typeof(Panel))
                {
                    return "http://dummy.com";
                }

                return string.Empty;
            }
        }
    }
}
