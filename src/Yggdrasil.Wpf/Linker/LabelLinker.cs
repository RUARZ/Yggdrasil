using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Controls;
using Yggdrasil.Wpf.Helper;

namespace Yggdrasil.Wpf.Linker
{
    [RegisterLinker(typeof(Label))]
    public class LabelLinker : ILinker
    {
        #region Interface Implementation

        public void Link(object viewElement, IEnumerable<LinkData> linkData, Action<object, object, string> createLinkAction)
        {
            if (!(viewElement is Label label))
                return;

            foreach (LinkData data in linkData)
            {
                PropertyInfo pInfo = data.ContextMemberInfo as PropertyInfo;

                switch (data.ViewElementName)
                {
                    case nameof(Label.Content):
                        BindingHandler.SetBinding(label, ContentControl.ContentProperty, pInfo, data.Context);
                        break;
                    default:
                        throw new NotSupportedException($"Property '{data.ViewElementName}' is not supported by {GetType().Name}!");
                }
            }
        }

        public void Unlink()
        {
            // nothing to do!
        }

        #endregion
    }
}
