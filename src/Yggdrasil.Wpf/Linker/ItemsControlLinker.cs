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

        public void Link(object control, object context, Dictionary<string, string> linkDefinitions, Dictionary<string, MemberInfo> foundLinks, Action<object, object, string> createLinkAction)
        {
            if (!(control is ItemsControl itemsControl))
                return;

            foreach (KeyValuePair<string, MemberInfo> definition in foundLinks)
            {
                switch (definition.Key)
                {
                    case nameof(ItemsControl.ItemsSource):
                        if (!(definition.Value is PropertyInfo propInfo))
                            throw new NotSupportedException($"The {nameof(ItemsControl.ItemsSource)} must be linked to a property!");

                        if (!(typeof(IEnumerable).IsAssignableFrom(propInfo.PropertyType)))
                            throw new NotSupportedException($"The {nameof(ItemsControl.ItemsSource)} must be linked to a implementation of '{typeof(IEnumerable)}'!");

                        _linkHelper = new ItemsControlLinkHelper(itemsControl, createLinkAction);

                        Binding binding = new Binding(propInfo.Name);
                        binding.Source = context;
                        binding.Mode = BindingMode.OneWay;
                        BindingOperations.SetBinding(itemsControl, ItemsControl.ItemsSourceProperty, binding);
                        break;
                    default:
                        throw new NotSupportedException($"The property name '{definition.Key}' is not supported by '{GetType().Name}'!");
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
