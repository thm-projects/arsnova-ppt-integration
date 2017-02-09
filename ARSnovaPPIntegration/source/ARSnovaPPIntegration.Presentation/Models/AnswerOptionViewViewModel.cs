using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

using ARSnovaPPIntegration.Business.Model;
using ARSnovaPPIntegration.Common.Enum;
using ARSnovaPPIntegration.Presentation.Commands;
using ARSnovaPPIntegration.Presentation.Helpers;
using ARSnovaPPIntegration.Presentation.Window;

namespace ARSnovaPPIntegration.Presentation.Models
{
    public class AnswerOptionViewViewModel : BaseViewModel
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

        public AnswerOptionViewViewModel(ViewModelRequirements requirements, Guid questionId, bool isNew, SlideQuestionModel questionBeforeEdit) 
            : base(requirements)
        {
            this.questionId = questionId;
            this.isNew = isNew;
            this.questionBeforeEdit = questionBeforeEdit;

            this.InitializeWindowCommandBindings();

            this.InitAnswerOptionList();

            foreach (var answerOption in this.SlideQuestionModel.AnswerOptions)
            {
                answerOption.IsTruePropertyChangedEventHandler += this.OnIsTruePropertyChanged;
            }
        }

        public string Header => this.LocalizationService.Translate("Set the answer option(s)");

        public bool ShowGeneralAnswerOptions =>
            this.SessionInformationProvider.GetAnswerOptionType(this.SlideQuestionModel.QuestionType) == AnswerOptionType.ShowGeneralAnswerOptions;

        public bool ShowFreeTextAnswerOptions =>
            this.SessionInformationProvider.GetAnswerOptionType(this.SlideQuestionModel.QuestionType) == AnswerOptionType.ShowFreeTextAnswerOptions;

        public bool ShowGradeOrEvaluationAnswerOptions =>
            this.SessionInformationProvider.GetAnswerOptionType(this.SlideQuestionModel.QuestionType) == AnswerOptionType.ShowGradeOrEvaluationAnswerOptions;

        public bool ShowRangedAnswerOption =>
            this.SessionInformationProvider.GetAnswerOptionType(this.SlideQuestionModel.QuestionType) == AnswerOptionType.ShowRangedAnswerOption;

        public bool ShowTwoAnswerOptions =>
            this.SessionInformationProvider.GetAnswerOptionType(this.SlideQuestionModel.QuestionType) == AnswerOptionType.ShowTwoAnswerOptions;

        public bool ShowSurveyAnswerOptions =>
            this.SessionInformationProvider.GetAnswerOptionType(this.SlideQuestionModel.QuestionType) == AnswerOptionType.ShowSurveyAnswerOptions;

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
                        var answerOptionModel = answerOption;

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
                        generalAnswerOption.ObjectChangedEventHandler += delegate 
                        {
                            this.SlideQuestionModel.AnswerOptionsSet = true;
                            this.SlideQuestionModel.AnswerOptionModelChanged();
                        };

                        generalAnswerOption.IsTruePropertyChangedEventHandler += this.OnIsTruePropertyChanged;

                        this.SlideQuestionModel.AnswerOptions.Add(generalAnswerOption);
                    }
                }

                this.SlideQuestionModel.AnswerOptionAmount = value;
                this.OnPropertyChanged(nameof(this.AnswerOptions));
            }
        }

        public void OnIsTruePropertyChanged(object o, EventArgs eventArgs)
        {
            if (this.SessionInformationProvider.IsSingleChoiceQuestion(this.SlideQuestionModel.QuestionType))
            {
                // just interact if more than one answer option is selected

                // can't deselect one, just select a new one (add reset option to model)

                // just
            }
        }

        public ObservableCollection<GeneralAnswerOption> AnswerOptions
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
                var generalAnswerOption = this.SlideQuestionModel.AnswerOptions.First();
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
                this.SlideQuestionModel.AnswerOptions.First().Text = value;
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
                var rangedAnswerOption = this.SlideQuestionModel.AnswerOptions.First();
                return rangedAnswerOption?.RangedLowerLimit ?? 0;
            }
            set
            {
                var rangedAnswerOption = this.SlideQuestionModel.AnswerOptions.First();
                if (rangedAnswerOption != null)
                {
                    if (rangedAnswerOption.RangedHigherLimit >= value)
                    {
                        rangedAnswerOption.RangedLowerLimit = value;
                        if (rangedAnswerOption.RangedCorrectValue < value)
                        {
                            rangedAnswerOption.RangedCorrectValue = value;
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
                var rangedAnswerOption = this.SlideQuestionModel.AnswerOptions.First();
                return rangedAnswerOption?.RangedCorrectValue ?? 50;
            }
            set
            {
                var rangedAnswerOption = this.SlideQuestionModel.AnswerOptions.First();
                if (rangedAnswerOption != null)
                {
                    if (rangedAnswerOption.RangedLowerLimit <= value && rangedAnswerOption.RangedHigherLimit >= value)
                    {
                        rangedAnswerOption.RangedCorrectValue = value;

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

        public string CorrectValLabelText => this.LocalizationService.Translate("RangedCorrectValue value");

        public int RangedMaxValue {
            get
            {
                var rangedAnswerOption = this.SlideQuestionModel.AnswerOptions.First();
                return rangedAnswerOption?.RangedHigherLimit ?? 100;
            }
            set
            {
                var rangedAnswerOption = this.SlideQuestionModel.AnswerOptions.First();
                if (rangedAnswerOption != null)
                {
                    if (rangedAnswerOption.RangedLowerLimit <= value)
                    {
                        rangedAnswerOption.RangedHigherLimit = value;

                        if (rangedAnswerOption.RangedCorrectValue > value)
                        {
                            rangedAnswerOption.RangedCorrectValue = value;
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

        public string LabelFreeTextSettings => this.LocalizationService.Translate("Freetext settings");

        public string LabelUpperLowerCaseSetting => this.LocalizationService.Translate("Observe upper and lower case:");

        public bool UpperLowerCaseSetting
        {
            get { return this.SlideQuestionModel.AnswerOptions.First().ConfigCaseSensitive; }
            set { this.SlideQuestionModel.AnswerOptions.First().ConfigCaseSensitive = value; }
        }

        public string LabelWhiteSpacesSetting => this.LocalizationService.Translate("Observe white spaces in the answer:");

        public bool WhiteSpacesSetting
        {
            get { return this.SlideQuestionModel.AnswerOptions.First().ConfigTrimWhitespaces; }
            set { this.SlideQuestionModel.AnswerOptions.First().ConfigTrimWhitespaces = value; }
        }

        public string LabelSequenzeSetting => this.LocalizationService.Translate("Observe sequence of the words:");

        public bool SequenzeSetting
        {
            get { return this.SlideQuestionModel.AnswerOptions.First().ConfigUseKeywords; }
            set { this.SlideQuestionModel.AnswerOptions.First().ConfigUseKeywords = value; }
        }

        public string LabelPunctuationSetting => this.LocalizationService.Translate("Observe punctuation marks:");

        public bool PunctuationSetting
        {
            get { return this.SlideQuestionModel.AnswerOptions.First().ConfigUsePunctuation; }
            set { this.SlideQuestionModel.AnswerOptions.First().ConfigUsePunctuation = value; }
        }

        protected override Tuple<bool, string> Validate()
        {
            var errorString = string.Empty;

            // no text to validate in ranged or freetext answer options
            var answerOptionType =
                this.SessionInformationProvider.GetAnswerOptionType(this.SlideQuestionModel.QuestionType);
            if (answerOptionType != AnswerOptionType.ShowRangedAnswerOption
                && answerOptionType != AnswerOptionType.ShowFreeTextAnswerOptions)
            {
                foreach (var answerOption in this.AnswerOptions)
                {
                    if (answerOption.Position <= this.AnswerOptionAmount && answerOption.Text.Length <= 0)
                        errorString += $"{this.LocalizationService.Translate("The question number")} {answerOption.Position} {this.LocalizationService.Translate("has no question text set.")}{Environment.NewLine}";
                }
            }

            if (this.SessionInformationProvider.IsSingleChoiceQuestion(this.SlideQuestionModel.QuestionType)
                && this.AnswerOptions.Count(ao => ao.IsTrue) != 1)
                errorString += this.LocalizationService.Translate("There should be exactly one correct answer options.")
                               + Environment.NewLine;

            if (this.SessionInformationProvider.IsMultipleChoiceQuestion(this.SlideQuestionModel.QuestionType)
                && (this.AnswerOptions.Count(ao => ao.IsTrue) < 2))
                errorString += this.LocalizationService.Translate("There must be more than one correct answer options.")
                               + Environment.NewLine;

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
                                        this.OnPropertyChanged(nameof(this.SlideSessionModel.Questions));
                                    }
                                    else
                                    {
                                        this.SlideQuestionModel = this.questionBeforeEdit;
                                        this.OnPropertyChanged(nameof(this.SlideSessionModel.Questions));
                                    }

                                    // don't save presentation on cancel (no changes here)
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
                                var validationResult = this.Validate();

                                if (validationResult.Item1)
                                {
                                    this.SlideQuestionModel.QuestionTypeText = this.QuestionTypeTranslator.TranslateQuestionType(this.SlideQuestionModel.QuestionType);

                                    var questionInfoSlide = SlideTracker.GetSlideById(this.SlideQuestionModel.QuestionInfoSlideId);
                                    if (questionInfoSlide != null)
                                    {
                                        var questionInfoSlideNumber = questionInfoSlide.SlideNumber;
                                        this.SlideQuestionModel.SlideNumbers = $"{questionInfoSlideNumber} - {questionInfoSlideNumber + 2}";
                                    }

                                    this.AddSessionToSlides(this.SlideQuestionModel);
                                    PresentationInformationStore.StoreSlideSessionModel(this.SlideSessionModel);
                                }
                                else
                                {
                                    this.DisplayFailedValidationResults(validationResult.Item2);
                                }
                            },
                            (e, o) => o.CanExecute = true)
                    });
        }

        private void InitAnswerOptionList()
        {
            if (this.AnswerOptions == null 
                || this.AnswerOptions.Count <= 0
                || this.SlideQuestionModel.QuestionInitType != this.SlideQuestionModel.QuestionType)
            {
                if (this.ShowGeneralAnswerOptions || this.ShowSurveyAnswerOptions)
                {
                    this.SlideQuestionModel.AnswerOptions = new ObservableCollection<GeneralAnswerOption>();

                    for (var i = 1; i <= this.AnswerOptionAmount; i++)
                    {
                        this.SlideQuestionModel.AnswerOptions.Add(this.CreateGeneralAnswerOption(i));
                    }
                }

                if (this.ShowFreeTextAnswerOptions)
                {
                    this.SlideQuestionModel.AnswerOptions = new ObservableCollection<GeneralAnswerOption>();

                    this.SlideQuestionModel.AnswerOptions.Add(this.CreateGeneralAnswerOption());
                }

                if (this.ShowGradeOrEvaluationAnswerOptions)
                {
                    this.SlideQuestionModel.AnswerOptions = new ObservableCollection<GeneralAnswerOption>();

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
                    this.SlideQuestionModel.AnswerOptions = new ObservableCollection<GeneralAnswerOption>
                                                           {
                                                               new GeneralAnswerOption
                                                               {
                                                                   RangedLowerLimit = 0,
                                                                   RangedCorrectValue = 50,
                                                                   RangedHigherLimit = 100,
                                                                   AnswerOptionType = AnswerOptionType.ShowRangedAnswerOption
                                                               }
                                                           };
                }

                if (this.ShowTwoAnswerOptions)
                {
                    this.SlideQuestionModel.AnswerOptions = new ObservableCollection<GeneralAnswerOption>();

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

                this.SlideQuestionModel.QuestionInitType = this.SlideQuestionModel.QuestionType;

                // default init doesn't count as manipulated
                this.SlideQuestionModel.AnswerOptionsSet = false;
            } 
        }

        private GeneralAnswerOption CreateGeneralAnswerOption(int position = 1, string text = "", bool isTrue = false)
        {
            var generalAnswerOption =  new GeneralAnswerOption
            {
                Position = position,
                Text = text,
                IsTrue = isTrue
            };

            generalAnswerOption.ObjectChangedEventHandler += delegate 
            {
                this.SlideQuestionModel.AnswerOptionsSet = true;
                this.SlideQuestionModel.AnswerOptionModelChanged();
            };

            return generalAnswerOption;
        }
    }
}
