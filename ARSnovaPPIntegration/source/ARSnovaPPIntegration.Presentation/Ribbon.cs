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
using ARSnovaPPIntegration.Presentation.ViewManagement;
using ARSnovaPPIntegration.Presentation.Window;

namespace ARSnovaPPIntegration.Presentation
{
    [ComVisible(true)]
    public class Ribbon : Office.IRibbonExtensibility
    {
        private readonly IViewPresenter viewPresenter;

        private readonly ILocalizationService localizationService;

        private ISlideManipulator slideManipulator;

        private readonly RibbonHelper ribbonHelper;

        private Office.IRibbonUI ribbon;

        public Ribbon(
            IViewPresenter viewPresenter,
            RibbonHelper ribbonHelper)
        {
            this.localizationService = ServiceLocator.Current.GetInstance<ILocalizationService>();

            this.slideManipulator = ServiceLocator.Current.GetInstance<ISlideManipulator>();

            this.viewPresenter = viewPresenter;

            this.ribbonHelper = ribbonHelper;
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

        public string GetAddNewSlidesLabel(Office.IRibbonControl control)
        {
            return this.localizationService.Translate("Add multiple slides with a complete quiz.");
        }

        public string GetAddNewSlidesSupertip(Office.IRibbonControl control)
        {
            return this.localizationService.Translate("Adds a question an multiple new slides. They are fully styled and ready-to-use.");
        }

        public Bitmap GetAddNewSlidesButtonImage(Office.IRibbonControl control)
        {
            return Images.document;
        }

        public string GetSessionTypeGroupLabel(Office.IRibbonControl control)
        {
            return "Session";
        }

        public string GetSetSessionTypeLabel(Office.IRibbonControl control)
        {
            return this.localizationService.Translate("Select ARSnova App");
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
            return Images.home;
        }

        public string GetManageSessionLabel(Office.IRibbonControl control)
        {
            return this.localizationService.Translate("Manage questions");
        }

        public string GetManageSessionSupertip(Office.IRibbonControl control)
        {
            return this.localizationService.Translate("An overview above all questions and the possibility to edit or delete them.");
        }

        public void ManageSessionButtonClick(Office.IRibbonControl control)
        {
            this.ribbonHelper.ShowManageSession();
        }

        public Bitmap GetManageSessionButtonImage(Office.IRibbonControl control)
        {
            return Images.settings;
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
            return Images.trash;
        }

        public string GetHideButtonLabel(Office.IRibbonControl control)
        {
            return this.localizationService.Translate("Hide/Show");
        }

        public string GetHideButtonSupertip(Office.IRibbonControl control)
        {
            return this.localizationService.Translate("A hidden quiz won't show up during the presentation.");
        }

        public void HideButtonClick(Office.IRibbonControl control)
        {
            this.ribbonHelper.HideOrShowQuizFromSelectedSlide(SlideTracker.CurrentSlide);
        }

        public Bitmap GetHideButtonImage(Office.IRibbonControl control)
        {
            return Images.unlock;
        }

        public string GetArsnovaSlideContextMenuLabel(Office.IRibbonControl control)
        {
            return "ARSnova";
        }

        public Bitmap GetArsnovaFavIcon(Office.IRibbonControl control)
        {
            return Images.ARSnova_Logo;
        }


        public void AddQuizToNewSlideButtonClick(Office.IRibbonControl control)
        {
            this.ribbonHelper.AddCompleteQuizToNewSlides();
        }

        public bool AnySlideSelected(Office.IRibbonControl control)
        {
            var currentSlide = SlideTracker.CurrentSlide;
            // if there is no slide selected, insert new slide at the end of the presentation?
            return currentSlide != null;
        }

        /*public string GetStartQuizLabel(Office.IRibbonControl control)
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
                if (SlideTracker.GetSlideById(slideQuestionModel.QuestionInfoSlideId).SlideNumber == currentShowedSlidePosition)
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
                var slide = SlideTracker.GetSlideById(slideQuestionModel.QuestionInfoSlideId);
                if (slide.SlideNumber == currentShowedSlidePosition)
                {
                    this.ribbonHelper.StartQuiz(slideQuestionModel);
                }
            }
        }*/

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
            return Images.world;
        }

        public void HelpButtonClick(Office.IRibbonControl control)
        {
            System.Diagnostics.Process.Start("https://arsnova.thm.de/blog/");
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
            return Images.information;
        }

        public void AboutButtonClick(Office.IRibbonControl control)
        {
            var infoText = $"{this.localizationService.Translate("Product Information")}: {this.localizationService.Translate("This is an integration of the two audience response systems ARSnova.voting and ARSnova.click.")}" +
                $"Version: 0.1 (beta){Environment.NewLine}" + 
                $"{this.localizationService.Translate("License")}: {this.localizationService.Translate("This software is Open Source and licensed with the GNU General Public License Version 3")}";

            PopUpWindow.InformationWindow(
                this.localizationService.Translate("Information"),
                infoText);
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
