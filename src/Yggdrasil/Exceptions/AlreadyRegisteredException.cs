using System;
using System.Runtime.Serialization;

namespace Yggdrasil.Exceptions
{

    [Serializable]
    public class AlreadyRegisteredException : Exception
    {
        public AlreadyRegisteredException()
        {
        }

        public AlreadyRegisteredException(string message) : base(message)
        {
        }

        public AlreadyRegisteredException(string message, Exception inner) : base(message, inner)
        {
        }

        protected AlreadyRegisteredException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
