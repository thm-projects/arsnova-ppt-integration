using System;
using System.Collections.Generic;

using ARSnovaPPIntegration.Business.Model;
using ARSnovaPPIntegration.Communication.Model.ArsnovaClick;

namespace ARSnovaPPIntegration.Communication.Contract
{
    public interface IArsnovaClickService
    {
        List<HashtagInfo> GetAllHashtagInfos();

        List<AnswerOptionModel> GetAnswerOptionsForHashtag(string hashtag);

        SessionConfiguration GetSessionConfiguration(string hashtag);

        Tuple<ValidationResult, string> CreateHashtag(string hashtag);

        ValidationResult UpdateQuestionGroup(SlideSessionModel slideSessionModel);

        ValidationResult ResetSession(SlideSessionModel slideSessionModel);

        ValidationResult StartNextQuestion(SlideSessionModel slideSessionModel, int questionIndex);

        ValidationResult ShowNextReadingConfirmation(SlideSessionModel slideSessionModel);
    }
}
