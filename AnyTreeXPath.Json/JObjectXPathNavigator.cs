using Newtonsoft.Json.Linq;

namespace AnyTreeXPath.Json
{
    public class JObjectXPathNavigator:AnyTreeXPathNavigator
    {
        public JObjectXPathNavigator(JObjectXPathElement root) : base(root)
        {
        }

        public JObjectXPathNavigator(JObject root):base(new JObjectXPathElement(root))
        {
        }
    }
}
