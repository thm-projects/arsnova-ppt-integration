using System;

using Microsoft.Office.Interop.PowerPoint;

using ARSnovaPPIntegration.Business.Model;

namespace ARSnovaPPIntegration.Business.Contract
{
    public interface ISessionManager
    {
        void CreateSession(SlideSessionModel slideSessionModel);

        void StartClickQuestion(SlideSessionModel slideSessionModel, int questionIndex, Slide questionSlide, Slide resultsSlide);

        void StartVotingQuestion(SlideSessionModel slideSessionModel, SlideQuestionModel slideQuestionModel);

        void GetAndDisplayArsnovaVotingResults(SlideSessionModel slideSessionModel,
            SlideQuestionModel slideQuestionModel, Slide resultsSlide);

        void RemoveClickQuizDataFromServer(SlideSessionModel slideSessionModel);

        void KeepAlive(SlideSessionModel slideSessionModel);

        void ActivateSession(SlideSessionModel slideSessionModel);

        void CreateOrUpdateArsnovaVotingQuestion(SlideSessionModel slideSessionModel, int questionIndex);

        event EventHandler ShowNextSlideEventHandler;
    }
}
