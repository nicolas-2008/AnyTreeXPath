using System.Windows.Forms;

namespace AnyTreeXPath.WinForms
{
    public class WinFormsXPathNavigator:AnyTreeXPathNavigator
    {
        public WinFormsXPathNavigator(ControlXPathElement rootElement) : base(rootElement)
        {
        }

        public WinFormsXPathNavigator(Control rootControl):base(new ControlXPathElement(rootControl))
        {
        }
    }
}
