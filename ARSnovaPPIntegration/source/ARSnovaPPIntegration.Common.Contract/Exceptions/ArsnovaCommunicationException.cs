using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARSnovaPPIntegration.Common.Contract.Exceptions
{
    public class ArsnovaCommunicationException : Exception
    {
        public ArsnovaCommunicationException() { }

        public ArsnovaCommunicationException(string message) : base(message){ }

        public ArsnovaCommunicationException(string message, Exception innerException) : base(message, innerException){ }
    }
}
