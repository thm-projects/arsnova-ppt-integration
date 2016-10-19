using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ARSnovaPPIntegration.Model.ArsnovaClick;

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
