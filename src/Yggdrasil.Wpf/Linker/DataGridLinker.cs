using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Yggdrasil.Wpf.Helper;

namespace Yggdrasil.Wpf.Linker
{
    [RegisterLinker(typeof(DataGrid))]
    public class DataGridLinker : ILinker
    {
        #region Interface Implementation

        public void Link(object viewElement, IEnumerable<LinkData> linkData, Action<object, object, string> createLinkAction)
        {
            if (!(viewElement is DataGrid dataGrid))
                return;

            foreach (LinkData data in linkData)
            {
                PropertyInfo pInfo = data.ContextMemberInfo as PropertyInfo;

                switch (data.ViewElementName)
                {
                    case nameof(DataGrid.ItemsSource):
                        if (pInfo == null || !typeof(IEnumerable).IsAssignableFrom(pInfo.PropertyType))
                            throw new NotSupportedException($"The member info for the '{nameof(DataGrid.ItemsSource)}' link is not of type '{typeof(PropertyInfo)}' or the property is not of type '{typeof(IEnumerable)}'!");

                        foreach (DataGridColumn col in dataGrid.Columns)
                        {
                            createLinkAction.Invoke(col, data.Context, col.GetValue(FrameworkElement.NameProperty) as string);
                        }

                        BindingHandler.SetBinding(dataGrid, ItemsControl.ItemsSourceProperty, pInfo, data.Context);
                        break;
                    case nameof(DataGrid.SelectedItem):
                        if (pInfo == null || typeof(IEnumerable).IsAssignableFrom(pInfo.PropertyType))
                            throw new NotSupportedException($"The member info for the '{nameof(DataGrid.SelectedItem)}' link is not of type '{typeof(PropertyInfo)}' or the property is of type '{typeof(IEnumerable)}'!");

                        BindingHandler.SetBinding(dataGrid, Selector.SelectedItemProperty, pInfo, data.Context);
                        break;
                    default:
                        throw new NotSupportedException($"The property name '{data.ViewElementName}' is not supported by '{GetType().Name}'!");
                }
            }
        }

        public void Unlink()
        {
            
        }

        #endregion
    }
}

