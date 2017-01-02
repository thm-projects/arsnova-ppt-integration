using System.Collections.Generic;
using System.Collections.ObjectModel;
using ARSnovaPPIntegration.Common.Enum;

namespace ARSnovaPPIntegration.Business.Model
{
    public class SlideSessionModel
    {
        public SlideSessionModel(bool edit = false)
        {
            this.NewSession = !edit;
        }

        public ObservableCollection<SlideQuestionModel> Questions { get; set; } = new ObservableCollection<SlideQuestionModel>();

        public string Hashtag { get; set; }

        public string PrivateKey { get; set; }

        public bool NewSession { get; set; }

        public SessionType SessionType { get; set; } = SessionType.ArsnovaClick;

        public bool SessionTypeSet { get; set; } = false;

        
    }
}
