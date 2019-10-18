using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Yggdrasil.Exceptions;
using Yggdrasil.Resource;

namespace Yggdrasil.Message
{
    /// <summary>
    /// Provides methods to show messages.
    /// </summary>
    public static class Message
    {
        #region Private Fields

        private static IMessageHandler _handler;
        private static readonly Regex _exceptionInformationReplaceRegex = new Regex("{{(?<PropertyName>\\w*)}}", RegexOptions.Compiled);
        private static Regex _viewNameRegex;
        private static Regex _exceptionNameRegex;
        private static string _messageDefinition;
        private static string _messageTitleDefinition;

        #endregion

        #region Public Methods

        /// <summary>
        /// Registers a implementation of <see cref="IMessageHandler"/>.
        /// </summary>
        /// <param name="handler">The handler for showing messages.</param>
        /// <exception cref="MessageHandlerAlreadyRegisteredException">Thrown if a <see cref="IMessageHandler"/> was already registered.</exception>
        public static void RegisterMessageHandler(IMessageHandler handler)
        {
            if (_handler != null)
                throw new MessageHandlerAlreadyRegisteredException("A message handler was already registered!");

            _handler = handler;
            _handler.RegisterGetViewFunc(ViewManager.GetViewForViewModel);
        }

        /// <summary>
        /// Get's a specific implementation of <see cref="IMessageHandler"/>.
        /// </summary>
        /// <typeparam name="T">Type of the specific implementation of <see cref="IMessageHandler"/>.</typeparam>
        /// <returns>The registered message handler.</returns>
        public static T GetHandler<T>() where T : IMessageHandler
        {
            if (_handler == null)
                throw new MessageHandlerNotRegisterdException("There is no registered message handler!");

            return (T)_handler;
        }

        /// <summary>
        /// Set's the rule definition for showing error messages.
        /// </summary>
        /// <param name="messageDefinition">Definition to get the message resource.</param>
        /// <param name="messageTitleDefinition">Definition to get the title resource.</param>
        /// <param name="viewNameDefinition">Definition of getting the name of the view for the resource key.</param>
        /// <param name="exceptionNameDefinition">Definition of getting the name of the exception for the resource key.</param>
        public static void SetErrorMessageResourceKeyRule(string messageDefinition, string messageTitleDefinition, string viewNameDefinition, string exceptionNameDefinition)
        {
            if (!string.IsNullOrEmpty(_messageDefinition) || !string.IsNullOrEmpty(_messageTitleDefinition) ||
                _viewNameRegex != null || _exceptionNameRegex != null)
                throw new Exception();//todo create a exception

            _viewNameRegex = CreateNameRegex(viewNameDefinition);
            _exceptionNameRegex = CreateNameRegex(exceptionNameDefinition);
            _messageDefinition = messageDefinition;
            _messageTitleDefinition = messageTitleDefinition;
        }

        /// <summary>
        /// Displays a error message for the passed <see cref="Exception"/>.
        /// </summary>
        /// <param name="ex">The <see cref="Exception"/> that was thrown.</param>
        /// <param name="viewModel">The view model which caused the <see cref="Exception"/>.</param>
        /// <param name="callerName">The name of the method which caused the <see cref="Exception"/>.</param>
        public static void DisplayError(Exception ex, object viewModel, [CallerMemberName] string callerName = null)
        {
            if (_handler == null)
                throw new MessageHandlerNotRegisterdException("There is no registered message handler!");

            string viewName = ViewManager.GetViewForViewModel(viewModel).GetType().Name;
            string exName = ex.GetType().Name;

            Match viewNameMatch = _viewNameRegex.Match(viewName);
            Match exNameMatch = _exceptionNameRegex.Match(exName);

            if (viewNameMatch.Success)
                viewName = viewNameMatch.Groups["Name"].Value;

            if (exNameMatch.Success)
                exName = exNameMatch.Groups["Name"].Value;

            string messageKey = GetResourceKey(_messageDefinition, viewName, exName, callerName);
            string messageTitleKey = GetResourceKey(_messageTitleDefinition, viewName, exName, callerName);

            string message = ResourceHandler.GetResource(messageKey);
            string messageTitle = ResourceHandler.GetResource(messageTitleKey);

            _handler.DisplayError(viewModel, ReplaceExceptionInformations(messageTitle, ex), ReplaceExceptionInformations(message, ex));
        }

        #endregion

        #region Private Methods

        private static Regex CreateNameRegex(string definition)
        {
            string pattern = definition.Replace("<Name>", "(?<Name>.*)");

            return new Regex(pattern);
        }

        private static string GetResourceKey(string definition, string viewName, string exceptionName, string callerName)
        {
            return definition.Replace("<ViewName>", viewName ?? string.Empty)
                .Replace("<ExceptionName>", exceptionName ?? string.Empty)
                .Replace("<CallerName>", callerName ?? string.Empty);
        }

        private static string ReplaceExceptionInformations(string text, Exception ex)
        {
            return _exceptionInformationReplaceRegex.Replace(text, m =>
            {
                string propertyName = m.Groups["PropertyName"].Value;
                PropertyInfo pInfo = ex.GetType().GetProperty(propertyName);

                if (pInfo == null)
                    return string.Empty;

                object informationValue = pInfo.GetValue(ex);
                return informationValue?.ToString() ?? string.Empty;
            });
        }

        #endregion
    }
}
