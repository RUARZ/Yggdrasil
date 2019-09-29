using System;

namespace Yggdrasil
{

    [Serializable]
    public class ModelNotFoundException : Exception
    {
        public ModelNotFoundException() { }
        public ModelNotFoundException(string message) : base(message) { }
        public ModelNotFoundException(string message, Exception inner) : base(message, inner) { }
        protected ModelNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
