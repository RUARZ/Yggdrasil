using System;

namespace Yggdrasil
{
    [Serializable]
    public class RootModelAlreadyDisplayedException : Exception
    {
        public RootModelAlreadyDisplayedException() { }
        public RootModelAlreadyDisplayedException(string message) : base(message) { }
        public RootModelAlreadyDisplayedException(string message, Exception inner) : base(message, inner) { }
        protected RootModelAlreadyDisplayedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
