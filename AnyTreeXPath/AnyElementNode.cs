namespace AnyTreeXPath
{
    internal class AnyElementNode
    {
        private readonly CachedEnumerable<IXPathElement> _elements;
        private readonly int _index;
        public IXPathElement Element { get; }
        public AnyElementNode Parent { get; }

        public AnyElementNode(AnyElementNode parent, CachedEnumerable<IXPathElement> elements, int index)
        {
            Parent = parent;
            Element = elements[index];
            _elements = elements;
            _index = index;
        }

        public AnyElementNode GetNext()
        {
            int next = _index + 1;
            if (_elements.ReadUntil(next))
            {
                return new AnyElementNode(Parent, _elements, next);
            }

            return null;
        }

        public AnyElementNode GetPrevious()
        {
            if (_index > 0)
            {
                return new AnyElementNode(Parent, _elements, _index - 1);
            }

            return null;
        }
    }
}