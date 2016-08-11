using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ARSnovaPPIntegration.Common.Contract
{
    public interface ILocalizationService
    {
        /// <summary>
        /// Translates text based on the current culture.
        /// </summary>
        /// <param name="text">The text for translation.</param>
        /// <returns></returns>
        string Translate(string text);
    }
}
