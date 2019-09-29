using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Yggdrasil.Wpf.Linker
{
    [RegisterLinker(typeof(Button))]
    public class ButtonLinker : ILinker
    {
        #region Private Fields

        private Button _button;
        private object _context;
        private PropertyInfo _enabledViewModelPropertyInfo;
        private MethodInfo _clickViewModelMethodInfo;
        private INotifyPropertyChanged _propertyNotifyChangedContext;

        #endregion

        #region Interface Implementation

        public void Link(object control, object context, Dictionary<string, MemberInfo> linkDefinitions, Action<object, object, string> createLinkAction)
        {
            if (!(control is Button button))
                return;

            _button = button;
            _context = context;
            bool commandBindingCreated = false;

            foreach(KeyValuePair<string, MemberInfo> definition in linkDefinitions)
            {
                commandBindingCreated = CreateCommandBindingIfPossible(_button, definition.Value, _context);

                switch (definition.Key)
                {
                    case nameof(Button.IsEnabled):
                        if (commandBindingCreated || !(definition.Value is PropertyInfo propInfo))
                            continue;

                        if (propInfo.PropertyType != typeof(bool))
                            throw new NotSupportedException($"The type '{propInfo.PropertyType}' is not supported for '{nameof(Button.IsEnabled)}'!");

                        button.IsEnabled = (bool)propInfo.GetValue(_context);

                        if (_context is INotifyPropertyChanged propertyChangedContext)
                        {
                            _enabledViewModelPropertyInfo = propInfo;
                            _propertyNotifyChangedContext = propertyChangedContext;
                            propertyChangedContext.PropertyChanged += PropertyChangedModel_PropertyChanged;
                        }
                        break;
                    case nameof(Button.Click):
                        if (commandBindingCreated || !(definition.Value is MethodInfo methodInfo))
                            continue;

                        _clickViewModelMethodInfo = methodInfo;
                        _button.Click += Button_Click;
                        break;
                    default:
                        throw new NotSupportedException($"The link for '{definition.Key}' is not supported by '{GetType().Name}'!");
                }
            }
        }

        public void Unlink()
        {
            if (_propertyNotifyChangedContext != null)
                _propertyNotifyChangedContext.PropertyChanged -= PropertyChangedModel_PropertyChanged;

            _button.Click -= Button_Click;
        }

        #endregion

        #region Private Methods

        private bool CreateCommandBindingIfPossible(Button button, MemberInfo memberInfo, object context)
        {
            if (!(memberInfo is PropertyInfo propInfo) || !typeof(ICommand).IsAssignableFrom(propInfo.PropertyType))
                return false;

            Binding binding = new Binding(memberInfo.Name);
            binding.Source = context;
            BindingOperations.SetBinding(button, System.Windows.Controls.Primitives.ButtonBase.CommandProperty, binding);

            return true;
        }

        #endregion

        #region Events

        private void PropertyChangedModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != _enabledViewModelPropertyInfo.Name)
                return;

            _button.IsEnabled = (bool)_enabledViewModelPropertyInfo.GetValue(_context);
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _clickViewModelMethodInfo.Invoke(_context, null);
        }

        #endregion
    }
}
