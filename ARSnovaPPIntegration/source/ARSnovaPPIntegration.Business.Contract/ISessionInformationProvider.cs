using System.Collections.Generic;

using ARSnovaPPIntegration.Business.Model;

namespace ARSnovaPPIntegration.Business.Contract
{
    public interface ISessionInformationProvider
    {
        List<QuestionType> GetAvailableQuestionsClick();

        List<QuestionType> GetAvailableQuestionsVoting();
    }
}
