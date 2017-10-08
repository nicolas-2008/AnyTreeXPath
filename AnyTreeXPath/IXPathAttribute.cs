namespace AnyTreeXPath
{
    public interface IXPathAttribute
    {
        object UnderlyingObject { get; }
        string GetName();
        string GetValue();
    }
}