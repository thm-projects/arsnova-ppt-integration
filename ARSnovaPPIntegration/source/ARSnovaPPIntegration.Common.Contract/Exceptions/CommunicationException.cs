using System;
using System.Net;

namespace ARSnovaPPIntegration.Common.Contract.Exceptions
{
    public class CommunicationException : Exception
    {
        public CommunicationException(string message) : base(message) { }

        public CommunicationException(string message, Exception innerException) : base(message, innerException){ }

        public CommunicationException(string message, HttpStatusCode httpStatusCode, string serverResponseString = null) : base(message)
        {
            this.HttpStatusCode = httpStatusCode;
            this.ServerResponseString = serverResponseString;
        }

        public HttpStatusCode? HttpStatusCode { get; set; }

        public bool WithHttpStatusCode => this.HttpStatusCode.HasValue;

        public string ServerResponseString { get; set; }

        public bool WithServerResponseString => !string.IsNullOrEmpty(this.ServerResponseString);
    }
}
