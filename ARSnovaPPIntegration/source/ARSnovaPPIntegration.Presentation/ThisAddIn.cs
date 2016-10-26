using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Microsoft.Practices.ServiceLocation;

using ARSnovaPPIntegration.Presentation.Configuration;

namespace ARSnovaPPIntegration.Presentation
{
    public partial class ThisAddIn
    {
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            Bootstrapper.SetCultureInfo();
        }

        /*private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
        }*/

        protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
        {
            var serviceLocator = Bootstrapper.GetUnityServiceLocator();

            ServiceLocator.SetLocatorProvider(() => serviceLocator);

            return new Ribbon();
        }

        #region Von VSTO generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            //this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }

        #endregion
    }
}
