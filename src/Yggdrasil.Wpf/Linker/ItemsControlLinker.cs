using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Data;

namespace Yggdrasil.Wpf.Linker
{
    [RegisterLinker(typeof(ItemsControl))]
    public class ItemsControlLinker : ILinker
    {
        #region Private Fields

        private ItemsControlLinkHelper _linkHelper;

        #endregion

        #region Interface Implementation

        public void Link(object viewElement, IEnumerable<LinkData> linkData, Action<object, object, string> createLinkAction)
        {
            if (!(viewElement is ItemsControl itemsControl))
                return;

            foreach (LinkData data in linkData)
            {
                switch (data.ViewElementName)
                {
                    case nameof(ItemsControl.ItemsSource):
                        if (!(data.ContextMemberInfo is PropertyInfo propInfo))
                            throw new NotSupportedException($"The {nameof(ItemsControl.ItemsSource)} must be linked to a property!");

                        if (!(typeof(IEnumerable).IsAssignableFrom(propInfo.PropertyType)))
                            throw new NotSupportedException($"The {nameof(ItemsControl.ItemsSource)} must be linked to a implementation of '{typeof(IEnumerable)}'!");

                        _linkHelper = new ItemsControlLinkHelper(itemsControl, createLinkAction);

                        Binding binding = new Binding(propInfo.Name);
                        binding.Source = data.Context;
                        binding.Mode = BindingMode.OneWay;
                        BindingOperations.SetBinding(itemsControl, ItemsControl.ItemsSourceProperty, binding);
                        break;
                    default:
                        throw new NotSupportedException($"The property name '{data.ViewElementName}' is not supported by '{GetType().Name}'!");
                }
            }
        }

        public void Unlink()
        {
            if (_linkHelper == null)
                return;

            _linkHelper.Dispose();
        }

        #endregion
    }
}
