using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Yggdrasil.Wpf.Helper;

namespace Yggdrasil.Wpf.Linker
{
    [RegisterLinker(typeof(UserControl))]
    public class UserControlLinker : ILinker
    {
        private UserControl _userControl;
        private readonly PropertyChangedHandler _propertyChangedHandler = new PropertyChangedHandler();

        #region Interface Implementation

        public void Link(object viewElement, IEnumerable<LinkData> linkData, Action<object, object, string> createLinkAction)
        {
            if (!(viewElement is UserControl userControl))
                return;

            _userControl = userControl;

            foreach (LinkData data in linkData)
            {
                switch (data.ViewElementName)
                {
                    case nameof(UserControl.DataContext):
                        Binding binding = new Binding(data.ContextMemberInfo.Name);
                        binding.Source = data.Context;
                        BindingOperations.SetBinding(userControl, FrameworkElement.DataContextProperty, binding);
                        break;
                    case nameof(UserControl.Visibility):
                        if (!(data.ContextMemberInfo is PropertyInfo pInfo))
                            continue;

                        SetVisibilityState(pInfo, data.Context);

                        if (data.Context is INotifyPropertyChanged propertyChangedContext)
                        {
                            _propertyChangedHandler.AddNotifyPropertyChangedItem(propertyChangedContext, pInfo.Name,
                                () => SetVisibilityState(pInfo, data.Context));
                        }
                        break;
                    default:
                        throw new NotSupportedException($"The link for '{data.ViewElementName}' is not supported by '{GetType().Name}'!");
                }
            }
        }

        public void Unlink()
        {
            _propertyChangedHandler.Dispose();
        }

        #endregion

        #region Private Methods

        private void SetVisibilityState(PropertyInfo pInfo, object context)
        {
            _userControl.Visibility = VisibilityHelper.BoolToVisibility((bool)pInfo.GetValue(context));
        }

        #endregion
    }
}
