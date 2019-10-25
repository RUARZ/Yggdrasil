using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace Yggdrasil.Wpf.Linker
{
    [RegisterLinker(typeof(ComboBox))]
    class ComboBoxLinker : ILinker
    {
        #region Interface Implementation

        public void Link(object viewElement, IEnumerable<LinkData> linkData, Action<object, object, string> createLinkAction)
        {
            if (!(viewElement is ComboBox comboBox))
                return;

            foreach (LinkData data in linkData)
            {
                switch (data.ViewElementName)
                {
                    case nameof(ComboBox.ItemsSource):
                        Binding binding = new Binding(data.ContextMemberInfo.Name);
                        binding.Source = data.Context;
                        BindingOperations.SetBinding(comboBox, ItemsControl.ItemsSourceProperty, binding);
                        break;
                    case nameof(ComboBox.SelectedItem):
                        Binding binding2 = new Binding(data.ContextMemberInfo.Name);
                        binding2.Source = data.Context;
                        BindingOperations.SetBinding(comboBox, Selector.SelectedValueProperty, binding2);
                        break;
                    default:
                        throw new NotSupportedException($"The link for '{data.ViewElementName}' is not supported by '{GetType().Name}'!");
                }
            }
        }

        public void Unlink()
        {
            //nothing to do
        }

        #endregion
    }
}
