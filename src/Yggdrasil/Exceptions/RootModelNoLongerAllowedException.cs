using System;

namespace Yggdrasil
{

    [Serializable]
    public class RootModelNoLongerAllowedException : Exception
    {
        public RootModelNoLongerAllowedException() { }
        public RootModelNoLongerAllowedException(string message) : base(message) { }
        public RootModelNoLongerAllowedException(string message, Exception inner) : base(message, inner) { }
        protected RootModelNoLongerAllowedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
