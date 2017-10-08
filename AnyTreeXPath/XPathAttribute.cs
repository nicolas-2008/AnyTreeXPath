namespace AnyTreeXPath
{
    public class XPathAttribute:IXPathAttribute
    {
        private readonly string _name;
        private readonly string _value;

        public XPathAttribute(string name, string value)
        {
            _name = name;
            _value = value;
        }

        public object UnderlyingObject
        {
            get { return _value; }
        }

        public string GetName()
        {
            return _name;
        }

        public string GetValue()
        {
            return _value;
        }
    }
}
