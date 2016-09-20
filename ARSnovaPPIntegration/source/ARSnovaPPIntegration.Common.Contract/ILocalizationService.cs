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
