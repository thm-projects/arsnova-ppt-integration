using System.Collections.Generic;
using ARSnovaPPIntegration.Business.Model;
using ARSnovaPPIntegration.Communication.Model.ArsnovaEu;

namespace ARSnovaPPIntegration.Communication.Contract
{
    public interface IArsnovaVotingService
    {
        void CreateNewSession(SlideSessionModel slideSessionModel);

        void CreateOrUpdateQuestion(SlideSessionModel slideSessionModel, int questionIndex);

        SessionModel GetSessionInformation(SlideSessionModel slideSessionModel);
    }
}
