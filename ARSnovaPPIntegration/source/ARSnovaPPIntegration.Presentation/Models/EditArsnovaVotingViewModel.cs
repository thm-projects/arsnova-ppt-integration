using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ARSnovaPPIntegration.Common.Contract;
using ARSnovaPPIntegration.Presentation.Commands;
using ARSnovaPPIntegration.Presentation.ViewPresenter;

namespace ARSnovaPPIntegration.Presentation.Models
{
    public class EditArsnovaVotingViewModel : IWindowCommandBindings
    {
        private readonly ViewPresenter.ViewPresenter viewPresenter;

        private readonly ILocalizationService localizationService;

        public List<CommandBinding> WindowCommandBindings { get; } = new List<CommandBinding>();

        public EditArsnovaVotingViewModel(
            ViewPresenter.ViewPresenter viewPresenter,
            ILocalizationService localizationService)
        {
            this.viewPresenter = viewPresenter;
            this.localizationService = localizationService;

            this.InitializeWindowCommandBindings();
        }

        public string LabelTest => "testText";

        private void InitializeWindowCommandBindings()
        {
            this.WindowCommandBindings.Add(
                new CommandBinding(
                    NavigationButtonCommands.Back,
                    (e, o) => {  },
                    (e, o) => o.CanExecute = true));
        }
    }
}
