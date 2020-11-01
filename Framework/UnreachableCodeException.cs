using System;
using System.Runtime.Serialization;

namespace Azoth.Tools.Bootstrap.Framework
{
    [Serializable]
    public class UnreachableCodeException : Exception
    {
        public UnreachableCodeException()
        {
        }

        public UnreachableCodeException(string message)
            : base(message)
        {

        }

        protected UnreachableCodeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
