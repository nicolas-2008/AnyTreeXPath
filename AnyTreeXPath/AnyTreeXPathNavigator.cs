using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.XPath;

namespace AnyTreeXPath
{
    public class AnyTreeXPathNavigator: XPathNavigator
    {
        private AnyElementNode _sourceElementNode;
        private AnyAttributeNode _sourceAttributeNode;
        private XPathNodeType _nodeType;
        private readonly XmlNameTable _nameTableStub = new NameTable();
        
        public AnyTreeXPathNavigator(params IXPathElement[] root)
        {
            _sourceElementNode = new AnyElementNode(null, new CachedEnumerable<IXPathElement>(root), 0);
            _nodeType = XPathNodeType.Root;
        }

        private AnyTreeXPathNavigator(AnyTreeXPathNavigator other)
        {
            _sourceElementNode = other._sourceElementNode;
            _sourceAttributeNode = other._sourceAttributeNode;
            _nodeType = other._nodeType;
            _nameTableStub = other._nameTableStub;
        }
        
        public object GetUnderlyingObject()
        {
            if (_nodeType == XPathNodeType.Attribute)
            {
                return _sourceAttributeNode.Attribute.UnderlyingObject;
            }
            else
            {
                return _sourceElementNode.Element.UnderlyingObject;
            }
        }

        public object GetNode()
        {
            if (_nodeType == XPathNodeType.Attribute)
            {
                return _sourceAttributeNode.Attribute;
            }
            else
            {
                return _sourceElementNode.Element;
            }
        }

        public override string Value
        {
            get
            {
                if (_nodeType == XPathNodeType.Attribute)
                {
                    return _sourceAttributeNode.Attribute.GetValue();
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
        }

        public override XPathNavigator Clone()
        {
            return new AnyTreeXPathNavigator(this);
        }

        public override bool MoveToFirstAttribute()
        {
            var attributes = new CachedEnumerable<IXPathAttribute>(_sourceElementNode.Element.GetAttributes());

            if (attributes.ReadUntil(0))
            {
                _sourceAttributeNode = new AnyAttributeNode(attributes, 0);
                _nodeType = XPathNodeType.Attribute;
                return true;
            }

            return false;
        }

        public override bool MoveToNextAttribute()
        {
            var nextAttribute = _sourceAttributeNode.GetNext();

            if (nextAttribute != null)
            {
                _sourceAttributeNode = nextAttribute;
                _nodeType = XPathNodeType.Attribute;
                return true;
            }

            return false;
        }

        public override bool MoveToNext()
        {
            var nextNode = _sourceElementNode.GetNext();

            if (nextNode != null)
            {
                SetElementNode(nextNode);
                return true;
            }

            return false;
        }

        public override bool MoveToPrevious()
        {
            var prevNode = _sourceElementNode.GetPrevious();

            if (prevNode != null)
            {
                SetElementNode(prevNode);
                return true;
            }

            return false;
        }

        public override bool MoveToFirstChild()
        {
            if (_nodeType == XPathNodeType.Text)
            {
                return false;
            }

            var childrenEnumerable = _sourceElementNode.Element.GetChildren();
            childrenEnumerable = AppendTextNode(childrenEnumerable, _sourceElementNode);

            var children = new CachedEnumerable<IXPathElement>(childrenEnumerable);
            if (children.ReadUntil(0))
            {
                var childNode = new AnyElementNode(_sourceElementNode, children, 0);
                SetElementNode(childNode);
                return true;
            }

            return false;
        }
        
        public override bool MoveToParent()
        {
            if (_sourceElementNode.Parent != null)
            {
                _sourceElementNode = _sourceElementNode.Parent;
                _nodeType = XPathNodeType.Element;
                return true;
            }
            return false;
        }

        public override bool MoveTo(XPathNavigator other)
        {
            return false;
        }

        public override bool MoveToId(string id)
        {
            throw new NotImplementedException();
        }

        public override bool IsSamePosition(XPathNavigator other)
        {
            var otherNav = other as AnyTreeXPathNavigator;
            return otherNav!=null
                && Equals(this._sourceElementNode.Element.UnderlyingObject, otherNav._sourceElementNode.Element.UnderlyingObject)
                && Equals(this._sourceAttributeNode?.Attribute.UnderlyingObject, otherNav._sourceAttributeNode?.Attribute.UnderlyingObject)
                && this._nodeType == otherNav._nodeType;
        }
        
        public override bool MoveToFirstNamespace(XPathNamespaceScope namespaceScope)
        {
            throw new NotSupportedException();
        }

        public override bool MoveToNextNamespace(XPathNamespaceScope namespaceScope)
        {
            throw new NotSupportedException();
        }

        public override XmlNameTable NameTable
        {
            get { return _nameTableStub; }
        }

        public override XPathNodeType NodeType
        {
            get { return _nodeType; }
        }

        public override string LocalName
        {
            get { return Name; }
        }

        public override string Name
        {
            get
            {
                if (_nodeType == XPathNodeType.Attribute)
                {
                    return _sourceAttributeNode.Attribute.GetName();
                }
                else
                {
                    return _sourceElementNode.Element.GetName();
                }
            }
        }

        public override string NamespaceURI
        {
            get
            {
                INamespaceProvider nsProvider = null;
                if (_nodeType == XPathNodeType.Attribute)
                {
                    nsProvider = _sourceAttributeNode.Attribute as INamespaceProvider;
                }
                else
                {
                    nsProvider = _sourceElementNode.Element as INamespaceProvider;
                }

                if (nsProvider != null)
                {
                    return nsProvider.GetNamespace();
                }

                return string.Empty;
            }
        }

        public override string Prefix
        {
            get { return string.Empty; }
        }

        public override string BaseURI
        {
            get { return string.Empty; }
        }

        public override bool IsEmptyElement
        {
            get { return false; }
        }
        
        private void SetElementNode(AnyElementNode node)
        {
            _sourceElementNode = node;
            _nodeType = GetNodeType(_sourceElementNode);
        }

        private IEnumerable<IXPathElement> AppendTextNode(IEnumerable<IXPathElement> childrenEnumerable, AnyElementNode elementNode)
        {
            if (elementNode.Element is ITextProvider)
            {
                ITextProvider textProvider = elementNode.Element as ITextProvider;
                childrenEnumerable = childrenEnumerable.Concat(new[] { new TextXPathElement(textProvider) });
            }
            return childrenEnumerable;
        }

        private static XPathNodeType GetNodeType(AnyElementNode node)
        {
            if (node.Element is TextXPathElement)
            {
                return XPathNodeType.Text;
            }
            else
            {
                return XPathNodeType.Element;
            }
        }
    }
}
