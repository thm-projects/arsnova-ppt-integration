using System;
using System.Collections.Generic;
using System.Linq;
using ARSnovaPPIntegration.Common.Contract;

using System.Resources;
using System.Text.RegularExpressions;
using System.Threading;

namespace ARSnovaPPIntegration.Common
{
    public class LocalizationService : ILocalizationService
    {
        /// <summary>
        /// Regex for all upper case letters
        /// </summary>
        private static readonly Regex upperCaseReplace = new Regex(@"([A-ZÄÖÜ])", RegexOptions.Compiled);

        /// <summary>
        /// XML-Replacement keys
        /// </summary>
        private readonly Dictionary<string, string> xmlSpecialCharReplacements = new Dictionary<string, string>
        {
            { "’", "´" },
            { "–", "-" },
            { " ", "_" }
        };
        
        /// <summary>
        /// The ressource manager for texts
        /// </summary>
        private readonly ResourceManager translations;
        
        private readonly Type translationResource = typeof(Resources.Translations);

        public LocalizationService()
        {
            this.translations = new ResourceManager(this.translationResource);
        }

        /// <summary>
        /// Translates text based on the current culture
        /// </summary>
        /// <param name="text">The text for translation.</param>
        /// <returns></returns>
        public string Translate(string text)
        {
            string escapedString = this.ConvertAdditionalCharsToXml(text);

            escapedString = upperCaseReplace.Replace(escapedString, "__$1__");

            string translationString = this.translations.GetString(escapedString, Thread.CurrentThread.CurrentCulture);

            return string.IsNullOrEmpty(translationString) ? text : translationString;
        }

        /// <summary>
        /// Converts special characters to their XML-entity
        /// </summary>
        /// <param name="text">Input text</param>
        /// <returns>Text with replaced special chars</returns>
        protected string ConvertAdditionalCharsToXml(string text)
        {
            return this.xmlSpecialCharReplacements.Aggregate(text, (current, replacement) => current.Replace(replacement.Key, replacement.Value));
        }
    }
}
