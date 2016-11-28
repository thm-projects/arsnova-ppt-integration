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
    }
}
