using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using ARSnovaPPIntegration.Business.Contract;
using ARSnovaPPIntegration.Business.Model;
using ARSnovaPPIntegration.Common.Contract;
using ARSnovaPPIntegration.Presentation.Commands;
using ARSnovaPPIntegration.Presentation.ViewPresenter;
using ARSnovaPPIntegration.Presentation.Window;

namespace ARSnovaPPIntegration.Presentation.Models
{
    public abstract class BaseModel : INotifyPropertyChanged, IWindowCommandBindings
    {
        protected readonly ViewPresenter.ViewPresenter ViewPresenter;

        protected readonly ILocalizationService LocalizationService;

        protected readonly ISessionManager SessionManager;

        protected readonly ISessionInformationProvider SessionInformationProvider;

        protected SlideSessionModel SlideSessionModel;

        protected BaseModel(ViewModelRequirements requirements)
        {
            this.ViewPresenter = requirements.ViewPresenter;
            this.LocalizationService = requirements.LocalizationService;
            this.SessionManager = requirements.SessionManager;
            this.SessionInformationProvider = requirements.SessionInformationProvider;

            this.SlideSessionModel = requirements.SlideSessionModel;

            this.WindowCommandBindings.AddRange(new List<CommandBinding>
            {
                new CommandBinding(
                    NavigationButtonCommands.Cancel,
                    (e, o) =>
                    {
                        this.ViewPresenter.CloseWithPrompt();
                    },
                    (e, o) => o.CanExecute = true)
              });
        }
        public List<CommandBinding> WindowCommandBindings { get; set; } = new List<CommandBinding>();

        public event PropertyChangedEventHandler PropertyChanged;

        protected void AddSessionToSlides()
        {
            // TODO setup finished, call business logik -> create / change session online (api service) (NewSession in model), manipulate / edit / create slide and fill up with content
            ValidationResult validationResult;

            if (this.SlideSessionModel.NewSession)
            {
                validationResult = this.SessionManager.CreateSession(this.SlideSessionModel);
            }
            else
            {
                validationResult = this.SessionManager.EditSession(this.SlideSessionModel);
            }

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
                this.LocalizationService,
                this.SessionManager,
                this.SessionInformationProvider,
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
    }

    public class ViewModelRequirements
    {
        public ViewModelRequirements(
            ViewPresenter.ViewPresenter viewPresenter,
            ILocalizationService localizationService,
            ISessionManager sessionManager,
            ISessionInformationProvider sessionInformationProvider,
            SlideSessionModel slideSessionModel)
        {
            this.ViewPresenter = viewPresenter;
            this.LocalizationService = localizationService;
            this.SessionManager = sessionManager;
            this.SessionInformationProvider = sessionInformationProvider;
            this.SlideSessionModel = slideSessionModel;
        }

        public ViewPresenter.ViewPresenter ViewPresenter { get; set; }

        public ILocalizationService LocalizationService { get; set; }

        public ISessionManager SessionManager { get; set; }

        public ISessionInformationProvider SessionInformationProvider { get; set; }

        public SlideSessionModel SlideSessionModel { get; set; }
}
}
