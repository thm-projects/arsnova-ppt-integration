using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ARSnovaPPIntegration.Business.Model;
using ARSnovaPPIntegration.Common.Contract;
using ARSnovaPPIntegration.Presentation.Commands;
using ARSnovaPPIntegration.Common.Enum;

namespace ARSnovaPPIntegration.Presentation.Models
{
    public class SelectArsnovaTypeViewModel : BaseModel
    {
        private SlideSessionModel slideSessionModel;

        public SelectArsnovaTypeViewModel(
            ViewPresenter.ViewPresenter viewPresenter,
            ILocalizationService localizationService,
            SlideSessionModel slideSessionModel) 
            : base(viewPresenter, localizationService)
        {
            this.slideSessionModel = slideSessionModel;

            this.InitializeWindowCommandBindings();
        }

        public bool IsArsnovaClickSession
        {
            get { return this.slideSessionModel.SessionType == SessionType.ArsnovaClick; }
            set
            {
                if (value)
                {
                    this.slideSessionModel.SessionType = SessionType.ArsnovaClick;
                    this.OnPropertyChanged(nameof(this.IsArsnovaVotingSession));
                }
            }
        }

        public bool IsArsnovaVotingSession
        {
            get { return this.slideSessionModel.SessionType == SessionType.ArsnovaVoting; }
            set
            {
                if (value)
                {
                    this.slideSessionModel.SessionType = SessionType.ArsnovaVoting;
                    this.OnPropertyChanged(nameof(this.IsArsnovaClickSession));
                }
            }
        }

        public string Header => this.LocalizationService.Translate("New question");

        public string Text
            =>
            this.LocalizationService.Translate(
                    "Which type of question do you want to ask? Arsnova.voting is the serious, grown up one while arsnova.click is faster, more colorful and crammed up with gamification.");

        private void InitializeWindowCommandBindings()
        {
            this.WindowCommandBindings.AddRange(
                    new List<CommandBinding>
                    {
                        new CommandBinding(
                            NavigationButtonCommands.Forward,
                            (e, o) =>
                            {
                                // TODO ViewPresenter: Forward to next view
                            },
                            (e, o) => o.CanExecute = true)
                    });
        }
    }
}
