using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Controls;
using Yggdrasil.Wpf.Helper;

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
                PropertyInfo pInfo = data.ContextMemberInfo as PropertyInfo;

                switch (data.ViewElementName)
                {
                    case nameof(ItemsControl.ItemsSource):
                        if (pInfo == null)
                            throw new NotSupportedException($"The {nameof(ItemsControl.ItemsSource)} must be linked to a property!");

                        if (!(typeof(IEnumerable).IsAssignableFrom(pInfo.PropertyType)))
                            throw new NotSupportedException($"The {nameof(ItemsControl.ItemsSource)} must be linked to a implementation of '{typeof(IEnumerable)}'!");

                        _linkHelper = new ItemsControlLinkHelper(itemsControl, createLinkAction);

                        BindingHandler.SetBinding(itemsControl, ItemsControl.ItemsSourceProperty, pInfo, data.Context);
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
