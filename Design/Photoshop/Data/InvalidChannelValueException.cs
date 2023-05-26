using System;
using System.Runtime.Serialization;

namespace MyPhotoshop
{
    [Serializable]
    public class InvalidChannelValueException : ApplicationException
    {
        public InvalidChannelValueException() { }

        public InvalidChannelValueException(string message) 
            : base(message) { }

        public InvalidChannelValueException(string message, Exception inner) 
            : base(message, inner) { }

        public InvalidChannelValueException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
