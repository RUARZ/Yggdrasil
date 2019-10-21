using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Data;

namespace Yggdrasil.Wpf.Linker
{
    [RegisterLinker(typeof(TextBox))]
    class TextBoxLinker : ILinker
    {
        #region Private Fields

        private TextBox _textBox;
        private INotifyPropertyChanged _propertyChangedModel;
        private PropertyInfo _isEnabledInfo;

        #endregion

        #region Interface Impelementation

        public void Link(object control, object context, Dictionary<string, string> linkDefinitions, Dictionary<string, MemberInfo> foundLinks, Action<object, object, string> createLinkAction)
        {
            if (!(control is TextBox textBox))
                return;

            _textBox = textBox;
            _propertyChangedModel = context as INotifyPropertyChanged;

            bool registerNotifyPropertyChanged = false;

            foreach (KeyValuePair<string, MemberInfo> link in foundLinks)
            {
                switch(link.Key)
                {
                    case nameof(TextBox.Text):
                        Binding binding = new Binding(link.Value.Name);
                        binding.Source = context;
                        BindingOperations.SetBinding(textBox, TextBox.TextProperty, binding);
                        break;
                    case nameof(TextBox.IsEnabled):
                        registerNotifyPropertyChanged = true;
                        _isEnabledInfo = link.Value as PropertyInfo;
                        SetIsEnabledState(context);
                        break;
                    default:
                        throw new NotSupportedException($"The link for '{link.Key}' is not supported by '{GetType().Name}'!");
                }
            }

            if (registerNotifyPropertyChanged && _propertyChangedModel != null)
                _propertyChangedModel.PropertyChanged += PropertyChangedModel_PropertyChanged;
        }

        public void Unlink()
        {
            if (_propertyChangedModel != null)
                _propertyChangedModel.PropertyChanged -= PropertyChangedModel_PropertyChanged;
        }

        #endregion

        #region Private Methods

        private void SetIsEnabledState(object context)
        {
            _textBox.IsEnabled = (bool)_isEnabledInfo.GetValue(context);
        }

        #endregion

        #region Event Handling

        private void PropertyChangedModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != _isEnabledInfo.Name)
                return;

            SetIsEnabledState(sender);
        }

        #endregion
    }
}
