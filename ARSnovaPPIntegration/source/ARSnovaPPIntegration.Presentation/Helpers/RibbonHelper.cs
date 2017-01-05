using System;
using System.Linq;

using Microsoft.Practices.ServiceLocation;
using Microsoft.Office.Interop.PowerPoint;

using ARSnovaPPIntegration.Common.Contract;
using ARSnovaPPIntegration.Presentation.Models;
using ARSnovaPPIntegration.Business.Contract;
using ARSnovaPPIntegration.Business.Model;
using ARSnovaPPIntegration.Common.Contract.Translators;
using ARSnovaPPIntegration.Common.Enum;
using ARSnovaPPIntegration.Presentation.Window;

namespace ARSnovaPPIntegration.Presentation.Helpers
{
    public class RibbonHelper
    {
        private readonly ViewPresenter.ViewPresenter viewPresenter;

        private readonly ILocalizationService localizationService;

        private readonly ISessionManager sessionManager;

        private readonly ISessionInformationProvider sessionInformationProvider;

        private readonly ISlideManipulator slideManipulator;

        private readonly IQuestionTypeTranslator questionTypeTranslator;

        public RibbonHelper(
            ViewPresenter.ViewPresenter viewPresenter,
            ILocalizationService localizationService)
        {
            this.viewPresenter = viewPresenter;
            this.localizationService = localizationService;

            this.sessionManager = ServiceLocator.Current.GetInstance<ISessionManager>();
            this.sessionInformationProvider = ServiceLocator.Current.GetInstance<ISessionInformationProvider>();
            this.questionTypeTranslator = ServiceLocator.Current.GetInstance<IQuestionTypeTranslator>();
            this.slideManipulator = ServiceLocator.Current.GetInstance<ISlideManipulator>();
        }

        public void ShowSetSessionTypeDialog()
        {
            var slideSessionModel = this.GetSlideSessionModel();

            this.viewPresenter.ShowInNewWindow(
                new SelectArsnovaTypeViewViewModel(
                    new ViewModelRequirements(
                        this.viewPresenter,
                        this.questionTypeTranslator,
                        this.localizationService,
                        this.sessionManager,
                        this.sessionInformationProvider,
                        this.slideManipulator,
                        slideSessionModel)));
        }

        public void AddQuizToSlide(Slide slide)
        {
            var slideSessionModel = this.GetSlideSessionModel();

            var newQuestion = new SlideQuestionModel(this.questionTypeTranslator)
            {
                QuestionType = slideSessionModel.SessionType == SessionType.ArsnovaClick
                                                                ? QuestionTypeEnum.SingleChoiceClick
                                                                : QuestionTypeEnum.SingleChoiceVoting,
                Index = slideSessionModel.Questions.Count,
                Slide = slide
            };

            slideSessionModel.Questions.Add(newQuestion);

            this.viewPresenter.ShowInNewWindow(
                new QuestionViewViewModel(this.CreateViewModelRequirements(slideSessionModel), newQuestion.Id, true));
        }

        public void EditQuizSetup(Slide slide)
        {
            var slideSessionModel = this.GetSlideSessionModel();

            var slideQuestionModel = slideSessionModel.Questions.First(q => q.Slide == slide);

            if (slideQuestionModel == null)
            {
                throw new Exception("No existing arsnova question on slide.");
            }

            this.viewPresenter.ShowInNewWindow(
                new QuestionViewViewModel(this.CreateViewModelRequirements(slideSessionModel), slideQuestionModel.Id, false));
        }

        public void DeleteQuizFromSelectedSlide(Slide slide)
        {
            var slideSessionModel = this.GetSlideSessionModel();

            var slideQuestionModel = slideSessionModel.Questions.First(q => q.Slide == slide);

            if (slideQuestionModel == null)
            {
                throw new Exception("No existing arsnova question on slide.");
            }

            var questionText = $"{this.localizationService.Translate("Do you really want to delete this question?")}{Environment.NewLine}{Environment.NewLine}";
            questionText += slideQuestionModel.QuestionText;

            var deleteQuestion = PopUpWindow.ConfirmationWindow(
                this.localizationService.Translate("Delete"),
                questionText);

            if (deleteQuestion)
            {
                slideSessionModel.Questions.Remove(slideQuestionModel);
            }
        }

        public Slide CreateNewSlide()
        {
            var currentSlide = SlideTracker.CurrentSlide;

            return currentSlide == null 
                ? this.CreateNewSlide(Globals.ThisAddIn.Application.ActivePresentation.Slides.Count)
                : this.CreateNewSlide(currentSlide.SlideIndex + 1);
        }

        public Slide CreateNewSlide(int index)
        {
            var newSlide = Globals.ThisAddIn.Application.ActivePresentation.Slides.Add(index, PpSlideLayout.ppLayoutTitle);
            return newSlide;
        }

        private SlideSessionModel GetSlideSessionModel()
        {
            var slideSessionModel = PresentationInformationStore.GetStoredSlideSessionModel();

            return slideSessionModel ?? new SlideSessionModel();
        }

        private ViewModelRequirements CreateViewModelRequirements(SlideSessionModel slideSessionModel)
        {
            return new ViewModelRequirements(
                this.viewPresenter,
                this.questionTypeTranslator,
                this.localizationService,
                this.sessionManager,
                this.sessionInformationProvider,
                this.slideManipulator,
                slideSessionModel);
        }
    }
}
