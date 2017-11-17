using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace AnyTreeXPath.Json
{
    public class JObjectXPathElement:IXPathElement, ITextProvider
    {
        private readonly JToken _jObject;
        
        public JObjectXPathElement(JObject jObject)
        {
            _jObject = jObject;
        }

        protected JObjectXPathElement(JToken jToken)
        {
            _jObject = jToken;
        }
        
        public virtual object UnderlyingObject
        {
            get { return _jObject; }
        }

        public virtual string GetName()
        {
            var jProperty = _jObject as JProperty;
            return jProperty?.Name ?? string.Empty;
        }

        public virtual IEnumerable<IXPathElement> GetChildren()
        {
            foreach (var jToken in EnumerateExpandChildren())
            {
                yield return CreateXPathElement(jToken);
            }
        }

        protected virtual JObjectXPathElement CreateXPathElement(JToken jArrayItem)
        {
            return new JObjectXPathElement(jArrayItem);
        }

        public virtual IEnumerable<IXPathAttribute> GetAttributes()
        {
            // let's get rid of proprietary Newtonsoft.Json system attributes 'type', 'valuetype' to keep consistency with classical XPath for XML
            // it should be implemented as something like an extension
            //if (_jObject is JProperty)
            //{
            //    // type of inner node, if current node is JProperty
            //    yield return new XPathAttribute("valuetype", (_jObject as JProperty).Value.Type.ToString());
            //}

            //// type of current node
            //yield return new XPathAttribute("type", _jObject.Type.ToString());

            // yield properties with simple values
            foreach (JToken jToken in EnumerateExpandChildren())
            {
                JProperty jProperty = jToken as JProperty;
                if (jProperty != null)
                {
                    JValue jValue = jProperty.Value as JValue;
                    if (jValue != null)
                    {
                        yield return new XPathAttribute(jProperty.Name, Convert.ToString(jValue.Value, CultureInfo.InvariantCulture)); 
                    }
                }
            }
        }

        public string GetText()
        {
            var jValue = GetJValue();
            if (jValue != null)
            {
                return Convert.ToString(jValue.Value, CultureInfo.InvariantCulture);                
            }

            return null;
        }

        public bool HasText
        {
            get { return GetJValue() != null; }
        }

        private JValue GetJValue()
        {
            if (_jObject is JValue)
            {
                return _jObject as JValue;
            }

            if (_jObject is JProperty)
            {
                var jPropertyValue = (_jObject as JProperty).Value;
                if (jPropertyValue is JValue)
                {
                    return jPropertyValue as JValue;
                }
            }

            return null;
        }

        private IEnumerable<JToken> EnumerateExpandChildren()
        {
            foreach (var jToken in _jObject.Children())
            {
                // omit array object and yield array items directly
                JArray jArray = jToken as JArray;
                if (jArray != null)
                {
                    foreach (var jArrayItem in jArray.Children())
                    {
                        yield return jArrayItem;
                    }

                    continue;
                }

                // omit container object and yield property items directly
                JObject jObject = jToken as JObject;
                if (jObject != null)
                {
                    foreach (var jObjectProperty in jObject.Children())
                    {
                        yield return jObjectProperty;
                    }

                    continue;
                }

                yield return jToken;
            }
        }
    }
}
