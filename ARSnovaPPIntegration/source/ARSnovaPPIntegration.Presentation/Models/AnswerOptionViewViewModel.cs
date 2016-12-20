using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

using Microsoft.Practices.ServiceLocation;

using ARSnovaPPIntegration.Business.Contract;
using ARSnovaPPIntegration.Business.Model;
using ARSnovaPPIntegration.Common.Enum;
using ARSnovaPPIntegration.Presentation.Commands;
using ARSnovaPPIntegration.Presentation.Window;

namespace ARSnovaPPIntegration.Presentation.Models
{
    public class AnswerOptionViewViewModel : BaseViewModel
    {
        private readonly ISessionInformationProvider sessionInformationProvider;

        private readonly Guid questionId;

        private bool isNew;

        private SlideQuestionModel questionBeforeEdit;

        private SlideQuestionModel SlideQuestionModel
        {
            get { return this.SlideSessionModel.Questions.FirstOrDefault(q => q.Id == this.questionId); }
            set
            {
                this.SlideSessionModel.Questions.Remove(this.SlideSessionModel.Questions.FirstOrDefault(q => q.Id == this.questionId));
                this.SlideSessionModel.Questions.Add(value);
            }
        }

        public AnswerOptionViewViewModel(ViewModelRequirements requirements, Guid questionId, bool isNew, SlideQuestionModel questionBeforeEdit) 
            : base(requirements)
        {
            this.questionId = questionId;
            this.isNew = isNew;
            this.questionBeforeEdit = questionBeforeEdit;

            this.InitializeWindowCommandBindings();

            this.sessionInformationProvider = ServiceLocator.Current.GetInstance<ISessionInformationProvider>();

            this.InitAnswerOptionList();
        }

        public string Header => this.LocalizationService.Translate("Set the answer option(s)");

        public bool ShowGeneralAnswerOptions =>
            this.sessionInformationProvider.GetAnswerOptionType(this.SlideQuestionModel.QuestionType) == AnswerOptionType.ShowGeneralAnswerOptions;

        public bool ShowFreeTextAnswerOptions =>
            this.sessionInformationProvider.GetAnswerOptionType(this.SlideQuestionModel.QuestionType) == AnswerOptionType.ShowFreeTextAnswerOptions;

        public bool ShowGradeOrEvaluationAnswerOptions =>
            this.sessionInformationProvider.GetAnswerOptionType(this.SlideQuestionModel.QuestionType) == AnswerOptionType.ShowGradeOrEvaluationAnswerOptions;

        public bool ShowRangedAnswerOption =>
            this.sessionInformationProvider.GetAnswerOptionType(this.SlideQuestionModel.QuestionType) == AnswerOptionType.ShowRangedAnswerOption;

        public bool ShowTwoAnswerOptions =>
            this.sessionInformationProvider.GetAnswerOptionType(this.SlideQuestionModel.QuestionType) == AnswerOptionType.ShowTwoAnswerOptions;

        public List<int> PossibleAnswerOptionsAmount =>
            this.SlideSessionModel.SessionType == SessionType.ArsnovaClick
                ? new List<int>
                  {
                      1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24
                  }
                : new List<int>
                  {
                      1, 2, 3, 4, 5, 6, 7, 8
                  };

        public int AnswerOptionAmount
        {
            get { return this.SlideQuestionModel.AnswerOptionAmount; }
            set
            {
                if (value < this.SlideQuestionModel.AnswerOptionAmount)
                {
                    var currentAnswerOptionList = this.SlideQuestionModel.AnswerOptions.ToList();

                    foreach (var answerOption in currentAnswerOptionList)
                    {
                        var answerOptionModel = answerOption as GeneralAnswerOption;

                        if (answerOptionModel == null)
                        {
                            throw new ArgumentException("Unexpected answer option type. Watch stack trace for further informations.");
                        }

                        if (answerOptionModel.Position > value)
                        {
                            this.SlideQuestionModel.AnswerOptions.Remove(answerOption);
                        }
                    }
                }

                if (value > this.SlideQuestionModel.AnswerOptionAmount)
                {
                    for (int i = this.SlideQuestionModel.AnswerOptionAmount + 1; i <= value; i++)
                    {
                        var generalAnswerOption = new GeneralAnswerOption
                                                  {
                                                      Position = i,
                                                      Text = string.Empty,
                                                      IsTrue = false
                        };
                        generalAnswerOption.ObjectChangedEventHandler += delegate {
                            this.SlideQuestionModel.AnswerOptionsSet = true;
                        };

                        this.SlideQuestionModel.AnswerOptions.Add(generalAnswerOption);
                    }
                }

                this.SlideQuestionModel.AnswerOptionAmount = value;
                this.OnPropertyChanged(nameof(this.AnswerOptions));
            }
        }

        public ObservableCollection<object> AnswerOptions
        {
            get { return this.SlideQuestionModel.AnswerOptions; }
            set
            {
                this.SlideQuestionModel.AnswerOptions = value;
            }
        }

        public string FreeTextAnswerOption
        {
            get
            {
                var generalAnswerOption = this.SlideQuestionModel.AnswerOptions.First() as GeneralAnswerOption;
                if (generalAnswerOption != null)
                {
                    return generalAnswerOption.Text;
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                ((GeneralAnswerOption)this.SlideQuestionModel.AnswerOptions.First()).Text = value;
            }
        }

        public string SelectAnswerOptionAmountText => this.LocalizationService.Translate("Answer option amount:");

        public string GridHeaderPosition => this.LocalizationService.Translate("Position");

        public string GridHeaderText => this.LocalizationService.Translate("Text");

        public string GridHeaderIsTrue => this.LocalizationService.Translate("Is true");

        public string GradeOrEvaluationAnswerOptionsInfoText => this.LocalizationService.Translate("Grade- and evaluation questions own predefined answer options. No manipulation possible.");

        public int RangedMinValue
        {
            get
            {
                var rangedAnswerOption = this.SlideQuestionModel.AnswerOptions.First() as RangedAnswerOption;
                return rangedAnswerOption?.LowerLimit ?? 0;
            }
            set
            {
                var rangedAnswerOption = this.SlideQuestionModel.AnswerOptions.First() as RangedAnswerOption;
                if (rangedAnswerOption != null)
                {
                    if (rangedAnswerOption.HigherLimit >= value)
                    {
                        rangedAnswerOption.LowerLimit = value;
                        if (rangedAnswerOption.Correct < value)
                        {
                            rangedAnswerOption.Correct = value;
                            this.OnPropertyChanged(nameof(this.RangedCorrectValue));
                            this.OnPropertyChanged(nameof(this.RangedCorrectValueString));
                        }
                    }
                }  
            }
        }

        public string RangedMinValueString
        {
            get { return this.RangedMinValue.ToString(); } 
            set
            {
                var numericVal = int.Parse(value);
                this.RangedMinValue = numericVal;
                this.OnPropertyChanged(nameof(this.RangedMinValueString));
            }
        }

        public string MinValLabelText => this.LocalizationService.Translate("Minimum range");

        public int RangedCorrectValue {
            get
            {
                var rangedAnswerOption = this.SlideQuestionModel.AnswerOptions.First() as RangedAnswerOption;
                return rangedAnswerOption?.Correct ?? 50;
            }
            set
            {
                var rangedAnswerOption = this.SlideQuestionModel.AnswerOptions.First() as RangedAnswerOption;
                if (rangedAnswerOption != null)
                {
                    if (rangedAnswerOption.LowerLimit <= value && rangedAnswerOption.HigherLimit >= value)
                    {
                        rangedAnswerOption.Correct = value;

                        this.OnPropertyChanged(nameof(this.RangedCorrectValue));
                        this.OnPropertyChanged(nameof(this.RangedCorrectValueString));
                    }
                }  
            }
        }

        public string RangedCorrectValueString {
            get { return this.RangedCorrectValue.ToString(); }
            set
            {
                var numericVal = int.Parse(value);
                this.RangedCorrectValue = numericVal;
                this.OnPropertyChanged(nameof(this.RangedCorrectValueString));
            }
        }

        public string CorrectValLabelText => this.LocalizationService.Translate("Correct value");

        public int RangedMaxValue {
            get
            {
                var rangedAnswerOption = this.SlideQuestionModel.AnswerOptions.First() as RangedAnswerOption;
                return rangedAnswerOption?.HigherLimit ?? 100;
            }
            set
            {
                var rangedAnswerOption = this.SlideQuestionModel.AnswerOptions.First() as RangedAnswerOption;
                if (rangedAnswerOption != null)
                {
                    if (rangedAnswerOption.LowerLimit <= value)
                    {
                        rangedAnswerOption.HigherLimit = value;

                        if (rangedAnswerOption.Correct > value)
                        {
                            rangedAnswerOption.Correct = value;
                            this.OnPropertyChanged(nameof(this.RangedCorrectValue));
                            this.OnPropertyChanged(nameof(this.RangedCorrectValueString));
                        }
                    }
                }
            }
        }

        public string RangedMaxValueString {
            get { return this.RangedMaxValue.ToString(); }
            set
            {
                var numericVal = int.Parse(value);
                this.RangedMaxValue = numericVal;
                this.OnPropertyChanged(nameof(this.RangedMaxValueString));
            }
        }

        public string MaxValLabelText => this.LocalizationService.Translate("Maximum range");

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

                                    this.ViewPresenter.CloseWithoutPrompt();
                                }
                            },
                            (e, o) => o.CanExecute = true),
                        new CommandBinding(
                            NavigationButtonCommands.Back,
                            (e, o) =>
                            {
                                this.ViewPresenter.Show(
                                        new QuestionViewViewModel(
                                            this.GetViewModelRequirements(), this.questionId, this.isNew,
                                            this.questionBeforeEdit));
                            },
                            (e, o) => o.CanExecute = true),
                        new CommandBinding(
                            NavigationButtonCommands.Finish,
                            (e, o) =>
                            {
                                // TODO validate this question
                                this.ViewPresenter.CloseWithoutPrompt();
                            },
                            (e, o) => o.CanExecute = true)
                    });
        }

        private void InitAnswerOptionList()
        {
            if (this.AnswerOptions == null 
                || this.SlideQuestionModel.AnswerOptionInitType != this.SlideQuestionModel.AnswerOptionType 
                || this.AnswerOptions.Count != this.AnswerOptionAmount)
            {
                if (this.ShowGeneralAnswerOptions)
                {
                    this.SlideQuestionModel.AnswerOptions = new ObservableCollection<object>();

                    for (var i = 1; i <= this.AnswerOptionAmount; i++)
                    {
                        this.SlideQuestionModel.AnswerOptions.Add(this.CreateGeneralAnswerOption(i));
                    }
                }

                if (this.ShowFreeTextAnswerOptions)
                {
                    this.SlideQuestionModel.AnswerOptions = new ObservableCollection<object>();

                    this.SlideQuestionModel.AnswerOptions.Add(this.CreateGeneralAnswerOption());
                }

                if (this.ShowGradeOrEvaluationAnswerOptions)
                {
                    this.SlideQuestionModel.AnswerOptions = new ObservableCollection<object>();

                    if (this.SlideQuestionModel.QuestionType == QuestionTypeEnum.EvaluationVoting)
                    {
                        this.SlideQuestionModel.AnswerOptions.Add(
                            this.CreateGeneralAnswerOption(1, this.LocalizationService.Translate("totally agree")));
                        this.SlideQuestionModel.AnswerOptions.Add(
                            this.CreateGeneralAnswerOption(2, this.LocalizationService.Translate("rather applies")));
                        this.SlideQuestionModel.AnswerOptions.Add(
                            this.CreateGeneralAnswerOption(3, this.LocalizationService.Translate("neither")));
                        this.SlideQuestionModel.AnswerOptions.Add(
                            this.CreateGeneralAnswerOption(4, this.LocalizationService.Translate("does not apply")));
                        this.SlideQuestionModel.AnswerOptions.Add(
                            this.CreateGeneralAnswerOption(5, this.LocalizationService.Translate("strongly disagree")));
                    }

                    if (this.SlideQuestionModel.QuestionType == QuestionTypeEnum.GradsVoting)
                    {
                        this.SlideQuestionModel.AnswerOptions.Add(
                            this.CreateGeneralAnswerOption(1, this.LocalizationService.Translate("very good")));
                        this.SlideQuestionModel.AnswerOptions.Add(
                            this.CreateGeneralAnswerOption(2, this.LocalizationService.Translate("good")));
                        this.SlideQuestionModel.AnswerOptions.Add(
                            this.CreateGeneralAnswerOption(3, this.LocalizationService.Translate("satisfying")));
                        this.SlideQuestionModel.AnswerOptions.Add(
                            this.CreateGeneralAnswerOption(4, this.LocalizationService.Translate("sufficient")));
                        this.SlideQuestionModel.AnswerOptions.Add(
                            this.CreateGeneralAnswerOption(5, this.LocalizationService.Translate("inadequate")));
                        this.SlideQuestionModel.AnswerOptions.Add(
                            this.CreateGeneralAnswerOption(6, this.LocalizationService.Translate("insufficient")));
                    }
                }

                if (this.ShowRangedAnswerOption)
                {
                    this.SlideQuestionModel.AnswerOptions = new ObservableCollection<object>
                                                           {
                                                               new RangedAnswerOption
                                                               {
                                                                   LowerLimit = 0,
                                                                   Correct = 50,
                                                                   HigherLimit = 100
                                                               }
                                                           };
                }

                if (this.ShowTwoAnswerOptions)
                {
                    this.SlideQuestionModel.AnswerOptions = new ObservableCollection<object>();

                    if (this.SlideQuestionModel.QuestionType == QuestionTypeEnum.YesNoClick
                        || this.SlideQuestionModel.QuestionType == QuestionTypeEnum.YesNoVoting)
                    {
                        this.SlideQuestionModel.AnswerOptions.Add(
                                this.CreateGeneralAnswerOption(1, this.LocalizationService.Translate("Yes"), true));
                        this.SlideQuestionModel.AnswerOptions.Add(
                                this.CreateGeneralAnswerOption(2, this.LocalizationService.Translate("No")));
                    }

                    if (this.SlideQuestionModel.QuestionType == QuestionTypeEnum.TrueFalseClick)
                    {
                        this.SlideQuestionModel.AnswerOptions.Add(
                                this.CreateGeneralAnswerOption( 1, this.LocalizationService.Translate("True"), true));
                        this.SlideQuestionModel.AnswerOptions.Add(
                                this.CreateGeneralAnswerOption(2, this.LocalizationService.Translate("False")));
                    }
                }

                this.SlideQuestionModel.AnswerOptionInitType = this.SlideQuestionModel.AnswerOptionType;

                // default init doesn't count as manipulated
                this.SlideQuestionModel.AnswerOptionsSet = false;
            } 
        }

        private GeneralAnswerOption CreateGeneralAnswerOption(int position = 0, string text = "", bool isTrue = false)
        {
            var generalAnswerOption =  new GeneralAnswerOption
            {
                Position = position,
                Text = text,
                IsTrue = isTrue
            };

            generalAnswerOption.ObjectChangedEventHandler += delegate {
                                                                 this.SlideQuestionModel.AnswerOptionsSet = true;
                                                             };

            return generalAnswerOption;
        }
    }
}
