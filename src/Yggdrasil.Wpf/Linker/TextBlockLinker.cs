using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Data;
using Yggdrasil.Transposer;

namespace Yggdrasil.Wpf.Linker
{

    [RegisterLinker(typeof(TextBlock))]
    public class TextBlockLinker : ILinker
    {
        #region Private Fields

        private TextBlock _textBlock;

        #endregion

        #region Interface Implementation

        public void Link(object control, object context, Dictionary<string, string> linkDefinitions, Dictionary<string, MemberInfo> foundLinks, Action<object, object, string> createLinkAction)
        {
            if (!(control is TextBlock textBlock))
                return;

            foreach (KeyValuePair<string, MemberInfo> link in foundLinks)
            {
                switch (link.Key)
                {
                    case nameof(TextBlock.Text):
                        Binding binding = new Binding(link.Value.Name);
                        binding.Source = context;
                        BindingOperations.SetBinding(textBlock, TextBlock.TextProperty, binding);
                        break;
                    default:
                        throw new NotSupportedException($"Property '{link.Key}' is not supported by {GetType().Name}!");
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
