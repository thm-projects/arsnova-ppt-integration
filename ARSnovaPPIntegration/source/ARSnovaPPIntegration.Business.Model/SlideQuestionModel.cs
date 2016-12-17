using System;
using System.Collections.ObjectModel;
using ARSnovaPPIntegration.Common.Contract.Translators;
using ARSnovaPPIntegration.Common.Enum;

namespace ARSnovaPPIntegration.Business.Model
{
    public class SlideQuestionModel
    {
        // TODO: Concept: which default values should be taken?
        private readonly IQuestionTypeTranslator questionTypeTranslator;

        private ObservableCollection<object> answerOptions;

        private string questionText;

        public SlideQuestionModel(IQuestionTypeTranslator questionTypeTranslator)
        {
            this.Id = new Guid();

            this.questionTypeTranslator = questionTypeTranslator;
        }

        // TODO test data
        public int SlideNumber { get; set; } = 0;

        // TODO select on which slide this question should be

        public Guid Id { get; }

        public QuestionTypeEnum QuestionType { get; set; } = QuestionTypeEnum.SingleChoiceClick;

        public string QuestionTypeText => this.questionTypeTranslator.TranslateQuestionType(this.QuestionType);

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
            }
        }

        public ObservableCollection<object> AnswerOptions
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

        public AnswerOptionType AnswerOptionInitType { get; set; }
    }
}
