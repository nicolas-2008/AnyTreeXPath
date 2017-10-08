using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

namespace AnyTreeXPath.WinForms
{
    public class ControlXPathElement : IXPathElement
    {
        protected readonly Control _control;

        public ControlXPathElement(Control control)
        {
            _control = control;
        }

        public virtual string GetName()
        {
            return _control.GetType().Name;
        }

        public virtual IEnumerable<IXPathElement> GetChildren()
        {
            foreach (Control child in _control.Controls)
            {
                yield return CreateXPathElement(child);
            }
        }

        protected virtual ControlXPathElement CreateXPathElement(Control control)
        {
            return new ControlXPathElement(control);
        }

        public virtual IEnumerable<IXPathAttribute> GetAttributes()
        {
            foreach (var propertyInfo in _control.GetType().GetProperties())
            {
                yield return new XPathAttribute(_control, propertyInfo);
            }
        }

        public virtual object UnderlyingObject
        {
            get { return _control; }
        }

        public class XPathAttribute : IXPathAttribute
        {
            private readonly Control _owner;
            private readonly PropertyInfo _propertyInfo;

            public XPathAttribute(Control owner, PropertyInfo propertyInfo)
            {
                _owner = owner;
                _propertyInfo = propertyInfo;
            }
            
            private object GetPropertyValue()
            {
                return _propertyInfo.GetValue(_owner);
            }

            public virtual string GetName()
            {
                return _propertyInfo.Name;
            }

            public virtual string GetValue()
            {
                object value = GetPropertyValue();
                if (value != null)
                {
                    return (value as string) ?? value.ToString();
                }
                else
                {
                    return null;
                }
            }

            public virtual object UnderlyingObject
            {
                get { return GetPropertyValue(); }
            }
        }
    }
}
