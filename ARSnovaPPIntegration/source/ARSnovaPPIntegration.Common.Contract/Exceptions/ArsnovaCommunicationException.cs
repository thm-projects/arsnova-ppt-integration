using System;

namespace ARSnovaPPIntegration.Common.Contract.Exceptions
{
    public class ArsnovaCommunicationException : Exception
    {
        public ArsnovaCommunicationException() { }

        public ArsnovaCommunicationException(string message) : base(message){ }

        public ArsnovaCommunicationException(string message, Exception innerException) : base(message, innerException){ }
    }
}
