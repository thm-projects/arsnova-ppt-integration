using System.Collections.Generic;
using ARSnovaPPIntegration.Model.ArsnovaClick;

namespace ARSnovaPPIntegration.Communication.Contract
{
    public interface IArsnovaClickService
    {
        List<HashtagInfo> GetAllHashtagInfos();

        List<AnswerOptionModel> GetAnswerOptionsForHashtag(string hashtag);

        SessionConfiguration GetSessionConfiguration(string hashtag);
    }
}
