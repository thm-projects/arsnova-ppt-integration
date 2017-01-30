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

        List<ExcelChartType> GetExcelChartTypes();

        AnswerOptionType GetAnswerOptionType(QuestionTypeEnum questionType);

        bool IsClickQuestion(QuestionTypeEnum questionType);

        bool IsSingleChoiceQuestion(QuestionTypeEnum questionType);

        bool IsMultipleChoiceQuestion(QuestionTypeEnum questionType);
    }
}
