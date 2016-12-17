using System.Collections.Generic;

using ARSnovaPPIntegration.Common.Enum;

namespace ARSnovaPPIntegration.Business.Model
{
    public class SlideSessionModel
    {
        public SlideSessionModel(bool edit = false)
        {
            this.NewSession = !edit;
        }

        public List<SlideQuestionModel> Questions { get; set; } = new List<SlideQuestionModel>();

        public string Hashtag { get; set; }

        public string PrivateKey { get; set; }

        public bool NewSession { get; set; }

        public SessionType SessionType { get; set; } = SessionType.ArsnovaClick;

        public bool SessionTypeSet { get; set; } = false;

        
    }
}
