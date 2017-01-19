using System.Windows;

using Microsoft.Practices.ServiceLocation;

using ARSnovaPPIntegration.Common.Contract;

namespace ARSnovaPPIntegration.Presentation.Window
{
    public static class PopUpWindow
    {
        public static void InformationWindow(string title, string text)
        {
            MessageBox.Show(text, title, MessageBoxButton.OK);
        }

        public static bool ConfirmationWindow(string title, string text)
        {
            var messageBoxResult = MessageBox.Show(text, title, MessageBoxButton.YesNoCancel);

            return messageBoxResult == MessageBoxResult.Yes;
        }

        public static void ErrorWindow(string title, string text)
        {
            MessageBox.Show(text, title, MessageBoxButton.OK);
        }

        /// <summary>
        /// Prompts a confirmation window whether the user wants to close this window or not. Returns answer as bool (true for yes, false for no or cancel)
        /// </summary>
        /// <returns></returns>
        public static bool CloseWindowPrompt()
        {
            var localizationService = ServiceLocator.Current.GetInstance<ILocalizationService>();

            return ConfirmationWindow(
                localizationService.Translate("Cancel"),
                localizationService.Translate(
                    "If this process is canceld, every progress will be deleted. Do you like to continue?"));
        }
    }
}
