using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using ARSnovaPPIntegration.Business.Contract;
using ARSnovaPPIntegration.Business.Model;
using ARSnovaPPIntegration.Common.Contract;
using ARSnovaPPIntegration.Presentation.ViewPresenter;

namespace ARSnovaPPIntegration.Presentation.Models
{
    public abstract class BaseModel : INotifyPropertyChanged, IWindowCommandBindings
    {
        protected readonly ViewPresenter.ViewPresenter ViewPresenter;

        protected readonly ILocalizationService LocalizationService;

        protected readonly ISessionManager SessionManager;

        protected SlideSessionModel SlideSessionModel;

        protected BaseModel(ViewModelRequirements requirements)
        {
            this.ViewPresenter = requirements.ViewPresenter;
            this.LocalizationService = requirements.LocalizationService;
            this.SessionManager = requirements.SessionManager;

            this.SlideSessionModel = requirements.SlideSessionModel;

            // Question if window should be closed is triggered twice. Can't find a solution atm -> cancel button is defered
            /*this.WindowCommandBindings.AddRange(new List<CommandBinding>
            {
                new CommandBinding(
                    NavigationButtonCommands.Cancel,
                    (e, o) =>
                    {
                        var close = PopUpWindow.CloseWindowPrompt();

                        if (close)
                        {
                            this.ViewPresenter.ContentCleanUp(true);
                        }
                    },
                    (e, o) => o.CanExecute = true)
              });*/
        }
        public List<CommandBinding> WindowCommandBindings { get; set; } = new List<CommandBinding>();

        public event PropertyChangedEventHandler PropertyChanged;

        protected void AddSessionToSlides()
        {
            // TODO setup finished, call business logik -> create / change session online (api service) (NewSession in model), manipulate / edit / create slide and fill up with content
            if (this.SlideSessionModel.NewSession)
            {
                this.SessionManager.CreateSession(this.SlideSessionModel);
            }
            else
            {
                this.SessionManager.EditSession(this.SlideSessionModel);
            }
        }

        protected ViewModelRequirements GetViewModelRequirements()
        {
            return new ViewModelRequirements(
                this.ViewPresenter,
                this.LocalizationService,
                this.SessionManager,
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
            SlideSessionModel slideSessionModel)
        {
            this.ViewPresenter = viewPresenter;
            this.LocalizationService = localizationService;
            this.SessionManager = sessionManager;
            this.SlideSessionModel = slideSessionModel;
        }

        public ViewPresenter.ViewPresenter ViewPresenter { get; set; }

        public ILocalizationService LocalizationService { get; set; }

        public ISessionManager SessionManager { get; set; }

        public SlideSessionModel SlideSessionModel { get; set; }
}
}
