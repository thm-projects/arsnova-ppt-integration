using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ARSnovaPPIntegration.Common.Contract;
using ARSnovaPPIntegration.Presentation.ViewPresenter;
using Microsoft.Practices.ServiceLocation;

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
        }

        public string LabelTest => "testText";
    }
}
