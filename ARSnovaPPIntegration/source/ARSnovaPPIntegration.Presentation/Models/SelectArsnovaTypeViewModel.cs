using System.Collections.Generic;
using System.Windows.Input;

using ARSnovaPPIntegration.Business.Model;
using ARSnovaPPIntegration.Common.Contract;
using ARSnovaPPIntegration.Presentation.Commands;
using ARSnovaPPIntegration.Common.Enum;

namespace ARSnovaPPIntegration.Presentation.Models
{
    public class SelectArsnovaTypeViewModel : BaseModel
    {
        public SelectArsnovaTypeViewModel(
            ViewPresenter.ViewPresenter viewPresenter,
            ILocalizationService localizationService,
            SlideSessionModel slideSessionModel) 
            : base(viewPresenter, localizationService, slideSessionModel)
        {
            this.InitializeWindowCommandBindings();
        }

        public bool IsArsnovaClickSession
        {
            get { return this.SlideSessionModel.SessionType == SessionType.ArsnovaClick; }
            set
            {
                if (value)
                {
                    this.SlideSessionModel.SessionType = SessionType.ArsnovaClick;
                    this.OnPropertyChanged(nameof(this.IsArsnovaVotingSession));
                }
            }
        }

        public bool IsArsnovaVotingSession
        {
            get { return this.SlideSessionModel.SessionType == SessionType.ArsnovaVoting; }
            set
            {
                if (value)
                {
                    this.SlideSessionModel.SessionType = SessionType.ArsnovaVoting;
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
                                this.ViewPresenter.Show(
                                    new QuestionViewModel(
                                        this.ViewPresenter,
                                        this.LocalizationService,
                                        this.SlideSessionModel));
                            },
                            (e, o) => o.CanExecute = true)
                    });
        }
    }
}
