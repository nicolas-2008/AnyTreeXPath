using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace AnyTreeXPath.Json
{
    /*
     * Json is quite similar to XML, however it has some key differences which should be taken into account:
     *
     * - Json object ( {} ) has no tag name
     * - Json array ( [] ) has no tag name
     * - Json simple value has no tag name 
     * - Json doesn't separate inner items into attributes and nodes
     *
     * The following mappings were implemented to solve these differences and make Json XPath querying more convenient and close to XML: 
     *
     * - Json object declaration is omitted, unless it is an array item.
     * - Json simple value declaration is ommited and treated as a text content, unless it is an array item.
     * - Array item is named according to its index in the array, with '_' prefix.
     * - JProperty with a simple value is contained in both - attributes collection and nodes collection.
     *
     * JSON:
     * ---------------------------
     * {
     *    property1: "100",
     *    property2:
     *      [
     *          {o1:"123"},
     *          {o1:"456"}
     *      ],
     *    property3: [ 123, 456 ]     
     * }
     *----------------------------
     * Corresponding 'XML':
     * ---------------------------
     * <property1>100</property1>
     * <property2>
     *   <_1>
     *     <o1>123</o1>
     *   </_1>
     *   <_2>
     *     <o1>456</o1>
     *   </_2>
     * <property2>
     * <property3>
     *   <_1>123</_1>
     *   <_2>456</_2>
     * <property3>
     * ---------------------------
     *
     */

    public class JObjectXPathElement:IXPathElement, ITextProvider
    {
        protected readonly JTokenDescriptor _jTokenDescriptor;
        
        public JObjectXPathElement(JObject jObject):this(new JTokenDescriptor(jObject))
        {
        }

        protected JObjectXPathElement(JTokenDescriptor jTokenDescriptor)
        {
            _jTokenDescriptor = jTokenDescriptor;
        }
        
        public virtual object UnderlyingObject
        {
            get { return _jTokenDescriptor.JToken; }
        }

        public virtual string GetName()
        {
            return _jTokenDescriptor.Name;
        }

        public virtual IEnumerable<IXPathElement> GetChildren()
        {
            foreach (JTokenDescriptor jTokenDescriptor in EnumerateExpandChildren().Where(jd => !jd.IsSimpleValueOfProperty))
            {
                yield return new JObjectXPathElement(jTokenDescriptor);
            }
        }

        protected virtual JObjectXPathElement CreateXPathElement(JTokenDescriptor jTokenDescriptor)
        {
            return new JObjectXPathElement(jTokenDescriptor);
        }

        public virtual IEnumerable<IXPathAttribute> GetAttributes()
        {
            foreach (JTokenDescriptor jTokenDescriptor in EnumerateExpandChildren().Where(jd => jd.IsPropertyWithSimpleValue))
            {
                yield return new XPathAttribute(jTokenDescriptor.Name, Convert.ToString(jTokenDescriptor.SimpleValue, CultureInfo.InvariantCulture));
            }
        }

        public string GetText()
        {
            if (_jTokenDescriptor.HasSimpleValue)
            {
                return Convert.ToString(_jTokenDescriptor.SimpleValue, CultureInfo.InvariantCulture);  
            }

            return null;
        }

        public bool HasText
        {
            get { return _jTokenDescriptor.HasSimpleValue; }
        }
        
        private IEnumerable<JTokenDescriptor> EnumerateExpandChildren()
        {
            foreach (JToken jToken in _jTokenDescriptor.JToken.Children())
            {
                // omit array object and yield array items directly
                JArray jArray = jToken as JArray;
                if (jArray != null)
                {
                    for (int i = 0; i < jArray.Count; ++i)
                    {
                        var jArrayItem = jArray[i];
                        yield return new JTokenDescriptor(jArrayItem, $"_{i+1}"); // start index is 1 to keep consistency with XPath indexer
                    }

                    continue;
                }

                // omit container object and yield property items directly
                JObject jObject = jToken as JObject;
                if (jObject != null)
                {
                    foreach (JToken jObjectProperty in jObject.Children())
                    {
                        yield return  new JTokenDescriptor(jObjectProperty);
                    }

                    continue;
                }

                yield return new JTokenDescriptor(jToken);
            }
        }

        protected class JTokenDescriptor
        {
            public JToken JToken { get; }

            public string Name { get; }

            public object SimpleValue
            {
                get { return GetSimpleValue(); }
            }
            
            public bool HasSimpleValue
            {
                get { return SimpleValue != null; }
            }

            public bool IsPropertyWithSimpleValue
            {
                get { return (JToken is JProperty) && SimpleValue!=null; }
            }
            
            public bool IsSimpleValue
            {
                get { return JToken is JValue; }
            }

            public bool IsSimpleValueOfProperty
            {
                get { return IsSimpleValue && JToken.Parent is JProperty; }
            }
            
            public JTokenDescriptor(JToken jToken, string name)
            {
                JToken = jToken;
                Name = name;
            }

            public JTokenDescriptor(JToken jToken):this(jToken, GetName(jToken))
            {
            }

            private static string GetName(JToken jToken)
            {
                JProperty jProperty = jToken as JProperty;
                if (jProperty != null)
                {
                    return jProperty.Name;
                }

                return string.Empty;
            }

            private object GetSimpleValue()
            {
                JValue jValue = JToken as JValue;
                if (jValue != null)
                {
                    return jValue.Value;
                }

                JProperty jProperty = JToken as JProperty;
                if (jProperty != null)
                {
                    jValue = jProperty.Value as JValue;
                    if (jValue != null)
                    {
                        return jValue.Value;
                    }
                }

                return null;
            }
        }
    }
}
