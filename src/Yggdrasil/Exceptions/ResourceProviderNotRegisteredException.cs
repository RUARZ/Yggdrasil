using System;

namespace Yggdrasil.Exceptions
{

    [Serializable]
    public class ResourceProviderNotRegisteredException : Exception
    {
        public ResourceProviderNotRegisteredException() { }
        public ResourceProviderNotRegisteredException(string message) : base(message) { }
        public ResourceProviderNotRegisteredException(string message, Exception inner) : base(message, inner) { }
        protected ResourceProviderNotRegisteredException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
