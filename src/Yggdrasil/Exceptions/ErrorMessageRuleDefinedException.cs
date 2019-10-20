using System;

namespace Yggdrasil.Exceptions
{

    [System.Serializable]
    public class ErrorMessageRuleDefinedException : Exception
    {
        public ErrorMessageRuleDefinedException() { }
        public ErrorMessageRuleDefinedException(string message) : base(message) { }
        public ErrorMessageRuleDefinedException(string message, System.Exception inner) : base(message, inner) { }
        protected ErrorMessageRuleDefinedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
