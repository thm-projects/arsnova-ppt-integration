using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Microsoft.Practices.ServiceLocation;
using Microsoft.Office.Core;

using ARSnovaPPIntegration.Presentation.Configuration;
using ARSnovaPPIntegration.Presentation.Helpers;

namespace ARSnovaPPIntegration.Presentation
{
    public partial class ThisAddIn
    {
        public ExceptionHandler ExceptionHandler;

        // TODO ViewPresenter
        // private readonly ViewPresenter viewPresenter;

        /*private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {

        }*/
        private void Setup()
        {
            // Setup ExceptionHandler
            this.ExceptionHandler = new ExceptionHandler();

            // Setup Bootstrapper
            //this.ootstrapper.SetCultureInfo();

            // Setup Unity
            var serviceLocator = Bootstrapper.GetUnityServiceLocator();
            ServiceLocator.SetLocatorProvider(() => serviceLocator);

            // Setup ViewPresenter
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

            return new Ribbon();
        }

        #region Von VSTO generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
       private void InternalStartup()
        {
            // already init on CreateRibbon
            //this.Startup += new System.EventHandler(this.ThisAddIn_Startup);
            //this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }

        #endregion
    }
}
