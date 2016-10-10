using System.Collections.Generic;

namespace ARSnovaPPIntegration.Communication.Contract
{
    public interface IArsnovaClickService
    {
        string FindAllHashtags();

        List<string> GetAnswerOptionsForHashtag(string hashtag);
    }
}
