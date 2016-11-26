using System.Globalization;
using System.Windows.Controls;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Office.Core;

using ARSnovaPPIntegration.Presentation.Configuration;
using ARSnovaPPIntegration.Presentation.Helpers;
using ARSnovaPPIntegration.Presentation.Models;
using ARSnovaPPIntegration.Presentation.Views;
using Microsoft.Office.Interop.PowerPoint;

namespace ARSnovaPPIntegration.Presentation
{
    public partial class ThisAddIn
    {
        private ExceptionHandler exceptionHandler;

        private ViewPresenter.ViewPresenter viewPresenter;
        
        private void ThisAddInStartup(object sender, System.EventArgs e)
        {
            // Add new context menu entries
            // Supported from v2000 until v2016 (current): http://officeone.mvps.org/vba/events_version.html
            this.Application.WindowBeforeRightClick +=
                new EApplication_WindowBeforeRightClickEventHandler(this.application_windowBeforeRightClick);

            //var commandBar = this.Application.CommandBars["Text"];
           //ar slideCommandBar = this.Application.CommandBars["Shape"];
            //  var button = commandBar.Context;
        }
        
        /* private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            // Clean up events?
        }*/

        public void application_windowBeforeRightClick(Selection selection, ref bool cancel)
        {
            /*if (selection != null && selection.Type == PpSelectionType.ppSelectionSlides && selection.SlideRange != null)
            {
                
            }*/
        }

        private void Setup()
        {
            // Setup Unity
            var serviceLocator = Bootstrapper.GetUnityServiceLocator();
            ServiceLocator.SetLocatorProvider(() => serviceLocator);

            // Setup ExceptionHandler
            this.exceptionHandler = new ExceptionHandler();

            // Setup Bootstrapper
            //this.ootstrapper.SetCultureInfo();

            // Setup ViewPresenter
            this.viewPresenter = new ViewPresenter.ViewPresenter();
            this.viewPresenter.Add<EditArsnovaVotingViewModel, EditArsnovaVotingView>();
        }

        protected override IRibbonExtensibility CreateRibbonExtensibilityObject()
        {
            // set the cultureinfo according to the running office instance
            var app = this.GetHostItem<Microsoft.Office.Interop.PowerPoint.Application>(
                typeof(Microsoft.Office.Interop.PowerPoint.Application), "Application");
            var languageId = app.LanguageSettings.LanguageID[MsoAppLanguageID.msoLanguageIDUI];
            System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo(languageId);

            // is called on office load (create ribbon bar) -> init here instead of startup because some dependencies are already needed
            this.Setup();

            return new Ribbon(this.viewPresenter, this.exceptionHandler);
        }

        #region Von VSTO generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
       private void InternalStartup()
        {
            // already init on CreateRibbon
            this.Startup += new System.EventHandler(this.ThisAddInStartup);
            //this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }

        #endregion
    }
}
