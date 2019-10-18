using System;
using System.Collections.Generic;
using System.Linq;
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
                            Window window = Window.GetWindow(listView);

                            foreach (GridViewColumn col in view.Columns)
                            {
                                createLinkAction(col, context, GetColumnName(col, window));
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

        private string GetColumnName(GridViewColumn column, object context)
        {
            foreach (FieldInfo fieldInfo in context.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
            {
                object value = fieldInfo.GetValue(context);
                if (!typeof(GridViewColumn).IsAssignableFrom(value?.GetType()))
                    continue;

                if (ReferenceEquals(value, column))
                    return fieldInfo.Name;
            }

            return null;
        }

        #endregion
    }
}
