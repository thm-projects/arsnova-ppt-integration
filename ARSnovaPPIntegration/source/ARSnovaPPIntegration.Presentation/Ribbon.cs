using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Office.Interop.PowerPoint;
using Microsoft.Practices.ServiceLocation;
using Office = Microsoft.Office.Core;

using ARSnovaPPIntegration.Business.Contract;
using ARSnovaPPIntegration.Common.Contract;
using ARSnovaPPIntegration.Presentation.Content;
using ARSnovaPPIntegration.Presentation.Helpers;

// TODO:  Führen Sie diese Schritte aus, um das Element auf dem Menüband (XML) zu aktivieren:

// 1: Kopieren Sie folgenden Codeblock in die ThisAddin-, ThisWorkbook- oder ThisDocument-Klasse.

//  protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
//  {
//      return new Ribbon();
//  }

// 2. Erstellen Sie Rückrufmethoden im Abschnitt "Menübandrückrufe" dieser Klasse, um Benutzeraktionen
//    zu behandeln, z.B. das Klicken auf eine Schaltfläche. Hinweis: Wenn Sie dieses Menüband aus dem Menüband-Designer exportiert haben,
//    verschieben Sie den Code aus den Ereignishandlern in die Rückrufmethoden, und ändern Sie den Code für die Verwendung mit dem
//    Programmmodell für die Menübanderweiterung (RibbonX).

// 3. Weisen Sie den Steuerelementtags in der Menüband-XML-Datei Attribute zu, um die entsprechenden Rückrufmethoden im Code anzugeben.  

// Weitere Informationen erhalten Sie in der Menüband-XML-Dokumentation in der Hilfe zu Visual Studio-Tools für Office.


namespace ARSnovaPPIntegration.Presentation
{
    [ComVisible(true)]
    public class Ribbon : Office.IRibbonExtensibility
    {
        private readonly ILocalizationService localizationService;

        private ISlideManipulator slideManipulator;

        private Office.IRibbonUI ribbon;

        public Ribbon()
        {
            this.localizationService = ServiceLocator.Current.GetInstance<ILocalizationService>();

            this.slideManipulator = ServiceLocator.Current.GetInstance<ISlideManipulator>();
        }

        #region manageQuiz

        public string GetQuizGroupLabel(Office.IRibbonControl control)
        {
            return this.localizationService.Translate("Manage Quiz");
        }

        public string GetAddButtonLabel(Office.IRibbonControl control)
        {
            return this.localizationService.Translate("Add");
        }

        public string GetAddButtonSupertip(Office.IRibbonControl control)
        {
            return this.localizationService.Translate("Add");
        }

        public Bitmap GetAddButtonImage(Office.IRibbonControl control)
        {
            return Images.add;
        }

        public void AddButtonClick(Office.IRibbonControl control)
        {
            // Just a test: add header to current slide
            /*var currentSlide = SlideTracker.CurrentSlide;
            var slideManipulator = new SlideManipulator(currentSlide);
            slideManipulator.AddFooter();*/

            var currentSlide = SlideTracker.CurrentSlide;
            if (currentSlide == null)
            {
                System.Windows.Forms.MessageBox.Show(this.localizationService.Translate("Please select a slide"), this.localizationService.Translate("Unable to add new slide"));
            }

            var newArsnovaSlide = Globals.ThisAddIn.Application.ActivePresentation.Slides.Add
                (currentSlide.SlideIndex + 1, PpSlideLayout.ppLayoutTitle);

            var slideHeight = Globals.ThisAddIn.Application.ActivePresentation.SlideMaster.Height;
            var slideWidth = Globals.ThisAddIn.Application.ActivePresentation.SlideMaster.Width;

            // this works but for all slides! (master)
            /*Globals.ThisAddIn.Application.ActivePresentation.SlideMaster.Shapes.AddPicture(
    @"C:\fox.jpg",
    Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoTrue, 0, 0, slideWidth, slideHeight);*/
            
            
            //this.slideManipulator.SetArsnovaClickStyle(newArsnovaSlide);
            newArsnovaSlide.FollowMasterBackground = Office.MsoTriState.msoFalse;
            newArsnovaSlide.Background.Fill.UserPicture(@"C:\fox.jpg");
        }

        #endregion

        #region infoGroup

        public string GetInfoGroupLabel(Office.IRibbonControl control)
        {
            return this.localizationService.Translate("Info");
        }

        // HelpButton

        public string GetHelpButtonLabel(Office.IRibbonControl control)
        {
            return this.localizationService.Translate("Help");
        }

        public string GetHelpButtonSupertip(Office.IRibbonControl control)
        {
            return this.localizationService.Translate("Help");
        }

        public Bitmap GetHelpButtonImage(Office.IRibbonControl control)
        {
            return Images.information;
        }

        public void HelpButtonClick(Office.IRibbonControl control)
        {
            throw new NotImplementedException();
        }

        // AboutButton

        public string GetAboutButtonLabel(Office.IRibbonControl control)
        {
            return this.localizationService.Translate("About");
        }

        public string GetAboutButtonSupertip(Office.IRibbonControl control)
        {
            return this.localizationService.Translate("About");
        }

        public Bitmap GetAboutButtonImage(Office.IRibbonControl control)
        {
            return Images.ARSnova_Logo;
        }

        public void AboutButtonClick(Office.IRibbonControl control)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IRibbonExtensibility-Member

        public string GetCustomUI(string ribbonId)
        {
            return GetResourceText("ARSnovaPPIntegration.Presentation.Ribbon.xml");
        }

        #endregion

        #region Menübandrückrufe
        //Erstellen Sie hier Rückrufmethoden. Weitere Informationen zum Hinzufügen von Rückrufmethoden finden Sie unter "http://go.microsoft.com/fwlink/?LinkID=271226".

        public void Ribbon_Load(Office.IRibbonUI ribbonUI)
        {
            this.ribbon = ribbonUI;
        }

        #endregion

        #region Hilfsprogramme

        private static string GetResourceText(string resourceName)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            string[] resourceNames = asm.GetManifestResourceNames();
            for (int i = 0; i < resourceNames.Length; ++i)
            {
                if (string.Compare(resourceName, resourceNames[i], StringComparison.OrdinalIgnoreCase) == 0)
                {
                    using (StreamReader resourceReader = new StreamReader(asm.GetManifestResourceStream(resourceNames[i])))
                    {
                        if (resourceReader != null)
                        {
                            return resourceReader.ReadToEnd();
                        }
                    }
                }
            }
            return null;
        }

        #endregion
    }
}
