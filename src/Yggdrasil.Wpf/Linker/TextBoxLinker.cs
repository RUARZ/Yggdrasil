using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Data;
using Yggdrasil.Wpf.Helper;

namespace Yggdrasil.Wpf.Linker
{
    [RegisterLinker(typeof(TextBox))]
    class TextBoxLinker : ILinker
    {
        #region Private Fields

        private TextBox _textBox;
        private readonly PropertyChangedHandler _propertyChangedHandler = new PropertyChangedHandler();

        #endregion

        #region Interface Impelementation

        public void Link(object viewElement, IEnumerable<LinkData> linkData, Action<object, object, string> createLinkAction)
        {
            if (!(viewElement is TextBox textBox))
                return;

            _textBox = textBox;

            foreach (LinkData data in linkData)
            {
                switch (data.ViewElementName)
                {
                    case nameof(TextBox.Text):
                        Binding binding = new Binding(data.ContextMemberInfo.Name);
                        binding.Source = data.Context;
                        BindingOperations.SetBinding(textBox, TextBox.TextProperty, binding);
                        break;
                    case nameof(TextBox.IsEnabled):
                        PropertyInfo pInfo = data.ContextMemberInfo as PropertyInfo;
                        SetIsEnabledState(data.Context, pInfo);

                        if(data.Context is INotifyPropertyChanged propertyChangedContext)
                        {
                            _propertyChangedHandler.AddNotifyPropertyChangedItem(propertyChangedContext, data.ContextMemberInfo.Name,
                                () => SetIsEnabledState(data.Context, pInfo));
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

        private void SetIsEnabledState(object context, PropertyInfo pInfo)
        {
            _textBox.IsEnabled = (bool)pInfo.GetValue(context);
        }

        #endregion
    }
}
