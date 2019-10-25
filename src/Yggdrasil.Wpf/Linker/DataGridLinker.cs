using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

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
                switch (data.ViewElementName)
                {
                    case nameof(DataGrid.ItemsSource):
                        if (!(data.ContextMemberInfo is PropertyInfo itemsSourcePInfo) || !typeof(IEnumerable).IsAssignableFrom(itemsSourcePInfo.PropertyType))
                            throw new NotSupportedException($"The member info for the '{nameof(DataGrid.ItemsSource)}' link is not of type '{typeof(PropertyInfo)}' or the property is not of type '{typeof(IEnumerable)}'!");

                        foreach (DataGridColumn col in dataGrid.Columns)
                        {
                            createLinkAction.Invoke(col, data.Context, col.GetValue(FrameworkElement.NameProperty) as string);
                        }

                        CreateBinding(dataGrid, DataGrid.ItemsSourceProperty, itemsSourcePInfo.Name, data.Context, BindingMode.OneWay);
                        break;
                    case nameof(DataGrid.SelectedItem):
                        if (!(data.ContextMemberInfo is PropertyInfo selectedItemPInfo) || typeof(IEnumerable).IsAssignableFrom(selectedItemPInfo.PropertyType))
                            throw new NotSupportedException($"The member info for the '{nameof(DataGrid.SelectedItem)}' link is not of type '{typeof(PropertyInfo)}' or the property is of type '{typeof(IEnumerable)}'!");

                        CreateBinding(dataGrid, DataGrid.SelectedItemProperty, selectedItemPInfo.Name, data.Context, BindingMode.TwoWay);
                        break;
                    case nameof(DataGrid.SelectedItems):
                        if (!(data.ContextMemberInfo is PropertyInfo selectedItemsPInfo) || !typeof(IEnumerable).IsAssignableFrom(selectedItemsPInfo.PropertyType))
                            throw new NotSupportedException($"The member info for the '{nameof(DataGrid.ItemsSource)}' link is not of type '{typeof(PropertyInfo)}' or the property is not of type '{typeof(IEnumerable)}'!");

                        CreateBinding(dataGrid, DataGrid.ItemsSourceProperty, selectedItemsPInfo.Name, data.Context, BindingMode.OneWay);
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

        #region Private Methods

        private void CreateBinding(DataGrid dataGrid, DependencyProperty dependencyProperty, string path, object source, BindingMode mode)
        {
            Binding binding = new Binding(path);
            binding.Source = source;
            binding.Mode = mode;

            BindingOperations.SetBinding(dataGrid, dependencyProperty, binding);
        }

        #endregion
    }
}

