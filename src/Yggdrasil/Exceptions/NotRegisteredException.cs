using System;

namespace Yggdrasil.Exceptions
{
    [Serializable]
    public class NotRegisteredException : Exception
    {
        public NotRegisteredException() { }
        public NotRegisteredException(string message) : base(message) { }
        public NotRegisteredException(string message, Exception inner) : base(message, inner) { }
        protected NotRegisteredException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
