using ARSnovaPPIntegration.Common.Enum;

namespace ARSnovaPPIntegration.Common.Contract.Translators
{
    public interface IQuestionTypeTranslator
    {
        string TranslateQuestionType(QuestionTypeEnum questionType);
    }
}
