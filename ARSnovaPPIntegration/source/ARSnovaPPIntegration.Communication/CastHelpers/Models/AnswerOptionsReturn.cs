using System.Collections.Generic;

using ARSnovaPPIntegration.Communication.Model.ArsnovaClick;

namespace ARSnovaPPIntegration.Communication.CastHelpers.Models
{
    public class AnswerOptionModelWithId : AnswerOptionModel
    {
        public string _id { get; set; }
    }

    public class AnswerOptionsReturn
    {
        public List<AnswerOptionModelWithId> answeroptions { get; set; }
    }
}
