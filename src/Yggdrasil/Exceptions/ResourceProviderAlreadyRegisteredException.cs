using System;

namespace Yggdrasil.Exceptions
{

    [Serializable]
    public class ResourceProviderAlreadyRegisteredException : Exception
    {
        public ResourceProviderAlreadyRegisteredException() { }
        public ResourceProviderAlreadyRegisteredException(string message) : base(message) { }
        public ResourceProviderAlreadyRegisteredException(string message, Exception inner) : base(message, inner) { }
        protected ResourceProviderAlreadyRegisteredException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
