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
    public class AnswerOptionViewModel : BaseModel
    {
        private readonly ISessionInformationProvider sessionInformationProvider;

        public AnswerOptionViewModel(
            ViewPresenter.ViewPresenter viewPresenter,
            ILocalizationService localizationService,
            SlideSessionModel slideSessionModel) 
            : base(viewPresenter, localizationService, slideSessionModel)
        {
            this.InitializeWindowCommandBindings();

            this.sessionInformationProvider = ServiceLocator.Current.GetInstance<ISessionInformationProvider>();
        }

        public string Header => this.LocalizationService.Translate("Set the answer option(s)");

        public bool ShowGeneralAnswerOptions =>
            this.sessionInformationProvider.GetAnswerOptionType(this.SlideSessionModel.QuestionType) == AnswerOptionType.ShowGeneralAnswerOptions;

        public bool ShowFreeTextAnswerOptions =>
            this.sessionInformationProvider.GetAnswerOptionType(this.SlideSessionModel.QuestionType) == AnswerOptionType.ShowFreeTextAnswerOptions;

        public bool ShowEvaluationAnswerOptions =>
            this.sessionInformationProvider.GetAnswerOptionType(this.SlideSessionModel.QuestionType) == AnswerOptionType.ShowEvaluationAnswerOptions;

        public bool ShowGradeAnswerOptions =>
            this.sessionInformationProvider.GetAnswerOptionType(this.SlideSessionModel.QuestionType) == AnswerOptionType.ShowGradeAnswerOptions;

        public bool ShowRangedAnswerOption =>
            this.sessionInformationProvider.GetAnswerOptionType(this.SlideSessionModel.QuestionType) == AnswerOptionType.ShowRangedAnswerOption;

        public bool ShowTwoAnswerOptions =>
            this.sessionInformationProvider.GetAnswerOptionType(this.SlideSessionModel.QuestionType) == AnswerOptionType.ShowTwoAnswerOptions;

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
                                    new QuestionViewModel(
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
