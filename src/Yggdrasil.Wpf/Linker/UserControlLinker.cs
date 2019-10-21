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
        private INotifyPropertyChanged _notifyPropertyChangedContext;
        private PropertyInfo _visibleStateContextPropertyInfo;

        #region Interface Implementation

        public void Link(object control, object context, Dictionary<string, string> linkDefinitions, Dictionary<string, MemberInfo> foundLinks, Action<object, object, string> createLinkAction)
        {
            if (!(control is UserControl userControl))
                return;

            _userControl = userControl;
            _notifyPropertyChangedContext = context as INotifyPropertyChanged;
            bool registerNotifyPropertyChangeEvent = false;

            foreach(KeyValuePair<string, MemberInfo> link in foundLinks)
            {
                switch (link.Key)
                {
                    case nameof(UserControl.DataContext):
                        Binding binding = new Binding(link.Value.Name);
                        binding.Source = context;
                        BindingOperations.SetBinding(userControl, FrameworkElement.DataContextProperty, binding);
                        break;
                    case nameof(UserControl.Visibility):
                        registerNotifyPropertyChangeEvent = true;
                        _visibleStateContextPropertyInfo = link.Value as PropertyInfo;
                        SetVisibilityState();
                        break;
                    default:
                        throw new NotSupportedException($"The link for '{link.Key}' is not supported by '{GetType().Name}'!");
                }
            }

            if(registerNotifyPropertyChangeEvent && _notifyPropertyChangedContext != null)
                _notifyPropertyChangedContext.PropertyChanged += NotifyPropertyChangedContext_PropertyChanged;                    
        }

        public void Unlink()
        {
            if (_notifyPropertyChangedContext != null)
                _notifyPropertyChangedContext.PropertyChanged -= NotifyPropertyChangedContext_PropertyChanged;
        }

        #endregion

        #region Private Methods

        private void SetVisibilityState()
        {
            _userControl.Visibility = VisibilityHelper.BoolToVisibility((bool)_visibleStateContextPropertyInfo.GetValue(_notifyPropertyChangedContext));
        }

        #endregion

        #region Event Handling

        private void NotifyPropertyChangedContext_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != _visibleStateContextPropertyInfo.Name)
                return;

            SetVisibilityState();
        }

        #endregion
    }
}
