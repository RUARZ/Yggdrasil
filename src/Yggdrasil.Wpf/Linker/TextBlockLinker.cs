using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Data;

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
                switch (data.ViewElementName)
                {
                    case nameof(TextBlock.Text):
                        Binding binding = new Binding(data.ContextMemberInfo.Name);
                        binding.Source = data.Context;
                        BindingOperations.SetBinding(textBlock, TextBlock.TextProperty, binding);
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
