using Yggdrasil.Exceptions;

namespace Yggdrasil.Message
{
    /// <summary>
    /// Provides methods to show messages.
    /// </summary>
    public static class Message
    {
        #region Private Fields

        private static IMessageHandler _handler;

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

        #endregion
    }
}
