using System;
using System.Collections.Generic;
using System.Text;

namespace Bonavena.Exceptions
{
    [Serializable]
    public class ClosedUnitOfWorkException : Exception
    {
        public ClosedUnitOfWorkException() { }
        public ClosedUnitOfWorkException(string message) : base(message) { }
        public ClosedUnitOfWorkException(string message, Exception inner) : base(message, inner) { }
        protected ClosedUnitOfWorkException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
