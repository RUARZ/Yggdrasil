using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Yggdrasil.Wpf.Helper;

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
                PropertyInfo pInfo = data.ContextMemberInfo as PropertyInfo;

                switch (data.ViewElementName)
                {
                    case nameof(ComboBox.ItemsSource):
                        if (pInfo == null)
                            continue;

                        BindingHandler.SetBinding(comboBox, ItemsControl.ItemsSourceProperty, pInfo, data.Context);
                        break;
                    case nameof(ComboBox.SelectedItem):
                        if (pInfo == null)
                            continue;

                        BindingHandler.SetBinding(comboBox, Selector.SelectedValueProperty, pInfo, data.Context);
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
