using System.Collections.Generic;

using ARSnovaPPIntegration.Business.Model;
using ARSnovaPPIntegration.Common.Enum;

namespace ARSnovaPPIntegration.Business.Contract
{
    public interface ISessionInformationProvider
    {
        List<string> GetHashtagList();

        List<QuestionType> GetAvailableQuestionsClick();

        List<QuestionType> GetAvailableQuestionsVoting();

        AnswerOptionType GetAnswerOptionType(QuestionTypeEnum questionType);
    }
}
