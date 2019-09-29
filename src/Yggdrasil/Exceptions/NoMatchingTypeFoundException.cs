using System;

namespace Yggdrasil.Exceptions
{

    [Serializable]
    public class NoMatchingTypeFoundException : Exception
    {
        public NoMatchingTypeFoundException() { }
        public NoMatchingTypeFoundException(string message) : base(message) { }
        public NoMatchingTypeFoundException(string message, Exception inner) : base(message, inner) { }
        protected NoMatchingTypeFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
