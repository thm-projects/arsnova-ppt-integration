using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

using ARSnovaPPIntegration.Business.Contract;
using ARSnovaPPIntegration.Business.Model;
using ARSnovaPPIntegration.Common.Contract;
using ARSnovaPPIntegration.Common.Contract.Translators;
using ARSnovaPPIntegration.Presentation.Helpers;
using ARSnovaPPIntegration.Presentation.ViewManagement;
using ARSnovaPPIntegration.Presentation.Window;

namespace ARSnovaPPIntegration.Presentation.Models
{
    public abstract class BaseViewModel : INotifyPropertyChanged, IWindowCommandBindings
    {
        protected readonly ViewPresenter ViewPresenter;

        protected readonly IQuestionTypeTranslator QuestionTypeTranslator;

        protected readonly ILocalizationService LocalizationService;

        protected readonly ISessionManager SessionManager;

        protected readonly ISessionInformationProvider SessionInformationProvider;

        protected readonly ISlideManipulator SlideManipulator;

        protected readonly RibbonHelper RibbonHelper;

        protected SlideSessionModel SlideSessionModel;

        protected BaseViewModel(ViewModelRequirements requirements)
        {
            this.ViewPresenter = requirements.ViewPresenter;
            this.QuestionTypeTranslator = requirements.QuestionTypeTranslator;
            this.LocalizationService = requirements.LocalizationService;
            this.SessionManager = requirements.SessionManager;
            this.SessionInformationProvider = requirements.SessionInformationProvider;
            this.SlideManipulator = requirements.SlideManipulator;
            this.RibbonHelper = new RibbonHelper(this.ViewPresenter, this.LocalizationService);

            this.SlideSessionModel = requirements.SlideSessionModel;
        }
        public List<CommandBinding> WindowCommandBindings { get; set; } = new List<CommandBinding>();

        public event PropertyChangedEventHandler PropertyChanged;

        protected void DisplayFailedValidationResults(string failedValidtionsList)
        {
            PopUpWindow.ErrorWindow(
                this.LocalizationService.Translate("Validation error"),
                this.LocalizationService.Translate("Validation failed") + ":"
                    + Environment.NewLine + Environment.NewLine + failedValidtionsList);
        }

        protected abstract Tuple<bool, string> Validate();

        protected void AddSessionToSlides(SlideQuestionModel slideQuestionModel)
        {
            var hasIntroSlide = this.HasIntroSlide();
            if (!hasIntroSlide)
            {
                var introSlide = this.RibbonHelper.CreateNewSlide(1);

                this.SlideManipulator.AddIntroSlide(this.SlideSessionModel, introSlide);

                this.SlideSessionModel.IntroSlideId = introSlide.SlideID;

                PresentationInformationStore.SetArsnovaIntroSlideAdded();
            }

            if (slideQuestionModel.QuizInOneShape)
            {
                var selectedSlide = SlideTracker.CurrentSlide;
                slideQuestionModel.ResultsSlideId = selectedSlide.SlideID;
                slideQuestionModel.QuestionInfoSlideId = selectedSlide.SlideID;
                slideQuestionModel.QuestionTimerSlideId = selectedSlide.SlideID;

                this.SlideManipulator.AddQuizToSlideWithoutStyling(slideQuestionModel, selectedSlide);
            }
            else
            {
                var questionInfoSlide = SlideTracker.GetSlideById(slideQuestionModel.QuestionInfoSlideId);

                var questionTimerSlide = slideQuestionModel.QuestionTimerSlideId.HasValue
                                            ? SlideTracker.GetSlideById(slideQuestionModel.QuestionTimerSlideId.Value) 
                                            : this.RibbonHelper.CreateNewSlide(questionInfoSlide.SlideIndex + 1);
                slideQuestionModel.QuestionTimerSlideId = questionTimerSlide.SlideID;

                var resultsSlide = slideQuestionModel.ResultsSlideId.HasValue
                                            ? SlideTracker.GetSlideById(slideQuestionModel.ResultsSlideId.Value)
                                            : this.RibbonHelper.CreateNewSlide(questionInfoSlide.SlideIndex + 2);
                slideQuestionModel.ResultsSlideId = resultsSlide.SlideID;

                this.SlideManipulator.AddQuizToStyledSlides(slideQuestionModel, questionInfoSlide, questionTimerSlide, resultsSlide);
            }

            PresentationInformationStore.StoreSlideSessionModel(this.SlideSessionModel);
            this.ViewPresenter.CloseWithoutPrompt();
        }

        protected ViewModelRequirements GetViewModelRequirements()
        {
            return new ViewModelRequirements(
                this.ViewPresenter,
                this.QuestionTypeTranslator,
                this.LocalizationService,
                this.SessionManager,
                this.SessionInformationProvider,
                this.SlideManipulator,
                this.SlideSessionModel);
        }

        protected void OnPropertyChanged(string propertyName = null)
        {
            var handler = this.PropertyChanged;

            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        private bool HasIntroSlide()
        {
            // TODO set to false if the user deletes the slide
            return PresentationInformationStore.IsArsnovaIntroSlideAlreadyAdded();
        }
    }

    public class ViewModelRequirements
    {
        public ViewModelRequirements(
            ViewPresenter viewPresenter,
            IQuestionTypeTranslator questionTypeTranslator,
            ILocalizationService localizationService,
            ISessionManager sessionManager,
            ISessionInformationProvider sessionInformationProvider,
            ISlideManipulator slideManipulator,
            SlideSessionModel slideSessionModel)
        {
            this.ViewPresenter = viewPresenter;
            this.QuestionTypeTranslator = questionTypeTranslator;
            this.LocalizationService = localizationService;
            this.SessionManager = sessionManager;
            this.SessionInformationProvider = sessionInformationProvider;
            this.SlideSessionModel = slideSessionModel;
            this.SlideManipulator = slideManipulator;
        }

        public ViewPresenter ViewPresenter { get; }

        public IQuestionTypeTranslator QuestionTypeTranslator { get; }

        public ILocalizationService LocalizationService { get; }

        public ISessionManager SessionManager { get; }

        public ISessionInformationProvider SessionInformationProvider { get; }

        public ISlideManipulator SlideManipulator { get; }

        public SlideSessionModel SlideSessionModel { get; }
    }
}
