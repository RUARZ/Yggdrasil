using System;

namespace Yggdrasil.Exceptions
{

    [Serializable]
    public class MessageHandlerAlreadyRegisteredException : Exception
    {
        public MessageHandlerAlreadyRegisteredException() { }
        public MessageHandlerAlreadyRegisteredException(string message) : base(message) { }
        public MessageHandlerAlreadyRegisteredException(string message, Exception inner) : base(message, inner) { }
        protected MessageHandlerAlreadyRegisteredException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
