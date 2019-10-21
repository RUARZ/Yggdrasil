using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace Yggdrasil.Wpf.Linker
{
    [RegisterLinker(typeof(ComboBox))]
    class ComboBoxLinker : ILinker
    {
        #region Interface Implementation

        public void Link(object control, object context, Dictionary<string, string> linkDefinitions, Dictionary<string, MemberInfo> foundLinks, Action<object, object, string> createLinkAction)
        {
            if (!(control is ComboBox comboBox))
                return;

            foreach (KeyValuePair<string, MemberInfo> link in foundLinks)
            {
                switch(link.Key)
                {
                    case nameof(ComboBox.ItemsSource):
                        Binding binding = new Binding(link.Value.Name);
                        binding.Source = context;
                        BindingOperations.SetBinding(comboBox, ItemsControl.ItemsSourceProperty, binding);
                        break;
                    case nameof(ComboBox.SelectedItem):
                        Binding binding2 = new Binding(link.Value.Name);
                        binding2.Source = context;
                        BindingOperations.SetBinding(comboBox, Selector.SelectedValueProperty, binding2);
                        break;
                    default:
                        throw new NotSupportedException($"The link for '{link.Key}' is not supported by '{GetType().Name}'!");
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
