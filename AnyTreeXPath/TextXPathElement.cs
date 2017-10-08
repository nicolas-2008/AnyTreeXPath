using System;
using System.Collections.Generic;

namespace AnyTreeXPath
{
    internal class TextXPathElement:IXPathElement
    {
        private readonly ITextProvider _textProvider;
        private string _text;

        public object UnderlyingObject
        {
            get { return _text ?? (_text = _textProvider.GetText()); }
        }

        public TextXPathElement(ITextProvider textProvider)
        {
            _textProvider = textProvider;
        }

        public string GetName()
        {
            throw new InvalidOperationException();
        }

        public IEnumerable<IXPathElement> GetChildren()
        {
            throw new InvalidOperationException();
        }

        public IEnumerable<IXPathAttribute> GetAttributes()
        {
            throw new InvalidOperationException();
        }
    }
}
