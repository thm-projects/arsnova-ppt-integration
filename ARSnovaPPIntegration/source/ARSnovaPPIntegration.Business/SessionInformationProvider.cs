using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Practices.ServiceLocation;

using ARSnovaPPIntegration.Business.Contract;
using ARSnovaPPIntegration.Business.Model;
using ARSnovaPPIntegration.Common.Contract.Translators;
using ARSnovaPPIntegration.Common.Enum;

namespace ARSnovaPPIntegration.Business
{
    public class SessionInformationProvider : ISessionInformationProvider
    {
        private readonly IQuestionTypeTranslator questionTypeTranslator;

        private readonly List<QuestionTypeEnum> votingQuestionTypes = new List<QuestionTypeEnum>
        {
            QuestionTypeEnum.SingleChoiceVoting,
            QuestionTypeEnum.MultipleChoiceVoting,
            QuestionTypeEnum.YesNoVoting,
            QuestionTypeEnum.FreeTextVoting,
            QuestionTypeEnum.EvaluationVoting,
            QuestionTypeEnum.GradsVoting
        };

        private readonly List<QuestionTypeEnum> clickQuestionTypes = new List<QuestionTypeEnum>
        {
            QuestionTypeEnum.SingleChoiceClick,
            QuestionTypeEnum.MultipleChoiceClick,
            QuestionTypeEnum.YesNoClick,
            QuestionTypeEnum.TrueFalseClick,
            QuestionTypeEnum.RangedQuestionClick,
            QuestionTypeEnum.FreeTextClick,
            QuestionTypeEnum.SurveyClick
        };

        public SessionInformationProvider()
        {
            this.questionTypeTranslator = ServiceLocator.Current.GetInstance<IQuestionTypeTranslator>();
        }

        public List<QuestionType> GetAvailableQuestionsClick()
        {
            return this.clickQuestionTypes.Select(qte => new QuestionType
            {
                QuestionTypeEnum = qte,
                Name = this.questionTypeTranslator.TranslateQuestionType(qte)
            }).ToList();
        }

        public List<QuestionType> GetAvailableQuestionsVoting()
        {
            return this.votingQuestionTypes.Select(qte => new QuestionType
            {
                QuestionTypeEnum = qte,
                Name = this.questionTypeTranslator.TranslateQuestionType(qte)
            }).ToList();
        }

        public AnswerOptionType GetAnswerOptionType(QuestionTypeEnum questionType)
        {
            switch (questionType)
            {
                case QuestionTypeEnum.SingleChoiceVoting:
                case QuestionTypeEnum.MultipleChoiceVoting:
                case QuestionTypeEnum.SingleChoiceClick:
                case QuestionTypeEnum.MultipleChoiceClick:
                case QuestionTypeEnum.SurveyClick:
                    return AnswerOptionType.ShowGeneralAnswerOptions;
                case QuestionTypeEnum.FreeTextVoting:
                case QuestionTypeEnum.FreeTextClick:
                    return AnswerOptionType.ShowFreeTextAnswerOptions;
                case QuestionTypeEnum.EvaluationVoting:
                case QuestionTypeEnum.GradsVoting:
                    return AnswerOptionType.ShowGradeOrEvaluationAnswerOptions;
                case QuestionTypeEnum.RangedQuestionClick:
                    return AnswerOptionType.ShowRangedAnswerOption;
                case QuestionTypeEnum.YesNoVoting:
                case QuestionTypeEnum.YesNoClick:
                case QuestionTypeEnum.TrueFalseClick:
                    return AnswerOptionType.ShowTwoAnswerOptions;
                default:
                    throw new ArgumentException($"QuestionType not handled in GetAnswerOptionType: '{questionType}'");
            }
        }
    }
}
