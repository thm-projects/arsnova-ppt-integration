using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

using Microsoft.Practices.ServiceLocation;

using Office = Microsoft.Office.Core;

using ARSnovaPPIntegration.Business.Contract;
using ARSnovaPPIntegration.Common.Contract;
using ARSnovaPPIntegration.Presentation.Content;
using ARSnovaPPIntegration.Presentation.Helpers;
using Microsoft.Office.Interop.PowerPoint;

namespace ARSnovaPPIntegration.Presentation
{
    [ComVisible(true)]
    public class Ribbon : Office.IRibbonExtensibility
    {
        private readonly ViewPresenter.ViewPresenter viewPresenter;

        private readonly ExceptionHandler exceptionHandler;

        private readonly ILocalizationService localizationService;

        private ISlideManipulator slideManipulator;

        private readonly RibbonHelper ribbonHelper;

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

        public void RefreshRibbonControl(string id)
        {
            this.ribbon.InvalidateControl(id);
        }

        public bool OneArsnovaSlideSelected { get; set; } = false;

        public bool IsOneArsnovaSlideSelected(Office.IRibbonControl control)
        {
            return this.OneArsnovaSlideSelected;
        }

        public bool IsNoneArsnovaSlideSelected(Office.IRibbonControl control)
        {
            return !this.OneArsnovaSlideSelected;
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

            this.ribbonHelper.AddQuizToSlide(arsnovaSlide);

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

        public string GetSessionTypeGroupLabel(Office.IRibbonControl control)
        {
            return "Session";
        }

        public string GetSetSessionTypeLabel(Office.IRibbonControl control)
        {
            return this.localizationService.Translate("Set session type");
        }

        public string GetSetSessionTypeSupertip(Office.IRibbonControl control)
        {
            return
                this.localizationService.Translate(
                    "Decide whether you want your listeners to use the grown up arsnova voting app or the fancy arsnova click one.");
        }

        public void SetSessionTypeButtonClick(Office.IRibbonControl control)
        {
            this.ribbonHelper.ShowSetSessionTypeDialog();
        }

        public Bitmap GetSetSessionTypeButtonImage(Office.IRibbonControl control)
        {
            return Images.ARSnova_Logo;
        }

        public string GetEditButtonLabel(Office.IRibbonControl control)
        {
            return this.localizationService.Translate("Edit");
        }

        public string GetEditButtonSupertip(Office.IRibbonControl control)
        {
            return this.localizationService.Translate("Edit a already existing quiz.");
        }

        public void EditButtonClick(Office.IRibbonControl control)
        {
            this.ribbonHelper.EditQuizSetup(SlideTracker.CurrentSlide);
        }

        public Bitmap GetEditButtonImage(Office.IRibbonControl control)
        {
            return Images.pencil;
        }

        public string GetDeleteButtonLabel(Office.IRibbonControl control)
        {
            return this.localizationService.Translate("Delete");
        }

        public string GetDeleteButtonSupertip(Office.IRibbonControl control)
        {
            return this.localizationService.Translate("Delete a already existing quiz.");
        }

        public void DeleteButtonClick(Office.IRibbonControl control)
        {
            this.ribbonHelper.DeleteQuizFromSelectedSlide(SlideTracker.CurrentSlide);
        }

        public Bitmap GetDeleteButtonImage(Office.IRibbonControl control)
        {
            return Images.multiply;
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
            this.ribbonHelper.AddQuizToSlide(currentSlide);
            
        }

        public string GetAddQuizToNewSlideLabel(Office.IRibbonControl control)
        {
            return this.localizationService.Translate("Add question to new slide");
        }

        public void AddQuizToNewSlideButtonClick(Office.IRibbonControl control)
        {
            var newSlide = this.ribbonHelper.CreateNewSlide();
            this.ribbonHelper.AddQuizToSlide(newSlide);
        }

        public bool AnySlideSelected(Office.IRibbonControl control)
        {
            var currentSlide = SlideTracker.CurrentSlide;
            // if there is no slide selected, insert new slide at the end of the presentation?
            return currentSlide != null;
        }

        public string GetStartQuizLabel(Office.IRibbonControl control)
        {
            return this.localizationService.Translate("Start quiz");
        }

        public Bitmap GetStartButtonImage(Office.IRibbonControl control)
        {
            return Images.right;
        }

        public bool PresentationOnArsnovaSlide(Office.IRibbonControl control)
        {
            var currentShowedSlidePosition = SlideTracker.CurrentShowedPresentationSlidePosition;

            var slideSessionModel = PresentationInformationStore.GetStoredSlideSessionModel();

            foreach (var slideQuestionModel in slideSessionModel.Questions)
            {
                if (SlideTracker.GetSlideById(slideQuestionModel.SlideId).SlideNumber == currentShowedSlidePosition)
                    return true;
            }

            return false;
        }

        public void StartNextQuestion(Office.IRibbonControl control)
        {
            var currentShowedSlidePosition = SlideTracker.CurrentShowedPresentationSlidePosition;

            var slideSessionModel = PresentationInformationStore.GetStoredSlideSessionModel();

            foreach (var slideQuestionModel in slideSessionModel.Questions)
            {
                var slide = SlideTracker.GetSlideById(slideQuestionModel.SlideId);
                if (slide.SlideNumber == currentShowedSlidePosition)
                {
                    this.ribbonHelper.StartQuiz(slideQuestionModel);
                }
            }
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
            var asm = Assembly.GetExecutingAssembly();
            var resourceNames = asm.GetManifestResourceNames();
            foreach (string t in resourceNames)
            {
                if (string.Compare(resourceName, t, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    using (var resourceReader = new StreamReader(asm.GetManifestResourceStream(t)))
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
