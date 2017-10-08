namespace AnyTreeXPath
{
    internal class AnyAttributeNode
    {
        private readonly CachedEnumerable<IXPathAttribute> _attributes;
        private readonly int _index;
        private readonly IXPathAttribute _attribute;

        public IXPathAttribute Attribute
        {
            get { return _attribute; }
        }

        public AnyAttributeNode(CachedEnumerable<IXPathAttribute> attributes, int index)
        {
            _attributes = attributes;
            _index = index;
            _attribute = _attributes[index];
        }

        public AnyAttributeNode GetNext()
        {
            int next = _index + 1;
            if (_attributes.ReadUntil(next))
            {
                return new AnyAttributeNode(_attributes, next);
            }

            return null;
        }
    }
}