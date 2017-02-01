using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;

using ARSnovaPPIntegration.Business.Model;
using ARSnovaPPIntegration.Common.Enum;
using ARSnovaPPIntegration.Presentation.Commands;
using ARSnovaPPIntegration.Presentation.Helpers;
using ARSnovaPPIntegration.Presentation.Window;

namespace ARSnovaPPIntegration.Presentation.Models
{
    public class QuestionViewViewModel : BaseViewModel
    {
        private readonly Guid questionId;

        private readonly bool isNew;

        private readonly SlideQuestionModel questionBeforeEdit;

        private SlideQuestionModel SlideQuestionModel
        {
            get { return this.SlideSessionModel.Questions.FirstOrDefault(q => q.Id == this.questionId); }
            set
            {
                this.SlideSessionModel.Questions.Remove(this.SlideSessionModel.Questions.FirstOrDefault(q => q.Id == this.questionId));
                this.SlideSessionModel.Questions.Add(value);
            }
        }

        public QuestionViewViewModel(ViewModelRequirements requirements, Guid questionId, bool isNew, SlideQuestionModel questionBeforeEdit = null)
            : base(requirements)
        {
            this.questionId = questionId;
            this.isNew = isNew;

            this.questionBeforeEdit = questionBeforeEdit ?? CopyHelper.CopySlideQuestionModel(this.SlideQuestionModel);

            this.InitializeWindowCommandBindings();

            this.QuestionTypes = this.SlideSessionModel.SessionType == SessionType.ArsnovaClick
                ? this.SessionInformationProvider.GetAvailableQuestionsClick()
                : this.SessionInformationProvider.GetAvailableQuestionsVoting();

            this.ExcelChartTypes = this.SessionInformationProvider.GetExcelChartTypes();
        }

        public string Header => this.LocalizationService.Translate("Set question");

        public string Text => this.LocalizationService.Translate("Choose a question type and enter the question text:");

        public string LabelSelectSessionType => this.LocalizationService.Translate("Sessiontype");

        public bool IsArsnovaClickQuestion
            => this.SessionInformationProvider.IsClickQuestion(this.SlideQuestionModel.QuestionType);

        public string Countdown
        {
            get { return this.SlideQuestionModel.Countdown.ToString(); }
            set { this.SlideQuestionModel.Countdown = Convert.ToInt32(value); }
        }

        public string SetCountdownLabel => this.LocalizationService.Translate("Countdown");

        public string LabelSelectChartType => this.LocalizationService.Translate("Choose results chart type");

        public List<ExcelChartType> ExcelChartTypes { get; set; }

        public ExcelChartType ChartType
        {
            get
            {
                return this.ExcelChartTypes.FirstOrDefault(ect => ect.ChartTypeEnum == this.SlideQuestionModel.ChartType);
            }
            set { this.SlideQuestionModel.ChartType = value.ChartTypeEnum; }
        }

        public List<QuestionType> QuestionTypes { get; set; }

        public QuestionTypeEnum QuestionType
        {
            get
            {
                if (this.SlideQuestionModel.QuestionType != 0)
                {
                    return this.SlideQuestionModel.QuestionType;
                }
                else
                {
                    return this.SlideSessionModel.SessionType == SessionType.ArsnovaClick
                     ? QuestionTypeEnum.SingleChoiceClick
                     : QuestionTypeEnum.SingleChoiceVoting;
                }
            }
            set
            {
                var oldQuestionType = this.SlideQuestionModel.QuestionType;

                if (oldQuestionType == value)
                    return;

                if (this.SlideQuestionModel.AnswerOptionsSet)
                {
                    var reset = PopUpWindow.ConfirmationWindow(
                        this.LocalizationService.Translate("Reset"),
                        this.LocalizationService.Translate(
                                "If this value is changed, other Session-Properties like the answer options or the question type will be reseted. Do you want to continue?"));

                    if (reset)
                    {
                        this.SlideQuestionModel.QuestionType = value;
                        this.SlideQuestionModel.QuestionTypeSet = true;
                        this.SlideQuestionModel.AnswerOptions = null;
                        this.SlideQuestionModel.AnswerOptionsSet = false;
                    }
                    else
                    {
                        // change the value back right after the current context operation is done (the change event of the selectlist needs to finish first)
                        Dispatcher.CurrentDispatcher.BeginInvoke(
                                new Action(() =>
                                {
                                    this.SlideQuestionModel.QuestionType = oldQuestionType;
                                    this.OnPropertyChanged("QuestionType");
                                }),
                                DispatcherPriority.ContextIdle,
                                null
                            );
                    }
                }
                else
                {
                    this.SlideQuestionModel.QuestionType = value;
                    this.SlideQuestionModel.QuestionTypeSet = true;
                }
            }
        }

        public string QuestionText
        {
            get { return this.SlideQuestionModel.QuestionText; }
            set { this.SlideQuestionModel.QuestionText = value; }
        }

        protected override Tuple<bool, string> Validate()
        {
            var errorString = string.Empty;

            if (this.SlideQuestionModel.Countdown < 5)
                errorString += this.LocalizationService.Translate("The countdown must have a value higher than 10 seconds.") + Environment.NewLine;

            if (string.IsNullOrEmpty(this.QuestionText) || this.QuestionText.Length == 0)
                errorString += this.LocalizationService.Translate("There is no question text set.") + Environment.NewLine;

            return new Tuple<bool, string>(errorString == string.Empty, errorString);
        }

        private void InitializeWindowCommandBindings()
        {
            this.WindowCommandBindings.AddRange(
                    new List<CommandBinding>
                    {
                        new CommandBinding(
                            NavigationButtonCommands.Cancel,
                            (e, o) =>
                            {
                                var delete = PopUpWindow.ConfirmationWindow(
                                    this.LocalizationService.Translate("Cancel"),
                                    this.LocalizationService.Translate("Do you really want to cancel and rewind all current changes?"));

                                if (delete)
                                {
                                    if (this.isNew)
                                    {
                                        this.SlideSessionModel.Questions.Remove(this.SlideQuestionModel);
                                    }
                                    else
                                    {
                                        this.SlideQuestionModel = this.questionBeforeEdit;
                                    }

                                    // don't save presentation on cancel (no changes here)
                                    this.ViewPresenter.CloseWithoutPrompt();
                                }
                            },
                            (e, o) => o.CanExecute = true),
                        new CommandBinding(
                            NavigationButtonCommands.Forward,
                            (e, o) =>
                            {
                                var validationResult = this.Validate();

                                if (validationResult.Item1)
                                {
                                    this.ViewPresenter.Show(
                                        new AnswerOptionViewViewModel(this.GetViewModelRequirements(), this.questionId, this.isNew, this.questionBeforeEdit));
                                }
                                else
                                {
                                    this.DisplayFailedValidationResults(validationResult.Item2);
                                }
                            },
                            (e, o) => o.CanExecute = true)
                    });
        }
    }
}
