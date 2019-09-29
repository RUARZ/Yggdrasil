using System;
using System.Windows;

namespace Yggdrasil.Wpf
{
    /// <summary>
    /// Implementation for the <see cref="IWpfMessageHandler"/> for showing messages.
    /// </summary>
    public class WpfMessageHandler : IWpfMessageHandler
    {
        #region Private Fields

        private Func<object, object> _getViewFunc;

        #endregion

        #region Private Methods

        private MessageBoxImage GetMessageBoxImage(MessageImage image)
        {
            switch (image)
            {
                case MessageImage.Information:
                    return MessageBoxImage.Information;
                case MessageImage.Error:
                    return MessageBoxImage.Error;
                case MessageImage.Question:
                    return MessageBoxImage.Question;
                case MessageImage.Warning:
                    return MessageBoxImage.Warning;
                default:
                    throw new NotSupportedException($"The {image} is not supported in method '{nameof(GetMessageBoxImage)}'!");
            }
        }

        private MessageBoxButton GetMessageBoxButton(MessageOptions options)
        {
            switch (options)
            {
                case MessageOptions.Ok:
                    return MessageBoxButton.OK;
                case MessageOptions.OkCancel:
                    return MessageBoxButton.OKCancel;
                case MessageOptions.YesNo:
                    return MessageBoxButton.YesNo;
                case MessageOptions.YesNoCancel:
                    return MessageBoxButton.YesNoCancel;
                default:
                    throw new NotSupportedException($"The {options} are not supported in method '{nameof(GetMessageBoxButton)}'!");
            }
        }

        private MessageResult GetMessageResult(MessageBoxResult result)
        {
            switch(result)
            {
                case MessageBoxResult.Yes:
                    return MessageResult.Yes;
                case MessageBoxResult.No:
                    return MessageResult.No;
                case MessageBoxResult.Cancel:
                    return MessageResult.Cancel;
                case MessageBoxResult.OK:
                    return MessageResult.Cancel;
                case MessageBoxResult.None:
                    return MessageResult.None;
                default:
                    throw new NotSupportedException($"The {result} is not supported in method '{nameof(GetMessageResult)}'!");
            }
        }

        private MessageResult ShowMessage(object ownerViewModel, string title, string message, MessageOptions messageOptions, MessageImage messageImage)
        {
            Window ownerView = null;

            if (ownerViewModel != null)
                ownerView = _getViewFunc(ownerViewModel) as Window;

            if (ownerView != null)
                return GetMessageResult(MessageBox.Show(ownerView, message, title, GetMessageBoxButton(messageOptions), GetMessageBoxImage(messageImage)));
            
            return GetMessageResult(MessageBox.Show(message, title, GetMessageBoxButton(messageOptions), GetMessageBoxImage(messageImage)));
        }

        #endregion

        #region Interface Implementations

        public void RegisterGetViewFunc(Func<object, object> getViewFunc)
        {
            _getViewFunc = getViewFunc;
        }

        #region Show Error

        public void ShowError(string title, string message)
        {
            ShowError(null, title, message);
        }

        public void ShowError(object owner, string title, string message)
        {
            ShowMessage(owner, title, message, MessageOptions.Ok, MessageImage.Error);
        }

        #endregion

        #region Show Information

        public void ShowInformation(string title, string message)
        {
            ShowInformation(null, title, message);
        }

        public void ShowInformation(object owner, string title, string message)
        {
            ShowMessage(owner, title, message, MessageOptions.Ok, MessageImage.Information);
        }

        #endregion

        #region Show Question

        public MessageResult ShowQuestion(string title, string message)
        {
            return ShowQuestion(null, title, message, MessageOptions.OkCancel);
        }

        public MessageResult ShowQuestion(object owner, string title, string message)
        {
            return ShowQuestion(owner, title, message, MessageOptions.OkCancel);
        }

        public MessageResult ShowQuestion(string title, string message, MessageOptions messageOptions)
        {
            return ShowQuestion(null, title, message, messageOptions);
        }

        public MessageResult ShowQuestion(object owner, string title, string message, MessageOptions messageOptions)
        {
            return ShowMessage(owner, title, message, messageOptions, MessageImage.Question);
        }

        #endregion

        #region Show Warning

        public MessageResult ShowWarning(string title, string message)
        {
            return ShowWarning(null, title, message, MessageOptions.OkCancel);
        }

        public MessageResult ShowWarning(object owner, string title, string message)
        {
            return ShowWarning(owner, title, message, MessageOptions.OkCancel);
        }

        public MessageResult ShowWarning(string title, string message, MessageOptions messageOptions)
        {
            return ShowWarning(null, title, message, messageOptions);
        }

        public MessageResult ShowWarning(object owner, string title, string message, MessageOptions messageOptions)
        {
            return ShowMessage(owner, title, message, messageOptions, MessageImage.Warning);
        }

        #endregion

        #endregion
    }
}
