using Newtonsoft.Json.Linq;

namespace AnyTreeXPath.Json
{
    public class JObjectXPathNavigator:AnyTreeXPathNavigator
    {
        public JObjectXPathNavigator(JObjectXPathElement rootElement) : base(rootElement)
        {
        }

        public JObjectXPathNavigator(JObject rootControl):base(new JObjectXPathElement(rootControl))
        {
        }
    }
}
