using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

using ARSnovaPPIntegration.Business.Contract;
using ARSnovaPPIntegration.Business.Model;
using ARSnovaPPIntegration.Common.Contract;
using ARSnovaPPIntegration.Common.Contract.Translators;
using ARSnovaPPIntegration.Common.Enum;
using ARSnovaPPIntegration.Presentation.Helpers;
using ARSnovaPPIntegration.Presentation.ViewPresenter;
using ARSnovaPPIntegration.Presentation.Window;
using Microsoft.Office.Core;

namespace ARSnovaPPIntegration.Presentation.Models
{
    public abstract class BaseViewModel : INotifyPropertyChanged, IWindowCommandBindings
    {
        protected readonly ViewPresenter.ViewPresenter ViewPresenter;

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

        protected void AddSessionToSlides()
        {
            var hasIntroSlide = this.HasIntroSlide();
            if (!hasIntroSlide)
            {
                var introSlide = this.RibbonHelper.CreateNewSlide(1);

                if (this.SlideSessionModel.SessionType == SessionType.ArsnovaClick)
                {
                    this.SlideManipulator.AddClickIntroSlide(introSlide, this.SlideSessionModel.Hashtag);
                }
                else
                {
                    // TODO voting intro slide   
                }

                PresentationInformationStore.SetArsnovaIntroSlideAdded();
            }

            // TODO setup finished, call business logik -> create / change session online (api service) (NewSession in model), manipulate / edit / create slide and fill up with content
            var validationResult = this.SlideSessionModel.NewSession ? 
                this.SessionManager.CreateSession(this.SlideSessionModel) : 
                this.SessionManager.EditSession(this.SlideSessionModel);

            if (validationResult.Success)
            {
                this.ViewPresenter.CloseWithoutPrompt();
            }
            else
            {
                PopUpWindow.ErrorWindow(validationResult.FailureTitel, validationResult.FailureMessage);
            }
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
            ViewPresenter.ViewPresenter viewPresenter,
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

        public ViewPresenter.ViewPresenter ViewPresenter { get; }

        public IQuestionTypeTranslator QuestionTypeTranslator { get; }

        public ILocalizationService LocalizationService { get; }

        public ISessionManager SessionManager { get; }

        public ISessionInformationProvider SessionInformationProvider { get; }

        public ISlideManipulator SlideManipulator { get; }

        public SlideSessionModel SlideSessionModel { get; }
    }
}
