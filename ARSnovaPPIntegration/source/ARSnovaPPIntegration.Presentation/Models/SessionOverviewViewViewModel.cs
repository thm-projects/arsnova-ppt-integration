using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

using ARSnovaPPIntegration.Business.Model;
using ARSnovaPPIntegration.Common.Enum;
using ARSnovaPPIntegration.Presentation.Commands;
using ARSnovaPPIntegration.Presentation.Window;

namespace ARSnovaPPIntegration.Presentation.Models
{
    public class SessionOverviewViewViewModel : BaseViewModel
    {
        public SessionOverviewViewViewModel(ViewModelRequirements requirements) : base(requirements)
        {
            this.InitializeWindowCommandBindings();
        }

        public SlideQuestionModel SelectedSlideQuestionModel { get; set; }

        public ObservableCollection<SlideQuestionModel> Questions => this.SlideSessionModel.Questions;

        public string Header => this.LocalizationService.Translate("Question overview");

        public string GridHeaderSlideNumber => this.LocalizationService.Translate("Slide");

        public string GridHeaderQuestionTypeText => this.LocalizationService.Translate("Type");

        public string GridHeaderQuestionText => this.LocalizationService.Translate("Question");

        private void InitializeWindowCommandBindings()
        {
            this.WindowCommandBindings.AddRange(
                    new List<CommandBinding>
                    {
                        new CommandBinding(
                            NavigationButtonCommands.Finish,
                            (e, o) =>
                            {
                                this.AddSessionToSlides();
                            },
                            (e, o) => o.CanExecute = true),
                        new CommandBinding(
                            NavigationButtonCommands.Back,
                            (e, o) =>
                            {
                                this.ViewPresenter.Show(
                                    new SelectArsnovaTypeViewViewModel(this.GetViewModelRequirements()));
                            },
                            (e, o) => o.CanExecute = true),
                        new CommandBinding(
                            NavigationButtonCommands.New,
                            (e, o) =>
                            {
                                var newQuestion = new SlideQuestionModel(this.QuestionTypeTranslator)
                                                  {
                                                      QuestionType = this.SlideSessionModel.SessionType
                                                                     == SessionType.ArsnovaClick
                                                                         ? QuestionTypeEnum.SingleChoiceClick
                                                                         : QuestionTypeEnum.SingleChoiceVoting
                                                  };


                                this.SlideSessionModel.Questions.Add(newQuestion);

                                this.OpenQuestionEditDialog(newQuestion.Id, true);
                            },
                            (e, o) => o.CanExecute = true),
                        new CommandBinding(
                            NavigationButtonCommands.Edit,
                            (e, o) =>
                            {
                                this.OpenQuestionEditDialog(this.SelectedSlideQuestionModel.Id, false);
                            },
                            (e, o) => o.CanExecute = this.SelectedSlideQuestionModel != null),
                        new CommandBinding(
                            NavigationButtonCommands.Delete,
                            (e, o) =>
                            {
                                var questionText =
                                    $"{this.LocalizationService.Translate("Do you really want to delete this question?")}{Environment.NewLine}{Environment.NewLine}";
                                questionText += this.SelectedSlideQuestionModel.QuestionText;

                                var deleteQuestion = PopUpWindow.ConfirmationWindow(
                                    this.LocalizationService.Translate("Delete"),
                                    questionText);

                                if (deleteQuestion)
                                {
                                    var questionModel = this.SelectedSlideQuestionModel;
                                    this.SelectedSlideQuestionModel = null;
                                    this.SlideSessionModel.Questions.Remove(questionModel);
                                }
                            },
                            (e, o) => o.CanExecute = this.SelectedSlideQuestionModel != null)
                    });
        }

        private void OpenQuestionEditDialog(Guid questionId, bool isNew)
        {
            this.ViewPresenter.ShowInNewWindow(
                new QuestionViewViewModel(this.GetViewModelRequirements(), questionId, isNew));
        }
    }
}
