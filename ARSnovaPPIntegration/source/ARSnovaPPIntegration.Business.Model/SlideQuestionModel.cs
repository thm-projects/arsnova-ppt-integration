﻿using System;
using System.Collections.ObjectModel;

using ARSnovaPPIntegration.Common.Enum;

namespace ARSnovaPPIntegration.Business.Model
{
    public class SlideQuestionModel
    {
        // TODO: Concept: which default values should be taken?
        private ObservableCollection<GeneralAnswerOption> answerOptions;

        private string questionText;

        private QuestionTypeEnum questionType = QuestionTypeEnum.SingleChoiceClick;

        public SlideQuestionModel()
        {
            this.Id = Guid.NewGuid();
        }

        public event EventHandler ObjectChangedEventHandler;

        public int QuestionInfoSlideId { get; set; }

        public int? QuestionTimerSlideId { get; set; }

        public int? ResultsSlideId { get; set; }

        public string SlideNumbers => $"{this.QuestionInfoSlideId} - {this.QuestionInfoSlideId + 2}";

        public int Index { get; set; }

        public Guid Id { get; set; }

        public bool QuizInOneShape { get; set; } = false;

        public int Countdown { get; set; } = 20;

        public QuestionTypeEnum QuestionType
        {
            get
            {
                return this.questionType;
            }
            set
            {
                this.questionType = value;
                this.ObjectChangedEventHandler?.Invoke(this, EventArgs.Empty);
            }
        }

        public string QuestionTypeText { get; set; }

        public bool QuestionTypeSet { get; set; } = false;

        public string QuestionText
        {
            get
            {
                return this.questionText;
            }
            set
            {
                this.questionText = value;
                this.QuestionTypeSet = true;
                this.ObjectChangedEventHandler?.Invoke(this, EventArgs.Empty);
            }
        }

        public ObservableCollection<GeneralAnswerOption> AnswerOptions
        {
            get { return this.answerOptions; }
            set
            {
                this.answerOptions = value;
                this.AnswerOptionsSet = true;
            }
        }

        public bool AnswerOptionsSet { get; set; } = false;

        public AnswerOptionType AnswerOptionType { get; set; }

        public int AnswerOptionAmount { get; set; } = 4;

        public QuestionTypeEnum QuestionInitType { get; set; }

        public void AnswerOptionModelChanged()
        {
            this.ObjectChangedEventHandler?.Invoke(this, EventArgs.Empty);
        }
    }
}
