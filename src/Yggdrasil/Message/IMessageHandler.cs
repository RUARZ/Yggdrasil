using System;

namespace Yggdrasil.Message
{
    /// <summary>
    /// Provides methods for displaying messages. The supported methods of showing a message will be declared
    /// in the specific implementations.
    /// </summary>
    public interface IMessageHandler
    {
        /// <summary>
        /// Registers a func for returning the view instance belonging to a view model. Useable for e.g. getting view to set a owner for messages.
        /// </summary>
        /// <param name="getViewFunc">The <see cref="Func{T, TResult}"/> for getting the view belonging to a view model.</param>
        void RegisterGetViewFunc(Func<object, object> getViewFunc);
    }
}
