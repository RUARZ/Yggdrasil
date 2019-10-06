using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Yggdrasil.Wpf.Linker
{
    [RegisterLinker(typeof(ListView))]
    public class ListViewLinker : ILinker
    {
        #region Private Fields

        private ItemsControlLinkHelper _linkHelper;

        #endregion

        #region Interface Implementations

        public void Link(object control, object context, Dictionary<string, string> linkDefinitions, Dictionary<string, MemberInfo> foundLinks, Action<object, object, string> createLinkAction)
        {
            if (!(control is ListView listView))
                return;

            foreach (KeyValuePair<string, MemberInfo> link in foundLinks)
            {
                switch(link.Key)
                {
                    case nameof(ListView.ItemsSource):
                        BindingOperations.SetBinding(listView, ItemsControl.ItemsSourceProperty, CreateBinding(link.Value.Name, context));

                        if (listView.View is GridView view)
                        {
                            foreach (GridViewColumn col in view.Columns)
                            {
                                createLinkAction(col, context, col.GetValue(FrameworkElement.NameProperty).ToString());
                            }
                        }
                        break;
                    case nameof(ListView.SelectedItem):
                        BindingOperations.SetBinding(listView, ListBox.SelectedItemProperty, CreateBinding(link.Value.Name, context));
                        break;
                    default:
                        throw new NotSupportedException($"Property '{link.Key}' is not supported by {GetType().Name}!");
                }
            }
        }

        public void Unlink()
        {
            _linkHelper?.Dispose();
        }

        #endregion

        #region Private Methods

        private Binding CreateBinding(string propertyName, object context)
        {
            Binding binding = new Binding(propertyName);

            if (context != null)
                binding.Source = context;

            return binding;
        }

        #endregion
    }
}
