using System.Collections.Generic;

namespace AnyTreeXPath
{
    public interface IXPathElement
    {
        object UnderlyingObject { get; }
        string GetName();
        IEnumerable<IXPathElement> GetChildren();
        IEnumerable<IXPathAttribute> GetAttributes();
    }
}