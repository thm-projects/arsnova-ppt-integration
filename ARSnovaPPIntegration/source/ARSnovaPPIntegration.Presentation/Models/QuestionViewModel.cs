using System.Collections.Generic;
using System.Windows.Input;

using Microsoft.Practices.ServiceLocation;

using ARSnovaPPIntegration.Business.Contract;
using ARSnovaPPIntegration.Business.Model;
using ARSnovaPPIntegration.Common.Contract;
using ARSnovaPPIntegration.Common.Enum;
using ARSnovaPPIntegration.Presentation.Commands;

namespace ARSnovaPPIntegration.Presentation.Models
{
    public class QuestionViewModel : BaseModel
    {
        public QuestionViewModel(
            ViewPresenter.ViewPresenter viewPresenter,
            ILocalizationService localizationService,
            SlideSessionModel slideSessionModel) 
            : base(viewPresenter, localizationService, slideSessionModel)
        {
            this.InitializeWindowCommandBindings();

            var sessionInformationProvider = ServiceLocator.Current.GetInstance<ISessionInformationProvider>();

            this.QuestionTypes = slideSessionModel.SessionType == SessionType.ArsnovaClick
                ? sessionInformationProvider.GetAvailableQuestionsClick()
                : sessionInformationProvider.GetAvailableQuestionsVoting();
        }

        public string Header => this.LocalizationService.Translate("Set question");

        public string Text => this.LocalizationService.Translate("Choose a question type and enter the question text:");

        public string QuestionTextPlaceholder => this.LocalizationService.Translate("Enter the question text here.");

        public List<QuestionType> QuestionTypes { get; set; }

        public QuestionTypeEnum QuestionType
        {
            get
            {
                if (this.SlideSessionModel.QuestionType != 0)
                {
                    return this.SlideSessionModel.QuestionType;
                }
                else
                {
                    return this.SlideSessionModel.SessionType == SessionType.ArsnovaClick
                     ? QuestionTypeEnum.SingleChoiceClick
                     : QuestionTypeEnum.SingleChoiceVoting;
                }
            }
            set { this.SlideSessionModel.QuestionType = value; }
        }

        public string QuestionText
        {
            get { return this.SlideSessionModel.QuestionText; }
            set { this.SlideSessionModel.QuestionText = value; }
        }

        private void InitializeWindowCommandBindings()
        {
            this.WindowCommandBindings.AddRange(
                    new List<CommandBinding>
                    {
                        new CommandBinding(
                            NavigationButtonCommands.Back,
                            (e, o) =>
                            {
                                this.ViewPresenter.Show(
                                    new SelectArsnovaTypeViewModel(
                                        this.ViewPresenter,
                                        this.LocalizationService,
                                        this.SlideSessionModel));
                            },
                            (e, o) => o.CanExecute = true),
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
