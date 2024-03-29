﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Yggdrasil.Wpf.Helper;

namespace Yggdrasil.Wpf.Linker
{
    [RegisterLinker(typeof(ListView))]
    public class ListViewLinker : ILinker
    {
        #region Interface Implementations

        public void Link(object viewElement, IEnumerable<LinkData> linkData, Action<object, object, string> createLinkAction)
        {
            if (!(viewElement is ListView listView))
                return;

            foreach (LinkData data in linkData)
            {
                PropertyInfo pInfo = data.ContextMemberInfo as PropertyInfo;

                switch (data.ViewElementName)
                {
                    case nameof(ListView.ItemsSource):
                        BindingHandler.SetBinding(listView, ItemsControl.ItemsSourceProperty, pInfo, data.Context);

                        if (listView.View is GridView view)
                        {
                            //get the window to which the list view belongs to retrieve the information of the columns (to get the name)
                            //because gridviewcolumn is no framework element therefore retrieving the name of a column with .GetValue() does not work
                            //the x:Name creates fields with the columns and the specified name in the window. Therefore the window is needed to 
                            //to get the name of the columns with reflection.
                            Window window = Window.GetWindow(listView);

                            foreach (GridViewColumn col in view.Columns)
                            {
                                createLinkAction(col, data.Context, GetColumnName(col, window));
                            }
                        }
                        break;
                    case nameof(ListView.SelectedItem):
                        BindingHandler.SetBinding(listView, Selector.SelectedItemProperty, pInfo, data.Context);
                        break;
                    default:
                        throw new NotSupportedException($"Property '{data.ViewElementName}' is not supported by {GetType().Name}!");
                }
            }
        }

        public void Unlink()
        {
            
        }

        #endregion

        #region Private Methods

        private string GetColumnName(GridViewColumn column, Window window)
        {
            foreach (FieldInfo fieldInfo in window.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                object value = fieldInfo.GetValue(window);
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
