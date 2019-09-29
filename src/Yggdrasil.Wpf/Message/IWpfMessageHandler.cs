using Yggdrasil.Message;

namespace Yggdrasil.Wpf
{
    /// <summary>
    /// Specific wpf interface of <see cref="IMessageHandler"/>.
    /// </summary>
    public interface IWpfMessageHandler : IMessageHandler
    {
        #region Show Information Messages

        /// <summary>
        /// Shows a information message.
        /// </summary>
        /// <param name="title">The title for the message.</param>
        /// <param name="message">The text for the message.</param>
        void ShowInformation(string title, string message);

        /// <summary>
        /// Shows a information message.
        /// </summary>
        /// <param name="owner">The owner view model for the message.</param>
        /// <param name="title">The title for the message.</param>
        /// <param name="message">The text for the message.</param>
        void ShowInformation(object owner, string title, string message);

        #endregion

        #region Show Question Messages

        /// <summary>
        /// Shows a question message and returns the answer of the dialog. OK/Cancel options will be used for the question.
        /// </summary>
        /// <param name="title">The title for the message.</param>
        /// <param name="message">The text for the message.</param>
        /// <returns>The result of the message.</returns>
        MessageResult ShowQuestion(string title, string message);

        /// <summary>
        /// Shows a question message and returns the answer of the dialog. OK/Cancel options will be used for the question.
        /// </summary>
        /// <param name="owner">The owner view model for the message.</param>
        /// <param name="title">The title for the message.</param>
        /// <param name="message">The text for the message.</param>
        /// <returns>The result of the message.</returns>
        MessageResult ShowQuestion(object owner, string title, string message);

        /// <summary>
        /// Shows a question message and returns the answer of the dialog. Set's the buttons to <paramref name="messageOptions"/>.
        /// </summary>
        /// <param name="title">The title for the message.</param>
        /// <param name="message">The text for the message.</param>
        /// <param name="messageOptions">Definition of the options which should be displayed.</param>
        /// <returns>The result of the message.</returns>
        MessageResult ShowQuestion(string title, string message, MessageOptions messageOptions);

        /// <summary>
        /// Shows a question message and returns the answer of the dialog. Set's the buttons to <paramref name="messageOptions"/>.
        /// </summary>
        /// <param name="owner">The owner view model for the message.</param>
        /// <param name="title">The title for the message.</param>
        /// <param name="message">The text for the message.</param>
        /// <param name="messageOptions">Definition of the options which should be displayed.</param>
        /// <returns>The result of the message.</returns>
        MessageResult ShowQuestion(object owner, string title, string message, MessageOptions messageOptions);

        #endregion

        #region Show Warning Messages

        /// <summary>
        /// Shows a warning message and returns the answer of the dialog. OK/Cancel options will be used for the question.
        /// </summary>
        /// <param name="title">The title for the message.</param>
        /// <param name="message">The text for the message.</param>
        /// <returns>The result of the message.</returns>
        MessageResult ShowWarning(string title, string message);

        /// <summary>
        /// Shows a warning message and returns the answer of the dialog. OK/Cancel options will be used for the question.
        /// </summary>
        /// <param name="owner">The owner view model for the message.</param>
        /// <param name="title">The title for the message.</param>
        /// <param name="message">The text for the message.</param>
        /// <returns>The result of the message.</returns>
        MessageResult ShowWarning(object owner, string title, string message);

        /// <summary>
        /// Shows a warning message and returns the answer of the dialog. Set's the buttons to <paramref name="messageOptions"/>.
        /// </summary>
        /// <param name="title">The title for the message.</param>
        /// <param name="message">The text for the message.</param>
        /// <param name="messageOptions">Definition of the options which should be displayed.</param>
        /// <returns>The result of the message.</returns>
        MessageResult ShowWarning(string title, string message, MessageOptions messageOptions);

        /// <summary>
        /// Shows a warning message and returns the answer of the dialog. Set's the buttons to <paramref name="messageOptions"/>.
        /// </summary
        /// <param name="owner">The owner view model for the message.</param>
        /// <param name="title">The title for the message.</param>
        /// <param name="message">The text for the message.</param>
        /// <param name="messageOptions">Definition of the options which should be displayed.</param>
        /// <returns>The result of the message.</returns>
        MessageResult ShowWarning(object owner, string title, string message, MessageOptions messageOptions);

        #endregion

        #region Show Error Messages

        /// <summary>
        /// Shows a error message.
        /// </summary>
        /// <param name="title">The title for the message.</param>
        /// <param name="message">The text for the message.</param>
        void ShowError(string title, string message);

        /// <summary>
        /// Shows a error message.
        /// </summary>
        /// <param name="owner">The owner view model for the message.</param>
        /// <param name="title">The title for the message.</param>
        /// <param name="message">The text for the message.</param>
        void ShowError(object owner, string title, string message);

        #endregion
    }
}
