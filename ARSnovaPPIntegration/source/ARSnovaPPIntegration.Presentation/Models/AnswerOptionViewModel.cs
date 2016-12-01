using System;
using System.Collections.Generic;
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

        public bool ShowEvaluationAnswerOptions =>
            this.sessionInformationProvider.GetAnswerOptionType(this.SlideSessionModel.QuestionType) == AnswerOptionType.ShowEvaluationAnswerOptions;

        public bool ShowGradeAnswerOptions =>
            this.sessionInformationProvider.GetAnswerOptionType(this.SlideSessionModel.QuestionType) == AnswerOptionType.ShowGradeAnswerOptions;

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

                this.OnPropertyChanged(nameof(this.AnswerOptions));
            }
        }

        public List<object> AnswerOptions
        {
            get { return this.SlideSessionModel.AnswerOptions; }
            set { this.SlideSessionModel.AnswerOptions = value; }
        }

        public string SelectAnswerOptionAmountText => this.LocalizationService.Translate("Answer option amount:");

        public string GridHeaderPosition => this.LocalizationService.Translate("Position");

        public string GridHeaderText => this.LocalizationService.Translate("Text");

        public string GridHeaderIsTrue => this.LocalizationService.Translate("Is true");

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

        private void InitAnswerOptionList()
        {
            if (this.AnswerOptions == null || this.AnswerOptions.Count != this.AnswerOptionAmount)
            {
                if (this.ShowGeneralAnswerOptions)
                {
                    this.SlideSessionModel.AnswerOptions = new List<object>();

                    for (int i = 1; i <= this.AnswerOptionAmount; i++)
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
                // TODO list init for other answer option types
            }
            
        }
    }
}
