using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Controls;
using Yggdrasil.Wpf.Helper;

namespace Yggdrasil.Wpf.Linker
{

    [RegisterLinker(typeof(TextBlock))]
    public class TextBlockLinker : ILinker
    {
        #region Interface Implementation
        public void Link(object viewElement, IEnumerable<LinkData> linkData, Action<object, object, string> createLinkAction)
        {
            if (!(viewElement is TextBlock textBlock))
                return;

            foreach (LinkData data in linkData)
            {
                PropertyInfo pInfo = data.ContextMemberInfo as PropertyInfo;

                switch (data.ViewElementName)
                {
                    case nameof(TextBlock.Text):
                        BindingHandler.SetBinding(textBlock, TextBlock.TextProperty, pInfo, data.Context);
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
