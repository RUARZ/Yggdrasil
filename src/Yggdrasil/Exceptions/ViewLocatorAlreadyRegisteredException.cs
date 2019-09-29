using System;
using System.Runtime.Serialization;

namespace Yggdrasil
{

    [Serializable]
    public class ViewLocatorAlreadyRegisteredException : Exception
    {
        public ViewLocatorAlreadyRegisteredException()
        {
        }

        public ViewLocatorAlreadyRegisteredException(string message) : base(message)
        {
        }

        public ViewLocatorAlreadyRegisteredException(string message, Exception inner) : base(message, inner)
        {
        }

        protected ViewLocatorAlreadyRegisteredException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
