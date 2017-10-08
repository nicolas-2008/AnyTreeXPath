using System.Collections.Generic;
using System.Xml.XPath;

namespace AnyTreeXPath
{
    public static class Extensions
    {
        public static IEnumerable<T> GetResult<T>(this XPathNodeIterator iterator)
        {
            return new AnyTreeXPathNodeList<T>(iterator);
        }
    }
}
