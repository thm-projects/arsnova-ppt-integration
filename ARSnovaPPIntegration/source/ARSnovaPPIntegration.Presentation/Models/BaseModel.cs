using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using ARSnovaPPIntegration.Common.Contract;
using ARSnovaPPIntegration.Presentation.ViewPresenter;
using ARSnovaPPIntegration.Presentation.Window;
using ARSnovaPPIntegration.Presentation.Commands;

namespace ARSnovaPPIntegration.Presentation.Models
{
    public abstract class BaseModel : INotifyPropertyChanged, IWindowCommandBindings
    {
        protected readonly ViewPresenter.ViewPresenter ViewPresenter;

        protected readonly ILocalizationService LocalizationService;

        protected BaseModel(
            ViewPresenter.ViewPresenter viewPresenter,
            ILocalizationService localizationService)
        {
            this.ViewPresenter = viewPresenter;
            this.LocalizationService = localizationService;

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
                            this.ViewPresenter.Close<SelectArsnovaTypeViewModel>();
                        }
                    },
                    (e, o) => o.CanExecute = true)
              });*/
        }
        public List<CommandBinding> WindowCommandBindings { get; set; } = new List<CommandBinding>();

        public event PropertyChangedEventHandler PropertyChanged;

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
}
