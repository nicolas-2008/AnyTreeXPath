using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.XPath;

namespace AnyTreeXPath
{
    public class AnyTreeXPathNodeList<T> : IEnumerable<T>
    {
        protected readonly CachedEnumerable<AnyTreeXPathNavigator> _list;
        protected readonly XPathNodeIterator _nodeIterator;

        public int Count
        {
            get { return _list.Count; }
        }

        public AnyTreeXPathNodeList(XPathNodeIterator nodeIterator)
        {
            _nodeIterator = nodeIterator;
            _list = new CachedEnumerable<AnyTreeXPathNavigator>(nodeIterator.Cast<AnyTreeXPathNavigator>());
        }

        public virtual IEnumerator<T> GetEnumerator()
        {
            foreach (var n in _list)
            {
                yield return (T)n.GetUnderlyingObject();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}