using System.Collections.Generic;
using ARSnovaPPIntegration.Model.ArsnovaClick;

namespace ARSnovaPPIntegration.Communication.Contract
{
    public interface IArsnovaClickService
    {
        string FindAllHashtags();

        List<AnswerOptionModel> GetAnswerOptionsForHashtag(string hashtag);
    }
}
