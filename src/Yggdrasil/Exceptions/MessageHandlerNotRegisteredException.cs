using System;

namespace Yggdrasil.Exceptions
{

    [Serializable]
    public class MessageHandlerNotRegisterdException : Exception
    {
        public MessageHandlerNotRegisterdException() { }
        public MessageHandlerNotRegisterdException(string message) : base(message) { }
        public MessageHandlerNotRegisterdException(string message, Exception inner) : base(message, inner) { }
        protected MessageHandlerNotRegisterdException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
