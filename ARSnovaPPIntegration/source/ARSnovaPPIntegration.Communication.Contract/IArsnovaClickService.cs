﻿using System;
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

        string CreateHashtag(string hashtag);

        ValidationResult UpdateQuestionGroup(SlideSessionModel slideSessionModel);

        ValidationResult StartNextQuestion(SlideSessionModel slideSessionModel, int questionIndex);

        ValidationResult ShowNextReadingConfirmation(SlideSessionModel slideSessionModel);

        ValidationResult MakeSessionAvailable(string hashtag, string privateKey);

        ValidationResult RemoveQuizData(string hashtag, string privateKey);

        ValidationResult KeepAlive(string hashtag, string privateKey);

        List<ResultModel> GetResultsForHashtag(string hashtag, int questionIndex);

        bool IsThisMineHashtag(string hashtag, string privateKey);
    }
}
