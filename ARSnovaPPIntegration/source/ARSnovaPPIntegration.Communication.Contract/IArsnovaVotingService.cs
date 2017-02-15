using ARSnovaPPIntegration.Business.Model;
using ARSnovaPPIntegration.Communication.Model.ArsnovaEu;

namespace ARSnovaPPIntegration.Communication.Contract
{
    public interface IArsnovaVotingService
    {
        void CreateNewSession(SlideSessionModel slideSessionModel);

        void CreateOrUpdateQuestion(SlideSessionModel slideSessionModel, int questionIndex);

        void SetSessionAsActive(SlideSessionModel slideSessionModel);

        void StartQuestion(SlideSessionModel slideSessionModel, SlideQuestionModel slideQuestionModel);

        ArsnovaVotingResultReturn GetResults(SlideSessionModel slideSessionModel, SlideQuestionModel slideQuestionModel);
    }
}
