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

        public void Link(object control, object context, Dictionary<string, string> linkDefinitions, Dictionary<string, MemberInfo> foundLinks, Action<object, object, string> createLinkAction)
        {
            if (!(control is DataGrid dataGrid))
                return;

            foreach (KeyValuePair<string, MemberInfo> definition in foundLinks)
            {
                switch (definition.Key)
                {
                    case nameof(DataGrid.ItemsSource):
                        if (!(definition.Value is PropertyInfo itemsSourcePInfo) || !typeof(IEnumerable).IsAssignableFrom(itemsSourcePInfo.PropertyType))
                            throw new NotSupportedException($"The member info for the '{nameof(DataGrid.ItemsSource)}' link is not of type '{typeof(PropertyInfo)}' or the property is not of type '{typeof(IEnumerable)}'!");

                        CreateBinding(dataGrid, DataGrid.ItemsSourceProperty, itemsSourcePInfo.Name, context, BindingMode.OneWay);
                        break;
                    case nameof(DataGrid.SelectedItem):
                        if (!(definition.Value is PropertyInfo selectedItemPInfo) || typeof(IEnumerable).IsAssignableFrom(selectedItemPInfo.PropertyType))
                            throw new NotSupportedException($"The member info for the '{nameof(DataGrid.SelectedItem)}' link is not of type '{typeof(PropertyInfo)}' or the property is of type '{typeof(IEnumerable)}'!");

                        CreateBinding(dataGrid, DataGrid.SelectedItemProperty, selectedItemPInfo.Name, context, BindingMode.TwoWay);
                        break;
                    case nameof(DataGrid.SelectedItems):
                        if (!(definition.Value is PropertyInfo selectedItemsPInfo) || !typeof(IEnumerable).IsAssignableFrom(selectedItemsPInfo.PropertyType))
                            throw new NotSupportedException($"The member info for the '{nameof(DataGrid.ItemsSource)}' link is not of type '{typeof(PropertyInfo)}' or the property is not of type '{typeof(IEnumerable)}'!");

                        CreateBinding(dataGrid, DataGrid.ItemsSourceProperty, selectedItemsPInfo.Name, context, BindingMode.OneWay);
                        break;
                    default:
                        throw new NotSupportedException($"The property name '{definition.Key}' is not supported by '{GetType().Name}'!");
                }
            }

            foreach (System.Windows.Controls.DataGridColumn col in dataGrid.Columns)
            {
                createLinkAction.Invoke(col, context, col.GetValue(FrameworkElement.NameProperty) as string);
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

