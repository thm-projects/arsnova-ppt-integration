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
using Microsoft.Office.Core;

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
                // TODO the next method is throwing errors!
                // Globals.ThisAddIn.Application.ActivePresentation.SlideShowWindow.View.Next();
            };

            this.sessionInformationProvider = ServiceLocator.Current.GetInstance<ISessionInformationProvider>();
            this.questionTypeTranslator = ServiceLocator.Current.GetInstance<IQuestionTypeTranslator>();
            this.slideManipulator = ServiceLocator.Current.GetInstance<ISlideManipulator>();
        }

        public void ActivateSessionIfExists()
        {
            var slideSessionModel = PresentationInformationStore.GetStoredSlideSessionModel();

            if (slideSessionModel != null)
            {
                this.sessionManager.ActivateSession(slideSessionModel);
            }
        }

        public void CleanUpOnStart()
        {
            var slideSessionModel = PresentationInformationStore.GetStoredSlideSessionModel();

            if (slideSessionModel != null)
            {
                foreach (var slideQuestionModel in slideSessionModel.Questions)
                {
                    if (slideQuestionModel.QuestionTimerSlideId.HasValue)
                    {
                        this.slideManipulator.SetTimerOnSlide(
                            slideQuestionModel,
                            SlideTracker.GetSlideById(slideQuestionModel.QuestionTimerSlideId.Value),
                            slideQuestionModel.Countdown);
                    }

                    if (slideQuestionModel.ResultsSlideId.HasValue)
                    {
                        this.slideManipulator.CleanResultsPage(SlideTracker.GetSlideById(slideQuestionModel.ResultsSlideId.Value));
                    }
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

        public void EditQuizSetup(Slide slide)
        {
            var slideSessionModel = this.GetSlideSessionModel();

            var slideQuestionModel = slideSessionModel.Questions.First(q => q.QuestionInfoSlideId == slide.SlideID
                                                                                || q.QuestionTimerSlideId == slide.SlideID
                                                                                || q.ResultsSlideId == slide.SlideID);

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
            var resultsSlide = SlideTracker.GetSlideById(slideQuestionModel.ResultsSlideId.Value);

            if (this.sessionInformationProvider.IsClickQuestion(slideQuestionModel.QuestionType))
            {
                // arsnova click
                var questionTimerSlide = SlideTracker.GetSlideById(slideQuestionModel.QuestionTimerSlideId.Value);
                this.sessionManager.StartClickQuestion(slideSessionModel, slideQuestionModel.Index, questionTimerSlide,
                    resultsSlide);
            }
            else
            {
                // arsnova.voting
                this.sessionManager.StartVotingQuestion(slideSessionModel, slideQuestionModel, resultsSlide);
            }
        }

        public void GetAndDisplayArsnovaVotingResults(SlideQuestionModel slideQuestionModel)
        {
            var slideSessionModel = this.GetSlideSessionModel();
            var resultsSlide = SlideTracker.GetSlideById(slideQuestionModel.ResultsSlideId.Value);

            this.sessionManager.GetAndDisplayArsnovaVotingResults(slideSessionModel, slideQuestionModel, resultsSlide);
        }

        public void DeleteQuizFromSelectedSlide(Slide slide)
        {
            var slideSessionModel = this.GetSlideSessionModel();

            var slideQuestionModel = slideSessionModel.Questions.First(q => q.QuestionInfoSlideId == slide.SlideID
                                                                                || q.QuestionTimerSlideId == slide.SlideID
                                                                                || q.ResultsSlideId == slide.SlideID);

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
                SlideTracker.RemoveSlide(slideQuestionModel.QuestionInfoSlideId);
                SlideTracker.RemoveSlide(slideQuestionModel.QuestionTimerSlideId.Value);
                SlideTracker.RemoveSlide(slideQuestionModel.ResultsSlideId.Value);
                slideSessionModel.Questions.Remove(slideQuestionModel);
            }
        }

        public void HideOrShowQuizFromSelectedSlide(Slide slide)
        {
            var slideSessionModel = this.GetSlideSessionModel();

            var slideQuestionModel = slideSessionModel.Questions.First(q => q.QuestionInfoSlideId == slide.SlideID
                                                                                || q.QuestionTimerSlideId == slide.SlideID
                                                                                || q.ResultsSlideId == slide.SlideID);

            if (slideQuestionModel == null)
            {
                throw new Exception("No existing arsnova question on slide.");
            }

            var questionInfoSlide = SlideTracker.GetSlideById(slideQuestionModel.QuestionInfoSlideId);
            var questionTimerSlide = SlideTracker.GetSlideById(slideQuestionModel.QuestionTimerSlideId.Value);
            var resultsSlide = SlideTracker.GetSlideById(slideQuestionModel.ResultsSlideId.Value);

            if (slideQuestionModel.Hidden)
            {
                questionInfoSlide.SlideShowTransition.Hidden = MsoTriState.msoFalse;
                questionTimerSlide.SlideShowTransition.Hidden = MsoTriState.msoFalse;
                resultsSlide.SlideShowTransition.Hidden = MsoTriState.msoFalse;
            }
            else
            {
                questionInfoSlide.SlideShowTransition.Hidden = MsoTriState.msoTrue;
                questionTimerSlide.SlideShowTransition.Hidden = MsoTriState.msoTrue;
                resultsSlide.SlideShowTransition.Hidden = MsoTriState.msoTrue;
            }

            slideQuestionModel.Hidden = !slideQuestionModel.Hidden;

            // recalculate question index: asrnova.click index needs to start with 0 and should not leave out any number!
            var ignoredQuestions = 0;

            for (var i = 0; i < slideSessionModel.Questions.Count; i++)
            {
                var question = slideSessionModel.Questions.FirstOrDefault(q => q.Index == i);

                if (question == null)
                    continue;

                if (question.Hidden)
                {
                    ignoredQuestions++;
                }
                else
                {
                    question.RecalculatedOnlineIndex = question.Index - ignoredQuestions;
                }
            }

            PresentationInformationStore.StoreSlideSessionModel(slideSessionModel);
        }

        public Slide CreateNewSlide()
        {
            var currentSlide = SlideTracker.CurrentSlide;
            while (SlideTracker.IsArsnovaSlide(currentSlide))
            {
                currentSlide = SlideTracker.GetSlideByIndex(currentSlide.SlideIndex + 1);
            }

            return currentSlide == null 
                ? this.CreateNewSlide(Globals.ThisAddIn.Application.ActivePresentation.Slides.Count + 1)
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

        public void SendKeepAlive(SlideSessionModel slideSessionModel)
        {
            this.sessionManager.KeepAlive(slideSessionModel);
        }

        private void AddQuizToSlide(Slide slide)
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
                RecalculatedOnlineIndex = slideSessionModel.Questions.Count(q => !q.Hidden),
                QuestionInfoSlideId = slide.SlideID
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
