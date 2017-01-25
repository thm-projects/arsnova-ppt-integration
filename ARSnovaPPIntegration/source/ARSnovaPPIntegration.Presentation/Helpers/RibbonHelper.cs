using System;
using System.Linq;

using Microsoft.Practices.ServiceLocation;
using Microsoft.Office.Interop.PowerPoint;

using ARSnovaPPIntegration.Common.Contract;
using ARSnovaPPIntegration.Presentation.Models;
using ARSnovaPPIntegration.Business.Contract;
using ARSnovaPPIntegration.Business.Model;
using ARSnovaPPIntegration.Common.Contract.Exceptions;
using ARSnovaPPIntegration.Common.Contract.Translators;
using ARSnovaPPIntegration.Common.Enum;
using ARSnovaPPIntegration.Presentation.ViewManagement;
using ARSnovaPPIntegration.Presentation.Window;

namespace ARSnovaPPIntegration.Presentation.Helpers
{
    public class RibbonHelper
    {
        private readonly ViewPresenter viewPresenter;

        private readonly ILocalizationService localizationService;

        private readonly ISessionManager sessionManager;

        private readonly ISessionInformationProvider sessionInformationProvider;

        private readonly ISlideManipulator slideManipulator;

        private readonly IQuestionTypeTranslator questionTypeTranslator;

        public RibbonHelper(
            ViewPresenter viewPresenter,
            ILocalizationService localizationService)
        {
            this.viewPresenter = viewPresenter;
            this.localizationService = localizationService;

            this.sessionManager = ServiceLocator.Current.GetInstance<ISessionManager>();

            // register events to enable the possibility to manipulate the presentation from the business layer (async methods, no return values)
            this.sessionManager.ShowNextSlideEventHandler += delegate
            {
                Globals.ThisAddIn.Application.ActivePresentation.SlideShowWindow.View.Next();
            };

            this.sessionInformationProvider = ServiceLocator.Current.GetInstance<ISessionInformationProvider>();
            this.questionTypeTranslator = ServiceLocator.Current.GetInstance<IQuestionTypeTranslator>();
            this.slideManipulator = ServiceLocator.Current.GetInstance<ISlideManipulator>();
        }

        public void ActivateSessionIfExists()
        {
            var slideSessionModel = PresentationInformationStore.GetStoredSlideSessionModel();

            // arsnova voting don't need to be activated
            if (slideSessionModel != null && slideSessionModel.SessionType == SessionType.ArsnovaClick)
            {
                var validationResult = this.sessionManager.ActivateClickSession(slideSessionModel);

                if (!validationResult.Success)
                {
                    throw new CommunicationException(validationResult.FailureMessage);
                }
            }
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

        public void ShowManageSession()
        {
            var slideSessionModel = this.GetSlideSessionModel();

            if (!slideSessionModel.SessionTypeSet)
            {
                this.viewPresenter.ShowInNewWindow(
                 new SelectArsnovaTypeViewViewModel(
                    new ViewModelRequirements(
                        this.viewPresenter,
                        this.questionTypeTranslator,
                        this.localizationService,
                        this.sessionManager,
                        this.sessionInformationProvider,
                        this.slideManipulator,
                        slideSessionModel)),
                    vm =>
                    {
                        vm.OnSelectArsnovaTypeViewClose += this.ShowManageSession;
                    });
                return;
            }

            this.viewPresenter.ShowInNewWindow(
                new SessionOverviewViewViewModel(
                    new ViewModelRequirements(
                        this.viewPresenter,
                        this.questionTypeTranslator,
                        this.localizationService,
                        this.sessionManager,
                        this.sessionInformationProvider,
                        this.slideManipulator,
                        slideSessionModel)));
        }

        public void AddCompleteQuizToNewSlides()
        {
            var newSlide = this.CreateNewSlide();
            this.AddQuizToSlide(newSlide);
        }

        public void AddQuizContentToShape()
        {
            var currentSlide = SlideTracker.CurrentSlide;
            this.AddQuizToSlide(currentSlide, true);
        }

        public void EditQuizSetup(Slide slide)
        {
            var slideSessionModel = this.GetSlideSessionModel();

            var slideQuestionModel = slideSessionModel.Questions.First(q => q.QuestionSlideId == slide.SlideID);

            if (slideQuestionModel == null)
            {
                throw new Exception("No existing arsnova question on slide.");
            }

            this.viewPresenter.ShowInNewWindow(
                new QuestionViewViewModel(this.CreateViewModelRequirements(slideSessionModel), slideQuestionModel.Id, false));
        }

        public void StartQuiz(SlideQuestionModel slideQuestionModel)
        {
            var slideSessionModel = this.GetSlideSessionModel();

            var questionSlide = SlideTracker.GetSlideById(slideQuestionModel.QuestionSlideId);
            var resultsSlide = SlideTracker.GetSlideById(slideQuestionModel.ResultsSlideId);

            this.sessionManager.StartSession(slideSessionModel, slideQuestionModel.Index, questionSlide, resultsSlide);
        }

        public void DeleteQuizFromSelectedSlide(Slide slide)
        {
            var slideSessionModel = this.GetSlideSessionModel();

            var slideQuestionModel = slideSessionModel.Questions.First(q => q.QuestionSlideId == slide.SlideID);

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

        public void RemoveClickQuizDataOnServer()
        {
            var slideSessionModel = PresentationInformationStore.GetStoredSlideSessionModel();

            if (slideSessionModel != null)
            {
                this.sessionManager.RemoveClickQuizDataFromServer(slideSessionModel);
            }
        }

        private void AddQuizToSlide(Slide slide, bool contentOnOneShape = false)
        {
            var slideSessionModel = this.GetSlideSessionModel();

            if (!slideSessionModel.SessionTypeSet)
            {
                this.viewPresenter.ShowInNewWindow(
                 new SelectArsnovaTypeViewViewModel(
                    new ViewModelRequirements(
                        this.viewPresenter,
                        this.questionTypeTranslator,
                        this.localizationService,
                        this.sessionManager,
                        this.sessionInformationProvider,
                        this.slideManipulator,
                        slideSessionModel)),
                    vm =>
                    {
                        vm.OnSelectArsnovaTypeViewClose += () => this.AddQuizToSlide(slide);
                    });
                return;
            }

            var newQuestion = new SlideQuestionModel
            {
                QuestionType = slideSessionModel.SessionType == SessionType.ArsnovaClick
                                                                ? QuestionTypeEnum.SingleChoiceClick
                                                                : QuestionTypeEnum.SingleChoiceVoting,
                Index = slideSessionModel.Questions.Count,
                QuestionSlideId = slide.SlideID,
                QuizInOneShape = contentOnOneShape
            };

            slideSessionModel.Questions.Add(newQuestion);

            this.viewPresenter.ShowInNewWindow(
                new QuestionViewViewModel(this.CreateViewModelRequirements(slideSessionModel), newQuestion.Id, true));
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
