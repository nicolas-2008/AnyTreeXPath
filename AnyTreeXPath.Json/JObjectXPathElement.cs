using System;
using System.Collections.Generic;
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
            foreach (var jToken in _jObject.Children())
            {
                // omit array object and yield array items directly
                JArray jArray = jToken as JArray;
                if (jArray != null)
                {
                    foreach (var jArrayItem in jArray.Children())
                    {
                        yield return CreateXPathElement(jArrayItem);
                    }

                    continue;
                }

                // omit container object and yield property items directly
                JObject jObject = jToken as JObject;
                if (jObject != null)
                {
                    foreach (var jObjectProperty in jObject.Children())
                    {
                        yield return CreateXPathElement(jObjectProperty);
                    }

                    continue;
                }

                yield return CreateXPathElement(jToken);
            }
        }

        protected virtual JObjectXPathElement CreateXPathElement(JToken jArrayItem)
        {
            return new JObjectXPathElement(jArrayItem);
        }

        public IEnumerable<IXPathAttribute> GetAttributes()
        {
            yield return new XPathAttribute("type", ((_jObject as JProperty)?.Value ?? _jObject).Type.ToString());
        }

        public string GetText()
        {
            return _jObject.ToString();
        }
    }
}
