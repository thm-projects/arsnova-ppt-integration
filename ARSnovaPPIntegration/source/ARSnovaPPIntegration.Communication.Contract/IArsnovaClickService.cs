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

        ValidationResult PostSession(SlideSessionModel slideSessionModel);

        ValidationResult UpdateSession(SlideSessionModel slideSessionModel);
    }
}
