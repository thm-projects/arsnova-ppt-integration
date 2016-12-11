using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

            this.InitAnswerOptionList();
        }

        public string Header => this.LocalizationService.Translate("Set the answer option(s)");

        public bool ShowGeneralAnswerOptions =>
            this.sessionInformationProvider.GetAnswerOptionType(this.SlideSessionModel.QuestionType) == AnswerOptionType.ShowGeneralAnswerOptions;

        public bool ShowFreeTextAnswerOptions =>
            this.sessionInformationProvider.GetAnswerOptionType(this.SlideSessionModel.QuestionType) == AnswerOptionType.ShowFreeTextAnswerOptions;

        public bool ShowGradeOrEvaluationAnswerOptions =>
            this.sessionInformationProvider.GetAnswerOptionType(this.SlideSessionModel.QuestionType) == AnswerOptionType.ShowGradeOrEvaluationAnswerOptions;

        public bool ShowRangedAnswerOption =>
            this.sessionInformationProvider.GetAnswerOptionType(this.SlideSessionModel.QuestionType) == AnswerOptionType.ShowRangedAnswerOption;

        public bool ShowTwoAnswerOptions =>
            this.sessionInformationProvider.GetAnswerOptionType(this.SlideSessionModel.QuestionType) == AnswerOptionType.ShowTwoAnswerOptions;

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
            get { return this.SlideSessionModel.AnswerOptionAmount; }
            set
            {
                if (value < this.SlideSessionModel.AnswerOptionAmount)
                {
                    var currentAnswerOptionList = this.SlideSessionModel.AnswerOptions.ToList();

                    foreach (var answerOption in currentAnswerOptionList)
                    {
                        var answerOptionModel = answerOption as GeneralAnswerOption;

                        if (answerOptionModel == null)
                        {
                            throw new ArgumentException("Unexpected answer option type. Watch stack trace for further informations.");
                        }

                        if (answerOptionModel.Position > value)
                        {
                            this.SlideSessionModel.AnswerOptions.Remove(answerOption);
                        }
                    }
                }

                if (value > this.SlideSessionModel.AnswerOptionAmount)
                {
                    for (int i = this.SlideSessionModel.AnswerOptionAmount + 1; i <= value; i++)
                    {
                        this.SlideSessionModel.AnswerOptions.Add(
                            new GeneralAnswerOption
                            {
                                Position = i,
                                Text = string.Empty,
                                IsTrue = false
                            });
                    }
                }

                this.SlideSessionModel.AnswerOptionAmount = value;
                this.SlideSessionModel.AnswerOptionsSet = true;
                this.OnPropertyChanged(nameof(this.AnswerOptions));
            }
        }

        public ObservableCollection<object> AnswerOptions
        {
            get { return this.SlideSessionModel.AnswerOptions; }
            set { this.SlideSessionModel.AnswerOptions = value; }
        }

        public string FreeTextAnswerOption
        {
            get
            {
                var generalAnswerOption = this.SlideSessionModel.AnswerOptions.First() as GeneralAnswerOption;
                if (generalAnswerOption != null)
                {
                    return generalAnswerOption.Text;
                }
                else
                {
                    return string.Empty;
                }
            }
            set { ((GeneralAnswerOption)this.SlideSessionModel.AnswerOptions.First()).Text = value; }
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
                var rangedAnswerOption = this.TryGetRangedAnswerOption();
                return rangedAnswerOption.LowerLimit;
            }
            set
            {
                var rangedAnswerOption = this.TryGetRangedAnswerOption();
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

                this.OnPropertyChanged(nameof(this.RangedMinValue));
                this.OnPropertyChanged(nameof(this.RangedMinValueString));
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
                var rangedAnswerOption = this.TryGetRangedAnswerOption();
                return rangedAnswerOption.Correct;
            }
            set
            {
                var rangedAnswerOption = this.TryGetRangedAnswerOption();
                if (rangedAnswerOption.LowerLimit <= value && rangedAnswerOption.HigherLimit >= value)
                {
                    rangedAnswerOption.Correct = value;
                }
                
                this.OnPropertyChanged(nameof(this.RangedCorrectValue));
                this.OnPropertyChanged(nameof(this.RangedCorrectValueString));
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
                var rangedAnswerOption = this.TryGetRangedAnswerOption();
                return rangedAnswerOption.HigherLimit;
            }
            set
            {
                var rangedAnswerOption = this.TryGetRangedAnswerOption();
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
                
                this.OnPropertyChanged(nameof(this.RangedMaxValue));
                this.OnPropertyChanged(nameof(this.RangedMaxValueString));
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
                            NavigationButtonCommands.Finish,
                            (e, o) =>
                            {
                                // TODO setup finished, call business logik -> create / change session online (api service) (NewSession in model), manipulate / edit / create slide and fill up with content
                            },
                            (e, o) => o.CanExecute = true)
                    });
        }

        private void InitAnswerOptionList()
        {
            if (this.AnswerOptions == null 
                || this.SlideSessionModel.AnswerOptionInitType != this.SlideSessionModel.AnswerOptionType 
                || this.AnswerOptions.Count != this.AnswerOptionAmount)
            {
                if (this.ShowGeneralAnswerOptions)
                {
                    this.SlideSessionModel.AnswerOptions = new ObservableCollection<object>();

                    for (var i = 1; i <= this.AnswerOptionAmount; i++)
                    {
                        this.SlideSessionModel.AnswerOptions.Add(this.CreateGeneralAnswerOption(i));
                    }
                }

                if (this.ShowFreeTextAnswerOptions)
                {
                    this.SlideSessionModel.AnswerOptions = new ObservableCollection<object>();

                    this.SlideSessionModel.AnswerOptions.Add(this.CreateGeneralAnswerOption());
                }

                if (this.ShowGradeOrEvaluationAnswerOptions)
                {
                    this.SlideSessionModel.AnswerOptions = new ObservableCollection<object>();

                    if (this.SlideSessionModel.QuestionType == QuestionTypeEnum.EvaluationVoting)
                    {
                        this.SlideSessionModel.AnswerOptions.Add(
                            this.CreateGeneralAnswerOption(1, this.LocalizationService.Translate("totally agree")));
                        this.SlideSessionModel.AnswerOptions.Add(
                            this.CreateGeneralAnswerOption(2, this.LocalizationService.Translate("rather applies")));
                        this.SlideSessionModel.AnswerOptions.Add(
                            this.CreateGeneralAnswerOption(3, this.LocalizationService.Translate("neither")));
                        this.SlideSessionModel.AnswerOptions.Add(
                            this.CreateGeneralAnswerOption(4, this.LocalizationService.Translate("does not apply")));
                        this.SlideSessionModel.AnswerOptions.Add(
                            this.CreateGeneralAnswerOption(5, this.LocalizationService.Translate("strongly disagree")));
                    }

                    if (this.SlideSessionModel.QuestionType == QuestionTypeEnum.GradsVoting)
                    {
                        this.SlideSessionModel.AnswerOptions.Add(
                            this.CreateGeneralAnswerOption(1, this.LocalizationService.Translate("very good")));
                        this.SlideSessionModel.AnswerOptions.Add(
                            this.CreateGeneralAnswerOption(2, this.LocalizationService.Translate("good")));
                        this.SlideSessionModel.AnswerOptions.Add(
                            this.CreateGeneralAnswerOption(3, this.LocalizationService.Translate("satisfying")));
                        this.SlideSessionModel.AnswerOptions.Add(
                            this.CreateGeneralAnswerOption(4, this.LocalizationService.Translate("sufficient")));
                        this.SlideSessionModel.AnswerOptions.Add(
                            this.CreateGeneralAnswerOption(5, this.LocalizationService.Translate("inadequate")));
                        this.SlideSessionModel.AnswerOptions.Add(
                            this.CreateGeneralAnswerOption(6, this.LocalizationService.Translate("insufficient")));
                    } 
                }

                if (this.ShowRangedAnswerOption)
                {
                    this.SlideSessionModel.AnswerOptions = new ObservableCollection<object>
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

                }

                this.SlideSessionModel.AnswerOptionInitType = this.SlideSessionModel.AnswerOptionType;
            } 
        }

        private GeneralAnswerOption CreateGeneralAnswerOption(int position = 0, string text = "", bool isTrue = false)
        {
            return new GeneralAnswerOption
            {
                Position = position,
                Text = text,
                IsTrue = isTrue
            };
        }

        private RangedAnswerOption TryGetRangedAnswerOption()
        {
            var rangedAnswerOption = this.SlideSessionModel.AnswerOptions.First() as RangedAnswerOption;
            if (rangedAnswerOption == null)
            {
                throw new ArgumentException("Unexpected answer option type. Watch stack trace for further informations.");
            }

            return rangedAnswerOption;
        }
    }
}
