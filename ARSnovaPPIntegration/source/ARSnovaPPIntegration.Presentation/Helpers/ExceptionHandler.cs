using System;
using System.Diagnostics;
using System.Windows;

using ARSnovaPPIntegration.Common.Contract;
using Microsoft.Practices.ServiceLocation;
using MessageBox = System.Windows.MessageBox;

namespace ARSnovaPPIntegration.Presentation.Helpers
{
    public class ExceptionHandler
    {
        private readonly ILocalizationService localizationService;

        public ExceptionHandler()
        {
            this.localizationService = ServiceLocator.Current.GetInstance<ILocalizationService>();
        }
        // Automatic-catching won't work as expected...
        /*public void Setup()
        {
            // This one should handle every exception that occures in our app
            // Note: Exceptions occuring on ms office calls can't be handled because powerpoint doesn't know our add-in

            System.Windows.Forms.Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            AppDomain.CurrentDomain.UnhandledException -= this.CurrentDomainOnUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += this.CurrentDomainOnUnhandledException;

            System.Windows.Forms.Application.ThreadException -= this.AppOnThreadException;
            System.Windows.Forms.Application.ThreadException += this.AppOnThreadException;
        }*/

        public void Handle(string message, string caption = "Error")
        {
            this.ShowMessageBox(message, caption);
            
            #if DEBUG
                Debugger.Break();
            #endif
        }

        private void ShowMessageBox(string message, string caption)
        {
            var text = this.localizationService.Translate("An error occured:");
            MessageBox.Show(text + Environment.NewLine + message, caption, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /*private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            this.Handle(((Exception)unhandledExceptionEventArgs.ExceptionObject).Message);
        }

        private void AppOnThreadException(object sender, ThreadExceptionEventArgs threadExceptionEventArgs)
        {
            this.Handle(threadExceptionEventArgs.Exception.Message);
        }*/
    }
}
