using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Yggdrasil.Wpf.Helper;

namespace Yggdrasil.Wpf.Linker
{
    [RegisterLinker(typeof(Button))]
    public class ButtonLinker : ILinker
    {
        #region Private Fields

        private Button _button;
        private MethodInfo _clickViewModelMethodInfo;
        private object _clickContext;
        private readonly PropertyChangedHandler _propertyChangedHandler = new PropertyChangedHandler();

        #endregion

        #region Interface Implementation

        public void Link(object viewElement, IEnumerable<LinkData> linkData, Action<object, object, string> createLinkAction)
        {
            if (!(viewElement is Button button))
                return;

            _button = button;

            foreach (LinkData data in linkData)
            {
                PropertyInfo pInfo = data.ContextMemberInfo as PropertyInfo;

                switch (data.ViewElementName)
                {
                    case nameof(Button.IsEnabled):
                        if (pInfo == null)
                            continue;

                        if (pInfo.PropertyType != typeof(bool))
                            throw new NotSupportedException($"The type '{pInfo.PropertyType}' is not supported for '{nameof(Button.IsEnabled)}'!");

                        button.IsEnabled = (bool)pInfo.GetValue(data.Context);

                        if (data.Context is INotifyPropertyChanged propertyChangedContext)
                        {
                            _propertyChangedHandler.AddNotifyPropertyChangedItem(propertyChangedContext, pInfo.Name, 
                                () => _button.IsEnabled = (bool)pInfo.GetValue(data.Context));
                        }
                        break;
                    case nameof(Button.Click):
                        if (!(data.ContextMemberInfo is MethodInfo methodInfo))
                            continue;

                        _clickViewModelMethodInfo = methodInfo;
                        _clickContext = data.Context;
                        _button.Click += Button_Click;
                        break;
                    case nameof(Button.Command):
                        if (pInfo == null || !typeof(ICommand).IsAssignableFrom(pInfo.PropertyType))
                            continue;

                        BindingHandler.SetBinding(button, ButtonBase.CommandProperty, pInfo, data.Context);
                        break;
                    default:
                        throw new NotSupportedException($"The link for '{data.ViewElementName}' is not supported by '{GetType().Name}'!");
                }
            }
        }

        public void Unlink()
        {
            _propertyChangedHandler.Dispose();
            _button.Click -= Button_Click;
        }

        #endregion

        #region Events

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _clickViewModelMethodInfo.Invoke(_clickContext, null);
        }

        #endregion
    }
}
