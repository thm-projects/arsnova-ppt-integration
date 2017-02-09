using System.Collections.Generic;
using ARSnovaPPIntegration.Business.Model;
using ARSnovaPPIntegration.Communication.Model.ArsnovaEu;

namespace ARSnovaPPIntegration.Communication.Contract
{
    public interface IArsnovaVotingService
    {
        SessionModel CreateNewSession(SlideSessionModel slideSessionModel);

        SessionModel GetSessionInformation(SlideSessionModel slideSessionModel);

        List<LectureQuestionModel> GetLectureQuestionInfos(SlideSessionModel slideSessionModel);

        LectureQuestionModel GetLectureQuestion(SlideSessionModel slideSessionModel, string questionId);
    }
}
