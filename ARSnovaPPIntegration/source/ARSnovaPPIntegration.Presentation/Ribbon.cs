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
using ARSnovaPPIntegration.Common.Contract.Exceptions;
using ARSnovaPPIntegration.Presentation.Content;
using ARSnovaPPIntegration.Presentation.Helpers;
using ARSnovaPPIntegration.Presentation.Models;
using ARSnovaPPIntegration.Presentation.ViewPresenter;
using ARSnovaPPIntegration.Presentation.Views;

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
        private readonly ViewPresenter.ViewPresenter viewPresenter;

        private readonly ExceptionHandler exceptionHandler;

        private readonly ILocalizationService localizationService;

        private ISlideManipulator slideManipulator;

        private RibbonHelper ribbonHelper;

        private Office.IRibbonUI ribbon;

        public Ribbon(
            ViewPresenter.ViewPresenter viewPresenter,
            ExceptionHandler exceptionHandler)
        {
            this.localizationService = ServiceLocator.Current.GetInstance<ILocalizationService>();

            this.slideManipulator = ServiceLocator.Current.GetInstance<ISlideManipulator>();

            this.viewPresenter = viewPresenter;

            this.exceptionHandler = exceptionHandler;

            this.ribbonHelper = new RibbonHelper(this.viewPresenter, this.localizationService);
        }

        #region manageQuiz

        public string GetQuizGroupLabel(Office.IRibbonControl control)
        {
            return this.localizationService.Translate("Manage Quiz");
        }

        public string GetAddButtonLabel(Office.IRibbonControl control)
        {
            return this.localizationService.Translate("New question");
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

            var arsnovaSlide = SlideTracker.CurrentSlide;
            // if there is no slide selected, insert new slide at the end of the presentation?
            if (arsnovaSlide == null)
            {
                arsnovaSlide = this.ribbonHelper.CreateNewSlide();
            }

            this.ribbonHelper.StartQuizSetup(arsnovaSlide);

            /*
             * test only -> should be called be helper after successful setup
            // TODO create hashtag first
            try
            {
                
                this.slideManipulator.SetArsnovaClickStyle(arsnovaSlide, "testhashtag");
            }
            catch (CommunicationException exception)
            {
                this.exceptionHandler.Handle(exception.Message, this.localizationService.Translate("Communication Error"));
            }*/
        }

        public string GetArsnovaSlideContextMenuLabel(Office.IRibbonControl control)
        {
            return "ARSnova";
        }

        public Bitmap GetArsnovaFavIcon(Office.IRibbonControl control)
        {
            return Images.ARSnova_Logo;
        }

        public string GetAddQuizToSlideLabel(Office.IRibbonControl control)
        {
            return this.localizationService.Translate("Add question to this slide");
        }

        public void AddQuizToSlideButtonClick(Office.IRibbonControl control)
        {
            var currentSlide = SlideTracker.CurrentSlide;

            // There can't be no selected slide because this event is fired after clicking on a slide
            this.ribbonHelper.StartQuizSetup(currentSlide);
            
        }

        public string GetAddQuizToNewSlideLabel(Office.IRibbonControl control)
        {
            return this.localizationService.Translate("Add question to new slide");
        }

        public void AddQuizToNewSlideButtonClick(Office.IRibbonControl control)
        {
            var newSlide = this.ribbonHelper.CreateNewSlide();
            this.ribbonHelper.StartQuizSetup(newSlide);
        }

        public bool AnySlideSelected(Office.IRibbonControl control)
        {
            var currentSlide = SlideTracker.CurrentSlide;
            // if there is no slide selected, insert new slide at the end of the presentation?
            return currentSlide != null;
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

        public void RibbonLoad(Office.IRibbonUI ribbonUI)
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
