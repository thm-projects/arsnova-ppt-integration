using System.Windows;
using System.Windows.Controls;

namespace ARSnovaPPIntegration.Presentation.Window
{
    public static class PopUpWindow
    {
        public static bool ConfirmationWindow(string title, string text)
        {
            var messageBoxResult = MessageBox.Show(text, title, MessageBoxButton.YesNoCancel);

            return messageBoxResult == MessageBoxResult.Yes;
        }
    }
}
