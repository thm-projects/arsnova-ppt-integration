using System.Collections.Generic;

using ARSnovaPPIntegration.Communication.Model.ArsnovaClick;

namespace ARSnovaPPIntegration.Communication.CastHelpers.Models
{
    public class ResultModelWithId : ResultModel
    {
        public string _id { get; set; }
    }

    public class ResultModelsReturn
    {
        public List<ResultModelWithId> responses { get; set; }
    }
}
